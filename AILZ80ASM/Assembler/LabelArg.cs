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

        public LabelArg(string labelName, AsmLoad asmLoad, AsmLoad calcAsmLoad)
            : this(labelName, asmLoad, default(LineItem), calcAsmLoad)
        {
        }

        public LabelArg(string labelName, AsmLoad asmLoad, LineItem calcLineItem, AsmLoad calcAsmLoad)
            : base(labelName, asmLoad, LabelTypeEnum.Arg)
        {
            CalculationAsmLoad = calcAsmLoad;
            CalcLineItem = calcLineItem;
        }

        public LabelArg(string labelName, string valueString, AsmLoad asmLoad, AsmLoad calcAsmLoad)
            : this(labelName, valueString, asmLoad, default(LineItem), calcAsmLoad)
        {

        }

        public LabelArg(string labelName, string valueString, AsmLoad asmLoad, LineItem calcLineItem, AsmLoad calcAsmLoad)
            : base(labelName, valueString, asmLoad, LabelTypeEnum.Arg)
        {
            CalculationAsmLoad = calcAsmLoad;
            CalcLineItem = calcLineItem;
        }

        public LabelArg(string labelName, string valueString, AIValue aiValue, AsmLoad asmLoad, AsmLoad calcAsmLoad)
            : this(labelName, valueString, aiValue, asmLoad, default(LineItem), calcAsmLoad)
        {

        }

        public LabelArg(string labelName, string valueString, AIValue aiValue, AsmLoad asmLoad, LineItem calcLineItem, AsmLoad calcAsmLoad)
            : base(labelName, valueString, aiValue, asmLoad, LabelTypeEnum.Arg)
        {
            CalculationAsmLoad = calcAsmLoad;
            CalcLineItem = calcLineItem;
        }

        /*
        public LabelArg(LineDetailExpansionItemOperation lineDetailExpansionItemOperation, AsmLoad asmLoad)
            : base(lineDetailExpansionItemOperation, asmLoad, LabelTypeEnum.Arg)
        {
        }
        */

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
