using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace AILZ80ASM
{
    public class LineDetailItemMacroDefine : LineDetailItem
    {
        private static readonly string RegexPatternMacroStart = @"^\s*Macro\s+(?<macro_name>[a-zA-Z0-9_]+)($|\s+(?<args>.+)$)";
        private static readonly string RegexPatternMacroEnd = @"^\s*End\s+Macro\s*$";

        private string MacroName { get; set; } = "";
        private string[] MacroArgs { get; set; }
        private readonly List<LineItem> MacroLines = new List<LineItem>();

        public LineDetailItemMacroDefine(LineItem lineItem, AsmLoad asmLoad)
            : base(lineItem, asmLoad)
        {

        }

        public static LineDetailItemMacroDefine Create(LineItem lineItem, AsmLoad asmLoad)
        {
            var startMatched = Regex.Match(lineItem.OperationString, RegexPatternMacroStart, RegexOptions.Singleline | RegexOptions.IgnoreCase);
            var endMatched = Regex.Match(lineItem.OperationString, RegexPatternMacroEnd, RegexOptions.Singleline | RegexOptions.IgnoreCase);

            if (asmLoad.LineDetailItemForExpandItem is LineDetailItemMacroDefine asmLoad_LineDetailItemMacro)
            {
                if (startMatched.Success)
                {
                    throw new ErrorMessageException(Error.ErrorCodeEnum.E1001);
                }

                if (endMatched.Success)
                {
                    // Macro登録
                    var lineDetailItemMacro = asmLoad_LineDetailItemMacro;
                    var macro = new Macro(lineDetailItemMacro.MacroName, lineDetailItemMacro.MacroArgs, lineDetailItemMacro.MacroLines.ToArray(), asmLoad);
                    asmLoad.Macros.Add(macro);

                    // 終了
                    asmLoad.LineDetailItemForExpandItem = default;
                }
                else
                {
                    // ローカルラベル以外は使用禁止
                    var lable = Label.GetLabelText(lineItem.OperationString);
                    if (lable.EndsWith(":"))
                    {
                        throw new ErrorMessageException(Error.ErrorCodeEnum.E1006);
                    }

                    asmLoad_LineDetailItemMacro.MacroLines.Add(lineItem);
                }
                return new LineDetailItemMacroDefine(lineItem, asmLoad);
            }
            else
            {
                if (startMatched.Success)
                {
                    var args = new List<string>();
                    var argsText = startMatched.Groups["args"].Value;

                    if (!string.IsNullOrEmpty(argsText.Trim()))
                    {
                        args.AddRange(argsText.Split(',').Select(m => m.Trim()));
                    }

                    var lineDetailItemMacro = new LineDetailItemMacroDefine(lineItem, asmLoad)
                    {
                        MacroName = startMatched.Groups["macro_name"].Value,
                        MacroArgs = args.ToArray(),
                    };

                    // 引数、有効文字チェック
                    if (!args.All(m => Label.IsArgument(m)))
                    {
                        lineDetailItemMacro.InternalErrorMessageException = new ErrorMessageException(Error.ErrorCodeEnum.E1005);
                    }

                    asmLoad.LineDetailItemForExpandItem = lineDetailItemMacro;

                    return lineDetailItemMacro;
                }

                if (endMatched.Success)
                {
                    throw new ErrorMessageException(Error.ErrorCodeEnum.E1002);
                }
            }
            return default;
        }

        public override void ExpansionItem()
        {
            LineDetailScopeItems = Array.Empty<LineDetailScopeItem>();
        }
    }
}
