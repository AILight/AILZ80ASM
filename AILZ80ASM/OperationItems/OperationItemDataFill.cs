using AILZ80ASM.AILight;
using AILZ80ASM.Assembler;
using AILZ80ASM.Exceptions;
using AILZ80ASM.Interfaces;
using AILZ80ASM.LineDetailItems.ScopeItem.ExpansionItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AILZ80ASM.OperationItems
{
    public class OperationItemDataFill : OperationItem, IOperationItemDefaultClearable
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
            dbf = 1, // DataLength = 1
            dwf = 2, // DataLength = 2
        }

        private OperationItemDataFill()
        {

        }

        public new static bool CanCreate(string operation, AsmLoad asmLoad)
        {
            var matched = Regex.Match(operation, RegexPatternDataOP, RegexOptions.Singleline | RegexOptions.IgnoreCase);
            var op1 = matched.Groups["op1"].Value;
            return (new[] { "DBF", "DWF" }).Any(m => string.Compare(m, op1, true) == 0);
        }

        public new static OperationItem Create(LineDetailExpansionItemOperation lineDetailExpansionItemOperation, AsmAddress address, AsmLoad asmLoad)
        {
            var returnValue = default(OperationItemDataFill);
            var matched = Regex.Match(lineDetailExpansionItemOperation.LineItem.OperationString, RegexPatternDataOP, RegexOptions.Singleline | RegexOptions.IgnoreCase);

            var op1 = matched.Groups["op1"].Value;
            var op2 = matched.Groups["op2"].Value;

            switch (op1.ToUpper())
            {
                case "DBF":
                    returnValue = DBDWF(DataTypeEnum.dbf, op2, lineDetailExpansionItemOperation, address, asmLoad);
                    break;
                case "DWF":
                    returnValue = DBDWF(DataTypeEnum.dwf, op2, lineDetailExpansionItemOperation, address, asmLoad);
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
        private static OperationItemDataFill DBDWF(DataTypeEnum dataType, string op2, LineDetailExpansionItemOperation lineDetailExpansionItemOperation, AsmAddress address, AsmLoad asmLoad)
        {
            var returnValue = default(OperationItemDataFill);
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

            var count = AIMath.Calculation(ops[0], asmLoad).ConvertTo<int>();
            if (count < 0)
            {
                throw new ErrorAssembleException(Error.ErrorCodeEnum.E0012);
            }

            var valuesStrings = Enumerable.Range(0, count).Select(_ => valueString).ToArray();

            returnValue = new OperationItemDataFill()
            {
                ValueStrings = valuesStrings,
                DataType = dataType,
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
                case DataTypeEnum.dwf:
                    try
                    {
                        foreach (var valueString in ValueStrings)
                        {
                            var values = AIMath.Calculation(valueString, asmLoad, LineDetailExpansionItemOperation.Address).ConvertTo<UInt16[]>();
                            foreach (var value in values)
                            {
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
                        }
                    }
                    catch (CharMapNotFoundException ex)
                    {
                        throw new ErrorAssembleException(Error.ErrorCodeEnum.E2106, ex.Message);
                    }
                    catch (CharMapConvertException ex)
                    {
                        throw new ErrorAssembleException(Error.ErrorCodeEnum.E2105, ex.Message);
                    }
                    catch (InvalidAIValueException ex)
                    {
                        throw new ErrorAssembleException(Error.ErrorCodeEnum.E0004, ex.Message);
                    }
                    catch (InvalidAIMathException ex)
                    {
                        throw new ErrorAssembleException(Error.ErrorCodeEnum.E0004, ex.Message);
                    }
                    catch (ErrorAssembleException)
                    {
                        throw;
                    }
                    catch (ErrorLineItemException)
                    {
                        throw;
                    }
                    catch (Exception)
                    {
                        throw new ErrorAssembleException(Error.ErrorCodeEnum.E0025, ValueStrings.FirstOrDefault());
                    }

                    break;
                case DataTypeEnum.dbf:
                    try
                    {
                        foreach (var valueString in ValueStrings)
                        {
                            byteList.AddRange(AIMath.Calculation(valueString, asmLoad, LineDetailExpansionItemOperation.Address).ConvertTo<byte[]>());
                        }
                    }
                    catch (CharMapNotFoundException ex)
                    {
                        throw new ErrorAssembleException(Error.ErrorCodeEnum.E2106, ex.Message);
                    }
                    catch (CharMapConvertException ex)
                    {
                        throw new ErrorAssembleException(Error.ErrorCodeEnum.E2105, ex.Message);
                    }
                    catch (InvalidAIValueException ex)
                    {
                        throw new ErrorAssembleException(Error.ErrorCodeEnum.E0004, ex.Message);
                    }
                    catch (InvalidAIMathException ex)
                    {
                        throw new ErrorAssembleException(Error.ErrorCodeEnum.E0004, ex.Message);
                    }
                    catch (ErrorAssembleException)
                    {
                        throw;
                    }
                    catch (ErrorLineItemException)
                    {
                        throw;
                    }
                    catch (Exception)
                    {
                        throw new ErrorAssembleException(Error.ErrorCodeEnum.E0024, ValueStrings.FirstOrDefault());
                    }
                    break;
                default:
                    throw new InvalidOperationException(nameof(DataType));
            }

            ItemDataBin = byteList.ToArray();
        }

    }
}