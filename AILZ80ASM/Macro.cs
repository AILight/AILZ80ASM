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

        public string[] Args { get; private set; }
        public LineItem[] LineItems { get; private set; }

        private static readonly string RegexPatternLabel = @"^(?<label>([^\.][a-zA-Z0-9_]+::?)|(\.[a-zA-Z0-9_]+[^:]))\s*.*$";
        private static readonly string RegexPatternMacro = @"^(?<macro>[a-zA-Z0-9_\.]+)\s*(?<args>.*)$";

        public Macro(string macroName, string[] args, LineItem[] lineItems, AsmLoad asmLoad)
        {
            this.GlobalLabelName = asmLoad.GlobalLableName;
            this.Name = macroName;

            Args = args;

            LineItems = lineItems;
        }

        public static LineDetailScopeItem[] Expansion(LineItem lineItem, AsmLoad asmLoad)
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
                    return default;
                }
                // 展開をする
                if (macro != default)
                {
                    var lineDetailScopeItems = new List<LineDetailScopeItem>();
                    if (!string.IsNullOrEmpty(labelName))
                    {
                        var localLineItem = new LineItem(lineItem);
                        localLineItem.SetLabelForMacro(labelName);

                        localLineItem.CreateLineDetailItem(asmLoad);
                        lineDetailScopeItems.Add(new LineDetailScopeItem(localLineItem, asmLoad));
                    }
                    // Macro展開用のAsmLoadを作成する
                    var macroAsmLoad = asmLoad.Clone(AsmLoad.ScopeModeEnum.Local);
                    var guid = $"{Guid.NewGuid():N}";
                    macroAsmLoad.GlobalLableName = $"global_{macro.Name}_{guid}";
                    macroAsmLoad.LabelName = $"label_{macro.Name}_{guid}";

                    if (!string.IsNullOrEmpty(macroArgs.Trim()))
                    {
                        var argValues = macroArgs.Split(',').Select(m => m.Trim()).ToArray();
                        if (argValues.Length != macro.Args.Length)
                        {
                            throw new ErrorMessageException(Error.ErrorCodeEnum.E1004);
                        }

                        // 引数の割り当て
                        foreach (var index in Enumerable.Range(0, argValues.Length))
                        {
                            macroAsmLoad.AddLabel(new Label(macro.Args[index], argValues[index], macroAsmLoad));
                        }
                    }
                    // LineItemsを作成
                    var lineItems = macro.LineItems.Select(m =>
                        {
                            var lineItem = new LineItem(m);
                            lineItem.CreateLineDetailItem(macroAsmLoad);
                            return lineItem;
                        }).ToArray(); //　シーケンシャルに処理する必要があるため、ToArrayは必須

                    foreach (var localLineItem in lineItems)
                    {
                        localLineItem.ExpansionItem();
                        lineDetailScopeItems.AddRange(localLineItem.LineDetailItem.LineDetailScopeItems);
                    }
                    return lineDetailScopeItems.ToArray();
                }

            }

            return default;

        }
    }
}
