using System;
using System.Collections.Generic;
using System.Text;

namespace AILZ80ASM
{
    public interface IOperationItem
    {
        void Assemble(Label[] labels);

        byte[] Bin { get; }
        AsmAddress Address { get; }
        AsmLength Length { get;}
    }
}
