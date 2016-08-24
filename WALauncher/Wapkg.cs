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

        IReadOnlyList<string> ReadProcessLines(Process p)
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
    }
}
