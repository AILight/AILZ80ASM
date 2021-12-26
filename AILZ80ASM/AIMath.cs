using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;

namespace AILZ80ASM
{
    public static class AIMath
    {
        private enum MacroValueEnum
        {
            None,
            High,
            Low,
            Text,
        }

        private static readonly string RegexPatternHexadecimal_H =   @"^(?<value>([0-9A-Fa-f_]+))H$";
        private static readonly string RegexPatternHexadecimal_X = @"^0x(?<value>([0-9A-Fa-f_]+))$";
        private static readonly string RegexPatternHexadecimal_D = @"^\$(?<value>([0-9A-Fa-f_]+))$";
        private static readonly string RegexPatternOctal_O = @"^(?<value>([0-7_]+))O$";
        private static readonly string RegexPatternBinaryNumber_B = @"^(?<value>([01_]+))B$";
        private static readonly string RegexPatternBinaryNumber_P = @"^%(?<value>([01_]+))$";
        private static readonly string RegexPatternChar = @"^'(?<value>(.|\\.))'$";
        private static readonly string RegexPatternDigit = @"^(\+|\-|)(\d+)$";
        private static readonly string RegexPatternFormulaChar = @"(?<formula>(\+|\-|\*|\/|\%|\~|\(|\)|!=|!|==|\<\<|\>\>|<=|\<|>=|\>|\&\&|\|\||\&|\||\^|\?|\:))";
        private static readonly Dictionary<string, int> FormulaPriority = new()
        {
            [")"] = 1,
            ["!"] = 2,  ["~"] = 2, // 単項演算子は別で処理する ["+"] = 2,  ["-"] = 2,
            ["*"] = 3,  ["/"] = 3, ["%"] = 3,
            ["+"] = 4,  ["-"] = 4,
            ["<<"] = 5, [">>"] = 5,
            ["<"] = 6,  [">"] = 6, ["<="] = 6, [">="] = 6,
            ["=="] = 7, ["!="] = 7,
            ["&"] = 8,
            ["^"] = 9,
            ["|"] = 10,
            ["&&"] = 11,
            ["||"] = 12,
            ["?"] = 14, [":"] = 13,
            ["("] = 15,

        };

        public static bool IsNumber(string value)
        {
            if (Regex.Match(value, RegexPatternHexadecimal_H, RegexOptions.Singleline | RegexOptions.IgnoreCase).Success)
            {
                return true;
            }
            if (Regex.Match(value, RegexPatternHexadecimal_X, RegexOptions.Singleline | RegexOptions.IgnoreCase).Success)
            {
                return true;
            }

            if (Regex.Match(value, RegexPatternHexadecimal_D, RegexOptions.Singleline | RegexOptions.IgnoreCase).Success)
            {
                return true;
            }
            
            if (Regex.Match(value, RegexPatternOctal_O, RegexOptions.Singleline | RegexOptions.IgnoreCase).Success)
            {
                return true;
            }

            if (Regex.Match(value, RegexPatternBinaryNumber_B, RegexOptions.Singleline | RegexOptions.IgnoreCase).Success)
            {
                return true;
            }

            if (Regex.Match(value, RegexPatternBinaryNumber_P, RegexOptions.Singleline | RegexOptions.IgnoreCase).Success)
            {
                return true;
            }

            if (decimal.TryParse(value, out var _))
            {
                return true;
            }

            return false;
        }

        public static bool TryParse<T>(string value, out T resultValue)
            where T : struct
        {
            return InternalTryParse(value, default(AsmLoad), default(AsmAddress?), out resultValue);
        }

        public static bool TryParse<T>(string value, AsmLoad asmLoad, out T resultValue)
            where T : struct
        {
            return InternalTryParse(value, asmLoad, default(AsmAddress?), out resultValue);
        }

        public static bool TryParse<T>(string value, AsmLoad asmLoad, AsmAddress? asmAddress, out T resultValue)
            where T : struct
        {
            return InternalTryParse(value, asmLoad, asmAddress, out resultValue);
        }

