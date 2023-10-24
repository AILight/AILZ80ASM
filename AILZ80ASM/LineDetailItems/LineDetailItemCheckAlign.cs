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
        private static readonly string RegexPatternStart_Arg1   = @"^(?<op1>(CHECK\s+ALIGN))\s+(?<arg1>[^,\s]+)$";
        private static readonly string RegexPatternStart_Arg1_2 = @"^(?<op1>(CHECK\s+ALIGN))\s+(?<arg1>[^,\s]+)\s*,\s*(?<arg2>[^,\s]*)$";

        private static readonly string RegexPatternEnd = @"^\s*ENDM\s*$";

        private int NestedCount { get; set; } = 0;

        public string AlignLabel { get; set; }
        public string DataLengthLabel { get; set; }

        private LineDetailItemCheckAlign(LineItem lineItem, AsmLoad asmLoad)
            : base(lineItem, asmLoad)
        {

        }

        private LineDetailItemCheckAlign(LineItem lineItem, string alignLabel, AsmLoad asmLoad)
            : this(lineItem, alignLabel, "1", asmLoad)
        {

        }

        private LineDetailItemCheckAlign(LineItem lineItem, string alignLabel, string dataLengthLabel, AsmLoad asmLoad)
            : base(lineItem, asmLoad)
        {
            AlignLabel = alignLabel;
            DataLengthLabel = dataLengthLabel;
        }

        public static LineDetailItemCheckAlign Create(LineItem lineItem, AsmLoad asmLoad)
        {
            if (!lineItem.IsCollectOperationString)
            {
                return default(LineDetailItemCheckAlign);
            }

            var startMatched_1   = Regex.Match(lineItem.OperationString, RegexPatternStart_Arg1, RegexOptions.Singleline | RegexOptions.IgnoreCase);
            var startMatched_1_2 = Regex.Match(lineItem.OperationString, RegexPatternStart_Arg1_2, RegexOptions.Singleline | RegexOptions.IgnoreCase);
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
                if (startMatched_1.Success || startMatched_1_2.Success)
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
                if (startMatched_1.Success)
                {
                    var arg1 = startMatched_1.Groups["arg1"].Value;
                    var lineDetailItemCheckAlign = new LineDetailItemCheckAlign(lineItem, arg1, asmLoad)
                    {
                        NestedCount = 1
                    };
                    asmLoad.Share.LineDetailItemForExpandItem = lineDetailItemCheckAlign;
                    asmLoad.AddValidateAssembles(lineDetailItemCheckAlign);
                    lineDetailItemCheckAlign.LineDetailItems.Add(new Detail { EnableAssemble = false });

                    return lineDetailItemCheckAlign;
                }

                if (startMatched_1_2.Success)
                {
                    var arg1 = startMatched_1_2.Groups["arg1"].Value;
                    var arg2 = startMatched_1_2.Groups["arg2"].Value;
                    var lineDetailItemCheckAlign = new LineDetailItemCheckAlign(lineItem, arg1, arg2, asmLoad)
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
            var dataLength = AIMath.Calculation(DataLengthLabel).ConvertTo<UInt16>();

            // アドレスチェック
            var startAddress = BinResults.Min(n => n.Address.Program);
            var endAddress = BinResults.Max(n => n.Address.Program + n.Data.Length + (n.Data.Length > 0 ? (dataLength * -1) : 0));

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
