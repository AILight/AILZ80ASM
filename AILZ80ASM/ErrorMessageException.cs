using System;
using System.Collections.Generic;
using System.Text;

namespace AILZ80ASM
{
    public class ErrorMessageException : Exception
    {
        public LineItemErrorMessage.ErrorTypeEnum ErrorType { get; private set; }

        public ErrorMessageException(LineItemErrorMessage.ErrorTypeEnum errorType, string message)
            : this(errorType, message, default(Exception))
        {
        }

        public ErrorMessageException(LineItemErrorMessage.ErrorTypeEnum errorType, string message, Exception innerException)
            : base(message, innerException)
        {
            ErrorType = errorType;
        }

    }
}
