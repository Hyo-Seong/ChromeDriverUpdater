using System;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using NUnit.Framework;

namespace ChromeDriverUpdater.Tests
{
    [TestFixture]
    public class UpdateTest
    {
        private const string WIN_TEST_CHROMEDRIVER_NAME = "chromedriver_win32.exe";
        private const string WIN_CHROMEDRIVER_NAME = "chromedriver.exe";
        private const string LINUX_TEST_CHROMEDRIVER_NAME = "chromedriver_linux64";
        private const string LINUX_CHROMEDRIVER_NAME = "chromedriver";

        [Test]
        public void ChromeDriverUpdateTest()
        {
            string chromeDriverName = string.Empty;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                chromeDriverName = WIN_CHROMEDRIVER_NAME;
                SetTestFile(WIN_TEST_CHROMEDRIVER_NAME, chromeDriverName);

                SetWindowsRegistryIfNotExist();
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                chromeDriverName = LINUX_CHROMEDRIVER_NAME;
                SetTestFile(LINUX_TEST_CHROMEDRIVER_NAME, chromeDriverName);

                ProcessExecuter processExecuter = new ProcessExecuter();
                processExecuter.Run("chmod", $"755 {chromeDriverName}");
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                throw new NotImplementedException();
            }

            ChromeDriverUpdater updater = new ChromeDriverUpdater();
            
            updater.Update(chromeDriverName);

            Assert.True(true);
        }

        [Test]
        public void WrongChromeDriverPathTest1()
        {
            try
            {
                ChromeDriverUpdater updater = new ChromeDriverUpdater();
                updater.Update("wrong");
            }
            catch(FileNotFoundException)
            {
                Assert.True(true);
            }
            catch(Exception)
            {
                Assert.True(false);
            }
        }

        [Test]
        public void WrongChromeDriverPathTest2()
        {
            try
            {
                ChromeDriverUpdater updater = new ChromeDriverUpdater();
                updater.Update(null);
            }
            catch (ArgumentNullException)
            {
                Assert.True(true);
            }
            catch (Exception)
            {
                Assert.True(false);
            }
        }

        private void SetTestFile(string testFile, string renameTo)
        {
            if (File.Exists(renameTo))
            {
                File.Delete(renameTo);
            }

            File.Copy(testFile, renameTo);
        }

        private void SetWindowsRegistryIfNotExist()
        {
            try
            {
                WindowsUpdateHelper updater = new WindowsUpdateHelper();
                Version v = updater.GetChromeVersion();

                if(v == null)
                {
                    throw new Exception();
                }
            }
            catch
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey("Software\\Google\\Chrome\\BLBeacon");
                if (key == null)
                {
                    key = Registry.CurrentUser.CreateSubKey("Software\\Google\\Chrome\\BLBeacon");
                }

                key.SetValue("version", "96.0.4664.110");
            }
        }
    }
}
