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
            // 命令を展開する
            ExpansionItem();

            // 値のラベルを処理する
            BuildValueLabel();

            // プレアセンブル
            PreAssemble();

            // 値のラベルを処理する
            BuildAddressLabel();

            // アセンブルを行う
            InternalAssemble();

            // エラーの出力
            foreach (var fileItem in FileItems)
            {
                ErrorMessages.Add(new FileItemErrorMessage(fileItem.ErrorMessages.ToArray(), fileItem));
            }
        }

        /// <summary>
        /// プレアセンブル
        /// </summary>
        private void PreAssemble()
        {
            var address = default(AsmAddress);

            foreach (var fileItem in FileItems)
            {
                fileItem.PreAssemble(ref address);
            }
        }

        /// <summary>
        /// 命令の展開
        /// </summary>
        private void ExpansionItem()
        {
            foreach (var fileItem in FileItems)
            {
                fileItem.ExpansionItem();
            }
        }

        /// <summary>
        /// 値ラベルを作りこむ
        /// </summary>
        private void BuildValueLabel()
        {
            var labelCount = 0;

            do
            {
                labelCount = AssembleLoad.Labels.Count(m => m.HasValue);
                foreach (var label in AssembleLoad.Labels.Where(m => !m.HasValue))
                {
                    label.SetValue(AssembleLoad);
                }

            } while (labelCount != AssembleLoad.Labels.Count(m => m.HasValue));
        }

        /// <summary>
        /// アドレスラベルを作りこむ
        /// </summary>
        private void BuildAddressLabel()
        {
            var labelCount = 0;

            do
            {
                labelCount = AssembleLoad.Labels.Count(m => m.HasValue);
                foreach (var fileItem in FileItems)
                {
                    fileItem.BuildAddressLabel();
                }

            } while (labelCount != AssembleLoad.Labels.Count(m => m.HasValue));
        }

        /// <summary>
        /// アセンブルを実行する
        /// </summary>
        private void InternalAssemble()
        {
            foreach (var fileItem in FileItems)
            {
                fileItem.Assemble();
            }
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
            if (ErrorMessages.Count > 0)
            {
                Console.WriteLine($"");
                OutputError(Errors, "Error");
                OutputError(Warnings, "Warning");
                OutputError(Infomations, "Infomation");
            }

            Console.WriteLine($"");
            Console.WriteLine($" {Errors.Length:0} error(s), {Warnings.Length} warning(s), {Infomations.Length} infomation");
        }

        private void OutputError(FileItemErrorMessage[] fileItemErrorMessages, string title)
        {
            var count = fileItemErrorMessages.Sum(m => m.LineItemErrorMessages.Length);
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
