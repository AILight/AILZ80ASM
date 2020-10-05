﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace AILZ80ASM
{
    public class LineItem
    {
        private string RawString { get; set; }

        public string LabelString { get; private set; }
        public string MnemonicString { get; private set; }
        public string CommentString { get; private set; }
        public UInt16 Address { get; private set; }
        public byte[] Bin { get; private set; }

        public LineItem(string lineString)
        {
            RawString = lineString;
            //コメントを処理する
            var indexCommnet = lineString.IndexOf(';');
            if (indexCommnet != -1)
            {
                CommentString = lineString.Substring(indexCommnet);
                lineString = lineString.Substring(0, indexCommnet);
            }

            var matched = Regex.Match(lineString, @"(?<lable>^.+:)?\s(?<mnemonic>[^;]+)", RegexOptions.Singleline);
            LabelString = matched.Groups["lable"].Value;
            MnemonicString = matched.Groups["mnemonic"].Value;
        }

        public void SetLabel(ref ushort address, IList<Lable> labelList)
        {
            if (!string.IsNullOrEmpty(LabelString))
            {
                labelList.Add(new Lable { LabelName = LabelString, DataLength = Lable.DataLengthEnum.DW, Value = address });
            }

            if (!string.IsNullOrEmpty(MnemonicString))
            {
                var opCodeItem = OPCodeTable.GetOPCodeItem(MnemonicString, null);
                switch (opCodeItem.OPCodeStatus)
                {
                    case OPCodeResult.OPCodeStatusEnum.ORG:
                        address = opCodeItem.Address;
                        break;
                    case OPCodeResult.OPCodeStatusEnum.OP:
                    case OPCodeResult.OPCodeStatusEnum.DATA:
                        address += (byte)opCodeItem.OPCode.Length;
                        break;
                    case OPCodeResult.OPCodeStatusEnum.ERROR:
                        break;
                    default:
                        break;
                }
            }
        }

        public void Assemble(ref ushort address, Lable[] labelList)
        {
            if (!string.IsNullOrEmpty(MnemonicString))
            {
                Address = address;
                var opCodeItem = OPCodeTable.GetOPCodeItem(MnemonicString, labelList);
                switch (opCodeItem.OPCodeStatus)
                {
                    case OPCodeResult.OPCodeStatusEnum.ORG:
                        address = opCodeItem.Address;
                        break;
                    case OPCodeResult.OPCodeStatusEnum.OP:
                    case OPCodeResult.OPCodeStatusEnum.DATA:
                        address += (byte)opCodeItem.OPCode.Length;
                        Bin = opCodeItem.ToBin();
                        break;
                    case OPCodeResult.OPCodeStatusEnum.ERROR:
                        break;
                    default:
                        break;
                }
            }
        }


    }
}
