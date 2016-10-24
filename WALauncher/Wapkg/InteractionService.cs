using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using WALauncher.Utils;

namespace WALauncher.Wapkg
{
    public class InteractionService
    {
        private static InteractionService service = null;

        public static InteractionService Get()
        {
            if(service == null)
            {
                service = new InteractionService();
            }

            return service;
        }

        private Process process;
        private UdpClient udp;

        private ushort lsnPort = 51123;
        private bool running = true;
        private bool ready = false;

        private Queue<string> delayedPackets = new Queue<string>(); // only required on daemon initialization stage

        public event EventHandler<ServiceMessageEventArgs> PackagesChanged;
        public event EventHandler<ServiceMessageEventArgs> DistributionsChanged;
        public event EventHandler<ServiceMessageEventArgs> AvailablePackagesAccepted;
        public event EventHandler<ServiceMessageEventArgs> AvailableDistributionsAccepted;

        public event EventHandler<ServiceMessageEventArgs> TextAccepted;
        public event EventHandler<ServiceMessageEventArgs> IndexChanged;
        public event EventHandler<ServiceMessageEventArgs> ActionProgressUpdated;
        public event EventHandler<ServiceMessageEventArgs> ActionComplete;
        public event EventHandler<ServiceMessageEventArgs> SourcesChanged;

        public string WorkingDirectory { get; private set; }

        public InteractionService()
        {
            process = CreateServiceProcess();
            process.OutputDataReceived += OnProcessOutputDataReceived;
            process.Start();
            process.BeginOutputReadLine();
        }

        public Process ServiceProcess
        {
            get { return process; }
        }

        private void OnProcessOutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if(e.Data == "ready")
            {
                process.OutputDataReceived -= OnProcessOutputDataReceived;

                udp = new UdpClient(lsnPort);
                udp.Connect(IPAddress.Loopback, 16723);

                var task = new Task(ServiceThread);
                task.ContinueWith(ExceptionHandling.TaskCrash);
                task.Start();

                ready = true;

                while(delayedPackets.Count > 0)
                {
                    Send(delayedPackets.Dequeue());
                }
            }
        }

        private Process CreateServiceProcess()
        {
            var i = new ProcessStartInfo();

            i.CreateNoWindow = true;
            i.UseShellExecute = false;
            i.RedirectStandardOutput = true;
            i.FileName = "wqdaemon.exe";

            var proc = new Process();
            proc.StartInfo = i;

            return proc;
        }

