using AILZ80ASM.AILight;
using AILZ80ASM.Assembler;
using AILZ80ASM.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using static AILZ80ASM.Assembler.Error;
using static AILZ80ASM.LineDetailItems.LineDetailItemPreProcConditional;

namespace AILZ80ASM.LineDetailItems
{
    public class LineDetailItemAddressAlignBlock : LineDetailItemAddressAlign
    {
        private static readonly string RegexPatternALIGN_Arg1 = @"^(?<op1>(ALIGN))\s+(?<arg1>[^,\s]+)\s+BLOCK$";
        private static readonly string RegexPatternALIGN_Arg1_2 = @"^(?<op1>(ALIGN))\s+(?<arg1>[^,\s]+)\s*,\s*(?<arg2>[^,\s]*)\s+BLOCK$";
        private static readonly string RegexPatternALIGNEnd = @"^\s*ENDM\s*$";

        private readonly List<LineDetailItemAddressAlignBlock> AlignLines = new List<LineDetailItemAddressAlignBlock>();
        private int AlignNestedCount { get; set; } = 0;
        private bool EnableAssemble { get; set; } = false;

        private LineDetailItemAddressAlignBlock(LineItem lineItem, AsmLoad asmLoad)
            : base(lineItem, asmLoad)
        {

        }

        private LineDetailItemAddressAlignBlock(LineItem lineItem, string alignLabel, string fillByteLabel, AsmLoad asmLoad)
            : base(lineItem, alignLabel, fillByteLabel, asmLoad)
        {
        }

        public new static LineDetailItemAddressAlignBlock Create(LineItem lineItem, AsmLoad asmLoad)
        {
            if (!lineItem.IsCollectOperationString)
            {
                return default(LineDetailItemAddressAlignBlock);
            }

            var startMatched_Arg1 = Regex.Match(lineItem.OperationString, RegexPatternALIGN_Arg1, RegexOptions.Singleline | RegexOptions.IgnoreCase);
            var startMatched_Arg1_2 = Regex.Match(lineItem.OperationString, RegexPatternALIGN_Arg1_2, RegexOptions.Singleline | RegexOptions.IgnoreCase);
            var endMatched = Regex.Match(lineItem.OperationString, RegexPatternALIGNEnd, RegexOptions.Singleline | RegexOptions.IgnoreCase);

            // 条件処理処理中
            if (asmLoad.Share.LineDetailItemForExpandItem is LineDetailItemAddressAlignBlock asmLoad_LineDetailItemAddressAlignBlock)
            {
                var lineDetailItemAddressAlignBlock = new LineDetailItemAddressAlignBlock(lineItem, asmLoad);
                // 終了条件チェック
                if (endMatched.Success)
                {
                    asmLoad_LineDetailItemAddressAlignBlock.AlignNestedCount--;

                    // 条件処理が終了
                    if (asmLoad_LineDetailItemAddressAlignBlock.AlignNestedCount == 0)
                    {
                        asmLoad.Share.LineDetailItemForExpandItem = default;
                        return lineDetailItemAddressAlignBlock;
                    }
                }

                // 開始条件
                if (startMatched_Arg1.Success || startMatched_Arg1_2.Success)
                {
                    asmLoad_LineDetailItemAddressAlignBlock.AlignNestedCount++;
                }
                asmLoad_LineDetailItemAddressAlignBlock.AlignLines.Add(lineDetailItemAddressAlignBlock);

                return lineDetailItemAddressAlignBlock;
            }
            else
            {
                // 終了条件チェック
                if (endMatched.Success)
                {
                    // 基本的にはこの処理は動かない
                    throw new ErrorAssembleException(Error.ErrorCodeEnum.E5001);
                }

                // 開始条件チェック
                if (startMatched_Arg1.Success)
                {
                    var arg1 = startMatched_Arg1.Groups["arg1"].Value;
                    var lineDetailItemAddressAlignBlock = new LineDetailItemAddressAlignBlock(lineItem, arg1, "", asmLoad)
                    {
                        AlignNestedCount = 1
                    };
                    asmLoad.Share.LineDetailItemForExpandItem = lineDetailItemAddressAlignBlock;

                    return lineDetailItemAddressAlignBlock;
                }

                if (startMatched_Arg1_2.Success)
                {
                    var arg1 = startMatched_Arg1.Groups["arg1"].Value;
                    var arg2 = startMatched_Arg1.Groups["arg2"].Value;
                    var lineDetailItemAddressAlignBlock = new LineDetailItemAddressAlignBlock(lineItem, arg1, arg2, asmLoad)
                    {
                        AlignNestedCount = 1
                    };
                    asmLoad.Share.LineDetailItemForExpandItem = lineDetailItemAddressAlignBlock;

                    return lineDetailItemAddressAlignBlock;
                }
            }

            return default(LineDetailItemAddressAlignBlock);
        }

        public override void Assemble()
        {
        }

        public override void PreAssemble(ref AsmAddress asmAddress)
        {
            if (AsmLoad.Share.LineDetailItemForExpandItem == this)
            {
                AsmLoad.AddError(new ErrorLineItem(LineItem, ErrorCodeEnum.E5001));
                return;
            }

            if (EnableAssemble)
            {
                base.PreAssemble(ref asmAddress);
            }
            else
            {
                if (AlignLines.Any())
                {
                    // 境界オーバーしているか確認をする処理へ追加
                    AsmLoad.AddValidateAssembles(this);

                    // プレアセンブルを実行する (ALIGN計算)
                    base.PreAssemble(ref asmAddress);
                }

                // 積まれているものを処理する
                try
                {
                    foreach (var item in AlignLines)
                    {
                        var lineItem = item.LineItem;
                        try
                        {
                            lineItem.CreateLineDetailItem(AsmLoad);
                        }
                        catch (ErrorAssembleException ex)
                        {
                            this.AsmLoad.AddError(new ErrorLineItem(lineItem, ex));
                        }
                        catch (ErrorLineItemException ex)
                        {
                            this.AsmLoad.AddError(ex.ErrorLineItem);
                        }
                        catch (Exception ex)
                        {
                            this.AsmLoad.AddError(new ErrorLineItem(lineItem, new ErrorAssembleException(Error.ErrorCodeEnum.E0000, ex.Message)));
                        }
                        item.EnableAssemble = true;
                    }
                }
                catch
                {
                    // 解析でエラーが出た場合ORGに積む
                    AsmLoad.AddErrorLineDetailItem(this);
                    throw;
                }
            }
        }

        public override void ValidateAssemble()
        {
            base.ValidateAssemble();
            
            var totalByte = AlignLines.Sum(m => m.LineItem.BinResults.Sum(n => n.Data.Length));
            if (totalByte > AlignValue)
            {
                this.AsmLoad.AddError(new ErrorLineItem(this.LineItem, new ErrorAssembleException(Error.ErrorCodeEnum.E5002, $"{totalByte - AlignValue}バイト超えています。")));
            }
        }
    }
}
