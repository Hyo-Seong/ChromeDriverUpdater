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
        internal const string DOWNLOAD_ZIP_FILE_NAME = "chromedriver_win32.zip";
        internal const string CHROME_DRIVER_BASE_NAME = "chromedriver.exe";

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
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                throw new UpdateFailException(ErrorCode.UnSupportedOSPlatform);
            }

            // change to full path to shutdown chromedriver
            chromeDriverFullPath = Path.GetFullPath(chromeDriverFullPath);

            if(!File.Exists(chromeDriverFullPath))
            {
                throw new UpdateFailException(ErrorCode.ChromeDriverNotFound);
            }

            Version chromeDriverVersion = GetChromeDriverVersion(chromeDriverFullPath);
            Version chromeVersion = GetChromeVersionFromRegistry();

            if (UpdateNecessary(chromeDriverVersion, chromeVersion))
            {
                ShutdownChromeDriver(chromeDriverFullPath);

                UpdateChromeDriver(chromeDriverFullPath, chromeVersion);
            }
        }

        internal Version GetChromeDriverVersion(string chromeDriverFullPath)
        {
            string output = new ProcessExecuter().Run("CMD.exe", $"/C \"{chromeDriverFullPath}\" -version");

            // output like this
            // ChromeDriver 88.0.4324.96 (68dba2d8a0b149a1d3afac56fa74648032bcf46b-refs/branch-heads/4324@{#1784})
            string versionStr = output.Split(' ')[1];
            Version version = new Version(versionStr);

            return version;
        }

        internal Version GetChromeVersionFromRegistry()
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

            string downloadUrl = $"{CHROME_DRIVER_BASE_URL}/{version}/{DOWNLOAD_ZIP_FILE_NAME}";
            string downloadZipFileFullPath = Path.Combine(Path.GetTempPath(), DOWNLOAD_ZIP_FILE_NAME);

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
                if (file.Name.ToLower() == CHROME_DRIVER_BASE_NAME.ToLower())
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
