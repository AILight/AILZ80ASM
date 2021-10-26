using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AILZ80ASM
{
    public class OperationItemData : OperationItem
    {
        private string[] ValueStrings { get; set; }
        private DataTypeEnum DataType { get; set; }
        private byte[] ItemDataBin { get; set; }
        private AsmLength ItemDataLength { get; set; }
        private static readonly string RegexPatternDataFunction = @"^\[(?<variable>[a-z|A-Z|0-9|_]+)\s*=\s*(?<start>[a-z|A-Z|0-9|_|$|%]+)\s*\.\.\s*(?<end>[a-z|A-Z|0-9|_|$|%]+)\s*:\s*(?<operation>.+)\]$";
        private static readonly string RegexPatternDataString = @"^\""(?<string>.*)\""$";
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

        public new static bool CanCreate(string operation)
        {
            var matched = Regex.Match(operation, RegexPatternDataOP, RegexOptions.Singleline | RegexOptions.IgnoreCase);
            var op1 = matched.Groups["op1"].Value;
            return (new[] { "DB", "DW", "DS", "DBS", "DWS" }).Any(m => string.Compare(m, op1, true) == 0);
        }

        public static OperationItem Create(LineDetailExpansionItemOperation lineDetailExpansionItemOperation, AsmAddress address, AsmLoad asmLoad)
        {
            var returnValue = default(OperationItemData);
            var matched = Regex.Match($"{lineDetailExpansionItemOperation.InstructionText} {lineDetailExpansionItemOperation.ArgumentText}", RegexPatternDataOP, RegexOptions.Singleline | RegexOptions.IgnoreCase);

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
                case "DS":
                case "DBS":
                    returnValue = DBDWS(DataTypeEnum.db, op2, lineDetailExpansionItemOperation, address);
                    break;
                case "DWS":
                    returnValue = DBDWS(DataTypeEnum.dw, op2, lineDetailExpansionItemOperation, address);
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
            var ops = op2.Split(',').Select(m => m.Trim()).ToArray();
            var matchString = Regex.Match(op2, RegexPatternDataString, RegexOptions.Singleline | RegexOptions.IgnoreCase);
            if (matchString.Success)
            {
                ops = System.Text.Encoding.ASCII.GetBytes(matchString.Groups["string"].Value).Select(m => m.ToString("0")).ToArray();
            }
            else
            {
                if (Regex.IsMatch(op2, RegexPatternDataFunction))
                {
                    ops = DBDW_Function(op2, lineDetailExpansionItemOperation, asmLoad).ToArray();
                }
            }

            return new OperationItemData()
            {
                ValueStrings = ops,
                DataType = dataType,
                Address = address,
                ItemDataLength = new AsmLength(ops.Length * (int)dataType),
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
                var startValue = (int)AIMath.ConvertTo<UInt16>(start, lineDetailExpansionItemOperation, asmLoad);
                var endValue = (int)AIMath.ConvertTo<UInt16>(end, lineDetailExpansionItemOperation, asmLoad);
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

        /// <summary>
        /// 指定数量分、データを作る処理
        /// </summary>
        /// <param name="dataType"></param>
        /// <param name="op2"></param>
        /// <param name="op3"></param>
        /// <param name="lineExpansionItem"></param>
        /// <param name="address"></param>
        /// <returns></returns>
        private static OperationItemData DBDWS(DataTypeEnum dataType, string op2, LineDetailExpansionItemOperation lineDetailExpansionItemOperation, AsmAddress address)
        {
            var returnValue = default(OperationItemData);
            var ops = op2.Split(',');
            var valueString = "0";

            if (ops.Length == 0 || ops.Length > 2)
            {
                throw new ErrorAssembleException(Error.ErrorCodeEnum.E0012);
            }
            if (ops.Length == 2)
            {
                valueString = ops[1];
            }
            
            var count = Convert.ToInt32(AIMath.Replace16Number(ops[0]));
            var valuesStrings = Enumerable.Range(0, count).Select(_ => valueString).ToArray();

            returnValue = new OperationItemData()
            {
                ValueStrings = valuesStrings,
                DataType = dataType,
                Address = address,
                ItemDataLength = new AsmLength(valuesStrings.Length * (int)dataType),
                LineDetailExpansionItemOperation = lineDetailExpansionItemOperation
            };

            return returnValue;
        }

        public override void Assemble(AsmLoad asmLoad)
        {
            var byteList = new List<byte>();
            switch (DataType)
            {
                case DataTypeEnum.dw:
                    foreach (var valueString in ValueStrings)
                    {
                        var value = AIMath.ConvertTo<UInt16>(valueString, LineDetailExpansionItemOperation, asmLoad);
                        byteList.Add((byte)(value % 256));
                        byteList.Add((byte)(value / 256));
                    }
                    break;
                case DataTypeEnum.db:
                    foreach (var valueString in ValueStrings)
                    {
                        byteList.Add((byte)AIMath.ConvertTo<UInt16>(valueString, LineDetailExpansionItemOperation, asmLoad));
                    }
                    break;
                default:
                    throw new InvalidOperationException(nameof(DataType));
            }

            ItemDataBin = byteList.ToArray();
        }
    }
}
