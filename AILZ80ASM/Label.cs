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
        private static readonly string RegexPatternValueLable = @"(?<lable>(^\w+))\s+equ\s+(?<value>([\$\w]+))";
        private static readonly string RegexPatternValue = @"^equ\s+(?<value>([\$\w]+))";

        public Label(LineItem lineItem)
        {
            //グローバルラベルの設定
            GlobalLabelName = lineItem.FileItem.WorkGlobalLabelName.ToUpper();
            LabelName = lineItem.FileItem.WorkLabelName.ToUpper();

            //ラベルを処理する
            var lineString = lineItem.OperationString.Trim();
            OperationCodeWithoutLabel = lineString;
            DataType = DataTypeEnum.None;

            var matchedGlobalLable = Regex.Match(lineString, RegexPatternGlobalLabel, RegexOptions.Singleline);
            if (matchedGlobalLable.Success)
            {
                // ラベルマッチ
                GlobalLabelName = matchedGlobalLable.Groups["lable"].Value.ToUpper();
                lineItem.FileItem.WorkGlobalLabelName = GlobalLabelName;

                OperationCodeWithoutLabel = lineString.Substring(GlobalLabelName.Length + 2).Trim();
                DataType = DataTypeEnum.Processing;
            }
            else
            {
                var matchedLable = Regex.Match(lineString, RegexPatternLabel, RegexOptions.Singleline);
                if (matchedLable.Success)
                {
                    // ラベルマッチ
                    LabelName = matchedLable.Groups["lable"].Value.ToUpper();
                    lineItem.FileItem.WorkLabelName = LabelName;

                    OperationCodeWithoutLabel = lineString.Substring(LabelName.Length + 1).Trim();
                    DataType = DataTypeEnum.Processing;
                }
                else
                {
                    var matchedSubLable = Regex.Match(lineString, RegexPatternSubLabel, RegexOptions.Singleline);
                    if (matchedSubLable.Success)
                    {
                        SubLabelName = matchedSubLable.Groups["lable"].Value.ToUpper().Substring(1);
                        OperationCodeWithoutLabel = lineString.Substring(SubLabelName.Length + 1).Trim();
                        DataType = DataTypeEnum.Processing;
                    }
                    else
                    {
                        var matchedValueLable = Regex.Match(lineString, RegexPatternValueLable, RegexOptions.Singleline | RegexOptions.IgnoreCase);
                        if (matchedValueLable.Success)
                        {
                            LabelName = matchedValueLable.Groups["lable"].Value.ToUpper();
                            OperationCodeWithoutLabel = lineString.Substring(LabelName.Length).Trim();
                            DataType = DataTypeEnum.Processing;
                        }

                    }
                }
            }
            if (DataType == DataTypeEnum.Processing)
            {
                var matchedValue = Regex.Match(OperationCodeWithoutLabel, RegexPatternValue, RegexOptions.Singleline | RegexOptions.IgnoreCase);
                if (matchedValue.Success)
                {
                    ValueString = matchedValue.Groups["value"].Value.ToUpper();
                    OperationCodeWithoutLabel = "";
                    DataType = DataTypeEnum.Value;
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
        public void SetValueLabel(AsmAddress address, Label[] labels)
        {

            switch (DataType)
            {
                case DataTypeEnum.None:
                case DataTypeEnum.Processing:
                case DataTypeEnum.ADDR:
                    break;
                case DataTypeEnum.Value:
                    Value = AIMath.ConvertToUInt16(ValueString, GlobalLabelName, LabelName, address, labels);
                    break;
                default:
                    break;
            }
            
        }
    }
}
