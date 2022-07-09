using AILZ80ASM.AILight;
using AILZ80ASM.Assembler;
using AILZ80ASM.Exceptions;
using System;
using System.IO;
using System.Text.RegularExpressions;

namespace AILZ80ASM.LineDetailItems
{
    public class LineDetailItemEndDefine : LineDetailItem
    {
        private static readonly string RegexPatternEnd = @"^\s*END(|\s+(?<value>.+))$";

        private string EndLabel { get; set; }
        private UInt16? EntryPoint { get; set; }

        public override AsmList[] Lists
        {
            get
            {
                return new[]
                {
                    AsmList.CreateLineItemEndDefine(EntryPoint, LineItem)
                };
            }
        }

        private LineDetailItemEndDefine(string endLabel, LineItem lineItem, AsmLoad asmLoad)
            : base(lineItem, asmLoad)
        {
            EndLabel = endLabel;
        }


        private LineDetailItemEndDefine(LineItem lineItem, AsmLoad asmLoad)
            : this("", lineItem, asmLoad)
        {

        }

        public static LineDetailItemEndDefine Create(LineItem lineItem, AsmLoad asmLoad)
        {
            if (!lineItem.IsCollectOperationString)
            {
                return default(LineDetailItemEndDefine);
            }

            var matched = Regex.Match(lineItem.OperationString, RegexPatternEnd, RegexOptions.Singleline | RegexOptions.IgnoreCase);
            if (matched.Success && AIName.ValidateNameEndArgument(matched.Groups["value"].Value))
            {
                asmLoad.Scope.AssembleEndFlg = true;
                return new LineDetailItemEndDefine(matched.Groups["value"].Value, lineItem, asmLoad);
            }

            return default(LineDetailItemEndDefine);
        }

        public override void Assemble()
        {
        }

        public override void PreAssemble(ref AsmAddress asmAddress)
        {
            base.PreAssemble(ref asmAddress);

            if (!string.IsNullOrEmpty(EndLabel))
            {
                AsmLoad.Share.EntryPoint = AIMath.Calculation(EndLabel, AsmLoad, asmAddress).ConvertTo<UInt16>();
                EntryPoint = AsmLoad.Share.EntryPoint;
            }
        }

        public override void ExpansionItem()
        {
        }
    }
}
