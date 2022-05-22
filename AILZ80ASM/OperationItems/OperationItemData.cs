using System;
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
        private enum DataTypeEnum
        {
            db = 1, // DataLength = 1
            dw = 2, // DataLength = 2
        }
        private static readonly string RegexPatternDataFunction = @"^\[(?<variable>[a-z|A-Z|0-9|_]+)\s*=\s*(?<start>[a-z|A-Z|0-9|_|$|%]+)\s*\.\.\s*(?<end>[a-z|A-Z|0-9|_|$|%]+)\s*:\s*(?<operation>.+)\]$";
        private static readonly string RegexPatternDataOP = @"(?<op1>^\S+)?\s*(?<op2>.+)*";

        private string[] ValueStrings { get; set; }
        private DataTypeEnum DataType { get; set; }
        private byte[] ItemDataBin { get; set; }
        private AsmLength ItemDataLength { get; set; }
        private AsmLoad AsmLoad { get; set; }
        private List<string> ValueList { get; set; } = new List<string>();

        public override byte[] Bin => ItemDataBin;
        public override AsmLength Length => ItemDataLength;

        private OperationItemData(DataTypeEnum dataType, string[] valueStrings, AsmLoad asmLoad)
        {
            DataType = dataType;
            ValueStrings = valueStrings;
            AsmLoad = asmLoad;
        }

        public static OperationItemData Create(LineItem listItem, AsmLoad asmLoad)
        {
            var matched = Regex.Match(listItem.OperationString, RegexPatternDataOP, RegexOptions.Singleline | RegexOptions.IgnoreCase);
            var dataType = default(DataTypeEnum);

            var op1 = matched.Groups["op1"].Value;
            var op2 = matched.Groups["op2"].Value;
            switch (op1.ToUpper())
            {
                case "DB":
                case "DEFB":
                    dataType = DataTypeEnum.db;
                    break;
                case "DW":
                case "DEFW":
                    dataType = DataTypeEnum.dw;
                    break;
                default:
                    return default;
            }
            var ops = AIName.ParseArguments(op2);
            return new OperationItemData(dataType, ops, asmLoad);
        }

        public override void PreAssemble(LineDetailExpansionItemOperation lineDetailExpansionItemOperation)
        {
            base.PreAssemble(lineDetailExpansionItemOperation);

            // 展開処理を行う
            ValueList.Clear();
            foreach (var item in ValueStrings.Select((value, index) => new { Value = value, Index = index }))
            {
                //文字列の判断
                if (AIString.IsChar(item.Value, AsmLoad) || AIString.IsString(item.Value, AsmLoad))
                {
                    try
                    {
                        var aiValue = AIMath.Calculation(item.Value, AsmLoad, lineDetailExpansionItemOperation.Address);
                        switch (DataType)
                        {
                            case DataTypeEnum.db:
                                ValueList.AddRange(aiValue.ConvertTo<byte[]>().Select(m => m.ToString("0")));
                                break;
                            case DataTypeEnum.dw:
                                ValueList.AddRange(aiValue.ConvertTo<UInt16[]>().Select(m => m.ToString("0")));
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
                else if (Regex.IsMatch(item.Value, RegexPatternDataFunction))
                {
                    ValueList.AddRange(DBDW_Function(item.Value, lineDetailExpansionItemOperation, AsmLoad));
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(item.Value))
                    {
                        // 最後は値の無指定が可能
                        if (item.Index < ValueStrings.Length - 1)
                        {
                            switch (DataType)
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
                        ValueList.Add(item.Value);
                    }
                }
            }

            ItemDataLength = new AsmLength(ValueList.Count * (int)DataType);
            LineDetailExpansionItemOperation = lineDetailExpansionItemOperation;
        }

        public override void Assemble(AsmLoad asmLoad)
        {
            base.Assemble(asmLoad);

            var byteList = new List<byte>();
            switch (DataType)
            {
                case DataTypeEnum.dw:
                    foreach (var valueItem in ValueList.Select((Value, Index) => new { Value, Index}))
                    {
                        try
                        {
                            var values = AIMath.Calculation(valueItem.Value, asmLoad, new AsmAddress(LineDetailExpansionItemOperation.Address, new AsmLength(valueItem.Index * (int)DataType))).ConvertTo<UInt16[]>();
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
                            throw new ErrorAssembleException(Error.ErrorCodeEnum.E0022, valueItem.Value);
                        }
                    }
                    break;
                case DataTypeEnum.db:
                    foreach (var valueItem in ValueList.Select((Value, Index) => new { Value, Index }))
                    {
                        try
                        {
                            byteList.AddRange(AIMath.Calculation(valueItem.Value, asmLoad, new AsmAddress(LineDetailExpansionItemOperation.Address, new AsmLength(valueItem.Index * (int)DataType))).ConvertTo<byte[]>());
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
                            throw new ErrorAssembleException(Error.ErrorCodeEnum.E0021, valueItem.Value);
                        }
                    }
                    break;
                default:
                    throw new InvalidOperationException(nameof(DataType));
            }

            ItemDataBin = byteList.ToArray();
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
    }
}
