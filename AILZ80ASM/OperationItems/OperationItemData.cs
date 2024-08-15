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

        private enum DataValueTypeEnum
        {
            ByteValue,
            UInt16Value,
            StringValue,
        }

        private class DataValue
        {
            public DataValueTypeEnum DataValueType;
            public byte ByteValue { get; set; }
            public UInt16 UInt16Value { get; set; }
            public string StringValue { get; set; }
        }

        private static readonly string RegexPatternDataFunction = @"^\[(?<variable>[a-zA-Z0-9_]+)\s*=\s*(?<start>[a-zA-Z0-9_$%]+)\s*\.\.\s*(?<end>[a-zA-Z0-9_$%]+)\s*:\s*(?<operation>.+)\]$";
        private static readonly Regex CompiledRegexPatternDataFunction = new Regex(
            RegexPatternDataFunction, RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase
        );

        private static readonly string RegexPatternDataOP = @"(?<op1>^\S+)?\s*(?<op2>.+)*";
        private static readonly Regex CompiledRegexPatternDataOP = new Regex(
            RegexPatternDataOP, RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase
        );


        private string[] ValueStrings { get; set; }
        private DataTypeEnum DataType { get; set; }
        private byte[] ItemDataBin { get; set; }
        private AsmLength ItemDataLength { get; set; }
        private List<DataValue> ValueList { get; set; } = new List<DataValue>();

        public override byte[] Bin => ItemDataBin;
        public override AsmLength Length => ItemDataLength;

        private OperationItemData(DataTypeEnum dataType, string[] valueStrings, LineItem lineItem, AsmLoad asmLoad)
            : base(lineItem, asmLoad)
        {
            DataType = dataType;
            ValueStrings = valueStrings;
        }

        public static OperationItemData Create(LineItem lineItem, AsmLoad asmLoad)
        {
            var matched = CompiledRegexPatternDataOP.Match(lineItem.OperationString);
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
            return new OperationItemData(dataType, ops, lineItem, asmLoad);
        }

        public override void PreAssemble(LineDetailExpansionItemOperation lineDetailExpansionItemOperation)
        {
            base.PreAssemble(lineDetailExpansionItemOperation);

            // 展開処理を行う
            ValueList.Clear();
            foreach (var item in ValueStrings.Select((value, index) => new { Value = value, Index = index }))
            {
                if (AIMath.TryParse(item.Value, AsmLoad, out var reaultValue) && reaultValue.ValueType.HasFlag(AIValue.ValueTypeEnum.Bytes))
                {
                    switch (DataType)
                    {
                        case DataTypeEnum.db:
                            ValueList.AddRange(reaultValue.ConvertTo<byte[]>().Select(m => new DataValue { DataValueType = DataValueTypeEnum.ByteValue, ByteValue = m }));
                            break;
                        case DataTypeEnum.dw:
                            ValueList.AddRange(reaultValue.ConvertTo<UInt16[]>().Select(m => new DataValue { DataValueType = DataValueTypeEnum.UInt16Value, UInt16Value = m }));
                            break;
                        default:
                            throw new NotImplementedException();
                    }

                }
                else if (CompiledRegexPatternDataFunction.IsMatch(item.Value))
                {
                    ValueList.AddRange(DBDW_Function(item.Value, lineDetailExpansionItemOperation, AsmLoad).Select(m => new DataValue { DataValueType = DataValueTypeEnum.StringValue, StringValue = m }));
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
                        ValueList.Add(new DataValue { DataValueType = DataValueTypeEnum.StringValue, StringValue = item.Value });
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
                    foreach (var valueItem in ValueList.Select((Value, Index) => new { Value, Index }))
                    {
                        switch (valueItem.Value.DataValueType)
                        {
                            case DataValueTypeEnum.ByteValue:
                                {
                                    var value = (UInt16)valueItem.Value.ByteValue;
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
                                break;
                            case DataValueTypeEnum.UInt16Value:
                                {
                                    var value = valueItem.Value.UInt16Value;
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
                                break;
                            case DataValueTypeEnum.StringValue:
                                AsmException.TryCatch(Error.ErrorCodeEnum.E0022, valueItem.Value.StringValue, () =>
                                {
                                    var values = AIMath.Calculation(valueItem.Value.StringValue, asmLoad, new AsmAddress(LineDetailExpansionItemOperation.Address, new AsmLength(valueItem.Index * (int)DataType))).ConvertTo<UInt16[]>();
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
                                    return values;
                                });
                                break;
                            default:
                                throw new InvalidOperationException();
                        }
                    }
                    break;
                case DataTypeEnum.db:
                    foreach (var valueItem in ValueList.Select((Value, Index) => new { Value, Index }))
                    {
                        switch (valueItem.Value.DataValueType)
                        {
                            case DataValueTypeEnum.ByteValue:
                                byteList.Add(valueItem.Value.ByteValue);
                                break;
                            case DataValueTypeEnum.UInt16Value:
                                byteList.Add((byte)valueItem.Value.UInt16Value);
                                break;
                            case DataValueTypeEnum.StringValue:
                                byteList.AddRange(AsmException.TryCatch(Error.ErrorCodeEnum.E0021, valueItem.Value.StringValue, () =>
                                {
                                    return AIMath.Calculation(valueItem.Value.StringValue, asmLoad, new AsmAddress(LineDetailExpansionItemOperation.Address, new AsmLength(valueItem.Index * (int)DataType))).ConvertTo<byte[]>();
                                }));
                                break;
                            default:
                                throw new InvalidOperationException();
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

            var matchFunction = CompiledRegexPatternDataFunction.Match(op);
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
                    if (CompiledRegexPatternDataFunction.IsMatch(tmpOperation))
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
