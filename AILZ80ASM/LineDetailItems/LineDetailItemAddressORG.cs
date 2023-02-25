using AILZ80ASM.AILight;
using AILZ80ASM.Assembler;
using AILZ80ASM.Exceptions;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace AILZ80ASM.LineDetailItems
{
    public class LineDetailItemAddressORG : LineDetailItemAddress
    {
        private static readonly string RegexPatternORG = @"^ORG\s+(?<arg1>[^,]+)\s*,*\s*(?<arg2>[^,]*)\s*,*\s*(?<arg3>[^,]*)$";
        public string ProgramLabel { get; set; }
        public string OutputLabel { get; set; }
        public string FillByteLabel { get; set; }

        private LineDetailItemAddressORG(LineItem lineItem, string programLabel, string outputLabel, string fillByteLabel, AsmLoad asmLoad)
            : base(lineItem, asmLoad)
        {
            ProgramLabel = programLabel;
            OutputLabel = outputLabel;
            FillByteLabel = fillByteLabel;
        }

        public static LineDetailItemAddressORG Create(LineItem lineItem, AsmLoad asmLoad)
        {
            if (!lineItem.IsCollectOperationString)
            {
                return default(LineDetailItemAddressORG);
            }

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
            // ProgramAddress
            var programAddress = AIMath.Calculation(ProgramLabel, this.AsmLoad, asmAddress).ConvertTo<UInt16>();
            asmAddress.Program = programAddress;
            if (!string.IsNullOrEmpty(OutputLabel) &&
                AIMath.TryParse(OutputLabel, AsmLoad, asmAddress, out var resultValue) &&
                resultValue.TryParse<UInt32>(out var outputAddress))
            {
                asmAddress.Output = outputAddress;
            }
            else
            {
                asmAddress.Output = default(UInt32?);
                if (AsmLoad.Share.AsmSuperAssembleMode.IsInitializeOutputAddress)
                {
                    if (AsmLoad.Share.AsmSuperAssembleMode.AsmORG_AddressList.Any(m => m.Program == programAddress))
                    {
                        var asmORG_Address = AsmLoad.Share.AsmSuperAssembleMode.AsmORG_AddressList.First(m => m.Program == programAddress);
                        asmAddress.Output = asmORG_Address.Output;
                    }
                }
            }

            base.PreAssemble(ref asmAddress);

            var asmORG = default(AsmORG);
            if (asmAddress.Output.HasValue)
            {
                asmORG = new AsmORG(programAddress, asmAddress.Output.Value, FillByteLabel, this.LineItem, AsmORG.ORGTypeEnum.ORG);
            }
            else
            {
                asmORG = new AsmORG(programAddress, OutputLabel, FillByteLabel, this.LineItem, AsmORG.ORGTypeEnum.ORG);
            }
            this.AsmLoad.AddORG(asmORG);
            this.AsmLoad.AddLineDetailItem(this); // 自分自身を追加する
        }

        public override void ExpansionItem()
        {
        }
    }
}
