using AILZ80ASM.Assembler;
using System;

namespace AILZ80ASM
{
    public class OperationItem
    {
        public OperationItem()
        {

        }

        public AsmAddress Address { get; protected set; }

        public LineDetailExpansionItemOperation LineDetailExpansionItemOperation { get; set; }

        public virtual byte[] Bin => throw new NotImplementedException();

        public virtual AsmList List
        {
            get
            {
                return AsmList.CreateLineItem(Address, Bin, "", LineDetailExpansionItemOperation.LineItem);
            }
        }

        public virtual AsmLength Length => throw new NotImplementedException();


        public static bool CanCreate(string operation, AsmLoad asmLoad)
        {
            var can = false;
            can = can || OperationItemOPCode.CanCreate(operation, asmLoad);     // OpeCode
            can = can || OperationItemData.CanCreate(operation, asmLoad);       // Data
            can = can || OperationItemDataSpace.CanCreate(operation, asmLoad);  // DataSpace
            //can = can || OperationItemSystem.CanCreate(operation, asmLoad);     // System

            return can;
        }


        public static OperationItem Create(LineDetailExpansionItemOperation lineDetailExpansionItemOperation, AsmAddress address, AsmLoad asmLoad)
        {
            var operationItem = default(OperationItem);

            // 命令を判別する
            operationItem ??= OperationItemOPCode.Create(lineDetailExpansionItemOperation, address, asmLoad);       // OpeCode
            operationItem ??= OperationItemData.Create(lineDetailExpansionItemOperation, address, asmLoad);         // Data
            operationItem ??= OperationItemDataSpace.Create(lineDetailExpansionItemOperation, address, asmLoad);    // DataSpace
            //operationItem ??= OperationItemSystem.Create(lineDetailExpansionItemOperation, address, asmLoad);       // System

            if (asmLoad.AssembleOption.OutputTrim && 
                operationItem is IOperationItemDefaultClearable defaultClearble && defaultClearble.IsDefaultValueClear)
            {
                asmLoad.AddTrimOperationItem(operationItem);
            }
            else
            {
                asmLoad.ClearTrimOperationItem();
            }

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
