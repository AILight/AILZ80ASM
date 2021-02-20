using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace AILZ80ASM
{
    public class OperationItemSystem : IOperationItem
    {
        private OperationItemSystem()
        {

        }

        public static IOperationItem Perse(LineItem lineItem, UInt16 address)
        {
            var returnValue = default(OperationItemSystem);
            var matched = Regex.Match(lineItem.Label.OperationCodeWithoutLabel, OPCodeTable.RegexPatternOP, RegexOptions.Singleline);

            var op1 = matched.Groups["op1"].Value.ToUpper();
            var op2 = matched.Groups["op2"].Value.ToUpper();
            var op3 = matched.Groups["op3"].Value.ToUpper();

            switch (op1)
            {
                case "ORG":
                    op2 = AIMath.Replace16Number(op2, address);
                    address = Convert.ToUInt16(op2);

                    returnValue = new OperationItemSystem { Address = address, NextAddress = address, Bin = new byte[] { } };
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
        }
    }
}
