using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace AILZ80ASM
{
    public class LineDetailItemConditional : LineDetailItem
    {
        // TODO: ラベルにLASTが使えない仕様になってしまっているので、あとでパーサーを強化して使えるようにする
        private static readonly string RegexPatternRepeatIf = @"^#IF\s+(?<condition>.+)$";
        private static readonly string RegexPatternRepeatElIf = @"^#ELIF\s+(?<condition>.+)$";
        private static readonly string RegexPatternRepeatElse = @"^#ELSE$";
        private static readonly string RegexPatternRepeatEnd = @"^#ENDIF$";

        private readonly Dictionary<string, List<LineItem>> Conditions = new Dictionary<string, List<LineItem>>();
        private string ConditionKey { get; set; }
        private int ConditionalNestedCount { get; set; } = 0;

        private LineDetailItemConditional(LineItem lineItem, AsmLoad asmLoad)
            : base(lineItem, asmLoad)
        {

        }

        public static LineDetailItemConditional Create(LineItem lineItem, AsmLoad asmLoad)
        {
            var ifMatched = Regex.Match(lineItem.OperationString, RegexPatternRepeatIf, RegexOptions.Singleline | RegexOptions.IgnoreCase);
            var elifMatched = Regex.Match(lineItem.OperationString, RegexPatternRepeatElIf, RegexOptions.Singleline | RegexOptions.IgnoreCase);
            var elseMatched = Regex.Match(lineItem.OperationString, RegexPatternRepeatElse, RegexOptions.Singleline | RegexOptions.IgnoreCase);
            var endMatched = Regex.Match(lineItem.OperationString, RegexPatternRepeatEnd, RegexOptions.Singleline | RegexOptions.IgnoreCase);

            // Conditionalでラベルが存在していたらエラー
            if (ifMatched.Success || elifMatched.Success || elseMatched.Success || endMatched.Success)
            {
                if (!string.IsNullOrEmpty(lineItem.LabelString))
                {
                    throw new ErrorAssembleException(Error.ErrorCodeEnum.E1024);
                }
            }

            // リピート処理中
            if (asmLoad.LineDetailItemForExpandItem is LineDetailItemConditional asmLoad_LineDetailItemConditional)
            {
                // 終了条件チェック
                if (endMatched.Success)
                {
                    asmLoad_LineDetailItemConditional.ConditionalNestedCount--;

                    // リピートが終了
                    if (asmLoad_LineDetailItemConditional.ConditionalNestedCount == 0)
                    {
                        asmLoad.LineDetailItemForExpandItem = default;
                        return new LineDetailItemConditional(lineItem, asmLoad);
                    }

                }

                // 開始条件
                if (ifMatched.Success)
                {
                    asmLoad_LineDetailItemConditional.ConditionalNestedCount++;
                }
                else if (elifMatched.Success)
                {
                    if (asmLoad_LineDetailItemConditional.Conditions.ContainsKey(""))
                    {
                        // Elseが既にある場合
                        throw new ErrorAssembleException(Error.ErrorCodeEnum.E1023);
                    }

                    asmLoad_LineDetailItemConditional.ConditionKey = elifMatched.Groups["condition"].Value;
                    asmLoad_LineDetailItemConditional.Conditions.Add(asmLoad_LineDetailItemConditional.ConditionKey, new List<LineItem>());
                }
                else if (elseMatched.Success)
                {
                    if (asmLoad_LineDetailItemConditional.Conditions.ContainsKey(""))
                    {
                        // Elseが既にある場合
                        throw new ErrorAssembleException(Error.ErrorCodeEnum.E1023);
                    }

                    asmLoad_LineDetailItemConditional.ConditionKey = "";
                    asmLoad_LineDetailItemConditional.Conditions.Add(asmLoad_LineDetailItemConditional.ConditionKey, new List<LineItem>());
                }
                else
                {
                    var lines = asmLoad_LineDetailItemConditional.Conditions[asmLoad_LineDetailItemConditional.ConditionKey];
                    lines.Add(lineItem);
                }
                return new LineDetailItemConditional(lineItem, asmLoad);
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
                    var lineDetailItemConditional = new LineDetailItemConditional(lineItem, asmLoad)
                    {
                        ConditionKey = ifMatched.Groups["condition"].Value,
                        ConditionalNestedCount = 1
                    };
                    lineDetailItemConditional.Conditions.Add(lineDetailItemConditional.ConditionKey, new List<LineItem>());

                    asmLoad.LineDetailItemForExpandItem = lineDetailItemConditional;

                    return lineDetailItemConditional;
                }
            }

            return default;
        }

        public override void ExpansionItem()
        {
            // 初期値設定
            LineDetailScopeItems = Array.Empty<LineDetailScopeItem>();
            // リピート数が設定されているものを処理する
            foreach (var condition in Conditions.Keys)
            {
                //try
                {
                    if (string.IsNullOrEmpty(condition) || AIMath.ConvertTo<bool>(condition, AsmLoad))
                    {
                        var lineItems = default(LineItem[]);
                        var lineDetailScopeItems = new List<LineDetailScopeItem>();

                        lineItems = Conditions[condition].Select(m =>
                        {
                            var lineItem = new LineItem(m);
                            lineItem.CreateLineDetailItem(AsmLoad);
                            return lineItem;
                        }).ToArray();

                        foreach (var lineItem in lineItems)
                        {
                            lineItem.ExpansionItem();
                            lineDetailScopeItems.AddRange(lineItem.LineDetailItem.LineDetailScopeItems);
                        }
                        LineDetailScopeItems = lineDetailScopeItems.ToArray();
                        break;
                    }
                }
                //catch
                { 
                }
            }

            base.ExpansionItem();
        }
    }
}
