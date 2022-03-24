using System;
using System.Collections.Generic;

namespace AILZ80ASM.Assembler
{
    public class AsmORG
    {
        public UInt16 ProgramAddress { get; private set; }
        public UInt32 OutputAddress { get; private set; }
        public byte FillByte { get; private set; }
        public List<LineDetailItem> LineDetailItems { get; private set; }
        public LineItem LineItem { get; set; } = default;

        public AsmORG()
            : this(0, 0, default(byte))
        {

        }

        public AsmORG(UInt16 programAddress, UInt32 outputAddress, byte fillByte)
        {
            ProgramAddress = programAddress;
            OutputAddress = outputAddress;
            FillByte = fillByte;
            LineDetailItems = new List<LineDetailItem>();
        }

        public void AddScopeItem(LineDetailItem lineDetailItem)
        {
            LineDetailItems.Add(lineDetailItem);
        }
    }
}
