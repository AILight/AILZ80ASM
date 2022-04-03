using AILZ80ASM.Assembler;
using AILZ80ASM.Exceptions;
using System.Text.RegularExpressions;

namespace AILZ80ASM
{
    public class LineDetailItemEqual : LineDetailItem
    {
        private static readonly string RegexPatternEqual = @"^equ\s+(?<value>.+)";
        public Label EquLabel { get; set; }
        public string LabelValue { get; set; }

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

        private LineDetailItemEqual(LineItem lineItem, string labelValue, AsmLoad asmLoad)
            : base(lineItem, asmLoad)
        {
            LabelValue = labelValue;
        }

        public static LineDetailItemEqual Create(LineItem lineItem, AsmLoad asmLoad)
        {
            var matched = Regex.Match(lineItem.OperationString, RegexPatternEqual, RegexOptions.Singleline | RegexOptions.IgnoreCase);
            if (matched.Success)
            {
                var labelValue = matched.Groups["value"].Value.Trim();

                return new LineDetailItemEqual(lineItem, labelValue, asmLoad);
            }

            return default;
        }
    }
}
