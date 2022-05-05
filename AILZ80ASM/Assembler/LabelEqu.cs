using AILZ80ASM.AILight;
using AILZ80ASM.Exceptions;
using AILZ80ASM.LineDetailItems;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace AILZ80ASM.Assembler
{
    public class LabelEqu : Label
    {
        /*
        public LabelEqu(string labelName, AsmLoad asmLoad)
            : base(labelName, asmLoad, LabelTypeEnum.Equ)
        {
        }
        */

        /*
        public LabelEqu(string labelName, string valueString, AsmLoad asmLoad)
            :base(labelName, valueString, asmLoad, LabelTypeEnum.Equ)
        {
        }
        */

        public LabelEqu(LineDetailItemEqual lineDetailItemEqual, AsmLoad asmLoad)
            : base(lineDetailItemEqual, asmLoad, LabelTypeEnum.Equ)
        {
        }
    }
}
