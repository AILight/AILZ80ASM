using AILZ80ASM.Assembler;
using AILZ80ASM.Exceptions;
using System.IO;
using System.Text.RegularExpressions;

namespace AILZ80ASM
{
    public class LineDetailItemEnd : LineDetailItem
    {
        private static readonly string RegexPatternEnd = @"^\s*END$";

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
            var matched = Regex.Match(lineItem.OperationString, RegexPatternEnd, RegexOptions.Singleline | RegexOptions.IgnoreCase);
            if (asmLoad.Scope.AssembleEndFlg || matched.Success)
            {
                asmLoad.Scope.AssembleEndFlg = true;

                return new LineDetailItemEnd(lineItem, asmLoad);
            }

            return default(LineDetailItemEnd);
        }

        public override void Assemble()
        {
        }

        public override void PreAssemble(ref AsmAddress asmAddress)
        {
            base.PreAssemble(ref asmAddress);
        }

        public override void ExpansionItem()
        {
        }
    }
}
