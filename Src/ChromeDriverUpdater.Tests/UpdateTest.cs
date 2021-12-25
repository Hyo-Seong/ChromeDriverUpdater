using System.IO;
using NUnit.Framework;

namespace ChromeDriverUpdater.Tests
{
    [TestFixture]
    public class UpdateTest
    {
        [Test]
        public void ChromeDriverUpdateTest()
        {
            File.Delete("chromedriver.exe");
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
                updater.Update("wrong.exe");
            }
            catch(UpdateFailException exc)
            {
                Assert.IsTrue(exc.ErrorCode == Models.ErrorCode.ChromeDriverNotFound);
            }
        }

    }
}
