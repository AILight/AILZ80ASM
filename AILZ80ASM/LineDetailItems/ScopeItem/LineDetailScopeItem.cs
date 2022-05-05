using AILZ80ASM.Assembler;
using AILZ80ASM.Exceptions;
using AILZ80ASM.LineDetailItems.ScopeItem.ExpansionItems;
using System;
using System.Linq;

namespace AILZ80ASM.LineDetailItems.ScopeItem
{
    public class LineDetailScopeItem
    {
        protected AsmLoad AsmLoad { get; set; }

        public LineDetailExpansionItem[] LineDetailExpansionItems { get; set; }
        public AsmResult[] BinResults => LineDetailExpansionItems.SelectMany(m => m.BinResults).ToArray();
        public AsmList[] Lists => LineDetailExpansionItems.Select(m => m.List).ToArray();

        /// <summary>
        /// 通常展開
        /// </summary>
        /// <param name="lineItem"></param>
        /// <param name="asmLoad"></param>
        public LineDetailScopeItem(LineItem lineItem, AsmLoad asmLoad)
        {
            AsmLoad = asmLoad;
            LineDetailExpansionItems = new[]
            {
                new LineDetailExpansionItemOperation(lineItem, AsmLoad)
            };
        }

        /// <summary>
        /// 特殊展開用
        /// </summary>
        /// <param name="lineDetailExpansionItems"></param>
        /// <param name="asmLoad"></param>
        public LineDetailScopeItem(LineDetailExpansionItem[] lineDetailExpansionItems, AsmLoad asmLoad)
        {
            AsmLoad = asmLoad;
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


        public virtual void AdjustAssemble(ref UInt32 outputAddress)
        {
            foreach (var lineDetailExpansionItem in LineDetailExpansionItems)
            {
                try
                {
                    lineDetailExpansionItem.AdjustAssemble(ref outputAddress);
                    outputAddress += lineDetailExpansionItem.Length.Output;
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
