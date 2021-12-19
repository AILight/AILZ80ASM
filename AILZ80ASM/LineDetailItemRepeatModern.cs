using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AILZ80ASM
{
    public class LineDetailItemRepeatModern : LineDetailItemRepeat
    {
        // TODO: ラベルにLASTが使えない仕様になってしまっているので、あとでパーサーを強化して使えるようにする
        private static readonly string RegexPatternRepeatFullStart = @"^\s*Repeat\s+(?<count>.+)\s+LAST\s+(?<last_arg>.+)$";
        private static readonly string RegexPatternRepeatSimpleStart = @"^\s*Repeat\s+(?<count>.+)$";
        private static readonly string RegexPatternRepeatEnd = @"^\s*End\s+Repeat\s*$";

        private string RepeatCountLabel { get; set; }
        private string RepeatLastLabel { get; set; }

        private readonly List<LineItem> RepeatLines = new List<LineItem>();
        private int RepeatNestedCount { get; set; } = 0;

        private LineDetailItemRepeatModern(LineItem lineItem, AsmLoad asmLoad)
            : base(lineItem, asmLoad)
        {

        }

        public static LineDetailItemRepeatModern Create(LineItem lineItem, AsmLoad asmLoad)
        {
            return (LineDetailItemRepeatModern)Create(new LineDetailItemRepeatModern(lineItem, asmLoad), RegexPatternRepeatFullStart, RegexPatternRepeatSimpleStart, RegexPatternRepeatEnd, asmLoad);
        }
    }
}
