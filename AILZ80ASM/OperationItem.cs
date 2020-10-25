using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AILZ80ASM
{
    public class OperationItem
    {
        private string RawString { get; set; }

        public string LabelString { get; private set; }
        public string MnemonicString { get; private set; }
        public string CommentString { get; private set; }
        public UInt16 Address { get; private set; }
        public byte[] Bin { get; private set; }

        public OperationItem(string lineString)
        {
            RawString = lineString;
            //コメントを処理する
            var indexCommnet = lineString.IndexOf(';');
            if (indexCommnet != -1)
            {
                CommentString = lineString.Substring(indexCommnet);
                lineString = lineString.Substring(0, indexCommnet);
            }
            //ラベルを処理する
            var matched = Regex.Match(lineString, @"(?<lable>(^.+:)|(^\.([.]|[^\s])+))", RegexOptions.Singleline);
            LabelString = matched.Groups["lable"].Value;

            MnemonicString = lineString.Substring(LabelString.Length).Trim();
        }

        /*
        public void SetLabel(ref ushort address, ref string nameSpace, IList<Label> labelList)
        {
            if (!string.IsNullOrEmpty(LabelString))
            {
                if (LabelString[0] == '.')
                {
                    var tmpNameSpace = nameSpace;
                    var label = labelList.Last(_ => _.NameSpace == tmpNameSpace && _.LocalLabelName == "");
                    labelList.Add(new Label { NameSpace = nameSpace, LabelName = LabelString, LocalLabelName = LabelString.Substring(1), DataLength = Label.DataLengthEnum.DW, Value = address });
                }
                else
                {
                    labelList.Add(new Label { NameSpace = nameSpace, LabelName = LabelString.Substring(0, LabelString.Length - 1), LocalLabelName = "", DataLength = Label.DataLengthEnum.DW, Value = address });
                }
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
        */

        public void Assemble(ref ushort address, Label[] labelList)
        {
            if (!string.IsNullOrEmpty(MnemonicString))
            {
                Address = address;
                var opCodeItem = OPCodeTable.GetOPCodeItem(MnemonicString);
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
