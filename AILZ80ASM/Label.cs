using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AILZ80ASM
{
    public class Label
    {
        public enum DataTypeEnum
        {
            None,
            Invalidate,
            ProcessingForAddress,
            ProcessingForValue,
            ProcessingForArgument,
            Value,
            ADDR,
        }

        private static readonly string RegexPatternGlobalLabel = @"(?<lable>(^\w+))::";
        private static readonly string RegexPatternLabel = @"(?<lable>(^\w+)):";
        private static readonly string RegexPatternEquLabel = @"(?<lable>(^\w+)):?";
        private static readonly string RegexPatternSubLabel = @"(?<lable>(^\.\w+))";
        private static readonly string RegexPatternValueLable = @"(?<lable>(^\w+))\s+equ\s+(?<value>([\$\w]+))";
        private static readonly string RegexPatternArgumentLabel = @"(?<start>\s?)(?<value>([\w\.:@]+))(?<end>\s?)";

        public string GlobalLabelName { get; private set; }
        public string LabelName { get; private set; }
        public string SubLabelName { get; private set; }
        public string LongLabelName => DataType == DataTypeEnum.None ? "" : $"{GlobalLabelName}:{MiddleLabelName}";
        public string MiddleLabelName => DataType == DataTypeEnum.None ? "" : $"{LabelName}{ShortLabelName}";
        public string ShortLabelName => DataType == DataTypeEnum.None ? "" : (string.IsNullOrEmpty(SubLabelName) ? $"" : $".{SubLabelName}");
        
        public bool HasValue => DataType == DataTypeEnum.Value || DataType == DataTypeEnum.ADDR;
        public bool Invalidate => this.DataType == DataTypeEnum.Invalidate;
        public UInt16 Value { get; private set; }
        public string ValueString { get; private set; }

        public DataTypeEnum DataType { get; private set; }
        public AsmLoad AsmLoadForArgmument { get; private set; }

        public Label(string labelName, string valueString, AsmLoad asmLoad)
        {
            GlobalLabelName = asmLoad.GlobalLableName;
            LabelName = asmLoad.LabelName;
            DataType = DataTypeEnum.None;

            if (string.IsNullOrEmpty(labelName))
            {
                return;
            }
            
            if (!AIName.DeclareLabelValidate(labelName))
            {
                DataType = DataTypeEnum.Invalidate;
            }
            else
            {
                DataType = DataTypeEnum.ProcessingForValue;

                var matchedGlobalLable = Regex.Match(labelName, RegexPatternGlobalLabel, RegexOptions.Singleline);
                if (matchedGlobalLable.Success)
                {
                    // グローバルラベル
                    GlobalLabelName = matchedGlobalLable.Groups["lable"].Value;
                    asmLoad.GlobalLableName = GlobalLabelName;
                }
                else
                {
                    // valueStringが空の場合、通常ラベル、値が設定されている場合にはEquLabel
                    var matchedLable = Regex.Match(labelName,string.IsNullOrEmpty(valueString) ? RegexPatternLabel : RegexPatternEquLabel, RegexOptions.Singleline);
                    if (matchedLable.Success)
                    {
                        // ラベル
                        LabelName = matchedLable.Groups["lable"].Value;
                        asmLoad.LabelName = LabelName;
                    }
                    else
                    {
                        var matchedSubLable = Regex.Match(labelName, RegexPatternSubLabel, RegexOptions.Singleline);
                        if (matchedSubLable.Success)
                        {
                            SubLabelName = matchedSubLable.Groups["lable"].Value.Substring(1);
                        }
                    }
                }

                ValueString = valueString;
            }
        }

        public Label(LineDetailExpansionItemOperation lineDetailExpansionItemOperation, AsmLoad asmLoad)
            : this(lineDetailExpansionItemOperation.LabelText, "", asmLoad)
        {
            if (this.DataType == DataTypeEnum.ProcessingForValue)
            {
                DataType = DataTypeEnum.ProcessingForAddress;
            }
        }

        public Label(string labelName, string valueString, AsmLoad asmLoad, AsmLoad asmLoadForArgument)
            : this(labelName, valueString, asmLoad)
        {
            if (this.DataType == DataTypeEnum.ProcessingForValue)
            {
                DataType = DataTypeEnum.ProcessingForArgument;
                AsmLoadForArgmument = asmLoadForArgument;
            }
        }

        public void SetValue(AsmLoad asmLoad)
        {
            if (this.DataType != DataTypeEnum.ProcessingForValue)
                return;

            try
            {
                Value = AIMath.ConvertToUInt16(ValueString, GlobalLabelName, LabelName, asmLoad);
                this.DataType = DataTypeEnum.Value;
            }
            catch
            {
            }
        }

        public void SetArgument()
        {
            if (this.DataType != DataTypeEnum.ProcessingForArgument)
                return;

            if (string.IsNullOrEmpty(ValueString))
                return;

            try
            {
                Value = AIMath.ConvertToUInt16(ValueString, AsmLoadForArgmument);
                this.DataType = DataTypeEnum.Value;
            }
            catch
            {
            }
        }

        public void SetValueAndAddress(AsmAddress address, AsmLoad asmLoad)
        {
            if (this.DataType != DataTypeEnum.ProcessingForAddress &&
                this.DataType != DataTypeEnum.ProcessingForValue)
                return;

            if (string.IsNullOrEmpty(ValueString))
                return;

            try
            {
                Value = AIMath.ConvertToUInt16(ValueString, GlobalLabelName, LabelName, address, asmLoad);
                this.DataType = DataTypeEnum.Value;
            }
            catch
            {
            }
        }

        /// <summary>
        /// アドレスをセット
        /// </summary>
        /// <param name="address"></param>
        public void SetAddressLabel(AsmAddress address)
        {
            if (DataType == DataTypeEnum.ProcessingForAddress && string.IsNullOrEmpty(ValueString))
            {
                DataType = DataTypeEnum.ADDR;
                Value = address.Program;
            }
        }

        /// <summary>
        /// 値をセット
        /// </summary>
        /// <param name="address"></param>
        /// <param name="labels"></param>
        public void SetValueLabel(AsmAddress address, AsmLoad asmLoad)
        {

            switch (DataType)
            {
                case DataTypeEnum.None:
                case DataTypeEnum.ProcessingForAddress:
                case DataTypeEnum.ADDR:
                    break;
                case DataTypeEnum.Value:
                    Value = AIMath.ConvertToUInt16(ValueString, GlobalLabelName, LabelName, address, asmLoad);
                    break;
                default:
                    break;
            }
            
        }
        
        public static string GetLabelText(string lineString)
        {
            var matchedGlobalLable = Regex.Match(lineString, RegexPatternGlobalLabel, RegexOptions.Singleline);
            if (matchedGlobalLable.Success)
            {
                return matchedGlobalLable.Groups["lable"].Value + "::";
            }
            var matchedLable = Regex.Match(lineString, RegexPatternLabel, RegexOptions.Singleline);
            if (matchedLable.Success)
            {
                return matchedLable.Groups["lable"].Value + ":";
            }
            var matchedSubLable = Regex.Match(lineString, RegexPatternSubLabel, RegexOptions.Singleline);
            if (matchedSubLable.Success)
            {
                return matchedSubLable.Groups["lable"].Value;
            }
            var matchedValueLable = Regex.Match(lineString, RegexPatternValueLable, RegexOptions.Singleline | RegexOptions.IgnoreCase);
            if (matchedValueLable.Success)
            {
                return matchedValueLable.Groups["lable"].Value;
            }

            return "";
        }

        /// <summary>
        /// ロングラベル名を生成する
        /// </summary>
        /// <param name="labelName"></param>
        /// <param name="asmLoad"></param>
        /// <returns></returns>
        public static string GetLongLabelName(string labelName, AsmLoad asmLoad)
        {
            if (!Regex.Match(labelName, RegexPatternArgumentLabel, RegexOptions.IgnoreCase | RegexOptions.Singleline).Success)
                return labelName;

            if (labelName.IndexOf(":") > 0)
            {
                return labelName;
            }

            var dotIndex = labelName.IndexOf(".");
            if (dotIndex == 0)
            {
                return $"{asmLoad.GlobalLableName}:{asmLoad.LabelName}{labelName}";
            }

            return $"{asmLoad.GlobalLableName}:{labelName}";
        }

        /*
        /// <summary>
        /// 引数を正規化する
        /// </summary>
        /// <param name="argument"></param>
        /// <param name="asmLoad"></param>
        /// <returns></returns>
        public static string GetArgumentNormalization(string argument, AsmLoad asmLoad)
        {
            var resultValue = "";
            var workValue = argument;
            var regexResult = default(Match);

            while ((regexResult = Regex.Match(workValue, RegexPatternArgumentLabel, RegexOptions.IgnoreCase | RegexOptions.Singleline)).Success)
            {
                var matchResult = regexResult.Groups["value"];
                var longLabelName = GetLongLabelName(matchResult.Value, asmLoad);

                resultValue += workValue.Substring(0, matchResult.Index);
                resultValue += longLabelName;
                workValue = workValue.Substring(matchResult.Index + matchResult.Length);
            }
            resultValue += workValue;

            return resultValue;
        }
        */
    }
}
