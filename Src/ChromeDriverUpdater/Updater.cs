using ChromeDriverUpdater.Models;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Runtime.InteropServices;

namespace ChromeDriverUpdater
{
    public class Updater
    {
        internal const string CHROME_DRIVER_BASE_URL = "https://chromedriver.storage.googleapis.com";

        private IUpdater update;

        /// <summary>
        /// Update the chromedriver
        /// <br>1. Check Chrome Version from Registry</br>
        /// <br>2. Shutdown chromedriver (exact full path)</br>
        /// <br>3. Download proper chromedriver from web</br>
        /// <br>4. Replace to downloaded new chromedriver.exe</br>
        /// </summary>
        /// <exception cref="UpdateFailException"></exception>
        public void Update(string chromeDriverFullPath)
        {
            // change to full path to shutdown chromedriver
            chromeDriverFullPath = Path.GetFullPath(chromeDriverFullPath);

            if (!File.Exists(chromeDriverFullPath))
            {
                throw new UpdateFailException(ErrorCode.ChromeDriverNotFound);
            }

            update = GetUpdate();

            Version chromeDriverVersion = update.GetChromeDriverVersion(chromeDriverFullPath);
            Version chromeVersion = update.GetChromeVersion();

            if (UpdateNecessary(chromeDriverVersion, chromeVersion))
            {
                ShutdownChromeDriver(chromeDriverFullPath);

                UpdateChromeDriver(chromeDriverFullPath, chromeVersion);
            }
        }

        internal IUpdater GetUpdate()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return new WindowsUpdater();
            }
            else if(RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {

            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {

            }

            return null;
        }

        internal bool UpdateNecessary(Version chromeVersion, Version chromeDriverVersion)
        {
            return !CompareVersionMajorToBuild(chromeVersion, chromeDriverVersion);
        }

        internal bool CompareVersionMajorToBuild(Version v1, Version v2)
        {
            return v1.Major == v2.Major &&
                   v1.Minor == v2.Minor &&
                   v1.Build == v2.Build;
        }

        internal void ShutdownChromeDriver(string chromeDriverFullPath)
        {
            var processes = Process.GetProcesses();

            foreach (Process process in processes)
            {
                // find exact path
                if (process.MainWindowTitle == chromeDriverFullPath)
                {
                    try
                    {
                        process.Kill();
                    } 
                    catch 
                    { 
                        throw new UpdateFailException(ErrorCode.CannotShutdownloadChromeDriver); 
                    }
                }
            }
        }

        internal void UpdateChromeDriver(string existChromeDriverFullPath, Version chromeVersion)
        {
            string zipFileDownloadPath = DownloadChromeDriverZipFile(chromeVersion);

            string newChromeDriverFullPath = GetNewChromeDriverFromZipFile(zipFileDownloadPath);

            File.Copy(newChromeDriverFullPath, existChromeDriverFullPath, true);

            File.Delete(newChromeDriverFullPath);
        }

        internal string GetProperChromeDriverVersion(Version chromeVersion)
        {
            try
            {
                string url = $"{CHROME_DRIVER_BASE_URL}/LATEST_RELEASE_{chromeVersion.Major}.{chromeVersion.Minor}.{chromeVersion.Build}";

                WebClient client = new WebClient();

                return client.DownloadString(url);
            }
            catch
            {
                throw new UpdateFailException(ErrorCode.CannotGetLatestRelease);
            }
        }

        internal string DownloadChromeDriverZipFile(Version chromeVersion)
        {
            string version = GetProperChromeDriverVersion(chromeVersion);

            string downloadUrl = $"{CHROME_DRIVER_BASE_URL}/{version}/{update.ChromeDriverZipFileName}";
            string downloadZipFileFullPath = Path.Combine(Path.GetTempPath(), update.ChromeDriverZipFileName);

            DownloadFile(downloadUrl, downloadZipFileFullPath);

            return downloadZipFileFullPath;
        }

        internal string GetNewChromeDriverFromZipFile(string zipFileDownloadPath)
        {
            string unzipPath = Path.ChangeExtension(zipFileDownloadPath, string.Empty);

            UnzipFile(zipFileDownloadPath, unzipPath, true);

            return FindNewChromeDriverFullPathFromUnzipPath(unzipPath);
        }

        internal string FindNewChromeDriverFullPathFromUnzipPath(string chromeDriverUnzipPath)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(chromeDriverUnzipPath);

            FileInfo[] files = directoryInfo.GetFiles();

            foreach (FileInfo file in files)
            {
                // ignore case
                if (file.Name.ToLower() == update.ChromeDriverName.ToLower())
                {
                    string newPath = Path.GetDirectoryName(chromeDriverUnzipPath) + file.Name;
                    
                    File.Copy(file.Name, newPath, true);

                    Directory.Delete(chromeDriverUnzipPath, true);

                    return newPath;
                }
            }

            throw new UpdateFailException(ErrorCode.CannotDownloadNewChromeDriver);
        }

        internal void DownloadFile(string downloadUrl, string downloadPath)
        {
            try
            {
                new WebClient().DownloadFile(downloadUrl, downloadPath);
            }
            catch
            {
                throw new UpdateFailException(ErrorCode.CannotDownloadNewChromeDriver);
            }
        }

        internal void UnzipFile(string zipPath, string unzipPath, bool deleteZipFile = true)
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
                throw new UpdateFailException(ErrorCode.CannotUnzipChromeDriverZipFile);
            }
        }
    }
}
