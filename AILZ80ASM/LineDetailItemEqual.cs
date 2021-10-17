using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace AILZ80ASM
{
    public class LineDetailItemEqual : LineDetailItem
    {
        private static readonly string RegexPatternEqual = @"^equ\s+(?<value>.+)";
        private Label EqualLabel { get; set; }

        private LineDetailItemEqual(LineItem lineItem, AsmLoad asmLoad)
            : base(lineItem, asmLoad)
        {

        }

        public static LineDetailItemEqual Create(LineItem lineItem, AsmLoad asmLoad)
        {
            var matched = Regex.Match(lineItem.OperationString, RegexPatternEqual, RegexOptions.Singleline | RegexOptions.IgnoreCase);
            if (matched.Success)
            {
                var lableValue = matched.Groups["value"].Value.Trim();

                var label = new Label(lineItem.LabelString, lableValue, asmLoad);
                if (label.Invalidate)
                {
                    throw new ErrorAssembleException(Error.ErrorCodeEnum.E0013);
                }

                label.SetValue(asmLoad);
                asmLoad.AddLabel(label);

                return new LineDetailItemEqual(lineItem, asmLoad);
            }

            return default;
        }

        public override void ExpansionItem()
        {
            LineDetailScopeItems = new[]
            {
                new LineDetailScopeItem(AsmLoad)
                {
                    LineDetailExpansionItems = new [] { new LineDetailExpansionItem(this.LineItem) { Label = EqualLabel } }
                }
            };

            base.ExpansionItem();
        }
    }
}
