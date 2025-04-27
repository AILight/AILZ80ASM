using AILZ80ASM.AILight;
using AILZ80ASM.Assembler;
using AILZ80ASM.Exceptions;
using AILZ80ASM.LineDetailItems.ScopeItem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AILZ80ASM.LineDetailItems
{
    public class LineDetailItemMacroDefine : LineDetailItem
    {
        private string MacroName { get; set; } = "";
        private string[] MacroArgs { get; set; }
        private readonly List<LineItem> MacroLines = new List<LineItem>();

        private static readonly string RegexPatternMacroStart = @"^(?<macro_name>[a-zA-Z0-9_\(\)]+)\s+Macro($|\s+(?<args>.+)$)";
        private static readonly Regex CompiledRegexPatternMacroStart = new Regex(
            RegexPatternMacroStart,
            RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase
        );

        private static readonly string RegexPatternMacroEnd = @"^\s*Endm\s*$";
        private static readonly Regex CompiledRegexPatternMacroEnd = new Regex(
            RegexPatternMacroEnd,
            RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase
        );

        public override AsmList[] Lists
        {
            get
            {
                if (!AsmLoad.Share.IsOutputList)
                {
                    return new AsmList[] { };
                }

                var lists = new List<AsmList>();
                foreach (var item in MacroLines)
                {
                    lists.Add(AsmList.CreateLineItem(item));
                }

                return lists.ToArray();
            }
        }

        public LineDetailItemMacroDefine(LineItem lineItem, AsmLoad asmLoad)
            : base(lineItem, asmLoad)
        {

        }

        public static LineDetailItemMacroDefine Create(LineItem lineItem, AsmLoad asmLoad)
        {
            if (!lineItem.IsCollectOperationString)
            {
                return default(LineDetailItemMacroDefine);
            }

            var startMatched = CompiledRegexPatternMacroStart.Match(lineItem.OperationString);
            var endMatched = CompiledRegexPatternMacroEnd.Match(lineItem.OperationString);

            if (asmLoad.Share.LineDetailItemForExpandItem is LineDetailItemMacroDefine asmLoad_LineDetailItemMacro)
            {
                if (startMatched.Success)
                {
                    throw new ErrorAssembleException(Error.ErrorCodeEnum.E3001);
                }

                // ローカルラベル以外は使用禁止
                if ((lineItem.LabelString.StartsWith('[') && 
                     lineItem.LabelString.EndsWith(']')) || lineItem.LabelString.EndsWith(':'))
                {
                    asmLoad.Share.LineDetailItemForExpandItem.Errors.Add(new ErrorLineItem(lineItem, Error.ErrorCodeEnum.E3006));
                }
                else
                {
                    asmLoad_LineDetailItemMacro.MacroLines.Add(lineItem);
                }

                // リピート開始
                if (LineDetailItemRepeat.IsMatchStart(lineItem))
                {
                    asmLoad.Share.LineDetailItemForExpandItem.NestCounter++;
                }

                // リピート終了
                if (asmLoad.Share.LineDetailItemForExpandItem.NestCounter > 0)
                {
                    if (LineDetailItemRepeat.IsMatchEnd(lineItem))
                    {
                        asmLoad.Share.LineDetailItemForExpandItem.NestCounter--;
                    }
                }
                else
                {
                    if (endMatched.Success)
                    {
                        // Macro登録
                        var lineDetailItemMacro = asmLoad_LineDetailItemMacro;
                        var macro = new Macro(lineDetailItemMacro.MacroName, lineDetailItemMacro.MacroArgs, lineDetailItemMacro.MacroLines.ToArray(), asmLoad);
                        // 同名マクロチェック
                        if (asmLoad.FindMacro(macro.FullName) != default)
                        {
                            asmLoad.Share.LineDetailItemForExpandItem.Errors.Add(new ErrorLineItem(asmLoad_LineDetailItemMacro.LineItem, Error.ErrorCodeEnum.E3010));
                        }

                        if (asmLoad.Share.LineDetailItemForExpandItem.Errors.Where(m => m.ErrorCode != Error.ErrorCodeEnum.E3006).Count() == 0)
                        {
                            asmLoad.AddMacro(macro);
                        }

                        asmLoad.AddErrors(asmLoad.Share.LineDetailItemForExpandItem.Errors);

                        // 終了
                        asmLoad.Share.LineDetailItemForExpandItem = default;
                    }
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

                    var lineDetailItemMacro = new LineDetailItemMacroDefine(lineItem, asmLoad);
                    lineDetailItemMacro.MacroName = startMatched.Groups["macro_name"].Value;
                    lineDetailItemMacro.MacroArgs = args.ToArray();
                    lineDetailItemMacro.MacroLines.Add(lineItem);

                    if (!AIName.ValidateMacroName(lineDetailItemMacro.MacroName, asmLoad))
                    {
                        lineDetailItemMacro.Errors.Add(new ErrorLineItem(lineItem, Error.ErrorCodeEnum.E3007));
                    }

                    var errorArgumentNames = args.Where(m => !AIName.ValidateMacroArgument(m, asmLoad));
                    if (errorArgumentNames.Any())
                    {
                        lineDetailItemMacro.Errors.Add(new ErrorLineItem(lineItem, Error.ErrorCodeEnum.E3005, string.Join(",", errorArgumentNames)));
                    }
                    asmLoad.Share.LineDetailItemForExpandItem = lineDetailItemMacro;

                    return lineDetailItemMacro;
                }

                if (endMatched.Success)
                {
                    throw new ErrorAssembleException(Error.ErrorCodeEnum.E3002);
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
