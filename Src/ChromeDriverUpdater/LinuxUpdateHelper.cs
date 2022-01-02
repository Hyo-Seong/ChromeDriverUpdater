using System;

namespace ChromeDriverUpdater
{
    internal class LinuxUpdateHelper : IUpdateHelper
    {
        public string ChromeDriverName => "chromedriver";
        public string ChromeDriverZipFileName => "chromedriver_linux64.zip";

        public Version GetChromeVersion()
        {
            string[] chromeNameArray = new string[] 
            {
                "google-chrome",
                "google-chrome-stable",
                "google-chrome-beta",
                "google-chrome-dev",
                "chromium-browser",
                "chromium"
            };

            foreach(string chromeName in chromeNameArray)
            {
                try
                {
                    ProcessExecuter processExecuter = new ProcessExecuter();
                    string executeResult = processExecuter.Run(chromeName, "--version");

                    string versionString = executeResult.Replace("Google Chrome", string.Empty).Trim();

                    return new Version(versionString);
                } catch (Exception exc)
                {
                    Console.WriteLine(exc.ToString());
                }
            }
            throw new Exception();
        }
    }
}
