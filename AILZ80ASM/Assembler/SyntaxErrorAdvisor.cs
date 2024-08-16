using AILZ80ASM.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AILZ80ASM.Assembler
{
    public static class SyntaxErrorAdvisor
    {
        private static readonly string RegexPatternSeparatorPattern = @"\b(?<FullMatch>[^\s]+,[^\s]+)\b";
        private static readonly Regex CompiledRegexPatternSeparatorPattern = new Regex(
            RegexPatternSeparatorPattern, RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase
        );

        public static void AnalyzeLine(LineItem lineItem, AsmLoad asmLoad)
        {
            InternalAnalyzeLineForLabel(lineItem, asmLoad);
        }

        private static void InternalAnalyzeLineForLabel(LineItem lineItem, AsmLoad asmLoad)
        {
            var matched = CompiledRegexPatternSeparatorPattern.Match(lineItem.OperationString);
            if (matched.Success)
            {
                var matchedString = matched.Groups["FullMatch"].Value;
                var replacedString = matchedString.Replace(" ", "").Replace(",", ".");

                if (asmLoad.FindLabel(replacedString) != default)
                {
                    throw new ErrorAssembleException(Error.ErrorCodeEnum.E0001, "ラベルに記号(,)が含まれています。");
                }
            }
        }
    }
}
