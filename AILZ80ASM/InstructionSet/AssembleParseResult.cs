using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AILZ80ASM.InstructionSet
{
    public class AssembleParseResult
    {
        public string Instruction { get; set; }
        public string InstructionForDic { get; set; }
        public Dictionary<string, string> ArgumentDic { get; private set; } = new Dictionary<string, string>();
    }
}
