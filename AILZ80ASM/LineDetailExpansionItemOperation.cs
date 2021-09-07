using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace AILZ80ASM
{
    public class LineDetailExpansionItemOperation : LineDetailExpansionItem
    {
        public string LabelText { get; private set; }
        public string InstructionText { get; private set; }
        public string ArgumentText { get; private set; }
        public IOperationItem OperationItem { get; private set; }

        private static readonly string RegexPatternInstruction = @"(?<Instruction>(^[\w\(\)]+))";


        public override byte[] Bin 
        {
            get 
            {
                return OperationItem == default(IOperationItem) ? Array.Empty<byte>() : OperationItem.Bin;
            } 
        }

        public LineDetailExpansionItemOperation(LineItem lineItem, AsmLoad asmLoad)
            : base(lineItem)
        {
            //ラベルの切り出し
            LabelText = Label.GetLabelText(lineItem.OperationString);

            // 命令の切りだし
            var tmpInstructionText = lineItem.OperationString.Substring(LabelText.Length).TrimStart();
            if (!string.IsNullOrEmpty(tmpInstructionText))
            {
                var matchResult = Regex.Match(tmpInstructionText, RegexPatternInstruction, RegexOptions.Singleline);
                if (matchResult.Success)
                {
                    InstructionText = matchResult.Groups["Instruction"].Value;
                    ArgumentText = tmpInstructionText.Substring(InstructionText.Length).TrimStart();
                }
                else
                {
                    throw new ErrorMessageException(Error.ErrorCodeEnum.E0001);
                }
            }

            OperationItem = default(IOperationItem);

            // ラベルを処理する
            Label = new Label(this, asmLoad);
        }

        public override void PreAssemble(ref AsmAddress asmAddress, AsmLoad asmLoad)
        {
            base.PreAssemble(ref asmAddress, asmLoad);

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
                    OperationItem ??= OperationItemOPCode.Parse(this, asmAddress, asmLoad); // OpeCode
                    OperationItem ??= OperationItemData.Parse(this, asmAddress, asmLoad);   // Data
                    OperationItem ??= OperationItemSystem.Parse(this, asmAddress, asmLoad);  // System

                    // Addressを設定
                    if (OperationItem != default(IOperationItem))
                    {
                        Address = OperationItem.Address;
                        asmAddress = new AsmAddress(OperationItem.Address, OperationItem.Length);
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
            if (Label.HasValue)
            {
                asmLoad.AddLabel(Label);
            }
        }

        public override void Assemble(AsmLoad asmLoad)
        {
            OperationItem?.Assemble(asmLoad);

            base.Assemble(asmLoad);
        }
    }
}
