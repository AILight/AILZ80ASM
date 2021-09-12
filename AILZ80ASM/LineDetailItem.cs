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
        public byte[] Bin => LineDetailScopeItems == default ? Array.Empty<byte>() : LineDetailScopeItems.SelectMany(m => m.Bin).ToArray();
        public ErrorMessageException InternalErrorMessageException { get; set; }

        public LineDetailItem(LineItem lineItem, AsmLoad asmLoad)
        {
            LineItem = lineItem;
            // ラベルの処理をする
            var labelName = Label.GetLabelText(lineItem.OperationString);
            if (labelName.EndsWith("::"))
            {
                asmLoad.GlobalLableName = labelName.Substring(0, labelName.Length - 2);
            }
            else if (labelName.EndsWith(":"))
            {
                asmLoad.LabelName = labelName.Substring(0, labelName.Length - 1);
            }

            AsmLoad = asmLoad.Clone();
        }

        public static LineDetailItem CreateLineDetailItem(LineItem lineItem, AsmLoad asmLoad)
        {
            // ラベルの処理
            ProcessAsmLoad(lineItem, asmLoad);

            // インクルードのチェック
            var lineDetailItem = default(LineDetailItem);

            lineDetailItem ??= LineDetailItemMacro.Create(lineItem, asmLoad);
            lineDetailItem ??= LineDetailItemRepeat.Create(lineItem, asmLoad);
            lineDetailItem ??= LineDetailItemEqual.Create(lineItem, asmLoad);
            lineDetailItem ??= LineDetailItemInclude.Create(lineItem, asmLoad);
            lineDetailItem ??= new LineDetailItemOperation(lineItem, asmLoad);

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
