using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AILZ80ASM.Exceptions
{
    public class InvalidCommanLineOptionException : Exception
    {
        public InvalidCommanLineOptionException(string message)
            : base(message)
        {

        }
    }
}
