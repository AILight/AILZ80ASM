using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AILZ80ASM
{
    public class Package
    {
        private List<FileItem> FileItems { get; set; } = new List<FileItem>();
        private List<FileItemErrorMessage> ErrorMessages { get; set; } = new List<FileItemErrorMessage>();

        public FileItemErrorMessage[] Errors => ErrorMessages.Where(m => m.LineItemErrorMessages.Any(n => n.ErrorType == LineItemErrorMessage.ErrorTypeEnum.Error)).Select(m => new FileItemErrorMessage(m.LineItemErrorMessages.Where(m => m.ErrorType == LineItemErrorMessage.ErrorTypeEnum.Error).ToArray(), m.FileItem)).ToArray();
        public FileItemErrorMessage[] Warnings => ErrorMessages.Where(m => m.LineItemErrorMessages.Any(n => n.ErrorType == LineItemErrorMessage.ErrorTypeEnum.Warning)).Select(m => new FileItemErrorMessage(m.LineItemErrorMessages.Where(m => m.ErrorType == LineItemErrorMessage.ErrorTypeEnum.Warning).ToArray(), m.FileItem)).ToArray();
        public FileItemErrorMessage[] Infomations => ErrorMessages.Where(m => m.LineItemErrorMessages.Any(n => n.ErrorType == LineItemErrorMessage.ErrorTypeEnum.Infomation)).Select(m => new FileItemErrorMessage(m.LineItemErrorMessages.Where(m => m.ErrorType == LineItemErrorMessage.ErrorTypeEnum.Infomation).ToArray(), m.FileItem)).ToArray();

        public Package(FileInfo[] Files)
        {
            foreach (var fileInfo in Files)
            {
                FileItems.Add(new FileItem(fileInfo, this));
            }
        }

        public void Assemble()
        {
            var address = default(UInt16);
            var labelList = new List<Label>();

            foreach (var fileItem in FileItems)
            {
                fileItem.PreAssemble(ref address);
                labelList.AddRange(fileItem.Items.Where(m => m.Label.DataType != Label.DataTypeEnum.None).Select(m => m.Label));
            }

            var labels = labelList.ToArray();
            foreach (var fileItem in FileItems)
            {
                fileItem.SetValueLabel(labels);
                labelList.AddRange(fileItem.Items.Where(m => m.Label.DataType != Label.DataTypeEnum.None).Select(m => m.Label));
            }

            foreach (var fileItem in FileItems)
            {
                fileItem.Assemble(labels);
                labelList.AddRange(fileItem.Items.Where(m => m.Label.DataType != Label.DataTypeEnum.None).Select(m => m.Label));
            }

            foreach (var fileItem in FileItems)
            {
                ErrorMessages.Add(new FileItemErrorMessage(fileItem.ErrorMessages.ToArray(), fileItem));
            }
            //ErrorMessages = ErrorMessages.OrderBy(m => (int)m.ErrorType).ThenBy(m => m.).ToList();
        }

        public void Save(FileInfo output)
        {
            var fileStream = output.OpenWrite();

            Save(fileStream);

            fileStream.Close();
        }

        public void Save(Stream stream)
        {
            foreach (var item in FileItems)
            {
                var bin = item.Bin;
                stream.Write(bin, 0, bin.Length);
            }
        }

        public void OutputError()
        {
            OutputError(Errors, "Error");
            OutputError(Warnings, "Warning");
            OutputError(Infomations, "Infomation");
        }

        private void OutputError(FileItemErrorMessage[] FileItemErrorMessages, string title)
        {
            var count = Errors.Sum(m => m.LineItemErrorMessages.Count());
            if (count > 0)
            {
                Console.WriteLine($"{title}:{count}");
                foreach (var fileItemError in Errors)
                {
                    Console.WriteLine($" File:{fileItemError.FileItem.LoadFileName}");
                    foreach (var lineItemError in fileItemError.LineItemErrorMessages)
                    {
                        Console.WriteLine($"  Line:{lineItemError.LineItem.LineIndex + 1:0000} Message:{lineItemError.ErrorMessageString}");
                    }
                }
            }
        }

    }
}
