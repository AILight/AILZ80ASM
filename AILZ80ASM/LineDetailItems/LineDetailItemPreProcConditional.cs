using AILZ80ASM.AILight;
using AILZ80ASM.Assembler;
using AILZ80ASM.Exceptions;
using AILZ80ASM.LineDetailItems.ScopeItem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AILZ80ASM.LineDetailItems
{
    public class LineDetailItemPreProcConditional : LineDetailItem
    {
        // TODO: ラベルにLASTが使えない仕様になってしまっているので、あとでパーサーを強化して使えるようにする
        private static readonly string RegexPatternRepeatIf = @"^#IF\s+(?<condition>.+)$";
        private static readonly string RegexPatternRepeatElIf = @"^#ELIF\s+(?<condition>.+)$";
        private static readonly string RegexPatternRepeatElse = @"^#ELSE$";
        private static readonly string RegexPatternRepeatEnd = @"^#ENDIF$";

        public class ConditionalPack
        {
            public string Condition { get; set; }
            public List<LineDetailItemPreProcConditional> LineDetailItemConditionaList = new List<LineDetailItemPreProcConditional>();
        
            public ConditionalPack(string condition)
            {
                Condition = condition;
            }

        }
        public List<ConditionalPack> ConditionPacks { get; private set; } = new List<ConditionalPack>();

        private int ConditionalNestedCount { get; set; } = 0;
        private bool EnableAssemble { get; set; } = false;

        private LineDetailItemPreProcConditional(LineItem lineItem, AsmLoad asmLoad)
            : base(lineItem, asmLoad)
        {

        }

        public static LineDetailItemPreProcConditional Create(LineItem lineItem, AsmLoad asmLoad)
        {
            if (!lineItem.IsCollectOperationString)
            {
                return default(LineDetailItemPreProcConditional);
            }

            var ifMatched = Regex.Match(lineItem.OperationString, RegexPatternRepeatIf, RegexOptions.Singleline | RegexOptions.IgnoreCase);
            var elifMatched = Regex.Match(lineItem.OperationString, RegexPatternRepeatElIf, RegexOptions.Singleline | RegexOptions.IgnoreCase);
            var elseMatched = Regex.Match(lineItem.OperationString, RegexPatternRepeatElse, RegexOptions.Singleline | RegexOptions.IgnoreCase);
            var endMatched = Regex.Match(lineItem.OperationString, RegexPatternRepeatEnd, RegexOptions.Singleline | RegexOptions.IgnoreCase);

            // Conditionalでラベルが存在していたらエラー
            if (!string.IsNullOrEmpty(lineItem.LabelString))
            {
                if (elifMatched.Success || elseMatched.Success || endMatched.Success)
                {
                    if (endMatched.Success)
                    {
                        if (asmLoad.Share.LineDetailItemForExpandItem is LineDetailItemPreProcConditional errorAsmLoad_LineDetailItemConditional)
                        {
                            errorAsmLoad_LineDetailItemConditional.ConditionalNestedCount--;

                            // 条件処理が終了
                            if (errorAsmLoad_LineDetailItemConditional.ConditionalNestedCount == 0)
                            {
                                asmLoad.Share.LineDetailItemForExpandItem = default;
                            }
                        }
                    }
                    throw new ErrorAssembleException(Error.ErrorCodeEnum.E1024);
                }
            }

            // 条件処理処理中
            if (asmLoad.Share.LineDetailItemForExpandItem is LineDetailItemPreProcConditional asmLoad_LineDetailItemConditional)
            {
                // 終了条件チェック
                if (endMatched.Success)
                {
                    asmLoad_LineDetailItemConditional.ConditionalNestedCount--;

                    // 条件処理が終了
                    if (asmLoad_LineDetailItemConditional.ConditionalNestedCount == 0)
                    {
                        asmLoad.Share.LineDetailItemForExpandItem = default;
                        return new LineDetailItemPreProcConditional(lineItem, asmLoad);
                    }

                }

                var lineDetailItemConditional = new LineDetailItemPreProcConditional(lineItem, asmLoad);
                // 開始条件
                if (ifMatched.Success)
                {
                    asmLoad_LineDetailItemConditional.ConditionalNestedCount++;
                    asmLoad_LineDetailItemConditional.ConditionPacks.Last().LineDetailItemConditionaList.Add(lineDetailItemConditional);
                }
                else if (asmLoad_LineDetailItemConditional.ConditionalNestedCount == 1)
                {
                    if (elifMatched.Success)
                    {
                        if (asmLoad_LineDetailItemConditional.ConditionPacks.Any(m => string.IsNullOrEmpty(m.Condition)))
                        {
                            // Elseが既にある場合
                            throw new ErrorAssembleException(Error.ErrorCodeEnum.E1023);
                        }
                        var conditionalPack = new ConditionalPack(elifMatched.Groups["condition"].Value);
                        asmLoad_LineDetailItemConditional.ConditionPacks.Add(conditionalPack);
                    }
                    else if (elseMatched.Success)
                    {
                        if (asmLoad_LineDetailItemConditional.ConditionPacks.Any(m => string.IsNullOrEmpty(m.Condition)))
                        {
                            // Elseが既にある場合
                            throw new ErrorAssembleException(Error.ErrorCodeEnum.E1023);
                        }

                        var conditionalPack = new ConditionalPack("");
                        asmLoad_LineDetailItemConditional.ConditionPacks.Add(conditionalPack);
                    }
                    else
                    {
                        asmLoad_LineDetailItemConditional.ConditionPacks.Last().LineDetailItemConditionaList.Add(lineDetailItemConditional);
                    }
                }

                return lineDetailItemConditional;
            }
            else
            {
                // 終了条件チェック
                if (endMatched.Success)
                {
                    throw new ErrorAssembleException(Error.ErrorCodeEnum.E1022);
                }

                // 開始条件チェック
                if (ifMatched.Success)
                {
                    var lineDetailItemConditional = new LineDetailItemPreProcConditional(lineItem, asmLoad)
                    {
                        ConditionalNestedCount = 1
                    };
                    lineDetailItemConditional.ConditionPacks.Add(new ConditionalPack(ifMatched.Groups["condition"].Value));
                    asmLoad.Share.LineDetailItemForExpandItem = lineDetailItemConditional;

                    return lineDetailItemConditional;
                }
            }

            return default;
        }

        public override void PreAssemble(ref AsmAddress asmAddress)
        {
            if (EnableAssemble)
            {
                base.PreAssemble(ref asmAddress);
            }
            else
            {
                // リピート数が設定されているものを処理する
                try
                {
                    foreach (var conditionPack in ConditionPacks)
                    {
                        if (string.IsNullOrEmpty(conditionPack.Condition) || AIMath.Calculation(conditionPack.Condition, AsmLoad, asmAddress).ConvertTo<bool>())
                        {
                            foreach (var item in conditionPack.LineDetailItemConditionaList)
                            {
                                var lineItem = item.LineItem;
                                try
                                {
                                    lineItem.CreateLineDetailItem(AsmLoad);
                                }
                                catch (ErrorAssembleException ex)
                                {
                                    this.AsmLoad.AddError(new ErrorLineItem(lineItem, ex));
                                }
                                catch (ErrorLineItemException ex)
                                {
                                    this.AsmLoad.AddError(ex.ErrorLineItem);
                                }
                                catch (Exception ex)
                                {
                                    this.AsmLoad.AddError(new ErrorLineItem(lineItem, new ErrorAssembleException(Error.ErrorCodeEnum.E0000, ex.Message)));
                                }
                                item.EnableAssemble = true;
                            }
                            break;
                        }
                    }
                }
                catch
                {
                    // 解析でエラーが出た場合ORGに積む
                    AsmLoad.AddErrorLineDetailItem(this);
                    throw;
                }
            }

            //base.PreAssemble(ref asmAddress);
        }

        public override AsmList[] Lists
        {
            get
            {
                if (!AsmLoad.Share.IsOutputList)
                {
                    return new AsmList[] { };
                }

                if (LineDetailScopeItems == default)
                {
                    return new[]
                    {
                        AsmList.CreateLineItem(LineItem)
                    };
                }
                else
                {
                    return LineDetailScopeItems.SelectMany(m => m.Lists).ToArray();
                }
            }
        }

        public override void AdjustAssemble(ref uint outputAddress)
        {
            Address = new AsmAddress(Address.Value.Program, outputAddress);
        }
    }
}
