using System;
using System.Collections.Generic;
using System.Text;

namespace AILZ80ASM
{
    public class ErrorMessageException : Exception
    {
        public ErrorMessage.ErrorTypeEnum ErrorType { get; private set; }

        public ErrorMessageException(ErrorMessage.ErrorTypeEnum errorType, string message)
            : base(message)
        {
            ErrorType = ErrorType;
        }

        public ErrorMessageException(ErrorMessage.ErrorTypeEnum errorType, string message, Exception innerException)
            : base(message, innerException)
        {
            ErrorType = ErrorType;
        }

    }
}
