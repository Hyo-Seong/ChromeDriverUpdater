using System;
using System.IO;
using System.Runtime.InteropServices;
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
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                if (File.Exists(WIN_CHROMEDRIVER_NAME))
                {
                    File.Delete(WIN_CHROMEDRIVER_NAME);
                }

                File.Copy(WIN_TEST_CHROMEDRIVER_NAME, WIN_CHROMEDRIVER_NAME);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                if (File.Exists(LINUX_CHROMEDRIVER_NAME))
                {
                    File.Delete(LINUX_CHROMEDRIVER_NAME);
                }

                File.Copy(LINUX_TEST_CHROMEDRIVER_NAME, LINUX_CHROMEDRIVER_NAME);

                ProcessExecuter processExecuter = new ProcessExecuter();
                processExecuter.Run("chmod", "755 {LINUX_CHROMEDRIVER_NAME}");
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                // tbd
            }

            Updater updater = new Updater();
            
            updater.Update("chromedriver.exe");

            Assert.True(true);
        }

        [Test]
        public void WrongChromeDriverPathTest()
        {
            try
            {
                Updater updater = new Updater();
                updater.Update("wrong");
            }
            catch(UpdateFailException exc)
            {
                Assert.IsTrue(exc.ErrorCode == Models.ErrorCode.ChromeDriverNotFound);
            }
        }
    }
}
