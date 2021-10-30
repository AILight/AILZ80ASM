using System;
using System.Text.RegularExpressions;

namespace AILZ80ASM
{
    public class LineDetailExpansionItemOperation : LineDetailExpansionItem
    {
        public string LabelText { get; private set; }
        public string InstructionText { get; private set; }
        public string ArgumentText { get; private set; }
        public OperationItem OperationItem { get; private set; }

        private static readonly string RegexPatternInstruction = @"(?<Instruction>(^[\w\(\)]+))";


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
            //ラベルの切り出し
            LabelText = lineItem.LabelString;

            // 命令の切りだし
            if (!string.IsNullOrEmpty(lineItem.OperationString))
            {
                var matchResult = Regex.Match(lineItem.OperationString, RegexPatternInstruction, RegexOptions.Singleline | RegexOptions.IgnoreCase);
                if (matchResult.Success)
                {
                    InstructionText = matchResult.Groups["Instruction"].Value;
                    ArgumentText = lineItem.OperationString.Substring(InstructionText.Length).TrimStart();
                }
                else
                {
                    throw new ErrorAssembleException(Error.ErrorCodeEnum.E0001);
                }
            }

            OperationItem = default(OperationItem);

            // ラベルを処理する
            Label = new Label(this, asmLoad);
            if (Label.Invalidate)
            {
                throw new ErrorAssembleException(Error.ErrorCodeEnum.E0013);
            }
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
