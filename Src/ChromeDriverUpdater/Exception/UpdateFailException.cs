using ChromeDriverUpdater.Models;
using System;

namespace ChromeDriverUpdater
{
    public class UpdateFailException : Exception
    {
        public ErrorCode ErrorCode { get; set; }

        public UpdateFailException()
        {
        }

        public UpdateFailException(string message, ErrorCode errorCode)
            : base(message)
        {
            ErrorCode = errorCode;
        }

        public UpdateFailException(string message, ErrorCode errorCode, Exception inner)
            : base(message, inner)
        {
            ErrorCode = errorCode;
        }
    }
}
