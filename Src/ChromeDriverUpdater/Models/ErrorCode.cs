namespace ChromeDriverUpdater.Models
{
    public enum ErrorCode
    {
        UnknownError = -1,
        CannotDownloadNewChromeDriver = -2,
        CannotGetLatestRelease = -3,
        ChromeNotInstalled = -4,
        ChromeDriverNotFound = -5,
        CannotShutdownloadChromeDriver = -6,
        CannotUnzipChromeDriverZipFile = -7,
        UnSupportedOSPlatform = -8
    }
}
