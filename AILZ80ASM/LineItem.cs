using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace AILZ80ASM
{
    public class LineItem
    {
        // ファイル情報
        public string LineString { get; private set; }
        public int LineIndex { get; private set; }
        //public FileItem FileItem { get; private set; }

        // 展開情報
        public string OperationString { get; private set; }
        public string CommentString { get; private set; }
        public LineDetailItem LineDetailItem { get; private set; }

        public LineItem(string lineString, int lineIndex, AsmLoad asmLoad)
        {
            // 読み込んだ情報
            LineString = lineString;
            LineIndex = lineIndex;
            //FileItem = fileItem;

            //展開情報作成
            //コメントを処理する
            var indexCommnet = lineString.IndexOf(';');
            if (indexCommnet != -1)
            {
                CommentString = lineString.Substring(indexCommnet);
                OperationString = lineString.Substring(0, indexCommnet).TrimEnd();
            }
            else
            {
                OperationString = lineString.TrimEnd();
            }
            // LineDetailItem作成
            LineDetailItem = LineDetailItem.CreateLineDetailItem(OperationString, asmLoad);
        }

        public LineAssemblyItem[] LineAssemblyItems
        {
            get 
            {
                return LineDetailItem.LineAssemblyItems;
            }
        }


        //public Label[] Labels => LineExpansionItems.Where(m => m.Label.DataType != Label.DataTypeEnum.None).Select(m => m.Label).ToArray();

        //public string OperationString { get; private set; }
        //public string CommentString { get; private set; }

        //public string LabelText { get; private set; }
        //public string InstructionText { get; private set; }
        //public string ArgumentText { get; private set; }
        //public bool IsAssembled { get; set; } = false;

        //private static readonly string RegexPatternInstruction = @"(?<Instruction>(^[\w\(\)]+))";

        //public List<LineExpansionItem> LineExpansionItems { get; private set; } = new List<LineExpansionItem>();

        //public byte[] Bin
        //{
        //    get
        //    {
        //        var bytes = new List<byte>();

        //        foreach (var item in LineExpansionItems)
        //        {
        //            if (item.Bin != default(byte[]))
        //            {
        //                bytes.AddRange(item.Bin);
        //            }
        //        }

        //        return bytes.ToArray();
        //    }
        //}

        ////public LineItem(string lineString, int lineIndex, FileItem fileItem)
        ////{
        ////    /*
        ////    LineString = lineString;
        ////    LineIndex = lineIndex;
        ////    FileItem = fileItem;

        ////    //コメントを処理する
        ////    var indexCommnet = lineString.IndexOf(';');
        ////    if (indexCommnet != -1)
        ////    {
        ////        CommentString = lineString.Substring(indexCommnet);
        ////        lineString = lineString.Substring(0, indexCommnet);
        ////    }

        ////    // 命令を切り出す
        ////    OperationString = lineString.Trim();

        ////    //ラベルの切り出し
        ////    LabelText= Label.GetLabelText(OperationString);

        ////    // 命令の切りだし
        ////    var tmpInstructionText = OperationString.Substring(LabelText.Length).TrimStart();
        ////    if (!string.IsNullOrEmpty(tmpInstructionText))
        ////    {
        ////        var matchResult = Regex.Match(tmpInstructionText, RegexPatternInstruction, RegexOptions.Singleline);
        ////        if (matchResult.Success)
        ////        {
        ////            InstructionText = matchResult.Groups["Instruction"].Value;
        ////            ArgumentText = tmpInstructionText.Substring(InstructionText.Length).TrimStart();
        ////        }
        ////    }
        ////    */
        ////}

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
        //    foreach (var item in macro.LineItems.Select((Value, Index) => new { Value, Index }))
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
