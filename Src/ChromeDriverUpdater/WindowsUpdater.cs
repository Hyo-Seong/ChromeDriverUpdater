using ChromeDriverUpdater.Models;
using Microsoft.Win32;
using System;

namespace ChromeDriverUpdater
{
    internal class WindowsUpdater : IUpdater
    {
        public string ChromeDriverName => "chromedriver.exe";
        public string ChromeDriverZipFileName => "chromedriver_win32.zip";
        
        public Version GetChromeDriverVersion(string chromeDriverPath)
        {
            string output = new ProcessExecuter().Run("CMD.exe", $"/C \"{chromeDriverPath}\" -version");

            // output like this
            // ChromeDriver 88.0.4324.96 (68dba2d8a0b149a1d3afac56fa74648032bcf46b-refs/branch-heads/4324@{#1784})
            string versionStr = output.Split(' ')[1];
            Version version = new Version(versionStr);

            return version;
        }

        public Version GetChromeVersion()
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey("Software\\Google\\Chrome\\BLBeacon"))
                {
                    if (key != null)
                    {
                        object versionObject = key.GetValue("version");

                        if (versionObject != null)
                        {
                            Version version = new Version(versionObject as String);

                            return version;
                        }
                    }
                }
            }
            catch { }

            throw new UpdateFailException(ErrorCode.ChromeNotInstalled);
        }
    }
}
