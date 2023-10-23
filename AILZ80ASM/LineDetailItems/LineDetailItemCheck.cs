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
        //public override AsmResult[] BinResults => FilteredLineItems.SelectMany(m => m.BinResults)?.ToArray() ?? new AsmResult[] { };
        //public override AsmList[] Lists => LineItemDic.SelectMany(m => { if (m.Value == default) { return m.Key.Lists; } else { return m.Value.Lists; } })?.ToArray() ?? new AsmList[] { };

        //protected Dictionary<LineDetailItemCheck, LineItem> LineItemDic { get; set; } = new();

        protected LineDetailItemCheck[] FilteredDetailItemChecks => LineDetailItemCheckList.Where(m => m.EnableAssemble).ToArray();
        protected Dictionary<LineItem> LineItems = new();
        private bool EnableAssemble { get; set; } = false;
        private LineDetailItem BaseLineDetailItem { get; set; } = default;

        protected LineDetailItemCheck(LineItem lineItem, AsmLoad asmLoad)
            : base(lineItem, asmLoad)
        {
            BlockStartLineDetailItemCheck = this;
        }


        protected LineDetailItemCheck(LineItem lineItem, AsmLoad asmLoad, bool enableAssemble, LineDetailItemCheck blockStartLineDetailItemCheck)
            : base(lineItem, asmLoad)
        {
            BlockStartLineDetailItemCheck = blockStartLineDetailItemCheck;
            BlockStartLineDetailItemCheck.LineDetailItemCheckList.Add(this);
            this.EnableAssemble = enableAssemble;
        }

        public override void ExpansionItem()
        {
            if (this.EnableAssemble)
            {
                try
                {
                    BaseLineDetailItem = LineDetailItem.CreateLineDetailItem(this.LineItem, AsmLoad);
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
            else
            {
                BaseLineDetailItem = this;
            }
            BaseLineDetailItem.ExpansionItem();

        }

        public override void AdjustAssemble(ref uint outputAddress)
        {
            if (this.EnableAssemble)
            {
                BaseLineDetailItem.AdjustAssemble(ref outputAddress);
            }
        }

        public override void PreAssemble(ref AsmAddress asmAddress)
        {
            if (this.EnableAssemble)
            {
                BaseLineDetailItem.PreAssemble(ref asmAddress);
            }
        }

        public override void Assemble()
        {
            if (this.EnableAssemble)
            {
                BaseLineDetailItem.Assemble();
            }
        }
    }
}
