using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AILZ80ASM
{
    public static class AssembleList
    {
        //private static int AddressLength = 8

        public static void WriteLineFileInfoBOF(this StreamWriter streamWriter, FileInfo fileInfo)
        {
            streamWriter.WriteLineString($"BOF:{fileInfo.Name}");
        }

        public static void WriteLineFileInfoEOF(this StreamWriter streamWriter, FileInfo fileInfo)
        {
            streamWriter.WriteLineString($"EOF:{fileInfo.Name}");
        }

        public static void WriteLineItem(this StreamWriter streamWriter, LineItem lineItem)
        {
            streamWriter.WriteLineString(lineItem.LineString);
        }

        public static void WriteLineString(this StreamWriter streamWriter, string target)
        {
            streamWriter.WriteLineInternal("", "", "", "", target);
        }

        public static void WriteLineItem(this StreamWriter streamWriter, AsmAddress asmAdddress, byte[] bin, string status, LineItem lineItem)
        {
            streamWriter.WriteLineInternal($"{asmAdddress.Output:X6}", $"{asmAdddress.Program:X4}", string.Concat(bin.Select(m => $"{m:X2}")), status, lineItem.LineString);
        }

        private static void WriteLineInternal(this StreamWriter streamWriter, string address1, string address2, string binary, string status, string source)
        {
            foreach (var item in Regex.Split(binary, @"(?<=\G.{16})(?!$)"))
            {
                address1 = address1.PadLeft(6);
                address2 = address2.PadLeft(4);
                status = status.PadLeft(3);
                var binaryString = item.PadRight(16);

                streamWriter.WriteLine($"{address1} {address2} {binaryString}{status} {source}");
                // クリアする
                address1 = "";
                address2 = "";
                status = "";
                source = "";
            }
        }
    }
}
