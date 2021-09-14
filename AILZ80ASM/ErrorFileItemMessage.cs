using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AILZ80ASM
{
    public class ErrorFileInfoMessage
    {
        public FileInfo FileInfo { get; private set; }
        public ErrorLineItemMessage[] ErrorLineItemMessages { get; private set; }

        public ErrorFileInfoMessage(ErrorLineItemMessage[] errorLineItemMessage, FileItem fileItem)
        {
            FileInfo = fileItem.FileInfo;
            ErrorLineItemMessages = errorLineItemMessage;
        }

        public ErrorFileInfoMessage(ErrorLineItemMessage[] errorLineItemMessage, FileInfo fileInfo)
        {
            FileInfo = fileInfo;
            ErrorLineItemMessages = errorLineItemMessage;
        }
    }
}
