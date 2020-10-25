using System;
using System.Collections.Generic;
using System.Text;

namespace AILZ80ASM
{
    public class OperationItemInclude : IOperationItem
    {
        public static IOperationItem Perse(LineItem lineItem, UInt16 address)
        {
            return default(IOperationItem);
        }

        public byte[] Bin => throw new NotImplementedException();

        public ushort Address => throw new NotImplementedException();

        public ushort NextAddress => throw new NotImplementedException();

        public void Assemble(Label[] labels)
        {
            throw new NotImplementedException();
        }
    }
}
