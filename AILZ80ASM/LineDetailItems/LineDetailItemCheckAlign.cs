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
                        asmLoad_LineDetailItemCheckAlign.LineDetailItems.Add(new Detail { EnableAssemble = false, TargetLineDetailItem = lineDetailItemCheckAlign });

                        return lineDetailItemCheckAlign;
                    }
                }

                // 開始条件
                if (startMatched.Success)
                {
                    asmLoad_LineDetailItemCheckAlign.NestedCount++;
                }
                asmLoad_LineDetailItemCheckAlign.LineDetailItems.Add(new Detail { EnableAssemble = true, TargetLineDetailItem = lineDetailItemCheckAlign });

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
                    var lineDetailItemCheckAlign = new LineDetailItemCheckAlign(lineItem, arg1, asmLoad)
                    {
                        NestedCount = 1
                    };
                    asmLoad.Share.LineDetailItemForExpandItem = lineDetailItemCheckAlign;
                    asmLoad.AddValidateAssembles(lineDetailItemCheckAlign);
                    lineDetailItemCheckAlign.LineDetailItems.Add(new Detail { EnableAssemble = false });

                    return lineDetailItemCheckAlign;
                }
            }

            return default(LineDetailItemCheckAlign);
        }

        public override void ExpansionItem()
        {
            if (AsmLoad.Share.LineDetailItemForExpandItem == this)
            {
                AsmLoad.AddError(new ErrorLineItem(LineItem, ErrorCodeEnum.E6001));
                return;
            }

            base.ExpansionItem();
        }

        public override void ValidateAssemble()
        {
            //base.ValidateAssemble();
            var alignValue = AIMath.Calculation(AlignLabel).ConvertTo<UInt16>();

            // アドレスチェック
            var startAddress = BinResults.Min(n => n.Address.Program);
            var endAddress = BinResults.Max(n => n.Address.Program + n.Data.Length + (n.Data.Length > 0 ? -1 : 0));

            var maskAddress = ~(alignValue - 1);

            var maskedStartAddress = startAddress & maskAddress;
            var maskedEndAddress = endAddress & maskAddress;

            if (maskedStartAddress != maskedEndAddress)
            {
                this.AsmLoad.AddError(new ErrorLineItem(this.LineItem, new ErrorAssembleException(Error.ErrorCodeEnum.E6002, startAddress, endAddress)));
            }
        }
    }
}
