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
        public List<LineItem> LineItems { get; private set; } = new List<LineItem>();

        private static readonly string RegexPatternLabel = @"^(?<label>([^\.][a-zA-Z0-9_]+::?)|(\.[a-zA-Z0-9_]+[^:]))\s*.*$";
        private static readonly string RegexPatternMacro = @"^(?<macro>[a-zA-Z0-9_\.]+)\s*(?<args>.*)$";

        public Macro(string macroName, string args, LineItem[] lineItems, AsmLoad asmLoad)
        {
            this.GlobalLabelName = asmLoad.GlobalLableName;
            this.Name = macroName;
            Args.AddRange(args.Split(',').Select(m => m.Trim()));

            LineItems.AddRange(lineItems);
        }

        public static LineDetailExpansionItem[] Expansion(string operationString, AsmLoad asmLoad)
        {
            var labelName = "";
            var labelMatched = Regex.Match(operationString, RegexPatternLabel, RegexOptions.Singleline | RegexOptions.IgnoreCase);
            if (labelMatched.Success)
            {
                labelName = labelMatched.Groups["label"].Value;
            }
            var operation = operationString.Substring(labelName.Length).Trim();
            var operationMatched = Regex.Match(operation, RegexPatternMacro, RegexOptions.Singleline | RegexOptions.IgnoreCase);
            if (operationMatched.Success)
            {
                var macroName = operationMatched.Groups["macro"].Value;
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
                var lineDetailExpansionItems = new List<LineDetailExpansionItem>();
                lineDetailExpansionItems.Add(new LineDetailExpansionItem)

            }


            return default(LineDetailExpansionItem[]);

        }

        /*
        public Macro(List<LineItem> lineItems, FileItem fileItem)
        {
            FileItem = fileItem;
            GlobalMacroName = fileItem.WorkGlobalLabelName;
            foreach (var item in lineItems)
            {
                //item.IsAssembled = true;
            }

            var firstLineItem = lineItems.First();
            var macroIndex = firstLineItem.OperationString.IndexOf("macro", StringComparison.OrdinalIgnoreCase);
            var operationString = firstLineItem.OperationString.Substring(macroIndex + 5).TrimStart();
            var argsIndex = operationString.IndexOf(" ");
            if (argsIndex == -1)
            {
                Name = operationString;
            }
            else
            {
                Name = operationString.Substring(0, argsIndex);
                Args.AddRange(operationString.Substring(argsIndex).Split(",").Select(m => m.Trim()));
            }

            LineItems.AddRange(lineItems.Skip(1).SkipLast(1));
        }
        */

    }
}
