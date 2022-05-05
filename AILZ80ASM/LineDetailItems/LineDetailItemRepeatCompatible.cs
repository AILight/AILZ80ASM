using AILZ80ASM.Assembler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AILZ80ASM.LineDetailItems
{
    public class LineDetailItemRepeatCompatible : LineDetailItemRepeat
    {
        // TODO: ラベルにLASTが使えない仕様になってしまっているので、あとでパーサーを強化して使えるようにする
        private static readonly string RegexPatternRepeatFullStart = @"^\s*REPT\s+(?<count>.+)\s+LAST\s+(?<last_arg>.+)$";
        private static readonly string RegexPatternRepeatSimpleStart = @"^\s*REPT\s+(?<count>.+)$";
        private static readonly string RegexPatternRepeatEnd = @"^\s*ENDM\s*$";

        private LineDetailItemRepeatCompatible(LineItem lineItem, AsmLoad asmLoad)
            : base(lineItem, asmLoad)
        {

        }

        public static LineDetailItemRepeatCompatible Create(LineItem lineItem, AsmLoad asmLoad)
        {
            return (LineDetailItemRepeatCompatible)Create(new LineDetailItemRepeatCompatible(lineItem, asmLoad), RegexPatternRepeatFullStart, RegexPatternRepeatSimpleStart, RegexPatternRepeatEnd, asmLoad);
        }
    }
}
