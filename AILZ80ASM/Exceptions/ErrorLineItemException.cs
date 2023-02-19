using AILZ80ASM.Assembler;
using System;

namespace AILZ80ASM.Exceptions
{
    public class ErrorLineItemException : Exception
    {
        public ErrorLineItem ErrorLineItem { get; private set; }

        public ErrorLineItemException(ErrorLineItem errorLineItem)
            : base(errorLineItem.ErrorMessage)
        {
            ErrorLineItem = errorLineItem;
        }

        public void AssociateErrorLineItem()
        {
            ErrorLineItem.AssociateErrorLineItem();
        }
    }
}
