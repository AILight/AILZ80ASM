using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace AILZ80ASM
{
    public class LineDetailScopeItem
    {
        protected AsmLoad AsmLoad {get;set; }

        public LineDetailExpansionItem[] LineDetailExpansionItems { get; set; }
        public byte[] Bin => LineDetailExpansionItems.SelectMany(m => m.Bin).ToArray();

        public LineDetailScopeItem(AsmLoad asmLoad)
        {
            AsmLoad = asmLoad.Clone();
            LineDetailExpansionItems = Array.Empty<LineDetailExpansionItem>();
        }

        public LineDetailScopeItem(LineItem lineItem, AsmLoad asmLoad)
        {
            AsmLoad = asmLoad.Clone();
            LineDetailExpansionItems = new[]
            {
               new LineDetailExpansionItemOperation(lineItem, asmLoad)
            };
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
