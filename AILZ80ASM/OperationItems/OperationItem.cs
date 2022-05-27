using AILZ80ASM.Assembler;
using AILZ80ASM.LineDetailItems.ScopeItem.ExpansionItems;
using System;

namespace AILZ80ASM.OperationItems
{
    public abstract class OperationItem
    {
        public LineDetailExpansionItemOperation LineDetailExpansionItemOperation { get; set; }
        public virtual byte[] Bin => throw new NotImplementedException();
        public virtual AsmLength Length => throw new NotImplementedException();

        protected LineItem LineItem { get; set; }
        protected AsmLoad AsmLoad { get; set; }
        
        public OperationItem(LineItem lineItem, AsmLoad asmLoad)
        {
            LineItem = lineItem;
            AsmLoad = asmLoad;
        }

        public virtual AsmList List(AsmAddress asmAddress)
        {
            return AsmList.CreateLineItem(asmAddress, Bin, "", LineItem);
        }

        public virtual void PreAssemble(LineDetailExpansionItemOperation lineDetailExpansionItemOperation)
        {
            LineDetailExpansionItemOperation = lineDetailExpansionItemOperation;
        }

        public virtual void Assemble(AsmLoad asmLoad)
        {
        }
    }
}
