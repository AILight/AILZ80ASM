using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace AILZ80ASM
{
    public class Mnemonic
    {
        public enum OP_StatusEnum
        {
            ORG,
            OP,
            ERROR,
        }

        public OP_StatusEnum OP_Status { get; set; }
        public UInt16 Address { get; set; }
        public byte[] OPCode { get; set; }
        public int M { get; set; }
        public int T { get; set; }
        public string ErrorMessage { get; set; }

        private string op1 { get; set; }
        private string op2 { get; set; }
        private string op3 { get; set; }


        public Mnemonic(string code)
        {
            var matched = Regex.Match(code, @"(?<op1>^\S+)?\s(?<op2>[A-Z|a-z|0-9|$]+)?\s?,?\s?(?<op3>.+)?", RegexOptions.Singleline);

            op1 = matched.Groups["op1"].Value.ToUpper();
            op2 = matched.Groups["op2"].Value.ToUpper();
            op3 = matched.Groups["op3"].Value.ToUpper();

            switch (op1)
            {
                case "ORG":
                    OP_Status = OP_StatusEnum.ORG;
                    Address = GeNumber16(op2);
                    break;
                case "LD":
                    if (Is8BitRegister(op2))
                    {
                        if (Is8BitRegister(op3))
                        {
                            // LD r1, r2
                            OPCode = new byte[1];
                            OPCode[0] = 0b_0100_0000;
                            OPCode[0] |= (byte)(GetDDDSSS(op2) << 3 | GetDDDSSS(op2));
                            M = 1;
                            T = 4;
                        }
                        else if (IsAddrHLRegister(op2))
                        {
                            // LD r, (HL)
                            OPCode = new byte[1];
                            OPCode[0] = 0b_0100_0000;
                            OPCode[0] |= (byte)(GetDDDSSS(op2) << 3 | 0b_110);
                            M = 2;
                            T = 7;
                        }
                        else
                        {
                            // LD r, n
                            OPCode = new byte[2];
                            OPCode[0] = 0b_0000_0110;
                            OPCode[0] |= (byte)(GetDDDSSS(op2) << 3);
                            OPCode[1] = GeNumber8(op3);
                            M = 1;
                            T = 4;
                        }
                    }
                    break;
                default:
                    throw new Exception($"Error:{code}");
            }
        }

        public static UInt16 GeNumber16(string source)
        {
            if (source.IndexOf("$") == 0)
            {
                return Convert.ToUInt16(source.Replace("$", ""), 16);
            }
            else
            {
                return Convert.ToUInt16(source);
            }
        }

        public static byte GeNumber8(string source)
        {
            if (source.IndexOf("$") == 0)
            {
                return Convert.ToByte(source.Replace("$", ""), 16);
            }
            else
            {
                return Convert.ToByte(source);
            }
        }

        public static bool IsRegister(string source)
        {
            return Is8BitRegister(source) | Is16BitRegister(source);
        }

        public static bool IsAccumulatorRegister(string source)
        {
            return source == "A";
        }

        public static bool Is8BitRegister(string source)
        {
            return Regex.IsMatch(source, "[A|B|C|D|E|H|L]");
        }

        public static bool Is16BitRegister(string source)
        {
            return Regex.IsMatch(source, "[|BC|DE|HL|SP]");
        }

        public static bool IsAddrHLRegister(string source)
        {
            return source == "(HL)";
        }

        public static bool IsAddrRegister(string source)
        {
            return Regex.IsMatch(source, "[|(BC)|(DE)|(HL)|(SP)]");
        }

        public static bool IsInterruptRegister(string source)
        {
            return source == "I";
        }

        public static bool IsRefreshRegister(string source)
        {
            return source == "R";
        }

        public static byte GetDDDSSS(string source)
        {
            switch (source)
            {
                case "A":
                    return 0b_111;
                case "B":
                    return 0b_000;
                case "C":
                    return 0b_001;
                case "D":
                    return 0b_010;
                case "E":
                    return 0b_011;
                case "H":
                    return 0b_100;
                case "L":
                    return 0b_101;
                default:
                    throw new Exception($"レジスタの指定が間違っています。{source}");
            }
        }



    }
}
