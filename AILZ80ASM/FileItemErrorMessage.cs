using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AILZ80ASM
{
    public class FileInfoErrorMessage
    {
        public FileInfo FileInfo { get; private set; }
        public ErrorLineItemMessage[] ErrorLineItemMessages { get; private set; }

        public FileInfoErrorMessage(ErrorLineItemMessage[] errorLineItemMessage, FileItem fileItem)
        {
            FileInfo = fileItem.FileInfo;
            ErrorLineItemMessages = errorLineItemMessage;
        }

        public FileInfoErrorMessage(ErrorLineItemMessage[] errorLineItemMessage, FileInfo fileInfo)
        {
            FileInfo = fileInfo;
            ErrorLineItemMessages = errorLineItemMessage;
        }

    }
}
