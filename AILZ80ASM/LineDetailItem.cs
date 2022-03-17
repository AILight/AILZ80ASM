using AILZ80ASM.Assembler;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AILZ80ASM
{
    public abstract class LineDetailItem
    {
        public LineItem LineItem { get; private set; }
        protected AsmLoad AsmLoad { get; set; }

        public LineDetailScopeItem[] LineDetailScopeItems { get; set; }
        public virtual byte[] Bin => LineDetailScopeItems == default ? Array.Empty<byte>() : LineDetailScopeItems.SelectMany(m => m.Bin).ToArray();
        public virtual AsmList[] Lists => LineDetailScopeItems == default ? Array.Empty<AsmList>() : LineDetailScopeItems.SelectMany(m => m.Lists).ToArray();
        public List<ErrorLineItem> Errors { get; private set; } = new List<ErrorLineItem>();

        protected LineDetailItem(LineItem lineItem, AsmLoad asmLoad)
        {
            LineItem = lineItem;

            AsmLoad = asmLoad.Clone();
        }

        public static LineDetailItem CreateLineDetailItem(LineItem lineItem, AsmLoad asmLoad)
        {
            // インクルードのチェック
            var lineDetailItem = default(LineDetailItem);

            if (asmLoad.LineDetailItemForExpandItem != default)
            {
                if (asmLoad.LineDetailItemForExpandItem is LineDetailItemMacroDefineModern)
                {
                    lineDetailItem ??= LineDetailItemMacroDefineModern.Create(lineItem, asmLoad);
                }
                else if (asmLoad.LineDetailItemForExpandItem is LineDetailItemMacroDefineCompatible)
                {
                    lineDetailItem ??= LineDetailItemMacroDefineCompatible.Create(lineItem, asmLoad);
                }
                else if (asmLoad.LineDetailItemForExpandItem is LineDetailItemRepeatModern)
                {
                    lineDetailItem ??= LineDetailItemRepeatModern.Create(lineItem, asmLoad);
                }
                else if (asmLoad.LineDetailItemForExpandItem is LineDetailItemRepeatCompatible)
                {
                    lineDetailItem ??= LineDetailItemRepeatCompatible.Create(lineItem, asmLoad);
                }
                else if (asmLoad.LineDetailItemForExpandItem is LineDetailItemConditional)
                {
                    lineDetailItem ??= LineDetailItemConditional.Create(lineItem, asmLoad);
                }
                else
                {
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
                lineDetailItem ??= LineDetailItemEnd.Create(lineItem, asmLoad);
                lineDetailItem ??= LineDetailItemOperation.Create(lineItem, asmLoad);
                lineDetailItem ??= LineDetailItemEqual.Create(lineItem, asmLoad);
                lineDetailItem ??= LineDetailItemMacroDefineModern.Create(lineItem, asmLoad);
                lineDetailItem ??= LineDetailItemMacroDefineCompatible.Create(lineItem, asmLoad);
                lineDetailItem ??= LineDetailItemFunctionDefine.Create(lineItem, asmLoad);
                lineDetailItem ??= LineDetailItemRepeatModern.Create(lineItem, asmLoad);
                lineDetailItem ??= LineDetailItemRepeatCompatible.Create(lineItem, asmLoad);
                lineDetailItem ??= LineDetailItemConditional.Create(lineItem, asmLoad);
                lineDetailItem ??= LineDetailItemORG.Create(lineItem, asmLoad);
                lineDetailItem ??= LineDetailItemALIGN.Create(lineItem, asmLoad);
                lineDetailItem ??= LineDetailItemError.Create(lineItem, asmLoad);
                lineDetailItem ??= LineDetailItemInclude.Create(lineItem, asmLoad);
                lineDetailItem ??= LineDetailItemCharMap.Create(lineItem, asmLoad);
                lineDetailItem ??= LineDetailItemMacro.Create(lineItem, asmLoad);
                lineDetailItem ??= LineDetailItemInvalid.Create(lineItem, asmLoad); // ここには来ない
            }

            if (lineDetailItem is LineDetailItemORG itemORG)
            {
                asmLoad.AddORG(itemORG.AssembleORG);
            }

            return lineDetailItem;
        }

        public virtual void ExpansionItem()
        {

        }

        public virtual void PreAssemble(ref AsmAddress asmAddress)
        {
            if (LineDetailScopeItems == default)
                return;

            foreach (var item in LineDetailScopeItems)
            {
                item.PreAssemble(ref asmAddress);
            }
        }

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
