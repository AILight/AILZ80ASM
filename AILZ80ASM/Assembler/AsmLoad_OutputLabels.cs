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
                    streamWriter.WriteLine($"{label.Value.ConvertTo<int>():X4} {label.LabelShortName}");
                }
                streamWriter.WriteLine();
            }

            if (globalLabelMode)
            {
                foreach (var label in this.Scope.Labels.Where(m => m.DataType == Label.DataTypeEnum.Value))
                {
                    streamWriter.WriteLine($"{label.Value.ConvertTo<int>():X4} {label.LabelFullName}");
                }
            }
        }

    }
}
