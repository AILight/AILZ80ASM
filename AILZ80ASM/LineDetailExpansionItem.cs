using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

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

        public virtual void PreAssemble(ref AsmAddress asmAddress, AsmLoad asmLoad)
        {
            Address = asmAddress;
        }

        public virtual void BuildAddressLabel(AsmLoad asmLoad)
        {
            Label?.SetValueAndAddress(Address, asmLoad);
        }

        public virtual void Assemble( AsmLoad asmLoad)
        {
        }
    }
}
