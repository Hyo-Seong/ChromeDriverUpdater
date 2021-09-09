using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChromeDriverUpdater.Models
{
    public enum ExitCode
    {
        Success = 1,
        Fail = -1,
        CannotDownloadNewChromeDriver = -2,
        CannotGetLatestRelase = -3,
        ChromeNotInstalled = -4
    }
}
