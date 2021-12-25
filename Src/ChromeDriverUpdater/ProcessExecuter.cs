using System.Diagnostics;

namespace ChromeDriverUpdater
{
    public class ProcessExecuter
    {
        public string Run(string fileName, string arguments)
        {
            Process process = new Process();

            process.StartInfo = GetProcessStartInfoForHiddenWindow(fileName, arguments);
            process.Start();

            string output = process.StandardOutput.ReadToEnd();

            process.WaitForExit();

            return output;
        }

        private ProcessStartInfo GetProcessStartInfoForHiddenWindow(string fileName, string arguments)
        {
            ProcessStartInfo processStartInfo = new ProcessStartInfo();
            
            processStartInfo.UseShellExecute = false;
            processStartInfo.RedirectStandardOutput = true;
            processStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            processStartInfo.CreateNoWindow = true;

            processStartInfo.FileName = fileName;
            processStartInfo.Arguments = arguments;

            return processStartInfo;
        }
    }
}
