using System;
using System.Collections.Generic;
using System.Text;

namespace AILZ80ASM
{
    public class OperationItemInclude : IOperationItem
    {
        public static IOperationItem Perse(LineItem lineItem)
        {
            return default(IOperationItem);
        }

        public byte[] Bin => new byte[] { };

    }
}
