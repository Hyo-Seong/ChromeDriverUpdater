namespace ChromeDriverUpdater.Models
{
    public enum ErrorCode : byte
    {
        UnknownError,
        CannotDownloadNewChromeDriver,
        CannotGetLatestRelease,
        ChromeNotInstalled,
        ChromeDriverNotFound,
        CannotShutdownloadChromeDriver,
        CannotUnzipChromeDriverZipFile,
        UnSupportedOSPlatform,
    }
}
