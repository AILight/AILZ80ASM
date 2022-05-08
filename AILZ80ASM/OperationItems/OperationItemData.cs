﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AILZ80ASM.AILight;
using AILZ80ASM.Assembler;
using AILZ80ASM.Exceptions;
using AILZ80ASM.LineDetailItems.ScopeItem.ExpansionItems;

namespace AILZ80ASM.OperationItems
{
    public class OperationItemData : OperationItem
    {
        private string[] ValueStrings { get; set; }
        private DataTypeEnum DataType { get; set; }
        private byte[] ItemDataBin { get; set; }
        private AsmLength ItemDataLength { get; set; }
        private static readonly string RegexPatternDataFunction = @"^\[(?<variable>[a-z|A-Z|0-9|_]+)\s*=\s*(?<start>[a-z|A-Z|0-9|_|$|%]+)\s*\.\.\s*(?<end>[a-z|A-Z|0-9|_|$|%]+)\s*:\s*(?<operation>.+)\]$";
        private static readonly string RegexPatternDataOP = @"(?<op1>^\S+)?\s*(?<op2>.+)*";

        public override byte[] Bin => ItemDataBin;

        public override AsmLength Length => ItemDataLength;

        private enum DataTypeEnum
        {
            db = 1, // DataLength = 1
            dw = 2, // DataLength = 2
        }

        private OperationItemData()
        {

        }

        public new static bool CanCreate(string operation, AsmLoad asmLoad)
        {
            var matched = Regex.Match(operation, RegexPatternDataOP, RegexOptions.Singleline | RegexOptions.IgnoreCase);
            var op1 = matched.Groups["op1"].Value;
            return (new[] { "DB", "DW", }).Any(m => string.Compare(m, op1, true) == 0);
        }

        public new static OperationItem Create(LineDetailExpansionItemOperation lineDetailExpansionItemOperation, AsmAddress address, AsmLoad asmLoad)
        {
            var returnValue = default(OperationItemData);
            var matched = Regex.Match(lineDetailExpansionItemOperation.LineItem.OperationString, RegexPatternDataOP, RegexOptions.Singleline | RegexOptions.IgnoreCase);

            var op1 = matched.Groups["op1"].Value;
            var op2 = matched.Groups["op2"].Value;

            switch (op1.ToUpper())
            {
                case "DB":
                    returnValue = DBDW(DataTypeEnum.db, op2, lineDetailExpansionItemOperation, address, asmLoad);
                    break;
                case "DW":
                    returnValue = DBDW(DataTypeEnum.dw, op2, lineDetailExpansionItemOperation, address, asmLoad);
                    break;
                default:
                    break;
            }

            return returnValue;
        }

        /// <summary>
        /// DB、DWの処理部
        /// </summary>
        /// <param name="dataType"></param>
        /// <param name="op2"></param>
        /// <param name="lineDetailExpansionItemOperation"></param>
        /// <param name="address"></param>
        /// <param name="labels"></param>
        /// <returns></returns>
        private static OperationItemData DBDW(DataTypeEnum dataType, string op2, LineDetailExpansionItemOperation lineDetailExpansionItemOperation, AsmAddress address, AsmLoad asmLoad)
        {
            var ops = AIName.ParseArguments(op2);
            var dataList = new List<string>();
            
            foreach (var item in ops.Select((value, index) => new { Value = value, Index = index }))
            {
                //文字列の判断
                if (AIString.IsChar(item.Value, asmLoad) || AIString.IsString(item.Value, asmLoad))
                {
                    try
                    {
                        var aiValue = AIMath.Calculation(item.Value, asmLoad, address);
                        switch (dataType)
                        {
                            case DataTypeEnum.db:
                                dataList.AddRange(aiValue.ConvertTo<byte[]>().Select(m => m.ToString("0")));
                                break;
                            case DataTypeEnum.dw:
                                dataList.AddRange(aiValue.ConvertTo<UInt16[]>().Select(m => m.ToString("0")));
                                break;
                            default:
                                throw new NotImplementedException();
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
                        throw new ErrorAssembleException(Error.ErrorCodeEnum.E0022, item.Value);
                    }
                }
                else if(Regex.IsMatch(item.Value, RegexPatternDataFunction))
                {
                    dataList.AddRange(DBDW_Function(item.Value, lineDetailExpansionItemOperation, asmLoad));
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(item.Value))
                    {
                        // 最後は値の無指定が可能
                        if (item.Index < ops.Length - 1)
                        {
                            switch (dataType)
                            {
                                case DataTypeEnum.db:
                                    throw new ErrorAssembleException(Error.ErrorCodeEnum.E0021, "値が指定されていません");
                                case DataTypeEnum.dw:
                                    throw new ErrorAssembleException(Error.ErrorCodeEnum.E0022, "値が指定されていません");
                                default:
                                    throw new NotImplementedException();
                            }
                        }
                    }
                    else
                    {
                        dataList.Add(item.Value);
                    }
                }
            }

            return new OperationItemData()
            {
                ValueStrings = dataList.ToArray(),
                DataType = dataType,
                ItemDataLength = new AsmLength(dataList.Count * (int)dataType),
                LineDetailExpansionItemOperation = lineDetailExpansionItemOperation
            };
        }

