using AILZ80ASM.AILight;
using AILZ80ASM.Assembler;
using AILZ80ASM.Exceptions;
using AILZ80ASM.LineDetailItems.ScopeItem.ExpansionItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AILZ80ASM.OperationItems
{
    public class OperationItemDataFill : OperationItem
    {
        private enum DataTypeEnum
        {
            dbfill = 1, // DataLength = 1
            dwfill = 2, // DataLength = 2
        }
        private static readonly string RegexPatternDataOP = @"(?<op1>^\S+)?\s*(?<op2>.+)*";
        private static readonly Regex CompiledRegexPatternDataOP = new Regex(
            RegexPatternDataOP,
            RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase
        );

        private string[] ValueStrings { get; set; }
        private DataTypeEnum DataType { get; set; }
        private byte[] ItemDataBin { get; set; }
        private AsmLength ItemDataLength { get; set; }
        private string[] Arguments { get; set; }

        public override byte[] Bin => ItemDataBin;
        public override AsmLength Length => ItemDataLength;

        private OperationItemDataFill(DataTypeEnum dataType, string[] valueStrings, LineItem lineItem, AsmLoad asmLoad)
            : base(lineItem, asmLoad)
        {
            DataType = dataType;
            Arguments = valueStrings;
        }

        public static OperationItemDataFill Create(LineItem lineItem, AsmLoad asmLoad)
        {
            var matched = CompiledRegexPatternDataOP.Match(lineItem.OperationString);

            var op1 = matched.Groups["op1"].Value;
            var op2 = matched.Groups["op2"].Value;
            var dataType = default(DataTypeEnum);

            switch (op1.ToUpper())
            {
                case "DBFIL":
                    dataType = DataTypeEnum.dbfill;
                    break;
                case "DWFIL":
                    dataType = DataTypeEnum.dwfill;
                    break;
                default:
                    return default;
            }

            var ops = AIName.ParseArguments(op2);
            return new OperationItemDataFill(dataType, ops, lineItem, asmLoad);
        }

        public override void PreAssemble(LineDetailExpansionItemOperation lineDetailExpansionItemOperation)
        {
            base.PreAssemble(lineDetailExpansionItemOperation);

            // 展開処理を行う
            var valueString = "0";
            var errorCode = DataType switch
            {
                DataTypeEnum.dbfill => Error.ErrorCodeEnum.E0024,
                DataTypeEnum.dwfill => Error.ErrorCodeEnum.E0025,
                _ => throw new NotImplementedException()
            };

            if (Arguments.Length == 0 || Arguments.Length > 2)
            {
                throw new ErrorAssembleException(errorCode, "引数の指定が間違っています");
            }
            if (Arguments.Length == 2)
            {
                valueString = Arguments[1];
            }

            var count = AIMath.Calculation(Arguments[0], AsmLoad).ConvertTo<int>();
            if (count < 0)
            {
                throw new ErrorAssembleException(errorCode, "負の値は指定できません");
            }

            ValueStrings = Enumerable.Range(0, count).Select(_ => valueString).ToArray();
            ItemDataLength = new AsmLength(ValueStrings.Length * (int)DataType);
            LineDetailExpansionItemOperation = lineDetailExpansionItemOperation;
        }

        public override void Assemble(AsmLoad asmLoad)
        {
            base.Assemble(asmLoad);

            var byteList = new List<byte>();
            if (ValueStrings != default)
            {
                switch (DataType)
                {
                    case DataTypeEnum.dwfill:
                        foreach (var valueItem in ValueStrings.Select((Value, Index) => new { Value, Index }))
                        {
                            var values = AsmException.TryCatch(Error.ErrorCodeEnum.E0022, valueItem.Value, () =>
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
                                return values;
                            });
                        }
                        break;
                    case DataTypeEnum.dbfill:
                        foreach (var valueItem in ValueStrings.Select((Value, Index) => new { Value, Index }))
                        {
                            byteList.AddRange(AsmException.TryCatch(Error.ErrorCodeEnum.E0021, valueItem.Value, () =>
                            {
                                 return AIMath.Calculation(valueItem.Value, asmLoad, new AsmAddress(LineDetailExpansionItemOperation.Address, new AsmLength(valueItem.Index * (int)DataType))).ConvertTo<byte[]>();
                            }));
                        }
                        break;
                    default:
                        throw new InvalidOperationException(nameof(DataType));
                }
            }

            ItemDataBin = byteList.ToArray();
        }
    }
}