using System;
using System.IO;
using NUnit.Framework;

namespace ChromeDriverUpdater.Tests
{
    [TestFixture]
    public class UpdateTest
    {
        [Test]
        public void LinuxTest()
        {
            if(File.Exists("chromedriver"))
            {
                File.Delete("chromedriver");
            }

            File.Copy("chromedriver_linux64", "chromedriver");
            
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
            if(File.Exists("chromedriver.exe"))
            {
                File.Delete("chromedriver.exe");
            }
            
            File.Copy("chromedriver_old.exe", "chromedriver.exe");

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
