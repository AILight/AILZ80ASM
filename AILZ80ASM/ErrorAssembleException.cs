using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace AILZ80ASM
{
    public class ErrorAssembleException : Exception
    {
        public Error.ErrorCodeEnum ErrorCode { get; private set; }
        public object[] Parameters { get; set; }

        public ErrorAssembleException(Error.ErrorCodeEnum errorCode)
            : this(errorCode, default(string[]))
        {
        }

        public ErrorAssembleException(Error.ErrorCodeEnum errorCode, params object[] parameters)
            : this(errorCode, default, parameters)
        {
        }

        public ErrorAssembleException(Error.ErrorCodeEnum errorCode, Exception innerException, params object[] parameters)
            : base(Error.GetMessage(errorCode, parameters), innerException)
        {
            this.ErrorCode = errorCode;
            this.Parameters = parameters;
        }

    }
}
