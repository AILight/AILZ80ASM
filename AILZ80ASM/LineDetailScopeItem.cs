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
        public AsmList[] Lists => LineDetailExpansionItems.Select(m => m.List).ToArray();

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
                    AsmLoad.Errors.Add(new ErrorLineItem(lineDetailExpansionItem.LineItem, ex));
                }
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

        public virtual void BuildArgumentLabel()
        {
            if (LineDetailExpansionItems == default)
                return;

            foreach (var label in AsmLoad.LocalLabels.Where(m => m.DataType == Label.DataTypeEnum.ProcessingForArgument))
            {
                label.SetArgument();
            }

        }

        public virtual void BuildValueLabel()
        {
            var labels = AsmLoad.AllLabels.Where(m => m.DataType == Label.DataTypeEnum.ProcessingForValue);

            foreach (var label in labels)
            {
                label.SetValue(AsmLoad);
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
                    AsmLoad.Errors.Add(new ErrorLineItem(lineDetailExpansionItem.LineItem, ex));
                }
            }
        }
    }
}
