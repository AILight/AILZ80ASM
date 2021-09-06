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
        protected LineItem LineItem { get; set; }
        protected AsmLoad AsmLoad {get;set; }

        public LineDetailExpansionItem[] LineDetailExpansionItems { get; set; }
        public byte[] Bin => LineDetailExpansionItems.SelectMany(m => m.Bin).ToArray();

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
            foreach (var lineDetailExpansionItem in LineDetailExpansionItems)
            {
                lineDetailExpansionItem.PreAssemble(ref asmAddress, AsmLoad);
            }
        }

        public virtual void BuildAddressLabel()
        {
            if (LineDetailExpansionItems == default)
                return;

            foreach (var lineDetailExpansionItem in LineDetailExpansionItems)
            {
                lineDetailExpansionItem.BuildAddressLabel(AsmLoad);
            }
        }

        public virtual void Assemble()
        {
            if (LineDetailExpansionItems == default)
                return;

            foreach (var lineDetailExpansionItem in LineDetailExpansionItems)
            {
                lineDetailExpansionItem.Assemble(AsmLoad);
            }
        }

    }
}
