using System;

namespace ChromeDriverUpdater
{
    internal interface IUpdateHelper
    {
        string ChromeDriverName { get; }
        string ChromeDriverZipFileName { get; }
        Version GetChromeVersion();
    }
}
