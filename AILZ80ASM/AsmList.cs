using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace AILZ80ASM
{
    public class AsmList
    {
        public enum NestedCodeTypeEnum
        {
            Macro,
            Repeat,
        }

        public UInt32? OutputAddress { get; set; }
        public UInt16? ProgramAddress { get; set; }
        public byte[] Bin { get; set; }
        public string Status { get; set; }
        public Stack<NestedCodeTypeEnum> NestedCodeTypes { get; set; }
        public string Source { get; set; }

        private AsmList()
        {
        }

        public static AsmList CreateFileInfoBOF(FileInfo fileInfo)
        {
            return CreateSource($"BOF:{fileInfo.Name}");
        }

        public static AsmList CreateFileInfoEOF(FileInfo fileInfo)
        {
            return CreateSource($"EOF:{fileInfo.Name}");
        }

        public static AsmList CreateSource(string target)
        {
            return Create(default(UInt32?), default(UInt16?), default(byte[]), "", target);
        }

        public static AsmList CreateLineItem(LineItem lineItem)
        {
            return CreateSource(lineItem.LineString);
        }
        public static AsmList CreateLineItemEqual(Label equLabel, LineItem lineItem)
        {
            return CreateLineItem(default(UInt32?), (ushort)(equLabel.Value & ushort.MaxValue), default(byte[]), "", lineItem);
        }

        public static AsmList CreateLineItemORG(AsmAddress address, AsmLength length, LineItem lineItem)
        {
            return CreateLineItem(new AsmAddress(address, length), default(byte[]), "", lineItem);
        }

        public static AsmList CreateLineItem(AsmAddress asmAdddress, byte[] bin, string status, LineItem lineItem)
        {
            return CreateLineItem(asmAdddress.Output, asmAdddress.Program, bin, status, lineItem);
        }

        public static AsmList CreateLineItem(UInt32? outputAddress, UInt16? programAddress, byte[] bin, string status, LineItem lineItem)
        {
            return Create(outputAddress, programAddress, bin, status, lineItem.LineString);
        }

        public static AsmList Create(UInt32? outputAddress, UInt16? programAddress, byte[] bin, string status, string source)
        {
            return new AsmList
            {
                OutputAddress = outputAddress,
                ProgramAddress = programAddress,
                Bin = bin,
                Status = status,
                Source = source
            };
        }

        public override string ToString()
        {
            var address1 = OutputAddress.HasValue ? $"{OutputAddress:X6}" : "";
            var address2 = ProgramAddress.HasValue ? $"{ProgramAddress:X4}" : "";
            var binary = Bin != default ? string.Concat(Bin.Select(m => $"{m:X2}")) : "";
            var codeType = "";
            var status = this.Status;
            var source = this.Source;
            if (this.Bin != default && this.Bin.Length > 16)
            {
                var startBin = this.Bin[0];
                if (this.Bin.Count(m => m == startBin) == this.Bin.Length)
                {
                    binary = $"{startBin:X2} LEN:{this.Bin.Length}";
                }
            }
            if (NestedCodeTypes != default && NestedCodeTypes.Count > 0)
            {
                codeType = "+";
            }

            var results = new List<string>();

            foreach (var item in Regex.Split(binary, @"(?<=\G.{16})(?!$)"))
            {
                address1 = address1.PadLeft(6);
                address2 = address2.PadLeft(4);
                status = status.PadLeft(2);
                codeType = codeType.PadLeft(1);
                var binaryString = item.PadRight(16);

                results.Add($"{address1} {address2} {binaryString}{status} {codeType}{source}");
                // クリアする
                address1 = "";
                address2 = "";
                status = "";
                source = "";
            }
            return string.Join(Environment.NewLine, results);
        }

        /// <summary>
        /// 展開されるコードのタイプを設定
        /// </summary>
        /// <param name="codeType"></param>
        public void PushNestedCodeType(NestedCodeTypeEnum nestedCodeTypeEnum)
        {
            if (NestedCodeTypes == default)
            {
                NestedCodeTypes = new Stack<NestedCodeTypeEnum>();
            }

            NestedCodeTypes.Push(nestedCodeTypeEnum);
        }

    }
}
