using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace AILZ80ASM
{
    public class LineDetailItemEqual : LineDetailItem
    {
        private static readonly string RegexPatternEqual = @"^\s*(?<label>\.?[a-zA-Z0-9_]+:?)\s+equ\s+(?<value>.+)";

        public LineDetailItemEqual()
        {

        }

        public static LineDetailItemEqual Create(string lineString, AsmLoad asmLoad)
        {
            var matched = Regex.Match(lineString, RegexPatternEqual, RegexOptions.Singleline | RegexOptions.IgnoreCase);
            if (matched.Success)
            {
                var labelName = matched.Groups["label"].Value;
                var lableValue = matched.Groups["value"].Value.Trim();

                // エラーチェック
                if (labelName.StartsWith(".") && labelName.EndsWith(":"))
                {
                    throw new ErrorMessageException(Error.ErrorCodeEnum.E0013);
                }

                if (!labelName.StartsWith("."))
                {
                    asmLoad.LabelName = labelName.Replace(":", "");
                }

                var label = new Label(labelName, lableValue, asmLoad);
                asmLoad.Labels.Add(label);

                return new LineDetailItemEqual()
                {
                    LineDetailExpansionItems = new[] 
                    {
                        new LineDetailExpansionItem { Label = label } 
                    }
                };
            }

            return default;
        }
    }
}
