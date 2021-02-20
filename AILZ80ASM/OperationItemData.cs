using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AILZ80ASM
{
    public class OperationItemData : IOperationItem
    {
        private string[] ValueStrings { get; set; }
        private DataTypeEnum DataType { get; set; }
        private LineItem LineItem { get; set; }

        private enum DataTypeEnum
        {
            dw = 1,
            db = 2,
        }

        private OperationItemData()
        {

        }

        public static IOperationItem Perse(LineItem lineItem, UInt16 address)
        {
            var returnValue = default(OperationItemData);
            var matched = Regex.Match(lineItem.Label.OperationCodeWithoutLabel, OPCodeTable.RegexPatternOP, RegexOptions.Singleline);

            var op1 = matched.Groups["op1"].Value.ToUpper();
            var op2 = matched.Groups["op2"].Value.ToUpper();
            var op3 = matched.Groups["op3"].Value.ToUpper();

            switch (op1)
            {
                case "DB":
                case "DW":
                    var valuesStrings = (op2 + (!string.IsNullOrEmpty(op3) ? "," : "") + op3).Split(',').ToArray();
                    var dataType = op1 == "DB" ? DataTypeEnum.db : DataTypeEnum.dw;
                    returnValue = new OperationItemData()
                    {
                        ValueStrings = valuesStrings,
                        DataType = dataType,
                        Address = address,
                        NextAddress = (UInt16)(address + (valuesStrings.Length * (int)dataType)),
                        LineItem = lineItem
                    };
                    break;
                default:
                    break;
            }

            return returnValue;
        }

        public byte[] Bin { get; set; }

        public UInt16 Address { get; set; }

        public UInt16 NextAddress { get; set; }

        public void Assemble(Label[] labels)
        {
            var byteList = new List<byte>();
            switch (DataType)
            {
                case DataTypeEnum.dw:
                    foreach (var valueString in ValueStrings)
                    {
                        var value = AIMath.ConvertToUInt16(valueString, LineItem, labels);
                        byteList.Add((byte)(value % 256));
                        byteList.Add((byte)(value / 256));
                    }
                    break;
                case DataTypeEnum.db:
                    foreach (var valueString in ValueStrings)
                    {
                        byteList.Add((byte) AIMath.ConvertToUInt16(valueString, LineItem, labels));
                    }
                    break;
                default:
                    throw new InvalidOperationException(nameof(DataType));
            }

            Bin = byteList.ToArray();
        }
    }
}
