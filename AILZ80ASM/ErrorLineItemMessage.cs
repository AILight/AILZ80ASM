using System;
using System.Collections.Generic;
using System.Text;

namespace AILZ80ASM
{
    public class ErrorLineItemMessage
    {
        public LineItem LineItem { get; private set; }
        public ErrorMessageException ErrorMessageException { get; private set; }

        public ErrorLineItemMessage(ErrorMessageException errorMessageException, LineItem lineItem)
        {
            ErrorMessageException = errorMessageException;
            LineItem = lineItem;
        }

    }
}
