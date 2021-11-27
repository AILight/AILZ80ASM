using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AILZ80ASM.InstructionSet
{
    public class AssembleOutOfRangeException : AssembleException
    {
        public InstructionRegister.InstructionRegisterModeEnum InstructionRegisterMode { get; private set; }
        public int Value { get; private set; }

        public AssembleOutOfRangeException(InstructionRegister.InstructionRegisterModeEnum instructionRegisterMode, int value, string message)
            : base(message)
        {
            InstructionRegisterMode = instructionRegisterMode;
            Value = value;
        }

    }
}
