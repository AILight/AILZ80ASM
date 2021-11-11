using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AILZ80ASM.Instructions
{
    public class InstructionRegister
    {
        public enum InstructionRegisterModeEnum
        {
            Register,
            RelativeAddress8Bit,
            Value3Bit,
            Value8Bit,
            Value16Bit,
        }

        public string MnemonicRegisterName { get; set; }
        public string MnemonicBitName { get; set; }
        public InstructionRegisterModeEnum InstructionRegisterMode { get; set; }
        public InstructionRegisterItem[] InstructionRegisterItems { get; set; }
        public string[] ExclusionItems { get; set; }
    }
}
