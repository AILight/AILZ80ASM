using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;

namespace AILZ80ASM
{
    public static class OPCodeTable
    {
        private static readonly string RegexPatternOP = @"(?<op1>^\S+)?\s+(?<op2>[A-Z|a-z|0-9|$|\.|\+|\(|\)]+)?\s?,?\s?(?<op3>.+)?";
        private static readonly string RegexPatternIndexReg = @"^\((IX\+|IY\+)(?<value>\w)\)";
        private static readonly string RegexPatternAddress = @"^\((?<addr>[\w|$]+)\)$";

        private static OPCodeItem[] OPCodeItems =
            {
                // 8ビットの転送命令
                new OPCodeItem { Operation = "LD r1,r2", OPCode = new[] { "01DDDSSS" }, M = 1, T = 4 },
                new OPCodeItem { Operation = "LD r,n", OPCode = new[] { "00DDD110", "NNNNNNNN" }, M = 2, T = 7 },
                new OPCodeItem { Operation = "LD r,(HL)", OPCode = new[] { "01DDD110" }, M = 2, T = 7 },
                new OPCodeItem { Operation = "LD r,(IX+d)", OPCode = new[] { "11011101", "01DDD110", "NNNNNNNN" }, M = 5, T = 19 },
                new OPCodeItem { Operation = "LD r,(IY+d)", OPCode = new[] { "11111101", "01DDD110", "NNNNNNNN" }, M = 5, T = 19 },
                new OPCodeItem { Operation = "LD (HL),r", OPCode = new[] { "01110SSS" }, M = 2, T = 7 },
                new OPCodeItem { Operation = "LD (IX+d),r", OPCode = new[] { "11011101", "01110SSS" }, M = 5, T = 19 },
                new OPCodeItem { Operation = "LD (IY+d),r", OPCode = new[] { "11111101", "01110SSS" }, M = 5, T = 19 },
                new OPCodeItem { Operation = "LD (HL),n", OPCode = new[] { "00110110" }, M = 3, T = 10 },
                new OPCodeItem { Operation = "LD (IX+d),n", OPCode = new[] { "11011101", "00110110" }, M = 5, T = 19 },
                new OPCodeItem { Operation = "LD (IY+d),n", OPCode = new[] { "11111101", "00110110" }, M = 5, T = 19 },
                new OPCodeItem { Operation = "LD A,(BC)", OPCode = new[] { "00001010" }, M = 2, T = 7 },
                new OPCodeItem { Operation = "LD A,(DE)", OPCode = new[] { "00011010" }, M = 2, T = 7 },
                new OPCodeItem { Operation = "LD A,(nn)", OPCode = new[] { "00111010", "LLLLLLLL", "HHHHHHHH" }, M = 4, T = 13 },
                new OPCodeItem { Operation = "LD (BC),A", OPCode = new[] { "00000010" }, M = 2, T = 7 },
                new OPCodeItem { Operation = "LD (DE),A", OPCode = new[] { "00010010" }, M = 2, T = 7 },
                new OPCodeItem { Operation = "LD (nn),A", OPCode = new[] { "00111010", "LLLLLLLL", "HHHHHHHH" }, M = 4, T = 13 },
                new OPCodeItem { Operation = "LD A,I", OPCode = new[] { "11101101", "01010111" }, M = 2, T = 9 },
                new OPCodeItem { Operation = "LD I,A", OPCode = new[] { "11101101", "01000111" }, M = 2, T = 9 },
                new OPCodeItem { Operation = "LD A,R", OPCode = new[] { "11101101", "01011111" }, M = 2, T = 9 },
                new OPCodeItem { Operation = "LD R,A", OPCode = new[] { "11101101", "01001111" }, M = 2, T = 9 },
                // 16ビットの転送命令
                new OPCodeItem { Operation = "LD rp,nn", OPCode = new[] { "00RP0001", "LLLLLLLL", "HHHHHHHH" }, M = 3, T = 10 },
                new OPCodeItem { Operation = "LD IX,nn", OPCode = new[] { "11011101", "00100001", "LLLLLLLL", "HHHHHHHH" }, M = 4, T = 14 },
                new OPCodeItem { Operation = "LD IY,nn", OPCode = new[] { "11011101", "00100001", "LLLLLLLL", "HHHHHHHH" }, M = 4, T = 14 },
                new OPCodeItem { Operation = "LD HL,(nn)", OPCode = new[] { "00101010", "LLLLLLLL", "HHHHHHHH" }, M = 5, T = 16 },
                new OPCodeItem { Operation = "LD rp,(nn)", OPCode = new[] { "11101101", "01RP1011", "LLLLLLLL", "HHHHHHHH" }, M = 6, T = 20 },
                new OPCodeItem { Operation = "LD IX,(nn)", OPCode = new[] { "11011101", "00101010", "LLLLLLLL", "HHHHHHHH" }, M = 6, T = 20 },
                new OPCodeItem { Operation = "LD IY,(nn)", OPCode = new[] { "11111101", "00101010", "LLLLLLLL", "HHHHHHHH" }, M = 6, T = 20 },
                new OPCodeItem { Operation = "LD (nn),HL", OPCode = new[] { "00100010", "LLLLLLLL", "HHHHHHHH" }, M = 5, T = 16 },
                new OPCodeItem { Operation = "LD (nn),rp", OPCode = new[] { "11101101", "01RP0011", "LLLLLLLL", "HHHHHHHH" }, M = 6, T = 20 },
                new OPCodeItem { Operation = "LD (nn),IX", OPCode = new[] { "11011101", "00100010", "LLLLLLLL", "HHHHHHHH" }, M = 6, T = 20 },
                new OPCodeItem { Operation = "LD (nn),IY", OPCode = new[] { "11111101", "00100010", "LLLLLLLL", "HHHHHHHH" }, M = 6, T = 20 },
                new OPCodeItem { Operation = "LD SP,HL", OPCode = new[] { "11111001" }, M = 1, T = 6 },
                new OPCodeItem { Operation = "LD SP,IX", OPCode = new[] { "11011101", "11111001" }, M = 2, T = 10 },
                new OPCodeItem { Operation = "LD SP,IY", OPCode = new[] { "11111101", "11111001" }, M = 2, T = 10 },
            };

