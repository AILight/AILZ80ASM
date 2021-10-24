using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

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


        public static bool CanCreate(string operation)
        {
            var can = false;
            can = can || OperationItemOPCode.CanCreate(operation); // OpeCode
            can = can || OperationItemData.CanCreate(operation);   // Data
            can = can || OperationItemSystem.CanCreate(operation); // System

            return can;
        }


        public static OperationItem Create(LineDetailExpansionItemOperation lineDetailExpansionItemOperation, AsmAddress address, AsmLoad asmLoad)
        {
            var operationItem = default(OperationItem);

            // 命令を判別する
            operationItem ??= OperationItemOPCode.Create(lineDetailExpansionItemOperation, address, asmLoad); // OpeCode
            operationItem ??= OperationItemData.Create(lineDetailExpansionItemOperation, address, asmLoad);   // Data
            operationItem ??= OperationItemSystem.Create(lineDetailExpansionItemOperation, address, asmLoad);  // System

            return operationItem;
        }

        public virtual void Assemble(AsmLoad asmLoad)
        {
            throw new NotImplementedException();
        }
    }
}
