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

        public static LineDetailItem CreateLineDetailItem(string operationString, AsmLoad asmLoad)
        {
            // ラベルの処理
            ProcessAsmLoad(operationString, asmLoad);

            // インクルードのチェック
            var lineDetailItem = default(LineDetailItem);

            lineDetailItem = lineDetailItem ?? LineDetailItemMacro.Create(operationString, asmLoad);
            lineDetailItem = lineDetailItem ?? LineDetailItemEqual.Create(operationString, asmLoad);
            lineDetailItem = lineDetailItem ?? LineDetailItemInclude.Create(operationString, asmLoad);
            lineDetailItem = lineDetailItem ?? new LineDetailItemOperation(operationString);

            return lineDetailItem;
        }

        private static void ProcessAsmLoad(string operationString, AsmLoad asmLoad)
        {
            var labelMatched = Regex.Match(operationString, RegexPatternLabel, RegexOptions.Singleline | RegexOptions.IgnoreCase);
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

        public virtual LineAssemblyItem[] LineAssemblyItems { get; } = new LineAssemblyItem[] { };
    }
}