        public static OPCodeResult GetOPCodeItem(string code, Lable[] lables)
        {
            var matched = Regex.Match(code, RegexPatternOP, RegexOptions.Singleline);

            var op1 = matched.Groups["op1"].Value.ToUpper();
            var op2 = matched.Groups["op2"].Value.ToUpper();
            var op3 = matched.Groups["op3"].Value.ToUpper();

            if (op1 == "ORG")
            {
                var values = GetNumber16(op2);
                var address = Convert.ToUInt16(values[0] + values[1], 2);
                return new OPCodeResult(address);
            }

            if (op1 == "DB")
            {
                var vales = (op2 + (!string.IsNullOrEmpty(op3) ? "," : "") + op3).Split(',').Select(m => GetNumber8(m.Trim())).ToArray();
                return new OPCodeResult(vales);
            }

            foreach (var opCodeItem in OPCodeItems)
            {
                var matchedOp = Regex.Match(opCodeItem.Operation, RegexPatternOP, RegexOptions.Singleline);
                var tableOp1 = matchedOp.Groups["op1"].Value;
                var tableOp2 = matchedOp.Groups["op2"].Value;
                var tableOp3 = matchedOp.Groups["op3"].Value;

                if (op1 == tableOp1)
                {
                    if (op2 == tableOp2 && op3 == tableOp3)
                    {
                        return new OPCodeResult(opCodeItem);
                    }
                    else
                    {
                        var dddd = "";
                        var ssss = "";
                        var rp = "";
                        var value8 = "";
                        var value16 = new string[2];

                        if (!ProcessMark(op2, 0, tableOp2, ref dddd, ref ssss, ref rp, ref value8, ref value16))
                            continue;

                        if (!ProcessMark(op3, 1, tableOp3, ref dddd, ref ssss, ref rp, ref value8, ref value16))
                            continue;

                        var opcodes = opCodeItem.OPCode;
                        opcodes = opcodes.Select(m => m.Replace("DDD", dddd)
                                                        .Replace("SSS", ssss)
                                                        .Replace("RP", rp)
                                                        .Replace("NNNNNNNN", value8)
                                                        .Replace("HHHHHHHH", value16[0])
                                                        .Replace("LLLLLLLL", value16[1])).ToArray();

                        return new OPCodeResult(opcodes, opCodeItem.M, opCodeItem.T);
                    }
                }
            }
            return null;
        }

        private static bool ProcessMark (string op, int index, string tableOp, ref string dddd, ref string ssss, ref string rp, ref string value8, ref string[] value16)
        {
            switch (tableOp)
            {
                case "A":
                    if (!IsAccumulatorRegister(op))
                        return false;
                    break;
                case "HL":
                    if (!IsHLRegister(op))
                        return false;
                    break;
                case "SP":
                    if (!IsSPRegister(op))
                        return false;
                    break;
                case "IX":
                case "IY":
                    if (!IsAddrIndexRegister(op))
                        return false;
                    break;
                case "r":
                case "r1":
                case "r2":
                    if (!Is8BitRegister(op))
                        return false;

                    if (index == 0)
                    {
                        dddd = GetDDDSSS(op);
                    }
                    else
                    {
                        ssss = GetDDDSSS(op);
                    }
                    break;
                case "rp":
                    if (!Is16BitRegister(op))
                        return false;

                    rp = GetRP(op);
                    break;
                case "n":
                    if (!IsNumber8(op))
                        return false;

                    value8 = GetNumber8(op);
                    break;
                case "nn":
                    if (!IsNumber16(op))
                        return false;

                    value16 = GetNumber16(op);
                    break;
                case "(nn)":
                    if (!IsAddrNumber16(op))
                        return false;

                    value16 = GetAddNumber16(op);
                    break;
                case "(HL)":
                    if (!IsAddrHLRegister(op))
                        return false;

                    break;
                case "(BC)":
                    if (!IsAddrBCRegister(op))
                        return false;

                    break;
                case "(DE)":
                    if (!IsAddrDERegister(op))
                        return false;

                    break;
                case "(IX+d)":
                case "(IY+d)":
                    if (!IsAddrIndexWithDRegister(op))
                        return false;

                    var matchedIndex = Regex.Match(op, RegexPatternIndexReg);
                    var value = matchedIndex.Groups["value"].Value;
                    value8 = GetNumber8(value);

                    break;
                default:
                    break;
            }
            return true;
        }

