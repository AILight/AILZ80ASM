using AILZ80ASM.AILight;
using AILZ80ASM.Exceptions;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace AILZ80ASM.Assembler
{
    public class LabelMacroArg : LabelArg
    {
        public LabelMacroArg(string labelName, AsmLoad asmLoad, AsmLoad calcAsmLoad, AsmAddress? calcAsmAddress)
            : this(labelName, asmLoad, default(LineItem), calcAsmLoad, calcAsmAddress)
        {
        }

        public LabelMacroArg(string labelName, AsmLoad asmLoad, LineItem calcLineItem, AsmLoad calcAsmLoad, AsmAddress? calcAsmAddress)
            : this(labelName, "", asmLoad, calcLineItem, calcAsmLoad, calcAsmAddress)
        {
        }

        public LabelMacroArg(string labelName, string valueString, AsmLoad asmLoad, AsmLoad calcAsmLoad, AsmAddress? calcAsmAddress)
            : this(labelName, valueString, asmLoad, default(LineItem), calcAsmLoad, calcAsmAddress)
        {

        }

        public LabelMacroArg(string labelName, string valueString, AsmLoad asmLoad, LineItem calcLineItem, AsmLoad calcAsmLoad, AsmAddress? calcAsmAddress)
            : this(labelName, valueString, default(AIValue), asmLoad, calcLineItem, calcAsmLoad, calcAsmAddress)
        {
        }

        public LabelMacroArg(string labelName, string valueString, AIValue aiValue, AsmLoad asmLoad, AsmLoad calcAsmLoad, AsmAddress? calcAsmAddress)
            : this(labelName, valueString, aiValue, asmLoad, default(LineItem), calcAsmLoad, calcAsmAddress)
        {

        }

        public LabelMacroArg(string labelName, string valueString, AIValue aiValue, AsmLoad asmLoad, LineItem calcLineItem, AsmLoad calcAsmLoad, AsmAddress? calcAsmAddress)
            : base(labelName, valueString, aiValue, asmLoad, calcLineItem, calcAsmLoad, calcAsmAddress, LabelTypeEnum.MacroArg)
        {
        }
    }
}
