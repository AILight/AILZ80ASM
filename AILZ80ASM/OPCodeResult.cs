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
                            var indexOffset = Convert.ToString(tmpValue8, 2).PadLeft(8, '0');
                            OPCode = OPCode.Select(m => m.Replace("IIIIIIII", indexOffset)).ToArray();
                        }
                        break;
                    case ValueTypeEnum.Value8:
                        {
                            var tmpValue8 = AIMath.ConvertToByte(opCodeLabel.ValueString, lineItem, labels);
                            var value8 = Convert.ToString(tmpValue8, 2).PadLeft(8, '0');
                            OPCode = OPCode.Select(m => m.Replace("NNNNNNNN", value8)).ToArray();
                        }
                        break;
                    case ValueTypeEnum.e8:
                        {
                            var tmpValue8 = AIMath.ConvertToByte(opCodeLabel.ValueString, lineItem, labels);
                            var e8 = Convert.ToString(tmpValue8, 2).PadLeft(8, '0');
                            OPCode = OPCode.Select(m => m.Replace("EEEEEEEE", e8)).ToArray();
                        }
                        break;
                    case ValueTypeEnum.Value16:
                        {
                            var tmpValue16 = AIMath.ConvertToUInt16(opCodeLabel.ValueString, lineItem, labels);
                            var tmpValue16String = Convert.ToString(tmpValue16, 2).PadLeft(16, '0');
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
    }
}
