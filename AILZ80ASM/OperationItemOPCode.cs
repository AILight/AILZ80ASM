namespace AILZ80ASM
{
    public class OperationItemOPCode<T> : OperationItem
        where T : Instructions.ISA, new()
    {
        private T ISA { get; set; }

        public override byte[] Bin => ISA.ToBin();
        public override AsmList List
        {
            get
            {
                return AsmList.CreateLineItem(Address, Bin, ISA.InstructionItem.T == 0 ? "" : ISA.InstructionItem.T.ToString(), LineDetailExpansionItemOperation.LineItem);
            }
        }
        public override AsmLength Length => new AsmLength(ISA.OPCodeLength);

        private OperationItemOPCode(T isa, LineDetailExpansionItemOperation lineDetailExpansionItemOperation, AsmAddress address)
        {
            ISA = isa;
            LineDetailExpansionItemOperation = lineDetailExpansionItemOperation;
            Address = address;
        }

        public static bool CanCreate(string operation)
        {
            if (string.IsNullOrEmpty(operation))
            {
                //空文字もOperationItemOPCodeとして処理をする
                return true;
            }

            var isa = new T() { Instruction = operation };
            if (isa.PreAssemble())
            {
                return true;
            }

            return false;
        }

        public new static OperationItem Create(LineDetailExpansionItemOperation lineDetailExpansionItemOperation, AsmAddress address, AsmLoad asmLoad)
        {
            var returnValue = default(OperationItemOPCode<T>);
            var code = lineDetailExpansionItemOperation.LineItem.OperationString;
            if (!string.IsNullOrEmpty(code))
            {
                var isa = new T() { Instruction = code };
                if (isa.PreAssemble())
                {
                    returnValue = new OperationItemOPCode<T>(isa, lineDetailExpansionItemOperation, address);
                }
            }

            return returnValue;
        }

        public override void Assemble(AsmLoad asmLoad)
        {
            ISA.Assemble(LineDetailExpansionItemOperation, asmLoad);
        }

    }
}
