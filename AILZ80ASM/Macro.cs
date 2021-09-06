using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace AILZ80ASM
{
    public class Macro
    {
        public string GlobalLabelName { get; private set; }
        public string Name { get; private set; }
        public string FullName => $"{GlobalLabelName}.{Name}";

        public List<string> Args { get; private set; } = new List<string>();
        public List<string> LineStrings { get; private set; } = new List<string>();

        private static readonly string RegexPatternLabel = @"^(?<label>([^\.][a-zA-Z0-9_]+::?)|(\.[a-zA-Z0-9_]+[^:]))\s*.*$";
        private static readonly string RegexPatternMacro = @"^(?<macro>[a-zA-Z0-9_\.]+)\s*(?<args>.*)$";

        public Macro(string macroName, string args, string[] lineStrings, AsmLoad asmLoad)
        {
            this.GlobalLabelName = asmLoad.GlobalLableName;
            this.Name = macroName;
            if (!string.IsNullOrEmpty(args.Trim()))
            {
                Args.AddRange(args.Split(',').Select(m => m.Trim()));
            }

            LineStrings.AddRange(lineStrings);
        }

        public static LineDetailExpansionItem[] Expansion(LineItem lineItem, AsmLoad asmLoad)
        {
            var labelName = "";
            var labelMatched = Regex.Match(lineItem.OperationString, RegexPatternLabel, RegexOptions.Singleline | RegexOptions.IgnoreCase);
            if (labelMatched.Success)
            {
                labelName = labelMatched.Groups["label"].Value;
            }
            var operation = lineItem.OperationString.Substring(labelName.Length).Trim();
            var operationMatched = Regex.Match(operation, RegexPatternMacro, RegexOptions.Singleline | RegexOptions.IgnoreCase);
            if (operationMatched.Success)
            {
                var macroName = operationMatched.Groups["macro"].Value;
                var macroArgs = operationMatched.Groups["args"].Value;
                var macro = default(Macro);

                try
                {
                    if (macroName.Contains('.'))
                    {
                        macro = asmLoad.Macros.Where(m => string.Compare(m.FullName, macroName, true) == 0).SingleOrDefault();
                    }
                    else
                    {
                        macro = asmLoad.Macros.Where(m => string.Compare(m.Name, macroName, true) == 0).SingleOrDefault();
                    }
                }
                catch
                {
                    return default(LineDetailExpansionItem[]);
                }
                // 展開をする
                if (macro != default)
                {
                    var lineDetailExpansionItems = new List<LineDetailExpansionItem>();
                    var lineIndex = 1;
                    if (!string.IsNullOrEmpty(labelName))
                    {
                        lineDetailExpansionItems.Add(new LineDetailExpansionItemOperation(new LineItem(labelName, lineIndex, asmLoad), asmLoad));
                    }
                    // Macro展開用のAsmLoadを作成する
                    var macroAsmLoad = asmLoad.CloneForLocal();
                    macroAsmLoad.LabelName = $"{macro.Name}_{Guid.NewGuid():N}";

                    if (!string.IsNullOrEmpty(macroArgs.Trim()))
                    {
                        var argValues = macroArgs.Split(',').Select(m => m.Trim()).ToArray();
                        if (argValues.Count() != macro.Args.Count())
                        {
                            throw new ErrorMessageException(Error.ErrorCodeEnum.E1004);
                        }

                        // 引数の割り当て
                        foreach (var index in Enumerable.Range(0, argValues.Count()))
                        {
                            macroAsmLoad.LocalLabels.Add(new Label(macro.Args[index], argValues[index], macroAsmLoad));
                        }
                    }
                    // LineItemsを作成
                    var lineStrings = macro.LineStrings.ToArray();
                    var lineItems = lineStrings.Select(m => new LineItem(m, lineIndex++, macroAsmLoad));
                    foreach (var localLineItem in lineItems)
                    {
                        localLineItem.ExpansionItem();
                        lineDetailExpansionItems.AddRange(localLineItem.LineDetailItem.LineDetailExpansionItems);
                    }
                    return lineDetailExpansionItems.ToArray();
                }

            }

            return default(LineDetailExpansionItem[]);

        }
    }
}
