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

        public AsmAddress? Address { get; protected set; }
        public LineDetailScopeItem[] LineDetailScopeItems { get; set; }
        //public virtual byte[] Bin => LineDetailScopeItems == default ? Array.Empty<byte>() : LineDetailScopeItems.SelectMany(m => m.Bin).ToArray();
        public virtual AsmResult[] BinResults => LineDetailScopeItems == default ? Array.Empty<AsmResult>() : LineDetailScopeItems.SelectMany(m => m.BinResults).ToArray();
        public virtual AsmList[] Lists => AsmLoad.Share.IsOutputList ? (LineDetailScopeItems == default ? new[] { AsmList.CreateLineItem(LineItem) } : LineDetailScopeItems.SelectMany(m => m.Lists).ToArray()) : new AsmList[] { };
        public List<ErrorLineItem> Errors { get; private set; } = new List<ErrorLineItem>();
        public int NestCounter { get; set; } = 0;

        protected LineDetailItem(LineItem lineItem, AsmLoad asmLoad)
        {
            LineItem = lineItem;
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
                    case LineDetailItemAddressAlignBlock:
                        lineDetailItem ??= LineDetailItemAddressAlignBlock.Create(lineItem, asmLoad);
                        break;
                    case LineDetailItemCheckAlign:
                        lineDetailItem ??= LineDetailItemCheckAlign.Create(lineItem, asmLoad);
                        break;
                    case LineDetailItemPreProcConditional:
                        lineDetailItem ??= LineDetailItemPreProcConditional.Create(lineItem, asmLoad);
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
                // ラベルの前処理を後処理を行うスコープ
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
                    lineDetailItem ??= LineDetailItemPreProcConditional.Create(lineItem, asmLoad);
                    lineDetailItem ??= LineDetailItemAddressORG.Create(lineItem, asmLoad);
                    lineDetailItem ??= LineDetailItemAddressAlign.Create(lineItem, asmLoad);
                    lineDetailItem ??= LineDetailItemAddressAlignBlock.Create(lineItem, asmLoad);
                    lineDetailItem ??= LineDetailItemAddressDS.Create(lineItem, asmLoad);
                    lineDetailItem ??= LineDetailItemCheckAlign.Create(lineItem, asmLoad);
                    lineDetailItem ??= LineDetailItemInclude.Create(lineItem, asmLoad);
                    lineDetailItem ??= LineDetailItemCharMap.Create(lineItem, asmLoad);
                    lineDetailItem ??= LineDetailItemPreProcPragma.Create(lineItem, asmLoad);
                    lineDetailItem ??= LineDetailItemPreProcError.Create(lineItem, asmLoad);
                    lineDetailItem ??= LineDetailItemPreProcPrint.Create(lineItem, asmLoad);
                    lineDetailItem ??= LineDetailItemPreProcList.Create(lineItem, asmLoad);
                    lineDetailItem ??= LineDetailItemEndDefine.Create(lineItem, asmLoad);
                    lineDetailItem ??= LineDetailItemMacro.Create(lineItem, asmLoad);
                    lineDetailItem ??= LineDetailItemInvalid.Create(lineItem, asmLoad); // 全角文字が命令に含まれていた時ここにたどり着く

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
            Address = new AsmAddress(Address.Value.Program, outputAddress);

            if (LineDetailScopeItems == default)
                return;

            foreach (var item in LineDetailScopeItems)
            {
                item.AdjustAssemble(ref outputAddress);
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

        public virtual void ValidateAssemble()
        {

        }
    }
}
