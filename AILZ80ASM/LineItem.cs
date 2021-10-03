﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace AILZ80ASM
{
    public class LineItem
    {
        // ファイル情報
        public FileInfo FileInfo { get; set; }

        // レコード情報
        public string LineString { get; private set; }
        public int LineIndex { get; private set; }
        public byte[] Bin => LineDetailItem.Bin;

        // 展開情報
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
        }

        public LineItem(LineItem lineItem)
        {
            FileInfo = lineItem.FileInfo;
            LineString = lineItem.LineString;
            LineIndex = lineItem.LineIndex;
            OperationString = lineItem.OperationString;
            CommentString = lineItem.CommentString;
            LineDetailItem = lineItem.LineDetailItem;
        }

        public void SetLabelForMacro(string labelName)
        {
            OperationString = labelName;
        }

        public void CreateLineDetailItem(AsmLoad asmLoad)
        {
            // LineDetailItem作成
            LineDetailItem = LineDetailItem.CreateLineDetailItem(this, asmLoad);
        }

        public void ExpansionItem()
        {
            LineDetailItem.ExpansionItem();
        }

        public void PreAssemble(ref AsmAddress address)
        {
            LineDetailItem.PreAssemble(ref address);
        }

        public void BuildAddressLabel()
        {
            LineDetailItem.BuildAddressLabel();
        }

        public void BuildArgumentLabel()
        {
            LineDetailItem.BuildArgumentLabel();
        }

        public void BuildValueLabel()
        {
            LineDetailItem.BuildValueLabel();
        }

        public void Assemble()
        {
            LineDetailItem.Assemble();
        }

    }
}
