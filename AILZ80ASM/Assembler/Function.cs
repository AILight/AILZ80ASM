using AILZ80ASM.AILight;
using System;
using System.Collections;
using System.Linq;
using System.Text.RegularExpressions;

namespace AILZ80ASM.Assembler
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
                var function = asmLoad.FindFunction(functionName);

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
        /// Functionを計算する
        /// </summary>
        /// <param name="arguments"></param>
        /// <param name="asmLoad"></param>
        /// <param name="asmAddress"></param>
        /// <returns></returns>
        public int Calculation(string[] arguments, AsmLoad asmLoad, AsmAddress? asmAddress)
        {
            if (Args.Length != arguments.Length)
            {
                throw new Exception($"引数の数が不一致です。Function:{this.Name}");
            }

            var guid = $"{Guid.NewGuid():N}";
            var localAsmLoad = asmLoad.CloneWithNewScore($"function_{guid}", $"label_{guid}");

            foreach (var index in Enumerable.Range(0, arguments.Length))
            {
                var label = new Label(Args[index], arguments[index], localAsmLoad);
                localAsmLoad.AddLabel(label);
            }

            return AIMath.ConvertTo<int>(Formula, localAsmLoad);
        }

        /// <summary>
        /// ロングファンクション名を生成する
        /// </summary>
        /// <param name="labelName"></param>
        /// <param name="asmLoad"></param>
        /// <returns></returns>
        public static string GetFunctionFullName(string functionName, AsmLoad asmLoad)
        {
            if (functionName.Contains("."))
            {
                return functionName;
            }

            return $"{asmLoad.GlobalLabelName}.{functionName}";
        }
    }
}
