using AILZ80ASM.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AILZ80ASM.Assembler
{
    public class Macro
    {
        public string GlobalLabelName { get; private set; }
        public string Name { get; private set; }
        public string FullName => $"{GlobalLabelName}.{Name}";

        public string[] Args { get; private set; }
        public LineItem[] LineItems { get; private set; }

        private static readonly string RegexPatternMacro = @"^(?<macro>[a-zA-Z0-9_\.\(\)]+)\s*(?<args>.*)$";

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
                var macro = asmLoad.FindMacro(macroName);

                if (macro == default)
                {
                    return default;
                }
                var arguments = string.IsNullOrEmpty(macroArgs) ? Array.Empty<string>() : macroArgs.Split(',').Select(m => m.Trim()).ToArray();

                return (macro, arguments);
            }
            return default;
        }

        public LineDetailScopeItem[] Expansion(LineItem lineItem, string[] arguments, AsmLoad asmLoad, ref AsmAddress asmAddress)
        {
            if (asmLoad.LoadMacros.Any(m => this == m))
            {
                throw new ErrorAssembleException(Error.ErrorCodeEnum.E3008);
            }

            var lineDetailScopeItems = new List<LineDetailScopeItem>();
            // ラベルを処理する
            if (!string.IsNullOrEmpty(lineItem.LabelString))
            {
                var localLineItem = new LineItem(lineItem.LabelString, 0, default(System.IO.FileInfo));
                var localLineDetailItemOperation = LineDetailItemOperation.Create(localLineItem, asmLoad);
                localLineDetailItemOperation.ExpansionItem();
                localLineDetailItemOperation.PreAssemble(ref asmAddress);
            }
            // Macro展開用のAsmLoadを作成する
            var guid = $"{Guid.NewGuid():N}";
            var lineItemList = new List<LineItem>();

            asmLoad.CreateNewScope($"macro_{guid}", $"label_{guid}", localAsmLoad =>
            {
                if (arguments.Length > 0)
                {
                    if (arguments.Length != this.Args.Length)
                    {
                        throw new ErrorAssembleException(Error.ErrorCodeEnum.E3004);
                    }

                    // 引数の割り当て
                    foreach (var index in Enumerable.Range(0, arguments.Length))
                    {
                        var argumentLabel = new LabelArg(arguments[index], asmLoad);
                        var argumentValue = argumentLabel.DataType != Label.DataTypeEnum.Invalidate ?
                                            argumentLabel.LabelFullName : arguments[index];

                        var label = new LabelArg(this.Args[index], argumentValue, localAsmLoad);
                        if (label.Invalidate)
                        {
                            throw new ErrorAssembleException(Error.ErrorCodeEnum.E3005);
                        }
                        localAsmLoad.AddLabel(label);
                    }
                }

                // LineItemsを作成
                var lineItems = this.LineItems.Skip(1).SkipLast(1).Select(m =>
                    {
                        var lineItem = new LineItem(m);
                        lineItem.CreateLineDetailItem(localAsmLoad);
                        return lineItem;
                    }).ToArray(); //　シーケンシャルに処理する必要があるため、ToArrayは必須

                foreach (var localLineItem in lineItems)
                {
                    try
                    {
                        localLineItem.ExpansionItem();
                        //localLineItem.PreAssemble(ref asmAddress);
                        //lineDetailScopeItems.AddRange(localLineItem.LineDetailItem.LineDetailScopeItems);
                    }
                    catch (ErrorAssembleException ex)
                    {
                        asmLoad.AddError(new ErrorLineItem(localLineItem, ex));
                    }

                }
                lineItemList.AddRange(lineItems);

            });


            // 展開領域
            asmLoad.LoadMacros.Push(this);

            foreach (var item in lineItemList)
            {
                try
                {
                    item.PreAssemble(ref asmAddress);
                }
                catch (ErrorAssembleException ex)
                {
                    asmLoad.AddError(new ErrorLineItem(item, ex));
                }
            }

            asmLoad.LoadMacros.Pop();

            return lineItemList.SelectMany(m => m.LineDetailItem.LineDetailScopeItems ?? Array.Empty<LineDetailScopeItem>()).ToArray();
        }

        /// <summary>
        /// ロングマクロ名を生成する
        /// </summary>
        /// <param name="labelName"></param>
        /// <param name="asmLoad"></param>
        /// <returns></returns>
        public static string GetMacroFullName(string macroName, AsmLoad asmLoad)
        {
            if (macroName.IndexOf(".") > 0)
            {
                return macroName;
            }

            return $"{asmLoad.GlobalLabelName}.{macroName}";
        }
    }
}
