using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace AILZ80ASM
{
    public class LineDetailItemRepeat : LineDetailItem
    {
        // TODO: ラベルにLASTが使えない仕様になってしまっているので、あとでパーサーを強化して使えるようにする
        private static readonly string RegexPatternRepeatFullStart = @"^\s*Repeat\s+(?<count>.+)\s+LAST\s+(?<last_arg>.+)$";
        private static readonly string RegexPatternRepeatSimpleStart = @"^\s*Repeat\s+(?<count>.+)$";
        private static readonly string RegexPatternRepeatEnd = @"^\s*End\s+Repeat\s*$";

        private static string RepeatCountLabel { get; set; }
        private static string RepeatLastLabel { get; set; }

        private List<LineItem> RepeatLines = new List<LineItem>();

        public LineDetailItemRepeat()
        {

        }

        public static LineDetailItemRepeat Create(string lineString, AsmLoad asmLoad)
        {
            var startMatched = Regex.Match(lineString, RegexPatternRepeatFullStart, RegexOptions.Singleline | RegexOptions.IgnoreCase);
            var startSimpleMatched = Regex.Match(lineString, RegexPatternRepeatSimpleStart, RegexOptions.Singleline | RegexOptions.IgnoreCase);
            var endMatched = Regex.Match(lineString, RegexPatternRepeatEnd, RegexOptions.Singleline | RegexOptions.IgnoreCase);

            if (asmLoad.LineDetailItemRepeat != default)
            {
                if (endMatched.Success)
                {
                    // 終了
                    asmLoad.LineDetailItemRepeat = null;
                }
                else
                {
                    var repeatLines = asmLoad.LineDetailItemRepeat.RepeatLines;
                    var repeatAsmLoad = asmLoad.Clone();
                    repeatAsmLoad.LineDetailItemRepeat = default;

                    repeatLines.Add(new LineItem(lineString, repeatLines.Count + 1, repeatAsmLoad));
                }
                return new LineDetailItemRepeat();
            }
            else
            {
                if (startMatched.Success)
                {
                    var lineDetailItemRepeat = new LineDetailItemRepeat();

                    RepeatCountLabel = startMatched.Groups["count"].Value;
                    RepeatLastLabel = startMatched.Groups["last_arg"].Value;

                    asmLoad.LineDetailItemRepeat = lineDetailItemRepeat;

                    return lineDetailItemRepeat;
                }
                else if (startSimpleMatched.Success)
                {
                    var lineDetailItemRepeat = new LineDetailItemRepeat();

                    RepeatCountLabel = startSimpleMatched.Groups["count"].Value;

                    asmLoad.LineDetailItemRepeat = lineDetailItemRepeat;

                    return lineDetailItemRepeat;

                }

                if (endMatched.Success)
                {
                    throw new ErrorMessageException(Error.ErrorCodeEnum.E1012);
                }
            }
            return default;
        }

        public override void ExpansionItem(AsmLoad assembleLoad)
        {
            // リピート数が設定されているものを処理する
            if (!string.IsNullOrEmpty(RepeatCountLabel))
            {
                var count = AIMath.ConvertToUInt16(RepeatCountLabel, assembleLoad);
                var last = string.IsNullOrEmpty(RepeatLastLabel) ? 0 : AIMath.ConvertToUInt16(RepeatLastLabel, assembleLoad);

                foreach (var repeatCounter in Enumerable.Range(0, count))
                {
                    foreach (var item in RepeatLines)
                    {

                    }
                }
            }

            base.ExpansionItem(assembleLoad);
        }
    }
}
