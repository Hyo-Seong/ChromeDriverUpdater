using System;

namespace ChromeDriverUpdater
{
    internal class LinuxUpdater : IUpdater
    {
        public string ChromeDriverName => "chromedriver";
        public string ChromeDriverZipFileName => "chromedriver_linux64.zip";

        public Version GetChromeDriverVersion(string chromeDriverPath)
        {
            throw new NotImplementedException();
        }

        public Version GetChromeVersion()
        {
            string[] aa = new string[] 
            {
                "google-chrome",
                "google-chrome-stable",
                "google-chrome-beta",
                "google-chrome-dev",
                "chromium-browser",
                "chromium"
            };

            foreach(string a in aa)
            {
                try
                {
                    ProcessExecuter processExecuter = new ProcessExecuter();
                    string result = processExecuter.Run(a, "--version");

                    string ad = result.Replace("Google Chrome", string.Empty);
                    ad = ad.Trim();

                    return new Version(ad);
                } catch (Exception exc)
                {
                    Console.WriteLine(exc.ToString());
                }
            }
            throw new Exception();
        }
    }
}
