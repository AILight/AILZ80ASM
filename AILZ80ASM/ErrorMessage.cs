using System;
using System.Collections.Generic;
using System.Text;

namespace AILZ80ASM
{
    public class ErrorMessage
    {
        public enum ErrorTypeEnum
        {
            Infomation,
            Warning,
            Error,
        }

        public LineItem LineItem { get; set; }
        public ErrorTypeEnum ErrorType { get; set; }
        public string ErrorMessageString { get; set; }
    }
}
