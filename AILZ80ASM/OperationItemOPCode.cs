using System;
using System.Collections.Generic;
using System.Text;

namespace AILZ80ASM
{
    public class OperationItemOPCode : IOperationItem
    {
        public static IOperationItem Perse(LineItem lineItem)
        {
            return default(IOperationItem);
        }

        public void Assemble()
        {
            throw new NotImplementedException();
        }

        public byte[] Bin => new byte[] { };

        public ushort NextAddress => throw new NotImplementedException();
    }
}
