using AILZ80ASM.AILight;
using AILZ80ASM.Assembler;
using AILZ80ASM.Exceptions;
using System;
using System.IO;
using System.Text.RegularExpressions;

namespace AILZ80ASM
{
    public class LineDetailItemAddressDS : LineDetailItemAddress
    {
        private static readonly string RegexPatternDS = @"^(?<op1>(DS|DBS|DWS))\s+(?<arg1>[^,]+)\s*,*\s*(?<arg2>[^,]*)$";

        public override AsmList[] Lists
        {
            get
            {
                return new[]
                {
                    AsmList.CreateLineItem(LineItem)
                };
            }
        }

        public string Operation { get; set; }
        public string LengthLabel { get; set; }
        public string FillByteLabel { get; set; }

        private LineDetailItemAddressDS(LineItem lineItem, string operation, string lengthLabel, string fillByteLabel, AsmLoad asmLoad)
            : base(lineItem, asmLoad)
        {
            Operation = operation;
            LengthLabel = lengthLabel;
            FillByteLabel = fillByteLabel;
        }

        public static LineDetailItemAddressDS Create(LineItem lineItem, AsmLoad asmLoad)
        {
            var matched = Regex.Match(lineItem.OperationString, RegexPatternDS, RegexOptions.Singleline | RegexOptions.IgnoreCase);
            if (matched.Success)
            {
                var op1 = matched.Groups["op1"].Value;
                var arg1 = matched.Groups["arg1"].Value;
                var arg2 = matched.Groups["arg2"].Value;
                return new LineDetailItemAddressDS(lineItem, op1, arg1, arg2, asmLoad);
            }

            return default(LineDetailItemAddressDS);
        }

        public override void Assemble()
        {
        }

        public override void PreAssemble(ref AsmAddress asmAddress)
        {
            var lastAsmORG = this.AsmLoad.GetLastAsmORG();

            if (string.IsNullOrEmpty(LengthLabel) || !AIMath.TryParse<UInt16>(LengthLabel, this.AsmLoad, out var length))
            {
                throw new ErrorAssembleException(Error.ErrorCodeEnum.E0004, LengthLabel);
            }

            if (length > 0)
            {
                var offset = 0;
                if (string.Compare(Operation, "DS", true) == 0 ||
                    string.Compare(Operation, "DBS", true) == 0)
                {
                    offset = length;

                    var fillByte = default(byte);
                    if (AIMath.TryParse<byte>(FillByteLabel, this.AsmLoad, out var tempFillByte))
                    {
                        fillByte = tempFillByte;
                    }

                    var asmORG = new AsmORG(asmAddress.Program, asmAddress.Output, new byte[] { fillByte }, AsmORG.ORGTypeEnum.DS);
                    this.AsmLoad.AddORG(asmORG);

                }
                else if (string.Compare(Operation, "DWS", true) == 0)
                {
                    offset = length * 2;

                    var fillBytes = new byte[2];
                    if (AIMath.TryParse<UInt16>(FillByteLabel, this.AsmLoad, out var value))
                    {
                        switch (this.AsmLoad.ISA.Endianness)
                        {
                            case InstructionSet.ISA.EndiannessEnum.LittleEndian:
                                fillBytes[0] = (byte)(value % 256);
                                fillBytes[1] = (byte)(value / 256);
                                break;
                            case InstructionSet.ISA.EndiannessEnum.BigEndian:
                                fillBytes[0] = (byte)(value / 256);
                                fillBytes[1] = (byte)(value % 256);
                                break;
                            default:
                                throw new InvalidOperationException();
                        }
                    }

                    var asmORG = new AsmORG(asmAddress.Program, asmAddress.Output, fillBytes, AsmORG.ORGTypeEnum.DS);
                    this.AsmLoad.AddORG(asmORG);
                }
                else
                {
                    throw new NotImplementedException();
                }

                asmAddress.Program += (UInt16)offset;
                asmAddress.Output += (UInt32)offset;

                // 次のORGを作成する
                AssembleORG = new AsmORG(asmAddress.Program, asmAddress.Output, lastAsmORG.FillBytes, AsmORG.ORGTypeEnum.NextORG);

                this.AsmLoad.AddORG(AssembleORG);
            }
        }

        public override void ExpansionItem()
        {
        }
    }
}
