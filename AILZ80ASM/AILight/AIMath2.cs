﻿using AILZ80ASM.Assembler;
using AILZ80ASM.Exceptions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;

namespace AILZ80ASM.AILight
{
    public static class AIMath2
    {
        private enum MacroValueEnum
        {
            None,
            High,
            Low,
            Text,
            Exists,
        }

        private static readonly string RegexPatternFormulaChar = @"(?<formula>(\+|\-|\*|\/|\%|\~|\(|\)|!=|!|==|\<\<|\>\>|<=|\<|>=|\>|\&\&|\|\||\&|\||\^|\?|\:))";

        public static bool TryParse(string value, out AIValue resultValue)
        {
            return InternalTryParse(value, default(AsmLoad), default(AsmAddress?), out resultValue);
        }

        public static bool TryParse(string value, AsmLoad asmLoad, out AIValue resultValue)
        {
            return InternalTryParse(value, asmLoad, default(AsmAddress?), out resultValue);
        }

        public static bool TryParse(string value, AsmLoad asmLoad, AsmAddress? asmAddress, out AIValue resultValue)
        {
            return InternalTryParse(value, asmLoad, asmAddress, out resultValue);
        }

        public static AIValue Calculation(string value)
        {
            return Calculation(value, default(AsmLoad), default(AsmAddress?));
        }

        public static AIValue Calculation(string value, AsmLoad asmLoad)
        {
            return Calculation(value, asmLoad, default(AsmAddress?));
        }

        /// <summary>
        /// 式の文字列から演算を行う
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static AIValue Calculation(string target, AsmLoad asmLoad, AsmAddress? asmAddress)
        {
            try
            {
                if (string.IsNullOrEmpty(target))
                {
                    throw new InvalidAIMathException("式が空文字です");
                }

                var terms = CalculationParse(target);
                CalculationSetValue(terms, asmLoad, asmAddress);

                var rvpns = CalculationMakeReversePolish(terms);
                var value = CalculationByReversePolish(rvpns, asmLoad, asmAddress);

                return value;
            }
            catch (ErrorAssembleException)
            {
                throw;
            }
            catch (InvalidAIValueException ex)
            {
                throw new ErrorAssembleException(Error.ErrorCodeEnum.E0004, $"演算対象：{target} エラー:{ex.Message}");
            }
            catch (InvalidAIMathException ex)
            {
                throw new ErrorAssembleException(Error.ErrorCodeEnum.E0004, $"演算対象：{target} エラー:{ex.Message}");
            }
            catch (Exception)
            {
                throw new ErrorAssembleException(Error.ErrorCodeEnum.E0004, $"演算対象：{target}");
            }
        }