        private static bool IsNumber16(string source)
        {
            return IsNumber8(source);
        }

        private static bool IsAddrNumber16(string source)
        {
            var matched = Regex.Match(source, RegexPatternAddress);
            if (!matched.Success)
                return false;

            return true;
        }

        private static string[] GetAddNumber16(string source)
        {
            var matched = Regex.Match(source, RegexPatternAddress);
            return GetNumber16(matched.Groups["addr"].Value);
        }

        private static string[] GetNumber16(string source)
        {
            var returnValues = new string[2];
            var tmpValue = "";
            if (source.IndexOf("$") == 0)
            {
                tmpValue = Convert.ToString(Convert.ToInt16(source.Replace("$", ""), 16), 2).PadLeft(16, '0');
            }
            else
            {
                tmpValue = Convert.ToString(Convert.ToInt16(source, 16), 2).PadLeft(16, '0');
            }
            returnValues[0] = tmpValue.Substring(0, 8);
            returnValues[1] = tmpValue.Substring(8);

            return returnValues;
        }

        private static bool IsNumber8(string source)
        {
            if (IsAllRegister(source))
                return false;

            // ()で囲まれた値は外す
            if (Regex.Match(source, @"^\([\w|$]+\)$").Success)
                return false;

            return true;
        }

        private static string GetNumber8(string source)
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

        private static bool IsAllRegister(string source)
        {
            return Is8BitRegister(source) ||
                   Is8BitIndexRegister(source) ||
                   Is16BitRegister(source) ||
                   IsAddrAndIndexRegister(source) ||
                   IsSPRegister(source) ||
                   IsRefreshRegister(source) ||
                   IsInterruptRegister(source) ||
                   IsPCRegister(source);
        }

        private static bool IsAccumulatorRegister(string source)
        {
            return source == "A";
        }


        private static bool IsHLRegister(string source)
        {
            return source == "HL";
        }

        private static bool IsBCRegister(string source)
        {
            return source == "BC";
        }

        private static bool IsDERegister(string source)
        {
            return source == "DE";
        }

        private static bool Is8BitRegister(string source)
        {
            return Regex.IsMatch(source, @"^(A|B|C|D|E|H|L)$");
        }

        private static bool Is8BitIndexRegister(string source)
        {
            return Regex.IsMatch(source, @"^(IXH|IXL|IYH|IYL)$");
        }

        private static bool Is16BitRegister(string source)
        {
            return Regex.IsMatch(source, @"^(BC|DE|HL|SP)$");
        }

        private static bool IsAddrAndIndexRegister(string source)
        {
            return IsAddrRegister(source) || IsAddrIndexWithDRegister(source);
        }

        private static bool IsAddrHLRegister(string source)
        {
            return source == "(HL)";
        }

        private static bool IsAddrBCRegister(string source)
        {
            return source == "(BC)";
        }

        private static bool IsAddrDERegister(string source)
        {
            return source == "(DE)";
        }

        private static bool IsAddrRegister(string source)
        {
            return Regex.IsMatch(source, @"^(\(BC\)|\(DE\)|\(HL\)|\(SP\))$");
        }

        private static bool IsAddrIndexRegister(string source)
        {
            return Regex.IsMatch(source, @"^(IX|IY)$");
        }

        private static bool IsAddrIndexWithDRegister(string source)
        {
            return Regex.IsMatch(source, RegexPatternIndexReg);
        }

        private static bool IsInterruptRegister(string source)
        {
            return source == "I";
        }

        private static bool IsRefreshRegister(string source)
        {
            return source == "R";
        }

        private static bool IsSPRegister(string source)
        {
            return source == "SP";
        }

        private static bool IsPCRegister(string source)
        {
            return source == "PC";
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

        private static string GetRP(string source)
        {
            switch (source)
            {
                case "BC":
                    return "00";
                case "DE":
                    return "01";
                case "HL":
                    return "10";
                case "SP":
                    return "11";
                default:
                    throw new Exception($"レジスタの指定が間違っています。{source}");
            }
        }
    }
}
