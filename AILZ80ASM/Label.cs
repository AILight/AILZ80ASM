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
            Processing,
            ProcessingForValue,
            Value,
            ADDR,
        }

        private static readonly string RegexPatternGlobalLabel = @"(?<lable>(^\w+))::";
        private static readonly string RegexPatternLabel = @"(?<lable>(^\w+)):";
        private static readonly string RegexPatternSubLabel = @"(?<lable>(^\.\w+))";
        private static readonly string RegexPatternValueLable = @"(?<lable>(^\w+))\s+equ\s+(?<value>([\$\w]+))";
        private static readonly string RegexPatternArgument = @"(?<argument>(^\w+))";

        public Label(string labelName, string valueString, AsmLoad asmLoad)
        {
            DataType = DataTypeEnum.ProcessingForValue;
            GlobalLabelName = asmLoad.GlobalLableName;
            LabelName = asmLoad.LabelName;
            if (labelName.StartsWith("."))
            {
                SubLabelName = labelName.Substring(1);
            }
            else
            {
                LabelName = labelName.Replace(":", "");
            }
            ValueString = valueString;

            SetValue(asmLoad);
        }

        public Label(LineDetailExpansionItemOperation lineDetailExpansionItemOperation, AsmLoad asmLoad)
        {
            //グローバルラベルの設定
            GlobalLabelName = asmLoad.GlobalLableName;
            LabelName = asmLoad.LabelName;

            //ラベルを処理する
            DataType = DataTypeEnum.None;

            if (string.IsNullOrEmpty(lineDetailExpansionItemOperation.LabelText))
            {
                return;
            }

            var matchedGlobalLable = Regex.Match(lineDetailExpansionItemOperation.LabelText, RegexPatternGlobalLabel, RegexOptions.Singleline);
            if (matchedGlobalLable.Success)
            {
                // ラベルマッチ
                GlobalLabelName = matchedGlobalLable.Groups["lable"].Value;
                asmLoad.GlobalLableName = GlobalLabelName;
                DataType = DataTypeEnum.Processing;
            }
            else
            {
                var matchedLable = Regex.Match(lineDetailExpansionItemOperation.LabelText, RegexPatternLabel, RegexOptions.Singleline);
                if (matchedLable.Success)
                {
                    // ラベルマッチ
                    LabelName = matchedLable.Groups["lable"].Value;
                    asmLoad.LabelName = LabelName;

                    DataType = DataTypeEnum.Processing;
                }
                else
                {
                    var matchedSubLable = Regex.Match(lineDetailExpansionItemOperation.LabelText, RegexPatternSubLabel, RegexOptions.Singleline);
                    if (matchedSubLable.Success)
                    {
                        SubLabelName = matchedSubLable.Groups["lable"].Value.Substring(1);
                        DataType = DataTypeEnum.Processing;
                    }
                    else
                    {
                        LabelName = lineDetailExpansionItemOperation.LabelText;
                        asmLoad.LabelName = LabelName;
                        DataType = DataTypeEnum.Processing;
                    }
                }
            }
            if (DataType == DataTypeEnum.Processing && string.Compare(lineDetailExpansionItemOperation.InstructionText, "equ", true) == 0)
            {
                ValueString = lineDetailExpansionItemOperation.ArgumentText;
                DataType = DataTypeEnum.ProcessingForValue;
                lineDetailExpansionItemOperation.IsAssembled = true;
            }
        }

        /// <summary>
        /// 引数のラベルチェック
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static bool IsArgument(string target)
        {
            if (string.IsNullOrEmpty(target))
                return false;

            var matched = Regex.Match(target, RegexPatternArgument, RegexOptions.Singleline);
            return matched.Success && matched.Groups["argument"].Value == target;
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

        public void SetValueAndAddress(AsmAddress address, AsmLoad asmLoad)
        {
            if (this.DataType != DataTypeEnum.Processing &&
                this.DataType != DataTypeEnum.ProcessingForValue)
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

        public string GlobalLabelName { get; private set; }
        public string LabelName { get; private set; }
        public string SubLabelName { get; private set; }
        public string LongLabelName => DataType == DataTypeEnum.None ? "" : $"{GlobalLabelName}.{MiddleLabelName}";
        public string MiddleLabelName => DataType == DataTypeEnum.None ? "" : $"{LabelName}{ShortLabelName}";
        public string ShortLabelName => DataType == DataTypeEnum.None ? "" : (string.IsNullOrEmpty(SubLabelName) ? $"" : $".{SubLabelName}");
        public bool HasValue => DataType == DataTypeEnum.Value || DataType == DataTypeEnum.ADDR;
        public string ValueString { get; private set; }
        public UInt16 Value { get; private set; }
        public DataTypeEnum DataType { get; private set; }

        /// <summary>
        /// アドレスをセット
        /// </summary>
        /// <param name="address"></param>
        public void SetAddressLabel(AsmAddress address)
        {
            if (DataType == DataTypeEnum.Processing && string.IsNullOrEmpty(ValueString))
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
                case DataTypeEnum.Processing:
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

    }
}
