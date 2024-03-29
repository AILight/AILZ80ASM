﻿using AILZ80ASM.AILight;
using AILZ80ASM.Assembler;
using AILZ80ASM.Exceptions;
using System;
using System.IO;
using System.Text.RegularExpressions;

namespace AILZ80ASM.LineDetailItems
{
    public abstract class LineDetailItemAddress : LineDetailItem
    {
        public override AsmList[] Lists
        {
            get
            {
                if (!AsmLoad.Share.IsOutputList)
                {
                    return new AsmList[] { };
                }

                return new[] { Address.HasValue ? AsmList.CreateLineItemORG(Address.Value, new AsmLength(), LineItem) : AsmList.CreateLineItem(LineItem) };
            }
        }

        protected LineDetailItemAddress(LineItem lineItem, AsmLoad asmLoad)
            : base(lineItem, asmLoad)
        {
        }

        public override void Assemble()
        {
        }

        public override void PreAssemble(ref AsmAddress asmAddress)
        {
            Address = asmAddress;
            //base.PreAssemble(ref asmAddress);
        }

        public override void ExpansionItem()
        {
        }
    }
}
