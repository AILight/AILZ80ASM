using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace AILZ80ASM
{
    public abstract class LineDetailItem
    {
        private static readonly string RegexPatternLabel = @"^\s*(?<label>[a-zA-Z0-9_]+::?)";
        protected LineItem LineItem { get; set; }

        public LineDetailExpansionItem[] LineDetailExpansionItems { get; set; }

        public LineDetailItem(LineItem lineItem)
        {
            LineItem = lineItem;
        }

        public static LineDetailItem CreateLineDetailItem(LineItem lineItem, AsmLoad asmLoad)
        {
            // ラベルの処理
            ProcessAsmLoad(lineItem, asmLoad);

            // インクルードのチェック
            var lineDetailItem = default(LineDetailItem);

            lineDetailItem = lineDetailItem ?? LineDetailItemMacro.Create(lineItem, asmLoad);
            lineDetailItem = lineDetailItem ?? LineDetailItemRepeat.Create(lineItem, asmLoad);
            lineDetailItem = lineDetailItem ?? LineDetailItemEqual.Create(lineItem, asmLoad);
            lineDetailItem = lineDetailItem ?? LineDetailItemInclude.Create(lineItem, asmLoad);
            lineDetailItem = lineDetailItem ?? new LineDetailItemOperation(lineItem);

            return lineDetailItem;
        }

        private static void ProcessAsmLoad(LineItem lineItem, AsmLoad asmLoad)
        {
            var labelMatched = Regex.Match(lineItem.OperationString, RegexPatternLabel, RegexOptions.Singleline | RegexOptions.IgnoreCase);
            if (labelMatched.Success)
            {
                var label = labelMatched.Groups["label"].Value;
                if (label.EndsWith("::"))
                {
                    asmLoad.GlobalLableName = label.Substring(0, label.Length - 2);
                }
                else if (label.EndsWith(":"))
                {
                    asmLoad.LabelName = label.Substring(0, label.Length - 1);
                }
            }
        }

        public virtual void ExpansionItem(AsmLoad asmLoad)
        {
            
        }

        public virtual LineAssemblyItem[] LineAssemblyItems { get; } = new LineAssemblyItem[] { };
    }
}
