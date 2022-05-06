using System;

namespace AILZ80ASM.Assembler
{
    public struct AsmAddress
    {
        public static readonly AsmAddress MaxValue = new AsmAddress(UInt16.MaxValue, UInt32.MaxValue);

        public AsmAddress(UInt16 programAddress)
        {
            Program = programAddress;
            Output = default(UInt32?);
        }

        public AsmAddress(UInt16 programAddress, UInt32 outputAddress)
        {
            Program = programAddress;
            Output = outputAddress;
        }

        public AsmAddress(AsmAddress address, AsmLength length)
        {
            Program = (UInt16)(address.Program + length.Program);
            if (address.Output.HasValue)
            {
                Output = (UInt32)(address.Output + length.Output);
            }
            else
            {
                Output = default(UInt32?);
            }
        }

        public UInt16 Program { get; set; }
        public UInt32? Output { get; set; }
    }
}
