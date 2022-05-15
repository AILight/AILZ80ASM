using AILZ80ASM.AILight;
using AILZ80ASM.Exceptions;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace AILZ80ASM.Assembler
{
    public class LabelFunctionArg : LabelArg
    {
        public LabelFunctionArg(string labelName, AsmLoad asmLoad, AsmLoad calcAsmLoad)
            : this(labelName, asmLoad, default(LineItem), calcAsmLoad)
        {
        }

        public LabelFunctionArg(string labelName, AsmLoad asmLoad, LineItem calcLineItem, AsmLoad calcAsmLoad)
            : this(labelName, "", asmLoad, calcLineItem, calcAsmLoad)
        {
        }

        public LabelFunctionArg(string labelName, string valueString, AsmLoad asmLoad, AsmLoad calcAsmLoad)
            : this(labelName, valueString, asmLoad, default(LineItem), calcAsmLoad)
        {

        }

        public LabelFunctionArg(string labelName, string valueString, AsmLoad asmLoad, LineItem calcLineItem, AsmLoad calcAsmLoad)
            : this(labelName, valueString, default(AIValue), asmLoad, calcLineItem, calcAsmLoad)
        {
        }

        public LabelFunctionArg(string labelName, string valueString, AIValue aiValue, AsmLoad asmLoad, AsmLoad calcAsmLoad)
            : this(labelName, valueString, aiValue, asmLoad, default(LineItem), calcAsmLoad)
        {

        }

        public LabelFunctionArg(string labelName, string valueString, AIValue aiValue, AsmLoad asmLoad, LineItem calcLineItem, AsmLoad calcAsmLoad)
            : base(labelName, valueString, aiValue, asmLoad, calcLineItem, calcAsmLoad, LabelTypeEnum.FunctionArg)
        {
        }
    }
}
