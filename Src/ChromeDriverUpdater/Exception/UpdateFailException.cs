using ChromeDriverUpdater.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChromeDriverUpdater
{
    public class UpdateFailException : Exception
    {
        public ExitCode ExitCode { get; set; }

        public UpdateFailException()
        {
        }

        public UpdateFailException(string message, ExitCode exitCode)
            : base(message)
        {
            ExitCode = exitCode;
        }

        public UpdateFailException(string message, ExitCode exitCode, Exception inner)
            : base(message, inner)
        {
            ExitCode = exitCode;
        }
    }
}
