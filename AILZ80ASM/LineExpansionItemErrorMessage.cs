using System;
using System.Collections.Generic;
using System.Text;

namespace AILZ80ASM
{
    public class LineExpansionItemErrorMessage
    {
        public ErrorMessageException ErrorMessageException { get; private set; }
        public LineItem LineItem { get; private set; }

        public LineExpansionItemErrorMessage(ErrorMessageException errorMessageException, LineItem lineItem)
        {
            ErrorMessageException = errorMessageException;
            LineItem = lineItem;
        }

    }
}
