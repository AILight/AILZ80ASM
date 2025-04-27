using AILZ80ASM.AILight;
using AILZ80ASM.Assembler;
using AILZ80ASM.LineDetailItems.ScopeItem;
using System;
using System.Text.RegularExpressions;

namespace AILZ80ASM.LineDetailItems
{
    public class LineDetailItemFunctionDefine : LineDetailItem
    {
        private static readonly string RegexPatternFunction = @"^\s*Function\s+(?<function_name>[a-zA-Z0-9_]+)\s*\((?<argument>.+)\)\s*=>\s(?<formula>.+)";
        private static readonly Regex CompiledRegexPatternFunction = new Regex(
            RegexPatternFunction,
            RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase
        );

        private LineDetailItemFunctionDefine(LineItem lineItem, AsmLoad asmLoad)
            : base(lineItem, asmLoad)
        {

        }

        public static LineDetailItemFunctionDefine Create(LineItem lineItem, AsmLoad asmLoad)
        {
            if (!lineItem.IsCollectOperationString)
            {
                return default(LineDetailItemFunctionDefine);
            }

            var matched = CompiledRegexPatternFunction.Match(lineItem.OperationString);

            if (matched.Success)
            {
                // 名前の切り出し
                var functionName = matched.Groups["function_name"].Value;
                var argument = matched.Groups["argument"].Value;
                var formula = matched.Groups["formula"].Value;

                var arguments = AIName.ParseArguments(argument);

                if (!AIName.ValidateFunctionName(functionName, asmLoad))
                {
                    asmLoad.AddError(new ErrorLineItem(lineItem, Error.ErrorCodeEnum.E4002));
                }

                foreach (var item in arguments)
                {
                    if (!AIName.ValidateFunctionArgument(item, asmLoad))
                    {
                        asmLoad.AddError(new ErrorLineItem(lineItem, Error.ErrorCodeEnum.E4005, item));
                    }
                }

                var function = new Function(functionName, arguments, formula, asmLoad);

                asmLoad.AddFunction(function);

                return new LineDetailItemFunctionDefine(lineItem, asmLoad);
            }

            return default;
        }
    }
}
