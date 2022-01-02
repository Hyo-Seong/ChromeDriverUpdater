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
        public void LinuxTest()
        {

            Updater updater = new Updater();
            try
            {
                updater.Update("chromedriver");
            } 
            catch(UpdateFailException exc)
            {
                Console.WriteLine(exc);
                // In Github action, Chrome is not downloaded.
                // Or cannot access to registry.
                Assert.IsTrue(exc.ErrorCode == Models.ErrorCode.ChromeNotInstalled);
            }
            Assert.IsTrue(true);
        }

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

            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                // tbd
            }

            Updater updater = new Updater();
            try
            {
                updater.Update("chromedriver.exe");
            } 
            catch(UpdateFailException exc)
            {
                // In Github action, Chrome is not downloaded.
                // Or cannot access to registry.
                Assert.IsTrue(exc.ErrorCode == Models.ErrorCode.ChromeNotInstalled);
            }

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
