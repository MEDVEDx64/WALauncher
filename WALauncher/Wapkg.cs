using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace WALauncher
{
    public class Wapkg
    {
        Process CreateProcess(string args, string exe = "wapt.exe")
        {
            var i = new ProcessStartInfo();

            i.CreateNoWindow = true;
            i.RedirectStandardOutput = true;
            i.UseShellExecute = false;
            i.FileName = exe;
            i.Arguments = args;

            var proc = new Process();
            proc.StartInfo = i;

            return proc;
        }

        public void Init()
        {
            var pr = CreateProcess("init");
            pr.Start();
            pr.WaitForExit();
        }

        private IReadOnlyList<string> ReadProcessLines(Process p)
        {
            List<string> lines = new List<string>();

            p.Start();
            var st = p.StandardOutput;
            while (true)
            {
                var line = st.ReadLine();
                if (line == null) break;
                if (line.Trim().Length == 0) continue;
                lines.Add(line);
            }

            return lines;
        }

        public IReadOnlyList<string> GetDists()
        {
            return ReadProcessLines(CreateProcess("dists"));
        }

        private IReadOnlyList<Tuple<string, uint>> ParsePackagesList(IEnumerable<string> lines)
        {
            var list = new List<Tuple<string, uint>>();
            foreach (var line in lines)
            {
                if (line.StartsWith("Distribution '") && line.EndsWith("' is not installed"))
                {
                    list.Clear();
                    return list;
                }

                if (!line.Contains(", revision ")) continue;
                var idx = line.LastIndexOf(',');
                var name = line.Substring(0, idx);
                var splitted = line.Split(' ');

                uint revision = 0;
                try
                {
                    revision = Convert.ToUInt32(splitted[splitted.Length - 1]);
                }
                catch(Exception)
                {
                    continue;
                }

                list.Add(new Tuple<string, uint>(name, revision));
            }

            return list;
        }

        public IReadOnlyList<Tuple<string, uint>> GetPackages(string distro)
        {
            return ParsePackagesList(ReadProcessLines(CreateProcess("packages " + distro)));
        }

        public IReadOnlyList<Tuple<string, uint>> GetPackagesAvailable()
        {
            return ParsePackagesList(ReadProcessLines(CreateProcess("packages-available")));
        }
    }
}
