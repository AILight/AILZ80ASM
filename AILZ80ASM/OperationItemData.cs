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
            db = 1,
            dw = 2,
        }

        private OperationItemData()
        {

        }

        public static IOperationItem Parse(LineItem lineItem, AsmAddress address)
        {
            var returnValue = default(OperationItemData);
            var matched = Regex.Match(lineItem.Label.OperationCodeWithoutLabel, OPCodeTable.RegexPatternOP, RegexOptions.Singleline);

            var op1 = matched.Groups["op1"].Value.ToUpper();
            var op2 = matched.Groups["op2"].Value.ToUpper();
            var op3 = matched.Groups["op3"].Value.ToUpper();
            var valuesStrings = default(string[]);

            switch (op1)
            {
                case "DB":
                    if (IsString(op2, op3))
                    {
                        valuesStrings = System.Text.Encoding.ASCII.GetBytes(op3.Substring(1, op3.Length - 2)).Select(m => m.ToString("0")).ToArray();
                    }
                    else
                    {
                        valuesStrings = (op2 + (!string.IsNullOrEmpty(op3) ? "," : "") + op3).Split(',').ToArray();
                    }
                    var dataType = op1 == "DB" ? DataTypeEnum.db : DataTypeEnum.dw;
                    returnValue = new OperationItemData()
                    {
                        ValueStrings = valuesStrings,
                        DataType = DataTypeEnum.db,
                        Address = address,
                        Length = new AsmLength(valuesStrings.Length),
                        LineItem = lineItem
                    };
                    break;
                case "DW":
                    valuesStrings = (op2 + (!string.IsNullOrEmpty(op3) ? "," : "") + op3).Split(',').ToArray();
                    returnValue = new OperationItemData()
                    {
                        ValueStrings = valuesStrings,
                        DataType = DataTypeEnum.dw,
                        Address = address,
                        Length = new AsmLength(valuesStrings.Length * 2),
                        LineItem = lineItem
                    };
                    break;
                default:
                    break;
            }

            return returnValue;
        }

        public byte[] Bin { get; set; }

        public AsmAddress Address { get; set; }

        public AsmLength Length { get; set; }

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
                        byteList.Add((byte)AIMath.ConvertToUInt16(valueString, LineItem, labels));
                    }
                    break;
                default:
                    throw new InvalidOperationException(nameof(DataType));
            }

            Bin = byteList.ToArray();
        }

        private static bool IsString(string op2, string op3)
        {
            return string.IsNullOrEmpty(op2) && op3.StartsWith("\"") && op3.EndsWith("\"");
        }
    }
}
