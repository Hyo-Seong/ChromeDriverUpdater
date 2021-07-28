using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChromeDriverUpdater
{
    public class ProcessExecuter
    {
        public string Run(string path, string argument)
        {
            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.FileName = path;
            startInfo.Arguments = argument;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.StartInfo = startInfo;
            process.Start();

            string output = process.StandardOutput.ReadToEnd();

            process.WaitForExit();

            return output;
        }
    }
}
