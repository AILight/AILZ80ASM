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
        public void SettingComandlineOptionDefineLabels()
        {
            if (this.AssembleOption.DefineLabels == default)
            {
                return;
            }

            foreach (var defineLabel in this.AssembleOption.DefineLabels)
            {
                var value = defineLabel.Split('=').Select(m => m.Trim()).ToArray();

                var label = default(Label);
                switch (value.Length)
                {
                    case 1:
                        label = new LabelEqu(value[0], "#TRUE", this);
                        break;
                    case 2:
                        label = new LabelEqu(value[0], value[1], this);
                        break;
                    default:
                        continue;
                }
                this.AddLabel(label);
            }
        }
    }
}
