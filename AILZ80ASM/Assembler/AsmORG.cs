using AILZ80ASM.LineDetailItems;
using AILZ80ASM.LineDetailItems.ScopeItem;
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

        public AsmAddress Address { get; private set; }
        public bool IsRomMode { get; set; }
        public byte FillByte { get; private set; }
        public List<LineDetailScopeItem> LineDetailScopeItems { get; private set; }
        public LineItem LineItem { get; private set; } = default;
        public ORGTypeEnum ORGType { get; private set; } = ORGTypeEnum.ORG;

        public AsmORG()
            : this(new AsmAddress(), false, 0, ORGTypeEnum.ORG)
        {

        }

        public AsmORG(AsmAddress address, bool isRomMode, byte fillByte, ORGTypeEnum orgType)
        {
            Address = address;
            IsRomMode = isRomMode;
            FillByte = fillByte;
            LineDetailScopeItems = new List<LineDetailScopeItem>();
            ORGType = orgType;
        }

        public void AddScopeItem(LineDetailScopeItem lineDetailScopeItem)
        {
            LineDetailScopeItems.Add(lineDetailScopeItem);
        }

        public void ResetAddress(ref AsmAddress asmAddress)
        {
            foreach (var lineDetailScopeItem in LineDetailScopeItems)
            {
                lineDetailScopeItem.ResetAddress(ref asmAddress);

            }
        }
    }
}
