using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AILZ80ASM.Exceptions
{
    public class InvalidAIValueLabelOperatorException : Exception
    {
        public InvalidAIValueLabelOperatorException(string message)
            : base(message)
        {

        }
    }
}
