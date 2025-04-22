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
        private static readonly Regex CompiledRegexPatternDS = new Regex(
            RegexPatternDS,
            RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase
        );

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

            var matched = CompiledRegexPatternDS.Match(lineItem.OperationString);
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

            var asmORG_DS = default(AsmORG);
            if (asmAddress.Output.HasValue)
            {
                asmORG_DS = new AsmORG(asmAddress.Program, asmAddress.Output.Value, FillByteLabel, this.LineItem, AsmORG.ORGTypeEnum.DS);
            }
            else
            {
                asmORG_DS = new AsmORG(asmAddress.Program, "", FillByteLabel, this.LineItem, AsmORG.ORGTypeEnum.DS);
            }

            this.AsmLoad.AddORG(asmORG_DS);
            this.AsmLoad.AddLineDetailItem(this); // 自分自身を追加する

            if (string.IsNullOrEmpty(LengthLabel) || !AIMath.TryParse(LengthLabel, this.AsmLoad, asmAddress, out var aiValue))
            {
                throw new ErrorAssembleException(Error.ErrorCodeEnum.E0004, LengthLabel);
            }
            var length = aiValue.ConvertTo<UInt16>();
            var asmLength = new AsmLength(length);

            asmAddress.Program = (UInt16)(asmAddress.Program + asmLength.Program);
            if (asmAddress.Output.HasValue)
            {
                asmAddress.Output = (UInt32)(asmAddress.Output + asmLength.Output);
            }
            else
            {
                asmAddress.Output = default(UInt32?);
            }

            var asmORG_Next = default(AsmORG);
            if (asmAddress.Output.HasValue)
            {
                asmORG_Next = new AsmORG(asmAddress.Program, asmAddress.Output.Value, "", this.LineItem, AsmORG.ORGTypeEnum.NextORG);
            }
            else
            {
                asmORG_Next = new AsmORG(asmAddress.Program, "", "", this.LineItem, AsmORG.ORGTypeEnum.NextORG);
            }
            this.AsmLoad.AddORG(asmORG_Next);
        }

        public override void ExpansionItem()
        {
        }
    }
}
