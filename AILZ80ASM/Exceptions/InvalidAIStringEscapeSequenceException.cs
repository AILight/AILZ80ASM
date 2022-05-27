using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AILZ80ASM.Exceptions
{
    public class InvalidAIStringEscapeSequenceException : InvalidAIStringException
    {
        public string Value { get; set; }

        public InvalidAIStringEscapeSequenceException(string message, string value)
            : base(message)
        {
            Value = value;
        }
    }
}
