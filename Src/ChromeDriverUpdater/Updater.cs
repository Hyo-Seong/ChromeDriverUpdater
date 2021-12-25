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
        private const string FILE_NAME = "chromedriver_win32.zip";

        /// <exception cref="UpdateFailException"></exception>
        public void Update(string chromeDriverFullPath)
        {
            // change to full path to shutdown chromedriver
            chromeDriverFullPath = Path.GetFullPath(chromeDriverFullPath);

            if(!File.Exists(chromeDriverFullPath))
            {
                throw new UpdateFailException($"Cannot found chromeDriver. Path: {chromeDriverFullPath}", ErrorCode.ChromeDriverNotFound);
            }

            Version chromeDriverVersion = GetChromeDriverVersion(chromeDriverFullPath);
            Version chromeVersion = GetChromeVersionFromRegistry();

            if (!CompareVersionMajorToBuild(chromeVersion, chromeDriverVersion))
            {
                ShutdownChromeDriver(chromeDriverFullPath);

                UpdateChromeDriver(chromeDriverFullPath, chromeVersion);
            }
        }

        private bool CompareVersionMajorToBuild(Version v1, Version v2)
        {
            return (v1.Major == v2.Major &&
                    v1.Minor == v2.Minor &&
                    v1.Build == v2.Build);
        }

        private Version GetChromeDriverVersion(string chromeDriverFullPath)
        {
            string output = new ProcessExecuter().Run("CMD.exe", $"/C \"{chromeDriverFullPath}\" -version");

            // output like this
            // ChromeDriver 88.0.4324.96 (68dba2d8a0b149a1d3afac56fa74648032bcf46b-refs/branch-heads/4324@{#1784})
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
                }
            }
            catch { }
            
            throw new UpdateFailException("Chrome Is Not Installed.", ErrorCode.ChromeNotInstalled);
        }

        private void ShutdownChromeDriver(string chromeDriverFullPath)
        {
            var processes = Process.GetProcesses();

            foreach (Process process in processes)
            {
                try
                {
                    // find exact path
                    if (process.MainModule.FileName == chromeDriverFullPath)
                    {
                        process.Kill();
                    }
                }
                catch
                {

                }
            }
        }

        private void UpdateChromeDriver(string existChromeDriverFullPath, Version chromeDriverVersion)
        {
            string version = GetProperChromeDriverVersion(chromeDriverVersion);

            string zipFileDownloadPath = DownloadChromeDriverZipFile(version);

            string newChromeDriverFullPath = GetNewChromeDriverFromZipFile(zipFileDownloadPath);

            File.Copy(newChromeDriverFullPath, existChromeDriverFullPath, true);
        }

        private string GetProperChromeDriverVersion(Version chromeVersion)
        {
            try
            {
                string url = $"{CHROME_DRIVER_BASE_URL}/LATEST_RELEASE_{chromeVersion.Major}.{chromeVersion.Minor}.{chromeVersion.Build}";

                WebClient client = new WebClient();

                return client.DownloadString(url);
            }
            catch
            {
                throw new UpdateFailException("Cannot get proper chromedriver version", ErrorCode.Fail);
            }
        }

        private string DownloadChromeDriverZipFile(string version)
        {
            string url = $"{CHROME_DRIVER_BASE_URL}/{version}/{FILE_NAME}";
            string downloadPath = Path.Combine(Path.GetTempPath(), FILE_NAME);

            DownloadFile(url, downloadPath);

            return downloadPath;
        }

        private string GetNewChromeDriverFromZipFile(string zipFileDownloadPath)
        {
            string unzipPath = Path.Combine(Path.GetTempPath(), "chromedriver_win32");

            UnzipFile(zipFileDownloadPath, unzipPath);

            File.Delete(zipFileDownloadPath);

            DirectoryInfo directoryInfo = new DirectoryInfo(unzipPath);

            FileInfo[] files = directoryInfo.GetFiles();

            foreach (FileInfo file in files)
            {
                if (file.Name == "chromedriver.exe")
                {
                    return file.FullName;
                }
            }

            throw new UpdateFailException("Cannot Get New ChromeDriver From unzip Path", ErrorCode.CannotDownloadNewChromeDriver);
        }

        private void DownloadFile(string url, string downloadPath)
        {
            try
            {
                new WebClient().DownloadFile(url, downloadPath);
            }
            catch
            {
                throw new UpdateFailException("Cannot download file", ErrorCode.CannotDownloadNewChromeDriver);
            }
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
