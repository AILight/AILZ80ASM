using AILZ80ASM.Assembler;
using AILZ80ASM.LineDetailItems.ScopeItem;
using AILZ80ASM.OperationItems;

namespace AILZ80ASM.LineDetailItems
{
    public class LineDetailItemOperation : LineDetailItem
    {
        private LineDetailItemOperation(LineItem lineItem, AsmLoad asmLoad)
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
                return new LineDetailItemOperation(lineItem, asmLoad);
            }

            return default;
        }
    }
}
