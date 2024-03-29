﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AILZ80ASM.InstructionSet
{
    public class InstructionRegister
    {
        public enum InstructionRegisterModeEnum
        {
            Register,
            RelativeAddress8Bit,
            Value0Bit,
            Value3Bit,
            Value8Bit,
            Value8BitSigned,
            Value16Bit,
            InterruptModeValue,
            RestartValue,
        }

        public string MnemonicRegisterName { get; set; }
        public string MnemonicBitName { get; set; }
        public InstructionRegisterModeEnum InstructionRegisterMode { get; set; }
        public InstructionRegisterItem[] InstructionRegisterItems { get; set; }
    }
}
