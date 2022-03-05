using AILZ80ASM.Assembler;
using AILZ80ASM.Exceptions;
using System;
using System.Linq;

namespace AILZ80ASM
{
    public class LineDetailScopeItem
    {
        protected AsmLoad AsmLoad { get; set; }

        public LineDetailExpansionItem[] LineDetailExpansionItems { get; set; }
        public byte[] Bin => LineDetailExpansionItems.SelectMany(m => m.Bin).ToArray();
        public AsmList[] Lists => LineDetailExpansionItems.Select(m => m.List).ToArray();

        public LineDetailScopeItem(AsmLoad asmLoad)
        {
            AsmLoad = asmLoad.Clone();
            LineDetailExpansionItems = Array.Empty<LineDetailExpansionItem>();
        }

        public LineDetailScopeItem(LineItem lineItem, AsmLoad asmLoad)
        {
            asmLoad.CreateScope(newAsmLoad =>
            {
                AsmLoad = newAsmLoad;
                LineDetailExpansionItems = new[]
                {
                    new LineDetailExpansionItemOperation(lineItem, AsmLoad)
                };
            });
        }

        public LineDetailScopeItem(LineDetailExpansionItem[] lineDetailExpansionItems, AsmLoad asmLoad)
        {
            AsmLoad = asmLoad.Clone();
            LineDetailExpansionItems = lineDetailExpansionItems;
        }

        public virtual void PreAssemble(ref AsmAddress asmAddress)
        {
            foreach (var lineDetailExpansionItem in LineDetailExpansionItems)
            {
                try
                {
                    lineDetailExpansionItem.PreAssemble(ref asmAddress, AsmLoad);
                }
                catch (ErrorAssembleException ex)
                {
                    AsmLoad.AddError(new ErrorLineItem(lineDetailExpansionItem.LineItem, ex));
                }
            }
        }

        public virtual void Assemble()
        {
            if (LineDetailExpansionItems == default)
                return;

            foreach (var lineDetailExpansionItem in LineDetailExpansionItems)
            {
                try
                {
                    lineDetailExpansionItem.Assemble(AsmLoad);
                }
                catch (ErrorAssembleException ex)
                {
                    AsmLoad.AddError(new ErrorLineItem(lineDetailExpansionItem.LineItem, ex));
                }
            }
        }
    }
}
