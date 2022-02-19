﻿using AILZ80ASM.Assembler;
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
                return false;
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
                return ValidateName(target, asmLoad);
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
        /// 引数のラベルチェック
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static bool ValidateFunctionArgument(string target, AsmLoad asmLoad)
        {
            if (string.IsNullOrEmpty(target))
            {
                return false;
            }

            return ValidateName(target, asmLoad);
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
            var argument = "";
            var skipCounter = 0;
            var stringMode = 0;
            var skipMode = false;

            foreach (var item in target.ToArray())
            {
                if (skipMode)
                {
                    skipMode = false;
                }
                else if (item == '\"' && (stringMode == 0 || stringMode == 1))
                {
                    if (stringMode == 0)
                    {
                        stringMode = 1;
                    }
                    else
                    {
                        stringMode = 0;
                    }
                }
                else if (item == '\'' && (stringMode == 0 || stringMode == 2))
                {
                    if (stringMode == 0)
                    {
                        stringMode = 2;
                    }
                    else
                    {
                        stringMode = 0;
                    }
                }
                else if (stringMode > 0 && item == '\\')
                {
                    skipMode = true;
                }
                else if (stringMode == 0)
                {
                    if (item == ',' && skipCounter == 0)
                    {
                        argumentList.Add(argument.Trim());
                        argument = "";
                        continue;
                    }
                    else if (item == '(')
                    {
                        skipCounter++;
                    }
                    else if (item == ')')
                    {
                        skipCounter--;
                        if (skipCounter < 0)
                        {
                            throw new Exception("カッコの数が不一致です");
                        }
                    }
                }
                argument += item;
            }
            argumentList.Add(argument.Trim());

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

            if (AIMath.IsNumber(target))
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

            if (AIMath.IsNumber(target))
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

            if (AIMath.IsNumber(target))
            {
                return false;
            }

            // レジスター文字列、命令の文字列は利用不可
            /*
            if (asmLoad.ISA.IsMatchRegisterName(target))
            {
                return false;
            }
            if (asmLoad.ISA.IsMatchInstructionName(target))
            {
                return false;
            }
            */

            return Regex.Match(target, RegexPatternLabelValidate, RegexOptions.Singleline | RegexOptions.IgnoreCase).Success &&
                   !Regex.Match(target, RegexPatternLabelInvalid, RegexOptions.Singleline | RegexOptions.IgnoreCase).Success;
        }
    }
}
