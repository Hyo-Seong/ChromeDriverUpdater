using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Runtime.InteropServices;

namespace ChromeDriverUpdater
{
    public class ChromeDriverUpdater
    {
        internal const string CHROME_DRIVER_BASE_URL = "https://chromedriver.storage.googleapis.com";

        private IUpdateHelper updateHelper;

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
            if(string.IsNullOrEmpty(chromeDriverFullPath))
            {
                throw new ArgumentNullException(nameof(chromeDriverFullPath));
            }

            // change to full path to shutdown chromedriver
            chromeDriverFullPath = Path.GetFullPath(chromeDriverFullPath);

            if (!File.Exists(chromeDriverFullPath))
            {
                throw new FileNotFoundException();
            }

            updateHelper = GetUpdateHelper();

            Version chromeDriverVersion = GetChromeDriverVersion(chromeDriverFullPath);
            Version chromeVersion = updateHelper.GetChromeVersion();

            if (UpdateNecessary(chromeDriverVersion, chromeVersion))
            {
                if(updateHelper is WindowsUpdateHelper){
                    ShutdownChromeDriver(chromeDriverFullPath);
                }

                UpdateChromeDriver(chromeDriverFullPath, chromeVersion);
            }
        }

        internal IUpdateHelper GetUpdateHelper()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return new WindowsUpdateHelper();
            }
            else if(RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return new LinuxUpdateHelper();
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
            }
         
            throw new NotSupportedException();
        }

        internal bool UpdateNecessary(Version chromeVersion, Version chromeDriverVersion)
        {
            return !CompareVersionMajorToBuild(chromeVersion, chromeDriverVersion);
        }

        internal bool CompareVersionMajorToBuild(Version v1, Version v2)
        {
            if(v1 == null)
            {
                throw new ArgumentNullException(nameof(v1));
            }
            if(v2 == null)
            {
                throw new ArgumentNullException(nameof(v2));
            }

            return v1.Major == v2.Major &&
                   v1.Minor == v2.Minor &&
                   v1.Build == v2.Build;
        }

        internal Version GetChromeDriverVersion(string chromeDriverPath)
        {
            string output = new ProcessExecuter().Run($"{chromeDriverPath}", "-v");

            // output like this
            // ChromeDriver 88.0.4324.96 (68dba2d8a0b149a1d3afac56fa74648032bcf46b-refs/branch-heads/4324@{#1784})
            string versionStr = output.Split(' ')[1];
            Version version = new Version(versionStr);

            return version;
        }

        internal void ShutdownChromeDriver(string chromeDriverFullPath)
        {
            var processes = Process.GetProcesses();

            foreach (Process process in processes)
            {
                // find exact path
                if (process.MainWindowTitle == chromeDriverFullPath)
                {
                    process.Kill();
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
            string url = $"{CHROME_DRIVER_BASE_URL}/LATEST_RELEASE_{chromeVersion.Major}.{chromeVersion.Minor}.{chromeVersion.Build}";

            WebClient client = new WebClient();

            return client.DownloadString(url);
        }

        internal string DownloadChromeDriverZipFile(Version chromeVersion)
        {
            string version = GetProperChromeDriverVersion(chromeVersion);

            string downloadUrl = $"{CHROME_DRIVER_BASE_URL}/{version}/{updateHelper.ChromeDriverZipFileName}";
            string downloadZipFileFullPath = Path.Combine(Path.GetTempPath(), updateHelper.ChromeDriverZipFileName);

            DownloadFile(downloadUrl, downloadZipFileFullPath);

            return downloadZipFileFullPath;
        }

        internal string GetNewChromeDriverFromZipFile(string zipFileDownloadPath)
        {
            string unzipPath = Path.Combine(Path.GetDirectoryName(zipFileDownloadPath), Path.GetFileNameWithoutExtension(zipFileDownloadPath));

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
                if (file.Name.ToLower() == updateHelper.ChromeDriverName.ToLower())
                {
                    string newPath = Path.Combine(Path.GetDirectoryName(chromeDriverUnzipPath), file.Name);
                    
                    File.Copy(file.FullName, newPath, true);

                    Directory.Delete(chromeDriverUnzipPath, true);

                    return newPath;
                }
            }

            throw new Exception("Can't find chromedriver name. name: " + updateHelper.ChromeDriverName.ToLower());
        }

        internal void DownloadFile(string downloadUrl, string downloadPath)
        {
            new WebClient().DownloadFile(downloadUrl, downloadPath);
        }

        internal void UnzipFile(string zipPath, string unzipPath, bool deleteZipFile = true)
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
    }
}
