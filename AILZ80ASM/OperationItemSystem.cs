using System;
using System.Collections.Generic;
using System.Text;

namespace AILZ80ASM
{
    public class OperationItemSystem : IOperationItem
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
