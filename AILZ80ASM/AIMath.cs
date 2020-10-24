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
        public static UInt16 ConvertToUInt16(string value, string globalLabelName, string lableName, UInt16 address, Lable[] lables)
        {
            // $の値を調整
            var result = default(Match);
            var limitCounter = 0;
            while ((result = Regex.Match(value, @"(?<value>(\$[\da-fA-F]+))")).Success && limitCounter < 10000)
            {
                var resultValue = Convert.ToUInt16(result.Value.Replace("$", ""));
                value = Regex.Replace(value, $@"(?<start>[^\da-fA-F]?)(\{result.Value})(?<end>[^\da-fA-F]?)", @"${start}" + $"{resultValue:0}" + "${end}");
                limitCounter++;
            }

            // $の値を現在のアドレスに置き換える
            value = value.Replace("$", $"{address:0}");

            // GlobalLabel込でリプレース
            foreach (var label in lables.Where(m => m.DataType != Lable.DataTypeEnum.None && m.DataType != Lable.DataTypeEnum.Processing))
            {
                value = value.Replace(label.LongLabelName, $"{label.Value:0}");
            }

            // 同一のGlobalLabel込みでリプレース
            foreach (var label in lables.Where(m => m.DataType != Lable.DataTypeEnum.None && m.DataType != Lable.DataTypeEnum.Processing && m.GlobalLabelName == globalLabelName))
            {
                value = value.Replace(label.MiddleLabelName, $"{label.Value:0}");
            }

            // 同一のGlobalLabel、LabelName込みでリプレース
            foreach (var label in lables.Where(m => m.DataType != Lable.DataTypeEnum.None && m.DataType != Lable.DataTypeEnum.Processing && m.GlobalLabelName == globalLabelName && m.LabelName == lableName))
            {
                value = value.Replace(label.ShortLabelName, $"{label.Value:0}");
            }

            // $10を置き換える

            string value1 = new DataTable().Compute("123", null).ToString();

            return 0;
        }
    }
}
