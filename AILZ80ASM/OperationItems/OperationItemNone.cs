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
            return AsmList.CreateLineItem(LineDetailExpansionItemOperation.LineItem);
        }

        private OperationItemNone()
        {
        }

        public static OperationItemNone Create(LineItem listItem, AsmLoad asmLoad)
        {
            if (!string.IsNullOrEmpty(listItem.OperationString))
            {
                return default;
            }

            return new OperationItemNone();
        }

        public override void Assemble(AsmLoad asmLoad)
        {
        }
    }
}
