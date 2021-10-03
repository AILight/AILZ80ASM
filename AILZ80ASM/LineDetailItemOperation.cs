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
            // マクロであるか調べる
            this.LineDetailScopeItems = Macro.Expansion(this.LineItem, this.AsmLoad);
            if (this.LineDetailScopeItems != default)
                return;

            // マクロ展開できなかったら通常展開
            this.LineDetailScopeItems = new[]
            {
                new LineDetailScopeItem(this.LineItem, this.AsmLoad)
            };

            base.ExpansionItem();
        }

    }
}
