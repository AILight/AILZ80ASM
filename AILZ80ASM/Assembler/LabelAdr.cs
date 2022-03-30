using AILZ80ASM.AILight;
using AILZ80ASM.Exceptions;
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

        public LabelAdr(string labelName, string valueString, AsmLoad asmLoad)
            : base(labelName, valueString, asmLoad, LabelTypeEnum.Adr)
        {
        }

        public LabelAdr(LineDetailExpansionItemOperation lineDetailExpansionItemOperation, AsmLoad asmLoad)
            : base(lineDetailExpansionItemOperation, asmLoad, LabelTypeEnum.Adr)
        {
        }

    }
}
