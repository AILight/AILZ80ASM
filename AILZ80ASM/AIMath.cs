using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Data;
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
        private static readonly string RegexPatternLabel = @"(?<start>\s?)(?<value>([\w\.@]+))(?<end>\s?)";

        public static byte ConvertToByte(string value, LineExpansionItem lineExpansionItem, Label[] labels)
        {
            return ConvertToByte(value, lineExpansionItem.Label.GlobalLabelName, lineExpansionItem.Label.LabelName, lineExpansionItem.Address, labels);
        }

        public static byte ConvertToByte(string value, string globalLabelName, string lableName, AsmAddress address, Label[] labels)
        {
            var tmpValue = ReplaceAll(value, globalLabelName, lableName, address, labels);

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
                throw new ErrorMessageException(Error.ErrorCodeEnum.E0004, $"演算対象：{value}", ex);
            }

        }

        public static UInt16 ConvertToUInt16(string value, LineExpansionItem lineExpansionItem, Label[] labels)
        {
            return ConvertToUInt16(value, lineExpansionItem.Label.GlobalLabelName, lineExpansionItem.Label.LabelName, lineExpansionItem.Address, labels);
        }

        public static UInt16 ConvertToUInt16(string value, string globalLabelName, string lableName, Label[] labels)
        {
            var tmpValue = ReplaceAll(value, globalLabelName, lableName, labels);
            return InternalConvertToUInt16(value, tmpValue);
        }

        public static UInt16 ConvertToUInt16(string value, string globalLabelName, string lableName, AsmAddress address, Label[] labels)
        {
            var tmpValue = ReplaceAll(value, globalLabelName, lableName, address, labels);
            return InternalConvertToUInt16(value, tmpValue);
        }

        public static UInt16 InternalConvertToUInt16(string value, string tmpValue)
        {
            try
            {
                var calcedValue = Convert.ToInt32(new DataTable().Compute(tmpValue, null).ToString());
                if (calcedValue < 0)
                {
                    return (UInt16)(UInt16.MaxValue + calcedValue + 1);
                }
                else
                {
                    return (UInt16)calcedValue;
                }
            }
            catch (Exception ex)
            {
                throw new ErrorMessageException(Error.ErrorCodeEnum.E0004, $"演算対象：{value}", ex);
            }
        }

        private static string ReplaceAll(string value, string globalLabelName, string lableName, Label[] labels)
        {
            //16進数の置き換え
            value = Replace16Number(value);

            //2進数の置き換え
            value = ReplaceBinaryNumber(value);

            // ラベルの置き換え
            value = ReplaceLabel(value, globalLabelName, lableName, labels);
            
            return value;
        }

        private static string ReplaceAll(string value, string globalLabelName, string lableName, AsmAddress address, Label[] labels)
        {
            //16進数の置き換え
            value = Replace16NumberAndCurrentAddress(value, address);

            //2進数の置き換え
            value = ReplaceBinaryNumber(value);

            // ラベルの置き換え
            value = ReplaceLabel(value, globalLabelName, lableName, labels);

            return value;
        }

        /// <summary>
        /// ラベル判別
        /// </summary>
        /// <param name="value"></param>
        /// <param name="globalLabelName"></param>
        /// <param name="lableName"></param>
        /// <param name="lables"></param>
        private static string ReplaceLabel(string value, string globalLabelName, string lableName, Label[] lables)
        {
            var resultValue = "";
            var workValue = value;
            var limitCounter = 0;

            var regexResult = default(Match);
            while ((regexResult = Regex.Match(workValue, RegexPatternLabel)).Success && limitCounter < 10000)
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
                var label = default(Label);

                label = label ?? lables.Where(m => m.HasValue && string.Compare(m.LongLabelName, matchResultString, true) == 0).FirstOrDefault();
                label = label ?? lables.Where(m => m.HasValue && string.Compare(m.MiddleLabelName, matchResultString, true) == 0).FirstOrDefault();
                label = label ?? lables.Where(m => m.HasValue && string.Compare(m.GlobalLabelName, globalLabelName, true) == 0 && string.Compare(m.LabelName, matchResultString, true) == 0).FirstOrDefault();
                label = label ?? lables.Where(m => m.HasValue && string.Compare(m.GlobalLabelName, globalLabelName, true) == 0 && string.Compare(m.LabelName, lableName, true) == 0 && string.Compare(m.ShortLabelName, matchResultString, true) == 0).FirstOrDefault();

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

                regexResult = Regex.Match(workValue, RegexPatternLabel);
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
        /// <param name="lableName"></param>
        /// <param name="lables"></param>
        private static string ReplaceHexadecimal(string value)
        {
            var resultValue = "";
            var workValue = value;
            var limitCounter = 0;

            if (Regex.Match(workValue, RegexPatternErrorHexadecimal).Success)
            {
                throw new ErrorMessageException(Error.ErrorCodeEnum.E0005, $"対象：{value}");
            }

            var regexResult = default(Match);
            while ((regexResult = Regex.Match(workValue, RegexPatternHexadecimal)).Success && limitCounter < 10000)
            {
                var matchResultString = regexResult.Groups["value"].Value;
                var index = workValue.IndexOf(matchResultString);

                resultValue += workValue.Substring(0, index);
                try
                {
                    resultValue += Convert.ToInt32(matchResultString.Substring(0, matchResultString.Length - 1), 16).ToString("0");
                }
                catch
                {
                    throw new ErrorMessageException(Error.ErrorCodeEnum.E0005, $"対象：{value}");
                }
                workValue = workValue.Substring(index + matchResultString.Length);

                regexResult = Regex.Match(workValue, RegexPatternHexadecimal);
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
        /// <param name="lableName"></param>
        /// <param name="lables"></param>
        private static string ReplaceDollarHexadecimal(string value)
        {
            var resultValue = "";
            var workValue = value;
            var limitCounter = 0;

            if (Regex.Match(workValue, RegexPatternErrorDollarHexadecimal).Success)
            {
                throw new ErrorMessageException(Error.ErrorCodeEnum.E0005, $"対象：{value}");
            }

            var regexResult = default(Match);
            while ((regexResult = Regex.Match(workValue, RegexPatternDollarHexadecimal)).Success && limitCounter < 10000)
            {
                var matchResultString = regexResult.Groups["value"].Value;
                var index = workValue.IndexOf(matchResultString);

                resultValue += workValue.Substring(0, index);
                try
                {
                    resultValue += Convert.ToInt32(matchResultString.Substring(1), 16).ToString("0");
                }
                catch
                {
                    throw new ErrorMessageException(Error.ErrorCodeEnum.E0005, $"対象：{value}");
                }
                workValue = workValue.Substring(index + matchResultString.Length);

                regexResult = Regex.Match(workValue, RegexPatternDollarHexadecimal);
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
        /// <param name="lableName"></param>
        /// <param name="lables"></param>
        public static string ReplaceBinaryNumber(string value)
        {
            var resultValue = "";
            var workValue = value;
            var limitCounter = 0;

            if (Regex.Match(workValue, RegexPatternErrorBinaryNumber).Success)
            {
                throw new ErrorMessageException(Error.ErrorCodeEnum.E0008, $"対象：{value}");
            }

            var regexResult = default(Match);
            while ((regexResult = Regex.Match(workValue, RegexPatternBinaryNumber, RegexOptions.IgnoreCase)).Success && limitCounter < 10000)
            {
                var matchResultString = regexResult.Groups["value"].Value;
                var index = workValue.IndexOf(matchResultString);

                resultValue += workValue.Substring(0, index);
                try
                {
                    resultValue += Convert.ToInt32(matchResultString.ToUpper().Replace("_", "").Replace("%", "").Replace("B", ""), 2).ToString("0");
                }
                catch
                {
                    throw new ErrorMessageException(Error.ErrorCodeEnum.E0008, $"対象：{value}");
                }
                workValue = workValue.Substring(index + matchResultString.Length);

                regexResult = Regex.Match(workValue, RegexPatternBinaryNumber);
                limitCounter++;
            }
            resultValue += workValue;

            return resultValue;
        }
    }
}
