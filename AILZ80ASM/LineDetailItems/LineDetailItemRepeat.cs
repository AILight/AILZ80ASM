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
    public class LineDetailItemRepeat : LineDetailItem
    {
        private class RepeatItem
        {
            public int Index { get; set; }
            public Label Label { get; set; }
            public LineItem[] LineItems { get; set; }

        }

        // TODO: ラベルにLASTが使えない仕様になってしまっているので、あとでパーサーを強化して使えるようにする
        private static readonly string RegexPatternRepeatFullStart = @"^\s*REPT\s+(?<count>.+)\s+LAST\s+(?<last_arg>.+)$";
        private static readonly string RegexPatternRepeatSimpleStart = @"^\s*REPT\s+(?<count>.+)$";
        private static readonly string RegexPatternRepeatEnd = @"^\s*ENDM\s*$";

        private string RepeatCountLabel { get; set; }
        private string RepeatLastLabel { get; set; }

        private readonly List<LineItem> RepeatLines = new List<LineItem>();
        private readonly List<RepeatItem> RepeatItems = new List<RepeatItem>();
        private int RepeatNestedCount { get; set; } = 0;

        public override AsmList[] Lists
        {
            get
            {
                if (!AsmLoad.Share.IsOutputList)
                {
                    return new AsmList[] { };
                }

                var asmList = new List<AsmList>();
                // 宣言
                foreach (var item in this.RepeatLines)
                {
                    asmList.Add(AsmList.CreateLineItem(item));
                }

                // 実体
                foreach (var repeatItem in RepeatItems)
                {
                    var headerList = AsmList.CreateSource($" #{repeatItem.Index:0000}");
                    headerList.PushNestedCodeType(AsmList.NestedCodeTypeEnum.Repeat);
                    asmList.Add(headerList);
                    foreach (var item in repeatItem.LineItems)
                    {
                        foreach (var list in item.Lists)
                        {
                            list.PushNestedCodeType(AsmList.NestedCodeTypeEnum.Repeat);
                            asmList.Add(list);
                        }
                    }
                }

                return asmList.ToArray();

                //return base.Lists;
            }
        }

        protected LineDetailItemRepeat(LineItem lineItem, AsmLoad asmLoad)
            : base(lineItem, asmLoad)
        {

        }

        public static LineDetailItemRepeat Create(LineItem lineItem, AsmLoad asmLoad)
        {
            if (!lineItem.IsCollectOperationString)
            {
                return default(LineDetailItemRepeat);
            }

            var startMatched = Regex.Match(lineItem.OperationString, RegexPatternRepeatFullStart, RegexOptions.Singleline | RegexOptions.IgnoreCase);
            var startSimpleMatched = Regex.Match(lineItem.OperationString, RegexPatternRepeatSimpleStart, RegexOptions.Singleline | RegexOptions.IgnoreCase);
            var endMatched = Regex.Match(lineItem.OperationString, RegexPatternRepeatEnd, RegexOptions.Singleline | RegexOptions.IgnoreCase);

            // リピート処理中
            if (asmLoad.Share.LineDetailItemForExpandItem is LineDetailItemRepeat asmLoad_LineDetailItemRepeat)
            {
                // 終了条件チェック
                if (endMatched.Success)
                {
                    asmLoad_LineDetailItemRepeat.RepeatNestedCount--;

                    // リピートが終了
                    if (asmLoad_LineDetailItemRepeat.RepeatNestedCount == 0)
                    {
                        asmLoad_LineDetailItemRepeat.RepeatLines.Add(lineItem);

                        asmLoad.Share.LineDetailItemForExpandItem = default;
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
                if (lineItem.LabelString.EndsWith(':'))
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
                    var lineDetailItemRepeat = new LineDetailItemRepeat(lineItem, asmLoad);

                    lineDetailItemRepeat.RepeatCountLabel = startMatched.Groups["count"].Value;
                    lineDetailItemRepeat.RepeatLastLabel = startMatched.Groups["last_arg"].Value;
                    lineDetailItemRepeat.RepeatNestedCount = 1;
                    lineDetailItemRepeat.RepeatLines.Add(lineItem);
                    asmLoad.Share.LineDetailItemForExpandItem = lineDetailItemRepeat;

                    return lineDetailItemRepeat;
                }
                else if (startSimpleMatched.Success)
                {
                    var lineDetailItemRepeat = new LineDetailItemRepeat(lineItem, asmLoad);

                    lineDetailItemRepeat.RepeatCountLabel = startSimpleMatched.Groups["count"].Value;
                    lineDetailItemRepeat.RepeatNestedCount = 1;
                    lineDetailItemRepeat.RepeatLines.Add(lineItem);
                    asmLoad.Share.LineDetailItemForExpandItem = lineDetailItemRepeat;

                    return lineDetailItemRepeat;
                }
                else if (string.Compare(lineItem.OperationString, "REPEAT") == 0 ||
                         string.Compare(lineItem.OperationString, "REPT") == 0)
                {
                    throw new ErrorAssembleException(Error.ErrorCodeEnum.E1015, "");
                }
            }

            return default;
        }

        public override void PreAssemble(ref AsmAddress asmAddress)
        {
            base.PreAssemble(ref asmAddress);
            if (!string.IsNullOrEmpty(RepeatCountLabel) && RepeatLines.Count > 2)
            {
                var count = AIMath.Calculation(RepeatCountLabel, this.AsmLoad, asmAddress).ConvertTo<int>();
                if (count < 0)
                {
                    throw new ErrorAssembleException(Error.ErrorCodeEnum.E1015, count);
                }
                var last = string.IsNullOrEmpty(RepeatLastLabel) ? 0 : AIMath.Calculation(RepeatLastLabel, this.AsmLoad).ConvertTo<int>();

                var lineDetailScopeItems = new List<LineDetailScopeItem>();
                var repeatLines = RepeatLines.Skip(1).SkipLast(1);
                var lineItemList = new List<LineItem>();

                foreach (var repeatCounter in Enumerable.Range(1, count))
                {
                    var repeatItem = new RepeatItem() { Index = repeatCounter };
                    var lineItems = default(LineItem[]);
                    var guid = $"{Guid.NewGuid():N}";
                    AsmLoad.CreateNewScope($"repeat_{guid}", $"label_{guid}", localAsmLoad =>
                    {
                        if (repeatCounter == count)
                        {
                            var take = repeatLines.Where(m => !string.IsNullOrEmpty(m.OperationString)).Count() + last;
                            if (last > 0)
                            {
                                throw new ErrorAssembleException(Error.ErrorCodeEnum.E1013, last);
                            }
                            if (take < 0)
                            {
                                throw new ErrorAssembleException(Error.ErrorCodeEnum.E1016, last);
                            }

                            //最終ページ処理（命令部だけを削除する）
                            var results = new List<LineItem>();
                            var count = 0;
                            foreach (var lineItem in repeatLines)
                            {
                                var addFlg = false;
                                if (string.IsNullOrEmpty(lineItem.OperationString))
                                {
                                    addFlg = true;
                                }
                                else
                                {
                                    if (count < take)
                                    {
                                        addFlg = true;
                                        count++;
                                    }
                                }

                                if (addFlg)
                                {
                                    var newLineItem = new LineItem(lineItem);
                                    newLineItem.CreateLineDetailItem(localAsmLoad);
                                    results.Add(newLineItem);
                                }
                            }
                            lineItems = results.ToArray();
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
                            catch (ErrorLineItemException ex)
                            {
                                AsmLoad.AddError(ex.ErrorLineItem);
                            }
                        }
                        repeatItem.LineItems = lineItems;
                        lineItemList.AddRange(lineItems);
                    });
                    RepeatItems.Add(repeatItem);
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
                    catch (ErrorLineItemException ex)
                    {
                        AsmLoad.AddError(ex.ErrorLineItem);
                    }
                }

                LineDetailScopeItems = lineItemList.Where(m => m.LineDetailItem.LineDetailScopeItems != default).SelectMany(m => m.LineDetailItem.LineDetailScopeItems).ToArray();
            }
            else
            {
                LineDetailScopeItems = Array.Empty<LineDetailScopeItem>();
            }
        }

        public override void AdjustAssemble(ref uint outputAddress)
        {
            Address = new AsmAddress(Address.Value.Program, outputAddress);
        }

        public static bool IsMatchStart(LineItem lineItem)
        {
            var startMatched = Regex.Match(lineItem.OperationString, RegexPatternRepeatFullStart, RegexOptions.Singleline | RegexOptions.IgnoreCase);
            var startSimpleMatched = Regex.Match(lineItem.OperationString, RegexPatternRepeatSimpleStart, RegexOptions.Singleline | RegexOptions.IgnoreCase);

            return startMatched.Success || startSimpleMatched.Success;
        }

        public static bool IsMatchEnd(LineItem lineItem)
        {
            var endMatched = Regex.Match(lineItem.OperationString, RegexPatternRepeatEnd, RegexOptions.Singleline | RegexOptions.IgnoreCase);

            return endMatched.Success;

        }
    }
}
