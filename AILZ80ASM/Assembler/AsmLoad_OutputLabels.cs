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
        public void OutputLabels(StreamWriter streamWriter)
        {
            var globalLabels = this.Scope.Labels.GroupBy(m => m.GlobalLabelName).Select(m => m.Key);
            var globalLabelMode = globalLabels.Count() > 1;

            foreach (var globalLabelName in globalLabels)
            {
                if (globalLabelMode)
                {
                    streamWriter.WriteLine($"[{globalLabelName}]");
                }
                foreach (var label in this.Scope.Labels.Where(m => m.DataType == Label.DataTypeEnum.Value && m.GlobalLabelName == globalLabelName))
                {
                    OutputLabelForShortName(label, streamWriter);
                }
                streamWriter.WriteLine();
            }

            if (globalLabelMode)
            {
                foreach (var label in this.Scope.Labels.Where(m => m.DataType == Label.DataTypeEnum.Value))
                {
                    OutputLabelForFullName(label, streamWriter);
                }
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
    }
}
