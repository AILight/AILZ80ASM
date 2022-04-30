using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AILZ80ASM.Exceptions
{
    public class InvalidAIMathException : Exception
    {
        public InvalidAIMathException()
        {

        }

        public InvalidAIMathException(string message)
            : base(message)
        {

        }
    }
}
