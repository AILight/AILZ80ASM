using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AILZ80ASM
{
    public class OperationItemDataSpace : OperationItem, IOperationItemDefaultClearable
    {
        private string[] ValueStrings { get; set; }
        private DataTypeEnum DataType { get; set; }
        private byte[] ItemDataBin { get; set; }
        private AsmLength ItemDataLength { get; set; }
        private static readonly string RegexPatternDataOP = @"(?<op1>^\S+)?\s*(?<op2>.+)*";

        public override byte[] Bin => ItemDataBin;

        public override AsmLength Length => ItemDataLength;

        public bool IsDefaultValueClear { get; set; } = true;

        private enum DataTypeEnum
        {
            dbs = 1, // DataLength = 1
            dws = 2, // DataLength = 2
        }

        private OperationItemDataSpace()
        {

        }

        public new static bool CanCreate(string operation, AsmLoad asmLoad)
        {
            var matched = Regex.Match(operation, RegexPatternDataOP, RegexOptions.Singleline | RegexOptions.IgnoreCase);
            var op1 = matched.Groups["op1"].Value;
            return (new[] { "DS", "DBS", "DWS" }).Any(m => string.Compare(m, op1, true) == 0);
        }

        public new static OperationItem Create(LineDetailExpansionItemOperation lineDetailExpansionItemOperation, AsmAddress address, AsmLoad asmLoad)
        {
            var returnValue = default(OperationItemDataSpace);
            var matched = Regex.Match(lineDetailExpansionItemOperation.LineItem.OperationString, RegexPatternDataOP, RegexOptions.Singleline | RegexOptions.IgnoreCase);

            var op1 = matched.Groups["op1"].Value;
            var op2 = matched.Groups["op2"].Value;

            switch (op1.ToUpper())
            {
                case "DS":
                case "DBS":
                    returnValue = DBDWS(DataTypeEnum.dbs, op2, lineDetailExpansionItemOperation, address, asmLoad);
                    break;
                case "DWS":
                    returnValue = DBDWS(DataTypeEnum.dws, op2, lineDetailExpansionItemOperation, address, asmLoad);
                    break;
                default:
                    break;
            }

            return returnValue;
        }

        /// <summary>
        /// 指定数量分、データを作る処理
        /// </summary>
        /// <param name="dataType"></param>
        /// <param name="op2"></param>
        /// <param name="op3"></param>
        /// <param name="lineExpansionItem"></param>
        /// <param name="address"></param>
        /// <returns></returns>
        private static OperationItemDataSpace DBDWS(DataTypeEnum dataType, string op2, LineDetailExpansionItemOperation lineDetailExpansionItemOperation, AsmAddress address, AsmLoad asmLoad)
        {
            var returnValue = default(OperationItemDataSpace);
            var ops = AIName.ParseArguments(op2);
            var valueString = "0";
            var isDefaultValueClear = true;

            if (ops.Length == 0 || ops.Length > 2)
            {
                throw new ErrorAssembleException(Error.ErrorCodeEnum.E0012);
            }
            if (ops.Length == 2)
            {
                valueString = ops[1];
                isDefaultValueClear = false;
            }

            var count = AIMath.ConvertTo<int>(ops[0], asmLoad);
            var valuesStrings = Enumerable.Range(0, count).Select(_ => valueString).ToArray();

            returnValue = new OperationItemDataSpace()
            {
                ValueStrings = valuesStrings,
                DataType = dataType,
                Address = address,
                ItemDataLength = new AsmLength(valuesStrings.Length * (int)dataType),
                LineDetailExpansionItemOperation = lineDetailExpansionItemOperation,
                IsDefaultValueClear = isDefaultValueClear,
            };

            return returnValue;
        }

        public override void Assemble(AsmLoad asmLoad)
        {
            var byteList = new List<byte>();
            switch (DataType)
            {
                case DataTypeEnum.dws:
                    foreach (var valueString in ValueStrings)
                    {
                        try
                        {
                            var value = AIMath.ConvertTo<UInt16>(valueString, asmLoad, LineDetailExpansionItemOperation.Address);
                            switch (asmLoad.ISA.Endianness)
                            {
                                case InstructionSet.ISA.EndiannessEnum.LittleEndian:
                                    byteList.Add((byte)(value % 256));
                                    byteList.Add((byte)(value / 256));
                                    break;
                                case InstructionSet.ISA.EndiannessEnum.BigEndian:
                                    byteList.Add((byte)(value / 256));
                                    byteList.Add((byte)(value % 256));
                                    break;
                                default:
                                    throw new InvalidOperationException();
                            }
                        }
                        catch (Exception)
                        {
                            throw new ErrorAssembleException(Error.ErrorCodeEnum.E0022, valueString);
                        }
                    }
                    break;
                case DataTypeEnum.dbs:
                    foreach (var valueString in ValueStrings)
                    {
                        try
                        {
                            byteList.Add((byte)AIMath.ConvertTo<UInt16>(valueString, asmLoad, LineDetailExpansionItemOperation.Address));
                        }
                        catch (Exception)
                        {
                            throw new ErrorAssembleException(Error.ErrorCodeEnum.E0021, valueString);
                        }
                    }
                    break;
                default:
                    throw new InvalidOperationException(nameof(DataType));
            }

            ItemDataBin = byteList.ToArray();
        }

        public override void TrimData()
        {
            ItemDataBin = Array.Empty<byte>();
            ItemDataLength = new AsmLength();
        }
    }
}
