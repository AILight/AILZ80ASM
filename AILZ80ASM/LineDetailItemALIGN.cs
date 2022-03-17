using AILZ80ASM.AILight;
using AILZ80ASM.Assembler;
using AILZ80ASM.Exceptions;
using System;
using System.IO;
using System.Text.RegularExpressions;

namespace AILZ80ASM
{
    public class LineDetailItemALIGN : LineDetailItem
    {
        private static readonly string RegexPatternORG = @"^(?<op1>(ALIGN))\s+(?<op2>[^,]+)\s*,*\s*(?<op3>[^,]*)\s*,*\s*(?<op4>[^,]*)$";
        private Match Matched { get; set; }

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

        private LineDetailItemALIGN(LineItem lineItem, Match match, AsmLoad asmLoad)
            : base(lineItem, asmLoad)
        {
            Matched = match;
        }

        public static LineDetailItemALIGN Create(LineItem lineItem, AsmLoad asmLoad)
        {
            var matched = Regex.Match(lineItem.OperationString, RegexPatternORG, RegexOptions.Singleline | RegexOptions.IgnoreCase);
            if (matched.Success)
            {
                return new LineDetailItemALIGN(lineItem, matched, asmLoad);
            }

            return default(LineDetailItemALIGN);
        }

        public override void Assemble()
        {
        }

        public override void PreAssemble(ref AsmAddress asmAddress)
        {
            var op1 = Matched.Groups["op1"].Value;
            var op2 = Matched.Groups["op2"].Value;

            var programAddress = AIMath.ConvertTo<UInt16>(op2, this.AsmLoad);

            asmAddress.Program += programAddress;

        }

        public override void ExpansionItem()
        {
        }
    }
}
