using System;

namespace AILZ80ASM.Assembler
{
    public struct AsmORG
    {
        public UInt16 ProgramAddress { get; set; }
        public UInt32? OutputAddress { get; set; }
        public byte? FillByte { get; set; }

        public AsmORG(UInt16 programAddress, UInt32? outputAddress, byte? fillByte)
        {
            ProgramAddress = programAddress;
            OutputAddress = outputAddress;
            FillByte = fillByte;
        }
    }
}
