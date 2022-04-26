using System;

namespace AILZ80ASM.Assembler
{
    public class AsmResult
    {
        public AsmAddress Address { get; set; }
        public byte[] Data { get; set; }
        public LineItem LineItem { get; set; }

        public AsmResult()
        {
        }
    }
}
