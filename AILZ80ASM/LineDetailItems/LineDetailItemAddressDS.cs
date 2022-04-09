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
        private static readonly string RegexPatternDS = @"^(?<op1>(DS))\s+(?<arg1>[^,]+)\s*,*\s*(?<arg2>[^,]*)$";

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

            var lastAsmORG = this.AsmLoad.GetLastAsmORG_ExcludingRomMode();

            if (string.IsNullOrEmpty(LengthLabel) || !AIMath.TryParse<UInt16>(LengthLabel, this.AsmLoad, out var length))
            {
                throw new ErrorAssembleException(Error.ErrorCodeEnum.E0004, LengthLabel);
            }

            if (length > 0)
            {
                var offset = length;

                var fillByte = default(byte);
                if (AIMath.TryParse<byte>(FillByteLabel, this.AsmLoad, out var tempFillByte))
                {
                    fillByte = tempFillByte;
                }

                var asmORG = new AsmORG(asmAddress, false, fillByte, AsmORG.ORGTypeEnum.DS);
                this.AsmLoad.AddORG(asmORG);

                asmAddress.Program += (UInt16)offset;
                asmAddress.Output += (UInt32)offset;

                // 次のORGを作成する
                AssembleORG = new AsmORG(asmAddress, false, lastAsmORG.FillByte, AsmORG.ORGTypeEnum.NextORG);

                this.AsmLoad.AddORG(AssembleORG);
            }
        }

        public override void ExpansionItem()
        {
        }
    }
}
