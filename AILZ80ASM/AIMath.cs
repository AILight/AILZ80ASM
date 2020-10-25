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
        private static readonly string RegexPatternLabel = @"(?<start>\s?)(?<value>([\w\.]+))(?<end>\s?)";

        public static UInt16 ConvertToUInt16(string value, string globalLabelName, string lableName, UInt16 address, Lable[] labels)
        {
            // $の値を調整
            var regexResult = default(Match);
            var limitCounter = 0;
            while ((regexResult = Regex.Match(value, @"(?<value>(\$[\da-fA-F]+))")).Success && limitCounter < 10000)
            {
                var resultValue = Convert.ToUInt16(regexResult.Value.Replace("$", ""), 16);
                value = Regex.Replace(value, $@"(?<start>[^\da-fA-F]?)(\{regexResult.Value})(?<end>[^\da-fA-F]?)", @"${start}" + $"{resultValue:0}" + "${end}");
                limitCounter++;
            }

            // $の値を現在のアドレスに置き換える
            value = value.Replace("$", $"{address:0}");

            // ラベルの置き換え
            value = ReplaceLabel(value, globalLabelName, lableName, labels);

            var calcedString = new DataTable().Compute(value, null).ToString();

            return Convert.ToUInt16(calcedString);
        }

        /// <summary>
        /// ラベル判別
        /// </summary>
        /// <param name="value"></param>
        /// <param name="globalLabelName"></param>
        /// <param name="lableName"></param>
        /// <param name="lables"></param>
        private static string ReplaceLabel(string value, string globalLabelName, string lableName, Lable[] lables)
        {
            var resultValue = "";
            var workValue = value;
            var limitCounter = 0;

            var regexResult = default(Match);
            while ((regexResult = Regex.Match(workValue, RegexPatternLabel)).Success && limitCounter < 10000)
            {
                var matchResultString = regexResult.Groups["value"].Value;
                var index = workValue.IndexOf(matchResultString);

                // ラベルチェック
                var label = default(Lable);

                label = label ?? lables.Where(m => m.HasValue && m.LongLabelName == matchResultString).FirstOrDefault();
                label = label ?? lables.Where(m => m.HasValue && m.MiddleLabelName == matchResultString).FirstOrDefault();
                label = label ?? lables.Where(m => m.HasValue && m.ShortLabelName == matchResultString).FirstOrDefault();

                resultValue += workValue.Substring(0, index);
                resultValue += label?.Value.ToString("0") ?? matchResultString;
                workValue = workValue.Substring(index + matchResultString.Length);

                regexResult = Regex.Match(workValue, RegexPatternLabel);
                limitCounter++;
            }
            resultValue += workValue;

            return resultValue;
        }
    }
}
