namespace ChromeDriverUpdater.Models
{
    public enum ErrorCode
    {
        Success = 1,
        Fail = -1,
        CannotDownloadNewChromeDriver = -2,
        CannotGetLatestRelease = -3,
        ChromeNotInstalled = -4,
        ChromeDriverNotFound = -5
    }
}
