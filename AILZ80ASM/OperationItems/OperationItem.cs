using AILZ80ASM.Assembler;
using AILZ80ASM.LineDetailItems.ScopeItem.ExpansionItems;
using System;

namespace AILZ80ASM.OperationItems
{
    public class OperationItem
    {
        public OperationItem()
        {

        }

        //public AsmAddress Address { get; protected set; }

        public LineDetailExpansionItemOperation LineDetailExpansionItemOperation { get; set; }

        public virtual byte[] Bin => throw new NotImplementedException();

        public virtual AsmList List(AsmAddress asmAddress)
        {
            return AsmList.CreateLineItem(asmAddress, Bin, "", LineDetailExpansionItemOperation.LineItem);
        }

        public virtual AsmLength Length => throw new NotImplementedException();


        public static bool CanCreate(string operation, AsmLoad asmLoad)
        {
            var can = false;
            can = can || OperationItemOPCode.CanCreate(operation, asmLoad);     // OpeCode
            can = can || OperationItemData.CanCreate(operation, asmLoad);       // Data
            can = can || OperationItemDataFill.CanCreate(operation, asmLoad);   // DataFill

            return can;
        }


        public static OperationItem Create(LineDetailExpansionItemOperation lineDetailExpansionItemOperation, AsmAddress address, AsmLoad asmLoad)
        {
            var operationItem = default(OperationItem);

            // 命令を判別する
            operationItem ??= OperationItemOPCode.Create(lineDetailExpansionItemOperation, address, asmLoad);       // OpeCode
            operationItem ??= OperationItemData.Create(lineDetailExpansionItemOperation, address, asmLoad);         // Data
            operationItem ??= OperationItemDataFill.Create(lineDetailExpansionItemOperation, address, asmLoad);     // DataFill

            return operationItem;
        }

        public virtual void Assemble(AsmLoad asmLoad)
        {
            throw new NotImplementedException();
        }

        public virtual void TrimData()
        {
            throw new NotImplementedException();
        }
    }
}
