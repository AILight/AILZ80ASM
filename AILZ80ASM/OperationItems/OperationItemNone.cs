using AILZ80ASM.Assembler;
using AILZ80ASM.Exceptions;
using AILZ80ASM.InstructionSet;
using AILZ80ASM.LineDetailItems.ScopeItem.ExpansionItems;
using System;
using System.Linq;

namespace AILZ80ASM.OperationItems
{
    public class OperationItemNone : OperationItem
    {
        public override byte[] Bin => Array.Empty<byte>();
        public override AsmLength Length => new AsmLength(0);
        public override AsmList List(AsmAddress asmAddress)
        {
            if (!string.IsNullOrEmpty(this.LineItem.LabelString) && !Label.IsGlobalLabel(this.LineItem.LabelString))
            {
                return AsmList.CreateLineItem(LineItem, asmAddress);
            }
            else
            {
                return AsmList.CreateLineItem(LineItem);
            }
        }

        private OperationItemNone(LineItem lineItem, AsmLoad asmLoad)
            : base(lineItem, asmLoad)
        {
        }

        public static OperationItemNone Create(LineItem lineItem, AsmLoad asmLoad)
        {
            if (!string.IsNullOrEmpty(lineItem.OperationString))
            {
                return default;
            }

            return new OperationItemNone(lineItem, asmLoad);
        }

        public override void Assemble(AsmLoad asmLoad)
        {
        }
    }
}
