using AILZ80ASM.Assembler;
using AILZ80ASM.Exceptions;
using System;

namespace AILZ80ASM
{
    public class LineDetailExpansionItemOperation : LineDetailExpansionItem
    {
        public OperationItem OperationItem { get; private set; }

        public override byte[] Bin
        {
            get
            {
                return OperationItem == default(OperationItem) ? Array.Empty<byte>() : OperationItem.Bin;
            }
        }

        public override AsmList List
        {
            get
            {
                return OperationItem == default(OperationItem) ? AsmList.CreateLineItem(this.LineItem) : OperationItem.List;
            }
        }

        public LineDetailExpansionItemOperation(LineItem lineItem, AsmLoad asmLoad)
            : base(lineItem)
        {
            OperationItem = default(OperationItem);

            // ラベルを処理する
            Label = new Label(this, asmLoad);
            if (Label.Invalidate)
            {
                throw new ErrorAssembleException(Error.ErrorCodeEnum.E0013);
            }
            if (!string.IsNullOrEmpty(lineItem.LabelString))
            {
                asmLoad.AddLabel(Label);
                // ローカルラベルの末尾に「:」がついている場合にはワーニング
                if (lineItem.LabelString.StartsWith(".") && lineItem.LabelString.EndsWith(":"))
                {
                    asmLoad.Errors.Add(new ErrorLineItem(lineItem, Error.ErrorCodeEnum.W9005));
                }
            }
        }

        public override void PreAssemble(ref AsmAddress asmAddress, AsmLoad asmLoad)
        {
            base.PreAssemble(ref asmAddress, asmLoad);

            // ビルド済みの場合処理しない
            if (!IsAssembled)
            {
                if (string.IsNullOrEmpty(this.LineItem.OperationString))
                {
                    IsAssembled = true;
                }
                else
                {
                    // 命令を判別する
                    OperationItem = OperationItem.Create(this, asmAddress, asmLoad);

                    // Addressを設定
                    if (OperationItem != default(OperationItem))
                    {
                        Address = OperationItem.Address;
                        asmAddress = new AsmAddress(OperationItem.Address, OperationItem.Length);
                    }
                    else
                    {
                        var operationCode = LineItem.OperationString;
                        if (!string.IsNullOrEmpty(operationCode))
                        {
                            throw new ErrorAssembleException(Error.ErrorCodeEnum.E0001, $"{operationCode}");
                        }
                    }
                }
            }

            // ラベル設定
            Label.SetAddressLabel(Address);
        }

        public override void Assemble(AsmLoad asmLoad)
        {
            OperationItem?.Assemble(asmLoad);

            base.Assemble(asmLoad);
        }
    }
}
