using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AILZ80ASM.InstructionSet
{
    public class AssembleException : Exception
    {
        public AssembleException()
        {

        }

        public AssembleException(string message)
            : base(message)
        {

        }
    }
}
