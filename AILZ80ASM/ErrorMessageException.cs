using System;
using System.Collections.Generic;
using System.Text;

namespace AILZ80ASM
{
    public class ErrorMessageException : Exception
    {
        public Error.ErrorCodeEnum ErrorCode { get; private set; }
        public string AdditionalMessage { get; private set; }

        public ErrorMessageException(Error.ErrorCodeEnum errorCode)
            : this(errorCode, "", default(Exception))
        {
        }

        public ErrorMessageException(Error.ErrorCodeEnum errorCode, string message)
            : this(errorCode, message, default(Exception))
        {
        }

        public ErrorMessageException(Error.ErrorCodeEnum errorCode, string message, Exception innerException)
            : base(Error.GetMessage(errorCode), innerException)
        {
            ErrorCode = errorCode;
            AdditionalMessage = message;
        }

    }
}
