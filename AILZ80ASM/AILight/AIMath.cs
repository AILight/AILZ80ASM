using AILZ80ASM.Assembler;
using AILZ80ASM.Exceptions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AILZ80ASM.AILight
{
    public static class AIMath
    {
        private static readonly string RegexPatternCharMap = @"^((?<charMap>@.*\:)\s*|)(""|')";
        private static readonly Regex CompiledRegexPatternCharMap = new Regex(
            RegexPatternCharMap,
            RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase
        );

        private static readonly string RegexPatternCharMapLabel = @"^((?<charMap>@.*\:)(?<label>[a-zA-Z0-9_]+))";
        private static readonly Regex CompiledRegexPatternCharMapLabel = new Regex(
            RegexPatternCharMapLabel,
            RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase
        );

        public static string[] LabelOperatorStrings => LabelOperatorDic.SelectMany(m => m.Value).ToArray();
        public static Dictionary<string, string[]> LabelOperatorDic => new Dictionary<string, string[]>
        {
            ["high"] = new[] { ".@H", ".@HIGH" },
            ["low"] = new[] { ".@L", ".@LOW" },
            ["text"] = new[] { ".@T", ".@TEXT" },
            ["exists"] = new[] { ".@E", ".@EXISTS" },
            ["backward"] = new[] { ".@B", ".@BACKWARD" },
            ["forward"] = new[] { ".@F", ".@FORWARD" },
            ["near"] = new[] { ".@NEAR" },
            ["far"] = new[] { ".@FAR" },
        };

        public static bool TryParse(string value, out AIValue resultValue)
        {
            return InternalTryParse(value, default(AsmLoad), default(AsmAddress?), new List<Label>(), out resultValue);
        }

        public static bool TryParse(string value, AsmLoad asmLoad, out AIValue resultValue)
        {
            return InternalTryParse(value, asmLoad, default(AsmAddress?), new List<Label>(), out resultValue);
        }

        public static bool TryParse(string value, AsmLoad asmLoad, AsmAddress? asmAddress, out AIValue resultValue)
        {
            return InternalTryParse(value, asmLoad, asmAddress, new List<Label>(), out resultValue);
        }

        public static AIValue Calculation(string value)
        {
            return Calculation(value, default(AsmLoad), default(AsmAddress?), new List<Label>());
        }

        public static AIValue Calculation(string value, AsmLoad asmLoad)
        {
            return Calculation(value, asmLoad, default(AsmAddress?), new List<Label>());
        }

        public static AIValue Calculation(string target, AsmLoad asmLoad, AsmAddress? asmAddress)
        {
            return Calculation(target, asmLoad, asmAddress, new List<Label>());
        }

        /// <summary>
        /// 式の文字列から演算を行う
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static AIValue Calculation(string target, AsmLoad asmLoad, AsmAddress? asmAddress, List<Label> entryLabels)
        {
            return AsmException.TryCatch(Error.ErrorCodeEnum.E0004, target, () => 
            {
                if (string.IsNullOrEmpty(target))
                {
                    throw new InvalidAIMathException("式が空文字です");
                }

                var terms = CalculationParse(target);            // 式の分解
                var lbmcs = CalculationLabelMacro(terms);        // .@系命令を演算子に置き換える
                var rvpns = CalculationMakeReversePolish(lbmcs); // 逆ポーランド
                var value = CalculationByReversePolish(rvpns, asmLoad, asmAddress, entryLabels); // 演算
                value.SetValue(asmLoad, asmAddress, entryLabels);            // 未確定の値になる場合に値を確定させる

                return value;
            });
        }

        private static AIValue[] CalculationLabelMacro(AIValue[] terms)
        {
            var result = new List<AIValue>();
            foreach (var item in terms)
            {
                if (item.ValueType == AIValue.ValueTypeEnum.None)
                {
                    // ラベル演算子を処理する
                    var operations = new List<AIValue>();
                    var labelName = item.OriginalValue;
                    var atmarkIndex = default(int);
                    while ((atmarkIndex = labelName.LastIndexOf(".@")) >= 0 &&
                           AIMath.LabelOperatorStrings.Any(m => labelName.EndsWith(m, StringComparison.CurrentCultureIgnoreCase)))
                    {
                        var operation = labelName.Substring(atmarkIndex);
                        labelName = labelName.Substring(0, atmarkIndex);
                        var notFound = true;

                        foreach (var labelOperation in LabelOperatorDic)
                        {
                            foreach (var shortHand in labelOperation.Value)
                            {
                                if (string.Compare(operation, shortHand, true) == 0)
                                {
                                    operations.Add(new AIValue(labelOperation.Key, AIValue.ValueTypeEnum.Operation));
                                    notFound = false;
                                    break;
                                }
                            }
                            if (!notFound)
                            {
                                break;
                            }
                        }

                        if (notFound)
                        {
                            // ここに来ることはないが、来たら例外を発生させる
                            throw new Exception();
                        }
                    }

                    if (operations.Count > 0)
                    {
                        result.AddRange(operations);
                        result.Add(new AIValue(labelName));
                        continue;
                    }
                }
                result.Add(item);
            }
            return result.ToArray();
        }

        /// <summary>
        /// 式を分解する
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static AIValue[] CalculationParse(string value)
        {
            // 式の分解処理
            var terms = new List<AIValue>();
            var tmpValue = value.Trim();

            // 数式との分離
            while (!string.IsNullOrEmpty(tmpValue))
            {
                if (TryParseString(ref tmpValue, out var resultString))
                {
                    terms.Add(new AIValue(resultString));
                }
                else if (AIValue.TryParseFormula(ref tmpValue, out var resultFormula))
                {
                    terms.Add(new AIValue(resultFormula, AIValue.ValueTypeEnum.Operation));
                }
                else if (AIValue.TryParseFunction(ref tmpValue, out var resultFunction))
                {
                    terms.Add(new AIValue(resultFunction, AIValue.ValueTypeEnum.Function));
                }
                else if (AIValue.TryParseSyntaxSuger(ref tmpValue, out var resultValues))
                {
                    foreach (var resultValue in resultValues)
                    {
                        terms.Add(new AIValue(resultValue));
                    }
                }    
                else if (AIValue.TryParseValue(ref tmpValue, out var resultValue))
                {
                    terms.Add(new AIValue(resultValue));
                }
                else
                {
                    throw new InvalidAIMathException("演算に使えない文字が含まれています");
                }
            }

            // 分解が小さい場合は抜ける(高速化対応)
            if (terms.Count <= 1)
            {
                return terms.ToArray();
            }

            // 符号処理
            var result = new List<AIValue>();
            for (int index = 0; index < terms.Count; index++)
            {
                var tmpTerm = terms[index];
                if (tmpTerm.IsOperationSignOrBNumber())
                {
                    if (index == 0 || !terms[index - 1].IsOperation(AIValue.OperationTypeEnum.RightParenthesis) && terms[index - 1].ValueType == AIValue.ValueTypeEnum.Operation)
                    {
                        if (index >= terms.Count - 1)
                        {
                            throw new ErrorAssembleException(Error.ErrorCodeEnum.E0023, value);
                        }
                        index++;
                        var addTerm = terms[index];

                        if (tmpTerm.IsOperation(AIValue.OperationTypeEnum.Remainder))
                        {
                            result.Add(new AIValue(tmpTerm, addTerm));
                        }
                        else
                        {
                            result.Add(new AIValue("(", AIValue.ValueTypeEnum.Operation));
                            result.Add(new AIValue("0", AIValue.ValueTypeEnum.Int32));
                            result.Add(tmpTerm);
                            result.Add(addTerm);
                            result.Add(new AIValue(")", AIValue.ValueTypeEnum.Operation));
                        }
                        continue;
                    }
                }

                result.Add(tmpTerm);
            }

            // 演算子、数値が連続しているものがないか確認をする
            if (result.Count >= 2)
            {
                var checkValues = result.Where(m => !m.IsOperation(AIValue.OperationTypeEnum.LeftParenthesis) &&
                                                    !m.IsOperation(AIValue.OperationTypeEnum.RightParenthesis)).ToArray();
                if (checkValues.Length >= 2)
                {
                    var firstOperation = checkValues.First().IsOperation();
                    foreach (var item in checkValues.Skip(1))
                    {
                        var secondOperation = item.IsOperation();
                        if (!firstOperation && !secondOperation)
                        {
                            throw new InvalidAIMathException("数値と数値の間には演算子が必要です");
                        }

                        if (firstOperation && secondOperation && item.ArgumentType != AIValue.ArgumentTypeEnum.SingleArgument)
                        {
                            throw new InvalidAIMathException("演算子が連続で指定されています。");
                        }
                        firstOperation = secondOperation;
                    }
                }
            }
            return result.ToArray();
        }

        private static bool TryParseString(ref string tmpValue, out string resultString)
        {
            var matched = CompiledRegexPatternCharMap.Match(tmpValue);
            if (matched.Success)
            {
                var stringCheck = tmpValue;
                var checkStartIndex = 0;
                if (!string.IsNullOrEmpty(matched.Groups["charMap"].Value))
                {
                    checkStartIndex = matched.Groups["charMap"].Value.Length;
                    stringCheck = tmpValue.Substring(checkStartIndex);
                }

                // 文字列
                if (stringCheck.StartsWith('\"'))
                {
                    ParseString("\"", checkStartIndex, ref tmpValue, out resultString);
                    return true;
                }
                else if (stringCheck.StartsWith('\''))
                {
                    ParseString("'", checkStartIndex, ref tmpValue, out resultString);
                    return true;
                }
            }
            else
            {
                // ラベルを使ってのCharMap指定
                var matchedLabel = CompiledRegexPatternCharMapLabel.Match(tmpValue);
                if (matchedLabel.Success)
                {
                    resultString = matchedLabel.Value;
                    tmpValue = tmpValue.Substring(resultString.Length);
                    return true;
                }
            }

            resultString = "";
            return false;
        }

        private static void ParseString(string stringMarkChar, int checkStartIndex, ref string tmpValue, out string resultString)
        {
            var endIndex = tmpValue.IndexOf(stringMarkChar, checkStartIndex + 1);
            var escapeIndex = tmpValue.IndexOf("\\", checkStartIndex + 1);
            while (endIndex != -1 && escapeIndex != -1 && endIndex - 1 == escapeIndex)
            {
                endIndex = tmpValue.IndexOf(stringMarkChar, endIndex + 1);
                escapeIndex = tmpValue.IndexOf("\\", endIndex + 1);
            }

            if (endIndex == -1)
            {
                if (escapeIndex == -1)
                {
                    throw new InvalidAIMathException("演算に使えない文字が検出されました。");
                }
                else
                {
                    throw new InvalidAIStringEscapeSequenceException($"有効なエスケープシーケンスではありません。[{tmpValue}]", tmpValue);
                }
            }

            resultString = tmpValue.Substring(0, endIndex + 1);
            tmpValue = tmpValue.Substring(endIndex + 1).TrimStart();

        }

        /// <summary>
        /// 逆ポーランド記法に変換する
        /// </summary>
        /// <param name="terms"></param>
        /// <returns></returns>
        private static AIValue[] CalculationMakeReversePolish(AIValue[] terms)
        {
            var result = new List<AIValue>();
            var formura = new Stack<AIValue>();

            foreach (var item in terms)
            {
                if (item.IsOperation())
                {
                    while (true)
                    {
                        if (item.IsOperation(AIValue.OperationTypeEnum.RightParenthesis))
                        {
                            while (formura.Count > 0 && !formura.Peek().IsOperation(AIValue.OperationTypeEnum.LeftParenthesis))
                            {
                                result.Add(formura.Pop());
                            }
                            if (formura.Count == 0)
                            {
                                throw new InvalidAIMathException("括弧の数が不一致です");
                            }
                            formura.Pop();
                            break;
                        }
                        else if (formura.Count == 0 || 
                                 item.IsOperation(AIValue.OperationTypeEnum.LeftParenthesis) ||
                                 formura.Peek().OperationPriority > item.OperationPriority ||
                                 (formura.Peek().ArgumentType == AIValue.ArgumentTypeEnum.SingleArgument && item.ArgumentType == AIValue.ArgumentTypeEnum.SingleArgument))
                        {
                            formura.Push(item);
                            break;
                        }
                        else
                        {
                            result.Add(formura.Pop());
                        }
                    }
                }
                else
                {
                    result.Add(item);
                }
            }

            result.AddRange(formura);
            if (result.Any(m => m.IsOperation(AIValue.OperationTypeEnum.LeftParenthesis)))
            {
                throw new InvalidAIMathException("括弧の数が不一致です");
            }

            // 三項演算子のチェック
            if (result.Count > 0)
            {
                var checkValues = result.ToArray();
                foreach (var index in Enumerable.Range(0, checkValues.Length - 1))
                {
                    if (( checkValues[index + 0].IsOperation(AIValue.OperationTypeEnum.Ternary_Colon) && !checkValues[index + 1].IsOperation(AIValue.OperationTypeEnum.Ternary_Question)) ||
                        (!checkValues[index + 0].IsOperation(AIValue.OperationTypeEnum.Ternary_Colon) &&  checkValues[index + 1].IsOperation(AIValue.OperationTypeEnum.Ternary_Question)))
                    {
                        throw new InvalidAIMathException("三項演算子の使い方が間違っています。");
                    }
                }
            }

            return result.ToArray();
        }

        /// <summary>
        /// 逆ポーランド記法から演算を行う
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rpns"></param>
        /// <returns></returns>
        private static AIValue CalculationByReversePolish(AIValue[] rpns, AsmLoad asmLoad, AsmAddress? asmAddress, List<Label> entryLabels)
        {
            var stack = new Stack<AIValue>();

            foreach (var item in rpns)
            {
                if (item.IsOperation())
                {
                    switch (item.ArgumentType)
                    {
                        case AIValue.ArgumentTypeEnum.None:
                            {
                                // 処理しない
                            }
                            break;
                        case AIValue.ArgumentTypeEnum.SingleArgument:
                            {
                                if (stack.Count < 1)
                                {
                                    throw new InvalidAIMathException("演算に失敗しました。");
                                }

                                var firstValue = stack.Pop();

                                var resultValue = AIValue.Calculation(item, firstValue, asmLoad, asmAddress, entryLabels);
                                stack.Push(resultValue);
                            }
                            break;
                        case AIValue.ArgumentTypeEnum.DoubleArgument:
                            {
                                if (stack.Count < 2)
                                {
                                    throw new InvalidAIMathException("演算に失敗しました。");
                                }
                                var secondPopValue = stack.Pop();
                                var firstValue = stack.Pop();

                                var resultValue = AIValue.Calculation(item, firstValue, secondPopValue, asmLoad, asmAddress, entryLabels);
                                stack.Push(resultValue);
                            }
                            break;
                        case AIValue.ArgumentTypeEnum.TripleArgument:
                            {
                                if (stack.Count < 3)
                                {
                                    throw new InvalidAIMathException("演算に失敗しました。");
                                }
                                var thirdPopValue = stack.Pop();
                                var secondPopValue = stack.Pop();
                                var firstValue = stack.Pop();

                                var resultValue = AIValue.Calculation(item, firstValue, secondPopValue, thirdPopValue, asmLoad, asmAddress, entryLabels);
                                stack.Push(resultValue);
                            }
                            break;
                        default:
                            throw new InvalidOperationException();
                    }
                }
                else
                {
                    stack.Push(item);
                }
            }

            return stack.Pop();
        }

        /// <summary>
        /// 演算可能かを判断し、可能なら演算をする
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="asmLoad"></param>
        /// <param name="asmAddress"></param>
        /// <param name="resultValue"></param>
        /// <returns></returns>
        private static bool InternalTryParse(string value, AsmLoad asmLoad, AsmAddress? asmAddress, List<Label> entryLabels, out AIValue resultValue)
        {
            try
            {
                resultValue = Calculation(value, asmLoad, asmAddress, entryLabels);
                return true;
            }
            catch
            {
                resultValue = default(AIValue);
                return false;
            }
        }

    }
}
