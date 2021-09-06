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
        private readonly List<string> MacroLines = new List<string>();

        public LineDetailItemMacro(LineItem lineItem, AsmLoad asmLoad)
            : base(lineItem, asmLoad)
        {

        }

        public static LineDetailItemMacro Create(LineItem lineItem, AsmLoad asmLoad)
        {
            var startMatched = Regex.Match(lineItem.OperationString, RegexPatternMacroStart, RegexOptions.Singleline | RegexOptions.IgnoreCase);
            var endMatched = Regex.Match(lineItem.OperationString, RegexPatternMacroEnd, RegexOptions.Singleline | RegexOptions.IgnoreCase);

            if (asmLoad.LineDetailItemMacro != default)
            {
                if (startMatched.Success)
                {
                    throw new ErrorMessageException(Error.ErrorCodeEnum.E1001);
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
                    asmLoad.LineDetailItemMacro.MacroLines.Add(lineItem.LineString);
                }
                return new LineDetailItemMacro(lineItem, asmLoad) { LineDetailExpansionItems = Array.Empty<LineDetailExpansionItem>() };
            }
            else
            {
                if (startMatched.Success)
                {
                    var lineDetailItemMacro = new LineDetailItemMacro(lineItem, asmLoad)
                    {
                        MacroName = startMatched.Groups["macro_name"].Value,
                        MacroArgs = startMatched.Groups["args"].Value,
                        LineDetailExpansionItems = Array.Empty<LineDetailExpansionItem>()
                    };

                    asmLoad.LineDetailItemMacro = lineDetailItemMacro;

                    return lineDetailItemMacro;
                }

                if (endMatched.Success)
                {
                    throw new ErrorMessageException(Error.ErrorCodeEnum.E1002);
                }
            }
            return default;
        }

    }
}
