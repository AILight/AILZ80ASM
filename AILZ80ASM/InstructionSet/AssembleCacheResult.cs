using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AILZ80ASM.InstructionSet
{
    public class AssembleCacheResult
    {
        public InstructionItem InstructionItem { get; set; }
        public Match PreAssembleMatched { get; set; }
    }
}
