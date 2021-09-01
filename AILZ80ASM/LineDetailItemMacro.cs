using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace AILZ80ASM
{
    public class LineDetailItemMacro : LineDetailItem
    {
        private static readonly string RegexPatternMacroStart = @"^\s*Macro\s+(?<macro_name>[a-zA-Z0-9_]+)($|\s+(?<args>.+)$)";
        private static readonly string RegexPatternMacroEnd = @"^\s*End\s+Macro\s*$";

        private string MacroName { get; set; } = "";
        private string MacroArgs { get; set; } = "";
        private List<LineItem> MacroLines = new List<LineItem>();

        public LineDetailItemMacro()
        {

        }

        public static LineDetailItemMacro Create(string lineString, AsmLoad asmLoad)
        {
            var startMatched = Regex.Match(lineString, RegexPatternMacroStart, RegexOptions.Singleline | RegexOptions.IgnoreCase);
            var endMatched = Regex.Match(lineString, RegexPatternMacroEnd, RegexOptions.Singleline | RegexOptions.IgnoreCase);

            if (asmLoad.LineDetailItemMacro != default)
            {
                if (startMatched.Success)
                {
                    throw new ErrorMessageException(Error.ErrorCodeEnum.E0010);
                }

                if (endMatched.Success)
                {
                    // Macro登録
                    var lineDetailItemMacro = asmLoad.LineDetailItemMacro;
                    var macro = new Macro(lineDetailItemMacro.MacroName, lineDetailItemMacro.MacroArgs, lineDetailItemMacro.MacroLines.ToArray(), asmLoad);
                    asmLoad.Macros.Add(macro);

                    // 終了
                    asmLoad.LineDetailItemMacro = default;
                }
                else
                {
                    asmLoad.LineDetailItemMacro.MacroLines.Add(new LineItem(lineString, asmLoad.LineDetailItemMacro.MacroLines.Count + 1, asmLoad));
                }
                return new LineDetailItemMacro();
            }
            else
            {
                if (startMatched.Success)
                {
                    var lineDetailItemMacro = new LineDetailItemMacro();

                    lineDetailItemMacro.MacroName = startMatched.Groups["macro_name"].Value;
                    lineDetailItemMacro.MacroArgs = startMatched.Groups["args"].Value;

                    asmLoad.LineDetailItemMacro = lineDetailItemMacro;

                    return lineDetailItemMacro;
                }

                if (endMatched.Success)
                {
                    throw new ErrorMessageException(Error.ErrorCodeEnum.E0014);
                }
            }
            return default;
        }
    }
}
