using AILZ80ASM.LineDetailItems;
using AILZ80ASM.LineDetailItems.ScopeItem;
using System;
using System.Linq;
using System.Collections.Generic;
using AILZ80ASM.Exceptions;

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
        public string OutputAddressLabel { get; private set; }
        public string FillByteLabel { get; private set; }
        public bool IsRomMode => !string.IsNullOrEmpty(OutputAddressLabel);
        public List<LineDetailItem> LineDetailItems { get; private set; }
        public LineItem LineItem { get; private set; } = default;
        public ORGTypeEnum ORGType { get; private set; } = ORGTypeEnum.ORG;
        public bool HasBinResult => LineDetailItems.Any(m => m.BinResults.Any());

        public byte FillByte { get; internal set; }

        public AsmORG()
            : this(0, "", "", default(LineItem), ORGTypeEnum.ORG)
        {

        }

        public AsmORG(UInt16 programAddress, string outputAddressLabel, string fillByteLabel, LineItem lineItem, ORGTypeEnum orgType)
        {
            ProgramAddress = programAddress;
            OutputAddressLabel = outputAddressLabel;
;
            FillByteLabel = fillByteLabel;
            LineDetailItems = new List<LineDetailItem>();
            LineItem = lineItem;
            ORGType = orgType;
        }

        public void AddLineDetailItem(LineDetailItem lineDetailItem)
        {
            LineDetailItems.Add(lineDetailItem);
        }

        public void AdjustAssemble(UInt32 outputAddress, AsmLoad asmLoad)
        {
            OutputAddress = outputAddress;

            foreach (var lineDetailItem in LineDetailItems)
            {
                try
                {
                    lineDetailItem.AdjustAssemble(ref outputAddress);
                }
                catch (ErrorAssembleException ex)
                {
                    asmLoad.AddError(new ErrorLineItem(lineDetailItem.LineItem, ex));
                }
                catch (ErrorLineItemException ex)
                {
                    asmLoad.AddError(ex.ErrorLineItem);
                }
            }
        }
    }
}
