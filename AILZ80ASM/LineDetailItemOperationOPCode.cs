using AILZ80ASM.Assembler;

namespace AILZ80ASM
{
    public class LineDetailItemOperationOPCode : LineDetailItem
    {
        private LineDetailItemOperationOPCode(LineItem lineItem, AsmLoad asmLoad)
            : base(lineItem, asmLoad)
        {
            this.LineDetailScopeItems = new[]
            {
                new LineDetailScopeItem(this.LineItem, asmLoad)
            };
        }

        public static LineDetailItem Create(LineItem lineItem, AsmLoad asmLoad)
        {
            if (OperationItem.CanCreate(lineItem.OperationString, asmLoad))
            {
                return new LineDetailItemOperationOPCode(lineItem, asmLoad);
            }

            return default;
        }
    }
}
