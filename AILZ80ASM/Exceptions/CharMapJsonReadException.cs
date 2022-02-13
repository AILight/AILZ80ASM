using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AILZ80ASM.Exceptions
{
    public class CharMapJsonReadException : Exception
    {
        public long? LineNumber { get; private set; }

        public CharMapJsonReadException(long? lineNumber)
            : this("", lineNumber)
        {
        }
        
        public CharMapJsonReadException(string message)
            : base(message)
        {
        }

        public CharMapJsonReadException(string message, long? lineNumber)
            : base(message)
        {
            LineNumber = lineNumber;
        }
    }
}
