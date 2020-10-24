using System;
using System.Collections.Generic;
using System.Text;

namespace AILZ80ASM
{
    public interface IOperationItem
    {
        static IOperationItem Perse(LineItem lineItem) => default(IOperationItem);
        void Assemble();

        byte[] Bin { get; }
        UInt16 NextAddress { get; }

    }
}
