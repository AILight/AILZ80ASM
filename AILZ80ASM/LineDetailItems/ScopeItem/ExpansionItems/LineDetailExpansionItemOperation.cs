using AILZ80ASM.Assembler;
using AILZ80ASM.Exceptions;
using AILZ80ASM.OperationItems;
using System;

namespace AILZ80ASM.LineDetailItems.ScopeItem.ExpansionItems
{
    public class LineDetailExpansionItemOperation : LineDetailExpansionItem
    {
        public OperationItem OperationItem { get; private set; }

        public override AsmResult[] BinResults
        {
            get
            {
                if (OperationItem == default(OperationItem) || OperationItem is OperationItemNone)
                {
                    return Array.Empty<AsmResult>();
                }
                else
                {
                    return new[] { new AsmResult() { Address = this.Address, Data = OperationItem.Bin, LineItem = this.LineItem } };
                }
            }
        }

        public override AsmList List
        {
            get
            {
                return OperationItem == default(OperationItem) ? AsmList.CreateLineItem(this.LineItem) : OperationItem.List(this.Address);
            }
        }

        public LineDetailExpansionItemOperation(LineItem lineItem, OperationItem operation, AsmLoad asmLoad)
            : base(lineItem)
        {
            OperationItem = operation;
        }

        public override void PreAssemble(ref AsmAddress asmAddress, AsmLoad asmLoad)
        {
            base.PreAssemble(ref asmAddress, asmLoad);

            OperationItem.PreAssemble(this);
            Length = OperationItem.Length;
            asmAddress = new AsmAddress(this.Address, Length);
        }

        public override void Assemble(AsmLoad asmLoad)
        {
            OperationItem?.Assemble(asmLoad);

            base.Assemble(asmLoad);
        }
    }
}
