using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AILZ80ASM
{
    public struct AsmLength
    {
        public AsmLength(int length)
        {
            Program = (UInt16)length;
            Output = (UInt32)length;
        }

        public AsmLength(UInt16 programLength, UInt32 outputLength)
        {
            Program = programLength;
            Output = outputLength;
        }

        public UInt16 Program { get; set; }
        public UInt32 Output { get; set; }
    }
}
