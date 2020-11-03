using System;
using System.Collections.Generic;
using System.Linq;
using static AILZ80ASM.OPCodeLabel;

namespace AILZ80ASM
{
    public class OPCodeResult
    {
        public string[] OPCode { get; private set; }
        public int M { get; private set; }
        public int T { get; private set; }

        private OPCodeLabel[] OPCodeLabels { get; set; }

        public OPCodeResult(OPCodeItem opCodeItem)
            : this(opCodeItem, new OPCodeLabel[] { })
        {

        }

        public OPCodeResult(OPCodeItem opCodeItem, OPCodeLabel[] opCodeLabels)
            : this(opCodeItem.OPCode, opCodeItem.M, opCodeItem.T, opCodeLabels)
        {
        }

        public OPCodeResult(string[] opCode, int m, int t, OPCodeLabel[] opCodeLabels)
        {
            OPCode = opCode;
            M = m;
            T = t;
            OPCodeLabels = opCodeLabels;
        }

        public byte[] ToBin()
        {
            return OPCode.Select(m => Convert.ToByte(m, 2)).ToArray();
        }

        public void Assemble(LineItem lineItem, Label[] labels)
        {
            var byteList = new List<byte>();
            foreach (var opCodeLabel in OPCodeLabels)
            {
                switch (opCodeLabel.ValueType)
                {
                    case ValueTypeEnum.IndexOffset:
                        {
                            var tmpValue8 = AIMath.ConvertToByte(opCodeLabel.ValueString, lineItem, labels);
                            var indexOffset = ConvertTo2BaseString(tmpValue8, 8);
                            OPCode = OPCode.Select(m => m.Replace("IIIIIIII", indexOffset)).ToArray();
                        }
                        break;
                    case ValueTypeEnum.Value8:
                        {
                            var tmpValue8 = AIMath.ConvertToByte(opCodeLabel.ValueString, lineItem, labels);
                            var value8 = ConvertTo2BaseString(tmpValue8, 8);
                            OPCode = OPCode.Select(m => m.Replace("NNNNNNNN", value8)).ToArray();
                        }
                        break;
                    case ValueTypeEnum.e8:
                        {
                            var tmpValue16 = AIMath.ConvertToUInt16(opCodeLabel.ValueString, lineItem, labels);
                            var offsetAddress = tmpValue16 - lineItem.Address - 2;
                            if (offsetAddress < SByte.MinValue || offsetAddress > SByte.MaxValue)
                            {
                                throw new ErrorMessageException(Error.ErrorCodeEnum.E0003, $"指定された値は、{offsetAddress}でした。");
                            }
                            var e8 = ConvertTo2BaseString(offsetAddress, 8);
                            OPCode = OPCode.Select(m => m.Replace("EEEEEEEE", e8)).ToArray();
                        }
                        break;
                    case ValueTypeEnum.Value16:
                        {
                            var tmpValue16 = AIMath.ConvertToUInt16(opCodeLabel.ValueString, lineItem, labels);
                            var tmpValue16String = ConvertTo2BaseString(tmpValue16, 16);
                            var value16 = new[] { "", "" };
                            value16[0] = tmpValue16String.Substring(0, 8);
                            value16[1] = tmpValue16String.Substring(8);
                            OPCode = OPCode.Select(m => m.Replace("HHHHHHHH", value16[0])
                                                         .Replace("LLLLLLLL", value16[1])).ToArray();
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        private string ConvertTo2BaseString(int value, int length)
        {
            var returnValue = Convert.ToString(value, 2).PadLeft(length, '0'); ;
            var overString = returnValue.Substring(0, returnValue.Length - length);
            if ((value > 0 && overString.Contains("1")) ||
                (value < 0 && overString.Contains("0")) )
            {
                throw new ErrorMessageException(Error.ErrorCodeEnum.E0002, $"{value:x}");
            }

            return returnValue.Substring(overString.Length);
        }
    }
}
