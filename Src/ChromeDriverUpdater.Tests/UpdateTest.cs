using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace ChromeDriverUpdater.Tests
{
    [TestClass]
    public class UpdateTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            File.Delete("chromedriver.exe");
            File.Copy("chromedriver_old.exe", "chromedriver.exe");

            Updater updater = new Updater();
            updater.Update("chromedriver.exe");
        }
    }
}
