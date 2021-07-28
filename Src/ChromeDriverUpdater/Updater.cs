using ChromeDriverUpdater.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChromeDriverUpdater
{
    public class Updater
    {
        public ExitCode Update(string chromeDriverPath, bool shutdownExistProcess = false)
        {
            try
            {
                if(shutdownExistProcess)
                {
                    ShutdownChromeDriver(chromeDriverPath);
                }


            } 
            catch(Exception exc)
            {

            }

            return ExitCode.Success;
        }

        private Version GetChromeDriverVersion(string chromeDriverPath)
        {
            string output = new ProcessExecuter().Run("CMD.exe", $"/C \"{chromeDriverPath}\" -version");

            string versionStr = output.Split(' ')[1];

            Version version = new Version(versionStr);

            return version;
        }

        private Version GetChromeVersionFromRegistry()
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey("Software\\Google\\Chrome\\BLBeacon"))
                {
                    if (key != null)
                    {
                        Object o = key.GetValue("version");
                        if (o != null)
                        {
                            Version version = new Version(o as String);

                            return version;
                        }
                    }
                }
            }
            catch
            {
                throw new UpdateFailException("Chrome Is Not Installed.", ExitCode.ChromeNotInstalled);
            }

            return null;
        }

        private void ShutdownChromeDriver(string chromeDriverPath)
        {
            var processes = Process.GetProcesses();

            foreach(Process process in processes)
            {
                if(process.MainModule.FileName == chromeDriverPath)
                {
                    process.Kill();
                }
            }

            //new ProcessExecuter().Run("CMD.exe", "/C taskkill /F /IM chromedriver.exe /T");
        }
    }
}
