using AILZ80ASM.Assembler;
using AILZ80ASM.LineDetailItems.ScopeItem;
using AILZ80ASM.OperationItems;

namespace AILZ80ASM.LineDetailItems
{
    public class LineDetailItemOperation : LineDetailItem
    {
        private LineDetailItemOperation(LineItem lineItem, OperationItem operationItem, AsmLoad asmLoad)
            : base(lineItem, asmLoad)
        {
            this.LineDetailScopeItems = new[]
            {
                new LineDetailScopeItem(this.LineItem, operationItem, asmLoad)
            };
        }

        public static LineDetailItem Create(LineItem lineItem, AsmLoad asmLoad)
        {
            if (!lineItem.IsCollectOperationString)
            {
                return default(LineDetailItemOperation);
            }

            var operationItem = default(OperationItem);

            operationItem ??= OperationItemOPCode.Create(lineItem, asmLoad);
            operationItem ??= OperationItemData.Create(lineItem, asmLoad);
            operationItem ??= OperationItemDataFill.Create(lineItem, asmLoad);
            operationItem ??= OperationItemNone.Create(lineItem, asmLoad);

            if (operationItem != default)
            {
                return new LineDetailItemOperation(lineItem, operationItem, asmLoad);
            }

            return default;
        }
    }
}
