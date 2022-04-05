using AILZ80ASM.LineDetailItems;
using System;
using System.Collections.Generic;

namespace AILZ80ASM.Assembler
{
    public class AsmORG
    {
        public enum ORGTypeEnum
        {
            ORG,
            ALIGN,
            DS,
            NextORG,
        }

        public UInt16 ProgramAddress { get; private set; }
        public UInt32 OutputAddress { get; private set; }
        public byte FillByte { get; private set; }
        public List<LineDetailItem> LineDetailItems { get; private set; }
        public LineItem LineItem { get; private set; } = default;
        public ORGTypeEnum ORGType { get; private set; } = ORGTypeEnum.ORG;

        public AsmORG()
            : this(0, 0, 0, ORGTypeEnum.ORG)
        {

        }

        public AsmORG(UInt16 programAddress, UInt32 outputAddress, byte fillByte, ORGTypeEnum orgType)
        {
            ProgramAddress = programAddress;
            OutputAddress = outputAddress;
            FillByte = fillByte;
            LineDetailItems = new List<LineDetailItem>();
            ORGType = orgType;
        }

        public void AddScopeItem(LineDetailItem lineDetailItem)
        {
            LineDetailItems.Add(lineDetailItem);
        }
    }
}
