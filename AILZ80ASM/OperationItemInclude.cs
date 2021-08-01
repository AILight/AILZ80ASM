using System;
using System.Collections.Generic;
using System.Text;

namespace AILZ80ASM
{
    public class OperationItemInclude : IOperationItem
    {
        public static IOperationItem Parse(LineExpansionItem lineExpansionItem, AsmAddress address)
        {
            return default(IOperationItem);
        }

        public byte[] Bin => throw new NotImplementedException();

        public AsmAddress Address => throw new NotImplementedException();
        public AsmLength Length => throw new NotImplementedException();

        public void Assemble(Label[] labels)
        {
            throw new NotImplementedException();
        }
    }
}