        /// <summary>
        /// DBDWのファンクション対応
        /// </summary>
        /// <param name="op"></param>
        /// <param name="lineExpansionItem"></param>
        /// <param name="labels"></param>
        /// <returns></returns>
        private static string[] DBDW_Function(string op, LineDetailExpansionItemOperation lineDetailExpansionItemOperation, AsmLoad asmLoad)
        {
            var returnValues = new List<string>();

            var matchFunction = Regex.Match(op, RegexPatternDataFunction, RegexOptions.Singleline | RegexOptions.IgnoreCase);
            if (matchFunction.Success)
            {
                var variableName = matchFunction.Groups["variable"].Value.Trim();
                var start = matchFunction.Groups["start"].Value.Trim();
                var end = matchFunction.Groups["end"].Value.Trim();
                var operation = matchFunction.Groups["operation"].Value.Trim();

                //ループの展開
                var startValue = AIMath.Calculation(start, asmLoad, lineDetailExpansionItemOperation.Address).ConvertTo<int>();
                var endValue = AIMath.Calculation(end, asmLoad, lineDetailExpansionItemOperation.Address).ConvertTo<int>();
                var stepValue = startValue < endValue ? 1 : -1;
                var loopCount = (endValue - startValue) * stepValue;
                var currentValue = startValue;
                for (var index = 0; index <= loopCount; index++)
                {
                    //ループの展開
                    var tmpOperation = Regex.Replace(operation, $"\\b{variableName}\\b", $"{currentValue}", RegexOptions.Singleline | RegexOptions.IgnoreCase);
                    if (Regex.IsMatch(tmpOperation, RegexPatternDataFunction))
                    {
                        returnValues.AddRange(DBDW_Function(tmpOperation, lineDetailExpansionItemOperation, asmLoad));
                    }
                    else
                    {
                        returnValues.Add(tmpOperation);
                    }

                    //カウンターを動かす
                    currentValue += stepValue;
                }
            }

            return returnValues.ToArray();
        }

        public override void Assemble(AsmLoad asmLoad)
        {
            var byteList = new List<byte>();
            switch (DataType)
            {
                case DataTypeEnum.dw:
                    foreach (var valueString in ValueStrings)
                    {
                        try
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
                            throw new ErrorAssembleException(Error.ErrorCodeEnum.E0022, valueString);
                        }
                    }
                    break;
                case DataTypeEnum.db:
                    foreach (var valueString in ValueStrings)
                    {
                        try
                        {
                            byteList.AddRange(AIMath.Calculation(valueString, asmLoad, LineDetailExpansionItemOperation.Address).ConvertTo<byte[]>());
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
                            throw new ErrorAssembleException(Error.ErrorCodeEnum.E0021, valueString);
                        }
                    }
                    break;
                default:
                    throw new InvalidOperationException(nameof(DataType));
            }

            ItemDataBin = byteList.ToArray();
        }
    }
}
