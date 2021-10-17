using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace AILZ80ASM
{
    public abstract class LineDetailItem
    {
        private static readonly string RegexPatternLabel = @"^\s*(?<label>[a-zA-Z0-9_]+::?)";
        public LineItem LineItem { get; private set; }
        protected AsmLoad AsmLoad {get;set; }

        public LineDetailScopeItem[] LineDetailScopeItems { get; set; }
        public virtual byte[] Bin => LineDetailScopeItems == default ? Array.Empty<byte>() : LineDetailScopeItems.SelectMany(m => m.Bin).ToArray();
        public List<ErrorLineItem> Errors { get; private set; } = new List<ErrorLineItem>();

        protected LineDetailItem(LineItem lineItem, AsmLoad asmLoad)
        {
            LineItem = lineItem;
            // ラベルの処理をする
            if (lineItem.LabelString.EndsWith("::"))
            {
                asmLoad.GlobalLableName = lineItem.LabelString.Substring(0, lineItem.LabelString.Length - 2);
            }
            else if (lineItem.LabelString.EndsWith(":"))
            {
                asmLoad.LabelName = lineItem.LabelString.Substring(0, lineItem.LabelString.Length - 1);
            }

            AsmLoad = asmLoad.Clone();
        }

        public static LineDetailItem CreateLineDetailItem(LineItem lineItem, AsmLoad asmLoad)
        {
            // ラベルの処理
            ProcessAsmLoad(lineItem, asmLoad);

            // インクルードのチェック
            var lineDetailItem = default(LineDetailItem);

            if (asmLoad.LineDetailItemForExpandItem != default)
            {
                if (asmLoad.LineDetailItemForExpandItem is LineDetailItemMacroDefine)
                {
                    lineDetailItem ??= LineDetailItemMacroDefine.Create(lineItem, asmLoad);
                }
                else if (asmLoad.LineDetailItemForExpandItem is LineDetailItemRepeat)
                {
                    lineDetailItem ??= LineDetailItemRepeat.Create(lineItem, asmLoad);
                }
                else if (asmLoad.LineDetailItemForExpandItem is LineDetailItemConditional)
                {
                    lineDetailItem ??= LineDetailItemConditional.Create(lineItem, asmLoad);
                }
                else
                {
                    throw new NotImplementedException();
                }
                /* 例外が切れるので、この処理は使わない
                var methodInfo = asmLoad.LineDetailItemForExpandItem.GetType().GetMethod("Create");
                if (methodInfo != null)
                {
                    lineDetailItem = (LineDetailItem)methodInfo.Invoke(null, new object[] { lineItem, asmLoad });
                }
                */
            }
            else
            {
                lineDetailItem ??= LineDetailItemMacroDefine.Create(lineItem, asmLoad);
                lineDetailItem ??= LineDetailItemRepeat.Create(lineItem, asmLoad);
                lineDetailItem ??= LineDetailItemConditional.Create(lineItem, asmLoad);
                lineDetailItem ??= LineDetailItemError.Create(lineItem, asmLoad);
                lineDetailItem ??= LineDetailItemEqual.Create(lineItem, asmLoad);
                lineDetailItem ??= LineDetailItemInclude.Create(lineItem, asmLoad);
                lineDetailItem ??= LineDetailItemOperation.Create(lineItem, asmLoad);
                lineDetailItem ??= LineDetailItemMacro.Create(lineItem, asmLoad);
                lineDetailItem ??= LineDetailItemInvalid.Create(lineItem, asmLoad); // ここには来ない
            }
            return lineDetailItem;
        }

        private static void ProcessAsmLoad(LineItem lineItem, AsmLoad asmLoad)
        {
            var labelMatched = Regex.Match(lineItem.OperationString, RegexPatternLabel, RegexOptions.Singleline | RegexOptions.IgnoreCase);
            if (labelMatched.Success)
            {
                var label = labelMatched.Groups["label"].Value;
                if (label.EndsWith("::"))
                {
                    asmLoad.GlobalLableName = label.Substring(0, label.Length - 2);
                }
                else if (label.EndsWith(":"))
                {
                    asmLoad.LabelName = label.Substring(0, label.Length - 1);
                }
            }
        }

        public virtual void ExpansionItem()
        {
            
        }

        public virtual void PreAssemble(ref AsmAddress asmAddress)
        {
            if (LineDetailScopeItems == default)
                return;

            foreach (var item in LineDetailScopeItems)
            {
                item.PreAssemble(ref asmAddress);
            }
        }

        public virtual void BuildAddressLabel()
        {
            if (LineDetailScopeItems == default)
                return;

            foreach (var item in LineDetailScopeItems)
            {
                item.BuildAddressLabel();
            }
        }

        public virtual void BuildArgumentLabel()
        {
            if (LineDetailScopeItems == default)
                return;

            foreach (var item in LineDetailScopeItems)
            {
                item.BuildArgumentLabel();
            }
        }

        public virtual void BuildValueLabel()
        {
            if (LineDetailScopeItems == default)
                return;

            foreach (var item in LineDetailScopeItems)
            {
                item.BuildValueLabel();
            }
        }


        public virtual void Assemble()
        {
            if (LineDetailScopeItems == default)
                return;

            foreach (var item in LineDetailScopeItems)
            {
                item.Assemble();
            }
        }

    }
}