        private void ServiceThread()
        {
            var endpoint = new IPEndPoint(IPAddress.Any, 0);
            var checklist = new List<Socket>();
            udp.Client.Blocking = false;

            while (running)
            {
                checklist.Add(udp.Client);
                Socket.Select(checklist, null, null, 1000000); // 1 sec
                if (checklist.Count == 0) continue;

                var packet = Encoding.UTF8.GetString(udp.Receive(ref endpoint));
                if (!packet.StartsWith("quack!")) continue;
                var lines = new List<string>();
                foreach(var x in packet.Split('\n'))
                {
                    if (x.Trim().Length > 0)
                    {
                        lines.Add(x);
                    }
                }

                var cmd = lines[0].Split('!')[1];

                if (cmd == "packages-changed")
                {
                    var dist = lines[1].Split('/')[1];
                    var pkgs = new List<Tuple<string, uint?>>();
                    
                    for (int i = 2; i < lines.Count; i++)
                    {
                        var s = lines[i].Split(':');
                        pkgs.Add(new Tuple<string, uint?>(s[0], Convert.ToUInt32(s[1])));
                    }

                    PackagesChanged?.Invoke(this, new ServiceMessageEventArgs(packet, dist, pkgs));
                }

                else if (cmd == "packages-available")
                {
                    var pkgs = new List<Tuple<string, uint?>>();
                    for (int i = 1; i < lines.Count; i++)
                    {
                        var s = lines[i].Split(':');
                        uint? rev = null;
                        if(s[1] != "virtual")
                        {
                            rev = Convert.ToUInt32(s[1]);
                        }

                        pkgs.Add(new Tuple<string, uint?>(s[0], rev));
                    }

                    AvailablePackagesAccepted?.Invoke(this, new ServiceMessageEventArgs(packet, null, pkgs));
                }

                else if (cmd.StartsWith("dists-"))
                {
                    var dists = new List<string>();
                    for (int i = 1; i < lines.Count; i++)
                    {
                        dists.Add(lines[i]);
                    }

                    var e = new ServiceMessageEventArgs(packet, dists);
                    if (cmd == "dists-changed") DistributionsChanged?.Invoke(this, e);
                    else AvailableDistributionsAccepted?.Invoke(this, e);
                }

                else if (cmd == "text")
                {
                    string text = "";
                    for (int i = 1; i < lines.Count; i++)
                    {
                        text += lines[i] + "\n";
                    }

                    if (text.Length > 0)
                    {
                        text = text.Remove(text.Length - 1);
                        TextAccepted?.Invoke(this, new ServiceMessageEventArgs(packet, text));
                    }
                }

                else if (cmd == "wd")
                {
                    WorkingDirectory = lines[1];
                }

                else if (cmd == "index-changed")
                {
                    IndexChanged?.Invoke(this, new ServiceMessageEventArgs(packet));
                }

                else if (cmd == "action-update")
                {
                    ActionProgressUpdated?.Invoke(this, new ServiceMessageEventArgs(packet, lines[1], Convert.ToInt32(lines[2]), Convert.ToInt32(lines[3])));
                }

                else if (cmd == "action-complete")
                {
                    ActionComplete?.Invoke(this, new ServiceMessageEventArgs(packet, lines[1], 0, 0));
                }

                else if (cmd == "sources-changed")
                {
                    List<string> sources = new List<string>();
                    for (int x = 1; x < lines.Count; x++)
                    {
                        sources.Add(lines[x]);
                    }

                    SourcesChanged?.Invoke(this, new ServiceMessageEventArgs(packet, null, sources));
                }
            }

            udp.Close();
        }

        void Send(string msg)
        {
            if (ready)
            {
                var bytes = Encoding.UTF8.GetBytes(msg);
                udp.Send(bytes, bytes.Length);
            }
            else
            {
                delayedPackets.Enqueue(msg);
            }
        }

        void SendWqRequest(params string[] args)
        {
            string msg = "wq/0.1";
            foreach(var a in args)
            {
                if (a == null) continue;
                msg += ";" + a;
            }

            msg += "\n";
            Send(msg);
        }

        public void Subscribe()
        {
            SendWqRequest("subscribe", "127.0.0.1", lsnPort.ToString());
        }

        public void InstallPackage(string distro, string package)
        {
            SendWqRequest("install", distro, package);
        }

        public void RemovePackage(string distro, string package)
        {
            SendWqRequest("remove", distro, package);
        }

        public void InstallDistribution(string distro, string name = null, string actionToken = null)
        {
            if (name == null)
                SendWqRequest("dist-install", distro);
            else
                SendWqRequest("dist-install", distro, name, actionToken);
        }

        public void RequestPackages(string distro)
        {
            SendWqRequest("packages", distro);
        }

        public void RequestAvailablePackages()
        {
            SendWqRequest("packages-available");
        }

        public void RequestDistributions()
        {
            SendWqRequest("dists");
        }

        public void RequestAvailableDistributions()
        {
            SendWqRequest("dists-available");
        }

        public void RequestUpdate()
        {
            SendWqRequest("update-index");
        }

        public void RequestWorkingDirectory()
        {
            SendWqRequest("wd");
        }

        public void RequestSources()
        {
            SendWqRequest("sources");
        }

        public void PushSources(IEnumerable sources)
        {
            var args = new List<string>() { "push-sources" };
            foreach(var x in sources)
            {
                args.Add(x.ToString());
            }

            SendWqRequest(args.ToArray());
        }

        public void Shutdown()
        {
            running = false;
            ready = false;

            if (!process.HasExited)
            {
                process.Kill();
            }

            service = null;
        }
    }
}
