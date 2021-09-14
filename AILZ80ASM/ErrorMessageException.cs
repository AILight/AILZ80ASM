using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace AILZ80ASM
{
    public class ErrorMessageException : Exception
    {
        public Error.ErrorCodeEnum ErrorCode { get; private set; }
        public string[] Parameters { get; private set; }
        public ErrorFileInfoMessage ErrorFileInfoMessage { get; private set; }

        public ErrorMessageException(Error.ErrorCodeEnum errorCode)
            : this(errorCode, default(Exception), default(string[]))
        {
        }

        public ErrorMessageException(Error.ErrorCodeEnum errorCode, params string[] parameters)
            : this(errorCode, default(Exception), parameters)
        {
        }

        public ErrorMessageException(Error.ErrorCodeEnum errorCode, ErrorFileInfoMessage frrorFileItemMessage)
            : this(errorCode, default(Exception), default(string[]))
        {
            ErrorFileInfoMessage = frrorFileItemMessage;
        }


        public ErrorMessageException(Error.ErrorCodeEnum errorCode, Exception innerException, params string[] parameters)
            : base(Error.GetMessage(errorCode), innerException)
        {
            ErrorCode = errorCode;
            Parameters = parameters;
        }

    }
}
