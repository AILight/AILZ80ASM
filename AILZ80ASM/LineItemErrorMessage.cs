using System;
using System.Collections.Generic;
using System.Text;

namespace AILZ80ASM
{
    public class LineItemErrorMessage
    {
        public ErrorMessageException ErrorMessageException { get; private set; }
        public LineItem LineItem { get; private set; }

        public LineItemErrorMessage(ErrorMessageException errorMessageException, LineItem lineItem)
        {
            ErrorMessageException = errorMessageException;
            LineItem = lineItem;
        }

    }
}
