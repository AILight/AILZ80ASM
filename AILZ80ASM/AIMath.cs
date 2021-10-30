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
        }

        private static readonly string RegexPatternErrorHexadecimal = @"(?<start>([\s|,]+)|(^))(?<value>(H[0-9A-Fa-f]+H))(?<end>(\s+)|($))";
        private static readonly string RegexPatternHexadecimal = @"(?<start>([\s|,]+)|(^))(?<value>([0-9A-Fa-f]+H))(?<end>(\s+)|($))";
        private static readonly string RegexPatternErrorDollarHexadecimal = @"(?<start>\s?)(?<value>(\$[0-9A-Fa-f]+\$))(?<end>\s?)";
        private static readonly string RegexPatternDollarHexadecimal = @"(?<start>\s?)(?<value>(\$[0-9A-Fa-f]+))(?<end>\s?)";
        private static readonly string RegexPatternErrorBinaryNumber = @"(?<start>\s?)(?<value>(%[01]+%))(?<end>\s?)";
        private static readonly string RegexPatternBinaryNumber = @"(?<start>\s?)(?<value>(^%[01_]+)|(^[01_]+B))(?<end>\s?)";
        private static readonly string RegexPatternLabel = @"(?<start>\s?)(?<value>([\w\.:@]+))(?<end>\s?)";
        private static readonly string RegexPatternDigit = @"^(\+|\-|)(\d+)$";
        private static readonly string RegexPatternFormuraAndDigit = @"^(\d+|\+|\-|\*|\/|\%|\~|\(|\)|!=|!|==|\<\<|\>\>|<=|\<|>=|\>|\&\&|\|\||\&|\||\?|\:)";
        private static readonly string RegexPatternFormuraChar = @"^(\+|\-|\*|\/|\%|\~|\(|\)|!=|!|==|\<\<|\>\>|<=|\<|>=|\>|\&\&|\|\||\&|\||\?|\:)";
        private static readonly Dictionary<string, int> FormuraPriority = new Dictionary<string, int>()
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

        private static readonly string RegexPatternFormuraChar1 = @"\d|\+|\-|\*|\/|\%|\~|\(|\)|!|=|\<|\>|\s|\&|\|";
        private static readonly string[] InvalidFormuras = new[] { "<>", "><", "===", "=>", "<=", ")(",
                                                                  "**", "*+", "*-", "*%",
                                                                  "+*", "++", "+-", "+%",
                                                                  "-*", "-+", "--", "-%",
                                                                  "%*", "%+", "%-", "%%",
        };

        public static bool IsNumber(string value)
        {
            if (Regex.Match(value, RegexPatternHexadecimal, RegexOptions.Singleline | RegexOptions.IgnoreCase).Success)
            {
                return true;
            }

            if (Regex.Match(value, RegexPatternDollarHexadecimal, RegexOptions.Singleline | RegexOptions.IgnoreCase).Success)
            {
                return true;
            }

            if (Regex.Match(value, RegexPatternBinaryNumber, RegexOptions.Singleline | RegexOptions.IgnoreCase).Success)
            {
                return true;
            }

            if (decimal.TryParse(value, out var dammy))
            {
                return true;
            }

            return false;
        }

        /*
        public static byte ConvertToByte(string value, LineDetailExpansionItemOperation lineDetailExpansionItemOperation, AsmLoad asmLoad)
        {
            return ConvertToByte(value, lineDetailExpansionItemOperation.Label.GlobalLabelName, lineDetailExpansionItemOperation.Label.LabelName, lineDetailExpansionItemOperation.Address, asmLoad);
        }

        public static byte ConvertToByte(string value, string globalLabelName, string labelName, AsmAddress address, AsmLoad asmLoad)
        {
            var tmpValue = ReplaceAll(value, globalLabelName, labelName, address, asmLoad);

            try
            {
                var nCalcExpression = new NCalc.Expression(tmpValue);
                var calcedValue = nCalcExpression.ToLambda<int>().Invoke();

                if (calcedValue < 0)
                {
                    return (byte)(byte.MaxValue + calcedValue + 1);
                }
                else
                {
                    return (byte)calcedValue;
                }
            }
            catch (Exception ex)
            {
                throw new ErrorAssembleException(Error.ErrorCodeEnum.E0004, ex, $"演算対象：{value}");
            }

        }
        */

        /// <summary>
        /// 使えない演算子をチェック
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        private static bool IsInvalidFormulaChar(string target)
        {
            if (InvalidFormuras.Any(m => target.Contains(m)))
            {
                return true;
            }

            return !string.IsNullOrEmpty(Regex.Replace(target.Replace(" ", ""), RegexPatternFormuraChar1, "",
                                        RegexOptions.Singleline | RegexOptions.IgnoreCase));
        }

        /// <summary>
        /// int方を指定の型に変換する
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        private static T InternalConvertNormalization<T>(int value)
            where T : struct
        {
            if (typeof(T) == typeof(UInt32))
            {
                if (value < 0)
                {
                    return (T)(object)Convert.ToUInt32(UInt32.MaxValue + value + 1);
                }
                else
                {
                    return (T)(object)Convert.ToUInt32(value & UInt32.MaxValue);
                }
            }
            else if (typeof(T) == typeof(UInt16))
            {
                if (value < 0)
                {
                    return (T)(object)Convert.ToUInt16(UInt16.MaxValue + value + 1);
                }
                else
                {
                    return (T)(object)Convert.ToUInt16(value & UInt16.MaxValue);
                }
            }
            else if (typeof(T) == typeof(byte))
            {
                if (value < 0)
                {
                    return (T)(object)Convert.ToByte(byte.MaxValue + value + 1);
                }
                else
                {
                    return (T)(object)Convert.ToByte(value & byte.MaxValue);
                }
            }
            else
            {
                throw new ArgumentException(nameof(value));
            }
        }

        /// <summary>
        /// 文字列を計算する
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        private static T InternalCalculation<T>(string value)
            where T : struct
        {
            if (typeof(T) == typeof(bool))
            {
                var nCalcExpression = new NCalc.Expression(value);
                var calcedValue = nCalcExpression.ToLambda<T>().Invoke();

                return calcedValue;
            }
            else if (typeof(T) == typeof(UInt32) ||
                     typeof(T) == typeof(UInt16) ||
                     typeof(T) == typeof(byte))
            {
                var nCalcExpression = new NCalc.Expression(value);
                var calcedValue = nCalcExpression.ToLambda<int>().Invoke();
                var normaledValue = InternalConvertNormalization<T>(calcedValue);

                return normaledValue;
            }
            else
            {
                throw new ArgumentException(nameof(T));
            }
        }

        /// <summary>
        /// 演算可能かを判断し、可能なら演算をする
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="globalLabelName"></param>
        /// <param name="labelName"></param>
        /// <param name="asmLoad"></param>
        /// <param name="resultValue"></param>
        /// <returns></returns>
        public static bool InternalTryParse<T>(string value, string globalLabelName, string labelName, AsmLoad asmLoad, AsmAddress? asmAddress, out T resultValue)
            where T : struct
        {
            var tmpValue = default(string);

            if (asmAddress.HasValue)
            {
                tmpValue = ReplaceAll(value, globalLabelName, labelName, asmLoad, asmAddress.Value);
            }
            else
            {
                tmpValue = ReplaceAll(value, globalLabelName, labelName, asmLoad);
            }

            // 計算不能かを判断
            if (IsInvalidFormulaChar(tmpValue))
            {
                resultValue = default(T);
                return false;
            }

            try
            {
                resultValue = InternalCalculation<T>(tmpValue);
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
        /// <param name="globalLabelName"></param>
        /// <param name="labelName"></param>
        /// <param name="asmLoad"></param>
        /// <param name="asmAddress"></param>
        /// <returns></returns>
        private static T InternalConvertTo<T>(string value, string globalLabelName, string labelName, AsmLoad asmLoad, AsmAddress? asmAddress)
            where T : struct
        {
            if (InternalTryParse<T>(value, globalLabelName, labelName, asmLoad, asmAddress, out var resultValue))
            {
                return resultValue;
            }
            else
            {
                throw new ErrorAssembleException(Error.ErrorCodeEnum.E0004, $"演算対象：{value}");
            }
        }


        public static bool TryParse<T>(string value, string globalLabelName, string labelName, AsmLoad asmLoad, AsmAddress asmAddress, out T resultValue)
            where T : struct
        {
            return InternalTryParse(value, globalLabelName, labelName, asmLoad, asmAddress, out resultValue);
        }

        public static bool TryParse<T>(string value, LineDetailExpansionItemOperation lineDetailExpansionItemOperation, AsmLoad asmLoad, AsmAddress asmAddress, out T resultValue)
            where T : struct
        {
            return TryParse<T>(value, lineDetailExpansionItemOperation.Label.GlobalLabelName, lineDetailExpansionItemOperation.Label.LabelName, asmLoad, asmAddress, out resultValue);
        }

        public static bool TryParse<T>(string value, string globalLabelName, string labelName, AsmLoad asmLoad, out T resultValue)
            where T : struct
        {
            return InternalTryParse(value, globalLabelName, labelName, asmLoad, default(AsmAddress?), out resultValue);
        }

        public static bool TryParse<T>(string value, LineDetailExpansionItemOperation lineDetailExpansionItemOperation, AsmLoad asmLoad, out T resultValue)
            where T : struct
        {
            return TryParse<T>(value, lineDetailExpansionItemOperation.Label.GlobalLabelName, lineDetailExpansionItemOperation.Label.LabelName, asmLoad, out resultValue);
        }

        public static bool TryParse<T>(string value, AsmLoad asmLoad, out T resultValue)
            where T : struct
        {
            return TryParse<T>(value, asmLoad.GlobalLabelName, asmLoad.LabelName, asmLoad, out resultValue);
        }

        public static T ConvertTo<T>(string value, AsmLoad asmLoad)
            where T : struct
        {
            return ConvertTo<T>(value, asmLoad.GlobalLabelName, asmLoad.LabelName, asmLoad);
        }

        public static T ConvertTo<T>(string value, LineDetailExpansionItemOperation lineDetailExpansionItemOperation, AsmLoad asmLoad)
            where T : struct
        {
            return ConvertTo<T>(value, lineDetailExpansionItemOperation.Label.GlobalLabelName, lineDetailExpansionItemOperation.Label.LabelName, asmLoad, lineDetailExpansionItemOperation.Address);
        }

        public static T ConvertTo<T>(string value, string globalLabelName, string labelName, AsmLoad asmLoad)
            where T : struct
        {
            return InternalConvertTo<T>(value, globalLabelName, labelName, asmLoad, default(AsmAddress?));
        }

        public static T ConvertTo<T>(string value, string globalLabelName, string labelName, AsmLoad asmLoad, AsmAddress asmAddress)
            where T : struct
        {
            return InternalConvertTo<T>(value, globalLabelName, labelName, asmLoad, asmAddress);
        }

        private static string ReplaceAll(string value, string globalLabelName, string labelName, AsmLoad asmLoad)
        {
            //16進数の置き換え
            value = Replace16Number(value);

            //2進数の置き換え
            value = ReplaceBinaryNumber(value);

            // ラベルの置き換え
            value = ReplaceLabel(value, globalLabelName, labelName, asmLoad);

            return value;
        }

        private static string ReplaceAll(string value, string globalLabelName, string labelName, AsmLoad asmLoad, AsmAddress address)
        {
            //16進数の置き換え
            value = Replace16NumberAndCurrentAddress(value, address);

            //2進数の置き換え
            value = ReplaceBinaryNumber(value);

            // ラベルの置き換え
            value = ReplaceLabel(value, globalLabelName, labelName, asmLoad);

            return value;
        }

        /// <summary>
        /// ラベル判別
        /// </summary>
        /// <param name="value"></param>
        /// <param name="globalLabelName"></param>
        /// <param name="labelName"></param>
        /// <param name="labels"></param>
        private static string ReplaceLabel(string value, string globalLabelName, string labelName, AsmLoad asmLoad)
        {
            var resultValue = "";
            var workValue = value;
            var limitCounter = 0;

            var regexResult = default(Match);
            while ((regexResult = Regex.Match(workValue, RegexPatternLabel, RegexOptions.Singleline | RegexOptions.IgnoreCase)).Success && limitCounter < 10000)
            {
                var macroValue = MacroValueEnum.None;

                var matchResultString = regexResult.Groups["value"].Value;
                var index = workValue.IndexOf(matchResultString);
                if (matchResultString.EndsWith(".@H", StringComparison.OrdinalIgnoreCase))
                {
                    macroValue = MacroValueEnum.High;
                    matchResultString = matchResultString.Substring(0, matchResultString.Length - 3);
                }
                else if (matchResultString.EndsWith(".@L", StringComparison.OrdinalIgnoreCase))
                {
                    macroValue = MacroValueEnum.Low;
                    matchResultString = matchResultString.Substring(0, matchResultString.Length - 3);
                }

                // ラベルチェック
                var labels = asmLoad.AllLabels.Where(m => m.HasValue).ToArray();
                var longLabelName = Label.GetLongLabelName(matchResultString, asmLoad);
                var label = labels.Where(m => m.HasValue && string.Compare(m.LongLabelName, longLabelName, true) == 0).FirstOrDefault();

                var valueString = "";
                switch (macroValue)
                {
                    case MacroValueEnum.High:
                        matchResultString += ".@H";
                        if (label == default)
                        {
                            valueString = matchResultString;
                        }
                        else
                        {
                            valueString = ((int)(label.Value / 256)).ToString("0");
                        }
                        break;
                    case MacroValueEnum.Low:
                        matchResultString += ".@L";
                        if (label == default)
                        {
                            valueString = matchResultString;
                        }
                        else
                        {
                            valueString = ((int)(label.Value % 256)).ToString("0");
                        }
                        break;
                    default:
                        valueString = label?.Value.ToString("0") ?? matchResultString;
                        break;
                }

                resultValue += workValue.Substring(0, index);
                resultValue += valueString;
                workValue = workValue.Substring(index + matchResultString.Length);

                regexResult = Regex.Match(workValue, RegexPatternLabel, RegexOptions.Singleline | RegexOptions.IgnoreCase);
                limitCounter++;
            }
            resultValue += workValue;

            return resultValue;
        }


        /// <summary>
        /// 16進数の変換
        /// </summary>
        /// <param name="value"></param>
        /// <param name="address"></param>
        /// <returns></returns>
        public static string Replace16Number(string value)
        {
            value = ReplaceHexadecimal(value);
            value = ReplaceDollarHexadecimal(value);

            return value;
        }

        /// <summary>
        /// 16進数の変換
        /// </summary>
        /// <param name="value"></param>
        /// <param name="address"></param>
        /// <returns></returns>
        public static string Replace16NumberAndCurrentAddress(string value, AsmAddress address)
        {
            value = Replace16Number(value);

            // $の値を現在のアドレスに置き換える
            value = value.Replace("$", $"{address.Program:0}");

            return value;
        }

        /// <summary>
        /// 16進数の変換(H)
        /// </summary>
        /// <param name="value"></param>
        /// <param name="globalLabelName"></param>
        /// <param name="labelName"></param>
        /// <param name="labels"></param>
        private static string ReplaceHexadecimal(string value)
        {
            var resultValue = "";
            var workValue = value;
            var limitCounter = 0;

            if (Regex.Match(workValue, RegexPatternErrorHexadecimal, RegexOptions.Singleline | RegexOptions.IgnoreCase).Success)
            {
                throw new ErrorAssembleException(Error.ErrorCodeEnum.E0005, $"対象：{value}");
            }

            var regexResult = default(Match);
            while ((regexResult = Regex.Match(workValue, RegexPatternHexadecimal, RegexOptions.Singleline | RegexOptions.IgnoreCase)).Success && limitCounter < 10000)
            {
                var matchResultString = regexResult.Groups["value"].Value;
                var index = workValue.IndexOf(matchResultString, StringComparison.OrdinalIgnoreCase);

                resultValue += workValue.Substring(0, index);
                try
                {
                    resultValue += Convert.ToInt32(matchResultString.Substring(0, matchResultString.Length - 1), 16).ToString("0");
                }
                catch
                {
                    throw new ErrorAssembleException(Error.ErrorCodeEnum.E0005, $"対象：{value}");
                }
                workValue = workValue.Substring(index + matchResultString.Length);

                limitCounter++;
            }
            resultValue += workValue;

            return resultValue;
        }

        /// <summary>
        /// 16進数の変換($)
        /// </summary>
        /// <param name="value"></param>
        /// <param name="globalLabelName"></param>
        /// <param name="labelName"></param>
        /// <param name="labels"></param>
        private static string ReplaceDollarHexadecimal(string value)
        {
            var resultValue = "";
            var workValue = value;
            var limitCounter = 0;

            if (Regex.Match(workValue, RegexPatternErrorDollarHexadecimal, RegexOptions.Singleline | RegexOptions.IgnoreCase).Success)
            {
                throw new ErrorAssembleException(Error.ErrorCodeEnum.E0005, $"対象：{value}");
            }

            var regexResult = default(Match);
            while ((regexResult = Regex.Match(workValue, RegexPatternDollarHexadecimal, RegexOptions.Singleline | RegexOptions.IgnoreCase)).Success && limitCounter < 10000)
            {
                var matchResultString = regexResult.Groups["value"].Value;
                var index = workValue.IndexOf(matchResultString, StringComparison.OrdinalIgnoreCase);

                resultValue += workValue.Substring(0, index);
                try
                {
                    resultValue += Convert.ToInt32(matchResultString.Substring(1), 16).ToString("0");
                }
                catch
                {
                    throw new ErrorAssembleException(Error.ErrorCodeEnum.E0005, $"対象：{value}");
                }
                workValue = workValue.Substring(index + matchResultString.Length);

                limitCounter++;
            }
            resultValue += workValue;

            return resultValue;
        }


        /// <summary>
        /// 2進数の変換
        /// </summary>
        /// <param name="value"></param>
        /// <param name="globalLabelName"></param>
        /// <param name="labelName"></param>
        /// <param name="labels"></param>
        public static string ReplaceBinaryNumber(string value)
        {
            var resultValue = "";
            var workValue = value;
            var limitCounter = 0;

            if (Regex.Match(workValue, RegexPatternErrorBinaryNumber, RegexOptions.Singleline | RegexOptions.IgnoreCase).Success)
            {
                throw new ErrorAssembleException(Error.ErrorCodeEnum.E0008, $"対象：{value}");
            }

            var regexResult = default(Match);
            while ((regexResult = Regex.Match(workValue, RegexPatternBinaryNumber, RegexOptions.Singleline | RegexOptions.IgnoreCase)).Success && limitCounter < 10000)
            {
                var matchResultString = regexResult.Groups["value"].Value;
                var index = workValue.IndexOf(matchResultString, StringComparison.OrdinalIgnoreCase);

                resultValue += workValue.Substring(0, index);
                try
                {
                    var target = matchResultString;
                    foreach (var item in "_%Bb".ToArray())
                    {
                        target = target.Replace(item.ToString(), "");
                    }

                    resultValue += Convert.ToInt32(target, 2).ToString("0");
                }
                catch
                {
                    throw new ErrorAssembleException(Error.ErrorCodeEnum.E0008, $"対象：{value}");
                }
                workValue = workValue.Substring(index + matchResultString.Length);

                limitCounter++;
            }
            resultValue += workValue;

            return resultValue;
        }

        public static int Calc<T>(string value)
        {
            var terms = CalcParse(value);
            var rpns = CalcReversePolish(terms);
            var calc = Calculation(rpns);

            return calc;
        }

        private static int Calculation(string[] rpns)
        {
            var stack = new Stack<int>();

            foreach (var item in rpns)
            {
                if (Regex.Match(item, RegexPatternDigit, RegexOptions.Singleline | RegexOptions.IgnoreCase).Success)
                {
                    if (int.TryParse(item, out var result))
                    {
                        stack.Push(result);
                    }
                    else
                    {
                        throw new Exception("数値に変換できませんでした。");
                    }
                }
                else
                {
                    if (stack.Count < 2)
                    {
                        throw new Exception("演算に失敗しました。");
                    }

                    var lastValue = stack.Pop();
                    var firstValue = stack.Pop();

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
                            stack.Push(firstValue > lastValue ? -1 : 0);
                            break;
                        case ">=":
                            stack.Push(firstValue >= lastValue ? -1 : 0);
                            break;
                        case "<":
                            stack.Push(firstValue < lastValue ? -1 : 0);
                            break;
                        case "<=":
                            stack.Push(firstValue <= lastValue ? -1 : 0);
                            break;
                        case "==":
                            stack.Push(firstValue == lastValue ? -1 : 0);
                            break;
                        case "!=":
                            stack.Push(firstValue != lastValue ? -1 : 0);
                            break;
                        case "&":
                            stack.Push(firstValue & lastValue);
                            break;
                        case "^":
                            stack.Push(firstValue ^ lastValue);
                            break;
                        case "|":
                            stack.Push(firstValue ^ lastValue);
                            break;
                            /*
                        case "&&":
                            stack.Push(firstValue && lastValue);
                            break;
                        case "||":
                            stack.Push(firstValue || lastValue);
                            break;
                            */
                        default:
                            throw new Exception("無効な演算子です。");
                    }
                }
            }

            return stack.Pop();
        }

        private static string[] CalcParse(string value)
        {
            var terms = new List<string>();
            var tmpValue = value.Trim();

            // 数値と演算子に分解する
            while (!string.IsNullOrEmpty(tmpValue))
            {
                var matched = Regex.Match(tmpValue, RegexPatternFormuraAndDigit, RegexOptions.Singleline | RegexOptions.IgnoreCase);
                if (matched.Success)
                {
                    terms.Add(matched.Value);
                    tmpValue = tmpValue.Substring(matched.Length).TrimStart();
                }
                else
                {
                    throw new Exception("演算に使えない文字が検出されました。");
                }
            }

            var result = new List<string>();
            var sign = "";
            // 単項演算子を結合する
            foreach (var index in Enumerable.Range(0, terms.Count))
            {
                var tmpString = terms[index];
                if (tmpString == "+" || tmpString == "-")
                {
                    if (index == 0 || Regex.Match(terms[index - 1], RegexPatternFormuraChar, RegexOptions.Singleline | RegexOptions.IgnoreCase).Success)
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
            foreach (var index in Enumerable.Range(0, checkValues.Length - 1))
            {
                if (Regex.Match(checkValues[index + 0], RegexPatternDigit,RegexOptions.Singleline | RegexOptions.IgnoreCase).Success &&
                    Regex.Match(checkValues[index + 1], RegexPatternDigit, RegexOptions.Singleline | RegexOptions.IgnoreCase).Success)
                {
                    throw new Exception("数値と数値の間には演算子が必要です");
                }

                if (Regex.Match(checkValues[index + 0], RegexPatternFormuraChar, RegexOptions.Singleline | RegexOptions.IgnoreCase).Success &&
                    Regex.Match(checkValues[index + 1], RegexPatternFormuraChar, RegexOptions.Singleline | RegexOptions.IgnoreCase).Success)
                {
                    throw new Exception("演算子が連続で指定されています。");
                }

            }

            return result.ToArray();
        }

        public static string[] CalcReversePolish(string[] terms)
        {
            var result = new List<string>();
            var formura = new Stack<string>();

            foreach (var item in terms)
            {
                var matched = Regex.Match(item, RegexPatternDigit, RegexOptions.Singleline | RegexOptions.IgnoreCase);
                if (matched.Success)
                {
                    result.Add(item);
                }
                else
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
                        else if (formura.Count == 0 || item == "(" || FormuraPriority[formura.Peek()] > FormuraPriority[item])
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
            }

            result.AddRange(formura);
            if (result.Any(m => m == "("))
            {
                throw new Exception("括弧の数が不一致です");
            }

            return result.ToArray();
        }
    }
}
