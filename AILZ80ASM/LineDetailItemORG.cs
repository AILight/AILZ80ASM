using AILZ80ASM.AILight;
using AILZ80ASM.Assembler;
using AILZ80ASM.Exceptions;
using System;
using System.IO;
using System.Text.RegularExpressions;

namespace AILZ80ASM
{
    public class LineDetailItemORG : LineDetailItem
    {
        private static readonly string RegexPatternORG = @"^ORG\s+(?<arg1>[^,]+)\s*,*\s*(?<arg2>[^,]*)\s*,*\s*(?<arg3>[^,]*)$";
        public AsmOrg  AssembleORG { get; set; }

        public override AsmList[] Lists
        {
            get
            {
                return new[]
                {
                    AsmList.CreateLineItem(LineItem)
                };
            }
        }

        private LineDetailItemORG(LineItem lineItem, AsmOrg asmOrg, AsmLoad asmLoad)
            : base(lineItem, asmLoad)
        {
            AssembleORG = asmOrg;
        }

        public static LineDetailItemORG Create(LineItem lineItem, AsmLoad asmLoad)
        {
            var matched = Regex.Match(lineItem.OperationString, RegexPatternORG, RegexOptions.Singleline | RegexOptions.IgnoreCase);
            if (matched.Success)
            {
                var arg1 = matched.Groups["arg1"].Value;
                var arg2 = matched.Groups["arg2"].Value;
                var arg3 = matched.Groups["arg3"].Value;
                if (string.IsNullOrEmpty(arg1) || !AIMath.TryParse<UInt16>(arg1, out var programAddress))
                {
                    throw new ErrorAssembleException(Error.ErrorCodeEnum.E0004, arg1);
                }

                var outputAddress = default(UInt32?);
                if (!string.IsNullOrEmpty(arg2) && !AIMath.TryParse<UInt16>(arg2, out var tmpOutputAddress))
                {
                    outputAddress = tmpOutputAddress;
                }

                var fillByte = default(byte?);
                if (!string.IsNullOrEmpty(arg3) && !AIMath.TryParse<byte>(arg2, out var tmpFillByte))
                {
                    fillByte = tmpFillByte;
                }
                var asmOrg = new AsmOrg(programAddress, outputAddress, fillByte);


                return new LineDetailItemORG(lineItem, asmOrg, asmLoad);
            }

            return default(LineDetailItemORG);
        }

        public override void Assemble()
        {
        }

        public override void PreAssemble(ref AsmAddress asmAddress)
        {
        }

        public override void ExpansionItem()
        {
        }
    }
}
