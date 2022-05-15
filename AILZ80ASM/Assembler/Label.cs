using AILZ80ASM.AILight;
using AILZ80ASM.Exceptions;
using AILZ80ASM.LineDetailItems;
using AILZ80ASM.LineDetailItems.ScopeItem.ExpansionItems;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace AILZ80ASM.Assembler
{
    public abstract class Label
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

        public enum LabelTypeEnum
        {
            Equ,
            Adr,
            //Arg,
            MacroArg,
            FunctionArg,
        }

        private static readonly string RegexPatternGlobalLabel = @"^\[(?<label>([a-zA-Z0-9!-/:-@\[-~]+))\](\s+|$)";
        private static readonly string RegexPatternLabel = @"(?<label>(^[a-zA-Z0-9!-/:-@\[-~]+)):+(\s+|$)";
        //private static readonly string RegexPatternEquLabel = @"(?<label>(^[a-zA-Z0-9!-/:-@\[-~]+)):?";
        private static readonly string RegexPatternSubLabel = @"(?<label>(^\.[a-zA-Z0-9!-/:-@\[-~]+:*))(\s+|$)";
        private static readonly string RegexPatternValueLabel1 = @"(?<label>(^[a-zA-Z0-9!-/:-@\[-~]+:*))\s+(" + String.Join('|', AsmReservedWord.GetReservedWordsForLabel().Select(m => m.Name)) + @")\s+(?<value>(.+))";
        private static readonly string RegexPatternValueLabel2 = @"(?<label>(^[a-zA-Z0-9!-/:-@\[-~]+\.[a-zA-Z0-9!-/:-@\[-~]+:*))\s+(" + String.Join('|', AsmReservedWord.GetReservedWordsForLabel().Select(m => m.Name)) + @")\s+(?<value>(.+))";
        //private static readonly string RegexPatternArgumentLabel = @"(?<start>\s?)(?<value>([\w\.@]+))(?<end>\s?)";

        public string GlobalLabelName { get; private set; }
        public string LabelName { get; private set; }
        public string SubLabelName { get; private set; }
        public string LabelFullName { get; private set; }
        public string LabelShortName => LabelLevel switch
        {
            LabelLevelEnum.GlobalLabel => GlobalLabelName,
            LabelLevelEnum.Label => LabelName,
            LabelLevelEnum.SubLabel => $"{LabelName}.{SubLabelName}",
            _ => throw new NotSupportedException()
        };

        public bool Invalidate => this.DataType == DataTypeEnum.Invalidate;
        public AIValue Value { get; private set; }
        public string ValueString { get; private set; }

        public DataTypeEnum DataType { get; private set; }
        public LabelLevelEnum LabelLevel { get; private set; }
        public LabelTypeEnum LabelType { get; private set; }
        public LineItem LineItem => LineDetailItem != default ? LineDetailItem.LineItem : LineDetailExpansionItem != default ? LineDetailExpansionItem.LineItem : default;

        private AsmLoad AsmLoad { get; set; }
        private LineDetailExpansionItem LineDetailExpansionItem { get; set; }
        private LineDetailItem LineDetailItem { get; set; }

        public Label(string labelName, AsmLoad asmLoad, LabelTypeEnum labelType)
            : this(labelName, "", asmLoad, labelType)
        {
        }
        public Label(string labelName, string valueString, AsmLoad asmLoad, LabelTypeEnum labelType) 
            : this(labelName, valueString, default(AIValue), asmLoad, labelType)
        {
        }

        public Label(string labelName, string valueString, AIValue aiValue, AsmLoad asmLoad, LabelTypeEnum labelType)
        {
            GlobalLabelName = asmLoad.Scope.GlobalLabelName;
            LabelName = asmLoad.Scope.LabelName;
            ValueString = valueString;
            LabelType = labelType;
            AsmLoad = asmLoad;
            LabelLevel = LabelLevelEnum.None;

            if (string.IsNullOrEmpty(labelName))
            {
                DataType = DataTypeEnum.Marker;
                return;
            }

            if (aiValue != default)
            {
                Value = aiValue;
                DataType = DataTypeEnum.Value;
            }
            else
            {
                DataType = DataTypeEnum.None;
            }

            switch (labelType)
            {
                case LabelTypeEnum.Equ:
                case LabelTypeEnum.Adr:
                    if (!AIName.DeclareLabelValidate(labelName, asmLoad))
                    {
                        DataType = DataTypeEnum.Invalidate;
                    }
                    break;
                    /*
                case LabelTypeEnum.Arg:
                    if (!AIName.ValidateMacroArgument(labelName, asmLoad))
                    {
                        DataType = DataTypeEnum.Invalidate;
                    }
                    break;*/
                case LabelTypeEnum.FunctionArg:
                    if (!AIName.ValidateFunctionArgument(labelName, asmLoad))
                    {
                        DataType = DataTypeEnum.Invalidate;
                    }
                    break;
                case LabelTypeEnum.MacroArg:
                    if (!AIName.ValidateMacroArgument(labelName, asmLoad))
                    {
                        DataType = DataTypeEnum.Invalidate;
                    }
                    break;
                default:
                    throw new NotSupportedException();
            }


            if (DataType != DataTypeEnum.Invalidate)
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
                                LabelName = asmLoad.Scope.LabelName;
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

        public Label(LineDetailItem lineDetailItem, AsmLoad asmLoad, LabelTypeEnum labelType)
            : this(lineDetailItem.LineItem.LabelString, labelType == LabelTypeEnum.Equ ? ((LineDetailItemEqual)lineDetailItem).LabelValue : "", asmLoad, labelType)
        {
            if (labelType == LabelTypeEnum.Adr)
            {
                if (LabelLevel == LabelLevelEnum.Label ||
                    LabelLevel == LabelLevelEnum.SubLabel)
                {
                    ValueString = "$";
                }
            }
            LineDetailItem = lineDetailItem;

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
                while (startIndex < lineString.Length && lineString[startIndex] == ':')
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
            if (matchedValueLabel1.Success && !AsmReservedWord.GetReservedWordsForLabel().Any(m => string.Compare(m.Name, matchedValueLabel1.Groups["label"].Value, true) == 0))
            {
                return matchedValueLabel1.Groups["label"].Value;
            }

            var matchedValueLabel2 = Regex.Match(lineString, RegexPatternValueLabel2, RegexOptions.Singleline | RegexOptions.IgnoreCase);
            if (matchedValueLabel2.Success && !AsmReservedWord.GetReservedWordsForLabel().Any(m => string.Compare(m.Name, matchedValueLabel2.Groups["label"].Value, true) == 0))
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
                    return $"{asmLoad.Scope.GlobalLabelName}.{splits[0]}";
                case 2:
                    if (string.IsNullOrEmpty(splits[0]))
                    {
                        return $"{asmLoad.Scope.GlobalLabelName}.{asmLoad.Scope.LabelName}.{splits[1]}";
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(asmLoad.FindGlobalLabelName(splits[0])))
                        {
                            return $"{splits[0]}.{splits[1]}";
                        }
                        else
                        {
                            return $"{asmLoad.Scope.GlobalLabelName}.{splits[0]}.{splits[1]}";
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

        /// <summary>
        /// ラベルの値を計算する
        /// </summary>
        public virtual void Calculation()
        {
            InternalCalculation(AsmLoad);
        }

        protected void InternalCalculation(AsmLoad asmLoad)
        {
            if (DataType != DataTypeEnum.None)
            {
                return;
            }

            var asmAddress = default(AsmAddress?);
            if (LineDetailExpansionItem != default)
            {
                asmAddress = LineDetailExpansionItem.Address;
            }
            else if (LineDetailItem != default)
            {
                asmAddress = LineDetailItem.Address;
            }
            Value = AIMath.Calculation(ValueString, asmLoad, asmAddress);
            DataType = DataTypeEnum.Value;
        }

        /// <summary>
        /// ラベルをビルドする（値を確定させる）
        /// </summary>
        public void BuildLabel()
        {
            try
            {
                Calculation();
                if (LineDetailExpansionItem != default && AsmLoad != default)
                {
                    AsmLoad.AddError(new ErrorLineItem(LineDetailExpansionItem.LineItem, Error.ErrorCodeEnum.I0001, $"{LabelShortName}"));
                }
            }
            catch (Exception ex)
            {
                if ((LineDetailExpansionItem != default || LineDetailItem != default) && AsmLoad != default)
                {
                    var lineItem = LineDetailExpansionItem != default ? LineDetailExpansionItem.LineItem : LineDetailItem.LineItem;
                    var errorLineItem = default(ErrorLineItem);

                    if (ex is ErrorAssembleException eae)
                    {
                        errorLineItem = new ErrorLineItem(lineItem, eae);
                    }
                    else
                    {
                        errorLineItem = new ErrorLineItem(lineItem, Error.ErrorCodeEnum.E0004, ex.Message);
                    }
                    AsmLoad.AddError(errorLineItem);
                }
                else
                {
                    throw;
                }
            }
        }

        public void SetLineDetailExpansionItem(LineDetailExpansionItem lineDetailExpansionItem)
        {
            LineDetailExpansionItem = lineDetailExpansionItem;
        }

    }
}
