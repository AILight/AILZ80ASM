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
        private AsmOption AssembleOption { get; set; } = default;

        public AsmLoad AssembleLoad { get; private set; }
        public ErrorLineItem[] Errors => AssembleLoad.AssembleErrors;
        public ErrorLineItem[] Warnings => AssembleLoad.AssembleWarnings;
        public ErrorLineItem[] Information => AssembleLoad.AssembleInformation;

        public Package(AsmOption asmOption, AsmISA asmISA)
        {
            AssembleOption = asmOption;

            switch (asmISA)
            {
                case AsmISA.Z80:
                    AssembleLoad = new AsmLoad(asmOption, new InstructionSet.Z80());
                    break;
                default:
                    throw new NotImplementedException();
            }
            var label = new Label("[NS_Main]", AssembleLoad);
            AssembleLoad.AddLabel(label);
            AssembleLoad.DefaultCharMap = "@SJIS";

            // CharMapの初期化;
            CharMaps.CharMapConverter.ReadCharMapFromResource(AssembleLoad.DefaultCharMap, AssembleLoad);

            foreach (var fileInfo in asmOption.InputFiles[AsmEnum.FileTypeEnum.Z80])
            {
                FileItems.Add(new FileItem(fileInfo, AssembleLoad));
            }

            this.AssembleLoad.LoadCloseValidate();
        }

        public void Assemble()
        {
            // 命令を展開する
            ExpansionItem();

            //var asmAddress = new AsmAddress();
            //AssembleLoad.PreAssemble(ref asmAddress);
            /*
            // ORGを確定
            BuildORG();
            */

            // プレアセンブル
            PreAssemble();

            // アセンブルを行う
            InternalAssemble();

            // 未使用ラベルの値確定
            BuildLabel();

            // 出力アドレスの重複チェック
            ValidateOutputAddress();
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
        /// 未確定ラベルを確定させる
        /// </summary>
        private void BuildLabel()
        {
            AssembleLoad.BuildLabel();
        }

        /// <summary>
        /// 出力アドレスの重複チェック
        /// </summary>
        private void ValidateOutputAddress()
        {
            if (this.Errors.Length > 0)
            {
                // アセンブルエラーが発生していてたら確認は行わない
                return;
            }

            var binResult = FileItems.SelectMany(m => m.BinResult).OrderBy(m => m.Address.Output);
            if (binResult.Count() == 0)
            {
                // 出力データが無い場合は終了
                return;
            }

            //var startResult = binResult.OrderByDescending(m => m.Address.Output).First();
            var endResult = binResult.OrderByDescending(m => m.Address.Output).ThenByDescending(m => m.Data.Length).First();
            //var startAddress = startResult.Address.Output;
            var endAddress = endResult.Address.Output + endResult.Data.Length;

            var outputPoints = new int[endAddress];
            foreach (var item in binResult)
            {
                for (var index = 0; index < item.Data.Length; index++)
                {
                    var address = item.Address.Output + index;
                    if (outputPoints[address] > 0)
                    {
                        // OutputAddressの衝突
                        var lineDetailItemAddress = this.AssembleLoad.FindLineDetailItemAddress((UInt32)address);
                        if (lineDetailItemAddress == default)
                        {
                            throw new Exception("出力アドレスで重複がありましたが、それを指定するORGが見つかりませんでした。");
                        }

                        this.AssembleLoad.AddError(new ErrorLineItem(lineDetailItemAddress.LineItem, Error.ErrorCodeEnum.E0009));

                        return;
                    }
                    outputPoints[address]++;
                }
            }

        }

        public void Trace_Information()
        {
            if (AssembleOption.FileDiff)
            {
                Trace.WriteLine("出力ファイル差分確認モード");
                Trace.WriteLine("");
            }
            else
            {
                Trace.WriteLine($"# Inputs");
                Trace.WriteLine("");

                foreach (var item in AssembleOption.InputFiles)
                {
                    foreach (var fileInfo in item.Value)
                    {
                        Trace.WriteLine($"- {item.Key.ToString()} filename [{fileInfo.Name}]");
                    }
                }
                Trace.WriteLine("");
            }
        }

        public bool SaveOutput(Dictionary<AsmEnum.FileTypeEnum, FileInfo> outputFiles)
        {
            var result = true;

            Trace.WriteLine($"# Outputs");
            Trace.WriteLine("");

            foreach (var item in outputFiles)
            {
                var status = "";
                try
                {

                    item.Value.Delete();
                    using var fileStream = item.Value.OpenWrite();

                    SaveOutput(fileStream, item);

                    fileStream.Close();

                    status = "Successful.";
                }
                catch (Exception ex)
                {
                    status = ex.Message;
                    result = false;
                }

                Trace.WriteLine($"- {item.Key.ToString()} filename [{item.Value.Name}]: {status}");
            }
            Trace.WriteLine($"");

            return result;
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

        public bool DiffOutput(Dictionary<AsmEnum.FileTypeEnum, FileInfo> outputFiles)
        {
            var result = true;
            foreach (var item in outputFiles)
            {
                if (!item.Value.Exists)
                {
                    Trace.WriteLine($"{item.Value.Name}: 不一致 ファイルが見つかりません。");
                    continue;
                }

                using var fileStream = item.Value.OpenRead();

                try
                {
                    DiffOutput(fileStream, item);
                }
                catch (Exception ex)
                {
                    result = false;
                    Trace.TraceError(ex.Message);
                }
                fileStream.Close();
            }
            return result;
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
            var asmAddress = new AsmAddress();
            var binResult = FileItems.SelectMany(m => m.BinResult).OrderBy(m => m.Address.Output);

            foreach (var item in binResult)
            {
                if (item.Address.Output > asmAddress.Output)
                {
                    var asmORGs = this.AssembleLoad.FindAsmORGs(asmAddress.Output, item.Address.Output);
                    var outputAddress = asmAddress.Output;
                    for (var index = 0; index < asmORGs.Length; index++)
                    {
                        var startAsmORG = asmORGs[index];
                        var endAsmORG = index < asmORGs.Length - 1 ? asmORGs[index + 1] : new AsmORG(UInt16.MaxValue, UInt32.MaxValue, default(byte), AsmORG.ORGTypeEnum.ORG);

                        var startOutputAddress = startAsmORG.OutputAddress < outputAddress ? outputAddress : startAsmORG.OutputAddress;
                        var endOutputAddress = endAsmORG.OutputAddress > item.Address.Output ? item.Address.Output : endAsmORG.OutputAddress;

                        var length = endOutputAddress - startOutputAddress;
                        var bytes = Enumerable.Repeat<byte>(startAsmORG.FillByte, (int)length).ToArray();

                        stream.Write(bytes, 0, bytes.Length);
                        outputAddress += length;
                        asmAddress.Output += length;
                    }
                }
                if (item.Data.Length > 0)
                {
                    if (item.Address.Output != asmAddress.Output)
                    {
                        throw new Exception("出力先アドレスが重複したため、BINファイルの出力に失敗ました");
                    }

                    stream.Write(item.Data, 0, item.Data.Length);
                    asmAddress.Program = (UInt16)(item.Address.Program + item.Data.Length);
                    asmAddress.Output = (UInt32)(asmAddress.Output + item.Data.Length);
                }
            }

            // 残りのデータ出力に対応
            var remainingAsmORGs = this.AssembleLoad.FindRemainingAsmORGs(asmAddress.Output);
            for (var index = 1; index < remainingAsmORGs.Length; index++)
            {
                var startAsmORG = remainingAsmORGs[index - 1];
                var endAsmORG = remainingAsmORGs[index];

                var startOutputAddress = startAsmORG.OutputAddress < asmAddress.Output ? asmAddress.Output : startAsmORG.OutputAddress;
                var endOutputAddress = endAsmORG.OutputAddress;

                var length = endOutputAddress - startOutputAddress;
                var bytes = Enumerable.Repeat<byte>(startAsmORG.FillByte, (int)length).ToArray();

                stream.Write(bytes, 0, bytes.Length);
                asmAddress.Output += length;
            }

        }

        public void SaveT88(Stream stream, string outputFilename)
        {
            using var memoryStream = new MemoryStream();
            SaveBin(memoryStream);

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
            SaveBin(memoryStream);

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
            using var memoryStream = new MemoryStream();
            using var streamWriter = new StreamWriter(memoryStream, AsmLoad.GetEncoding(AssembleLoad.AssembleOption.DecidedOutputEncodeMode));

            var title = $"{ProductInfo.ProductLongName}, SYM";
            streamWriter.WriteLine(title);

            AssembleLoad.OutputLabels(streamWriter);

            streamWriter.Flush();
            memoryStream.Position = 0;
            memoryStream.CopyTo(stream);
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
            using var streamWriter = new StreamWriter(memoryStream, AsmLoad.GetEncoding(AssembleLoad.AssembleOption.DecidedOutputEncodeMode));

            var title = $"{ProductInfo.ProductLongName}, LST:{AssembleLoad.AssembleOption.ListMode}:{AssembleLoad.AssembleOption.TabSize}";
            streamWriter.WriteLine(AsmList.CreateSource(title).ToString(AssembleLoad.AssembleOption.ListMode, AssembleLoad.AssembleOption.TabSize));

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
            Trace.WriteLine($"# Status");

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
