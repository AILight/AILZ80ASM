using System;
using System.Collections.Generic;
using System.Text;

namespace AILZ80ASM
{
    public class FileItemErrorMessage
    {
        public FileItem FileItem { get; private set; }
        public LineItemErrorMessage[] LineItemErrorMessages { get; private set; }

        public FileItemErrorMessage(LineItemErrorMessage[] lineItemErrorMessages, FileItem fileItem)
        {
            FileItem = fileItem;
            LineItemErrorMessages = lineItemErrorMessages;
        }

    }
}
