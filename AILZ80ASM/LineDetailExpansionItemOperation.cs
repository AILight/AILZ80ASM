using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace AILZ80ASM
{
    public class LineDetailExpansionItemOperation : LineDetailExpansionItem
    {
        //public int LineIndex { get; private set; }
        //public LineItem LineItem { get; private set; }

        public string LabelText { get; private set; }
        public string InstructionText { get; private set; }
        public string ArgumentText { get; private set; }


        public IOperationItem OperationItem { get; private set; }
        //public AsmAddress Address { get; private set; }
        //public bool IsAssembled { get; set; } = false;

        private static readonly string RegexPatternInstruction = @"(?<Instruction>(^[\w\(\)]+))";


        public override byte[] Bin 
        {
            get 
            {
                return OperationItem == default(IOperationItem) ? new byte[] { } : OperationItem.Bin;
            } 
        }

        public LineDetailExpansionItemOperation(LineItem lineItem, AsmLoad asmLoad)
        {
            //ラベルの切り出し
            LabelText= Label.GetLabelText(lineItem.LineString);

            // 命令の切りだし
            var tmpInstructionText = lineItem.LineString.Substring(LabelText.Length).TrimStart();
            if (!string.IsNullOrEmpty(tmpInstructionText))
            {
                var matchResult = Regex.Match(tmpInstructionText, RegexPatternInstruction, RegexOptions.Singleline);
                if (matchResult.Success)
                {
                    InstructionText = matchResult.Groups["Instruction"].Value;
                    ArgumentText = tmpInstructionText.Substring(InstructionText.Length).TrimStart();
                }
            }

            OperationItem = default(IOperationItem);

            // ラベルを処理する
            Label = new Label(this, asmLoad);
        }

    }
}
