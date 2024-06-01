using AILZ80ASM.AILight;
using AILZ80ASM.LineDetailItems;
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
        public void OutputTags(StreamWriter streamWriter)
        {
            var globalLabels = this.Scope.Labels.GroupBy(m => m.GlobalLabelName).Select(m => m.Key);
            var globalLabelMode = globalLabels.Count() > 1;

            // GlobalLabelは重複登録ができるので出力対象外とする
            // LabelName
            foreach (var item in this.Scope.Labels.Where(m => (m.LabelLevel == Label.LabelLevelEnum.Label || m.LabelLevel == Label.LabelLevelEnum.SubLabel) && m.LineItem != default && m.LineItem.FileInfo != default))
            {
                var lineItem = item.LineItem;
                if (globalLabelMode)
                {
                    streamWriter.WriteLine($"{lineItem.FileInfo.Name}({lineItem.LineIndex}) : {item.LabelFullName}");
                }
                streamWriter.WriteLine($"{lineItem.FileInfo.Name}({lineItem.LineIndex}) : {item.LabelShortName}");
            }
        }
    }
}
