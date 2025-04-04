using AILZ80ASM.Assembler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace AILZ80ASM.AILight
{
    public static class AIName
    {
        private static readonly string RegexPatternLabelValidate = @"^[a-zA-Z0-9_]+$";
        private static readonly Regex CompiledRegexPatternLabelValidate = new Regex(
            RegexPatternLabelValidate, RegexOptions.Singleline | RegexOptions.IgnoreCase
        );
        private static readonly string RegexPatternMacroValidate = @"^[a-zA-Z0-9_()]+$";
        private static readonly Regex CompiledRegexPatternMacroValidate = new Regex(
            RegexPatternMacroValidate, RegexOptions.Singleline | RegexOptions.IgnoreCase
        );
        private static readonly string RegexPatternLocalLabelNOValidate = @"^[a-zA-Z0-9_]+$";
        private static readonly Regex CompiledRegexPatternLocalLabelNOValidate = new Regex(
            RegexPatternLocalLabelNOValidate, RegexOptions.Singleline | RegexOptions.IgnoreCase
        );
        private static readonly string RegexPatternLocalLabelATValidate = @"^@[0-9]+$";
        private static readonly Regex CompiledRegexPatternLocalLabelATValidate = new Regex(
            RegexPatternLocalLabelATValidate, RegexOptions.Singleline | RegexOptions.IgnoreCase
        );
        private static readonly string RegexPatternLocalLabelANValidate = @"^@@(?<labelValue>([0-9]*))$";
        private static readonly Regex CompiledRegexPatternLocalLabelANValidate = new Regex(
            RegexPatternLocalLabelANValidate, RegexOptions.Singleline | RegexOptions.IgnoreCase
        );
        private static readonly string RegexPatternLabelInvalid = @"^[0-9]";
        private static readonly Regex CompiledRegexPatternLabelInvalid = new Regex(
            RegexPatternLabelInvalid, RegexOptions.Singleline | RegexOptions.IgnoreCase
        );
        private static readonly string RegexPatternCharMapInvalid = @"^@[a-zA-Z0-9_]+$";
        private static readonly Regex CompiledRegexPatternCharMapInvalid = new Regex(
            RegexPatternCharMapInvalid, RegexOptions.Singleline | RegexOptions.IgnoreCase
        );

        public static bool DeclareLabelValidate(string target, AsmLoad asmLoad)
        {
            var isGlobalLabel = false;

            if (target.StartsWith('.') && target.EndsWith(':'))
            {
                target = target.Substring(1, target.Length - 2);
                while (target.EndsWith(':'))
                {
                    target = target.Substring(0, target.Length - 1);
                }

                return ValidateNameForLocalLabel(target, asmLoad);
            }

            // ラベルの名称だけを取得
            if (target.StartsWith('[') && target.EndsWith(']'))
            {
                target = target.Substring(1, target.Length - 2);
                isGlobalLabel = true;
            }

            if (target.EndsWith(':'))
            {
                while (target.EndsWith(':'))
                {
                    target = target.Substring(0, target.Length - 1);
                }
            }

            if (target.StartsWith('.'))
            {
                target = target.Substring(1);

                return ValidateNameForLocalLabel(target, asmLoad);
            }
            else
            {
                var splits = target.Split('.');
                if (isGlobalLabel && splits.Length != 1)
                {
                    // GlobalLabelは、.は含める事が出来ない。
                    return false;
                }

                switch (splits.Length)
                {
                    case 1:
                        if (!ValidateName(splits[0], asmLoad))
                        {
                            return false;
                        }
                        break;
                    case 2:
                        if (string.IsNullOrEmpty(asmLoad.FindGlobalLabelName(splits[0])))
                        {
                            if (!ValidateName(splits[0], asmLoad))
                            {
                                return false;
                            }
                            if (!ValidateNameForLocalLabel(splits[1], asmLoad))
                            {
                                return false;
                            }
                        }
                        else
                        {
                            if (!ValidateName(splits[0], asmLoad))
                            {
                                return false;
                            }
                            if (!ValidateName(splits[1], asmLoad))
                            {
                                return false;
                            }
                        }
                        break;
                    case 3:
                        if (!ValidateName(splits[0], asmLoad))
                        {
                            return false;
                        }
                        if (!ValidateName(splits[1], asmLoad))
                        {
                            return false;
                        }
                        if (!ValidateNameForLocalLabel(splits[2], asmLoad))
                        {
                            return false;
                        }
                        break;
                    default:
                        return false;
                }
                return true;
            }
        }

        /// <summary>
        /// マクロ名をチェックする
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static bool ValidateMacroName(string target, AsmLoad asmLoad)
        {
            if (string.IsNullOrEmpty(target))
            {
                return false;
            }

            // 先頭に()は使えない
            if (target.StartsWith('(') || target.StartsWith(')'))
            {
                return false;
            }

            return ValidateNameForMacroName(target, asmLoad);
        }

        /// <summary>
        /// 引数のラベルチェック
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static bool ValidateMacroArgument(string target, AsmLoad asmLoad)
        {
            if (string.IsNullOrEmpty(target))
            {
                return false;
            }

            return ValidateName(target, asmLoad);
        }

        /// <summary>
        /// ファンクション名をチェックする
        /// </summary>
        /// <param name="target"></param>
        /// <param name="asmLoad"></param>
        /// <returns></returns>
        public static bool ValidateFunctionName(string target, AsmLoad asmLoad)
        {
            if (string.IsNullOrEmpty(target))
            {
                return false;
            }

            // ()は使えない
            if (target.IndexOfAny(new[] { '(', ')' }) != -1)
            {
                return false;
            }

            return ValidateNameForFunction(target, asmLoad);
        }

        /// <summary>
        /// ファンクション引数名をチェックする
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static bool ValidateFunctionArgument(string target, AsmLoad asmLoad)
        {
            if (string.IsNullOrEmpty(target))
            {
                return false;
            }


            return ValidateNameForFunctionArgument(target, asmLoad);
        }


        /// <summary>
        /// 引数の分解を行う（愚直に積む）
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static string[] ParseArguments(string target)
        {
            if (string.IsNullOrEmpty(target))
            {
                return Array.Empty<string>();
            }

            var argumentList = new List<string>();
            var startIndex = 0;
            var index = 0;

            while (index < target.Length)
            {
                var commaIndex = AIString.IndexOfSkipString(target, ',', index);
                var kakkoIndex = AIString.IndexOfSkipString(target, '(', index);
                if (commaIndex == -1)
                {
                    argumentList.Add(target.Substring(startIndex).Trim());
                    index = target.Length;
                    startIndex = index;
                }
                else if(kakkoIndex == -1 || commaIndex < kakkoIndex)
                {
                    argumentList.Add(target.Substring(startIndex, commaIndex - startIndex).Trim());
                    index = commaIndex + 1;
                    startIndex = index;
                    if (startIndex >= target.Length && commaIndex != -1)
                    {
                        argumentList.Add("");
                    }
                }
                else
                {
                    index = kakkoIndex + 1;
                    var kakkoCount = 1;
                    while (index < target.Length && kakkoCount > 0)
                    {
                        var nextKakkoIndex = AIString.IndexOfSkipString(target, '(', index);
                        var closeKakkoIndex = AIString.IndexOfSkipString(target, ')', index);
                        if (closeKakkoIndex == -1)
                        {
                            index = target.Length;
                        }
                        else if (nextKakkoIndex == -1 ||
                                 nextKakkoIndex > closeKakkoIndex)
                        {
                            index = closeKakkoIndex + 1;
                            kakkoCount--;
                        }
                        else
                        {
                            kakkoCount++;
                            index = nextKakkoIndex + 1;
                        }
                    }
                }
            }
            if (startIndex < target.Length)
            {
                argumentList.Add(target.Substring(startIndex).Trim());
            }
            return argumentList.ToArray();
        }

        /// <summary>
        /// CharMap名をチェックする
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static bool ValidateCharMapName(string target, AsmLoad asmLoad)
        {
            //先頭は@から始まるので、@を抜いて判断
            if (target.Length <= 1 || !ValidateName(target.Substring(1), asmLoad))
            {
                return false;
            }

            return CompiledRegexPatternCharMapInvalid.Match(target).Success;
        }

        /// <summary>
        /// Endステートメントでの有効文字列をチェック
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static bool ValidateNameEndArgument(string target)
        {
            return !AsmReservedWord.GetReservedWordsForEndArgument().Any(m => string.Compare(target, m.Name, true) == 0);
        }

        /// <summary>
        /// 名前のチェック（一般ラベル）
        /// </summary>
        /// <param name="target"></param>
        /// <param name="asmLoad"></param>
        /// <returns></returns>
        private static bool ValidateName(string target, AsmLoad asmLoad)
        {
            if (string.IsNullOrEmpty(target))
            {
                return false;
            }

            // 含まれてはいけない文字の調査
            if (target.ToArray().Any(m => ":. ".Contains(m)))
            {
                return false;
            }

            if (AIValue.IsNumber(target))
            {
                return false;
            }

            // レジスター文字列、命令の文字列は利用不可
            if (asmLoad.ISA.IsMatchRegisterName(target))
            {
                return false;
            }
            if (asmLoad.ISA.IsMatchInstructionName(target))
            {
                return false;
            }

            return CompiledRegexPatternLabelValidate.Match(target).Success &&
                   !CompiledRegexPatternLabelInvalid.Match(target).Success;
        }

        /// <summary>
        /// 名前のチェック（一般ラベル）
        /// </summary>
        /// <param name="target"></param>
        /// <param name="asmLoad"></param>
        /// <returns></returns>
        private static bool ValidateNameForMacroName(string target, AsmLoad asmLoad)
        {
            if (string.IsNullOrEmpty(target))
            {
                return false;
            }

            // 含まれてはいけない文字の調査
            if (target.ToArray().Any(m => ":. ".Contains(m)))
            {
                return false;
            }

            if (AIValue.IsNumber(target))
            {
                return false;
            }

            // レジスター文字列、命令の文字列は利用不可
            if (asmLoad.ISA.IsMatchRegisterName(target))
            {
                return false;
            }
            if (asmLoad.ISA.IsMatchInstructionName(target))
            {
                return false;
            }

            return CompiledRegexPatternMacroValidate.Match(target).Success &&
                   !CompiledRegexPatternLabelInvalid.Match(target).Success;
        }

        private static bool ValidateNameForLocalLabel(string target, AsmLoad asmLoad)
        {
            if (string.IsNullOrEmpty(target))
            {
                return false;
            }

            // 含まれてはいけない文字の調査
            if (target.ToArray().Any(m => ":. ".Contains(m)))
            {
                return false;
            }

            var regex = CompiledRegexPatternLocalLabelANValidate.Match(target);
            if (regex.Success)
            {
                var valueString= regex.Groups["labelValue"].Value;
                if (valueString.Length > 0)
                {
                    var sum = Convert.ToInt32(valueString[0]);
                    if (Guid.TryParse(valueString.Substring(1), out var anonGuid))
                    {
                        var checkSum = anonGuid.ToByteArray().Select(m => (int)m).Sum() % 10;
                        return (sum == checkSum);
                    }
                    return false;
                }
                return true;
            }

            // @と通常ラベルの処理
            return CompiledRegexPatternLocalLabelATValidate.Match(target).Success ||
                   CompiledRegexPatternLocalLabelNOValidate.Match(target).Success;
        }

        /// <summary>
        /// 名前のチェック（ファンクション）
        /// </summary>
        /// <param name="target"></param>
        /// <param name="asmLoad"></param>
        /// <returns></returns>
        private static bool ValidateNameForFunction(string target, AsmLoad asmLoad)
        {
            if (string.IsNullOrEmpty(target))
            {
                return false;
            }

            // 含まれてはいけない文字の調査
            if (target.ToArray().Any(m => ":. ".Contains(m)))
            {
                return false;
            }

            if (AIValue.IsNumber(target))
            {
                return false;
            }

            return CompiledRegexPatternLabelValidate.Match(target).Success &&
                   !CompiledRegexPatternLabelInvalid.Match(target).Success;
        }

        /// <summary>
        /// 名前のチェック（ファンクション引数）
        /// </summary>
        /// <param name="target"></param>
        /// <param name="asmLoad"></param>
        /// <returns></returns>
        private static bool ValidateNameForFunctionArgument(string target, AsmLoad asmLoad)
        {
            if (string.IsNullOrEmpty(target))
            {
                return false;
            }

            // 含まれてはいけない文字の調査
            if (target.ToArray().Any(m => ":. ".Contains(m)))
            {
                return false;
            }

            if (AIValue.IsNumber(target))
            {
                return false;
            }

            return CompiledRegexPatternLabelValidate.Match(target).Success &&
                   !CompiledRegexPatternLabelInvalid.Match(target).Success;
        }

    }
}
