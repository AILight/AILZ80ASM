using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AILZ80ASM.InstructionSet
{
    public class AssembleResult
    {
        public InstructionItem InstructionItem { get; set; }
        public AssembleParseResult ParseResult { get; set; }
        public Match PreAssembleMatched { get; set; }
        public string[] AssembledOPCodes { get; set; }
        public AssembleException InnerAssembleException { get; set; }

    }
}
