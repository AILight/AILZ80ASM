﻿using AILZ80ASM.AILight;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace AILZ80ASM.Assembler
{
    public class Label
    {
        public enum DataTypeEnum
        {
            None,
            Marker,
            Invalidate,
            Value,
        }

        public enum LabelLevelEnum
        {
            None,
            GlobalLabel,
            Label,
            SubLabel,
        }

        private static readonly string RegexPatternGlobalLabel = @"^\[(?<label>([a-zA-Z0-9!--/-/<-@^-`{-~]+))\](\s+|$)";
        private static readonly string RegexPatternLabel = @"(?<label>(^[a-zA-Z0-9!--/-/<-@¥[-`{-~]+)):+(\s+|$)";
        //private static readonly string RegexPatternEquLabel = @"(?<label>(^[a-zA-Z0-9!--/-/<-@¥[-`{-~]+)):?";
        private static readonly string RegexPatternSubLabel = @"(?<label>(^\.[a-zA-Z0-9!--/-/<-@¥[-`{-~]+:*))(\s+|$)";
        private static readonly string RegexPatternValueLabel1 = @"(?<label>(^[a-zA-Z0-9!--/-/<-@¥[-`{-~]+:*))\s+equ\s+(?<value>(.+))";
        private static readonly string RegexPatternValueLabel2 = @"(?<label>(^[a-zA-Z0-9!--/-/<-@¥[-`{-~]+\.[a-zA-Z0-9!--/-/<-@¥[-`{-~]+:*))\s+equ\s+(?<value>(.+))";
        //private static readonly string RegexPatternArgumentLabel = @"(?<start>\s?)(?<value>([\w\.@]+))(?<end>\s?)";

        public string GlobalLabelName { get; private set; }
        public string LabelName { get; private set; }
        public string SubLabelName { get; private set; }
        public string LabelFullName { get; private set; }

        public bool Invalidate => this.DataType == DataTypeEnum.Invalidate;
        public int Value { get; private set; }
        public string ValueString { get; private set; }

        public DataTypeEnum DataType { get; private set; }
        public LabelLevelEnum LabelLevel { get; private set; }

        private AsmLoad AsmLoad { get; set; }
        private LineDetailExpansionItem LineDetailExpansionItem { get; set; }

        public Label(string labelName, AsmLoad asmLoad)
            : this(labelName, "", asmLoad)
        {
        }

        public Label(string labelName, string valueString, AsmLoad asmLoad)
        {
            GlobalLabelName = asmLoad.GlobalLabelName;
            LabelName = asmLoad.LabelName;
            ValueString = valueString;
            AsmLoad = asmLoad;
            LabelLevel = LabelLevelEnum.None;
            
            if (string.IsNullOrEmpty(labelName))
            {
                DataType = DataTypeEnum.Marker;
                return;
            }
            DataType = DataTypeEnum.None;

            if (!AIName.DeclareLabelValidate(labelName, asmLoad))
            {
                DataType = DataTypeEnum.Invalidate;
            }
            else
            {
                if (labelName.StartsWith('[') && labelName.EndsWith(']'))
                {
                    // グローバルアドレス
                    GlobalLabelName = labelName.Substring(1, labelName.Length - 2);
                    LabelLevel = LabelLevelEnum.GlobalLabel;
                }
                else
                {
                    var splits = labelName.Replace(":", "").Split('.');
                    switch (splits.Length)
                    {
                        case 1:
                            LabelName = splits[0];
                            LabelLevel = LabelLevelEnum.Label;
                            break;
                        case 2:
                            if (!string.IsNullOrEmpty(splits[0]))
                            {
                                LabelName = splits[0];
                                LabelLevel = LabelLevelEnum.Label;
                            }
                            else
                            {
                                LabelName = asmLoad.LabelName;
                            }
                            SubLabelName = splits[1];
                            LabelLevel = LabelLevelEnum.SubLabel;
                            break;
                        case 3:
                            if (splits.Any(m => string.IsNullOrEmpty(m)))
                            {
                                DataType = DataTypeEnum.Invalidate;
                            }
                            else
                            {
                                GlobalLabelName = splits[0];
                                LabelName = splits[1];
                                SubLabelName = splits[2];
                                LabelLevel = LabelLevelEnum.SubLabel;
                            }
                            break;

                        default:
                            DataType = DataTypeEnum.Invalidate;
                            break;
                    }
                }

                LabelFullName = LabelLevel switch
                {
                    LabelLevelEnum.GlobalLabel => GlobalLabelName,
                    LabelLevelEnum.Label => $"{GlobalLabelName}.{LabelName}",
                    LabelLevelEnum.SubLabel => $"{GlobalLabelName}.{LabelName}.{SubLabelName}",
                    _ => throw new NotImplementedException()
                };
                
            }
        }

        public Label(LineDetailExpansionItemOperation lineDetailExpansionItemOperation, AsmLoad asmLoad)
            : this(lineDetailExpansionItemOperation.LineItem.LabelString, "", asmLoad)
        {
            if (LabelLevel == LabelLevelEnum.Label ||
                LabelLevel == LabelLevelEnum.SubLabel)
            {
                ValueString = "$";
            }
            LineDetailExpansionItem = lineDetailExpansionItemOperation;

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
            var matchedValueLabel1 = Regex.Match(lineString, RegexPatternValueLabel1, RegexOptions.Singleline | RegexOptions.IgnoreCase);
            if (matchedValueLabel1.Success)
            {
                return matchedValueLabel1.Groups["label"].Value;
            }

            var matchedValueLabel2 = Regex.Match(lineString, RegexPatternValueLabel2, RegexOptions.Singleline | RegexOptions.IgnoreCase);
            if (matchedValueLabel2.Success)
            {
                return matchedValueLabel2.Groups["label"].Value;
            }

            return "";
        }

        /// <summary>
        /// ロングラベル名を生成する
        /// </summary>
        /// <param name="labelName"></param>
        /// <param name="asmLoad"></param>
        /// <returns></returns>
        public static string GetLabelFullName(string labelName, AsmLoad asmLoad)
        {
            var splits = labelName.Split('.');
            switch (splits.Length)
            {
                case 1:
                    return $"{asmLoad.GlobalLabelName}.{splits[0]}";
                case 2:
                    if (string.IsNullOrEmpty(splits[0]))
                    {
                        return $"{asmLoad.GlobalLabelName}.{asmLoad.LabelName}.{splits[1]}";
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(asmLoad.FindGlobalLabelName(splits[0])))
                        {
                            return $"{splits[0]}.{splits[1]}";
                        }
                        else
                        {
                            return $"{asmLoad.GlobalLabelName}.{splits[0]}.{splits[1]}";
                        }
                    }
                case 3:
                    if (splits.Any(m => string.IsNullOrEmpty(m)))
                    {
                        throw new Exception($"ラベルの指定名が間違っています。{labelName}");
                    }
                    return labelName;
                default:
                    throw new Exception($"ラベルの指定名が間違っています。{labelName}");
            }
        }

        public void Calculation()
        {
            if (DataType != DataTypeEnum.None)
            {
                return;
            }

            if (LineDetailExpansionItem == default)
            {
                Value = AIMath.ConvertTo<int>(ValueString, AsmLoad);
            }
            else
            {
                Value = AIMath.ConvertTo<int>(ValueString, AsmLoad, LineDetailExpansionItem.Address);
            }
            DataType = DataTypeEnum.Value;
        }


        public void SetLineDetailExpansionItem(LineDetailExpansionItem lineDetailExpansionItem)
        {
            LineDetailExpansionItem = lineDetailExpansionItem;
        }

    }
}
