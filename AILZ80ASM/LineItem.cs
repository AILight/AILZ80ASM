using AILZ80ASM.AILight;
using AILZ80ASM.Assembler;
using AILZ80ASM.LineDetailItems;
using System.IO;

namespace AILZ80ASM
{
    public class LineItem
    {
        // ファイル情報
        public FileInfo FileInfo { get; set; }

        // レコード情報
        public string LineString { get; private set; }
        public int LineIndex { get; private set; }
        //public byte[] Bin => LineDetailItem.Bin;
        public AsmResult[] BinResults => LineDetailItem.BinResults;
        public AsmList[] Lists => LineDetailItem.Lists;

        // 展開情報
        public string LabelString { get; private set; }
        public string OperationString { get; private set; }
        public string CommentString { get; private set; }
        public LineDetailItem LineDetailItem { get; private set; }

        public LineItem(string lineString, int lineIndex, FileInfo fileInfo)
        {
            // ファイル情報
            FileInfo = fileInfo;

            // 読み込んだ情報
            LineString = lineString;
            LineIndex = lineIndex;

            //展開情報作成
            //コメントを処理する
            var indexCommnet = AIString.IndexOfSkipString(lineString, ';');
            var operationString = default(string);

            if (indexCommnet != -1)
            {
                CommentString = lineString.Substring(indexCommnet);
                operationString = lineString.Substring(0, indexCommnet).Trim();
            }
            else
            {
                operationString = lineString.Trim();
            }
            LabelString = Label.GetLabelText(operationString);
            OperationString = operationString.Substring(LabelString.Length).Trim();
        }

        public LineItem(LineItem lineItem)
        {
            FileInfo = lineItem.FileInfo;
            LineString = lineItem.LineString;
            LineIndex = lineItem.LineIndex;
            LabelString = lineItem.LabelString;
            OperationString = lineItem.OperationString;
            CommentString = lineItem.CommentString;
            LineDetailItem = lineItem.LineDetailItem;
        }

        public void SetLabel(string labelName)
        {
            LabelString = labelName;
        }

        public void ClearOperation()
        {
            OperationString = "";
            LineString = $"{LabelString} {CommentString}";
        }

        public void CreateLineDetailItem(AsmLoad asmLoad)
        {
            // LineDetailItem作成
            asmLoad.CreateScope(localAsmLoad => 
            {
                LineDetailItem = LineDetailItem.CreateLineDetailItem(this, localAsmLoad);
            });

        }

        public void ExpansionItem()
        {
            LineDetailItem.ExpansionItem();
        }

        public void PreAssemble(ref AsmAddress address)
        {
            LineDetailItem.PreAssemble(ref address);
        }

        public void Assemble()
        {
            LineDetailItem.Assemble();
        }
    }
}
