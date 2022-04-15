using AILZ80ASM.AILight;
using AILZ80ASM.Exceptions;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace AILZ80ASM.Assembler
{
    public class LabelArg : Label
    {

        public LabelArg(string labelName, AsmLoad asmLoad)
            : base(labelName, asmLoad, LabelTypeEnum.Arg)
        {
        }

        public LabelArg(string labelName, string valueString, AsmLoad asmLoad)
            : base(labelName, valueString, asmLoad, LabelTypeEnum.Arg)
        {
        }

        /*
        public LabelArg(LineDetailExpansionItemOperation lineDetailExpansionItemOperation, AsmLoad asmLoad)
            : base(lineDetailExpansionItemOperation, asmLoad, LabelTypeEnum.Arg)
        {
        }
        */
    }
}
