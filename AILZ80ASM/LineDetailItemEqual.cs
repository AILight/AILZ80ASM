using System.Text.RegularExpressions;

namespace AILZ80ASM
{
    public class LineDetailItemEqual : LineDetailItem
    {
        private static readonly string RegexPatternEqual = @"^equ\s+(?<value>.+)";
        private Label EquLabel { get; set; }
        private AsmAddress AsmAddress { get; set; }
        public override AsmList[] Lists
        {
            get
            {
                return new[]
                {
                    AsmList.CreateLineItemEqual(EquLabel, this.LineItem)
                };
            }
        }

        private LineDetailItemEqual(LineItem lineItem, AsmLoad asmLoad)
            : base(lineItem, asmLoad)
        {

        }

        public static LineDetailItemEqual Create(LineItem lineItem, AsmLoad asmLoad)
        {
            var matched = Regex.Match(lineItem.OperationString, RegexPatternEqual, RegexOptions.Singleline | RegexOptions.IgnoreCase);
            if (matched.Success)
            {
                var labelValue = matched.Groups["value"].Value.Trim();

                var label = new Label(lineItem.LabelString, labelValue, asmLoad);
                if (label.Invalidate)
                {
                    throw new ErrorAssembleException(Error.ErrorCodeEnum.E0013);
                }

                label.SetValue(asmLoad);
                asmLoad.AddLabel(label);

                return new LineDetailItemEqual(lineItem, asmLoad) { EquLabel = label };
            }

            return default;
        }

        public override void ExpansionItem()
        {
            LineDetailScopeItems = new[]
            {
                new LineDetailScopeItem(AsmLoad)
                {
                    LineDetailExpansionItems = new [] { new LineDetailExpansionItem(this.LineItem) }
                }
            };

            base.ExpansionItem();
        }

        public override void PreAssemble(ref AsmAddress asmAddress)
        {
            AsmAddress = asmAddress;

            base.PreAssemble(ref asmAddress);
        }

        public override void BuildAddressLabel()
        {
            if (!EquLabel.HasValue)
            {
                EquLabel.SetValueAndAddress(AsmAddress, AsmLoad);
            }
        }
    }
}
