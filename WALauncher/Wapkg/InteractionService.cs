using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

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

        public event EventHandler<ServiceMessageEventArgs> PackagesChanged;
        public event EventHandler<ServiceMessageEventArgs> DistributionsChanged;
        public event EventHandler<ServiceMessageEventArgs> AvailablePackagesAccepted;
        public event EventHandler<ServiceMessageEventArgs> AvailableDistributionsAccepted;

        public event EventHandler<ServiceMessageEventArgs> TextAccepted;
        public event EventHandler<ServiceMessageEventArgs> IndexChanged;

        public string WorkingDirectory { get; private set; }

        public InteractionService()
        {
            process = CreateServiceProcess();
            process.Start();

            udp = new UdpClient(lsnPort);
            udp.Connect(IPAddress.Loopback, 16723);
            new Thread(ServiceThread).Start();
        }

        private Process CreateServiceProcess()
        {
            var i = new ProcessStartInfo();

            i.CreateNoWindow = true;
            i.UseShellExecute = false;
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
                var lines = packet.Split('\n');
                var cmd = lines[0].Split('!')[1];

                if (cmd == "packages-changed")
                {
                    var dist = lines[1].Split('/')[1];
                    var pkgs = new List<Tuple<string, uint?>>();
                    
                    for (int i = 2; i < lines.Length; i++)
                    {
                        if (lines[i].Trim().Length == 0) continue;
                        var s = lines[i].Split(':');
                        pkgs.Add(new Tuple<string, uint?>(s[0], Convert.ToUInt32(s[1])));
                    }

                    PackagesChanged?.Invoke(this, new ServiceMessageEventArgs(packet, dist, pkgs));
                }

                else if (cmd == "packages-available")
                {
                    var pkgs = new List<Tuple<string, uint?>>();
                    for (int i = 1; i < lines.Length; i++)
                    {
                        if (lines[i].Trim().Length == 0) continue;
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
                    for (int i = 1; i < lines.Length; i++)
                    {
                        if (lines[i].Trim().Length == 0) continue;
                        dists.Add(lines[i]);
                    }

                    var e = new ServiceMessageEventArgs(packet, dists);
                    if (cmd == "dists-changed") DistributionsChanged?.Invoke(this, e);
                    else AvailableDistributionsAccepted?.Invoke(this, e);
                }

                else if (cmd == "text")
                {
                    string text = "";
                    for (int i = 1; i < lines.Length; i++)
                    {
                        if (lines[i].Trim().Length == 0) continue;
                        text += lines[i] + "\n";
                    }

                    TextAccepted?.Invoke(this, new ServiceMessageEventArgs(packet, text));
                }

                else if (cmd == "wd")
                {
                    WorkingDirectory = lines[1];
                }

                else if (cmd == "index-changed")
                {
                    IndexChanged?.Invoke(this, new ServiceMessageEventArgs(packet));
                }
            }

            udp.Close();
        }

        void Send(string msg)
        {
            var bytes = Encoding.UTF8.GetBytes(msg);
            udp.Send(bytes, bytes.Length);
        }

        void SendWqRequest(params string[] args)
        {
            string msg = "wq/0.1";
            foreach(var a in args)
            {
                msg += ":" + a;
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

        public void InstallDistribution(string distro, string name = null)
        {
            if (name == null)
                SendWqRequest("dist-install", distro);
            else
                SendWqRequest("dist-install", distro, name);
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

        public void Shutdown()
        {
            running = false;
            process.Kill();
            service = null;
        }
    }
}
