using AILZ80ASM.AILight;
using AILZ80ASM.Assembler;
using AILZ80ASM.Exceptions;
using AILZ80ASM.SuperAssembles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace AILZ80ASM
{
    public partial class Package
    {
        private List<FileItem> FileItems { get; set; } = default;
        private AsmOption AssembleOption { get; set; } = default;
        private const int AssembleStatusLength = 15;
        private const int SuperAssembleStatusLength = AssembleStatusLength + 4;

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
            InitializeAssemble();
        }

        public void InitializeAssemble()
        {
            FileItems = new List<FileItem>();

            var label = new LabelAdr("[NAME_SPACE_DEFAULT]", AssembleLoad);
            AssembleLoad.AddLabel(label);

            foreach (var fileInfo in AssembleOption.InputFiles[AsmEnum.FileTypeEnum.Z80])
            {
                FileItems.Add(new FileItem(fileInfo, AssembleLoad));
            }

            this.AssembleLoad.LoadCloseValidate();
        }

        public void Assemble()
        {
            // 命令を展開する
            Trace.WriteLine($"- Expand".PadRight(AssembleStatusLength) + "(1/6)");
            ExpansionItem();

            // プレアセンブル
            Trace.WriteLine($"- PreAssemble".PadRight(AssembleStatusLength) + "(2/6)");
            PreAssemble();

            // アジャストアセンブル
            Trace.WriteLine($"- AdjAssemble".PadRight(AssembleStatusLength) + "(3/6)");
            AdjustAssemble();

            // アセンブルを行う
            Trace.WriteLine($"- MainAssemble".PadRight(AssembleStatusLength) + "(4/6)");
            InternalAssemble();

            // 必要であればスーパーアセンブルモードを実行
            SuperAssemble();

            Trace.WriteLine($"- Validate".PadRight(AssembleStatusLength) + "(5/6)");
            // 未使用ラベルの値確定
            BuildLabel();

            // 出力アドレスの重複チェック
            ValidateOutputAddress();

            // 完了
            Complete();
            Trace.WriteLine($"- Complete".PadRight(AssembleStatusLength) + "(6/6)");
            Trace.WriteLine($"");
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

            // エントリポイントを確定させる
            if (!this.AssembleLoad.Share.EntryPoint.HasValue)
            {
                if (this.AssembleLoad.Share.AsmORGs.Count >= 2)
                {
                    this.AssembleLoad.Share.EntryPoint = this.AssembleLoad.Share.AsmORGs[1].ProgramAddress;
                }
            }
            if (this.AssembleOption.EntryPoint.HasValue)
            {
                this.AssembleLoad.Share.EntryPoint = this.AssembleOption.EntryPoint;
            }

            // OutputAddressを一時保存します
            this.AssembleLoad.Share.AsmORGs.ForEach(m => m.SaveOutputAddress());

            if (this.AssembleLoad.Share.AsmORGs.Any(m => m.IsRomMode))
            {
                // こちらの処理で、すべてのORGの再配置を処理できる
                // ただし現状では、不具合の発生が怖いのでelse側の旧処理を通常は使うこととする
                // 将来はifの条件をなくして、else側を削除して対応することにする
                AdjustAssembleAddress();
            }
            else
            {
                // 出力アドレスを確定(ROM以外)
                var asmORGs = this.AssembleLoad.Share.AsmORGs.Where(m => !m.IsRomMode && !m.OutputAddress.HasValue).OrderBy(m => m.ProgramAddress).ToList();
                var startORG = asmORGs.FirstOrDefault(m => m.HasBinResult);
                var endORG = asmORGs.LastOrDefault(m => m.HasBinResult);

                if (startORG != null && endORG != null)
                {
                    var outputAddress = default(UInt32);
                    var startIndex = asmORGs.IndexOf(startORG);
                    var endIndex = asmORGs.IndexOf(endORG);
                    for (var index = startIndex; index <= endIndex; index++)
                    {
                        try
                        {
                            asmORGs[index].AdjustAssemble(outputAddress, AssembleLoad);
                            if (index < endIndex)
                            {
                                var offset = (UInt32)(asmORGs[index + 1].ProgramAddress - asmORGs[index].ProgramAddress);
                                outputAddress += offset;
                            }
                        }
                        catch (ErrorAssembleException ex)
                        {
                            AssembleLoad.AddError(new ErrorLineItem(asmORGs[index].LineItem, ex));
                        }
                        catch (ErrorLineItemException ex)
                        {
                            AssembleLoad.AddError(ex.ErrorLineItem);
                        }
                        catch (Exception ex)
                        {
                            AssembleLoad.AddError(new ErrorLineItem(asmORGs[index].LineItem, Error.ErrorCodeEnum.E0000, ex.Message));
                        }
                    }
                }

                // ROM出力調整
                AdjustAssembleForROM();
            }

            // FillByteLabelの確定
            var defaultFillByte = AssembleLoad.Share.GapByte;
            foreach (var asmORG in this.AssembleLoad.Share.AsmORGs.OrderBy(m => m.OutputAddress))
            {
                try
                {
                    if (asmORG.ORGType == AsmORG.ORGTypeEnum.ORG)
                    {
                        defaultFillByte = AssembleLoad.Share.GapByte;
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
                catch (ErrorAssembleException ex)
                {
                    AssembleLoad.AddError(new ErrorLineItem(asmORG.LineItem, ex));
                }
                catch (ErrorLineItemException ex)
                {
                    AssembleLoad.AddError(ex.ErrorLineItem);
                }
                catch (Exception ex)
                {
                    AssembleLoad.AddError(new ErrorLineItem(asmORG.LineItem, Error.ErrorCodeEnum.E0000, ex.Message));
                }
            }
        }

        private void AdjustAssembleAddress()
        {
            var isRomMode = this.AssembleLoad.Share.AsmORGs.Any(m => m.IsRomMode);
            var asmORGItems = this.AssembleLoad.Share.AsmORGs.Select((item, index) => new { Item = item, Index = index }).ToArray();
            var asmORGItemsForRomMode = asmORGItems.Where(m => m.Item.IsRomMode);

            // RomModeのアドレスを確定させる
            foreach (var asmORGItem in asmORGItemsForRomMode)
            {
                try
                {
                    var resultAddress = default(AsmAddress);
                    var outputAddress = default(UInt32);

                    if (asmORGItem.Item.OutputAddress.HasValue)
                    {
                        outputAddress = asmORGItem.Item.OutputAddress.Value;
                    }
                    else if (AIMath.TryParse(asmORGItem.Item.OutputAddressLabel, this.AssembleLoad, resultAddress, out var outputAddressValue))
                    {
                        outputAddress = outputAddressValue.ConvertTo<UInt32>();
                    }
                    else
                    {
                        throw new ErrorAssembleException(Error.ErrorCodeEnum.E0004, asmORGItem.Item.OutputAddressLabel);
                    }
                    asmORGItem.Item.AdjustAssemble(outputAddress, AssembleLoad);
                }
                catch (ErrorAssembleException ex)
                {
                    AssembleLoad.AddError(new ErrorLineItem(asmORGItem.Item.LineItem, ex));
                }
                catch (ErrorLineItemException ex)
                {
                    AssembleLoad.AddError(ex.ErrorLineItem);
                }
                catch (Exception ex)
                {
                    AssembleLoad.AddError(new ErrorLineItem(asmORGItem.Item.LineItem, Error.ErrorCodeEnum.E0000, ex.Message));
                }
            }

            // 通常の出力アドレスを確定する
            {
                var asmORGs = (isRomMode ? asmORGItems.OrderBy(m => m.Index) : asmORGItems.OrderBy(m => m.Item.ProgramAddress)).Select(m => m.Item).ToList();
                var startORG = asmORGs.FirstOrDefault(m => m.OutputAddress.HasValue || m.HasBinResult);
                var endORG = asmORGs.LastOrDefault(m => m.OutputAddress.HasValue || m.HasBinResult);
                if (startORG != null && endORG != null)
                {
                    var outputAddress = default(UInt32);
                    var startIndex = asmORGs.IndexOf(startORG);
                    var endIndex = asmORGs.IndexOf(endORG);
                    for (var index = startIndex; index <= endIndex; index++)
                    {
                        try
                        {
                            outputAddress = asmORGs[index].OutputAddress ?? outputAddress;
                            asmORGs[index].AdjustAssemble(outputAddress, AssembleLoad);
                            if (index < endIndex)
                            {
                                var offset = (UInt32)(asmORGs[index + 1].ProgramAddress - asmORGs[index].ProgramAddress);
                                outputAddress += offset;
                            }
                        }
                        catch (ErrorAssembleException ex)
                        {
                            AssembleLoad.AddError(new ErrorLineItem(asmORGs[index].LineItem, ex));
                        }
                        catch (ErrorLineItemException ex)
                        {
                            AssembleLoad.AddError(ex.ErrorLineItem);
                        }
                        catch (Exception ex)
                        {
                            AssembleLoad.AddError(new ErrorLineItem(asmORGs[index].LineItem, Error.ErrorCodeEnum.E0000, ex.Message));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// ROM調整
        /// </summary>
        private void AdjustAssembleForROM()
        {
            var asmORGs = this.AssembleLoad.Share.AsmORGs.ToArray();
            var resultAddress = default(AsmAddress);

            for (var index = 0; index < asmORGs.Length; index++)
            {
                var asmORG = asmORGs[index];
                try
                {
                    if (!asmORG.IsRomMode || asmORG.OutputAddress.HasValue)
                    {
                        var findIndex = index - 1;
                        while (findIndex >= 0)
                        {
                            var foundAsmORG = asmORGs[findIndex];
                            var lastBinResult = foundAsmORG.LineDetailItems.Where(m => m.LineDetailScopeItems != default).SelectMany(m => m.LineDetailScopeItems.SelectMany(n => n.LineDetailExpansionItems.Select(m => new { m.Address, m.Length }))).OrderByDescending(m => m.Address.Output).FirstOrDefault();
                            if (lastBinResult != default && lastBinResult.Address.Output.HasValue)
                            {
                                resultAddress = new AsmAddress((UInt16)(lastBinResult.Address.Program + lastBinResult.Length.Program), (UInt32)(lastBinResult.Address.Output + lastBinResult.Length.Output));
                                break;
                            }
                            findIndex--;
                        }
                        continue;
                    }
                    if (!AIMath.TryParse(asmORG.OutputAddressLabel, this.AssembleLoad, resultAddress, out var outputAddressValue))
                    {
                        throw new ErrorAssembleException(Error.ErrorCodeEnum.E0004, asmORG.OutputAddressLabel);
                    }
                    var outputAddress = outputAddressValue.ConvertTo<UInt32>();

                    asmORG.AdjustAssemble(outputAddress, AssembleLoad);
                }
                catch (ErrorAssembleException ex)
                {
                    AssembleLoad.AddError(new ErrorLineItem(asmORG.LineItem, ex));
                }
                catch (ErrorLineItemException ex)
                {
                    AssembleLoad.AddError(ex.ErrorLineItem);
                }
                catch (Exception ex)
                {
                    AssembleLoad.AddError(new ErrorLineItem(asmORG.LineItem, Error.ErrorCodeEnum.E0000, ex.Message));
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
        /// スーパーアセンブルを実行する
        /// </summary>
        private void SuperAssemble()
        {
            if (this.AssembleOption.NoSuperAsmAssemble)
            {
                // スーパーアセンブルモードが無効の場合
                return;
            }

            var sp = this.AssembleLoad.Share.AsmSuperAssembleMode;
            // スーパーアセンブルモード
            while (true)
            {
                if (this.AssembleLoad.AssembleErrors.Length == 0)
                {
                    // エラーが無い場合には、スーパーアセンブルモードから抜ける
                    return;
                }

                var superAssembler = sp.GetSuperAssembler(this.AssembleLoad);
                if (superAssembler == default)
                {
                    return;
                }

                // スーパーアセンブルを開始
                sp.StartSuperAssemble();

                // 実際の処理を動かす
                var isLoop = true;
                while (isLoop)
                {
                    // アセンブル処理
                    SuperAssembleInternal(superAssembler);

                    // アセンブルの結果確認処理
                    isLoop = superAssembler.TarminateSuperAssemble(this.AssembleLoad);

                    // スーパーアセンブルを終了
                    sp.EndSuperAssemble();
                }
            }
        }

        /// <summary>
        /// スーパーアセンブルの実際の処理
        /// </summary>
        /// <param name="superAssemble"></param>
        private void SuperAssembleInternal(ISuperAssemble superAssemble)
        {
            // スーパーアセンブルモードの実行
            ReAssembleInitialize();

            Trace.Write($"  - Super Assemble Mode ({superAssemble.Title}:{this.AssembleLoad.Share.AsmSuperAssembleMode.LoopCounter})");

            // 命令を展開する
            Trace.Write($" E");
            ExpansionItem();

            // プレアセンブル
            Trace.Write($"->P");
            PreAssemble();

            // アジャストアセンブル
            Trace.Write($"->A");
            AdjustAssemble();

            // アセンブルを行う
            Trace.WriteLine($"->M");
            InternalAssemble();
        }

        /// <summary>
        /// 再アセンブルを行えるように、初期化します
        /// </summary>
        private void ReAssembleInitialize()
        {
            this.AssembleLoad.ReAssembleInitialize();
            this.InitializeAssemble();
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
            var binResults = FileItems.SelectMany(m => m.BinResults).OrderBy(m => m.Address.Output).ThenBy(m => m.Address.Program);
            if (binResults.Count() == 0)
            {
                // 出力データが無い場合は終了
                return;
            }

            // 余白を調整して出力をする
            foreach (var item in binResults)
            {
                if (!item.Address.Output.HasValue)
                {
                    continue;
                }

                if (item.Address.Output.Value < outputAddress)
                {
                    this.AssembleLoad.AddError(new ErrorLineItem(item.LineItem, Error.ErrorCodeEnum.E0009));
                    break;
                }
                else if (item.Address.Output.Value != outputAddress)
                {
                    outputAddress = item.Address.Output.Value;
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

        public bool SaveOutput(Dictionary<AsmEnum.FileTypeEnum, FileInfo> outputFiles, Dictionary<AsmEnum.FileTypeEnum, FileInfo> failedOutputFiles)
        {
            var result = true;

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
            foreach (var item in failedOutputFiles)
            {
                Trace.WriteLine($"- {item.Key.ToString()} filename [{item.Value.Name}]: Failed");
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
                case AsmEnum.FileTypeEnum.HEX:
                    SaveHEX(stream);
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
                case AsmEnum.FileTypeEnum.TAG:
                    SaveTAG(stream);
                    break;
                default:
                    throw new NotImplementedException($"指定の出力形式は選択できません。{outputFile.Key}");
            }
        }

        /// <summary>
        /// エラー結果の出力
        /// </summary>
        public void OutputError()
        {
            var errors = AssembleLoad.AssembleErrors;
            var warnings = AssembleLoad.AssembleWarnings;
            var informations = AssembleLoad.AssembleInformation;

            if (errors.Length > 0 ||
                warnings.Length > 0 ||
                informations.Length > 0)
            {
                OutputError(errors, "Error");
                OutputError(warnings, "Warning");
                OutputError(informations, "Information");
            }
        }

        /// <summary>
        /// リスティングファイルのエラー情報を出力
        /// </summary>
        public void OutputErrorForList()
        {
            var errors = this.AssembleLoad.Share.AsmLists.Where(m => m.ErrorCode.HasValue && Error.GetErrorType(m.ErrorCode.Value) == Error.ErrorTypeEnum.Error).ToArray();
            var fileName = this.AssembleOption.OutputFiles[AsmEnum.FileTypeEnum.LST].Name;

            if (errors.Any())
            {
                Trace.WriteLine($"# List");
                Trace.WriteLine("");

                OutputErrorForList(errors, fileName);
            }
        }

        /// <summary>
        /// エラーのサマリーを出力
        /// </summary>
        public void OutputErrorSummary()
        {
            var errors = AssembleLoad.AssembleErrors;
            var warnings = AssembleLoad.AssembleWarnings;
            var informations = AssembleLoad.AssembleInformation;

            Trace.WriteLine($"{errors.Length:0} error(s), {warnings.Length} warning(s), {informations.Length} information");
            Trace.WriteLine($"");
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
                Trace.WriteLine("");
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
            foreach (var errorLineItem in errorLineItems.Distinct().OrderBy(m => m?.LineItem?.FileInfo?.Name).ThenBy(m => m?.LineItem?.LineIndex))
            {
                var errorCode = errorLineItem.ErrorCode.ToString();
                var filePosition = $"{errorLineItem.LineItem.FileInfo.Name}:{(errorLineItem.LineItem.LineIndex)} ";
                Trace.WriteLine($"{filePosition} {errorCode}: {errorLineItem.ErrorMessage}");
            }
        }

        /// <summary>
        /// リストのエラーを表示する
        /// </summary>
        /// <param name="asmList"></param>
        /// <param name="fileName"></param>
        public static void OutputErrorForList(AsmList[] asmLists, string fileName)
        {
            if (asmLists.Length > 0)
            {
                InternalOutputErrorForList(asmLists, fileName);
                Trace.WriteLine("");
            }
        }

        /// <summary>
        /// リストのエラーの詳細を表示する
        /// </summary>
        /// <param name="asmLists"></param>
        /// <param name="fileName"></param>
        private static void InternalOutputErrorForList(AsmList[] asmLists, string fileName)
        {
            foreach (var asmList in asmLists.Distinct())
            {
                var errorCode = asmList.ErrorCode.ToString();
                var filePosition = $"{fileName}:{(asmList.OutputLineIndex)} ";
                Trace.WriteLine($"{filePosition} {errorCode}: {asmList.ErrorMessage}");
            }
        }
    }
}
