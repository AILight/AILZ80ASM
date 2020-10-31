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
        /* Configクラスへ移行
        private const int MAX_INCLUDE_NEST = 10;
        */

        public string OperationString { get; private set; }
        public string CommentString { get; private set; }
        //public Macro Macro { get; private set; }
        public Label Label { get; private set; }
        public IOperationItem OperationItem { get; private set; }
        public UInt16 Address { get; private set; }


        public byte[] Bin 
        {
            get 
            {
                return OperationItem == default(IOperationItem) ? new byte[] { } : OperationItem.Bin;
            } 
        }

        public LineItem(string lineString, int lineIndex, FileItem fileItem)
        {
            LineString = lineString;
            LineIndex = lineIndex;
            FileItem = fileItem;

            OperationItem = default(IOperationItem);

            //コメントを処理する
            var indexCommnet = lineString.IndexOf(';');
            if (indexCommnet != -1)
            {
                CommentString = lineString.Substring(indexCommnet);
                lineString = lineString.Substring(0, indexCommnet);
            }

            //命令を切り出す
            OperationString = lineString.TrimEnd();

            // ラベルを処理する
            Label = new Label(this);
        }

        public void PreAssemble(ref UInt16 address)
        {
            // Addressの設定
            Address = address;

            // 命令を判別する
            OperationItem = OperationItem ?? OperationItemOPCode.Perse(this, address);　// OpeCode
            OperationItem = OperationItem ?? OperationItemInclude.Perse(this, address); // Include
            OperationItem = OperationItem ?? OperationItemSystem.Perse(this, address); // System

            // Addressを設定
            if (OperationItem != default(IOperationItem))
            {
                Address = OperationItem.Address;
                address = OperationItem.NextAddress;
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
    }
}
