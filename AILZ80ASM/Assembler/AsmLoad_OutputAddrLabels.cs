using AILZ80ASM.AILight;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AILZ80ASM.Assembler
{
    public partial class AsmLoad
    {
        public void OutputAddrLabels(StreamWriter streamWriter)
        {
            var labelMaxLength = this.AssembleOption.TabSize;
            var globalLabels = this.Scope.Labels.GroupBy(m => m.GlobalLabelName).Select(m => m.Key);
            var globalLabelMode = globalLabels.Count() > 1;
            streamWriter.WriteLine("#pragma once");
            streamWriter.WriteLine();

            foreach (var globalLabelName in globalLabels)
            {
                if (globalLabelMode)
                {
                    streamWriter.WriteLine();
                    streamWriter.WriteLine($"[{globalLabelName}]");
                    streamWriter.WriteLine();
                }

                var labels = this.Scope.Labels.Where(m => m.DataType == Label.DataTypeEnum.Value && m.GlobalLabelName == globalLabelName);
                // addressModeで出し分けをする
                var equLabels = labels.Where(m => (m.LabelLevel == Label.LabelLevelEnum.Label && (m.LabelType == Label.LabelTypeEnum.Equ)));
                // addressModeがTrueの時だけ使う
                var orgLabels = labels.Where(m => m.LabelType == Label.LabelTypeEnum.Adr).OrderBy(m => m.Value.ConvertTo<int>()).Select(m => m.Value).Distinct();

                // ラベルの最大長を求める
                labelMaxLength = labels.Max(m => m.LabelName.Length + 2);

                // EQU
                foreach (var label in equLabels)
                {
                    var labelName = $"{label.LabelName}:";
                    var equValue = $"${label.Value.ConvertTo<UInt32>():X4}";
                    if (AIMath.TryParse(label.Value.OriginalValue, out var tmpAIValue) &&
                        label.Value.Equals(tmpAIValue))
                    {
                        equValue = label.Value.OriginalValue;
                    }
                    streamWriter.WriteLine($"{labelName.PadRight(labelMaxLength)}equ {equValue}");

                    // sub equ
                    foreach (var item in labels.Where(m => m.LabelName == label.LabelName && m.LabelLevel == Label.LabelLevelEnum.SubLabel && (m.LabelType == Label.LabelTypeEnum.Equ)))
                    {
                        var subLabelName = $".{item.SubLabelName}";
                        var subEquValue = $"${item.Value.ConvertTo<UInt32>():X4}";
                        if (AIMath.TryParse(item.Value.OriginalValue, out var subTmpAIValue) &&
                            item.Value.Equals(subTmpAIValue))
                        {
                            subEquValue = item.Value.OriginalValue;
                        }
                        streamWriter.WriteLine($"{subLabelName.PadRight(labelMaxLength)}equ {subEquValue}");
                    }
                }
                if (globalLabels.Any())
                {
                    streamWriter.WriteLine();
                }

                // ORG
                var saveAddress = int.MaxValue;
                foreach (var address in orgLabels.Select(m => m.ConvertTo<int>()))
                {
                    foreach (var label in labels.Where(m => m.Value.ConvertTo<int>() == address && m.LabelLevel == Label.LabelLevelEnum.Label && m.LabelType == Label.LabelTypeEnum.Adr))
                    {
                        if (saveAddress != address)
                        {
                            // ORG
                            streamWriter.WriteLine();
                            streamWriter.WriteLine($"{new string(' ', labelMaxLength)}org ${address:X4}");
                            saveAddress = address;
                        }

                        // Add Label
                        var labelName = $"{label.LabelName}:";
                        streamWriter.WriteLine($"{labelName.PadRight(labelMaxLength)}");

                        // sub equ
                        foreach (var item in labels.Where(m => m.LabelName == label.LabelName && m.LabelLevel == Label.LabelLevelEnum.SubLabel && m.LabelType == Label.LabelTypeEnum.Equ))
                        {
                            var subLabelName = $".{item.SubLabelName}";
                            var equValue = $"${item.Value.ConvertTo<UInt32>():X4}";
                            if (AIMath.TryParse(item.Value.OriginalValue, out var tmpValue) && item.Value.Equals(tmpValue))
                            {
                                equValue = item.Value.OriginalValue;
                            }

                            streamWriter.WriteLine($"{subLabelName.PadRight(labelMaxLength)}equ {equValue} ");
                        }

                        // SubAddress
                        foreach (var item in labels.Where(m => m.LabelName == label.LabelName && m.LabelLevel == Label.LabelLevelEnum.SubLabel && m.LabelType == Label.LabelTypeEnum.Adr))
                        {
                            var itemAddress = item.Value.ConvertTo<int>();
                            if (saveAddress != itemAddress)
                            {
                                // ORG
                                streamWriter.WriteLine();
                                streamWriter.WriteLine($"{new string(' ', labelMaxLength)}org ${address:X4}");
                                saveAddress = itemAddress;
                            }

                            var subLabelName = $".{item.SubLabelName}";
                            streamWriter.WriteLine($"{subLabelName.PadRight(16)}");
                        }
                    }
                }
                if (orgLabels.Any())
                {
                    streamWriter.WriteLine();
                }
            }
        }

    }
}
