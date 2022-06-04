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
        private AsmAddress? CalculationAsmAddress { get; set; }
        private AsmLoad CalculationAsmLoad { get; set; }

        protected LabelArg(string labelName, string valueString, AIValue aiValue, AsmLoad asmLoad, LineItem calcLineItem, AsmLoad calcAsmLoad, AsmAddress? calcAsmAddress, LabelTypeEnum labelTypeEnum)
            : base(labelName, valueString, aiValue, asmLoad, labelTypeEnum)
        {
            CalcLineItem = calcLineItem;
            CalculationAsmLoad = calcAsmLoad;
            CalculationAsmAddress = calcAsmAddress;
        }

        public override void Calculation()
        {
            try
            {
                InternalCalculation(CalculationAsmLoad, CalculationAsmAddress);
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
