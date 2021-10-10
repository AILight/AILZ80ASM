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
        private int RepeatNestedCount { get; set; } = 0;

        public LineDetailItemRepeat(LineItem lineItem, AsmLoad asmLoad)
            : base(lineItem, asmLoad)
        {

        }

        public static LineDetailItemRepeat Create(LineItem lineItem, AsmLoad asmLoad)
        {
            var startMatched = Regex.Match(lineItem.OperationString, RegexPatternRepeatFullStart, RegexOptions.Singleline | RegexOptions.IgnoreCase);
            var startSimpleMatched = Regex.Match(lineItem.OperationString, RegexPatternRepeatSimpleStart, RegexOptions.Singleline | RegexOptions.IgnoreCase);
            var endMatched = Regex.Match(lineItem.OperationString, RegexPatternRepeatEnd, RegexOptions.Singleline | RegexOptions.IgnoreCase);

            // リピート処理中
            if (asmLoad.LineDetailItemForExpandItem is LineDetailItemRepeat asmLoad_LineDetailItemRepeat)
            {
                // 終了条件チェック
                if (endMatched.Success)
                {
                    asmLoad_LineDetailItemRepeat.RepeatNestedCount--;

                    // リピートが終了
                    if (asmLoad_LineDetailItemRepeat.RepeatNestedCount == 0)
                    {
                        asmLoad.LineDetailItemForExpandItem = default;
                        return new LineDetailItemRepeat(lineItem, asmLoad);
                    }
                }

                // 開始条件
                if (startMatched.Success || startSimpleMatched.Success)
                {
                    asmLoad_LineDetailItemRepeat.RepeatNestedCount++;
                }

                var repeatLines = asmLoad_LineDetailItemRepeat.RepeatLines;

                // ローカルラベル以外は使用禁止
                var lable = Label.GetLabelText(lineItem.OperationString);
                if (lable.EndsWith(":"))
                {
                    throw new ErrorAssembleException(Error.ErrorCodeEnum.E1014);
                }

                repeatLines.Add(lineItem);
                return new LineDetailItemRepeat(lineItem, asmLoad);
            }
            else
            {
                // 終了条件チェック
                if (endMatched.Success)
                {
                    throw new ErrorAssembleException(Error.ErrorCodeEnum.E1012);
                }

                // 開始条件チェック
                if (startMatched.Success)
                {
                    var lineDetailItemRepeat = new LineDetailItemRepeat(lineItem, asmLoad)
                    {
                        RepeatCountLabel = startMatched.Groups["count"].Value,
                        RepeatLastLabel = startMatched.Groups["last_arg"].Value,
                        RepeatNestedCount = 1
                    };
                    asmLoad.LineDetailItemForExpandItem = lineDetailItemRepeat;

                    return lineDetailItemRepeat;
                }
                else if (startSimpleMatched.Success)
                {
                    var lineDetailItemRepeat = new LineDetailItemRepeat(lineItem, asmLoad)
                    {
                        RepeatCountLabel = startSimpleMatched.Groups["count"].Value,
                        RepeatNestedCount = 1
                    };
                    asmLoad.LineDetailItemForExpandItem = lineDetailItemRepeat;

                    return lineDetailItemRepeat;
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
                    var lineItems = default(LineItem[]);
                    var localAsmLoad = AsmLoad.Clone(AsmLoad.ScopeModeEnum.Local);
                    localAsmLoad.LabelName = $"REPEAT_{Guid.NewGuid():N}";

                    if (repeatCounter == count)
                    {
                        var take = RepeatLines.Count + last;
                        if (take <= 0 || last > 0)
                        {
                            throw new ErrorAssembleException(Error.ErrorCodeEnum.E1013);
                        }
                        //最終ページ処理
                        lineItems = RepeatLines.Take(take).Select(m =>
                        {
                            var lineItem = new LineItem(m);
                            lineItem.CreateLineDetailItem(localAsmLoad);
                            return lineItem;
                        }).ToArray();
                    }
                    else
                    {
                        lineItems = RepeatLines.Select(m =>
                        {
                            var lineItem = new LineItem(m);
                            lineItem.CreateLineDetailItem(localAsmLoad);
                            return lineItem;
                        }).ToArray();
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
