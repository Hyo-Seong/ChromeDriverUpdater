using ChromeDriverUpdater.Models;
using Microsoft.Win32;
using System;

namespace ChromeDriverUpdater
{
    internal class WindowsUpdater : IUpdater
    {
        public string ChromeDriverName => "chromedriver.exe";
        public string ChromeDriverZipFileName => "chromedriver_win32.zip";
        
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
