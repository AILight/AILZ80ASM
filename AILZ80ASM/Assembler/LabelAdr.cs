using AILZ80ASM.AILight;
using AILZ80ASM.Exceptions;
using AILZ80ASM.LineDetailItems;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace AILZ80ASM.Assembler
{
    public class LabelAdr : Label
    {
        public LabelAdr(string labelName, AsmLoad asmLoad)
            : base(labelName, asmLoad, LabelTypeEnum.Adr)
        {
        }

        // テスト用、将来private化したい
        public LabelAdr(string labelName, string valueString, AsmLoad asmLoad)
            : base(labelName, valueString, asmLoad, LabelTypeEnum.Adr)
        {
        }

        public LabelAdr(LineDetailItem lineDetailItem, AsmLoad asmLoad)
            : base(lineDetailItem, asmLoad, LabelTypeEnum.Adr)
        {

        }
    }
}
