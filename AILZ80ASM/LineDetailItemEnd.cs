using AILZ80ASM.Assembler;
using AILZ80ASM.Exceptions;
using System.IO;
using System.Text.RegularExpressions;

namespace AILZ80ASM
{
    public class LineDetailItemEnd : LineDetailItem
    {
        private static readonly string RegexPatternCharMap = @"^\s*END$";

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

        private LineDetailItemEnd(LineItem lineItem, AsmLoad asmLoad)
            : base(lineItem, asmLoad)
        {

        }

        public static LineDetailItemEnd Create(LineItem lineItem, AsmLoad asmLoad)
        {
            var matched = Regex.Match(lineItem.OperationString, RegexPatternCharMap, RegexOptions.Singleline | RegexOptions.IgnoreCase);
            if (asmLoad.AsmEnd || matched.Success)
            {
                asmLoad.AsmEnd = true;

                return new LineDetailItemEnd(lineItem, asmLoad);
            }

            return default(LineDetailItemEnd);
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
