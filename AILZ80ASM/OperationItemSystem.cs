using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace AILZ80ASM
{
    public class OperationItemSystem : OperationItem
    {
        private byte[] ItemDataBin { get; set; }
        private AsmLength ItemDataLength { get; set; }
        public override byte[] Bin => ItemDataBin;
        public override AsmList List
        {
            get
            {
                return AsmList.CreateLineItemORG(Address, Length, LineDetailExpansionItemOperation.LineItem);
            }
        }
        public override AsmLength Length => ItemDataLength;

        private OperationItemSystem()
        {

        }

        public new static bool CanCreate(string operation)
        {
            var matched = Regex.Match(operation, OPCodeTable.RegexPatternOP, RegexOptions.Singleline);
            var op1 = matched.Groups["op1"].Value;
            return (new[] { "ORG" }).Any(m => string.Compare(m, op1, true) == 0);
        }

        public new static OperationItem Create(LineDetailExpansionItemOperation lineDetailExpansionItemOperation, AsmAddress address, AsmLoad asmLoad)
        {
            var returnValue = default(OperationItemSystem);
            var matched = Regex.Match($"{lineDetailExpansionItemOperation.InstructionText} {lineDetailExpansionItemOperation.ArgumentText}", OPCodeTable.RegexPatternOP, RegexOptions.Singleline);

            var op1 = matched.Groups["op1"].Value;
            var op2 = matched.Groups["op2"].Value;
            var op3 = matched.Groups["op3"].Value;

            switch (op1.ToUpper())
            {
                case "ORG":
                    var programAddress = AIMath.ConvertTo<UInt16>(op2, lineDetailExpansionItemOperation, asmLoad);
                    var bytes = Array.Empty<byte>();
                    var outputAddress = address.Output;
                    var length = new AsmLength(0);

                    if (!string.IsNullOrEmpty(op3))
                    {
                        var localOutputAddress = AIMath.ConvertTo<UInt16>(op3, lineDetailExpansionItemOperation, asmLoad);
                        if (address.Output > localOutputAddress)
                        {
                            throw new ErrorAssembleException(Error.ErrorCodeEnum.E0009);
                        }

                        length.Output = localOutputAddress - address.Output;
                        bytes = new byte[length.Output];
                    }

                    returnValue = new OperationItemSystem { Address = new AsmAddress(programAddress, outputAddress), ItemDataLength = length, ItemDataBin = bytes, LineDetailExpansionItemOperation = lineDetailExpansionItemOperation };
                    break;
                default:
                    break;
            }

            return returnValue;
        }

        public override void Assemble(AsmLoad asmLoad)
        {
        }
    }
}
