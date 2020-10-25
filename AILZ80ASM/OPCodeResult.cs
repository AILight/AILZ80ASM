using System;
using System.Collections.Generic;
using System.Linq;

namespace AILZ80ASM
{
    public class OPCodeResult
    {
        public enum ValueTypeEnum
        {
            None,
            IndexOffset,
            Value8,
            e8,
            Value16
        }

        //public UInt16 Address { get; private set; }
        public string[] OPCode { get; private set; }
        public int M { get; private set; }
        public int T { get; private set; }

        private ValueTypeEnum ValueType { get; set; }
        private string ValueString { get; set; }

        public OPCodeResult(OPCodeItem opCodeItem)
            : this(opCodeItem, ValueTypeEnum.None, "")
        {

        }

        public OPCodeResult(OPCodeItem opCodeItem, ValueTypeEnum valueType, string valueString)
            : this(opCodeItem.OPCode, opCodeItem.M, opCodeItem.T, valueType, valueString)
        {
        }

        public OPCodeResult(string[] opCode, int m, int t, ValueTypeEnum valueType, string valueString)
        {
            ValueType = ValueTypeEnum.None;
            OPCode = opCode;
            M = m;
            T = t;
            ValueType = valueType;
            ValueString = valueString;
        }

        public byte[] ToBin()
        {
            return OPCode.Select(m => Convert.ToByte(m, 2)).ToArray();
        }

        public void Assemble(LineItem lineItem, Label[] labels)
        {
            var byteList = new List<byte>();
            if (ValueType != ValueTypeEnum.None)
            {
                var indexOffset = "";
                var value8 = "";
                var e8 = "";
                var value16 = new[] { "", "" };

                switch (ValueType)
                {
                    case ValueTypeEnum.IndexOffset:
                        {
                            var tmpValue8 = AIMath.ConvertToByte(ValueString, lineItem, labels);
                            indexOffset = Convert.ToString(tmpValue8, 2).PadLeft(8, '0');
                        }
                        break;
                    case ValueTypeEnum.Value8:
                        {
                            var tmpValue8 = AIMath.ConvertToByte(ValueString, lineItem, labels);
                            value8 = Convert.ToString(tmpValue8, 2).PadLeft(8, '0');
                        }
                        break;
                    case ValueTypeEnum.e8:
                        {
                            var tmpValue8 = AIMath.ConvertToByte(ValueString, lineItem, labels);
                            e8 = Convert.ToString(tmpValue8, 2).PadLeft(8, '0');
                        }
                        break;
                    case ValueTypeEnum.Value16:
                        {
                            var tmpValue16 = AIMath.ConvertToUInt16(ValueString, lineItem, labels);
                            var tmpValue16String = Convert.ToString(tmpValue16, 2).PadLeft(16, '0');
                            value16[0] = tmpValue16String.Substring(0, 8);
                            value16[1] = tmpValue16String.Substring(8);
                        }
                        break;
                    default:
                        break;
                }

                OPCode = OPCode.Select(m => m.Replace("IIIIIIII", indexOffset)
                                             .Replace("NNNNNNNN", value8)
                                             .Replace("EEEEEEEE", e8)
                                             .Replace("HHHHHHHH", value16[0])
                                             .Replace("LLLLLLLL", value16[1])).ToArray();
            }
        }
    }
}
