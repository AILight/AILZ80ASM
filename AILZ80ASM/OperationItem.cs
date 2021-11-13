using AILZ80ASM.Instructions;
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
            switch (asmLoad.AssembleISA)
            {
                case AsmISA.Z80:
                    can = can || OperationItemOPCode<Z80>.CanCreate(operation); // OpeCode
                    can = can || OperationItemData<Z80>.CanCreate(operation);   // Data
                    break;
                default:
                    throw new NotSupportedException();
            }
            can = can || OperationItemSystem.CanCreate(operation); // System

            return can;
        }


        public static OperationItem Create(LineDetailExpansionItemOperation lineDetailExpansionItemOperation, AsmAddress address, AsmLoad asmLoad)
        {
            var operationItem = default(OperationItem);

            // 命令を判別する
            switch (asmLoad.AssembleISA)
            {
                case AsmISA.Z80:
                    operationItem ??= OperationItemOPCode<Z80>.Create(lineDetailExpansionItemOperation, address, asmLoad); // OpeCode
                    operationItem ??= OperationItemData<Z80>.Create(lineDetailExpansionItemOperation, address, asmLoad);   // Data
                    break;
                default:
                    throw new NotSupportedException();
            }
            operationItem ??= OperationItemSystem.Create(lineDetailExpansionItemOperation, address, asmLoad);  // System

            return operationItem;
        }

        public virtual void Assemble(AsmLoad asmLoad)
        {
            throw new NotImplementedException();
        }
    }
}
