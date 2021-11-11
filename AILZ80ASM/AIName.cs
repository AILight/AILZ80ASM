using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace AILZ80ASM
{
    public static class AIName
    {
        private static readonly string RegexPatternLabelValidate = @"^(\D+)[a-zA-Z0-9_]+";

        public static bool DeclareLabelValidate(string target, AsmLoad asmLoad)
        {
            if (target.StartsWith(".") && target.EndsWith(":"))
            {
                return false;
            }

            // ラベルの名称だけを取得
            if (target.EndsWith("::"))
            {
                target = target.Substring(0, target.Length - 2);
            }

            if (target.EndsWith(":"))
            {
                target = target.Substring(0, target.Length - 1);
            }

            if (target.StartsWith("."))
            {
                target = target.Substring(1);
            }

            if (string.IsNullOrEmpty(target))
            {
                return false;
            }

            return ValidateName(target, asmLoad);
        }

        /// <summary>
        /// マクロ名をチェックする
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static bool ValidateMacroName(string target, AsmLoad asmLoad)
        {
            if (string.IsNullOrEmpty(target))
                return false;

            return ValidateName(target, asmLoad);
        }

        /// <summary>
        /// 引数のラベルチェック
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static bool ValidateMacroArgument(string target, AsmLoad asmLoad)
        {
            if (string.IsNullOrEmpty(target))
                return false;

            return ValidateName(target, asmLoad);
        }

        private static bool ValidateName(string target, AsmLoad asmLoad)
        {
            // 含まれてはいけない文字の調査
            if (target.ToArray().Any(m => ":. ".Contains(m)))
            {
                return false;
            }

            // レジスター文字列、命令の文字列は利用不可
            switch (asmLoad.AssembleISA)
            {
                case AsmISA.Z80:
                    var z80 = new Instructions.Z80();
                    if (z80.IsMatchRegisterName(target))
                    {
                        return false;
                    }
                    if (z80.IsMatchInstructionName(target))
                    {
                        return false;
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }

            if (AIMath.IsNumber(target))
            {
                return false;
            }

            return Regex.Match(target, RegexPatternLabelValidate, RegexOptions.Singleline | RegexOptions.IgnoreCase).Success;
        }

    }
}
