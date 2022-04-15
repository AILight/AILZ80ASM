using AILZ80ASM.Assembler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AILZ80ASM.LineDetailItems
{
    public class LineDetailItemMacroDefineCompatible : LineDetailItemMacroDefine
    {
        private static readonly string RegexPatternMacroStart = @"^(?<macro_name>[a-zA-Z0-9_\(\)]+)\s+Macro($|\s+(?<args>.+)$)";
        private static readonly string RegexPatternMacroEnd = @"^\s*Endm\s*$";

        private LineDetailItemMacroDefineCompatible(LineItem lineItem, AsmLoad asmLoad)
            : base(lineItem, asmLoad)
        {

        }

        public static LineDetailItemMacroDefineCompatible Create(LineItem lineItem, AsmLoad asmLoad)
        {
            return (LineDetailItemMacroDefineCompatible)Create(new LineDetailItemMacroDefineCompatible(lineItem, asmLoad), RegexPatternMacroStart, RegexPatternMacroEnd, asmLoad);
        }
    }
}
