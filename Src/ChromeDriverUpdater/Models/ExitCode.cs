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
        CannotGetLatestRelase = -2,
        ChromeNotInstalled = -3
    }
}
