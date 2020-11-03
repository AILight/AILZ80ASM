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

        public FileItemErrorMessage[] Errors => ErrorMessages.Where(m => m.LineItemErrorMessages.Any(n => Error.GetErrorType(n.ErrorMessageException.ErrorCode) == Error.ErrorTypeEnum.Error)).Select(m => new FileItemErrorMessage(m.LineItemErrorMessages.Where(m => Error.GetErrorType(m.ErrorMessageException.ErrorCode) == Error.ErrorTypeEnum.Error).ToArray(), m.FileItem)).ToArray();
        public FileItemErrorMessage[] Warnings => ErrorMessages.Where(m => m.LineItemErrorMessages.Any(n => Error.GetErrorType(n.ErrorMessageException.ErrorCode) == Error.ErrorTypeEnum.Warning)).Select(m => new FileItemErrorMessage(m.LineItemErrorMessages.Where(m => Error.GetErrorType(m.ErrorMessageException.ErrorCode) == Error.ErrorTypeEnum.Warning).ToArray(), m.FileItem)).ToArray();
        public FileItemErrorMessage[] Infomations => ErrorMessages.Where(m => m.LineItemErrorMessages.Any(n => Error.GetErrorType(n.ErrorMessageException.ErrorCode) == Error.ErrorTypeEnum.Infomation)).Select(m => new FileItemErrorMessage(m.LineItemErrorMessages.Where(m => Error.GetErrorType(m.ErrorMessageException.ErrorCode) == Error.ErrorTypeEnum.Infomation).ToArray(), m.FileItem)).ToArray();

        public Package(FileInfo[] Files)
        {
            foreach (var fileInfo in Files)
            {
                FileItems.Add(new FileItem(fileInfo, this));
            }
        }

        internal void OutputStart()
        {
            Console.WriteLine($"*** AILZ80ASM *** Z-80 Assembler, .NET Core version {System.Reflection.Assembly.GetExecutingAssembly().GetName().Version}");
            Console.WriteLine($"Copyright (C) {DateTime.Today.Year:0} by M.Ishino (AILight)");
            Console.WriteLine();
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
            using var fileStream = output.OpenWrite();

            Save(fileStream);

            fileStream.Close();
        }

        public void Save(Stream stream)
        {
            foreach (var item in FileItems)
            {
                var bin = item.Bin;
                if (bin.Length > 0)
                {
                    stream.Write(bin, 0, bin.Length);
                }
            }
        }

        public void OutputError()
        {
            Console.WriteLine($"");
            OutputError(Errors, "Error");
            OutputError(Warnings, "Warning");
            OutputError(Infomations, "Infomation");

            Console.WriteLine($"");
            Console.WriteLine($" {Errors.Count():0} error(s), {Warnings.Count()} warning(s), {Infomations.Count()} infomation");
        }

        private void OutputError(FileItemErrorMessage[] fileItemErrorMessages, string title)
        {
            var count = fileItemErrorMessages.Sum(m => m.LineItemErrorMessages.Count());
            if (count > 0)
            {
                Console.WriteLine(new string('-', 60));
                foreach (var fileItemError in fileItemErrorMessages)
                {
                    foreach (var lineItemError in fileItemError.LineItemErrorMessages)
                    {
                        Console.WriteLine($"{fileItemError.FileItem.FileInfo.Name}:{lineItemError.LineItem.LineIndex + 1:000000} {lineItemError.ErrorMessageException.Message} -> {lineItemError.ErrorMessageException.AdditionalMessage}");
                    }
                }
            }
        }

    }
}
