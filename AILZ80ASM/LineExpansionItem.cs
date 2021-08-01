﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace AILZ80ASM
{
    public class LineExpansionItem
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
        public AsmAddress Address { get; private set; }


        public byte[] Bin 
        {
            get 
            {
                return OperationItem == default(IOperationItem) ? new byte[] { } : OperationItem.Bin;
            } 
        }

        public LineExpansionItem(string lineString, int lineIndex, FileItem fileItem)
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

        public void PreAssemble(ref AsmAddress address)
        {
            // Addressの設定
            Address = address;

            // 命令を判別する
            OperationItem = OperationItem ?? OperationItemOPCode.Parse(this, address);　// OpeCode
            OperationItem = OperationItem ?? OperationItemData.Parse(this, address);　  // Data
            OperationItem = OperationItem ?? OperationItemInclude.Parse(this, address); // Include
            OperationItem = OperationItem ?? OperationItemSystem.Parse(this, address);  // System

            // Addressを設定
            if (OperationItem != default(IOperationItem))
            {
                Address = OperationItem.Address;
                address = new AsmAddress(OperationItem.Address, OperationItem.Length);
            }
            else
            {
                var operationCode = this.Label.OperationCodeWithoutLabel;
                if (!string.IsNullOrEmpty(operationCode))
                {
                    throw new ErrorMessageException(Error.ErrorCodeEnum.E0001, $"{operationCode}");
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
    }
}