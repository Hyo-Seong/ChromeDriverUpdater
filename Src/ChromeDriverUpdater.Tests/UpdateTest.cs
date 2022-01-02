using System;
using System.IO;
using System.Runtime.InteropServices;
using NUnit.Framework;

namespace ChromeDriverUpdater.Tests
{
    [TestFixture]
    public class UpdateTest
    {
        [Test]
        public void ChromeDriverUpdateTest()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                if (File.Exists("chromedriver.exe"))
                {
                    File.Delete("chromedriver.exe");
                }

                File.Copy("chromedriver_win32.exe", "chromedriver.exe");
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                if (File.Exists("chromedriver"))
                {
                    File.Delete("chromedriver");
                }

                File.Copy("chromedriver_linux64", "chromedriver");

                ProcessExecuter p = new ProcessExecuter();
                p.Run("chmod chromedriver", "-r 755");
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
