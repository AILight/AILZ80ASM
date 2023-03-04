using AILZ80ASM.Assembler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AILZ80ASM.Exceptions
{
    public class UnresolvedProgramAddressException : Exception
    {
        public Label TargetLabel { get; set; }

        public UnresolvedProgramAddressException()
            : base()
        {

        }

        public UnresolvedProgramAddressException(Label label)
            : base($"[{label.LabelName}]")
        {
            TargetLabel = label;
        }
    }
}
