using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AILZ80ASM.Instructions
{
    public class InstructionSet
    {
        public char[] SplitChars { get; set; }
        public string[] Brackets { get; set; }
        public string[] RegisterNames { get; set; }
        public InstructionRegister[] InstructionRegisters { get; set; }
        public InstructionItem[] InstructionItems { get; set; }

        public void MakeDataSet()
        {
            foreach (var item in InstructionItems)
            {
                item.MakeDataSet(SplitChars, Brackets, InstructionRegisters);
            }
        }
    }
}
