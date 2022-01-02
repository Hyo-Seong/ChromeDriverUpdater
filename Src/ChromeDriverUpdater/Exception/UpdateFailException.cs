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

        public override string ToString()
        {
            return $"ErrorCode: {ErrorCode}" + Environment.NewLine + base.ToString() ;
        }
    }
}
