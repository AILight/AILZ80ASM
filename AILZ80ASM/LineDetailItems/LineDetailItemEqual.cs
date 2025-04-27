using AILZ80ASM.AILight;
using AILZ80ASM.Assembler;
using AILZ80ASM.Exceptions;
using System.Text.RegularExpressions;

namespace AILZ80ASM.LineDetailItems
{
    public class LineDetailItemEqual : LineDetailItem
    {
        private static readonly string RegexPatternEqual = @"^equ\s+(?<value>.+)";
        private static readonly Regex CompiledRegexPatternEqual = new Regex(
            RegexPatternEqual,
            RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase
        );
        public Label EquLabel { get; set; }
        public string LabelValue { get; set; }

        public override AsmList[] Lists
        {
            get
            {
                if (!AsmLoad.Share.IsOutputList)
                {
                    return new AsmList[] { };
                }

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
            if (!lineItem.IsCollectOperationString)
            {
                return default(LineDetailItemEqual);
            }

            var matched = CompiledRegexPatternEqual.Match(lineItem.OperationString);
            if (matched.Success)
            {
                var labelValue = matched.Groups["value"].Value.Trim();

                return new LineDetailItemEqual(lineItem, labelValue, asmLoad);
            }

            return default;
        }

        public override void PreAssemble(ref AsmAddress asmAddress)
        {
            base.PreAssemble(ref asmAddress);

            if (this.EquLabel == default)
            {
                this.AsmLoad.AddError(new ErrorLineItem(this.LineItem, Error.ErrorCodeEnum.E0027));
            }
        }
    }
}
