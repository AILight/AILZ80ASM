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
        private static readonly string RegexPatternLabel = @"(?<start>\s?)(?<value>([\w\.:@]+))(?<end>\s?)";

        public static bool IsNumber(string value)
        {
            if (Regex.Match(value, RegexPatternHexadecimal).Success)
            {
                return true;
            }

            if (Regex.Match(value, RegexPatternDollarHexadecimal).Success)
            {
                return true;
            }

            if (Regex.Match(value, RegexPatternBinaryNumber).Success)
            {
                return true;
            }

            if (decimal.TryParse(value, out var dammy))
            {
                return true;
            }

            return false;
        }

        public static byte ConvertToByte(string value, LineDetailExpansionItemOperation lineDetailExpansionItemOperation, AsmLoad asmLoad)
        {
            return ConvertToByte(value, lineDetailExpansionItemOperation.Label.GlobalLabelName, lineDetailExpansionItemOperation.Label.LabelName, lineDetailExpansionItemOperation.Address, asmLoad);
        }

        public static byte ConvertToByte(string value, string globalLabelName, string lableName, AsmAddress address, AsmLoad asmLoad)
        {
            var tmpValue = ReplaceAll(value, globalLabelName, lableName, address, asmLoad);

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

        public static UInt16 ConvertToUInt16(string value, LineDetailExpansionItemOperation lineDetailExpansionItemOperation, AsmLoad asmLoad)
        {
            return ConvertToUInt16(value, lineDetailExpansionItemOperation.Label.GlobalLabelName, lineDetailExpansionItemOperation.Label.LabelName, lineDetailExpansionItemOperation.Address, asmLoad);
        }

        public static UInt16 ConvertToUInt16(string value, AsmLoad asmLoad)
        {
            var tmpValue = ReplaceAll(value, asmLoad.GlobalLableName, asmLoad.LabelName, asmLoad);
            return InternalConvertToUInt16(value, tmpValue);
        }

        public static UInt16 ConvertToUInt16(string value, string globalLabelName, string lableName, AsmLoad asmLoad)
        {
            var tmpValue = ReplaceAll(value, globalLabelName, lableName, asmLoad);
            return InternalConvertToUInt16(value, tmpValue);
        }

        public static UInt16 ConvertToUInt16(string value, string globalLabelName, string lableName, AsmAddress address, AsmLoad asmLoad)
        {
            var tmpValue = ReplaceAll(value, globalLabelName, lableName, address, asmLoad);
            return InternalConvertToUInt16(value, tmpValue);
        }

        public static UInt32 ConvertToUInt32(string value, LineDetailExpansionItemOperation lineDetailExpansionItemOperation, AsmLoad asmLoad)
        {
            return ConvertToUInt32(value, lineDetailExpansionItemOperation.Label.GlobalLabelName, lineDetailExpansionItemOperation.Label.LabelName, lineDetailExpansionItemOperation.Address, asmLoad);
        }

        public static UInt32 ConvertToUInt32(string value, AsmLoad asmLoad)
        {
            var tmpValue = ReplaceAll(value, asmLoad.GlobalLableName, asmLoad.LabelName, asmLoad);
            return InternalConvertToUInt32(value, tmpValue);
        }

        public static UInt32 ConvertToUInt32(string value, string globalLabelName, string lableName, AsmLoad asmLoad)
        {
            var tmpValue = ReplaceAll(value, globalLabelName, lableName, asmLoad);
            return InternalConvertToUInt32(value, tmpValue);
        }

        public static UInt32 ConvertToUInt32(string value, string globalLabelName, string lableName, AsmAddress address, AsmLoad asmLoad)
        {
            var tmpValue = ReplaceAll(value, globalLabelName, lableName, address, asmLoad);
            return InternalConvertToUInt32(value, tmpValue);
        }

        public static bool ConvertToBoolean(string value, LineDetailExpansionItemOperation lineDetailExpansionItemOperation, AsmLoad asmLoad)
        {
            return ConvertToBoolean(value, lineDetailExpansionItemOperation.Label.GlobalLabelName, lineDetailExpansionItemOperation.Label.LabelName, lineDetailExpansionItemOperation.Address, asmLoad);
        }

        public static bool ConvertToBoolean(string value, AsmLoad asmLoad)
        {
            var tmpValue = ReplaceAll(value, asmLoad.GlobalLableName, asmLoad.LabelName, asmLoad);
            return InternalConvertToBoolean(value, tmpValue);
        }

        public static bool ConvertToBoolean(string value, string globalLabelName, string lableName, AsmLoad asmLoad)
        {
            var tmpValue = ReplaceAll(value, globalLabelName, lableName, asmLoad);
            return InternalConvertToBoolean(value, tmpValue);
        }

        public static bool ConvertToBoolean(string value, string globalLabelName, string lableName, AsmAddress address, AsmLoad asmLoad)
        {
            var tmpValue = ReplaceAll(value, globalLabelName, lableName, address, asmLoad);
            return InternalConvertToBoolean(value, tmpValue);
        }

        private static UInt16 InternalConvertToUInt16(string value, string tmpValue)
        {
            return (UInt16)InternalConvertToUInt32(value, tmpValue);
        }

        private static UInt32 InternalConvertToUInt32(string value, string tmpValue)
        {
            try
            {
                var nCalcExpression = new NCalc.Expression(tmpValue);
                var calcedValue = nCalcExpression.ToLambda<int>().Invoke();

                if (calcedValue < 0)
                {
                    return (UInt32)(UInt32.MaxValue + calcedValue + 1);
                }
                else
                {
                    return (UInt32)calcedValue;
                }
            }
            catch (Exception ex)
            {
                throw new ErrorAssembleException(Error.ErrorCodeEnum.E0004, ex, $"演算対象：{value}");
            }
        }

        private static bool InternalConvertToBoolean(string value, string tmpValue)
        {
            try
            {
                var nCalcExpression = new NCalc.Expression(tmpValue);
                var calcedValue = nCalcExpression.ToLambda<bool>().Invoke();

                return calcedValue;
            }
            catch (Exception ex)
            {
                throw new ErrorAssembleException(Error.ErrorCodeEnum.E0004, ex, $"演算対象：{value}");
            }
        }

        private static string ReplaceAll(string value, string globalLabelName, string lableName, AsmLoad asmLoad)
        {
            //16進数の置き換え
            value = Replace16Number(value);

            //2進数の置き換え
            value = ReplaceBinaryNumber(value);

            // ラベルの置き換え
            value = ReplaceLabel(value, globalLabelName, lableName, asmLoad);
            
            return value;
        }

        private static string ReplaceAll(string value, string globalLabelName, string lableName, AsmAddress address, AsmLoad asmLoad)
        {
            //16進数の置き換え
            value = Replace16NumberAndCurrentAddress(value, address);

            //2進数の置き換え
            value = ReplaceBinaryNumber(value);

            // ラベルの置き換え
            value = ReplaceLabel(value, globalLabelName, lableName, asmLoad);

            return value;
        }

        /// <summary>
        /// ラベル判別
        /// </summary>
        /// <param name="value"></param>
        /// <param name="globalLabelName"></param>
        /// <param name="lableName"></param>
        /// <param name="lables"></param>
        private static string ReplaceLabel(string value, string globalLabelName, string lableName, AsmLoad asmLoad)
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
                var labels = asmLoad.AllLables.Where(m => m.HasValue).ToArray();
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
        /// <param name="lableName"></param>
        /// <param name="lables"></param>
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
        /// <param name="lableName"></param>
        /// <param name="lables"></param>
        public static string ReplaceBinaryNumber(string value)
        {
            var resultValue = "";
            var workValue = value;
            var limitCounter = 0;

            if (Regex.Match(workValue, RegexPatternErrorBinaryNumber).Success)
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
                    resultValue += Convert.ToInt32(matchResultString.ToUpper().Replace("_", "").Replace("%", "").Replace("B", ""), 2).ToString("0");
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
    }
}
