using AILZ80ASM.AILight;
using AILZ80ASM.Assembler;
using System;
using System.Linq;
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

        public enum LabelLevelEnum
        {
            GlobalLabel,
            Label,
            SubLabel,
        }

        private static readonly string RegexPatternGlobalLabel = @"^\[(?<label>([a-zA-Z0-9!--/-/<-@^-`{-~]+))\](\s+|$)";
        private static readonly string RegexPatternLabel = @"(?<label>(^[a-zA-Z0-9!--/-/<-@¥[-`{-~]+)):+(\s+|$)";
        private static readonly string RegexPatternEquLabel = @"(?<label>(^[a-zA-Z0-9!--/-/<-@¥[-`{-~]+)):?";
        private static readonly string RegexPatternSubLabel = @"(?<label>(^\.[a-zA-Z0-9!--/-/<-@¥[-`{-~]+:?))(\s+|$)";
        private static readonly string RegexPatternValueLabel = @"(?<label>(^[a-zA-Z0-9!--/-/<-@¥[-`{-~]+))\s+equ\s+(?<value>(.+))";
        private static readonly string RegexPatternArgumentLabel = @"(?<start>\s?)(?<value>([\w\.@]+))(?<end>\s?)";

        public string GlobalLabelName { get; private set; }
        public string LabelName { get; private set; }
        public string SubLabelName { get; private set; }
        public string LongLabelName => DataType == DataTypeEnum.None ? "" : $"{GlobalLabelName}.{MiddleLabelName}";
        public string MiddleLabelName => DataType == DataTypeEnum.None ? "" : $"{LabelName}{ShortLabelName}";
        public string ShortLabelName => DataType == DataTypeEnum.None ? "" : (string.IsNullOrEmpty(SubLabelName) ? $"" : $".{SubLabelName}");

        public bool HasValue => DataType == DataTypeEnum.Value || DataType == DataTypeEnum.ADDR;
        public bool Invalidate => this.DataType == DataTypeEnum.Invalidate;
        public int Value { get; private set; }
        public string ValueString { get; private set; }

        public DataTypeEnum DataType { get; private set; }
        public LabelLevelEnum LabelLevel { get; private set; }

        public AsmLoad AsmLoadForArgmument { get; private set; }

        public Label(string labelName, AsmLoad asmLoad)
            : this(labelName, "", asmLoad)
        {
        }

        public Label(string labelName, string valueString, AsmLoad asmLoad)
        {
            GlobalLabelName = asmLoad.GlobalLabelName;
            LabelName = asmLoad.LabelName;
            DataType = DataTypeEnum.None;

            if (string.IsNullOrEmpty(labelName))
            {
                return;
            }

            if (!AIName.DeclareLabelValidate(labelName, asmLoad))
            {
                DataType = DataTypeEnum.Invalidate;
            }
            else
            {
                DataType = DataTypeEnum.ProcessingForValue;

                var matchedGlobalLabel = Regex.Match(labelName, RegexPatternGlobalLabel, RegexOptions.Singleline | RegexOptions.IgnoreCase);
                if (matchedGlobalLabel.Success)
                {
                    // グローバルラベル
                    GlobalLabelName = matchedGlobalLabel.Groups["label"].Value;
                    LabelName = "";
                    LabelLevel = LabelLevelEnum.GlobalLabel;
                }
                else
                {
                    // valueStringが空の場合、通常ラベル、値が設定されている場合にはEquLabel
                    var matchedLabel = Regex.Match(labelName, string.IsNullOrEmpty(valueString) ? RegexPatternLabel : RegexPatternEquLabel, RegexOptions.Singleline | RegexOptions.IgnoreCase);
                    if (matchedLabel.Success)
                    {
                        // ラベル
                        LabelName = matchedLabel.Groups["label"].Value;
                        LabelLevel = LabelLevelEnum.Label;

                    }
                    else
                    {
                        var matchedSubLabel = Regex.Match(labelName, RegexPatternSubLabel, RegexOptions.Singleline | RegexOptions.IgnoreCase);
                        if (matchedSubLabel.Success)
                        {
                            var subLabelName = matchedSubLabel.Groups["label"].Value.Substring(1);
                            if (subLabelName.EndsWith(":"))
                            {
                                subLabelName = subLabelName.Substring(0, subLabelName.Length - 1);
                            }

                            SubLabelName = subLabelName;
                            LabelLevel = LabelLevelEnum.SubLabel;
                        }
                    }
                }

                ValueString = valueString;
            }
        }

        public Label(LineDetailExpansionItemOperation lineDetailExpansionItemOperation, AsmLoad asmLoad)
            : this(lineDetailExpansionItemOperation.LineItem.LabelString, "", asmLoad)
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

            if (AIMath.TryParse<int>(ValueString, asmLoad, out var value))
            {
                Value = value;
                this.DataType = DataTypeEnum.Value;
            }
        }

        public void SetArgument()
        {
            if (this.DataType != DataTypeEnum.ProcessingForArgument)
                return;

            if (string.IsNullOrEmpty(ValueString))
                return;

            if (AIMath.TryParse<int>(ValueString, AsmLoadForArgmument, out var value))
            {
                Value = value;
                this.DataType = DataTypeEnum.Value;
            }
        }

        public void SetValueAndAddress(AsmAddress address, AsmLoad asmLoad)
        {
            if (this.DataType != DataTypeEnum.ProcessingForAddress &&
                this.DataType != DataTypeEnum.ProcessingForValue)
                return;

            if (string.IsNullOrEmpty(ValueString))
                return;

            if (AIMath.TryParse<int>(ValueString, asmLoad, address, out var value))
            {
                Value = value;
                this.DataType = DataTypeEnum.Value;
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
                    if (AIMath.TryParse<int>(ValueString, asmLoad, address, out var value))
                    {
                        Value = value;
                    }
                    break;
                default:
                    break;
            }

        }

        public static string GetLabelText(string lineString)
        {
            var matchedGlobalLabel = Regex.Match(lineString, RegexPatternGlobalLabel, RegexOptions.Singleline | RegexOptions.IgnoreCase);
            if (matchedGlobalLabel.Success)
            {
                return "[" + matchedGlobalLabel.Groups["label"].Value + "]";
            }
            var matchedLabel = Regex.Match(lineString, RegexPatternLabel, RegexOptions.Singleline | RegexOptions.IgnoreCase);
            if (matchedLabel.Success)
            {
                var label = matchedLabel.Groups["label"].Value;
                var startIndex = label.Length;
                while (lineString.IndexOf(":", startIndex) != -1)
                {
                    startIndex++;
                }

                return lineString.Substring(0, startIndex);
            }
            var matchedSubLabel = Regex.Match(lineString, RegexPatternSubLabel, RegexOptions.Singleline | RegexOptions.IgnoreCase);
            if (matchedSubLabel.Success)
            {
                return matchedSubLabel.Groups["label"].Value;
            }
            var matchedValueLabel = Regex.Match(lineString, RegexPatternValueLabel, RegexOptions.Singleline | RegexOptions.IgnoreCase);
            if (matchedValueLabel.Success)
            {
                return matchedValueLabel.Groups["label"].Value;
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

            if (labelName.StartsWith("."))
            {
                return $"{asmLoad.GlobalLabelName}.{asmLoad.LabelName}{labelName}";
            }

            var count = labelName.ToArray().Count(m => m == '.');
            if (count == 2)
            {
                return labelName;
            }
            else if (count == 1)
            {
                var nameSpace = labelName.Substring(0, labelName.IndexOf("."));
                if (asmLoad.AllLabels.Where(m => string.Compare(m.LongLabelName, labelName, true) == 0).Any())
                {
                    return labelName;
                }
                else
                {
                    return $"{asmLoad.GlobalLabelName}.{labelName}";
                }
            }
            else if (count == 0)
            {
                return $"{asmLoad.GlobalLabelName}.{labelName}";
            }
            else
            {
                throw new Exception($"ラベルの指定名が間違っています。{labelName}");
            }
        }
    }
}
