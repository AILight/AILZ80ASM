namespace AILZ80ASM
{
    public class LineDetailItemOperation : LineDetailItem
    {
        private LineDetailItemOperation(LineItem lineItem, AsmLoad asmLoad)
            : base(lineItem, asmLoad)
        {
            this.LineDetailScopeItems = new[]
            {
                new LineDetailScopeItem(this.LineItem, this.AsmLoad)
            };
            asmLoad.SetScope(this.AsmLoad);
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
