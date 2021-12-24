using System;
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
            updater.Update("chromedriver.exe");

            Assert.True(true);
        }
    }
}
