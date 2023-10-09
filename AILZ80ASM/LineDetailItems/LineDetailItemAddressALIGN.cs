using AILZ80ASM.AILight;
using AILZ80ASM.Assembler;
using AILZ80ASM.Exceptions;
using System;
using System.IO;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace AILZ80ASM.LineDetailItems
{
    public class LineDetailItemAddressAlign : LineDetailItemAddress
    {
        private static readonly string RegexPatternALIGN_Arg1 = @"^(?<op1>(ALIGN))\s+(?<arg1>[^,\s]+)$";
        private static readonly string RegexPatternALIGN_Arg1_2 = @"^(?<op1>(ALIGN))\s+(?<arg1>[^,\s]+)\s*,\s*(?<arg2>[^,\s]*)$";

        public string AlignLabel { get; set; }
        public string FillByteLabel { get; set; }
        public UInt16 AlignValue { get; set; } = 0;

        protected LineDetailItemAddressAlign(LineItem lineItem, AsmLoad asmLoad)
            : base(lineItem, asmLoad)
        {

        }

        protected LineDetailItemAddressAlign(LineItem lineItem, string alignLabel, string fillByteLabel, AsmLoad asmLoad)
            : base(lineItem, asmLoad)
        {
            AlignLabel = alignLabel;
            FillByteLabel = fillByteLabel;
        }

        public static LineDetailItemAddressAlign Create(LineItem lineItem, AsmLoad asmLoad)
        {
            if (!lineItem.IsCollectOperationString)
            {
                return default(LineDetailItemAddressAlign);
            }

            {
                var matched = Regex.Match(lineItem.OperationString, RegexPatternALIGN_Arg1, RegexOptions.Singleline | RegexOptions.IgnoreCase);
                if (matched.Success)
                {
                    var arg1 = matched.Groups["arg1"].Value;
                    return new LineDetailItemAddressAlign(lineItem, arg1, "", asmLoad);
                }
            }
            {
                var matched = Regex.Match(lineItem.OperationString, RegexPatternALIGN_Arg1_2, RegexOptions.Singleline | RegexOptions.IgnoreCase);
                if (matched.Success)
                {
                    var arg1 = matched.Groups["arg1"].Value;
                    var arg2 = matched.Groups["arg2"].Value;
                    return new LineDetailItemAddressAlign(lineItem, arg1, arg2, asmLoad);
                }
            }

            return default(LineDetailItemAddressAlign);
        }

        public override void Assemble()
        {
        }

        public override void PreAssemble(ref AsmAddress asmAddress)
        {
            asmAddress.Output = default(UInt32?);
            base.PreAssemble(ref asmAddress);

            if (string.IsNullOrEmpty(AlignLabel) || !AIMath.TryParse(AlignLabel, this.AsmLoad, out var aiValue))
            {
                throw new ErrorAssembleException(Error.ErrorCodeEnum.E0004, AlignLabel);
            }
            AlignValue = aiValue.ConvertTo<UInt16>();

            if (AlignValue <= 0 || (AlignValue & (AlignValue - 1)) != 0)
            {
                throw new ErrorAssembleException(Error.ErrorCodeEnum.E0015);
            }

            var remainder = (asmAddress.Program % AlignValue);
            if (remainder != 0)
            {
                var offset = AlignValue - remainder;
                if (offset > 0)
                {
                    var asmORG_ALIGN = new AsmORG(asmAddress.Program, "", FillByteLabel, this.LineItem, AsmORG.ORGTypeEnum.ALIGN);
                    this.AsmLoad.AddORG(asmORG_ALIGN);
                    this.AsmLoad.AddLineDetailItem(this); // 自分自身を追加する

                    asmAddress.Program += (UInt16)offset;

                    var asmORG_Next = new AsmORG(asmAddress.Program, "", "", this.LineItem, AsmORG.ORGTypeEnum.NextORG);
                    this.AsmLoad.AddORG(asmORG_Next);
                }
            }
        }

        public override void ExpansionItem()
        {
        }
    }
}
