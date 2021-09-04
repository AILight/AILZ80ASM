using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace AILZ80ASM
{
    public class LineExpansionItem
    {
        public int LineIndex { get; private set; }
        public LineItem LineItem { get; private set; }

        public string LabelText { get; private set; }
        public string InstructionText { get; private set; }
        public string ArgumentText { get; private set; }

        public Label Label { get; private set; }
        public IOperationItem OperationItem { get; private set; }
        public AsmAddress Address { get; private set; }
        public bool IsAssembled { get; set; } = false;

        public byte[] Bin 
        {
            get 
            {
                return OperationItem == default(IOperationItem) ? new byte[] { } : OperationItem.Bin;
            } 
        }

        public LineExpansionItem(string labelText, string instructionText, string argumentText, int lineIndex, LineItem lineItem)
        {
            LabelText = labelText;
            InstructionText = instructionText;
            ArgumentText = argumentText;

            LineItem = lineItem;

            OperationItem = default(IOperationItem);

            // ラベルを処理する
            //Label = new Label(this);
        }

        public void PreAssemble(ref AsmAddress address, Label[] labels)
        {
            // Addressの設定
            Address = address;

            // ビルド済みの場合処理しない
            if (!IsAssembled)
            {
                if (string.IsNullOrEmpty(InstructionText))
                {
                    IsAssembled = true;
                }
                else
                {
                    // 命令を判別する
                    OperationItem = OperationItem ?? OperationItemOPCode.Parse(this, address, labels); // OpeCode
                    OperationItem = OperationItem ?? OperationItemData.Parse(this, address, labels);   // Data
                    OperationItem = OperationItem ?? OperationItemInclude.Parse(this, address, labels); // Include
                    OperationItem = OperationItem ?? OperationItemSystem.Parse(this, address, labels);  // System

                    // Addressを設定
                    if (OperationItem != default(IOperationItem))
                    {
                        Address = OperationItem.Address;
                        address = new AsmAddress(OperationItem.Address, OperationItem.Length);
                    }
                    else
                    {
                        var operationCode = LineItem.OperationString;
                        if (!string.IsNullOrEmpty(operationCode))
                        {
                            throw new ErrorMessageException(Error.ErrorCodeEnum.E0001, $"{operationCode}");
                        }
                    }
                }
            }

            // ラベル設定
            Label.SetAddressLabel(Address);
        }

        public void SetValueLabel(Label[] labels)
        {
            Label.SetValueLabel(Address, labels);
        }

        public void Assemble(Label[] labels)
        {
            OperationItem?.Assemble(labels);
        }

        public void ProcessLabelValue(Label[] labels)
        {
            Label.SetValue(labels);
        }

        public void ProcessLabelValueAndAddress(Label[] labels)
        {
            Label.SetValueAndAddress(Address, labels);
        }
    }
}
