using System;
using System.Collections.Generic;
using System.Text;

namespace AILZ80ASM
{
    public class LineItemErrorMessage
    {
        public enum ErrorTypeEnum
        {
            Infomation,
            Warning,
            Error,
        }

        public ErrorTypeEnum ErrorType { get; private set; }
        public string ErrorMessageString { get; private set; }
        public LineItem LineItem { get; private set; }

        public LineItemErrorMessage(ErrorTypeEnum errorType, string errorMessage, LineItem lineItem)
        {
            ErrorType = errorType;
            ErrorMessageString = errorMessage;
            LineItem = lineItem;
        }

    }
}
