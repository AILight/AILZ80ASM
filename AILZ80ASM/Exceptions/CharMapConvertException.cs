using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AILZ80ASM.Exceptions
{
    public class CharMapConvertException : Exception
    {
        public CharMapConvertException(string message)
            : base(message)
        {
        }
    }
}
