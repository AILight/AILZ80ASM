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
    public partial class Package
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
            var label = new LabelAdr("[NAME_SPACE_DEFAULT]", AssembleLoad);
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
            foreach (var asmORG in this.AssembleLoad.Share.AsmORGs.Where(m => m.IsRomMode).OrderBy(m => m.ProgramAddress))
            {
                try
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

            //
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
