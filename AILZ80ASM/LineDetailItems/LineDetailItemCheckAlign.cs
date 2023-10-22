using AILZ80ASM.AILight;
using AILZ80ASM.Assembler;
using AILZ80ASM.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using static AILZ80ASM.Assembler.Error;

namespace AILZ80ASM.LineDetailItems
{
    public class LineDetailItemCheckAlign : LineDetailItemCheck
    {
        private static readonly string RegexPatternStart = @"^(?<op1>(CHECK\s+ALIGN))\s+(?<arg1>[^,\s]+)$";
        private static readonly string RegexPatternEnd = @"^\s*ENDM\s*$";

        private int NestedCount { get; set; } = 0;

        public string AlignLabel { get; set; }
        public UInt16 AlignValue { get; set; } = 0;
        public bool EnableAssemble { get; set; } = false;

        private LineDetailItemCheckAlign(LineItem lineItem, AsmLoad asmLoad)
            : base(lineItem, asmLoad)
        {

        }

        private LineDetailItemCheckAlign(LineItem lineItem, string alignLabel, AsmLoad asmLoad)
            : base(lineItem, asmLoad)
        {
            AlignLabel = alignLabel;
        }

        public static LineDetailItemCheckAlign Create(LineItem lineItem, AsmLoad asmLoad)
        {
            if (!lineItem.IsCollectOperationString)
            {
                return default(LineDetailItemCheckAlign);
            }

            var startMatched = Regex.Match(lineItem.OperationString, RegexPatternStart, RegexOptions.Singleline | RegexOptions.IgnoreCase);
            var endMatched = Regex.Match(lineItem.OperationString, RegexPatternEnd, RegexOptions.Singleline | RegexOptions.IgnoreCase);

            // 条件処理処理中
            if (asmLoad.Share.LineDetailItemForExpandItem is LineDetailItemCheckAlign asmLoad_LineDetailItemCheckAlign)
            {
                var lineDetailItemCheckAlign = new LineDetailItemCheckAlign(lineItem, asmLoad);
                // 終了条件チェック
                if (endMatched.Success)
                {
                    asmLoad_LineDetailItemCheckAlign.NestedCount--;

                    // 条件処理が終了
                    if (asmLoad_LineDetailItemCheckAlign.NestedCount == 0)
                    {
                        asmLoad.Share.LineDetailItemForExpandItem = default;
                        return lineDetailItemCheckAlign;
                    }
                }

                // 開始条件
                if (startMatched.Success)
                {
                    asmLoad_LineDetailItemCheckAlign.NestedCount++;
                }
                asmLoad_LineDetailItemCheckAlign.LineDetailItemDic.Add(lineDetailItemCheckAlign, LineDetailItem.CreateLineDetailItem(lineItem, asmLoad));

                return lineDetailItemCheckAlign;
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
                if (startMatched.Success)
                {
                    var arg1 = startMatched.Groups["arg1"].Value;
                    var lineDetailItemAddressAlignBlock = new LineDetailItemCheckAlign(lineItem, arg1, asmLoad)
                    {
                        NestedCount = 1
                    };
                    asmLoad.Share.LineDetailItemForExpandItem = lineDetailItemAddressAlignBlock;

                    return lineDetailItemAddressAlignBlock;
                }
            }

            return default(LineDetailItemCheckAlign);
        }

        public override void Assemble()
        {
            if (this.EnableAssemble)
            {
                this.LineItem.Assemble();
            }
        }

        public override void PreAssemble(ref AsmAddress asmAddress)
        {
            if (AsmLoad.Share.LineDetailItemForExpandItem == this)
            {
                AsmLoad.AddError(new ErrorLineItem(LineItem, ErrorCodeEnum.E6001));
                return;
            }

            if (this.EnableAssemble)
            {
                this.LineItem.PreAssemble(ref asmAddress);
            }
            else if (AlignLines.Any())
            {
                // 境界オーバーしているか確認をする処理へ追加
                AsmLoad.AddValidateAssembles(this);

                if (string.IsNullOrEmpty(AlignLabel) || !AIMath.TryParse(AlignLabel, this.AsmLoad, out var aiValue))
                {
                    throw new ErrorAssembleException(Error.ErrorCodeEnum.E0004, AlignLabel);
                }
                AlignValue = aiValue.ConvertTo<UInt16>();

                if (AlignValue <= 0 || (AlignValue & (AlignValue - 1)) != 0)
                {
                    throw new ErrorAssembleException(Error.ErrorCodeEnum.E0015);
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

            // アドレスチェック
            //var alignLines = AlignLines.Where(m => m.LineItem.BinResults.Select);
            var startAddress = AlignLines.Min(m => m.LineItem.BinResults.Min(n => n.Address.Program));
            var endAddress = AlignLines.Max(m => m.LineItem.BinResults.Max(n => n.Address.Program + n.Data.Length + (n.Data.Length > 0 ? -1 : 0)));

            var maskAddress = ~(AlignValue - 1);

            var maskedStartAddress = startAddress & maskAddress;
            var maskedEndAddress = endAddress & maskAddress;

            if (maskedStartAddress != maskedEndAddress)
            {
                this.AsmLoad.AddError(new ErrorLineItem(this.LineItem, new ErrorAssembleException(Error.ErrorCodeEnum.E6002, startAddress, endAddress)));
            }
        }
    }
}
