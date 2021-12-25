using ChromeDriverUpdater.Models;
using System;

namespace ChromeDriverUpdater
{
    public class UpdateFailException : Exception
    {
        public ErrorCode ErrorCode { get; set; }

        public UpdateFailException(ErrorCode errorCode)
        {
            ErrorCode = errorCode;
        }
    }
}
