using System;
using System.Collections.Generic;
using System.Text;

namespace AILZ80ASM
{
    public class OperationItemOPCode : IOperationItem
    {
        private OPCodeResult OPCodeResult { get; set; }
        private LineExpansionItem LineExpansionItem { get; set; }

        private OperationItemOPCode(OPCodeResult opCodeResult, LineExpansionItem lineExpansionItem, AsmAddress address)
        {
            OPCodeResult = opCodeResult;
            LineExpansionItem = lineExpansionItem;
            Address = address;
        }

        public static IOperationItem Parse(LineExpansionItem lineExpansionItem, AsmAddress address)
        {
            var returnValue = default(OperationItemOPCode);
            var code = lineExpansionItem.Label.OperationCodeWithoutLabel;
            if (!string.IsNullOrEmpty(code))
            {
                var opCodeResult = OPCodeTable.GetOPCodeItem(code);
                if (opCodeResult != default(OPCodeResult))
                {
                    returnValue = new OperationItemOPCode(opCodeResult, lineExpansionItem, address);
                }
            }

            return returnValue;
        }

        public void Assemble(Label[] labels)
        {
            OPCodeResult.Assemble(LineExpansionItem, labels);
        }

        public byte[] Bin => OPCodeResult.ToBin();

        public AsmAddress Address { get; set; }
        public AsmLength Length => new AsmLength(OPCodeResult.OPCode.Length);
    }
}
