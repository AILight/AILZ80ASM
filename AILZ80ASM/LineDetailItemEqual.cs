﻿using AILZ80ASM.Assembler;
using AILZ80ASM.Exceptions;
using System.Text.RegularExpressions;

namespace AILZ80ASM
{
    public class LineDetailItemEqual : LineDetailItem
    {
        private static readonly string RegexPatternEqual = @"^equ\s+(?<value>.+)";
        private Label EquLabel { get; set; }
        private AsmAddress AsmAddress { get; set; }
        public override AsmList[] Lists
        {
            get
            {
                return new[]
                {
                    AsmList.CreateLineItemEqual(EquLabel, this.LineItem)
                };
            }
        }

        private LineDetailItemEqual(LineItem lineItem, AsmLoad asmLoad)
            : base(lineItem, asmLoad)
        {

        }

        public static LineDetailItemEqual Create(LineItem lineItem, AsmLoad asmLoad)
        {
            var matched = Regex.Match(lineItem.OperationString, RegexPatternEqual, RegexOptions.Singleline | RegexOptions.IgnoreCase);
            if (matched.Success)
            {
                var labelValue = matched.Groups["value"].Value.Trim();
                var localAsmLoad = asmLoad.Clone();

                var label = new Label(lineItem.LabelString, labelValue, localAsmLoad);
                if (label.Invalidate)
                {
                    throw new ErrorAssembleException(Error.ErrorCodeEnum.E0013);
                }
                localAsmLoad.AddLabel(label);

                asmLoad.SetScope(localAsmLoad);

                return new LineDetailItemEqual(lineItem, localAsmLoad) { EquLabel = label };
            }

            return default;
        }

        public override void ExpansionItem()
        {
            var lineDetailExpansionItem = new LineDetailExpansionItem(this.LineItem);
            EquLabel.SetLineDetailExpansionItem(lineDetailExpansionItem);

            LineDetailScopeItems = new[]
            {
                new LineDetailScopeItem(AsmLoad)
                {
                    LineDetailExpansionItems = new [] { lineDetailExpansionItem }
                }
            };

            base.ExpansionItem();
        }

        public override void PreAssemble(ref AsmAddress asmAddress)
        {
            AsmAddress = asmAddress;

            base.PreAssemble(ref asmAddress);
        }
    }
}
