using AILZ80ASM.AILight;
using AILZ80ASM.Assembler;
using AILZ80ASM.Exceptions;
using System;
using System.IO;
using System.Text.RegularExpressions;

namespace AILZ80ASM.LineDetailItems
{
    public class LineDetailItemAddressDS : LineDetailItemAddress
    {
        private static readonly string RegexPatternDS = @"^(?<op1>(DS|DEFS))\s+(?<arg1>[^,]+)\s*,*\s*(?<arg2>[^,]*)$";

        public string Operation { get; set; }
        public string LengthLabel { get; set; }
        public string FillByteLabel { get; set; }

        private LineDetailItemAddressDS(LineItem lineItem, string operation, string lengthLabel, string fillByteLabel, AsmLoad asmLoad)
            : base(lineItem, asmLoad)
        {
            Operation = operation;
            LengthLabel = lengthLabel;
            FillByteLabel = fillByteLabel;
        }

        public static LineDetailItemAddressDS Create(LineItem lineItem, AsmLoad asmLoad)
        {
            if (!lineItem.IsCollectOperationString)
            {
                return default(LineDetailItemAddressDS);
            }

            var matched = Regex.Match(lineItem.OperationString, RegexPatternDS, RegexOptions.Singleline | RegexOptions.IgnoreCase);
            if (matched.Success)
            {
                var op1 = matched.Groups["op1"].Value;
                var arg1 = matched.Groups["arg1"].Value;
                var arg2 = matched.Groups["arg2"].Value;
                return new LineDetailItemAddressDS(lineItem, op1, arg1, arg2, asmLoad);
            }

            return default(LineDetailItemAddressDS);
        }

        public override void Assemble()
        {
        }

        public override void PreAssemble(ref AsmAddress asmAddress)
        {
            base.PreAssemble(ref asmAddress);

            var asmORG_DS = new AsmORG(asmAddress.Program, "", FillByteLabel, this.LineItem, AsmORG.ORGTypeEnum.ORG);
            this.AsmLoad.AddORG(asmORG_DS);
            this.AsmLoad.AddLineDetailItem(this); // 自分自身を追加する

            if (string.IsNullOrEmpty(LengthLabel) || !AIMath.TryParse(LengthLabel, this.AsmLoad, out var aiValue))
            {
                throw new ErrorAssembleException(Error.ErrorCodeEnum.E0004, LengthLabel);
            }
            var length = aiValue.ConvertTo<UInt16>();

            asmAddress.Program += length;

            var asmORG_Next = new AsmORG(asmAddress.Program, "", "", this.LineItem, AsmORG.ORGTypeEnum.NextORG);
            this.AsmLoad.AddORG(asmORG_Next);
        }

        public override void ExpansionItem()
        {
        }
    }
}
