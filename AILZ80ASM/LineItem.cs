using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace AILZ80ASM
{
    public class LineItem
    {
        private string LineString { get; set; }
        public int LineIndex { get; private set; }
        public FileItem FileItem { get; private set; }

        public string OperationString { get; private set; }
        public string CommentString { get; private set; }

        public string LabelText { get; private set; }
        public string InstructionText { get; private set; }
        public string ArgumentText { get; private set; }
        public bool IsAssembled { get; set; } = false;

        private static readonly string RegexPatternInstruction = @"(?<Instruction>(^[\w\(\)]+))";

        public List<LineExpansionItem> LineExpansionItems { get; private set; } = new List<LineExpansionItem>();

        public byte[] Bin 
        {
            get 
            {
                var bytes = new List<byte>();

                foreach (var item in LineExpansionItems)
                {
                    if (item.Bin != default(byte[]))
                    {
                        bytes.AddRange(item.Bin);
                    }
                }

                return bytes.ToArray();
            } 
        }

        public LineItem(string lineString, int lineIndex, FileItem fileItem)
        {
            LineString = lineString;
            LineIndex = lineIndex;
            FileItem = fileItem;

            //コメントを処理する
            var indexCommnet = lineString.IndexOf(';');
            if (indexCommnet != -1)
            {
                CommentString = lineString.Substring(indexCommnet);
                lineString = lineString.Substring(0, indexCommnet);
            }

            // 命令を切り出す
            OperationString = lineString.TrimEnd();

            //ラベルの切り出し
            LabelText= Label.GetLabelText(OperationString);
            
            // 命令の切りだし
            var tmpInstructionText = OperationString.Substring(LabelText.Length).TrimStart();
            if (!string.IsNullOrEmpty(tmpInstructionText))
            {
                var matchResult = Regex.Match(tmpInstructionText, RegexPatternInstruction, RegexOptions.Singleline);
                if (matchResult.Success)
                {
                    InstructionText = matchResult.Groups["Instruction"].Value;
                    ArgumentText = tmpInstructionText.Substring(InstructionText.Length);
                }
            }
        }

        public void PreAssemble(ref AsmAddress address)
        {
            if (IsAssembled)
                return;

            foreach (var item in LineExpansionItems)
            {
                item.PreAssemble(ref address);
            }
        }

        public void SetValueLabel(Label[] labels)
        {
            if (IsAssembled)
                return;

            foreach (var item in LineExpansionItems)
            {
                item.SetValueLabel(labels);
            }
        }

        public void Assemble(Label[] labels)
        {
            if (IsAssembled)
                return;

            foreach (var item in LineExpansionItems)
            {
                item.Assemble(labels);
            }
        }

        /// <summary>
        /// マクロを展開する
        /// </summary>
        /// <param name="macro"></param>
        public void ExpansionMacro(Macro macro)
        {
            foreach (var item in macro.LineItems)
            {
                LineExpansionItems.Add(new LineExpansionItem(item.LineString, LineIndex, item.FileItem));
            }
        }

        /// <summary>
        /// 通常命令の展開
        /// </summary>
        public void ExpansionItem()
        {
            LineExpansionItems.Add(new LineExpansionItem(LineString, LineIndex, FileItem));
        }
    }
}
