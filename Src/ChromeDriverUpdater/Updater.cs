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
        private const string DOWNLOAD_ZIP_FILE_NAME = "chromedriver_win32.zip";
        private const string CHROME_DRIVER_BASE_NAME = "chromedriver.exe";

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

        private void UpdateChromeDriver(string existChromeDriverFullPath, Version chromeVersion)
        {
            string zipFileDownloadPath = DownloadChromeDriverZipFile(chromeVersion);

            string newChromeDriverFullPath = GetNewChromeDriverFromZipFile(zipFileDownloadPath);

            File.Copy(newChromeDriverFullPath, existChromeDriverFullPath, true);

            File.Delete(newChromeDriverFullPath);
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

        private string DownloadChromeDriverZipFile(Version chromeVersion)
        {
            string version = GetProperChromeDriverVersion(chromeVersion);

            string downloadUrl = $"{CHROME_DRIVER_BASE_URL}/{version}/{DOWNLOAD_ZIP_FILE_NAME}";
            string downloadZipFileFullPath = Path.Combine(Path.GetTempPath(), DOWNLOAD_ZIP_FILE_NAME);

            DownloadFile(downloadUrl, downloadZipFileFullPath);

            return downloadZipFileFullPath;
        }

        private string GetNewChromeDriverFromZipFile(string zipFileDownloadPath)
        {
            string unzipPath = Path.ChangeExtension(zipFileDownloadPath, string.Empty);

            UnzipFile(zipFileDownloadPath, unzipPath, true);

            return FindNewChromeDriverFullPathFromUnzipPath(unzipPath);
        }

        private string FindNewChromeDriverFullPathFromUnzipPath(string chromeDriverUnzipPath)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(chromeDriverUnzipPath);

            FileInfo[] files = directoryInfo.GetFiles();

            foreach (FileInfo file in files)
            {
                // ignore case
                if (file.Name.ToLower() == CHROME_DRIVER_BASE_NAME.ToLower())
                {
                    string newPath = Path.GetDirectoryName(chromeDriverUnzipPath) + file.Name;
                    
                    File.Copy(file.Name, newPath, true);

                    Directory.Delete(chromeDriverUnzipPath, true);

                    return newPath;
                }
            }

            throw new UpdateFailException("Cannot Get New ChromeDriver From unzip Path", ErrorCode.CannotDownloadNewChromeDriver);
        }

        private void DownloadFile(string downloadUrl, string downloadPath)
        {
            try
            {
                new WebClient().DownloadFile(downloadUrl, downloadPath);
            }
            catch
            {
                throw new UpdateFailException("Cannot download file", ErrorCode.CannotDownloadNewChromeDriver);
            }
        }

        private void UnzipFile(string zipPath, string unzipPath, bool deleteZipFile = true)
        {
            try
            {
                if (Directory.Exists(unzipPath))
                {
                    Directory.Delete(unzipPath, true);
                }

                ZipFile.ExtractToDirectory(zipPath, unzipPath);

                if(deleteZipFile)
                {
                    File.Delete(zipPath);
                }
            }
            catch
            {
                throw new UpdateFailException("Cannot unzip file", ErrorCode.Fail);
            }
        }

    }
}
