using AILZ80ASM.Assembler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AILZ80ASM.LineDetailItems
{
    public class LineDetailItemMacroDefineModern : LineDetailItemMacroDefine
    {
        private static readonly string RegexPatternMacroStart = @"^\s*Macro\s+(?<macro_name>[a-zA-Z0-9_\(\)]+)($|\s+(?<args>.+)$)";
        private static readonly string RegexPatternMacroEnd = @"^\s*End\s+Macro\s*$";

        private LineDetailItemMacroDefineModern(LineItem lineItem, AsmLoad asmLoad)
            : base(lineItem, asmLoad)
        {

        }

        public static LineDetailItemMacroDefineModern Create(LineItem lineItem, AsmLoad asmLoad)
        {
            return (LineDetailItemMacroDefineModern)Create(new LineDetailItemMacroDefineModern(lineItem, asmLoad), RegexPatternMacroStart, RegexPatternMacroEnd, asmLoad);
        }
    }
}
