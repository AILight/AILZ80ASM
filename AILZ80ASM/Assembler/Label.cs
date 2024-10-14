using AILZ80ASM.AILight;
using AILZ80ASM.Exceptions;
using AILZ80ASM.LineDetailItems;
using AILZ80ASM.LineDetailItems.ScopeItem.ExpansionItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AILZ80ASM.Assembler
{
    public abstract class Label
    {
        public enum DataTypeEnum
        {
            None,
            Invalid,
            Value,
        }

        public enum LabelLevelEnum
        {
            None,
            GlobalLabel,
            Label,
            SubLabel,
            TmpLabel,
            AnonLabel,
        }

        public enum LabelTypeEnum
        {
            Equ,
            Adr,
            MacroArg,
            FunctionArg,
        }

        public enum LabelValueTypeEnum
        {
            Normal,
            Register,
        }

        private static readonly string RegexPatternGlobalLabel = @"^\[(?<label>([a-zA-Z0-9!-/:-@\[-~]+))\]";
        private static readonly Regex CompiledRegexPatternGlobalLabel = new Regex(
                RegexPatternGlobalLabel, RegexOptions.Singleline | RegexOptions.IgnoreCase
        );
        private static readonly string RegexPatternLabel = @"(?<label>(^[a-zA-Z0-9!-/:-@\[-~]+)):+";
        private static readonly Regex CompiledRegexPatternLabel = new Regex(
                RegexPatternLabel, RegexOptions.Singleline | RegexOptions.IgnoreCase
        );
        private static readonly string RegexPatternSubLabel = @"(?<label>(^\.[a-zA-Z0-9!-/;-@\[-~]+:*))(\s+|$)";
        private static readonly Regex CompiledRegexPatternSubLabel = new Regex(
                RegexPatternSubLabel, RegexOptions.Singleline | RegexOptions.IgnoreCase
        );
        private static readonly string RegexPatternValueLabel1 = @"(?<label>(^[a-zA-Z0-9!-/:-@\[-~]+:*))\s+(" + String.Join('|', AsmReservedWord.GetReservedWordsForLabel().Select(m => m.Name)) + @")\s+(?<value>(.+))";
        private static readonly Regex CompiledRegexPatternValueLabel1 = new Regex(
                RegexPatternValueLabel1, RegexOptions.Singleline | RegexOptions.IgnoreCase
        );
        private static readonly string RegexPatternValueLabel2 = @"(?<label>(^[a-zA-Z0-9!-/:-@\[-~]+\.[a-zA-Z0-9!-/:-@\[-~]+:*))\s+(" + String.Join('|', AsmReservedWord.GetReservedWordsForLabel().Select(m => m.Name)) + @")\s+(?<value>(.+))";
        private static readonly Regex CompiledRegexPatternValueLabel2 = new Regex(
                RegexPatternValueLabel2, RegexOptions.Singleline | RegexOptions.IgnoreCase
        );
        private static readonly string RegexPatternValueLabel3 = @"(?<label>(^[a-zA-Z0-9!-/:-@\[-~]+)):?\s*equ";
        private static readonly Regex CompiledRegexPatternValueLabel3 = new Regex(
                RegexPatternValueLabel3, RegexOptions.Singleline | RegexOptions.IgnoreCase
        );

        public string GlobalLabelName { get; private set; }
        public string LabelName { get; private set; }
        public string SubLabelName { get; private set; }
        public string TmpLabelName { get; private set; }
        public string AnonLabelName { get; private set; }
        public string LabelFullName { get; private set; }
        public string LabelShortName => LabelLevel switch
        {
            LabelLevelEnum.GlobalLabel => GlobalLabelName,
            LabelLevelEnum.Label => LabelName,
            LabelLevelEnum.SubLabel => $"{LabelName}.{SubLabelName}",
            LabelLevelEnum.TmpLabel => $"{LabelName}.{SubLabelName}.{TmpLabelName}",
            LabelLevelEnum.AnonLabel => $"{LabelName}.{SubLabelName}.{AnonLabelName}",
            _ => throw new NotSupportedException()
        };

        public bool Invalidate => this.DataType == DataTypeEnum.Invalid;
        public AIValue Value { get; private set; }
        public string ValueString { get; private set; }
        public LabelValueTypeEnum LabelValueType { get; private set; } = LabelValueTypeEnum.Normal;

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
            SubLabelName = asmLoad.Scope.SubLabelName;
            ValueString = valueString;
            LabelType = labelType;
            AsmLoad = asmLoad;
            LabelLevel = LabelLevelEnum.None;
            LabelValueType = asmLoad.ISA.IsMatchRegisterName(valueString) ? LabelValueTypeEnum.Register : LabelValueTypeEnum.Normal;

            if (string.IsNullOrEmpty(labelName))
            {
                DataType = DataTypeEnum.Invalid;
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
                        DataType = DataTypeEnum.Invalid;
                    }
                    break;
                case LabelTypeEnum.FunctionArg:
                    if (!AIName.ValidateFunctionArgument(labelName, asmLoad))
                    {
                        DataType = DataTypeEnum.Invalid;
                    }
                    break;
                case LabelTypeEnum.MacroArg:
                    if (!AIName.ValidateMacroArgument(labelName, asmLoad))
                    {
                        DataType = DataTypeEnum.Invalid;
                    }
                    break;
                default:
                    throw new NotSupportedException();
            }


            if (DataType != DataTypeEnum.Invalid)
            {
                if (labelName.StartsWith('[') && labelName.EndsWith(']'))
                {
                    // グローバルアドレス
                    GlobalLabelName = labelName.Substring(1, labelName.Length - 2);
                    LabelName = "";
                    SubLabelName = "";
                    TmpLabelName = "";
                    LabelLevel = LabelLevelEnum.GlobalLabel;
                }
                else
                {
                    var splits = labelName.Replace(":", "").Split('.');
                    switch (splits.Length)
                    {
                        case 1:
                            LabelName = splits[0];
                            SubLabelName = "";
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
                                SubLabelName = asmLoad.Scope.SubLabelName;
                            }

                            if (splits[1].StartsWith("@@"))
                            {
                                var anonGuid = Guid.NewGuid();
                                var sum = anonGuid.ToByteArray().Select(m => (int)m).Sum() % 10;
                                AnonLabelName = $"@@{sum}{anonGuid:N}";
                                LabelLevel = LabelLevelEnum.AnonLabel;
                            }
                            else if (splits[1].StartsWith("@"))
                            {
                                TmpLabelName = splits[1];
                                LabelLevel = LabelLevelEnum.TmpLabel;
                            }
                            else
                            {
                                SubLabelName = splits[1];
                                TmpLabelName = "";
                                LabelLevel = LabelLevelEnum.SubLabel;
                            }
                            break;
                        case 3:
                            if (splits.Any(m => string.IsNullOrEmpty(m)))
                            {
                                DataType = DataTypeEnum.Invalid;
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
                            DataType = DataTypeEnum.Invalid;
                            break;
                    }
                }

                LabelFullName = LabelLevel switch
                {
                    LabelLevelEnum.GlobalLabel => GlobalLabelName,
                    LabelLevelEnum.Label => $"{GlobalLabelName}.{LabelName}",
                    LabelLevelEnum.SubLabel => $"{GlobalLabelName}.{LabelName}.{SubLabelName}",
                    LabelLevelEnum.TmpLabel => $"{GlobalLabelName}.{LabelName}.{SubLabelName}.{TmpLabelName}",
                    LabelLevelEnum.AnonLabel => $"{GlobalLabelName}.{LabelName}.{SubLabelName}.{AnonLabelName}",
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
                    LabelLevel == LabelLevelEnum.SubLabel ||
                    LabelLevel == LabelLevelEnum.TmpLabel ||
                    LabelLevel == LabelLevelEnum.AnonLabel)
                {
                    ValueString = "$";
                }
            }
            LineDetailItem = lineDetailItem;

        }

        public static string GetLabelText(string lineString)
        {
            var matchedGlobalLabel = CompiledRegexPatternGlobalLabel.Match(lineString);
            if (matchedGlobalLabel.Success)
            {
                return "[" + matchedGlobalLabel.Groups["label"].Value + "]";
            }
            var matchedLabel = CompiledRegexPatternLabel.Match(lineString);
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
            var matchedSubLabel = CompiledRegexPatternSubLabel.Match(lineString);
            if (matchedSubLabel.Success)
            {
                return matchedSubLabel.Groups["label"].Value;
            }
            var matchedValueLabel1 = CompiledRegexPatternValueLabel1.Match(lineString);
            if (matchedValueLabel1.Success && !AsmReservedWord.GetReservedWordsForLabel().Any(m => string.Compare(m.Name, matchedValueLabel1.Groups["label"].Value, true) == 0))
            {
                return matchedValueLabel1.Groups["label"].Value;
            }

            var matchedValueLabel2 = CompiledRegexPatternValueLabel2.Match(lineString);
            if (matchedValueLabel2.Success && !AsmReservedWord.GetReservedWordsForLabel().Any(m => string.Compare(m.Name, matchedValueLabel2.Groups["label"].Value, true) == 0))
            {
                return matchedValueLabel2.Groups["label"].Value;
            }

            var matchedValueLabel3 = CompiledRegexPatternValueLabel3.Match(lineString);
            if (matchedValueLabel3.Success)
            {
                // equの時には、ラベル指定可能にする
                return matchedValueLabel3.Groups["label"].Value;
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
            //.@演算子を削除する
            labelName = RemoveOperators(labelName);

            var splits = labelName.Split('.');
            if (splits.Length == 0)
            {
                throw new Exception($"ラベルの指定名が間違っています。{labelName}");
            }

            // .なし
            if (splits.Length == 1)
            {
                return $"{asmLoad.Scope.GlobalLabelName}.{splits[0]}";
            }

            // テンポラリーラベルの存在を確認する
            var isTempLabel = splits.Last().StartsWith("@");
            var isLocalLabel = !isTempLabel && string.IsNullOrEmpty(splits[0]);
            if (isLocalLabel)
            {
                // ローカルラベル指定
                switch (splits.Length)
                {
                    case 2:
                        return $"{asmLoad.Scope.GlobalLabelName}.{asmLoad.Scope.LabelName}.{splits[1]}";
                    default:
                        throw new Exception($"ラベルの指定名が間違っています。{labelName}");
                }
            } 
            else if (isTempLabel)
            {
                // テンポラリーラベル指定
                if (string.IsNullOrEmpty(splits[0]))
                {
                    // ローカルラベル指定の場合
                    switch (splits.Length)
                    {
                        case 2:
                            return $"{asmLoad.Scope.GlobalLabelName}.{asmLoad.Scope.LabelName}.{asmLoad.Scope.SubLabelName}.{splits[1]}";
                        case 3:
                            return $"{asmLoad.Scope.GlobalLabelName}.{asmLoad.Scope.LabelName}.{splits[1]}.{splits[2]}";
                        default:
                            throw new Exception($"ラベルの指定名が間違っています。{labelName}");
                    }
                }
                else
                {
                    switch (splits.Length)
                    {
                        case 2:
                            return $"{asmLoad.Scope.GlobalLabelName}.{splits[0]}..{splits[1]}";
                        case 3:
                            if (!string.IsNullOrEmpty(asmLoad.FindGlobalLabelName(splits[0])))
                            {
                                return $"{splits[0]}.{splits[1]}..{splits[2]}";
                            }
                            else
                            {
                                return $"{asmLoad.Scope.GlobalLabelName}.{splits[0]}.{splits[1]}.{splits[2]}";
                            }
                        case 4:
                            return labelName;
                        default:
                            throw new Exception($"ラベルの指定名が間違っています。{labelName}");
                    }
                }
            }
            else
            {
                // 通常ラベル指定
                switch (splits.Length)
                {
                    case 1:
                        return $"{asmLoad.Scope.GlobalLabelName}.{splits[0]}";
                    case 2:
                        if (!string.IsNullOrEmpty(asmLoad.FindGlobalLabelName(splits[0])))
                        {
                            return $"{splits[0]}.{splits[1]}";
                        }
                        else
                        {
                            return $"{asmLoad.Scope.GlobalLabelName}.{splits[0]}.{splits[1]}";
                        }
                    case 3:
                        if (splits.Any(m => string.IsNullOrEmpty(m)))
                        {
                            throw new Exception($"ラベルの指定名が間違っています。{labelName}");
                        }
                        return $"{labelName}";
                    default:
                        throw new Exception($"ラベルの指定名が間違っています。{labelName}");
                }
            }
        }

        /// <summary>
        /// ラベル演算子を削除する
        /// </summary>
        /// <param name="labelName"></param>
        /// <returns></returns>
        private static string RemoveOperators(string labelName)
        {
            var atmarkIndex = default(int);
            while ((atmarkIndex = labelName.LastIndexOf(".@")) >= 0 &&
                   AIMath.LabelOperatorStrings.Any(m => labelName.EndsWith(m, StringComparison.CurrentCultureIgnoreCase)))
            {
                labelName = labelName.Substring(0, atmarkIndex);
            }

            return labelName;
        }


        public virtual void Calculation()
        {
            InternalCalculation(AsmLoad, new List<Label>());
        }

        /// <summary>
        /// ラベルの値を計算する
        /// </summary>
        public virtual void Calculation(List<Label> entryLabels)
        {
            InternalCalculation(AsmLoad, entryLabels);
        }

        public virtual void PredictCalculation(UInt16 address)
        {
            Value = new AIValue(address, AIValue.ValueInt32TypeEnum.Hex);
            DataType = DataTypeEnum.Value;
        }

        protected void InternalCalculation(AsmLoad asmLoad, List<Label> entryLabels)
        {
            var asmAddress = default(AsmAddress?);
            if (LineDetailExpansionItem != default)
            {
                asmAddress = LineDetailExpansionItem.Address;
            }
            else if (LineDetailItem != default)
            {
                asmAddress = LineDetailItem.Address;
            }

            InternalCalculation(asmLoad, asmAddress, entryLabels);
        }

        protected void InternalCalculation(AsmLoad asmLoad, AsmAddress? asmAddress, List<Label> entryLabels)
        {
            if (DataType != DataTypeEnum.None)
            {
                return;
            }

            Value = AIMath.Calculation(ValueString, asmLoad, asmAddress, entryLabels);
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
                if (LineItem != default && AsmLoad != default)
                {
                    var labelName = GlobalLabelName == Package.NAME_SPACE_DEFAULT_NAME ? $"{LabelShortName}" : $"{GlobalLabelName}.{LabelShortName}";
                    AsmLoad.AddError(new ErrorLineItem(LineItem, Error.ErrorCodeEnum.I0002, $"{labelName}"));
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

        public static bool IsGlobalLabel(string labelString)
        {
            return CompiledRegexPatternGlobalLabel.IsMatch(labelString);
        }
    }
}
