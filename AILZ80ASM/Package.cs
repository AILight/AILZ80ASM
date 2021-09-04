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
        //private List<Macro> Macros { get; set; } = new List<Macro>();
        public AsmLoad AssembleLoad { get; private set; }  = new AsmLoad();

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

            this.AssembleLoad.LoadCloseValidate();
        }

        public void Assemble()
        {
            var address = default(AsmAddress);

            /*
            // マクロをロードする
            foreach (var fileItem in FileItems)
            {
                fileItem.LoadMacro();
                Macros.AddRange(this.Macros);
            }
            */

            // 命令を展開する
            foreach (var fileItem in FileItems)
            {
                fileItem.ExpansionItem(AssembleLoad);
            }

            // 値のラベルを処理する
            ProcessLabelValue();

            {
                /*
                var labels = FileItems.SelectMany(m => m.Labels.Where(m => m.HasValue)).ToArray();
                foreach (var fileItem in FileItems)
                {
                    fileItem.PreAssemble(ref address, labels);
                }
                */
            }

            // 値のラベルを処理する
            ProcessLabelValueAndAddress();

            //　アセンブルを行う
            {
                /*
                var labels = FileItems.SelectMany(m => m.Labels.Where(m => m.HasValue)).ToArray();
                foreach (var fileItem in FileItems)
                {
                    fileItem.Assemble(labels);
                }
                */
            }

            // エラーの出力
            foreach (var fileItem in FileItems)
            {
                ErrorMessages.Add(new FileItemErrorMessage(fileItem.ErrorMessages.ToArray(), fileItem));
            }

            /*
            {
                var labels = labelList.ToArray();
                foreach (var fileItem in FileItems)
                {
                    fileItem.SetValueLabel(labels);
                    labelList.AddRange(fileItem.Items.SelectMany(m => m.LineExpansionItems.Where(m => m.Label.DataType != Label.DataTypeEnum.None).Select(m => m.Label)));
                }

                foreach (var fileItem in FileItems)
                {
                    fileItem.Assemble(labels);
                    labelList.AddRange(fileItem.Items.SelectMany(m => m.LineExpansionItems.Where(m => m.Label.DataType != Label.DataTypeEnum.None).Select(m => m.Label)));
                }

                foreach (var fileItem in FileItems)
                {
                    ErrorMessages.Add(new FileItemErrorMessage(fileItem.ErrorMessages.ToArray(), fileItem));
                }
            }
            */
        }

        private void ProcessLabelValue()
        {
            var labelValueCount = 0;
            var labels = default(Label[]);
            do
            {
                //labels = FileItems.SelectMany(m => m.Labels).ToArray();
                labelValueCount = labels.Count(m => m.HasValue);
                foreach (var fileItem in FileItems)
                {
                    fileItem.ProcessLabelValue(labels);
                }
            } while (labelValueCount != labels.Count(m => m.HasValue));
        }

        private void ProcessLabelValueAndAddress()
        {
            var labelValueCount = 0;
            var labels = default(Label[]);
            do
            {
                //labels = FileItems.SelectMany(m => m.Labels).ToArray();
                labelValueCount = labels.Count(m => m.HasValue);
                foreach (var fileItem in FileItems)
                {
                    fileItem.ProcessLabelValueAndAddress(labels);
                }
            } while (labelValueCount != labels.Count(m => m.HasValue));
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
                /*
                var bin = item.Bin;
                if (bin.Length > 0)
                {
                    stream.Write(bin, 0, bin.Length);
                }
                */
            }
        }

        public void OutputError()
        {
            if (ErrorMessages.Count > 0)
            {
                Console.WriteLine($"");
                OutputError(Errors, "Error");
                OutputError(Warnings, "Warning");
                OutputError(Infomations, "Infomation");
            }

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
