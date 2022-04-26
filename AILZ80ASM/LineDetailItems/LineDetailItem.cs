using AILZ80ASM.Assembler;
using AILZ80ASM.Exceptions;
using AILZ80ASM.LineDetailItems.ScopeItem;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AILZ80ASM.LineDetailItems
{
    public abstract class LineDetailItem
    {
        public LineItem LineItem { get; private set; }
        protected AsmLoad AsmLoad { get; set; }
        
        public AsmAddress Address { get; protected set; }
        public LineDetailScopeItem[] LineDetailScopeItems { get; set; }
        public virtual byte[] Bin => LineDetailScopeItems == default ? Array.Empty<byte>() : LineDetailScopeItems.SelectMany(m => m.Bin).ToArray();
        public virtual AsmResult[] BinResults => LineDetailScopeItems == default ? Array.Empty<AsmResult>() : LineDetailScopeItems.SelectMany(m => m.BinResults).ToArray();
        public virtual AsmList[] Lists => LineDetailScopeItems == default ? Array.Empty<AsmList>() : LineDetailScopeItems.SelectMany(m => m.Lists).ToArray();
        public List<ErrorLineItem> Errors { get; private set; } = new List<ErrorLineItem>();

        protected LineDetailItem(LineItem lineItem, AsmLoad asmLoad)
        {
            LineItem = lineItem;

            //AsmLoad = asmLoad.Clone();
            AsmLoad = asmLoad;

        }

        public static LineDetailItem CreateLineDetailItem(LineItem lineItem, AsmLoad asmLoad)
        {
            // インクルードのチェック
            var lineDetailItem = default(LineDetailItem);

            if (asmLoad.Share.LineDetailItemForExpandItem != default)
            {
                switch (asmLoad.Share.LineDetailItemForExpandItem)
                {
                    case LineDetailItemMacroDefineModern:
                        lineDetailItem ??= LineDetailItemMacroDefineModern.Create(lineItem, asmLoad);
                        break;
                    case LineDetailItemMacroDefineCompatible:
                        lineDetailItem ??= LineDetailItemMacroDefineCompatible.Create(lineItem, asmLoad);
                        break;
                    case LineDetailItemRepeatModern:
                        lineDetailItem ??= LineDetailItemRepeatModern.Create(lineItem, asmLoad);
                        break;
                    case LineDetailItemRepeatCompatible:
                        lineDetailItem ??= LineDetailItemRepeatCompatible.Create(lineItem, asmLoad);
                        break;
                    case LineDetailItemConditional:
                        lineDetailItem ??= LineDetailItemConditional.Create(lineItem, asmLoad);
                        break;
                    default:
                        throw new NotImplementedException();
                }
                /* 例外が切れるので、この処理は使わない
                var methodInfo = asmLoad.LineDetailItemForExpandItem.GetType().GetMethod("Create");
                if (methodInfo != null)
                {
                    lineDetailItem = (LineDetailItem)methodInfo.Invoke(null, new object[] { lineItem, asmLoad });
                }
                */
            }
            else
            {
                asmLoad.ProcessLabel(() =>
                {
                    lineDetailItem ??= LineDetailItemEnd.Create(lineItem, asmLoad);
                    lineDetailItem ??= LineDetailItemOperation.Create(lineItem, asmLoad);
                    lineDetailItem ??= LineDetailItemEqual.Create(lineItem, asmLoad);
                    lineDetailItem ??= LineDetailItemMacroDefineModern.Create(lineItem, asmLoad);
                    lineDetailItem ??= LineDetailItemMacroDefineCompatible.Create(lineItem, asmLoad);
                    lineDetailItem ??= LineDetailItemFunctionDefine.Create(lineItem, asmLoad);
                    lineDetailItem ??= LineDetailItemRepeatModern.Create(lineItem, asmLoad);
                    lineDetailItem ??= LineDetailItemRepeatCompatible.Create(lineItem, asmLoad);
                    lineDetailItem ??= LineDetailItemConditional.Create(lineItem, asmLoad);
                    lineDetailItem ??= LineDetailItemAddressORG.Create(lineItem, asmLoad);
                    lineDetailItem ??= LineDetailItemAddressALIGN.Create(lineItem, asmLoad);
                    lineDetailItem ??= LineDetailItemAddressDS.Create(lineItem, asmLoad);
                    lineDetailItem ??= LineDetailItemError.Create(lineItem, asmLoad);
                    lineDetailItem ??= LineDetailItemInclude.Create(lineItem, asmLoad);
                    lineDetailItem ??= LineDetailItemCharMap.Create(lineItem, asmLoad);
                    lineDetailItem ??= LineDetailItemPragma.Create(lineItem, asmLoad);
                    lineDetailItem ??= LineDetailItemMacro.Create(lineItem, asmLoad);
                    lineDetailItem ??= LineDetailItemInvalid.Create(lineItem, asmLoad); // ここには来ない

                    return lineDetailItem;

                }, lineItem);

            }

            return lineDetailItem;
        }

        public virtual void ExpansionItem()
        {

        }

        public virtual void PreAssemble(ref AsmAddress asmAddress)
        {
            Address = asmAddress;
            AsmLoad.AddLineDetailItem(this);

            if (LineDetailScopeItems == default)
                return;

            foreach (var item in LineDetailScopeItems)
            {
                item.PreAssemble(ref asmAddress);
            }
        }

        public virtual void AdjustAssemble(ref UInt32 outputAddress)
        {
            Address = new AsmAddress(Address.Program, outputAddress);

            if (LineDetailScopeItems == default)
                return;

            foreach (var item in LineDetailScopeItems)
            {
                item.AdjustAssemble(ref outputAddress);
            }
        }

        /*
        public virtual void ResetOutputAddress(ref AsmAddress asmAddress)
        {
            Address = asmAddress;

            if (LineDetailScopeItems == default)
                return;

            foreach (var item in LineDetailScopeItems)
            {
                item.ResetOutputAddress(ref asmAddress);
            }
        }
        */


        public virtual void Assemble()
        {
            if (LineDetailScopeItems == default)
                return;

            foreach (var item in LineDetailScopeItems)
            {
                item.Assemble();
            }
        }
    }
}
