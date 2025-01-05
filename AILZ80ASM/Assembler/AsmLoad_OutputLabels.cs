using AILZ80ASM.AILight;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace AILZ80ASM.Assembler
{
    public partial class AsmLoad
    {
        public void OutputLabels(StreamWriter streamWriter, AsmEnum.SymbolFormatEnum symbolMode)
        {
            var globalLabels = this.Scope.Labels.GroupBy(m => m.GlobalLabelName).Select(m => m.Key);
            var globalLabelMode = globalLabels.Count() > 1;

            switch (symbolMode)
            {
                case AsmEnum.SymbolFormatEnum.Minimal_Equ:
                    OutputMinimalEqualLabels(streamWriter);
                    break;
                case AsmEnum.SymbolFormatEnum.Normal:
                    foreach (var globalLabelName in globalLabels)
                    {
                        if (globalLabelMode)
                        {
                            streamWriter.WriteLine($"[{globalLabelName}]");
                        }
                        foreach (var label in this.Scope.Labels.Where(m => m.DataType == Label.DataTypeEnum.Value &&
                                                                            m.GlobalLabelName == globalLabelName &&
                                                                            m.LabelLevel != Label.LabelLevelEnum.AnonLabel))
                        {
                            OutputLabelForShortName(label, streamWriter);
                        }
                        streamWriter.WriteLine();
                    }

                    if (globalLabelMode)
                    {
                        foreach (var label in this.Scope.Labels.Where(m => m.DataType == Label.DataTypeEnum.Value && m.LabelLevel != Label.LabelLevelEnum.AnonLabel))
                        {
                            OutputLabelForFullName(label, streamWriter);
                        }
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }


        }

        private void OutputLabelForShortName(Label label, StreamWriter streamWriter)
        {
            if (label.Value.ValueType == AIValue.ValueTypeEnum.Bool)
            {
                streamWriter.WriteLine($"{label.Value.ConvertTo<bool>()} {label.LabelShortName}");
            }
            else
            {
                streamWriter.WriteLine($"{label.Value.ConvertTo<int>():X4} {label.LabelShortName}");
            }
        }

        private void OutputLabelForFullName(Label label, StreamWriter streamWriter)
        {
            if (label.Value.ValueType == AIValue.ValueTypeEnum.Bool)
            {
                streamWriter.WriteLine($"{label.Value.ConvertTo<bool>()} {label.LabelFullName}");
            }
            else
            {
                streamWriter.WriteLine($"{label.Value.ConvertTo<int>():X4} {label.LabelFullName}");
            }
        }

        public void OutputMinimalEqualLabels(StreamWriter streamWriter)
        {
            var labelMaxLength = this.AssembleOption.TabSize;
            var globalLabels = this.Scope.Labels.GroupBy(m => m.GlobalLabelName).Select(m => m.Key);
            var globalLabelMode = globalLabels.Count() > 1;

            foreach (var globalLabelName in globalLabels)
            {
                var labels = this.Scope.Labels.Where(m => m.DataType == Label.DataTypeEnum.Value && m.GlobalLabelName == globalLabelName);
                foreach (var label in this.Scope.Labels.Where(m => m.DataType == Label.DataTypeEnum.Value &&
                                                    m.GlobalLabelName == globalLabelName &&
                                                    m.LabelLevel != Label.LabelLevelEnum.AnonLabel))
                {
                    if (globalLabelMode)
                    {
                        if (label.LabelLevel == Label.LabelLevelEnum.SubLabel)
                        {
                            continue;
                        }

                        var labelName = $"{label.LabelFullName}";
                        if (label.Value.TryParse<int>(out var result))
                        {
                            var equValue = $"${result:X4}";
                            if (AIMath.TryParse(label.Value.OriginalValue, out var tmpAIValue) &&
                                label.Value.Equals(tmpAIValue))
                            {
                                equValue = label.Value.OriginalValue;
                            }
                            var length = labelName.Length < labelMaxLength ? labelMaxLength : labelName.Length + 1;
                            streamWriter.WriteLine($"{labelName.PadRight(length)}equ {equValue}");

                        }
                        else
                        {
                            var length = labelName.Length < labelMaxLength ? labelMaxLength : labelName.Length + 1;
                            streamWriter.WriteLine($"{labelName.PadRight(length)}equ {label.Value.OriginalValue}");
                        }
                    }
                    else
                    {
                        var labelName = $"{label.LabelShortName}";
                        if (label.Value.TryParse<int>(out var result))
                        {
                            var equValue = $"${result:X4}";
                            if (AIMath.TryParse(label.Value.OriginalValue, out var tmpAIValue) &&
                                label.Value.Equals(tmpAIValue))
                            {
                                equValue = label.Value.OriginalValue;
                            }
                            var length = labelName.Length < labelMaxLength ? labelMaxLength : labelName.Length + 1;
                            streamWriter.WriteLine($"{labelName.PadRight(length)}equ {equValue}");

                        }
                        else
                        {
                            var length = labelName.Length < labelMaxLength ? labelMaxLength : labelName.Length + 1;
                            streamWriter.WriteLine($"{labelName.PadRight(length)}equ {label.Value.OriginalValue}");
                        }
                    }
                }



                /*
                // addressModeで出し分けをする
                var equLabels = labels.Where(m => ((m.LabelLevel == Label.LabelLevelEnum.Label || m.LabelLevel == Label.LabelLevelEnum.SubLabel) && (m.LabelType == Label.LabelTypeEnum.Equ || m.LabelType == Label.LabelTypeEnum.Adr)));
                // ラベルの最大長を求める
                labelMaxLength = labels.Max(m => m.LabelShortName.Length + 2);  // コロンとスペース分
                try
                {
                    labelMaxLength += (this.AssembleOption.TabSize - (labelMaxLength % this.AssembleOption.TabSize));
                }
                catch { }

                // EQU
                foreach (var label in equLabels)
                {
                    var labelName = $"{label.LabelShortName}";
                    if (label.Value.TryParse<int>(out var result))
                    {
                        var equValue = $"${result:X4}";
                        if (AIMath.TryParse(label.Value.OriginalValue, out var tmpAIValue) &&
                            label.Value.Equals(tmpAIValue))
                        {
                            equValue = label.Value.OriginalValue;
                        }
                        streamWriter.WriteLine($"{labelName.PadRight(labelMaxLength)}equ {equValue}");

                    }
                    else
                    {
                        streamWriter.WriteLine($"{labelName.PadRight(labelMaxLength)}equ {label.Value.OriginalValue}");
                    }
                }
                if (globalLabels.Any())
                {
                    streamWriter.WriteLine();
                }
                */
            }
        }
    }
}
