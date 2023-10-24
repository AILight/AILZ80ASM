using AILZ80ASM.AILight;
using AILZ80ASM.Assembler;
using AILZ80ASM.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using static AILZ80ASM.Assembler.Error;

namespace AILZ80ASM.LineDetailItems
{
    public abstract class LineDetailItemCheck : LineDetailItem
    {
        public class Detail
        {
            public bool EnableAssemble { get; set; } = false;
            public LineDetailItem TargetLineDetailItem { get; set; } = default;
        }
        
        public override AsmResult[] BinResults => LineDetailItems.SelectMany(m => m.TargetLineDetailItem?.BinResults ?? base.BinResults)?.ToArray() ?? new AsmResult[] { };
        public override AsmList[] Lists => LineDetailItems.SelectMany(m => { if (m.TargetLineDetailItem == default) { return base.Lists; } else if (m.TargetLineDetailItem is LineDetailItemCheck lineDetailItemCheck) { return lineDetailItemCheck.BaseLists; } else { return m.TargetLineDetailItem.Lists; } })?.ToArray() ?? new AsmList[] { };

        protected List<LineDetailItemCheck.Detail> LineDetailItems = new();
        private AsmList[] BaseLists => base.Lists;

        protected LineDetailItemCheck(LineItem lineItem, AsmLoad asmLoad)
            : base(lineItem, asmLoad)
        {
        }

        public override void ExpansionItem()
        {
            foreach (var item in LineDetailItems)
            {
                if (item.EnableAssemble)
                {
                    try
                    {
                        var lineItem = new LineItem(item.TargetLineDetailItem.LineItem);
                        item.TargetLineDetailItem = LineDetailItem.CreateLineDetailItem(lineItem, AsmLoad);
                    }
                    catch (ErrorAssembleException ex)
                    {
                        this.AsmLoad.AddError(new ErrorLineItem(this.LineItem, ex));
                    }
                    catch (ErrorLineItemException ex)
                    {
                        this.AsmLoad.AddError(ex.ErrorLineItem);
                    }
                    catch (Exception ex)
                    {
                        this.AsmLoad.AddError(new ErrorLineItem(this.LineItem, new ErrorAssembleException(Error.ErrorCodeEnum.E0000, ex.Message)));
                    }
                }
            }

            foreach (var item in LineDetailItems)
            {
                if (item.EnableAssemble)
                {
                    try
                    {
                        item.TargetLineDetailItem.ExpansionItem();
                    }
                    catch (ErrorAssembleException ex)
                    {
                        this.AsmLoad.AddError(new ErrorLineItem(this.LineItem, ex));
                    }
                    catch (ErrorLineItemException ex)
                    {
                        this.AsmLoad.AddError(ex.ErrorLineItem);
                    }
                    catch (Exception ex)
                    {
                        this.AsmLoad.AddError(new ErrorLineItem(this.LineItem, new ErrorAssembleException(Error.ErrorCodeEnum.E0000, ex.Message)));
                    }
                }
            }
        }

        public override void AdjustAssemble(ref uint outputAddress)
        {
            foreach (var item in LineDetailItems)
            {
                if (item.EnableAssemble)
                {
                    item.TargetLineDetailItem.AdjustAssemble(ref outputAddress);
                }
            }
        }

        public override void PreAssemble(ref AsmAddress asmAddress)
        {
            foreach (var item in LineDetailItems)
            {
                if (item.EnableAssemble)
                {
                    item.TargetLineDetailItem.PreAssemble(ref asmAddress);
                }
            }
        }

        public override void Assemble()
        {
            foreach (var item in LineDetailItems)
            {
                if (item.EnableAssemble)
                {
                    item.TargetLineDetailItem.Assemble();
                }
            }
        }
    }
}
