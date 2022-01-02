using System;

namespace ChromeDriverUpdater
{
    internal interface IUpdater
    {
        string ChromeDriverName { get; }
        string ChromeDriverZipFileName { get; }
        Version GetChromeVersion();
    }
}
