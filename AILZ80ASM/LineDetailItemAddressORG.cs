using AILZ80ASM.AILight;
using AILZ80ASM.Assembler;
using AILZ80ASM.Exceptions;
using System;
using System.IO;
using System.Text.RegularExpressions;

namespace AILZ80ASM
{
    public class LineDetailItemAddressORG : LineDetailItemAddress
    {
        private static readonly string RegexPatternORG = @"^ORG\s+(?<arg1>[^,]+)\s*,*\s*(?<arg2>[^,]*)\s*,*\s*(?<arg3>[^,]*)$";
        public string ProgramLabel { get; set; }
        public string OutputLabel { get; set; }
        public string FillByteLabel { get; set; }
        public override AsmList[] Lists
        {
            get
            {
                return new[] { AsmList.CreateLineItemORG(Address, new AsmLength(), LineItem) };
            }
        }

        private LineDetailItemAddressORG(LineItem lineItem, string programLabel, string outputLabel, string fillByteLabel, AsmLoad asmLoad)
            : base(lineItem, asmLoad)
        {
            ProgramLabel = programLabel;
            OutputLabel = outputLabel;
            FillByteLabel = fillByteLabel;
        }

        public static LineDetailItemAddressORG Create(LineItem lineItem, AsmLoad asmLoad)
        {
            var matched = Regex.Match(lineItem.OperationString, RegexPatternORG, RegexOptions.Singleline | RegexOptions.IgnoreCase);
            if (matched.Success)
            {
                var arg1 = matched.Groups["arg1"].Value;
                var arg2 = matched.Groups["arg2"].Value;
                var arg3 = matched.Groups["arg3"].Value;

                return new LineDetailItemAddressORG(lineItem, arg1, arg2, arg3, asmLoad);
            }

            return default(LineDetailItemAddressORG);
        }

        public override void Assemble()
        {
        }

        public override void PreAssemble(ref AsmAddress asmAddress)
        {
            var programAddress = AIMath.ConvertTo<UInt16>(ProgramLabel, this.AsmLoad, asmAddress);
            var amsORG = this.AsmLoad.GetLastAsmORG();
            var diff = (int)programAddress - asmAddress.Program;

            asmAddress.Program = programAddress;
            if (!string.IsNullOrEmpty(OutputLabel))
            {
                var outputAddress = AIMath.ConvertTo<UInt32>(OutputLabel, this.AsmLoad, asmAddress);
                asmAddress.Output = outputAddress;

            }
            else if (asmAddress.Output != amsORG.OutputAddress)
            {
                asmAddress.Output = (UInt32)(asmAddress.Output + diff);
            }
            var fillByte = default(byte);
            if (AIMath.TryParse<byte>(FillByteLabel, this.AsmLoad, out var tempFillByte))
            {
                fillByte = tempFillByte;
            }

            AssembleORG = new AsmORG(asmAddress.Program, asmAddress.Output, new[] { fillByte }, AsmORG.ORGTypeEnum.ORG);
            this.AsmLoad.AddORG(AssembleORG);

            base.PreAssemble(ref asmAddress);
        }

        public override void ExpansionItem()
        {
        }
    }
}
