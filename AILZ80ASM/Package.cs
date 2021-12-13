using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AILZ80ASM
{
    public class Package
    {
        private List<FileItem> FileItems { get; set; } = new List<FileItem>();
        public AsmLoad AssembleLoad { get; private set; }

        public ErrorLineItem[] Errors => AssembleLoad.Errors.Where(m => m.ErrorType == Error.ErrorTypeEnum.Error).ToArray();
        public ErrorLineItem[] Warnings => AssembleLoad.Errors.Where(m => m.ErrorType == Error.ErrorTypeEnum.Warning).ToArray();
        public ErrorLineItem[] Infomations => AssembleLoad.Errors.Where(m => m.ErrorType == Error.ErrorTypeEnum.Infomation).ToArray();

        public Package(FileInfo[] files, AsmLoad.EncodeModeEnum encodeMode, AsmISA asmISA)
        {
            switch (asmISA)
            {
                case AsmISA.Z80:
                    AssembleLoad = new AsmLoad(new InstructionSet.Z80());
                    break;
                default:
                    throw new NotImplementedException();
            }
            var label = new Label("NS_Main::", AssembleLoad);
            AssembleLoad.AddLabel(label);
            AssembleLoad.InputEncodeMode = encodeMode;

            foreach (var fileInfo in files)
            {
                FileItems.Add(new FileItem(fileInfo, AssembleLoad));
            }

            this.AssembleLoad.LoadCloseValidate();
        }

        public void Assemble()
        {
            // 値のラベルを処理する
            BuildValueLabel();

            // 命令を展開する
            ExpansionItem();

            // 命令展開後に値のラベルを処理する
            BuildValueLabel();

            // プレアセンブル
            PreAssemble();

            // アドレスラベルを処理する
            BuildAddressLabel();
            BuildValueLabel();
            BuildArgumentLabel();

            // アセンブルを行う
            InternalAssemble();
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
                labelCount = AssembleLoad.AllLabels.Count(m => m.HasValue);
                foreach (var label in AssembleLoad.AllLabels.Where(m => !m.HasValue))
                {
                    label.SetValue(AssembleLoad);
                }

            } while (labelCount != AssembleLoad.AllLabels.Count(m => m.HasValue));

            foreach (var fileItem in FileItems)
            {
                fileItem.BuildValueLabel();
            }
        }

        /// <summary>
        /// アドレスラベルを作りこむ
        /// </summary>
        private void BuildAddressLabel()
        {
            var labelCount = 0;

            do
            {
                labelCount = AssembleLoad.AllLabels.Count(m => m.HasValue);
                foreach (var fileItem in FileItems)
                {
                    fileItem.BuildAddressLabel();
                }

            } while (labelCount != AssembleLoad.AllLabels.Count(m => m.HasValue));
        }

        /// <summary>
        /// 引数ラベルを作りこむ
        /// </summary>
        private void BuildArgumentLabel()
        {
            foreach (var fileItem in FileItems)
            {
                fileItem.BuildArgumentLabel();
            }
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

        public void SaveOutput(Dictionary<AsmLoad.OutputModeEnum, FileInfo> outputFiles)
        {
            foreach (var item in outputFiles)
            {
                item.Value.Delete();
                using var fileStream = item.Value.OpenWrite();

                SaveOutput(fileStream, item);

                fileStream.Close();
            }

        }

        public void SaveOutput(Stream stream, KeyValuePair<AsmLoad.OutputModeEnum, FileInfo> outputFile)
        {
            switch (outputFile.Key)
            {
                case AsmLoad.OutputModeEnum.BIN:
                    SaveBin(stream);
                    break;
                case AsmLoad.OutputModeEnum.T88:
                    SaveT88(stream, outputFile.Value.Name);
                    break;
                case AsmLoad.OutputModeEnum.CMT:
                    SaveCMT(stream);
                    break;
                default:
                    break;
            }
        }

        public void SaveBin(Stream stream)
        {
            foreach (var item in FileItems)
            {
                item.SaveBin(stream);
            }
        }

        public void SaveT88(Stream stream, string outputFilename)
        {
            using var memoryStream = new MemoryStream();
            foreach (var item in FileItems)
            {
                item.SaveBin(memoryStream);
            }

            var address = default(UInt16);
            if (AssembleLoad.AsmAddresses.Count > 0)
            {
                address = AssembleLoad.AsmAddresses.FirstOrDefault().Program;
            }

            var binaryWriter = new IO.T88BinaryWriter(outputFilename, address, memoryStream.ToArray(), stream);
            binaryWriter.Write();
        }

        public void SaveCMT(Stream stream)
        {
            using var memoryStream = new MemoryStream();
            foreach (var item in FileItems)
            {
                item.SaveBin(memoryStream);
            }

            var address = default(UInt16);
            if (AssembleLoad.AsmAddresses.Count > 0)
            {
                address = AssembleLoad.AsmAddresses.FirstOrDefault().Program;
            }

            var binaryWriter = new IO.CMTBinaryWriter(address, memoryStream.ToArray(), stream);
            binaryWriter.Write();
        }

        public void SaveSymbol(FileInfo symbol)
        {
            using var fileStream = symbol.OpenWrite();

            SaveSymbol(fileStream);

            fileStream.Close();
        }

        public void SaveSymbol(Stream stream)
        {
            using var streamWriter = new StreamWriter(stream);

            foreach (var label in AssembleLoad.AllLabels.Where(m => m.HasValue))
            {
                streamWriter.WriteLine($"{label.Value:X4} {label.LabelName}");
            }
            foreach (var label in AssembleLoad.AllLabels.Where(m => m.HasValue))
            {
                streamWriter.WriteLine($"{label.Value:X4} {label.LongLabelName}");
            }
        }

        public void SaveList(FileInfo list)
        {
            using var streamWriter = new StreamWriter(list.FullName, false, AssembleLoad.GetOutputEncoding());
            SaveList(streamWriter);
            streamWriter.Close();
        }

        public void SaveList(StreamWriter streamWriter)
        {
            streamWriter.WriteLine(AsmList.CreateSource($"{ProductInfo.ProductLongName}").ToString());

            foreach (var item in FileItems)
            {
                item.SaveList(streamWriter);
            }
        }

        public void OutputError()
        {
            if (AssembleLoad.Errors.Count > 0)
            {
                Console.WriteLine($"");
                OutputError(Errors, "Error");
                OutputError(Warnings, "Warning");
                OutputError(Infomations, "Infomation");
            }

            Console.WriteLine($"");
            Console.WriteLine($" {Errors.Length:0} error(s), {Warnings.Length} warning(s), {Infomations.Length} infomation");
        }

        /// <summary>
        /// エラーを表示する
        /// </summary>
        /// <param name="fileItemErrorMessages"></param>
        /// <param name="title"></param>
        public static void OutputError(ErrorLineItem[] errorLineItems, string title)
        {
            if (errorLineItems.Length > 0)
            {
                Console.WriteLine($"> {title}");
                InternalOutputError(errorLineItems, title);
                Console.WriteLine();
            }
        }

        /// <summary>
        /// エラーの詳細を表示
        /// </summary>
        /// <param name="errorLineItems"></param>
        private static void InternalOutputError(ErrorLineItem[] errorLineItems, string title)
        {
            foreach (var errorLineItem in errorLineItems)
            {
                var errorCode = errorLineItem.ErrorCode.ToString();
                var filePosition = $"{errorLineItem.LineItem.FileInfo.Name}:{(errorLineItem.LineItem.LineIndex)} ";
                Console.WriteLine($"{filePosition} {errorCode}: {errorLineItem.ErrorMessage}");
            }
        }
    }
}
