using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AILZ80ASM
{
    public class LineDetailItemFunctionDefine : LineDetailItem
    {
        private LineDetailItemFunctionDefine(LineItem lineItem, AsmLoad asmLoad)
            : base(lineItem, asmLoad)
        {

        }

        public static LineDetailItemFunctionDefine Create(LineItem lineItem, AsmLoad asmLoad)
        {
            if (lineItem.OperationString.StartsWith("Function ", StringComparison.OrdinalIgnoreCase))
            {
                var target = lineItem.OperationString.Substring(9).TrimStart();
                var indexBracket = target.IndexOf("(");
                var indexArrow = target.IndexOf("=>");

                if (indexBracket == -1 || indexBracket > indexArrow)
                {
                    throw new Exception("ファンクション名の次に ( が必要です。");
                }

                if (indexArrow == -1)
                {
                    throw new Exception("ファンクションには、=> が必要です。");
                }

                // 名前の切り出し
                var functionName = target.Substring(0, indexBracket);

                // 引数の切り出し
                var argument = target.Substring(indexBracket, indexArrow - indexBracket).Trim();
                argument = argument.Substring(1, argument.Length - 2);
                var arguments = AIName.ParseArguments(argument);
                // 式の切り出し
                var formula = target.Substring(indexArrow + 2).Trim();

                var function = new Function(functionName, arguments, formula, asmLoad);
                asmLoad.AddFunction(function);

                return new LineDetailItemFunctionDefine(lineItem, asmLoad);
            }

            return default;
        }

        public override void ExpansionItem()
        {
            LineDetailScopeItems = Array.Empty<LineDetailScopeItem>();
        }

    }
}
