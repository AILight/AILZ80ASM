using System;
using System.Collections.Generic;
using AILZ80ASM.Assembler;
using AILZ80ASM.Exceptions;
using System.Linq;
using System.Text.RegularExpressions;
using AILZ80ASM.AILight;
using AILZ80ASM.LineDetailItems.ScopeItem;

namespace AILZ80ASM.LineDetailItems
{
    public abstract class LineDetailItemRepeat : LineDetailItem
    {
        // TODO: ラベルにLASTが使えない仕様になってしまっているので、あとでパーサーを強化して使えるようにする
        private string RepeatCountLabel { get; set; }
        private string RepeatLastLabel { get; set; }

        private readonly List<LineItem> RepeatLines = new List<LineItem>();
        private int RepeatNestedCount { get; set; } = 0;

        protected LineDetailItemRepeat(LineItem lineItem, AsmLoad asmLoad)
            : base(lineItem, asmLoad)
        {

        }

        protected static LineDetailItemRepeat Create(LineDetailItemRepeat lineDetailItemRepeat, string regexPatternRepeatFullStart, string regexPatternRepeatSimpleStart, string regexPatternRepeatEnd, AsmLoad asmLoad)
        {
            var lineItem = lineDetailItemRepeat.LineItem;
            var startMatched = Regex.Match(lineItem.OperationString, regexPatternRepeatFullStart, RegexOptions.Singleline | RegexOptions.IgnoreCase);
            var startSimpleMatched = Regex.Match(lineItem.OperationString, regexPatternRepeatSimpleStart, RegexOptions.Singleline | RegexOptions.IgnoreCase);
            var endMatched = Regex.Match(lineItem.OperationString, regexPatternRepeatEnd, RegexOptions.Singleline | RegexOptions.IgnoreCase);

            // リピート処理中
            if (asmLoad.Share.LineDetailItemForExpandItem != default &&
                asmLoad.Share.LineDetailItemForExpandItem.GetType() == lineDetailItemRepeat.GetType())
            {
                var asmLoad_LineDetailItemRepeat = asmLoad.Share.LineDetailItemForExpandItem as LineDetailItemRepeat;

                // 終了条件チェック
                if (endMatched.Success)
                {
                    asmLoad_LineDetailItemRepeat.RepeatNestedCount--;

                    // リピートが終了
                    if (asmLoad_LineDetailItemRepeat.RepeatNestedCount == 0)
                    {
                        asmLoad_LineDetailItemRepeat.RepeatLines.Add(lineItem);

                        asmLoad.Share.LineDetailItemForExpandItem = default;
                        return lineDetailItemRepeat;
                    }
                }

                // 開始条件
                if (startMatched.Success || startSimpleMatched.Success)
                {
                    asmLoad_LineDetailItemRepeat.RepeatNestedCount++;
                }

                var repeatLines = asmLoad_LineDetailItemRepeat.RepeatLines;

                // ローカルラベル以外は使用禁止
                if (lineItem.LabelString.EndsWith(":"))
                {
                    throw new ErrorAssembleException(Error.ErrorCodeEnum.E1014);
                }

                repeatLines.Add(lineItem);
                return lineDetailItemRepeat;
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
                    lineDetailItemRepeat.RepeatCountLabel = startMatched.Groups["count"].Value;
                    lineDetailItemRepeat.RepeatLastLabel = startMatched.Groups["last_arg"].Value;
                    lineDetailItemRepeat.RepeatNestedCount = 1;
                    lineDetailItemRepeat.RepeatLines.Add(lineItem);
                    asmLoad.Share.LineDetailItemForExpandItem = lineDetailItemRepeat;

                    return lineDetailItemRepeat;
                }
                else if (startSimpleMatched.Success)
                {
                    lineDetailItemRepeat.RepeatCountLabel = startSimpleMatched.Groups["count"].Value;
                    lineDetailItemRepeat.RepeatNestedCount = 1;
                    lineDetailItemRepeat.RepeatLines.Add(lineItem);
                    asmLoad.Share.LineDetailItemForExpandItem = lineDetailItemRepeat;

                    return lineDetailItemRepeat;
                }
                else if (string.Compare(lineItem.OperationString, "REPEAT") == 0)
                {
                    throw new ErrorAssembleException(Error.ErrorCodeEnum.E1015);
                }
            }

            return default;
        }

        public override void PreAssemble(ref AsmAddress asmAddress)
        {
            base.PreAssemble(ref asmAddress);

            // リピート数が設定されているものを処理する
            if (!string.IsNullOrEmpty(RepeatCountLabel) && RepeatLines.Count > 2)
            {
                var lineDetailScopeItems = new List<LineDetailScopeItem>();
                var count = AIMath.ConvertTo<UInt16>(RepeatCountLabel, this.AsmLoad, asmAddress);
                var last = string.IsNullOrEmpty(RepeatLastLabel) ? 0 : (Int16)AIMath.ConvertTo<UInt16>(RepeatLastLabel, this.AsmLoad);
                var repeatLines = RepeatLines.Skip(1).SkipLast(1);
                var lineItemList = new List<LineItem>();

                foreach (var repeatCounter in Enumerable.Range(1, count))
                {
                    var lineItems = default(LineItem[]);
                    var guid = $"{Guid.NewGuid():N}";
                    AsmLoad.CreateNewScope($"repeat_{guid}", $"label_{guid}", localAsmLoad =>
                    {
                        if (repeatCounter == count)
                        {
                            var take = repeatLines.Count() + last;
                            if (take <= 0 || last > 0)
                            {
                                throw new ErrorAssembleException(Error.ErrorCodeEnum.E1013);
                            }
                            //最終ページ処理
                            lineItems = repeatLines.Take(take).Select(m =>
                            {
                                var lineItem = new LineItem(m);
                                lineItem.CreateLineDetailItem(localAsmLoad);
                                return lineItem;
                            }).ToArray();
                        }
                        else
                        {
                            lineItems = repeatLines.Select(m =>
                            {
                                var lineItem = new LineItem(m);
                                lineItem.CreateLineDetailItem(localAsmLoad);
                                return lineItem;
                            }).ToArray();
                        }

                        foreach (var lineItem in lineItems)
                        {
                            try
                            {
                                lineItem.ExpansionItem();
                            }
                            catch (ErrorAssembleException ex)
                            {
                                AsmLoad.AddError(new ErrorLineItem(lineItem, ex));
                            }
                        }
                        lineItemList.AddRange(lineItems);
                    });

                    
                }

                foreach (var item in lineItemList)
                {
                    try
                    {
                        item.PreAssemble(ref asmAddress);
                    }
                    catch (ErrorAssembleException ex)
                    {
                        AsmLoad.AddError(new ErrorLineItem(item, ex));
                    }
                }

                LineDetailScopeItems = lineItemList.SelectMany(m => m.LineDetailItem.LineDetailScopeItems).ToArray();
            }
            else
            {
                LineDetailScopeItems = Array.Empty<LineDetailScopeItem>();
            }
        }

        public override void AdjustAssemble(ref uint outputAddress)
        {
            Address = new AsmAddress(Address.Program, outputAddress);
        }
    }
}
