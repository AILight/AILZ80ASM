using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;

namespace AILZ80ASM
{
    public static class OPCodeTable
    {
        private static OPCodeItem[] OPCodeItems =
            {
                new OPCodeItem { Operation = "LD r1,r2", OPCode = new[] { "01DDDSSS" }, M = 1, T = 4 },
                new OPCodeItem { Operation = "LD r,n", OPCode = new[] { "00DDD110", "NNNNNNNN" }, M = 2, T = 7 },
                new OPCodeItem { Operation = "LD r,(HL)", OPCode = new[] { "01DDD110" }, M = 2, T = 7 },
                new OPCodeItem { Operation = "LD r,(IX+d)", OPCode = new[] { "11011101", "01DDD110", "NNNNNNNN" }, M = 5, T = 19 },
                new OPCodeItem { Operation = "LD r,(IY+d)", OPCode = new[] { "11111101", "01DDD110", "NNNNNNNN" }, M = 5, T = 19 },
            };

        public static OPCodeResult GetOPCodeItem(string code, Lable[] lables)
        {
            var matched = Regex.Match(code, @"(?<op1>^\S+)?\s(?<op2>[A-Z|a-z|0-9|$]+)?\s?,?\s?(?<op3>.+)?", RegexOptions.Singleline);

            var op1 = matched.Groups["op1"].Value.ToUpper();
            var op2 = matched.Groups["op2"].Value.ToUpper();
            var op3 = matched.Groups["op3"].Value.ToUpper();

            if (op1 == "ORG")
            {
                var address = Convert.ToUInt16(GeNumber16(op2), 2);
                return new OPCodeResult(address);
            }

            foreach (var opCodeItem in OPCodeItems)
            {
                var matchedOp = Regex.Match(opCodeItem.Operation, @"(?<op1>^\S+)?\s(?<op2>[A-Z|a-z|0-9|$]+)?\s?,?\s?(?<op3>.+)?", RegexOptions.Singleline);
                var op1Op = matchedOp.Groups["op1"].Value;
                var op2Op = matchedOp.Groups["op2"].Value;
                var op3Op = matchedOp.Groups["op3"].Value;

                if (op1 == op1Op)
                {
                    if (op2 == op2Op && op3 == op3Op)
                    {
                        return new OPCodeResult(opCodeItem);
                    }
                    else
                    {
                        var matchOP2 = false;
                        var matchOP3 = false;

                        var dddd = "";
                        var ssss = "";
                        var value8 = "";

                        switch (op2Op)
                        {
                            case "r1":
                            case "r":
                                if (!Is8BitRegister(op2))
                                    continue;
                                dddd = GetDDDSSS(op2);
                                matchOP2 = true;
                                break;
                            case "r2":
                                if (Is8BitRegister(op2))
                                    continue;
                                dddd = GetDDDSSS(op2);
                                matchOP2 = true;
                                break;
                            default:
                                break;
                        }

                        switch (op3Op)
                        {
                            case "r2":
                                if (!Is8BitRegister(op3))
                                    continue;
                                ssss = GetDDDSSS(op3);
                                matchOP3 = true;
                                break;
                            case "n":
                                if (IsRegister(op3) || IsAddrRegister(op3))
                                    continue;

                                value8 = GeNumber8(op3);
                                matchOP3 = true;
                                break;
                            default:
                                if (op3Op == op3)
                                    matchOP3 = true;
                                break;
                        }

                        if (matchOP2 && matchOP3)
                        {
                            var opcodes = opCodeItem.OPCode;
                            opcodes = opcodes.Select(m => m.Replace("DDD", dddd)
                                                           .Replace("SSS", ssss)
                                                           .Replace("NNNNNNNN", value8)).ToArray();

                            return new OPCodeResult(opcodes, opCodeItem.M, opCodeItem.T);
                        }
                    }
                }
            }
            return null;
        }

        private static string GeNumber16(string source)
        {
            if (source.IndexOf("$") == 0)
            {
                return Convert.ToString(Convert.ToInt16(source.Replace("$", ""), 16), 2).PadLeft(16, '0');
            }
            else
            {
                return Convert.ToString(Convert.ToInt16(source), 2).PadLeft(16, '0');
            }
        }

        private static string GeNumber8(string source)
        {
            if (source.IndexOf("$") == 0)
            {
                return Convert.ToString(Convert.ToByte(source.Replace("$", ""), 16), 2).PadLeft(8, '0');
            }
            else
            {
                return Convert.ToString(Convert.ToByte(source), 2).PadLeft(8, '0');
            }
        }

        private static bool IsRegister(string source)
        {
            return Is8BitRegister(source) | Is16BitRegister(source);
        }

        private static bool IsAccumulatorRegister(string source)
        {
            return source == "A";
        }

        private static bool Is8BitRegister(string source)
        {
            return Regex.IsMatch(source, "[A|B|C|D|E|H|L]");
        }

        private static bool Is16BitRegister(string source)
        {
            return Regex.IsMatch(source, "[|BC|DE|HL|SP]");
        }

        private static bool IsAddrHLRegister(string source)
        {
            return source == "(HL)";
        }

        private static bool IsAddrRegister(string source)
        {
            return Regex.IsMatch(source, "[|(BC)|(DE)|(HL)|(SP)]");
        }

        private static bool IsInterruptRegister(string source)
        {
            return source == "I";
        }

        private static bool IsRefreshRegister(string source)
        {
            return source == "R";
        }

        private static string GetDDDSSS(string source)
        {
            switch (source)
            {
                case "A":
                    return "111";
                case "B":
                    return "000";
                case "C":
                    return "001";
                case "D":
                    return "010";
                case "E":
                    return "011";
                case "H":
                    return "100";
                case "L":
                    return "101";
                default:
                    throw new Exception($"レジスタの指定が間違っています。{source}");
            }
        }
    }
}
