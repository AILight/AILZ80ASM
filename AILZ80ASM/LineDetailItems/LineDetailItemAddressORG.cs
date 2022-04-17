using AILZ80ASM.AILight;
using AILZ80ASM.Assembler;
using AILZ80ASM.Exceptions;
using System;
using System.IO;
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
            base.PreAssemble(ref asmAddress);

            var saveIsUsingOutputAddressVariableProgram = AsmLoad.Share.IsUsingOutputAddressVariable;
            AsmLoad.Share.IsUsingOutputAddressVariable = false; // 下品なコードでゴメン
            var programAddress = AIMath.ConvertTo<UInt16>(ProgramLabel, this.AsmLoad, asmAddress);
            if (AsmLoad.Share.IsUsingOutputAddressVariable)
            {
                AsmLoad.Share.UsingOutputAddressLineDetailItemAddressList.Add(this);
            }
            else
            {
                AsmLoad.Share.IsUsingOutputAddressVariable = saveIsUsingOutputAddressVariableProgram;
            }

            var asmORG = this.AsmLoad.GetLastAsmORG_ExcludingRomMode();
            var diff = (int)programAddress - asmORG.Address.Program;
            var isRomMode = false;

            asmAddress.Program = programAddress;
            if (!string.IsNullOrEmpty(OutputLabel))
            {
                var saveIsUsingOutputAddressVariableOutput = AsmLoad.Share.IsUsingOutputAddressVariable;
                AsmLoad.Share.IsUsingOutputAddressVariable = false; // 下品なコードでゴメン
                var outputAddress = AIMath.ConvertTo<UInt32>(OutputLabel, this.AsmLoad, asmAddress);
                if (AsmLoad.Share.IsUsingOutputAddressVariable)
                {
                    AsmLoad.Share.UsingOutputAddressLineDetailItemAddressList.Add(this);
                }
                else
                {
                    AsmLoad.Share.IsUsingOutputAddressVariable = saveIsUsingOutputAddressVariableOutput;
                }
                asmAddress.Output = outputAddress;
                isRomMode = true;
            }
            else if (asmAddress.Output != asmORG.Address.Output)
            {
                if (asmORG.Address.Output + diff < 0)
                {
                    this.AsmLoad.Share.NeedResetAddress = true;
                }

                asmAddress.Output = (UInt32)(asmORG.Address.Output + diff);
            }
            
            var fillByte = default(byte);
            if (AIMath.TryParse<byte>(FillByteLabel, this.AsmLoad, out var tempFillByte))
            {
                fillByte = tempFillByte;
            }

            AssembleORG = new AsmORG(asmAddress, isRomMode, fillByte, AsmORG.ORGTypeEnum.ORG);
            this.AsmLoad.AddORG(AssembleORG);

        }

        public override void ExpansionItem()
        {
        }
    }
}
