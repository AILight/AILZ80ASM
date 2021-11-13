namespace AILZ80ASM
{
    public class ErrorLineItem
    {
        public LineItem LineItem { get; private set; }
        public Error.ErrorCodeEnum ErrorCode { get; private set; }
        public Error.ErrorTypeEnum ErrorType => Error.GetErrorType(ErrorCode);
        public string ErrorMessage => Error.GetMessage(this.ErrorCode, Parameters);
        private object[] Parameters { get; set; }

        public ErrorLineItem(LineItem lineItem, ErrorAssembleException errorAssembleException)
            : this(lineItem, errorAssembleException.ErrorCode, errorAssembleException.Parameters)
        {
        }

        public ErrorLineItem(LineItem lineItem, Error.ErrorCodeEnum errorCode)
            : this(lineItem, errorCode, "")
        {
        }

        public ErrorLineItem(LineItem lineItem, Error.ErrorCodeEnum errorCode, params object[] parameters)
        {
            LineItem = lineItem;
            ErrorCode = errorCode;
            Parameters = parameters;
        }
    }
}
