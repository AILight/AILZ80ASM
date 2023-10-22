using AILZ80ASM.AILight;
using AILZ80ASM.Assembler;
using AILZ80ASM.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using static AILZ80ASM.Assembler.Error;

namespace AILZ80ASM.LineDetailItems
{
    public abstract class LineDetailItemCheck : LineDetailItem
    {
        protected readonly Dictionary<LineDetailItemCheck, LineDetailItem> LineDetailItemDic = new ();

        protected LineDetailItemCheck(LineItem lineItem, AsmLoad asmLoad)
            : base(lineItem, asmLoad)
        {

        }

        public override void Assemble()
        {
            if (LineDetailItemDic.ContainsKey(this))
            {
                LineDetailItemDic[this].Assemble();
            }
        }

        public override void ExpansionItem()
        {
            if (LineDetailItemDic.ContainsKey(this))
            {
                LineDetailItemDic[this].ExpansionItem();
            }
        }

        public override void AdjustAssemble(ref uint outputAddress)
        {
            if (LineDetailItemDic.ContainsKey(this))
            {
                LineDetailItemDic[this].AdjustAssemble(ref outputAddress);
            }
        }

        public override void PreAssemble(ref AsmAddress asmAddress)
        {
            if (LineDetailItemDic.ContainsKey(this))
            {
                LineDetailItemDic[this].PreAssemble(ref asmAddress);
            }
        }
    }
}
