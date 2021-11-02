namespace AILZ80ASM
{
    public class OperationItemOPCode : OperationItem
    {
        private OPCodeResult OPCodeResult { get; set; }

        public override byte[] Bin => OPCodeResult.ToBin();
        public override AsmList List
        {
            get
            {
                return AsmList.CreateLineItem(Address, Bin, OPCodeResult.T == 0 ? "" : OPCodeResult.T.ToString(), LineDetailExpansionItemOperation.LineItem);
            }
        }
        public override AsmLength Length => new AsmLength(OPCodeResult.OPCode.Length);

        private OperationItemOPCode(OPCodeResult opCodeResult, LineDetailExpansionItemOperation lineDetailExpansionItemOperation, AsmAddress address)
        {
            OPCodeResult = opCodeResult;
            LineDetailExpansionItemOperation = lineDetailExpansionItemOperation;
            Address = address;
        }

        public new static bool CanCreate(string operation)
        {
            if (string.IsNullOrEmpty(operation))
            {
                //空文字もOperationItemOPCodeとして処理をする
                return true;
            }

            var opCodeResult = OPCodeTable.GetOPCodeItem(operation);
            if (opCodeResult != default)
            {
                return true;
            }

            return false;
        }

        public new static OperationItem Create(LineDetailExpansionItemOperation lineDetailExpansionItemOperation, AsmAddress address, AsmLoad asmLoad)
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

        public override void Assemble(AsmLoad asmLoad)
        {
            OPCodeResult.Assemble(LineDetailExpansionItemOperation, asmLoad);
        }

    }
}
