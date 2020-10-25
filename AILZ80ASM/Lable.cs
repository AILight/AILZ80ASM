using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;

namespace AILZ80ASM
{
    public class Lable
    {
        private static readonly string RegexPatternGlobalLabel = @"(?<lable>(^\w+))::";
        private static readonly string RegexPatternLabel = @"(?<lable>(^\w+)):";
        private static readonly string RegexPatternSubLabel = @"(?<lable>(^\.\w+))";
        private static readonly string RegexPatternValue = @"\s+equ\s+(?<value>(.+))";

        public Lable(LineItem lineItem)
        {
            //グローバルラベルの設定
            GlobalLabelName = lineItem.FileItem.WorkGlobalLabelName;
            LabelName = lineItem.FileItem.WorkLabelName;

            //ラベルを処理する
            var lineString = lineItem.OperationString;
            OperationCodeWithoutLabel = lineString;
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

        public void SetAddressLabel()
        {
            if (DataType == DataTypeEnum.Processing)
            {
                Value = Convert.ToUInt16(new DataTable().Compute(ValueString, null));
            }
        }
    }
}
