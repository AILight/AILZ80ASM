using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AILZ80ASM
{
    public class Function
    {
        public string GlobalLabelName { get; private set; }
        public string Name { get; private set; }
        public string FullName => $"{GlobalLabelName}.{Name}";

        public string[] Args { get; private set; }
        public string Formula { get; private set; }

        private static readonly string RegexPatternFunction = @"^(?<function>[a-zA-Z0-9_\.]+)\s*(?<args>.*)$";

        public Function(string functionName, string[] args, string formula, AsmLoad asmLoad)
        {
            this.GlobalLabelName = asmLoad.GlobalLabelName;
            this.Name = functionName;

            Args = args;

            Formula = formula;
        }

        public static (Function function, string[] Arguments) Find(string operation, AsmLoad asmLoad)
        {
            var operationMatched = Regex.Match(operation, RegexPatternFunction, RegexOptions.Singleline | RegexOptions.IgnoreCase);
            if (operationMatched.Success)
            {
                var functionName = operationMatched.Groups["function"].Value;
                var functionArgs = operationMatched.Groups["args"].Value;
                var function = default(Function);

                try
                {
                    functionName = Function.GetLongFunctionName(functionName, asmLoad);
                    function = asmLoad.Functions.Where(m => string.Compare(m.FullName, functionName, true) == 0).SingleOrDefault();
                }
                catch
                {
                    return default;
                }

                if (function == default)
                {
                    return default;
                }
                var arguments = string.IsNullOrEmpty(functionArgs) ? Array.Empty<string>() : functionArgs.Split(',').Select(m => m.Trim()).ToArray();

                return (function, arguments);
            }
            return default;
        }

        public static (Function function, string[] Arguments) FindWithoutLongName(LineItem lineItem, AsmLoad asmLoad)
        {
            var operationMatched = Regex.Match(lineItem.OperationString, RegexPatternFunction, RegexOptions.Singleline | RegexOptions.IgnoreCase);
            if (operationMatched.Success)
            {
                var functionName = operationMatched.Groups["function"].Value;
                var functionArgs = operationMatched.Groups["args"].Value;
                var function = default(Function);

                try
                {
                    function = asmLoad.Functions.Where(m => string.Compare(m.FullName, functionName, true) == 0).SingleOrDefault();
                }
                catch
                {
                    return default;
                }

                if (function == default)
                {
                    return default;
                }
                var arguments = string.IsNullOrEmpty(functionArgs) ? Array.Empty<string>() : functionArgs.Split(',').Select(m => m.Trim()).ToArray();

                return (function, arguments);
            }
            return default;
        }


        /// <summary>
        /// ロングファンクション名を生成する
        /// </summary>
        /// <param name="labelName"></param>
        /// <param name="asmLoad"></param>
        /// <returns></returns>
        public static string GetLongFunctionName(string functionName, AsmLoad asmLoad)
        {
            if (functionName.Contains("."))
            {
                return functionName;
            }

            return $"{asmLoad.GlobalLabelName}.{functionName}";
        }
    }
}
