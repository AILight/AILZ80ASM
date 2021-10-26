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
        public string FullName => $"{GlobalLabelName}:{Name}";

        public string[] Args { get; private set; }
        public LineItem[] LineItems { get; private set; }

        private static readonly string RegexPatternMacro = @"^(?<macro>[a-zA-Z0-9_:]+)\s*(?<args>.*)$";

        public Macro(string macroName, string[] args, LineItem[] lineItems, AsmLoad asmLoad)
        {
            this.GlobalLabelName = asmLoad.GlobalLabelName;
            this.Name = macroName;

            Args = args;

            LineItems = lineItems;
        }

        public static (Macro Macro, string[] Arguments) Find(LineItem lineItem, AsmLoad asmLoad)
        {
            var operationMatched = Regex.Match(lineItem.OperationString, RegexPatternMacro, RegexOptions.Singleline | RegexOptions.IgnoreCase);
            if (operationMatched.Success)
            {
                var macroName = operationMatched.Groups["macro"].Value;
                var macroArgs = operationMatched.Groups["args"].Value;
                var macro = default(Macro);

                try
                {
                    macroName = Macro.GetLognMacroName(macroName, asmLoad);
                    macro = asmLoad.Macros.Where(m => string.Compare(m.FullName, macroName, true) == 0).SingleOrDefault();
                }
                catch
                {
                    return default;
                }

                if (macro == default)
                {
                    return default;
                }
                var arguments = string.IsNullOrEmpty(macroArgs) ? Array.Empty<string>() : macroArgs.Split(',').Select(m => m.Trim()).ToArray();

                return (macro, arguments);
            }
            return default;
        }

        public static (Macro Macro, string[] Arguments) FindWithoutLongName(LineItem lineItem, AsmLoad asmLoad)
        {
            var operationMatched = Regex.Match(lineItem.OperationString, RegexPatternMacro, RegexOptions.Singleline | RegexOptions.IgnoreCase);
            if (operationMatched.Success)
            {
                var macroName = operationMatched.Groups["macro"].Value;
                var macroArgs = operationMatched.Groups["args"].Value;
                var macro = default(Macro);

                try
                {
                    macro = asmLoad.Macros.Where(m => string.Compare(m.Name, macroName, true) == 0).SingleOrDefault();
                }
                catch
                {
                    return default;
                }

                if (macro == default)
                {
                    return default;
                }
                var arguments = string.IsNullOrEmpty(macroArgs) ? Array.Empty<string>() : macroArgs.Split(',').Select(m => m.Trim()).ToArray();

                return (macro, arguments);
            }
            return default;
        }

        public LineDetailScopeItem[] Expansion(LineItem lineItem, string[] arguments, AsmLoad asmLoad)
        {
            if (asmLoad.LoadMacros.Any(m => this == m))
            {
                throw new ErrorAssembleException(Error.ErrorCodeEnum.E3008);
            }

            var lineDetailScopeItems = new List<LineDetailScopeItem>();
            if (!string.IsNullOrEmpty(lineItem.LabelString))
            {
                var localLineItem = new LineItem(lineItem);
                localLineItem.SetLabel(lineItem.LabelString);

                localLineItem.CreateLineDetailItem(asmLoad);
                lineDetailScopeItems.Add(new LineDetailScopeItem(localLineItem, asmLoad));
            }
            // Macro展開用のAsmLoadを作成する
            var macroAsmLoad = asmLoad.Clone(AsmLoad.ScopeModeEnum.Local);
            var guid = $"{Guid.NewGuid():N}";
            macroAsmLoad.GlobalLabelName = $"macro_{this.Name}_{guid}";

            if (arguments.Length > 0)
            {
                if (arguments.Length != this.Args.Length)
                {
                    throw new ErrorAssembleException(Error.ErrorCodeEnum.E3004);
                }

                // 引数の割り当て
                foreach (var index in Enumerable.Range(0, arguments.Length))
                {
                    var label = new Label(this.Args[index], arguments[index], macroAsmLoad, asmLoad);
                    if (label.Invalidate)
                    {
                        throw new ErrorAssembleException(Error.ErrorCodeEnum.E3005);
                    }
                    // 引数で解決できるものは先行で処理する
                    label.SetArgument();
                    macroAsmLoad.AddLabel(label);
                }
            }
            macroAsmLoad.LabelName = $"label_{this.Name}_{guid}";

            // LineItemsを作成
            var lineItems = this.LineItems.Skip(1).SkipLast(1).Select(m =>
                {
                    var lineItem = new LineItem(m);
                    lineItem.CreateLineDetailItem(macroAsmLoad);
                    return lineItem;
                }).ToArray(); //　シーケンシャルに処理する必要があるため、ToArrayは必須

            // 展開領域
            asmLoad.LoadMacros.Push(this);

            foreach (var localLineItem in lineItems)
            {
                try
                {
                    localLineItem.ExpansionItem();
                    lineDetailScopeItems.AddRange(localLineItem.LineDetailItem.LineDetailScopeItems);
                }
                catch (ErrorAssembleException ex)
                {
                    asmLoad.Errors.Add(new ErrorLineItem(localLineItem, ex));
                }

            }

            asmLoad.LoadMacros.Pop();

            return lineDetailScopeItems.ToArray();
        }

        /// <summary>
        /// ロングマクロ名を生成する
        /// </summary>
        /// <param name="labelName"></param>
        /// <param name="asmLoad"></param>
        /// <returns></returns>
        public static string GetLognMacroName(string macroName, AsmLoad asmLoad)
        {
            if (macroName.IndexOf(":") > 0)
            {
                return macroName;
            }

            return $"{asmLoad.GlobalLabelName}:{macroName}";
        }
    }
}
