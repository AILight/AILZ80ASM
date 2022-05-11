using AILZ80ASM.Assembler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AILZ80ASM.AILight
{
    public static class AIName
    {
        private static readonly string RegexPatternLabelValidate = @"^[a-zA-Z0-9_]+$";
        private static readonly string RegexPatternMacroValidate = @"^[a-zA-Z0-9_()]+$";
        private static readonly string RegexPatternLocalLabelNOValidate = @"^[a-zA-Z0-9_]+$";
        private static readonly string RegexPatternLocalLabelATValidate = @"^@[0-9]+$";
        private static readonly string RegexPatternLabelInvalid = @"^[0-9]";
        private static readonly string RegexPatternCharMapInvalid = @"^@[a-zA-Z0-9_]+$";

        public static bool DeclareLabelValidate(string target, AsmLoad asmLoad)
        {
            if (target.StartsWith(".") && target.EndsWith(":"))
            {
                target = target.Substring(1, target.Length - 2);
                while (target.EndsWith(":"))
                {
                    target = target.Substring(0, target.Length - 1);
                }

                return ValidateNameForLocalLabel(target, asmLoad);
            }

            // ラベルの名称だけを取得
            if (target.StartsWith("[") && target.EndsWith("]"))
            {
                target = target.Substring(1, target.Length - 2);
            }

            if (target.EndsWith(":"))
            {
                while (target.EndsWith(":"))
                {
                    target = target.Substring(0, target.Length - 1);
                }
            }

            if (target.StartsWith("."))
            {
                target = target.Substring(1);

                return ValidateNameForLocalLabel(target, asmLoad);
            }
            else
            {
                var splits = target.Split('.');
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
            if (target.StartsWith("(") || target.StartsWith(")"))
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
            while (startIndex < target.Length)
            {
                var commaIndex = AIString.IndexOfSkipString(target, ',', startIndex);
                var kakkoIndex = AIString.IndexOfSkipString(target, '(', startIndex);
                if (commaIndex == -1)
                {
                    argumentList.Add(target.Substring(startIndex).Trim());
                    startIndex = target.Length;
                }
                else if (kakkoIndex == -1 || commaIndex < kakkoIndex)
                {
                    argumentList.Add(target.Substring(startIndex, commaIndex - startIndex).Trim());
                    startIndex = commaIndex + 1;
                    // 最後のカンマで、次のデータが無い場合には空を積む
                    if (startIndex == target.Length)
                    {
                        argumentList.Add("");
                    }
                }
                else
                {
                    var searchIndex = kakkoIndex + 1;
                    var kakkoCounter = 1;
                    // カッコ開始から終了まで移動させる
                    while (kakkoCounter > 0 && searchIndex < target.Length)
                    {
                        kakkoIndex = AIString.IndexOfSkipString(target, '(', searchIndex);
                        var closeIndex = AIString.IndexOfSkipString(target, ')', searchIndex);
                        if (closeIndex == -1)
                        {
                            throw new Exception("カッコの数が不一致です");
                        }
                        else if (kakkoIndex == -1 || closeIndex < kakkoIndex)
                        {
                            kakkoCounter--;
                            searchIndex = closeIndex + 1;
                        }
                        else if (kakkoIndex < closeIndex)
                        {
                            kakkoCounter++;
                            searchIndex = kakkoIndex + 1;
                        }
                        // カッコを飛び越えたら、次のカンマと比較する
                        if (kakkoCounter == 0)
                        {
                            // 次のカンマとカッコの開始を調べる
                            commaIndex = AIString.IndexOfSkipString(target, ',', searchIndex);
                            kakkoIndex = AIString.IndexOfSkipString(target, '(', searchIndex);
                            if (commaIndex != -1 && kakkoIndex == -1)
                            {
                                // カンマはあるけど、カッコが無い場合は、カンマの前まで
                                searchIndex = commaIndex;
                            }
                            else if (kakkoIndex != -1 && 
                                    (commaIndex == -1 || kakkoIndex < commaIndex))
                            {
                                // カッコが存在してて、カンマが無い場合 or カッコがカンマの手前
                                // カッコのサーチをもう一度行う
                                searchIndex = kakkoIndex + 1;
                                kakkoCounter = 1;
                            }
                        }
                    }
                    argumentList.Add(target.Substring(startIndex, searchIndex - startIndex).Trim());
                    startIndex = searchIndex + 1;
                    // 最後のカンマで、次のデータが無い場合には空を積む
                    if (startIndex == target.Length && commaIndex != -1)
                    {
                        argumentList.Add("");
                    }

                }
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

            return Regex.Match(target, RegexPatternCharMapInvalid, RegexOptions.Singleline | RegexOptions.IgnoreCase).Success;
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

            return  Regex.Match(target, RegexPatternLabelValidate, RegexOptions.Singleline | RegexOptions.IgnoreCase).Success &&
                   !Regex.Match(target, RegexPatternLabelInvalid,  RegexOptions.Singleline | RegexOptions.IgnoreCase).Success;
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

            return Regex.Match(target, RegexPatternMacroValidate, RegexOptions.Singleline | RegexOptions.IgnoreCase).Success &&
                   !Regex.Match(target, RegexPatternLabelInvalid, RegexOptions.Singleline | RegexOptions.IgnoreCase).Success;
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

            // @と通常ラベルの処理
            return Regex.Match(target, RegexPatternLocalLabelATValidate, RegexOptions.Singleline | RegexOptions.IgnoreCase).Success ||
                   Regex.Match(target, RegexPatternLocalLabelNOValidate, RegexOptions.Singleline | RegexOptions.IgnoreCase).Success;
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

            return Regex.Match(target, RegexPatternLabelValidate, RegexOptions.Singleline | RegexOptions.IgnoreCase).Success &&
                   !Regex.Match(target, RegexPatternLabelInvalid, RegexOptions.Singleline | RegexOptions.IgnoreCase).Success;
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

            return Regex.Match(target, RegexPatternLabelValidate, RegexOptions.Singleline | RegexOptions.IgnoreCase).Success &&
                   !Regex.Match(target, RegexPatternLabelInvalid, RegexOptions.Singleline | RegexOptions.IgnoreCase).Success;
        }
    }
}
