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

        private string RepeatCountLabel { get; set; }
        private string RepeatLastLabel { get; set; }

        private readonly List<LineItem> RepeatLines = new List<LineItem>();

        public LineDetailItemRepeat(LineItem lineItem, AsmLoad asmLoad)
            : base(lineItem, asmLoad)
        {

        }

        public static LineDetailItemRepeat Create(LineItem lineItem, AsmLoad asmLoad)
        {
            var startMatched = Regex.Match(lineItem.OperationString, RegexPatternRepeatFullStart, RegexOptions.Singleline | RegexOptions.IgnoreCase);
            var startSimpleMatched = Regex.Match(lineItem.OperationString, RegexPatternRepeatSimpleStart, RegexOptions.Singleline | RegexOptions.IgnoreCase);
            var endMatched = Regex.Match(lineItem.OperationString, RegexPatternRepeatEnd, RegexOptions.Singleline | RegexOptions.IgnoreCase);

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

                    repeatLines.Add(lineItem);
                }
                return new LineDetailItemRepeat(lineItem, asmLoad);
            }
            else
            {
                if (startMatched.Success)
                {
                    var lineDetailItemRepeat = new LineDetailItemRepeat(lineItem, asmLoad)
                    {
                        RepeatCountLabel = startMatched.Groups["count"].Value,
                        RepeatLastLabel = startMatched.Groups["last_arg"].Value,

                    };
                    asmLoad.LineDetailItemRepeat = lineDetailItemRepeat;

                    return lineDetailItemRepeat;
                }
                else if (startSimpleMatched.Success)
                {
                    var lineDetailItemRepeat = new LineDetailItemRepeat(lineItem, asmLoad)
                    {
                        RepeatCountLabel = startSimpleMatched.Groups["count"].Value

                    };
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

        public override void ExpansionItem()
        {
            // リピート数が設定されているものを処理する
            if (!string.IsNullOrEmpty(RepeatCountLabel))
            {
                var lineDetailScopeItems = new List<LineDetailScopeItem>();
                var count = AIMath.ConvertToUInt16(RepeatCountLabel, this.AsmLoad);
                var last = string.IsNullOrEmpty(RepeatLastLabel) ? 0 : (Int16)AIMath.ConvertToUInt16(RepeatLastLabel, this.AsmLoad);

                foreach (var repeatCounter in Enumerable.Range(1, count))
                {
                    var lineItems = default(IEnumerable<LineItem>);
                    var localAsmLoad = AsmLoad.Clone(AsmLoad.ScopeModeEnum.Local);
                    localAsmLoad.LabelName = $"REPEAT_{Guid.NewGuid():N}";

                    if (repeatCounter == count)
                    {
                        var take = RepeatLines.Count + last;
                        if (take <= 0 || last > 0)
                        {
                            throw new ErrorMessageException(Error.ErrorCodeEnum.E1013);
                        }
                        //最終ページ処理
                        lineItems = RepeatLines.Take(take).Select(m =>
                        {
                            var lineItem = new LineItem(m);
                            lineItem.CreateLineDetailItem(localAsmLoad);
                            return lineItem;
                        });
                    }
                    else
                    {
                        lineItems = RepeatLines.Select(m =>
                        {
                            var lineItem = new LineItem(m);
                            lineItem.CreateLineDetailItem(localAsmLoad);
                            return lineItem;
                        });
                    }

                    foreach (var lineItem in lineItems)
                    {
                        lineItem.ExpansionItem();
                        lineDetailScopeItems.AddRange(lineItem.LineDetailItem.LineDetailScopeItems);
                    }
                }
                LineDetailScopeItems = lineDetailScopeItems.ToArray();
            }
            else
            {
                LineDetailScopeItems = Array.Empty<LineDetailScopeItem>();
            }    

            base.ExpansionItem();
        }
    }
}
