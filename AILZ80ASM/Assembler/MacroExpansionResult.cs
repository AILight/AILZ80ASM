using AILZ80ASM.Assembler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AILZ80ASM
{
    public class MacroExpansionResult
    {
        public LineItem[] LineItems { get; set; }
        public Label[] ArgumetLabels { get; set; }
    }
}
