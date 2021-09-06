using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace AILZ80ASM
{
    public class LineDetailItemOperation : LineDetailItem
    {
        public LineDetailItemOperation(LineItem lineItem, AsmLoad asmLoad)
            : base(lineItem, asmLoad)
        {
        }

        public override void ExpansionItem()
        {
            this.LineDetailExpansionItems = Macro.Expansion(this.LineItem, this.AsmLoad);
            if (this.LineDetailExpansionItems != default)
                return;
            // マクロ展開できなかったら通常展開
            this.LineDetailExpansionItems = new[]
            {
                new LineDetailExpansionItemOperation(this.LineItem, this.AsmLoad)
            };

            base.ExpansionItem();
        }

    }
}
