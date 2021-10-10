using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AILZ80ASM
{
    public static class AIName
    {
        private static readonly string RegexPatternLabelValidate = @"^(\D+)[a-zA-Z0-9_]+";

        public static bool DeclareLabelValidate(string target)
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

            return ValidateName(target);
        }

        /// <summary>
        /// マクロ名をチェックする
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static bool ValidateMacroName(string target)
        {
            if (string.IsNullOrEmpty(target))
                return false;

            if (OPCodeTable.IsOPCode(target))
                return false;

            return ValidateName(target);
        }

        /// <summary>
        /// 引数のラベルチェック
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static bool ValidateMacroArgument(string target)
        {
            if (string.IsNullOrEmpty(target))
                return false;

            return ValidateName(target);
        }

        private static bool ValidateName(string target)
        {
            // 含まれてはいけない文字の調査
            if (target.ToArray().Any(m => ":. ".Contains(m)))
            {
                return false;
            }

            // レジスター文字列は利用不可
            if (OPCodeTable.IsRegister(target.ToUpper()))
            {
                return false;
            }

            if (AIMath.IsNumber(target))
            {
                return false;
            }

            return Regex.Match(target, RegexPatternLabelValidate, RegexOptions.Singleline).Success;
        }

    }
}
