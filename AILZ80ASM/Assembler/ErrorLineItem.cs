using AILZ80ASM.Assembler;
using System.Reflection.Emit;

namespace AILZ80ASM.Assembler
{
    public class ErrorLineItem
    {
        public LineItem LineItem { get; private set; }
        public Error.ErrorCodeEnum ErrorCode { get; private set; }
        public Error.ErrorTypeEnum ErrorType => Error.GetErrorType(ErrorCode);
        public Exceptions.ErrorAssembleException InnerErrorAssembleException { get; private set; }
        public string ErrorMessage => Error.GetMessage(this.ErrorCode, Parameters);
        private object[] Parameters { get; set; }

        public ErrorLineItem(LineItem lineItem, Exceptions.ErrorAssembleException errorAssembleException)
            : this(lineItem, errorAssembleException.ErrorCode, errorAssembleException.Parameters)
        {
            InnerErrorAssembleException = errorAssembleException;
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

        public void AssociateErrorLineItem()
        {
            LineItem.ErrorLineItem = this; //エラー情報と紐づけ
        }
    }
}
