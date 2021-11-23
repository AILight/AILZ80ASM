using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace AILZ80ASM
{
    public class OperationItemSystem : OperationItem
    {
        private static readonly string RegexPatternOP = @"^(?<op1>(ORG|ALIGN))\s+(?<op2>[^,]+)\s*,*\s*(?<op3>[^,]*)\s*,*\s*(?<op4>[^,]*)$";

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

        public static bool CanCreate(string operation)
        {
            var matched = Regex.Match(operation, RegexPatternOP, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            var op1 = matched.Groups["op1"].Value;
            return (new[] { "ORG", "ALIGN" }).Any(m => string.Compare(m, op1, true) == 0);
        }

        public new static OperationItem Create(LineDetailExpansionItemOperation lineDetailExpansionItemOperation, AsmAddress address, AsmLoad asmLoad)
        {
            var returnValue = default(OperationItemSystem);
            var matched = Regex.Match(lineDetailExpansionItemOperation.LineItem.OperationString, RegexPatternOP, RegexOptions.IgnoreCase | RegexOptions.Singleline);

            var op1 = matched.Groups["op1"].Value;
            var op2 = matched.Groups["op2"].Value;
            var op3 = matched.Groups["op3"].Value;
            var op4 = matched.Groups["op4"].Value;

            switch (op1.ToUpper())
            {
                case "ORG":
                    {
                        var programAddress = AIMath.ConvertTo<UInt16>(op2, asmLoad, lineDetailExpansionItemOperation.Address);
                        var bytes = Array.Empty<byte>();
                        var outputAddress = address.Output;
                        var length = new AsmLength(0);

                        if (!string.IsNullOrEmpty(op3))
                        {
                            var localOutputAddress = AIMath.ConvertTo<UInt16>(op3, asmLoad, lineDetailExpansionItemOperation.Address);
                            if (address.Output > localOutputAddress)
                            {
                                throw new ErrorAssembleException(Error.ErrorCodeEnum.E0009);
                            }

                            length.Output = localOutputAddress - address.Output;
                            bytes = new byte[length.Output];
                            if (!string.IsNullOrEmpty(op4))
                            {
                                var value = AIMath.ConvertTo<byte>(op4, asmLoad, lineDetailExpansionItemOperation.Address);
                                bytes = bytes.Select(m => value).ToArray();
                            }
                        }

                        returnValue = new OperationItemSystem { Address = new AsmAddress(programAddress, outputAddress), ItemDataLength = length, ItemDataBin = bytes, LineDetailExpansionItemOperation = lineDetailExpansionItemOperation };
                    }
                    break;
                case "ALIGN":
                    {
                        var align = AIMath.ConvertTo<UInt16>(op2, asmLoad, lineDetailExpansionItemOperation.Address);
                        if ((align & (align - 1)) != 0)
                        {
                            throw new ErrorAssembleException(Error.ErrorCodeEnum.E0015);
                        }

                        var offset = align - (address.Program % align);
                        var length = new AsmLength(offset);
                        var bytes = new byte[length.Output];
                        if (!string.IsNullOrEmpty(op3))
                        {
                            var value = AIMath.ConvertTo<byte>(op3, asmLoad, lineDetailExpansionItemOperation.Address);
                            bytes = bytes.Select(m => value).ToArray();
                        }

                        returnValue = new OperationItemSystem { Address = address, ItemDataLength = length, ItemDataBin = bytes, LineDetailExpansionItemOperation = lineDetailExpansionItemOperation };
                    }
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
