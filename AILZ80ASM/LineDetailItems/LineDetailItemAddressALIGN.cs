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

            var lastAsmORG = this.AsmLoad.GetLastAsmORG_ExcludingRomMode();
            var align = default(UInt16);
            var saveIsUsingOutputAddressVariable = AsmLoad.Share.IsUsingOutputAddressVariable;
            AsmLoad.Share.IsUsingOutputAddressVariable = false; // 下品なコードでゴメン
            try
            {
                if (string.IsNullOrEmpty(AlignLabel) || !AIMath.TryParse<UInt16>(AlignLabel, this.AsmLoad, out align))
                {
                    throw new ErrorAssembleException(Error.ErrorCodeEnum.E0004, AlignLabel);
                }
            }
            finally
            {
                if (AsmLoad.Share.IsUsingOutputAddressVariable)
                {
                    AsmLoad.Share.UsingOutputAddressLineDetailItemAddressList.Add(this);
                }
                else
                {
                    AsmLoad.Share.IsUsingOutputAddressVariable = saveIsUsingOutputAddressVariable;
                }
            }

            if ((align & (align - 1)) != 0)
            {
                throw new ErrorAssembleException(Error.ErrorCodeEnum.E0015);
            }

            var remainder = (asmAddress.Program % align);
            if (remainder != 0)
            {
                var offset = align - remainder;
                if (offset > 0)
                {
                    var fillByte = default(byte);
                    if (AIMath.TryParse<byte>(FillByteLabel, this.AsmLoad, out var tempFillByte))
                    {
                        fillByte = tempFillByte;
                    }
                    var asmORG = new AsmORG(asmAddress, false, fillByte, AsmORG.ORGTypeEnum.ALIGN);
                    this.AsmLoad.AddORG(asmORG);

                    asmAddress.Program += (UInt16)offset;
                    asmAddress.Output += (UInt32)offset;

                    // 次のORGを作成する
                    AssembleORG = new AsmORG(asmAddress, false, lastAsmORG.FillByte, AsmORG.ORGTypeEnum.NextORG);

                    this.AsmLoad.AddORG(AssembleORG);
                }
            }
        }

        public override void ExpansionItem()
        {
        }
    }
}
