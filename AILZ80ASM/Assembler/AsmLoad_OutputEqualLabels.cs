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
        public void OutputEqualLabels(StreamWriter streamWriter)
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
                    var equValue = $"${label.Value.ConvertTo<int>():X4}";
                    if (AIMath.TryParse(label.Value.OriginalValue, out var tmpAIValue) &&
                        label.Value.Equals(tmpAIValue))
                    {
                        equValue = label.Value.OriginalValue;
                    }
                    streamWriter.WriteLine($"{labelName.PadRight(labelMaxLength)}equ {equValue}");
                }
                if (globalLabels.Any())
                {
                    streamWriter.WriteLine();
                }
            }
        }
    }
}
