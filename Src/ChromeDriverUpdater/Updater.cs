using ChromeDriverUpdater.Models;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;

namespace ChromeDriverUpdater
{
    public class Updater
    {
        private const string CHROME_DRIVER_BASE_URL = "https://chromedriver.storage.googleapis.com";

        /// <exception cref="UpdateFailException"></exception>
        public void Update(string chromeDriverPath)
        {
            Version chromeDriverVersion = GetChromeDriverVersion(chromeDriverPath);
            Version chromeVersion = GetChromeVersionFromRegistry();

            if (!CompareVersionMajorToBuild(chromeVersion, chromeDriverVersion))
            {
                ShutdownChromeDriver(chromeDriverPath);

                UpdateChromeDriver(chromeDriverPath, chromeVersion);
            }
        }

        private bool CompareVersionMajorToBuild(Version v1, Version v2)
        {
            return (v1.Major == v2.Major &&
                    v1.Minor == v2.Minor &&
                    v1.Build == v2.Build);
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
                        Object versionObject = key.GetValue("version");

                        if (versionObject != null)
                        {
                            Version version = new Version(versionObject as String);

                            return version;
                        }
                    }

                    throw new Exception();
                }
            }
            catch
            {
                throw new UpdateFailException("Chrome Is Not Installed.", ErrorCode.ChromeNotInstalled);
            }
        }

        private void ShutdownChromeDriver(string chromeDriverPath)
        {
            var processes = Process.GetProcesses();

            foreach (Process process in processes)
            {
                try
                {
                    if (process.MainModule.FileName == chromeDriverPath)
                    {
                        process.Kill();
                    }
                }
                catch
                {

                }
            }
        }

        private void UpdateChromeDriver(string chromeDriverPath, Version chromeDriverVersion)
        {
            string version = GetProperChromeDriverVersion(chromeDriverVersion);

            string driverPath = DownloadChromeDriver(version);

            File.Copy(driverPath, chromeDriverPath, true);
        }

        private string GetProperChromeDriverVersion(Version chromeVersion)
        {
            string url = $"{CHROME_DRIVER_BASE_URL}/LATEST_RELEASE_{chromeVersion.Major}.{chromeVersion.Minor}.{chromeVersion.Build}";

            try
            {
                WebClient client = new WebClient();

                string result = client.DownloadString(url);

                return result;
            }
            catch
            {
                throw new UpdateFailException("Cannot get proper chromedriver version", ErrorCode.Fail);
            }
        }

        private string DownloadChromeDriver(string version)
        {
            string fileName = "chromedriver_win32.zip";
            string url = $"{CHROME_DRIVER_BASE_URL}/{version}/{fileName}";

            string downloadPath = Path.Combine(Path.GetTempPath(), fileName);
            string unzipPath = Path.Combine(Path.GetTempPath(), "chromedriver_win32");

            try
            {
                new WebClient().DownloadFile(url, downloadPath);
            }
            catch
            {
                throw new UpdateFailException("Cannot download file", ErrorCode.CannotDownloadNewChromeDriver);
            }

            UnzipFile(downloadPath, unzipPath);

            // delete zip file
            File.Delete(downloadPath);

            DirectoryInfo directoryInfo = new DirectoryInfo(unzipPath);

            var files = directoryInfo.GetFiles();

            if (files.Length == 1)
            {
                return files[0].FullName;
            }

            foreach (FileInfo file in files)
            {
                if (file.Name == "chromedriver.exe")
                {
                    return file.FullName;
                }
            }

            throw new UpdateFailException("Cannot Get New ChromeDriver From unzip Path", ErrorCode.CannotDownloadNewChromeDriver);
        }

        private void UnzipFile(string zipPath, string unzipPath)
        {
            try
            {
                if (Directory.Exists(unzipPath))
                {
                    Directory.Delete(unzipPath, true);
                }

                ZipFile.ExtractToDirectory(zipPath, unzipPath);
            }
            catch
            {
                throw new UpdateFailException("Cannot unzip file", ErrorCode.Fail);
            }
        }

    }
}