        /// <summary>
        /// 式を分解する
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static AIValue[] CalculationParse(string value)
        {
            var terms = new List<AIValue>();
            var tmpValue = value.Trim();
            var matched = default(Match);

            // 数式との分離
            while (!string.IsNullOrEmpty(tmpValue))
            {
                if (tmpValue.StartsWith("\""))
                {
                    var endIndex = tmpValue.IndexOf("\"", 1);
                    var escapeIndex = tmpValue.IndexOf("\\", 1);
                    while (endIndex != -1 && escapeIndex != -1 && endIndex - 1 == escapeIndex)
                    {
                        endIndex = tmpValue.IndexOf("\"", endIndex + 1);
                        escapeIndex = tmpValue.IndexOf("\\", endIndex + 1);
                    }
                    if (endIndex == -1)
                    {
                        throw new InvalidAIMathException("演算に使えない文字が検出されました。");
                    }
                    else
                    {
                        terms.Add(new AIValue(tmpValue.Substring(0, endIndex + 1)));
                        tmpValue = tmpValue.Substring(endIndex + 1).TrimStart();
                    }
                }
                else if (tmpValue.StartsWith("'"))
                {
                    var endIndex = tmpValue.IndexOf("'", 1);
                    var escapeIndex = tmpValue.IndexOf("\\", 1);

                    if (escapeIndex != -1 && endIndex > escapeIndex && tmpValue.Length > escapeIndex + 2)
                    {
                        endIndex = tmpValue.IndexOf("'", escapeIndex + 2);
                    }

                    if (endIndex == -1)
                    {
                        throw new InvalidAIMathException("演算に使えない文字が検出されました。");
                    }
                    else
                    {
                        terms.Add(new AIValue(tmpValue.Substring(0, endIndex + 1)));
                        tmpValue = tmpValue.Substring(endIndex + 1).TrimStart();
                    }
                }
                else if ((matched = Regex.Match(tmpValue, "^" + RegexPatternFormulaChar, RegexOptions.Singleline | RegexOptions.IgnoreCase)).Success)
                {
                    var formula = matched.Groups["formula"].Value;
                    terms.Add(new AIValue(formula, AIValue.ValueTypeEnum.Operation));
                    tmpValue = tmpValue.Substring(formula.Length).TrimStart();
                }
                else
                {
                    matched = Regex.Match(tmpValue, RegexPatternFormulaChar, RegexOptions.Singleline | RegexOptions.IgnoreCase);
                    if (matched.Success)
                    {
                        //Functionの判断
                        var group = matched.Groups["formula"];
                        if (group.Value.StartsWith("("))
                        {
                            //Functionの切り出し
                            var valueString = "";
                            var skipCounter = 0;
                            foreach (var item in tmpValue.ToArray())
                            {
                                valueString += item;
                                if (item == '(')
                                {
                                    skipCounter++;
                                }
                                else if (item == ')')
                                {
                                    skipCounter--;
                                    if (skipCounter == 0)
                                    {
                                        break;
                                    }
                                    else if (skipCounter < 0)
                                    {
                                        throw new InvalidAIMathException("カッコの数が不一致です");
                                    }
                                }
                            }
                            terms.Add(new AIValue(valueString, AIValue.ValueTypeEnum.Function));
                            tmpValue = tmpValue.Substring(valueString.Length).TrimStart();
                        }
                        else
                        {
                            var valueString = tmpValue.Substring(0, group.Index);
                            terms.Add(new AIValue(valueString.Trim()));
                            tmpValue = tmpValue.Substring(valueString.Length).TrimStart();
                        }
                    }
                    else
                    {
                        terms.Add(new AIValue(tmpValue));
                        tmpValue = "";
                    }
                }
            }

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
            var checkValues = result.Where(m => !m.IsOperation(AIValue.OperationTypeEnum.LeftParenthesis) && 
                                                !m.IsOperation(AIValue.OperationTypeEnum.RightParenthesis)).ToArray();
            if (checkValues.Length > 0)
            {
                foreach (var index in Enumerable.Range(0, checkValues.Length - 1))
                {
                    if (!checkValues[index + 0].IsOperation() &&
                        !checkValues[index + 1].IsOperation())
                    {
                        throw new InvalidAIMathException("数値と数値の間には演算子が必要です");
                    }

                    if (checkValues[index + 0].IsOperation() &&
                        checkValues[index + 1].IsOperation())
                    {
                        throw new InvalidAIMathException("演算子が連続で指定されています。");
                    }
                }
            }
            return result.ToArray();
        }

        private static void CalculationSetValue(AIValue[] terms, AsmLoad asmLoad, AsmAddress? asmAddress)
        {
            foreach (var item in terms.Where(m => m.ValueType == AIValue.ValueTypeEnum.None).ToArray())
            {
                item.SetValue(asmLoad, asmAddress);
            }
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
                        else if (formura.Count == 0 || item.IsOperation(AIValue.OperationTypeEnum.LeftParenthesis) || formura.Peek().OperationPriority > item.OperationPriority)
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
            var checkValues = result.ToArray();
            if (checkValues.Length > 0)
            {
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
        private static AIValue CalculationByReversePolish(AIValue[] rpns, AsmLoad asmLoad, AsmAddress? asmAddress)
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

                                var resultValue = AIValue.Calculation(item, firstValue);
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

                                var resultValue = AIValue.Calculation(item, firstValue, secondPopValue);
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

                                var resultValue = AIValue.Calculation(item, firstValue, secondPopValue, thirdPopValue);
                                stack.Push(resultValue);
                            }
                            break;
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
        private static bool InternalTryParse(string value, AsmLoad asmLoad, AsmAddress? asmAddress, out AIValue resultValue)
        {
            try
            {
                resultValue = Calculation(value, asmLoad, asmAddress);
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