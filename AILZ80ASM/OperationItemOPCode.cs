using System;
using System.Collections.Generic;
using System.Text;

namespace AILZ80ASM
{
    public class OperationItemOPCode : IOperationItem
    {
        private OPCodeResult OPCodeResult { get; set; }
        private LineDetailExpansionItemOperation LineDetailExpansionItemOperation { get; set; }

        private OperationItemOPCode(OPCodeResult opCodeResult, LineDetailExpansionItemOperation lineDetailExpansionItemOperation, AsmAddress address)
        {
            OPCodeResult = opCodeResult;
            LineDetailExpansionItemOperation = lineDetailExpansionItemOperation;
            Address = address;
        }

        public static IOperationItem Parse(LineDetailExpansionItemOperation lineDetailExpansionItemOperation, AsmAddress address, AsmLoad asmLoad)
        {
            var returnValue = default(OperationItemOPCode);
            var code = $"{lineDetailExpansionItemOperation.InstructionText} {lineDetailExpansionItemOperation.ArgumentText}";
            if (!string.IsNullOrEmpty(code))
            {
                var opCodeResult = OPCodeTable.GetOPCodeItem(code);
                if (opCodeResult != default(OPCodeResult))
                {
                    returnValue = new OperationItemOPCode(opCodeResult, lineDetailExpansionItemOperation, address);
                }
            }

            return returnValue;
        }

        public void Assemble(AsmLoad asmLoad)
        {
            OPCodeResult.Assemble(LineDetailExpansionItemOperation, asmLoad);
        }

        public byte[] Bin => OPCodeResult.ToBin();

        public AsmAddress Address { get; set; }
        public AsmLength Length => new AsmLength(OPCodeResult.OPCode.Length);
    }
}
