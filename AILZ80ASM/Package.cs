using AILZ80ASM.Assembler;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace AILZ80ASM
{
    public class Package
    {
        private List<FileItem> FileItems { get; set; } = new List<FileItem>();
        public AsmLoad AssembleLoad { get; private set; }
        public ErrorLineItem[] Errors => AssembleLoad.AssembleErrors;
        public ErrorLineItem[] Warnings => AssembleLoad.AssembleWarnings;
        public ErrorLineItem[] Information => AssembleLoad.AssembleInformation;

        public Package(FileInfo[] files, AsmEnum.EncodeModeEnum encodeMode, AsmEnum.ListFormatEnum listMode, bool outputTrim, Error.ErrorCodeEnum[] disableWarningCodes, AsmISA asmISA)
        {
            switch (asmISA)
            {
                case AsmISA.Z80:
                    AssembleLoad = new AsmLoad(new InstructionSet.Z80());
                    break;
                default:
                    throw new NotImplementedException();
            }
            var label = new Label("[NS_Main]", AssembleLoad);
            AssembleLoad.AddLabel(label);
            AssembleLoad.InputEncodeMode = encodeMode;
            AssembleLoad.ListMode = listMode;

            AssembleLoad.OutputTrim = outputTrim;
            AssembleLoad.DisableWarningCodes = disableWarningCodes;
            AssembleLoad.CharMap = "@SJIS";

            // CharMapの初期化;
            CharMaps.CharMapConverter.ReadCharMapFromResource(AssembleLoad.CharMap, AssembleLoad);

            foreach (var fileInfo in files)
            {
                FileItems.Add(new FileItem(fileInfo, AssembleLoad));
            }

            this.AssembleLoad.LoadCloseValidate();
        }

        public void Assemble()
        {
            // 命令を展開する
            ExpansionItem();

            // プレアセンブル
            PreAssemble();

            // アセンブルを行う
            InternalAssemble();

            // データのトリムを行う
            TrimData();
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
        /// アセンブルを実行する
        /// </summary>
        private void InternalAssemble()
        {
            foreach (var fileItem in FileItems)
            {
                fileItem.Assemble();
            }
        }

        /// <summary>
        /// データトリムする
        /// </summary>
        private void TrimData()
        {
            foreach (var operationItem in AssembleLoad.TirmOperationItems)
            {
                operationItem.TrimData();
            }
        }

        public void SaveOutput(Dictionary<AsmEnum.FileTypeEnum, FileInfo> outputFiles)
        {
            foreach (var item in outputFiles)
            {
                item.Value.Delete();
                using var fileStream = item.Value.OpenWrite();

                SaveOutput(fileStream, item);

                fileStream.Close();
            }

        }

        public void SaveOutput(Stream stream, KeyValuePair<AsmEnum.FileTypeEnum, FileInfo> outputFile)
        {
            switch (outputFile.Key)
            {
                case AsmEnum.FileTypeEnum.BIN:
                    SaveBin(stream);
                    break;
                case AsmEnum.FileTypeEnum.T88:
                    SaveT88(stream, outputFile.Value.Name);
                    break;
                case AsmEnum.FileTypeEnum.CMT:
                    SaveCMT(stream);
                    break;
                case AsmEnum.FileTypeEnum.LST:
                    SaveLST(stream);
                    break;
                case AsmEnum.FileTypeEnum.SYM:
                    SaveSYM(stream);
                    break;
                default:
                    throw new NotImplementedException($"指定の出力形式は選択できません。{outputFile.Key}");
            }
        }

        public void DiffOutput(Dictionary<AsmEnum.FileTypeEnum, FileInfo> outputFiles)
        {
            foreach (var item in outputFiles)
            {
                using var fileStream = item.Value.OpenRead();

                try
                {
                    DiffOutput(fileStream, item);
                }
                catch (Exception ex)
                {
                    Trace.TraceError(ex.Message);
                }
                fileStream.Close();
            }
        }

        public void DiffOutput(Stream stream, KeyValuePair<AsmEnum.FileTypeEnum, FileInfo> outputFile)
        {
            using var assembledStream = new MemoryStream();
            using var originalStream = new MemoryStream();
            stream.CopyTo(originalStream);

            SaveOutput(assembledStream, outputFile);
            DiffOutput(originalStream.ToArray(), assembledStream.ToArray(), outputFile);
        }

        public void DiffOutput(byte[] original, byte[] assembled, KeyValuePair<AsmEnum.FileTypeEnum, FileInfo> outputFile)
        {
            var resultString = "一致";

            Trace.Write($"{outputFile.Value.Name}: ");

            if (outputFile.Key == AsmEnum.FileTypeEnum.LST)
            {
                // テキスト比較
                var originals = AILight.AIEncode.GetString(original).Replace("\r","").Split('\n');
                var assembleds = AILight.AIEncode.GetString(assembled).Replace("\r", "").Split('\n');
                if (original.Length != assembled.Length)
                {
                    resultString = $"不一致 {originals.Length:#,##0} -> {assembleds.Length:#,##0}行数";
                    resultString += $"{Environment.NewLine}";
                }
                else
                {
                    var byteDiffCounter = 0;
                    var tmpResultStream = "";

                    for (var index = 0; index < originals.Length && index < assembleds.Length; index++)
                    {
                        if (originals[index] != assembleds[index])
                        {
                            if (byteDiffCounter < 20)
                            {
                                tmpResultStream += $"{Environment.NewLine}{index+1:#0}: {originals[index]}";
                                tmpResultStream += $"{Environment.NewLine}{index+1:#0}: {assembleds[index]}";
                            }
                            byteDiffCounter++;
                        }
                    }
                    if (byteDiffCounter > 0)
                    {
                        resultString = $"不一致 ({byteDiffCounter:#,##0}/{originals.Length:#,##0})" + tmpResultStream;
                        resultString += $"{Environment.NewLine}";
                    }
                }


                AILight.AIEncode.IsSHIFT_JIS(assembled);

            }
            else
            {
                // バイナリー比較
                if (original.Length != assembled.Length)
                {
                    resultString = $"不一致 {original.Length:#,##0} -> {assembled.Length:#,##0} bytes";
                    resultString += $"{Environment.NewLine}";
                }
                else
                {
                    var byteDiffCounter = 0;
                    var tmpResultStream = "";
                    for (var index = 0; index < original.Length; index++)
                    {
                        if (original[index] != assembled[index])
                        {
                            if (byteDiffCounter < 20)
                            {
                                tmpResultStream += $"{Environment.NewLine}{index:X6}: {original[index]:X} -> {assembled[index]:X}";
                            }
                            byteDiffCounter++;
                        }
                    }
                    if (byteDiffCounter > 0)
                    {
                        resultString = $"不一致 ({byteDiffCounter:#,##0}/{original.Length:#,##0})" + tmpResultStream;
                        resultString += $"{Environment.NewLine}";
                    }
                }
            }

            Trace.WriteLine(resultString);
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

        public void SaveSYM(FileInfo symbol)
        {
            using var fileStream = symbol.OpenWrite();

            SaveSYM(fileStream);

            fileStream.Close();
        }

        public void SaveSYM(Stream stream)
        {
            AssembleLoad.OutputLabels(stream);
        }

        public void SaveLST(FileInfo list)
        {
            using var fileStream = list.OpenWrite();

            SaveLST(fileStream);

            fileStream.Close();
        }

        public void SaveLST(Stream stream)
        {
            using var memoryStream = new MemoryStream();
            using var streamWriter = new StreamWriter(memoryStream);

            streamWriter.WriteLine(AsmList.CreateSource($"{ProductInfo.ProductLongName}").ToString(AssembleLoad.ListMode));

            foreach (var item in FileItems)
            {
                item.SaveList(streamWriter);
            }
            streamWriter.Flush();
            memoryStream.Position = 0;
            memoryStream.CopyTo(stream);
        }

        public void OutputError()
        {
            var errors = AssembleLoad.AssembleErrors;
            var warnings = AssembleLoad.AssembleWarnings;
            var informations = AssembleLoad.AssembleInformation;

            if (errors.Length > 0 ||
                warnings.Length > 0 ||
                informations.Length > 0)
            {
                Trace.WriteLine($"");
                OutputError(errors, "Error");
                OutputError(warnings, "Warning");
                OutputError(informations, "Information");
            }

            Trace.WriteLine($"");
            Trace.WriteLine($" {errors.Length:0} error(s), {warnings.Length} warning(s), {informations.Length} information");
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
                Trace.WriteLine($"> {title}");
                InternalOutputError(errorLineItems, title);
                Trace.WriteLine("");
            }
        }

        /// <summary>
        /// エラーの詳細を表示
        /// </summary>
        /// <param name="errorLineItems"></param>
        private static void InternalOutputError(ErrorLineItem[] errorLineItems, string title)
        {
            foreach (var errorLineItem in errorLineItems.Distinct())
            {
                var errorCode = errorLineItem.ErrorCode.ToString();
                var filePosition = $"{errorLineItem.LineItem.FileInfo.Name}:{(errorLineItem.LineItem.LineIndex)} ";
                Trace.WriteLine($"{filePosition} {errorCode}: {errorLineItem.ErrorMessage}");
            }
        }
    }
}
