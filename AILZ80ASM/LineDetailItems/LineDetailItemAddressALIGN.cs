using AILZ80ASM.AILight;
using AILZ80ASM.Assembler;
using AILZ80ASM.Exceptions;
using System;
using System.IO;
using System.Text.RegularExpressions;

namespace AILZ80ASM.LineDetailItems
{
    public class LineDetailItemAddressALIGN : LineDetailItemAddress
    {
        private static readonly string RegexPatternALIGN = @"^(?<op1>(ALIGN))\s+(?<arg1>[^,]+)\s*,*\s*(?<arg2>[^,]*)$";

        public string AlignLabel { get; set; }
        public string FillByteLabel { get; set; }

        private LineDetailItemAddressALIGN(LineItem lineItem, string alignLabel, string fillByteLabel, AsmLoad asmLoad)
            : base(lineItem, asmLoad)
        {
            AlignLabel = alignLabel;
            FillByteLabel = fillByteLabel;
        }

        public static LineDetailItemAddressALIGN Create(LineItem lineItem, AsmLoad asmLoad)
        {
            if (!lineItem.IsCollectOperationString)
            {
                return default(LineDetailItemAddressALIGN);
            }

            var matched = Regex.Match(lineItem.OperationString, RegexPatternALIGN, RegexOptions.Singleline | RegexOptions.IgnoreCase);
            if (matched.Success)
            {
                var arg1 = matched.Groups["arg1"].Value;
                var arg2 = matched.Groups["arg2"].Value;
                return new LineDetailItemAddressALIGN(lineItem, arg1, arg2, asmLoad);
            }

            return default(LineDetailItemAddressALIGN);
        }

        public override void Assemble()
        {
        }

        public override void PreAssemble(ref AsmAddress asmAddress)
        {
            base.PreAssemble(ref asmAddress);

            if (string.IsNullOrEmpty(AlignLabel) || !AIMath.TryParse(AlignLabel, this.AsmLoad, out var aiValue))
            {
                throw new ErrorAssembleException(Error.ErrorCodeEnum.E0004, AlignLabel);
            }
            var align = aiValue.ConvertTo<UInt16>();

            if (align <= 0 || (align & (align - 1)) != 0)
            {
                throw new ErrorAssembleException(Error.ErrorCodeEnum.E0015);
            }

            var remainder = (asmAddress.Program % align);
            if (remainder != 0)
            {
                var offset = align - remainder;
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
