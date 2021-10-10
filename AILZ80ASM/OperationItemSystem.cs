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

        public static IOperationItem Parse(LineDetailExpansionItemOperation lineDetailExpansionItemOperation, AsmAddress address, AsmLoad asmLoad)
        {
            var returnValue = default(OperationItemSystem);
            var matched = Regex.Match($"{lineDetailExpansionItemOperation.InstructionText} {lineDetailExpansionItemOperation.ArgumentText}", OPCodeTable.RegexPatternOP, RegexOptions.Singleline);

            var op1 = matched.Groups["op1"].Value.ToUpper();
            var op2 = matched.Groups["op2"].Value.ToUpper();
            var op3 = matched.Groups["op3"].Value.ToUpper();

            switch (op1)
            {
                case "ORG":
                    var programAddress = AIMath.ConvertToUInt16(op2, lineDetailExpansionItemOperation, asmLoad);
                    var bytes = Array.Empty<byte>();
                    var outputAddress = address.Output;
                    var length = new AsmLength(0);

                    if (!string.IsNullOrEmpty(op3))
                    {
                        var localOutputAddress = AIMath.ConvertToUInt16(op3, lineDetailExpansionItemOperation, asmLoad);
                        if (address.Output > localOutputAddress)
                        {
                            throw new ErrorAssembleException(Error.ErrorCodeEnum.E0009);
                        }

                        length.Output = localOutputAddress - address.Output;
                        bytes = new byte[length.Output];
                    }

                    returnValue = new OperationItemSystem { Address = new AsmAddress(programAddress, outputAddress), Length = length, Bin = bytes };
                    break;
                default:
                    break;
            }

            return returnValue;
        }

        public byte[] Bin { get; set; }

        public AsmAddress Address { get; set; }
        public AsmLength Length { get; set; }

        public void Assemble(AsmLoad asmLoad)
        {
        }
    }
}
