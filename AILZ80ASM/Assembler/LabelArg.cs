using AILZ80ASM.AILight;
using AILZ80ASM.Exceptions;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace AILZ80ASM.Assembler
{
    public class LabelArg : Label
    {
        private LineItem CalcLineItem { get; set; }
        private AsmLoad CalculationAsmLoad { get; set; }
        /*
        public LabelArg(string labelName, AsmLoad asmLoad, AsmLoad calcAsmLoad)
            : this(labelName, asmLoad, default(LineItem), calcAsmLoad)
        {
        }

        public LabelArg(string labelName, AsmLoad asmLoad, LineItem calcLineItem, AsmLoad calcAsmLoad)
            : this(labelName, "", asmLoad, calcLineItem, calcAsmLoad)
        {
        }

        public LabelArg(string labelName, string valueString, AsmLoad asmLoad, AsmLoad calcAsmLoad)
            : this(labelName, valueString, asmLoad, default(LineItem), calcAsmLoad)
        {

        }

        public LabelArg(string labelName, string valueString, AsmLoad asmLoad, LineItem calcLineItem, AsmLoad calcAsmLoad)
            : this(labelName, valueString, default(AIValue), asmLoad, calcLineItem, calcAsmLoad)
        {
        }

        public LabelArg(string labelName, string valueString, AIValue aiValue, AsmLoad asmLoad, AsmLoad calcAsmLoad)
            : this(labelName, valueString, aiValue, asmLoad, default(LineItem), calcAsmLoad)
        {

        }

        public LabelArg(string labelName, string valueString, AIValue aiValue, AsmLoad asmLoad, LineItem calcLineItem, AsmLoad calcAsmLoad)
            : this(labelName, valueString, aiValue, asmLoad, calcLineItem, calcAsmLoad, LabelTypeEnum.Arg)
        {
        }*/

        protected LabelArg(string labelName, string valueString, AIValue aiValue, AsmLoad asmLoad, LineItem calcLineItem, AsmLoad calcAsmLoad, LabelTypeEnum labelTypeEnum)
            : base(labelName, valueString, aiValue, asmLoad, labelTypeEnum)
        {
            CalculationAsmLoad = calcAsmLoad;
            CalcLineItem = calcLineItem;
        }

        public override void Calculation()
        {
            try
            {
                InternalCalculation(CalculationAsmLoad);
            }
            catch (ErrorAssembleException ex)
            {
                if (this.CalcLineItem == default)
                {
                    throw;
                }
                else
                {
                    throw new ErrorLineItemException(new ErrorLineItem(this.CalcLineItem, ex));
                }
            }
        }
    }
}
