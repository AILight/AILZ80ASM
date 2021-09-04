using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace AILZ80ASM
{
    public class LineDetailItemOperation : LineDetailItem
    {
        public LineDetailItemOperation(LineItem lineItem)
            : base(lineItem)
        {
        }

        public override void ExpansionItem(AsmLoad asmLoad)
        {
            this.LineDetailExpansionItems = Macro.Expansion(this.LineItem.OperationString, asmLoad);
            if (this.LineDetailExpansionItems == default)
                return;
            // マクロ展開できなかったら通常展開
            this.LineDetailExpansionItems = new[]
            {
                new LineDetailExpansionItemOperation(this.LineItem, asmLoad)
            };

            base.ExpansionItem(asmLoad);
        }

        //public void PreAssemble(ref AsmAddress address, Label[] labels)
        //{
        //    if (IsAssembled)
        //        return;

        //    foreach (var item in LineExpansionItems)
        //    {
        //        item.PreAssemble(ref address, labels);
        //    }
        //}

        //public void SetValueLabel(Label[] labels)
        //{
        //    if (IsAssembled)
        //        return;

        //    foreach (var item in LineExpansionItems)
        //    {
        //        item.SetValueLabel(labels);
        //    }
        //}

        //public void Assemble(Label[] labels)
        //{
        //    if (IsAssembled)
        //        return;

        //    foreach (var item in LineExpansionItems)
        //    {
        //        item.Assemble(labels);
        //    }
        //}

        ///// <summary>
        ///// マクロを展開する
        ///// </summary>
        ///// <param name="macro"></param>
        //public void ExpansionMacro(Macro macro)
        //{
        //    foreach (var item in macro.LineItems.Select((Value, Index) => new { Value, Index}))
        //    {
        //        LineExpansionItems.Add(new LineExpansionItem(item.Value.LabelText, item.Value.InstructionText, item.Value.ArgumentText, item.Index + 1, this));
        //    }
        //}

        ///// <summary>
        ///// 通常命令の展開
        ///// </summary>
        //public void ExpansionItem()
        //{
        //    LineExpansionItems.Add(new LineExpansionItem(LabelText, InstructionText, ArgumentText, LineIndex, this));
        //}

        ///// <summary>
        ///// ラベルを処理するする（値）
        ///// </summary>
        //public void ProcessLabelValue(Label[] labels)
        //{
        //    foreach (var item in LineExpansionItems)
        //    {
        //        item.ProcessLabelValue(labels);
        //    }
        //}

        ///// <summary>
        ///// ラベルを処理するする（値とアドレス）
        ///// </summary>
        //public void ProcessLabelValueAndAddress(Label[] labels)
        //{
        //    foreach (var item in LineExpansionItems)
        //    {
        //        item.ProcessLabelValueAndAddress(labels);
        //    }
        //}

    }
}
