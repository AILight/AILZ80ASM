using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;

namespace AILZ80ASM
{
    public class Label
    {
        private static readonly string RegexPatternGlobalLabel = @"(?<lable>(^\w+))::";
        private static readonly string RegexPatternLabel = @"(?<lable>(^\w+)):";
        private static readonly string RegexPatternSubLabel = @"(?<lable>(^\.\w+))";
        private static readonly string RegexPatternValue = @"\s+equ\s+(?<value>(.+))";

        public Label(LineItem lineItem)
        {
            //グローバルラベルの設定
            GlobalLabelName = lineItem.FileItem.WorkGlobalLabelName;
            LabelName = lineItem.FileItem.WorkLabelName;

            //ラベルを処理する
            var lineString = lineItem.OperationString;
            OperationCodeWithoutLabel = lineString.Trim();
            DataType = DataTypeEnum.None;

            var matchedGlobalLable = Regex.Match(lineString, RegexPatternGlobalLabel, RegexOptions.Singleline);
            if (matchedGlobalLable.Success)
            {
                // ラベルマッチ
                GlobalLabelName = matchedGlobalLable.Groups["lable"].Value;
                lineItem.FileItem.WorkGlobalLabelName = GlobalLabelName;

                OperationCodeWithoutLabel = lineString.Substring(GlobalLabelName.Length).Trim();
                DataType = DataTypeEnum.Processing;
            }
            else
            {
                var matchedLable = Regex.Match(lineString, RegexPatternLabel, RegexOptions.Singleline);
                if (matchedLable.Success)
                {
                    // ラベルマッチ
                    LabelName = matchedLable.Groups["lable"].Value;
                    lineItem.FileItem.WorkLabelName = LabelName;

                    OperationCodeWithoutLabel = lineString.Substring(LabelName.Length).Trim();
                    DataType = DataTypeEnum.Processing;
                }
                else
                {
                    var matchedSubLable = Regex.Match(lineString, RegexPatternSubLabel, RegexOptions.Singleline);
                    if (matchedSubLable.Success)
                    {
                        SubLabelName = matchedLable.Groups["lable"].Value;
                        OperationCodeWithoutLabel = lineString.Substring(SubLabelName.Length).Trim();
                        DataType = DataTypeEnum.Processing;
                    }
                }
            }
            if (DataType == DataTypeEnum.Processing)
            {
                var matchedValue = Regex.Match(OperationCodeWithoutLabel, RegexPatternValue, RegexOptions.Singleline);
                if (matchedValue.Success)
                {
                    ValueString = matchedValue.Groups["value"].Value;
                    OperationCodeWithoutLabel = "";
                }
            }

        }

        public enum DataTypeEnum
        {
            None,
            Processing,
            Value,
            ADDR,
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
        public string OperationCodeWithoutLabel { get; private set; }

        /// <summary>
        /// アドレスをセット
        /// </summary>
        /// <param name="address"></param>
        public void SetAddressLabel(ushort address)
        {
            if (DataType == DataTypeEnum.Processing && string.IsNullOrEmpty(ValueString))
            {
                DataType = DataTypeEnum.ADDR;
                Value = address;
            }
        }

        /// <summary>
        /// 値をセット
        /// </summary>
        /// <param name="address"></param>
        /// <param name="labels"></param>
        public void SetValueLabel(ushort address, Label[] labels)
        {
            if (DataType == DataTypeEnum.Processing && !string.IsNullOrEmpty(ValueString))
            {
                Value = AIMath.ConvertToUInt16(ValueString, GlobalLabelName, LabelName, address, labels);
                DataType = DataTypeEnum.Value;
            }
            
        }
    }
}