        public static T ConvertTo<T>(string value)
            where T : struct
        {
            return ConvertTo<T>(value, default(AsmLoad), default(AsmAddress?));
        }

        public static T ConvertTo<T>(string value, AsmLoad asmLoad)
            where T : struct
        {
            return ConvertTo<T>(value, asmLoad, default(AsmAddress?));
        }

        public static T ConvertTo<T>(string value, AsmLoad asmLoad, AsmAddress? asmAddress)
            where T : struct
        {
            return InternalConvertTo<T>(value, asmLoad, asmAddress);
        }

        /// <summary>
        /// 式の文字列から演算を行う
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        private static T Calculation<T>(string target, AsmLoad asmLoad, AsmAddress? asmAddress)
            where T : struct
        {
            if (string.IsNullOrEmpty(target))
            {
                throw new Exception("式が空文字です");
            }

            var terms = CalculationParse(target);
            var rvpns = CalculationMakeReversePolish(terms);
            var value = CalculationByReversePolish<T>(rvpns, asmLoad, asmAddress);

            return value;
        }

        /// <summary>
        /// 式を分解する
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static string[] CalculationParse(string value)
        {
            var terms = new List<string>();
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
                        throw new Exception("演算に使えない文字が検出されました。");
                    }
                    else
                    {
                        terms.Add(tmpValue.Substring(0, endIndex + 1));
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
                        throw new Exception("演算に使えない文字が検出されました。");
                    }
                    else
                    {
                        terms.Add(tmpValue.Substring(0, endIndex + 1));
                        tmpValue = tmpValue.Substring(endIndex + 1).TrimStart();
                    }
                }
                else if ((matched = Regex.Match(tmpValue, "^" + RegexPatternFormulaChar, RegexOptions.Singleline | RegexOptions.IgnoreCase)).Success)
                {
                    var formula = matched.Groups["formula"].Value;
                    terms.Add(formula);
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
                                        throw new Exception("カッコの数が不一致です");
                                    }
                                }
                            }
                            terms.Add(valueString);
                            tmpValue = tmpValue.Substring(valueString.Length).TrimStart();
                        }
                        else
                        {
                            var valueString = tmpValue.Substring(0, group.Index);
                            terms.Add(valueString.Trim());
                            tmpValue = tmpValue.Substring(valueString.Length).TrimStart();
                        }
                    }
                    else
                    {
                        terms.Add(tmpValue);
                        tmpValue = "";
                    }
                }
            }

            var result = new List<string>();
            var sign = "";
            // 単項演算子と%を結合する
            foreach (var index in Enumerable.Range(0, terms.Count))
            {
                var tmpString = terms[index];
                if (tmpString == "+" || tmpString == "-" || tmpString == "%")
                {
                    if (index == 0 || terms[index - 1] != ")" && Regex.Match(terms[index - 1], RegexPatternFormulaChar, RegexOptions.Singleline | RegexOptions.IgnoreCase).Success)
                    {
                        sign = tmpString;
                        continue;
                    }
                }

                result.Add(sign + tmpString);
                sign = "";
            }
            if (!string.IsNullOrEmpty(sign))
            {
                result.Add(sign);
            }

            // 演算子、数値が連続しているものがないか確認をする
            var checkValues = result.Where(m => m != "(" && m != ")").ToArray();
            if (checkValues.Length > 0)
            {
                foreach (var index in Enumerable.Range(0, checkValues.Length - 1))
                {
                    if (!Regex.Match(checkValues[index + 0], "^" + RegexPatternFormulaChar + "$", RegexOptions.Singleline | RegexOptions.IgnoreCase).Success &&
                        !Regex.Match(checkValues[index + 1], "^" + RegexPatternFormulaChar + "$", RegexOptions.Singleline | RegexOptions.IgnoreCase).Success)
                    {
                        throw new Exception("数値と数値の間には演算子が必要です");
                    }

                    if (Regex.Match(checkValues[index + 0], "^" + RegexPatternFormulaChar + "$", RegexOptions.Singleline | RegexOptions.IgnoreCase).Success &&
                        Regex.Match(checkValues[index + 1], "^" + RegexPatternFormulaChar + "$", RegexOptions.Singleline | RegexOptions.IgnoreCase).Success)
                    {
                        throw new Exception("演算子が連続で指定されています。");
                    }
                }
            }
            return result.ToArray();
        }

        /// <summary>
        /// 逆ポーランド記法に変換する
        /// </summary>
        /// <param name="terms"></param>
        /// <returns></returns>
        private static string[] CalculationMakeReversePolish(string[] terms)
        {
            var result = new List<string>();
            var formura = new Stack<string>();

            foreach (var item in terms)
            {
                var matched = Regex.Match(item, "^" + RegexPatternFormulaChar + "$", RegexOptions.Singleline | RegexOptions.IgnoreCase);
                if (matched.Success)
                {
                    while (true)
                    {
                        if (item == ")")
                        {
                            while (formura.Count > 0 && formura.Peek() != "(")
                            {
                                result.Add(formura.Pop());
                            }
                            if (formura.Count == 0)
                            {
                                throw new Exception("括弧の数が不一致です");
                            }
                            formura.Pop();
                            break;
                        }
                        else if (formura.Count == 0 || item == "(" || FormulaPriority[formura.Peek()] > FormulaPriority[item])
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
            if (result.Any(m => m == "("))
            {
                throw new Exception("括弧の数が不一致です");
            }

            // 三項演算子のチェック
            var checkValues = result.ToArray();
            if (checkValues.Length > 0)
            {
                foreach (var index in Enumerable.Range(0, checkValues.Length - 1))
                {
                    if ((checkValues[index + 0] == ":" && checkValues[index + 1] != "?") ||
                        (checkValues[index + 0] != ":" && checkValues[index + 1] == "?"))
                    {
                        throw new Exception("三項演算子の使い方が間違っています。");
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
        private static T CalculationByReversePolish<T>(string[] rpns, AsmLoad asmLoad, AsmAddress? asmAddress)
            where T : struct
        {
            var stack = new Stack<object>();

            foreach (var item in rpns)
            {
                var matched = default(Match);
                if (Regex.Match(item, "^" + RegexPatternFormulaChar + "$", RegexOptions.Singleline | RegexOptions.IgnoreCase).Success)
                {
                    switch (item)
                    {
                        case "+":
                        case "-":
                        case "*":
                        case "/":
                        case "%":
                        case "<<":
                        case ">>":
                        case ">":
                        case ">=":
                        case "<":
                        case "<=":
                        case "==":
                        case "!=":
                        case "&":
                        case "^":
                        case "|":
                            {
                                if (stack.Count < 2)
                                {
                                    throw new Exception("演算に失敗しました。");
                                }

                                var lastPopValue = stack.Pop();
                                var firstPopValue = stack.Pop();

                                if (lastPopValue is int lastValue &&
                                    firstPopValue is int firstValue)
                                {
                                    switch (item)
                                    {
                                        case "+":
                                            stack.Push(firstValue + lastValue);
                                            break;
                                        case "-":
                                            stack.Push(firstValue - lastValue);
                                            break;
                                        case "*":
                                            stack.Push(firstValue * lastValue);
                                            break;
                                        case "/":
                                            stack.Push(firstValue / lastValue);
                                            break;
                                        case "%":
                                            stack.Push(firstValue % lastValue);
                                            break;
                                        case "<<":
                                            stack.Push(firstValue << lastValue);
                                            break;
                                        case ">>":
                                            stack.Push(firstValue >> lastValue);
                                            break;
                                        case ">":
                                            stack.Push(firstValue > lastValue);
                                            break;
                                        case ">=":
                                            stack.Push(firstValue >= lastValue);
                                            break;
                                        case "<":
                                            stack.Push(firstValue < lastValue);
                                            break;
                                        case "<=":
                                            stack.Push(firstValue <= lastValue);
                                            break;
                                        case "==":
                                            stack.Push(firstValue == lastValue);
                                            break;
                                        case "!=":
                                            stack.Push(firstValue != lastValue);
                                            break;
                                        case "&":
                                            stack.Push(firstValue & lastValue);
                                            break;
                                        case "^":
                                            stack.Push(firstValue ^ lastValue);
                                            break;
                                        case "|":
                                            stack.Push(firstValue | lastValue);
                                            break;
                                        default:
                                            throw new NotImplementedException();
                                    }
                                }
                                else if (lastPopValue is string lastStringValue &&
                                         firstPopValue is string firstStringValue)
                                {
                                    switch (item)
                                    {
                                        case "+":
                                            stack.Push(firstStringValue + lastStringValue);
                                            break;
                                        case "==":
                                            stack.Push(string.Compare(firstStringValue, lastStringValue, true) == 0);
                                            break;
                                        case "!=":
                                            stack.Push(string.Compare(firstStringValue, lastStringValue, true) != 0);
                                            break;
                                        default:
                                            throw new NotImplementedException();
                                    }
                                }
                                else
                                {
                                    throw new Exception($"演算に使う値の型が一致していません。{lastPopValue}{item}{firstPopValue}");
                                }


                            }
                            break;
                        case "&&":
                        case "||":
                            {
                                if (stack.Count < 2)
                                {
                                    throw new Exception("演算に失敗しました。");
                                }

                                var lastValue = (bool)stack.Pop();
                                var firstValue = (bool)stack.Pop();

                                switch (item)
                                {
                                    case "&&":
                                        stack.Push(firstValue && lastValue);
                                        break;
                                    case "||":
                                        stack.Push(firstValue || lastValue);
                                        break;
                                    default:
                                        throw new NotImplementedException();
                                }
                            }
                            break;
                        case "!":
                            {
                                if (stack.Count < 1)
                                {
                                    throw new Exception("演算に失敗しました。");
                                }

                                var firstValue = stack.Pop();
                                if (firstValue is bool boolValue)
                                {
                                    stack.Push(!boolValue);
                                }
                                else if (firstValue is int intValue)
                                {
                                    stack.Push(~intValue);
                                }
                            }
                            break;
                        case "~":
                            {
                                if (stack.Count < 1)
                                {
                                    throw new Exception("演算に失敗しました。");
                                }

                                var firstValue = (int)stack.Pop();
                                stack.Push(~firstValue);
                            }
                            break;
                        case ":":
                            // 三項演算子の記号、?で処理するのでここは無処理
                            break;
                        case "?":
                            {
                                if (stack.Count < 3)
                                {
                                    throw new Exception("演算に失敗しました。");
                                }

                                var falseValue = stack.Pop();
                                var trueValue = stack.Pop();
                                var conditionValue = (bool)stack.Pop();
                                if (conditionValue)
                                {
                                    stack.Push(trueValue);
                                }
                                else
                                {
                                    stack.Push(falseValue);
                                }
                            }
                            break;

                    }
                }
                else if (item.StartsWith("\"") && item.EndsWith("\""))
                {
                    stack.Push(item.Substring(1, item.Length - 2));
                }
                else if (Regex.Match(item, RegexPatternDigit, RegexOptions.Singleline | RegexOptions.IgnoreCase).Success)
                {
                    if (int.TryParse(item, out var result))
                    {
                        stack.Push(result);
                    }
                    else
                    {
                        throw new Exception($"数値に変換できませんでした。{item}");
                    }
                }
                else if ((matched = Regex.Match(item, RegexPatternHexadecimal_H, RegexOptions.Singleline | RegexOptions.IgnoreCase)).Success ||
                         (matched = Regex.Match(item, RegexPatternHexadecimal_X, RegexOptions.Singleline | RegexOptions.IgnoreCase)).Success ||
                         (matched = Regex.Match(item, RegexPatternHexadecimal_D, RegexOptions.Singleline | RegexOptions.IgnoreCase)).Success)
                {
                    // 16進数
                    try
                    {
                        stack.Push(Convert.ToInt32(matched.Groups["value"].Value.Replace("_", ""), 16));
                    }
                    catch
                    {
                        throw new Exception($"数値に変換できませんでした。{item}");
                    }
                }
                else if ((matched = Regex.Match(item, RegexPatternBinaryNumber_B, RegexOptions.Singleline | RegexOptions.IgnoreCase)).Success ||
                         (matched = Regex.Match(item, RegexPatternBinaryNumber_P, RegexOptions.Singleline | RegexOptions.IgnoreCase)).Success)
                {
                    // 2進数
                    try
                    {
                        stack.Push(Convert.ToInt32(matched.Groups["value"].Value.Replace("_", ""), 2));
                    }
                    catch
                    {
                        throw new Exception($"数値に変換できませんでした。{item}");
                    }
                }
                else if ((matched = Regex.Match(item, RegexPatternOctal_O, RegexOptions.Singleline | RegexOptions.IgnoreCase)).Success)
                {
                    // 8進数
                    try
                    {
                        stack.Push(Convert.ToInt32(matched.Groups["value"].Value.Replace("_", ""), 8));
                    }
                    catch
                    {
                        throw new Exception($"数値に変換できませんでした。{item}");
                    }
                }
                else if ((matched = Regex.Match(item, RegexPatternChar, RegexOptions.Singleline | RegexOptions.IgnoreCase)).Success)
                {
                    // 文字
                    try
                    {
                        var value = matched.Groups["value"].Value;
                        var chars = value.ToArray();
                        if (chars.Length == 1)
                        {
                            stack.Push(Convert.ToInt32(chars[0]));
                        }
                        else
                        {
                            stack.Push(value switch
                            {
                                "\\0" => 0,
                                "\\'" => 0x27,
                                _ => throw new Exception()
                            });
                        }

                    }
                    catch
                    {
                        throw new Exception($"数値に変換できませんでした。{item}");
                    }
                }
                else if (item == "$")
                {
                    if (!asmAddress.HasValue)
                    {
                        throw new ArgumentNullException(nameof(asmAddress));
                    }
                    // プログラム・ロケーションカウンター
                    stack.Push((int)asmAddress.Value.Program);
                }
                else if (item == "$$")
                {
                    if (!asmAddress.HasValue)
                    {
                        throw new ArgumentNullException(nameof(asmAddress));
                    }
                    // アウトプット・ロケーションカウンター
                    stack.Push((int)asmAddress.Value.Output);
                }
                else
                {
                    if (asmLoad == default)
                    {
                        throw new ArgumentNullException(nameof(asmLoad));
                    }

                    var startIndex = item.IndexOf('(');
                    if (startIndex != -1)
                    {
                        var functionName = item.Substring(0, startIndex).Trim();
                        var function = asmLoad.FindFunction(functionName);
                        var lastIndex = item.LastIndexOf(')');
                        if (function == default || lastIndex == -1)
                        {
                            throw new Exception($"Functionが見つかりませんでした。{functionName}");
                        }
                        var arguments = AIName.ParseArguments(item.Substring(startIndex + 1, lastIndex - startIndex - 1));

                        stack.Push(function.Calculation(arguments, asmLoad, asmAddress));
                    }
                    else
                    {
                        stack.Push(CalculationByReversePolishForLabel(item, asmLoad));
                    }

                }
            }

            return CalculationNormalization<T>(stack.Pop());
        }

        private static object CalculationByReversePolishForLabel(string target, AsmLoad asmLoad)
        {
            var macroValue = MacroValueEnum.None;
            var tmpLabel = target;
            var optionIndex = target.IndexOf(".@");
            var hasValue = true;
            if (optionIndex > 0)
            {
                var option = target.Substring(optionIndex);
                if (string.Compare(option, ".@H", true) == 0 ||
                    string.Compare(option, ".@HIGH", true) == 0)
                {
                    tmpLabel = target.Substring(0, optionIndex);
                    macroValue = MacroValueEnum.High;
                }
                else if (string.Compare(option, ".@L", true) == 0 ||
                            string.Compare(option, ".@LOW", true) == 0)
                {
                    tmpLabel = target.Substring(0, optionIndex);
                    macroValue = MacroValueEnum.Low;
                }
                else if (string.Compare(option, ".@T", true) == 0 ||
                            string.Compare(option, ".@TEXT", true) == 0)
                {
                    tmpLabel = target.Substring(0, optionIndex);
                    hasValue = false;
                    macroValue = MacroValueEnum.Text;
                }
            }

            var label = asmLoad.FindLabel(tmpLabel, hasValue);
            if (label == default)
            {
                throw new Exception($"未定義のラベルが指定されています。{target}");
            }
            var value = (int)label.Value;

            switch (macroValue)
            {
                case MacroValueEnum.High:
                    return value / 256;
                case MacroValueEnum.Low:
                    return value % 256;
                case MacroValueEnum.Text:
                    return label.ValueString;
                default:
                    return value;
            }
        }

        /// <summary>
        /// 指定の型に変換する
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        private static T CalculationNormalization<T>(object value)
            where T : struct
        {
            if (value is int intValue)
            {
                if (typeof(T) == typeof(int))
                {
                    return (T)(object)intValue;
                }
                else if (typeof(T) == typeof(UInt32))
                {
                    if (intValue < 0)
                    {
                        return (T)(object)Convert.ToUInt32(UInt32.MaxValue + intValue + 1);
                    }
                    else
                    {
                        return (T)(object)Convert.ToUInt32(intValue & UInt32.MaxValue);
                    }
                }
                else if (typeof(T) == typeof(UInt16))
                {
                    if (intValue < 0)
                    {
                        return (T)(object)Convert.ToUInt16(UInt16.MaxValue + intValue + 1);
                    }
                    else
                    {
                        return (T)(object)Convert.ToUInt16(intValue & UInt16.MaxValue);
                    }
                }
                else if (typeof(T) == typeof(byte))
                {
                    if (intValue < 0)
                    {
                        return (T)(object)Convert.ToByte(byte.MaxValue + intValue + 1);
                    }
                    else
                    {
                        return (T)(object)Convert.ToByte(intValue & byte.MaxValue);
                    }
                }
                else
                {
                    throw new ArgumentException(nameof(intValue));
                }
            }
            else if (typeof(T) == typeof(bool) && value is bool boolValue)
            {
                return (T)(object)boolValue;
            }
            else
            {
                throw new ArgumentException(nameof(value));
            }
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
        private static bool InternalTryParse<T>(string value, AsmLoad asmLoad, AsmAddress? asmAddress, out T resultValue)
            where T : struct
        {
            try
            {
                resultValue = Calculation<T>(value, asmLoad, asmAddress);
                return true;
            }
            catch
            {
                resultValue = default(T);
                return false;
            }
        }

        /// <summary>
        /// コンバートを行う
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="asmLoad"></param>
        /// <param name="asmAddress"></param>
        /// <returns></returns>
        private static T InternalConvertTo<T>(string value, AsmLoad asmLoad, AsmAddress? asmAddress)
            where T : struct
        {
            if (InternalTryParse<T>(value, asmLoad, asmAddress, out var resultValue))
            {
                return resultValue;
            }
            else
            {
                throw new ErrorAssembleException(Error.ErrorCodeEnum.E0004, $"演算対象：{value}");
            }
        }
    }
}
