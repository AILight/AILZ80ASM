using System;
using System.Collections.Generic;
using System.Text;

namespace AILZ80ASM
{
    public class OperationItemOPCode : IOperationItem
    {
        private OPCodeResult OPCodeResult { get; set; }
        private LineItem LineItem { get; set; }

        private OperationItemOPCode(OPCodeResult opCodeResult, LineItem lineItem, UInt16 address)
        {
            OPCodeResult = opCodeResult;
            LineItem = lineItem;
            Address = address;
        }

        public static IOperationItem Perse(LineItem lineItem, UInt16 address)
        {
            var returnValue = default(OperationItemOPCode);
            var code = lineItem.Label.OperationCodeWithoutLabel;
            if (!string.IsNullOrEmpty(code))
            {
                var opCodeResult = OPCodeTable.GetOPCodeItem(code);
                if (opCodeResult != default(OPCodeResult))
                {
                    returnValue = new OperationItemOPCode(opCodeResult, lineItem, address);
                }
            }

            return returnValue;
        }

        public void Assemble(Label[] labels)
        {
            OPCodeResult.Assemble(LineItem, labels);
        }

        public byte[] Bin => OPCodeResult.ToBin();

        public UInt16 Address { get; set; }
        public UInt16 NextAddress => (UInt16)(Address + OPCodeResult.OPCode.Length);
    }
}
