using AILZ80ASM.Assembler;
using System;

namespace AILZ80ASM
{
    public class LineDetailExpansionItem
    {
        public Label Label { get; set; }
        public bool IsAssembled { get; set; }
        public LineItem LineItem { get; private set; }
        public AsmAddress Address { get; protected set; }

        public LineDetailExpansionItem(LineItem lineItem)
        {
            LineItem = lineItem;
        }

        public virtual byte[] Bin
        {
            get
            {
                return Array.Empty<byte>();
            }
        }

        public virtual AsmList List
        {
            get
            {
                return default(AsmList);
            }
        }

        public virtual void PreAssemble(ref AsmAddress asmAddress, AsmLoad asmLoad)
        {
            Address = asmAddress;
        }

        public virtual void Assemble(AsmLoad asmLoad)
        {
        }
    }
}
