using AILZ80ASM.AILight;
using AILZ80ASM.Assembler;
using AILZ80ASM.Exceptions;
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
            var label = new LabelAdr("[NS_Main]", AssembleLoad);
            AssembleLoad.AddLabel(label);

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

            // プレアセンブル
            PreAssemble();

            // アジャストアセンブル
            AdjustAssemble();

            // アセンブルを行う
            InternalAssemble();

            // 未使用ラベルの値確定
            BuildLabel();

            // 出力アドレスの重複チェック
            ValidateOutputAddress();

            // 完了
            Complete();
        }

        /// <summary>
        /// プレアセンブル
        /// </summary>
        private void PreAssemble()
        {
            this.AssembleLoad.Share.AsmStep = AsmLoadShare.AsmStepEnum.PreAssemble;

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
            this.AssembleLoad.Share.AsmStep = AsmLoadShare.AsmStepEnum.ExpansionItem;

            foreach (var fileItem in FileItems)
            {
                fileItem.ExpansionItem();
            }
        }

        /// <summary>
        /// アジャストアセンブル（出力アドレスの調整）
        /// </summary>
        private void AdjustAssemble()
        {
            this.AssembleLoad.Share.AsmStep = AsmLoadShare.AsmStepEnum.AdjustAssemble;

            // 出力アドレスを確定(ROM以外)
            var asmORGs = this.AssembleLoad.Share.AsmORGs.Where(m => !m.IsRomMode).OrderBy(m => m.ProgramAddress).ToList();
            var startORG = asmORGs.FirstOrDefault(m => m.HasBinResult);
            var endORG = asmORGs.LastOrDefault(m => m.HasBinResult);

            if (startORG != null && endORG != null)
            {
                var outputAddress = default(UInt32);
                var startIndex = asmORGs.IndexOf(startORG);
                var endIndex = asmORGs.IndexOf(endORG);
                for (var index = startIndex; index <= endIndex; index++)
                {
                    asmORGs[index].AdjustAssemble(outputAddress);
                    if (index < endIndex)
                    {
                        var offset = (UInt32)(asmORGs[index + 1].ProgramAddress - asmORGs[index].ProgramAddress);
                        outputAddress += offset;
                    }
                }
            }

            // ROM出力調整
            foreach (var asmORG in this.AssembleLoad.Share.AsmORGs.Where(m => m.IsRomMode).OrderBy(m => m.ProgramAddress))
            {
                var outputAddress = default(UInt32);

                if (!AIMath.TryParse(asmORG.OutputAddressLabel, this.AssembleLoad, out var outputAddressValue))
                {
                    // 最後のアドレスを取得して再計算する
                    var foundAsmORG = this.AssembleLoad.Share.AsmORGs.Where(m => m != asmORG && m.HasBinResult && m.ProgramAddress <= asmORG.ProgramAddress).LastOrDefault();
                    var resultAddress = new AsmAddress();
                    if (foundAsmORG != default)
                    {
                        var lastBinResult = foundAsmORG.LineDetailItems.Where(m => m.LineDetailScopeItems != default).SelectMany(m => m.LineDetailScopeItems.SelectMany(n => n.LineDetailExpansionItems.Select(m => new { m.Address, m.Length }))).OrderByDescending(m => m.Address.Output).FirstOrDefault();
                        resultAddress = new AsmAddress((UInt16)(lastBinResult.Address.Program + lastBinResult.Length.Program), (UInt32)(lastBinResult.Address.Output + lastBinResult.Length.Output));
                    }
                    if (!AIMath.TryParse(asmORG.OutputAddressLabel, this.AssembleLoad, resultAddress, out outputAddressValue))
                    {
                        throw new ErrorAssembleException(Error.ErrorCodeEnum.E0004, asmORG.OutputAddressLabel);
                    }
                }
                outputAddress = outputAddressValue.ConvertTo<UInt32>();

                asmORG.AdjustAssemble(outputAddress);
            }

            //
            var defaultFillByte = default(byte);
            foreach (var asmORG in this.AssembleLoad.Share.AsmORGs.OrderBy(m => m.OutputAddress))
            {
                if (asmORG.ORGType == AsmORG.ORGTypeEnum.ORG)
                {
                    defaultFillByte = 0;
                }

                var fillByte = defaultFillByte;
                if (!string.IsNullOrEmpty(asmORG.FillByteLabel) && !(AIMath.TryParse(asmORG.FillByteLabel, this.AssembleLoad, out var aiValueFillByte) && aiValueFillByte.TryParse(out fillByte)))
                {
                    throw new ErrorAssembleException(Error.ErrorCodeEnum.E0004, asmORG.FillByteLabel);
                }
                asmORG.FillByte = fillByte;

                if (asmORG.ORGType == AsmORG.ORGTypeEnum.ORG)
                {
                    defaultFillByte = fillByte;
                }

            }
        }


        /// <summary>
        /// アセンブルを実行する
        /// </summary>
        private void InternalAssemble()
        {
            this.AssembleLoad.Share.AsmStep = AsmLoadShare.AsmStepEnum.InternalAssemble;

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
            this.AssembleLoad.Share.AsmStep = AsmLoadShare.AsmStepEnum.BuildLabel;

            AssembleLoad.BuildLabel();
        }

        /// <summary>
        /// 出力アドレスの重複チェック
        /// </summary>
        private void ValidateOutputAddress()
        {
            this.AssembleLoad.Share.AsmStep = AsmLoadShare.AsmStepEnum.ValidateOutputAddress;

            if (this.Errors.Length > 0)
            {
                // アセンブルエラーが発生していてたら確認は行わない
                return;
            }

            var outputAddress = default(UInt32);
            var binResults = FileItems.SelectMany(m => m.BinResults).OrderBy(m => m.Address.Output);
            if (binResults.Count() == 0)
            {
                // 出力データが無い場合は終了
                return;
            }

            // 余白を調整して出力をする
            foreach (var item in binResults)
            {
                if (item.Address.Output < outputAddress)
                {
                    this.AssembleLoad.AddError(new ErrorLineItem(item.LineItem, Error.ErrorCodeEnum.E0009));
                    break;
                }
                else if (item.Address.Output != outputAddress)
                {
                    outputAddress = item.Address.Output;
                }

                // 通常出力
                if (item.Data.Length > 0)
                {
                    outputAddress += (UInt32)item.Data.Length;
                }
            }
        }

        private void Complete()
        {
            this.AssembleLoad.Share.AsmStep = AsmLoadShare.AsmStepEnum.Complete;
        }

        public void Trace_Information()
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

            if (AssembleOption.DiffFile)
            {
                Trace.WriteLine("# 出力ファイル差分確認モード");
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
                case AsmEnum.FileTypeEnum.EQU:
                    SaveEQU(stream);
                    break;
                case AsmEnum.FileTypeEnum.ADR:
                    SaveADR(stream);
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
            Trace.WriteLine("");
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
                var originals = AILight.AIEncode.GetString(original).Replace("\r", "").Split('\n');
                var assembleds = AILight.AIEncode.GetString(assembled).Replace("\r", "").Split('\n');
                if (originals.Length != assembleds.Length)
                {
                    resultString = $"不一致 {originals.Length:0} -> {assembleds.Length:0} 行数";
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
                            if (byteDiffCounter < 100)
                            {
                                tmpResultStream += $"{Environment.NewLine}{index + 1:#0}: {originals[index]}";
                                tmpResultStream += $"{Environment.NewLine}{index + 1:#0}: {assembleds[index]}";
                            }
                            byteDiffCounter++;
                        }
                    }
                    if (byteDiffCounter > 0)
                    {
                        resultString = $"不一致 ( {byteDiffCounter:0}件 / 全体:{originals.Length:0}行 )" + tmpResultStream;
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
                    resultString = $"不一致 {original.Length:0} -> {assembled.Length:0} bytes";
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
                            if (byteDiffCounter < 100)
                            {
                                tmpResultStream += $"{Environment.NewLine}{index:X6}: {original[index]:X2} -> {assembled[index]:X2}";
                            }
                            byteDiffCounter++;
                        }
                    }
                    if (byteDiffCounter > 0)
                    {
                        resultString = $"不一致 ( {byteDiffCounter:0}件 / 全体:{original.Length:0} bytes )" + tmpResultStream;
                        resultString += $"{Environment.NewLine}";
                    }
                }
            }

            Trace.WriteLine(resultString);
        }

        public void SaveBin(Stream stream)
        {
            var outputAddress = default(UInt32);
            var binResults = FileItems.SelectMany(m => m.BinResults).OrderBy(m => m.Address.Output);

            // 余白を調整して出力をする
            foreach (var item in binResults)
            {
                if (item.Address.Output < outputAddress)
                {
                    throw new Exception("出力先アドレスが重複したため、BINファイルの出力に失敗ました");
                }
                else if (item.Address.Output != outputAddress)
                {
                    // 余白調整
                    this.AssembleLoad.OutputORGSpace(ref outputAddress, item.Address.Output, stream);
                }

                // 通常出力
                if (item.Data.Length > 0)
                {
                    stream.Write(item.Data, 0, item.Data.Length);
                    outputAddress += (UInt32)item.Data.Length;
                }
            }
        }

        public void SaveT88(Stream stream, string outputFilename)
        {
            using var memoryStream = new MemoryStream();
            SaveBin(memoryStream);

            var address = default(UInt16);
            if (AssembleLoad.Share.AsmORGs.Count >= 2)
            {
                address = AssembleLoad.Share.AsmORGs.Skip(1).First().ProgramAddress;
            }

            var binaryWriter = new IO.T88BinaryWriter(outputFilename, address, memoryStream.ToArray(), stream);
            binaryWriter.Write();
        }

        public void SaveCMT(Stream stream)
        {
            using var memoryStream = new MemoryStream();
            SaveBin(memoryStream);

            var address = default(UInt16);
            if (AssembleLoad.Share.AsmORGs.Count >= 2)
            {
                address = AssembleLoad.Share.AsmORGs.Skip(1).First().ProgramAddress;
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

            var title = $";{ProductInfo.ProductLongName}, SYM";
            streamWriter.WriteLine(title);

            AssembleLoad.OutputLabels(streamWriter);

            streamWriter.Flush();
            memoryStream.Position = 0;
            memoryStream.CopyTo(stream);
        }

        public void SaveEQU(FileInfo equal)
        {
            using var fileStream = equal.OpenWrite();

            SaveEQU(fileStream);

            fileStream.Close();
        }

        public void SaveEQU(Stream stream)
        {
            using var memoryStream = new MemoryStream();
            using var streamWriter = new StreamWriter(memoryStream, AsmLoad.GetEncoding(AssembleLoad.AssembleOption.DecidedOutputEncodeMode));

            var title = $";{ProductInfo.ProductLongName}, EQU";
            streamWriter.WriteLine(title);

            AssembleLoad.OutputEqualLabels(streamWriter);

            streamWriter.Flush();
            memoryStream.Position = 0;
            memoryStream.CopyTo(stream);
        }

        public void SaveADR(FileInfo equal)
        {
            using var fileStream = equal.OpenWrite();

            SaveADR(fileStream);

            fileStream.Close();
        }

        public void SaveADR(Stream stream)
        {
            using var memoryStream = new MemoryStream();
            using var streamWriter = new StreamWriter(memoryStream, AsmLoad.GetEncoding(AssembleLoad.AssembleOption.DecidedOutputEncodeMode));

            var title = $";{ProductInfo.ProductLongName}, ADR";
            streamWriter.WriteLine(title);

            AssembleLoad.OutputAddrLabels(streamWriter);

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

            var title = $";{ProductInfo.ProductLongName}, LST:{AssembleLoad.AssembleOption.ListMode}:{AssembleLoad.AssembleOption.TabSize}";
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
                Trace.WriteLine($"# {title}");
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
