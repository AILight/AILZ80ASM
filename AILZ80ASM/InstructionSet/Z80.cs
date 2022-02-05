using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AILZ80ASM.InstructionSet
{
    public class Z80 : ISA
    {
        private static readonly string[] RegisterAndFlagNames = new[] { "A", "B", "C", "D", "E", "H", "L", "I", "F", "R", "IXH", "IXL", "IYH", "IYL", "AF", "AF'", "BC", "DE", "HL", "SP", "IX", "IY", "NZ", "Z", "NC", "C", "PO", "PE", "P", "M" };
        private static readonly InstructionSet Z80InstructionSet = new()
        {
            NumberReplaseChar = '$',
            SplitChars = new[] { ' ', ',' , '+', '(', ')' },
            RegisterAndFlagNames = RegisterAndFlagNames,
            InstructionRegisters = new[]
            {
                new InstructionRegister
                {
                    MnemonicRegisterName = "rs1",
                    MnemonicBitName = "DDD",
                    InstructionRegisterItems = new[]
                    {
                        new InstructionRegisterItem { RegisterName = "A", BitCode = "111" },
                        new InstructionRegisterItem { RegisterName = "B", BitCode = "000" },
                        new InstructionRegisterItem { RegisterName = "C", BitCode = "001" },
                        new InstructionRegisterItem { RegisterName = "D", BitCode = "010" },
                        new InstructionRegisterItem { RegisterName = "E", BitCode = "011" },
                    }
                },
                new InstructionRegister
                {
                    MnemonicRegisterName = "rs2",
                    MnemonicBitName = "SSS",
                    InstructionRegisterItems = new[]
                    {
                        new InstructionRegisterItem { RegisterName = "A", BitCode = "111" },
                        new InstructionRegisterItem { RegisterName = "B", BitCode = "000" },
                        new InstructionRegisterItem { RegisterName = "C", BitCode = "001" },
                        new InstructionRegisterItem { RegisterName = "D", BitCode = "010" },
                        new InstructionRegisterItem { RegisterName = "E", BitCode = "011" },
                    }
                },
                new InstructionRegister 
                {
                    MnemonicRegisterName = "r1", 
                    MnemonicBitName = "DDD",
                    InstructionRegisterItems = new[]
                    {
                        new InstructionRegisterItem { RegisterName = "A", BitCode = "111" },
                        new InstructionRegisterItem { RegisterName = "B", BitCode = "000" },
                        new InstructionRegisterItem { RegisterName = "C", BitCode = "001" },
                        new InstructionRegisterItem { RegisterName = "D", BitCode = "010" },
                        new InstructionRegisterItem { RegisterName = "E", BitCode = "011" },
                        new InstructionRegisterItem { RegisterName = "H", BitCode = "100" },
                        new InstructionRegisterItem { RegisterName = "L", BitCode = "101" },
                    }
                },
                new InstructionRegister
                {
                    MnemonicRegisterName = "r2",
                    MnemonicBitName = "SSS",
                    InstructionRegisterItems = new[]
                    {
                        new InstructionRegisterItem { RegisterName = "A", BitCode = "111" },
                        new InstructionRegisterItem { RegisterName = "B", BitCode = "000" },
                        new InstructionRegisterItem { RegisterName = "C", BitCode = "001" },
                        new InstructionRegisterItem { RegisterName = "D", BitCode = "010" },
                        new InstructionRegisterItem { RegisterName = "E", BitCode = "011" },
                        new InstructionRegisterItem { RegisterName = "H", BitCode = "100" },
                        new InstructionRegisterItem { RegisterName = "L", BitCode = "101" },
                    }
                },
                new InstructionRegister
                {
                    MnemonicRegisterName = "ixr1",
                    MnemonicBitName = "DDD",
                    InstructionRegisterItems = new[]
                    {
                        new InstructionRegisterItem { RegisterName = "IXH", BitCode = "100" },
                        new InstructionRegisterItem { RegisterName = "IXL", BitCode = "101" },
                    }
                },
                new InstructionRegister
                {
                    MnemonicRegisterName = "ixr2",
                    MnemonicBitName = "SSS",
                    InstructionRegisterItems = new[]
                    {
                        new InstructionRegisterItem { RegisterName = "IXH", BitCode = "100" },
                        new InstructionRegisterItem { RegisterName = "IXL", BitCode = "101" },
                    }
                },
                new InstructionRegister
                {
                    MnemonicRegisterName = "iyr1",
                    MnemonicBitName = "DDD",
                    InstructionRegisterItems = new[]
                    {
                        new InstructionRegisterItem { RegisterName = "IYH", BitCode = "100" },
                        new InstructionRegisterItem { RegisterName = "IYL", BitCode = "101" },
                    }
                },
                new InstructionRegister
                {
                    MnemonicRegisterName = "iyr2",
                    MnemonicBitName = "SSS",
                    InstructionRegisterItems = new[]
                    {
                        new InstructionRegisterItem { RegisterName = "IYH", BitCode = "100" },
                        new InstructionRegisterItem { RegisterName = "IYL", BitCode = "101" },
                    }
                },
                new InstructionRegister
                {
                    MnemonicRegisterName = "rp",
                    MnemonicBitName = "RP",
                    InstructionRegisterItems = new[]
                    {
                        new InstructionRegisterItem { RegisterName = "BC", BitCode = "00" },
                        new InstructionRegisterItem { RegisterName = "DE", BitCode = "01" },
                        new InstructionRegisterItem { RegisterName = "HL", BitCode = "10" },
                        new InstructionRegisterItem { RegisterName = "SP", BitCode = "11" },
                    }
                },
                new InstructionRegister
                {
                    MnemonicRegisterName = "rps",
                    MnemonicBitName = "RP",
                    InstructionRegisterItems = new[]
                    {
                        new InstructionRegisterItem { RegisterName = "BC", BitCode = "00" },
                        new InstructionRegisterItem { RegisterName = "DE", BitCode = "01" },
                        new InstructionRegisterItem { RegisterName = "SP", BitCode = "11" },
                    }
                },
                new InstructionRegister
                {
                    MnemonicRegisterName = "rpns",
                    MnemonicBitName = "RP",
                    InstructionRegisterItems = new[]
                    {
                        new InstructionRegisterItem { RegisterName = "BC", BitCode = "00" },
                        new InstructionRegisterItem { RegisterName = "DE", BitCode = "01" },
                        new InstructionRegisterItem { RegisterName = "HL", BitCode = "10" },
                    }
                },
                new InstructionRegister
                {
                    MnemonicRegisterName = "cc",
                    MnemonicBitName = "CCC",
                    InstructionRegisterItems = new[]
                    {
                        new InstructionRegisterItem { RegisterName = "NZ", BitCode = "000" },
                        new InstructionRegisterItem { RegisterName = "Z", BitCode = "001" },
                        new InstructionRegisterItem { RegisterName = "NC", BitCode = "010" },
                        new InstructionRegisterItem { RegisterName = "C", BitCode = "011" },
                        new InstructionRegisterItem { RegisterName = "PO", BitCode = "100" },
                        new InstructionRegisterItem { RegisterName = "PE", BitCode = "101" },
                        new InstructionRegisterItem { RegisterName = "P", BitCode = "110" },
                        new InstructionRegisterItem { RegisterName = "M", BitCode = "111" },
                    }
                },
                new InstructionRegister
                {
                    MnemonicRegisterName = "p",
                    MnemonicBitName = "TTT",
                    InstructionRegisterMode = InstructionRegister.InstructionRegisterModeEnum.RestartValue,
                },
                new InstructionRegister
                {
                    MnemonicRegisterName = "zero",
                    MnemonicBitName = "",
                    InstructionRegisterMode = InstructionRegister.InstructionRegisterModeEnum.Value0Bit,
                },
                new InstructionRegister
                {
                    MnemonicRegisterName = "n",
                    MnemonicBitName = "NNNNNNNN",
                    InstructionRegisterMode = InstructionRegister.InstructionRegisterModeEnum.Value8Bit,
                },
                new InstructionRegister
                {
                    MnemonicRegisterName = "nn",
                    MnemonicBitName = "HHHHHHHH,LLLLLLLL",
                    InstructionRegisterMode = InstructionRegister.InstructionRegisterModeEnum.Value16Bit,
                },
                new InstructionRegister
                {
                    MnemonicRegisterName = "d",
                    MnemonicBitName = "IIIIIIII",
                    InstructionRegisterMode = InstructionRegister.InstructionRegisterModeEnum.Value8Bit,
                },
                new InstructionRegister
                {
                    MnemonicRegisterName = "b",
                    MnemonicBitName = "BBB",
                    InstructionRegisterMode = InstructionRegister.InstructionRegisterModeEnum.Value3Bit,
                },
                new InstructionRegister
                {
                    MnemonicRegisterName = "e",
                    MnemonicBitName = "EEEEEEEE",
                    InstructionRegisterMode = InstructionRegister.InstructionRegisterModeEnum.RelativeAddress8Bit,
                },
                new InstructionRegister
                {
                    MnemonicRegisterName = "imv",
                    MnemonicBitName = "IV",
                    InstructionRegisterMode = InstructionRegister.InstructionRegisterModeEnum.InterruptModeValue,
                },
            },
            InstructionItems = new []
            {
                // 8ビットの転送命令
                new InstructionItem { Mnemonics = new[] { "LD r1,r2" }, OPCode = new[]{ "01DDDSSS" }, M = 1, T = 4 },
                new InstructionItem { Mnemonics = new[] { "LD r1,n" }, OPCode = new[] { "00DDD110", "NNNNNNNN" }, M = 2, T = 7 },
                new InstructionItem { Mnemonics = new[] { "LD r1,(HL)" }, OPCode = new[] { "01DDD110" }, M = 2, T = 7 },
                new InstructionItem { Mnemonics = new[] { "LD r1,(IX+d)" }, OPCode = new[] { "11011101", "01DDD110", "IIIIIIII" }, M = 5, T = 19 },
                new InstructionItem { Mnemonics = new[] { "LD r1,(IY+d)" }, OPCode = new[] { "11111101", "01DDD110", "IIIIIIII" }, M = 5, T = 19 },
                new InstructionItem { Mnemonics = new[] { "LD r1,(IX)" }, OPCode = new[] { "11011101", "01DDD110", "00000000" }, M = 5, T = 19, ErrorCode = Error.ErrorCodeEnum.W9001 },
                new InstructionItem { Mnemonics = new[] { "LD r1,(IY)" }, OPCode = new[] { "11111101", "01DDD110", "00000000" }, M = 5, T = 19, ErrorCode = Error.ErrorCodeEnum.W9002 },
                new InstructionItem { Mnemonics = new[] { "LD (HL),r2" }, OPCode = new[] { "01110SSS" }, M = 2, T = 7 },
                new InstructionItem { Mnemonics = new[] { "LD (IX+d),r2" }, OPCode = new[] { "11011101", "01110SSS", "IIIIIIII" }, M = 5, T = 19 },
                new InstructionItem { Mnemonics = new[] { "LD (IY+d),r2" }, OPCode = new[] { "11111101", "01110SSS", "IIIIIIII" }, M = 5, T = 19 },
                new InstructionItem { Mnemonics = new[] { "LD (IX),r2" }, OPCode = new[] { "11011101", "01110SSS", "00000000" }, M = 5, T = 19, ErrorCode = Error.ErrorCodeEnum.W9001 },
                new InstructionItem { Mnemonics = new[] { "LD (IY),r2" }, OPCode = new[] { "11111101", "01110SSS", "00000000" }, M = 5, T = 19, ErrorCode = Error.ErrorCodeEnum.W9002 },
                new InstructionItem { Mnemonics = new[] { "LD (HL),n" }, OPCode = new[] { "00110110", "NNNNNNNN" }, M = 3, T = 10 },
                new InstructionItem { Mnemonics = new[] { "LD (IX+d),n" }, OPCode = new[] { "11011101", "00110110", "IIIIIIII", "NNNNNNNN" }, M = 5, T = 19 },
                new InstructionItem { Mnemonics = new[] { "LD (IY+d),n" }, OPCode = new[] { "11111101", "00110110", "IIIIIIII", "NNNNNNNN" }, M = 5, T = 19 },
                new InstructionItem { Mnemonics = new[] { "LD (IX),n" }, OPCode = new[] { "11011101", "00110110", "00000000", "NNNNNNNN" }, M = 5, T = 19, ErrorCode = Error.ErrorCodeEnum.W9001 },
                new InstructionItem { Mnemonics = new[] { "LD (IY),n" }, OPCode = new[] { "11111101", "00110110", "00000000", "NNNNNNNN" }, M = 5, T = 19, ErrorCode = Error.ErrorCodeEnum.W9002 },
                new InstructionItem { Mnemonics = new[] { "LD A,(BC)" }, OPCode = new[] { "00001010" }, M = 2, T = 7 },
                new InstructionItem { Mnemonics = new[] { "LD A,(DE)" }, OPCode = new[] { "00011010" }, M = 2, T = 7 },
                new InstructionItem { Mnemonics = new[] { "LD A,(nn)" }, OPCode = new[] { "00111010", "LLLLLLLL", "HHHHHHHH" }, M = 4, T = 13 },
                new InstructionItem { Mnemonics = new[] { "LD (BC),A" }, OPCode = new[] { "00000010" }, M = 2, T = 7 },
                new InstructionItem { Mnemonics = new[] { "LD (DE),A" }, OPCode = new[] { "00010010" }, M = 2, T = 7 },
                new InstructionItem { Mnemonics = new[] { "LD (nn),A" }, OPCode = new[] { "00110010", "LLLLLLLL", "HHHHHHHH" }, M = 4, T = 13 },
                new InstructionItem { Mnemonics = new[] { "LD A,I" }, OPCode = new[] { "11101101", "01010111" }, M = 2, T = 9 },
                new InstructionItem { Mnemonics = new[] { "LD I,A" }, OPCode = new[] { "11101101", "01000111" }, M = 2, T = 9 },
                new InstructionItem { Mnemonics = new[] { "LD A,R" }, OPCode = new[] { "11101101", "01011111" }, M = 2, T = 9 },
                new InstructionItem { Mnemonics = new[] { "LD R,A" }, OPCode = new[] { "11101101", "01001111" }, M = 2, T = 9 },
                new InstructionItem { Mnemonics = new[] { "LD ixr1,rs2" }, OPCode = new[] { "11011101", "01DDDSSS" }, M = 2, T = 9, UnDocumented = true },
                new InstructionItem { Mnemonics = new[] { "LD rs1,ixr2" }, OPCode = new[] { "11011101", "01DDDSSS" }, M = 2, T = 9, UnDocumented = true },
                new InstructionItem { Mnemonics = new[] { "LD ixr1,n" }, OPCode = new[] { "11011101", "00DDD110", "NNNNNNNN" }, M = 3, T = 10, UnDocumented = true },
                new InstructionItem { Mnemonics = new[] { "LD iyr1,rs2" }, OPCode = new[] { "11111101", "01DDDSSS" }, M = 2, T = 9, UnDocumented = true },
                new InstructionItem { Mnemonics = new[] { "LD rs1,iyr2" }, OPCode = new[] { "11111101", "01DDDSSS" }, M = 2, T = 9, UnDocumented = true },
                new InstructionItem { Mnemonics = new[] { "LD iyr1,n" }, OPCode = new[] { "11111101", "00DDD110", "NNNNNNNN" }, M = 3, T = 10, UnDocumented = true },

                // 16ビットの転送命令
                new InstructionItem { Mnemonics = new[] { "LD rp,nn" }, OPCode = new[] { "00RP0001", "LLLLLLLL", "HHHHHHHH" }, M = 3, T = 10 },
                new InstructionItem { Mnemonics = new[] { "LD IX,nn" }, OPCode = new[] { "11011101", "00100001", "LLLLLLLL", "HHHHHHHH" }, M = 4, T = 14 },
                new InstructionItem { Mnemonics = new[] { "LD IY,nn" }, OPCode = new[] { "11111101", "00100001", "LLLLLLLL", "HHHHHHHH" }, M = 4, T = 14 },
                new InstructionItem { Mnemonics = new[] { "LD HL,(nn)" }, OPCode = new[] { "00101010", "LLLLLLLL", "HHHHHHHH" }, M = 5, T = 16 },
                new InstructionItem { Mnemonics = new[] { "LD rp,(nn)" }, OPCode = new[] { "11101101", "01RP1011", "LLLLLLLL", "HHHHHHHH" }, M = 6, T = 20 },
                new InstructionItem { Mnemonics = new[] { "LD IX,(nn)" }, OPCode = new[] { "11011101", "00101010", "LLLLLLLL", "HHHHHHHH" }, M = 6, T = 20 },
                new InstructionItem { Mnemonics = new[] { "LD IY,(nn)" }, OPCode = new[] { "11111101", "00101010", "LLLLLLLL", "HHHHHHHH" }, M = 6, T = 20 },
                new InstructionItem { Mnemonics = new[] { "LD (nn),HL" }, OPCode = new[] { "00100010", "LLLLLLLL", "HHHHHHHH" }, M = 5, T = 16 },
                new InstructionItem { Mnemonics = new[] { "LD (nn),rp" }, OPCode = new[] { "11101101", "01RP0011", "LLLLLLLL", "HHHHHHHH" }, M = 6, T = 20 },
                new InstructionItem { Mnemonics = new[] { "LD (nn),IX" }, OPCode = new[] { "11011101", "00100010", "LLLLLLLL", "HHHHHHHH" }, M = 6, T = 20 },
                new InstructionItem { Mnemonics = new[] { "LD (nn),IY" }, OPCode = new[] { "11111101", "00100010", "LLLLLLLL", "HHHHHHHH" }, M = 6, T = 20 },
                new InstructionItem { Mnemonics = new[] { "LD SP,HL" }, OPCode = new[] { "11111001" }, M = 1, T = 6 },
                new InstructionItem { Mnemonics = new[] { "LD SP,IX" }, OPCode = new[] { "11011101", "11111001" }, M = 2, T = 10 },
                new InstructionItem { Mnemonics = new[] { "LD SP,IY" }, OPCode = new[] { "11111101", "11111001" }, M = 2, T = 10 },
                // ブロック転送命令
                new InstructionItem { Mnemonics = new[] { "LDI" }, OPCode = new[] { "11101101", "10100000" }, M = 4, T = 16 },
                new InstructionItem { Mnemonics = new[] { "LDIR" }, OPCode = new[] { "11101101", "10110000" }, M = 0, T = 0 },
                new InstructionItem { Mnemonics = new[] { "LDD" }, OPCode = new[] { "11101101", "10101000" }, M = 4, T = 16 },
                new InstructionItem { Mnemonics = new[] { "LDDR" }, OPCode = new[] { "11101101", "10111000" }, M = 0, T = 0 },
                // 交換
                new InstructionItem { Mnemonics = new[] { "EX DE,HL" }, OPCode = new[] { "11101011" }, M = 1, T = 4 },
                new InstructionItem { Mnemonics = new[] { "EX HL,DE" }, OPCode = new[] { "11101011" }, M = 1, T = 4, ErrorCode = Error.ErrorCodeEnum.W9004 },
                new InstructionItem { Mnemonics = new[] { "EX AF,AF'" }, OPCode = new[] { "00001000" }, M = 1, T = 4 },
                new InstructionItem { Mnemonics = new[] { "EXX" }, OPCode = new[] { "11011001" }, M = 1, T = 4 },
                new InstructionItem { Mnemonics = new[] { "EX (SP),HL" }, OPCode = new[] { "11100011" }, M = 5, T = 19 },
                new InstructionItem { Mnemonics = new[] { "EX (SP),IX" }, OPCode = new[] { "11011101", "11100011" }, M = 6, T = 23 },
                new InstructionItem { Mnemonics = new[] { "EX (SP),IY" }, OPCode = new[] { "11111101", "11100011" }, M = 6, T = 23 },
                // スタック操作命令
                new InstructionItem { Mnemonics = new[] { "PUSH rpns" }, OPCode = new[] { "11RP0101" }, M = 3, T = 11 },
                new InstructionItem { Mnemonics = new[] { "PUSH AF" }, OPCode = new[] { "11110101" }, M = 3, T = 11 },
                new InstructionItem { Mnemonics = new[] { "PUSH IX" }, OPCode = new[] { "11011101", "11100101" }, M = 4, T = 15 },
                new InstructionItem { Mnemonics = new[] { "PUSH IY" }, OPCode = new[] { "11111101", "11100101" }, M = 4, T = 15 },
                new InstructionItem { Mnemonics = new[] { "POP rpns" }, OPCode = new[] { "11RP0001" }, M = 3, T = 10 },
                new InstructionItem { Mnemonics = new[] { "POP AF" }, OPCode = new[] { "11110001" }, M = 3, T = 10 },
                new InstructionItem { Mnemonics = new[] { "POP IX" }, OPCode = new[] { "11011101", "11100001" }, M = 4, T = 14 },
                new InstructionItem { Mnemonics = new[] { "POP IY" }, OPCode = new[] { "11111101", "11100001" }, M = 4, T = 14 },
                // 左ローテート
                new InstructionItem { Mnemonics = new[] { "RLCA" }, OPCode = new[] { "00000111" }, M = 1, T = 4 },
                new InstructionItem { Mnemonics = new[] { "RLA" }, OPCode = new[] { "00010111" }, M = 1, T = 4 },
                new InstructionItem { Mnemonics = new[] { "RLC r1" }, OPCode = new[] { "11001011", "00000DDD" }, M = 1, T = 4 },
                new InstructionItem { Mnemonics = new[] { "RLC (HL)" }, OPCode = new[] { "11001011", "00000110" }, M = 4, T = 15 },
                new InstructionItem { Mnemonics = new[] { "RLC (IX+d)" }, OPCode = new[] { "11011101", "11001011", "IIIIIIII", "00000110" }, M = 6, T = 23 },
                new InstructionItem { Mnemonics = new[] { "RLC (IY+d)" }, OPCode = new[] { "11111101", "11001011", "IIIIIIII", "00000110" }, M = 6, T = 23 },
                new InstructionItem { Mnemonics = new[] { "RLC (IX)" }, OPCode = new[] { "11011101", "11001011", "00000000", "00000110" }, M = 6, T = 23, ErrorCode = Error.ErrorCodeEnum.W9001 },
                new InstructionItem { Mnemonics = new[] { "RLC (IY)" }, OPCode = new[] { "11111101", "11001011", "00000000", "00000110" }, M = 6, T = 23, ErrorCode = Error.ErrorCodeEnum.W9002 },
                new InstructionItem { Mnemonics = new[] { "RLC r1,(IX+d)", "RLC (IX+d),r1" }, OPCode = new[] { "11011101", "11001011", "IIIIIIII", "00000DDD" }, M = 6, T = 23, UnDocumented = true },
                new InstructionItem { Mnemonics = new[] { "RLC r1,(IY+d)", "RLC (IY+d),r1" }, OPCode = new[] { "11111101", "11001011", "IIIIIIII", "00000DDD" }, M = 6, T = 23, UnDocumented = true },
                new InstructionItem { Mnemonics = new[] { "RLC r1,(IX)", "RLC (IX),r1" }, OPCode = new[] { "11011101", "11001011", "00000000", "00000DDD" }, M = 6, T = 23, UnDocumented = true, ErrorCode = Error.ErrorCodeEnum.W9001 },
                new InstructionItem { Mnemonics = new[] { "RLC r1,(IY)", "RLC (IY),r1" }, OPCode = new[] { "11111101", "11001011", "00000000", "00000DDD" }, M = 6, T = 23, UnDocumented = true, ErrorCode = Error.ErrorCodeEnum.W9002 },
                new InstructionItem { Mnemonics = new[] { "RL r1" }, OPCode = new[] { "11001011", "00010DDD" }, M = 2, T = 8 },
                new InstructionItem { Mnemonics = new[] { "RL (HL)" }, OPCode = new[] { "11001011", "00010110" }, M = 4, T = 15 },
                new InstructionItem { Mnemonics = new[] { "RL (IX+d)" }, OPCode = new[] { "11011101", "11001011", "IIIIIIII", "00010110" }, M = 6, T = 23 },
                new InstructionItem { Mnemonics = new[] { "RL (IY+d)" }, OPCode = new[] { "11111101", "11001011", "IIIIIIII", "00010110" }, M = 6, T = 23 },
                new InstructionItem { Mnemonics = new[] { "RL (IX)" }, OPCode = new[] { "11011101", "11001011", "00000000", "00010110" }, M = 6, T = 23, ErrorCode = Error.ErrorCodeEnum.W9001  },
                new InstructionItem { Mnemonics = new[] { "RL (IY)" }, OPCode = new[] { "11111101", "11001011", "00000000", "00010110" }, M = 6, T = 23, ErrorCode = Error.ErrorCodeEnum.W9002  },
                new InstructionItem { Mnemonics = new[] { "RL r1,(IX+d)", "RL (IX+d),r1" }, OPCode = new[] { "11011101", "11001011", "IIIIIIII", "00010DDD" }, M = 6, T = 23, UnDocumented = true },
                new InstructionItem { Mnemonics = new[] { "RL r1,(IY+d)", "RL (IY+d),r1" }, OPCode = new[] { "11111101", "11001011", "IIIIIIII", "00010DDD" }, M = 6, T = 23, UnDocumented = true },
                new InstructionItem { Mnemonics = new[] { "RL r1,(IX)", "RL (IX),r1" }, OPCode = new[] { "11011101", "11001011", "00000000", "00010DDD" }, M = 6, T = 23, UnDocumented = true, ErrorCode = Error.ErrorCodeEnum.W9001 },
                new InstructionItem { Mnemonics = new[] { "RL r1,(IY)", "RL (IY),r1" }, OPCode = new[] { "11111101", "11001011", "00000000", "00010DDD" }, M = 6, T = 23, UnDocumented = true, ErrorCode = Error.ErrorCodeEnum.W9002 },
                
                // 右ローテート
                new InstructionItem { Mnemonics = new[] { "RRCA" }, OPCode = new[] { "00001111" }, M = 1, T = 4 },
                new InstructionItem { Mnemonics = new[] { "RRA" }, OPCode = new[] { "00011111" }, M = 1, T = 4 },
                new InstructionItem { Mnemonics = new[] { "RRC r1" }, OPCode = new[] { "11001011", "00001DDD" }, M = 1, T = 4 },
                new InstructionItem { Mnemonics = new[] { "RRC (HL)" }, OPCode = new[] { "11001011", "00001110" }, M = 4, T = 15 },
                new InstructionItem { Mnemonics = new[] { "RRC (IX+d)" }, OPCode = new[] { "11011101", "11001011", "IIIIIIII", "00001110" }, M = 6, T = 23 },
                new InstructionItem { Mnemonics = new[] { "RRC (IY+d)" }, OPCode = new[] { "11111101", "11001011", "IIIIIIII", "00001110" }, M = 6, T = 23 },
                new InstructionItem { Mnemonics = new[] { "RRC (IX)" }, OPCode = new[] { "11011101", "11001011", "00000000", "00001110" }, M = 6, T = 23, ErrorCode = Error.ErrorCodeEnum.W9001 },
                new InstructionItem { Mnemonics = new[] { "RRC (IY)" }, OPCode = new[] { "11111101", "11001011", "00000000", "00001110" }, M = 6, T = 23, ErrorCode = Error.ErrorCodeEnum.W9002 },
                new InstructionItem { Mnemonics = new[] { "RRC r1,(IX+d)", "RRC (IX+d),r1" }, OPCode = new[] { "11011101", "11001011", "IIIIIIII", "00001DDD" }, M = 6, T = 23, UnDocumented = true },
                new InstructionItem { Mnemonics = new[] { "RRC r1,(IY+d)", "RRC (IY+d),r1" }, OPCode = new[] { "11111101", "11001011", "IIIIIIII", "00001DDD" }, M = 6, T = 23, UnDocumented = true },
                new InstructionItem { Mnemonics = new[] { "RRC r1,(IX)", "RRC (IX),r1" }, OPCode = new[] { "11011101", "11001011", "00000000", "00001DDD" }, M = 6, T = 23, UnDocumented = true, ErrorCode = Error.ErrorCodeEnum.W9001 },
                new InstructionItem { Mnemonics = new[] { "RRC r1,(IY)", "RRC (IY),r1" }, OPCode = new[] { "11111101", "11001011", "00000000", "00001DDD" }, M = 6, T = 23, UnDocumented = true, ErrorCode = Error.ErrorCodeEnum.W9002 },
                new InstructionItem { Mnemonics = new[] { "RR r1" }, OPCode = new[] { "11001011", "00011DDD" }, M = 2, T = 8 },
                new InstructionItem { Mnemonics = new[] { "RR (HL)" }, OPCode = new[] { "11001011", "00011110" }, M = 4, T = 15 },
                new InstructionItem { Mnemonics = new[] { "RR (IX+d)" }, OPCode = new[] { "11011101", "11001011", "IIIIIIII", "00011110" }, M = 6, T = 23 },
                new InstructionItem { Mnemonics = new[] { "RR (IY+d)" }, OPCode = new[] { "11111101", "11001011", "IIIIIIII", "00011110" }, M = 6, T = 23 },
                new InstructionItem { Mnemonics = new[] { "RR (IX)" }, OPCode = new[] { "11011101", "11001011", "00000000", "00011110" }, M = 6, T = 23, ErrorCode = Error.ErrorCodeEnum.W9001 },
                new InstructionItem { Mnemonics = new[] { "RR (IY)" }, OPCode = new[] { "11111101", "11001011", "00000000", "00011110" }, M = 6, T = 23, ErrorCode = Error.ErrorCodeEnum.W9002 },
                new InstructionItem { Mnemonics = new[] { "RR r1,(IX+d)", "RR (IX+d),r1" }, OPCode = new[] { "11011101", "11001011", "IIIIIIII", "00011DDD" }, M = 6, T = 23, UnDocumented = true },
                new InstructionItem { Mnemonics = new[] { "RR r1,(IY+d)", "RR (IY+d),r1" }, OPCode = new[] { "11111101", "11001011", "IIIIIIII", "00011DDD" }, M = 6, T = 23, UnDocumented = true },
                new InstructionItem { Mnemonics = new[] { "RR r1,(IX)", "RR (IX),r1" }, OPCode = new[] { "11011101", "11001011", "00000000", "00011DDD" }, M = 6, T = 23, UnDocumented = true, ErrorCode = Error.ErrorCodeEnum.W9001 },
                new InstructionItem { Mnemonics = new[] { "RR r1,(IY)", "RR (IY),r1" }, OPCode = new[] { "11111101", "11001011", "00000000", "00011DDD" }, M = 6, T = 23, UnDocumented = true, ErrorCode = Error.ErrorCodeEnum.W9002 },

                // 左シフト
                new InstructionItem { Mnemonics = new[] { "SLA r1" }, OPCode = new[] { "11001011", "00100DDD" }, M = 2, T = 8 },
                new InstructionItem { Mnemonics = new[] { "SLA (HL)" }, OPCode = new[] { "11001011", "00100110" }, M = 4, T = 15 },
                new InstructionItem { Mnemonics = new[] { "SLA (IX+d)" }, OPCode = new[] { "11011101", "11001011", "IIIIIIII", "00100110" }, M = 6, T = 23 },
                new InstructionItem { Mnemonics = new[] { "SLA (IY+d)" }, OPCode = new[] { "11111101", "11001011", "IIIIIIII", "00100110" }, M = 6, T = 23 },
                new InstructionItem { Mnemonics = new[] { "SLA (IX)" }, OPCode = new[] { "11011101", "11001011", "00000000", "00100110" }, M = 6, T = 23, ErrorCode = Error.ErrorCodeEnum.W9001 },
                new InstructionItem { Mnemonics = new[] { "SLA (IY)" }, OPCode = new[] { "11111101", "11001011", "00000000", "00100110" }, M = 6, T = 23, ErrorCode = Error.ErrorCodeEnum.W9002 },
                new InstructionItem { Mnemonics = new[] { "SLA r1,(IX+d)", "SLA (IX+d),r1" }, OPCode = new[] { "11011101", "11001011", "IIIIIIII", "00100DDD" }, M = 6, T = 23, UnDocumented = true },
                new InstructionItem { Mnemonics = new[] { "SLA r1,(IY+d)", "SLA (IY+d),r1"}, OPCode = new[] { "11111101", "11001011", "IIIIIIII", "00100DDD" }, M = 6, T = 23, UnDocumented = true },
                new InstructionItem { Mnemonics = new[] { "SLA r1,(IX)", "SLA (IX),r1" }, OPCode = new[] { "11011101", "11001011", "00000000", "00100DDD" }, M = 6, T = 23, UnDocumented = true, ErrorCode = Error.ErrorCodeEnum.W9001 },
                new InstructionItem { Mnemonics = new[] { "SLA r1,(IY)", "SLA (IY),r1"}, OPCode = new[] { "11111101", "11001011", "00000000", "00100DDD" }, M = 6, T = 23, UnDocumented = true, ErrorCode = Error.ErrorCodeEnum.W9002 },
                new InstructionItem { Mnemonics = new[] { "SLL r1" }, OPCode = new[] { "11001011", "00110DDD" }, M = 2, T = 8, UnDocumented = true },
                new InstructionItem { Mnemonics = new[] { "SLL (HL)" }, OPCode = new[] { "11001011", "00110110" }, M = 4, T = 15, UnDocumented = true },
                new InstructionItem { Mnemonics = new[] { "SLL (IX+d)" }, OPCode = new[] { "11011101", "11001011", "IIIIIIII", "00110110" }, M = 6, T = 23 },
                new InstructionItem { Mnemonics = new[] { "SLL (IY+d)" }, OPCode = new[] { "11111101", "11001011", "IIIIIIII", "00110110" }, M = 6, T = 23 },
                new InstructionItem { Mnemonics = new[] { "SLL (IX)" }, OPCode = new[] { "11011101", "11001011", "00000000", "00110110" }, M = 6, T = 23, ErrorCode = Error.ErrorCodeEnum.W9001 },
                new InstructionItem { Mnemonics = new[] { "SLL (IY)" }, OPCode = new[] { "11111101", "11001011", "00000000", "00110110" }, M = 6, T = 23, ErrorCode = Error.ErrorCodeEnum.W9002 },
                new InstructionItem { Mnemonics = new[] { "SLL r1,(IX+d)", "SLL (IX+d),r1" }, OPCode = new[] { "11011101", "11001011", "IIIIIIII", "00110DDD" }, M = 6, T = 23, UnDocumented = true },
                new InstructionItem { Mnemonics = new[] { "SLL r1,(IY+d)", "SLL (IY+d),r1" }, OPCode = new[] { "11111101", "11001011", "IIIIIIII", "00110DDD" }, M = 6, T = 23, UnDocumented = true },
                new InstructionItem { Mnemonics = new[] { "SLL r1,(IX)", "SLL (IX),r1" }, OPCode = new[] { "11011101", "11001011", "00000000", "00110DDD" }, M = 6, T = 23, UnDocumented = true, ErrorCode = Error.ErrorCodeEnum.W9001 },
                new InstructionItem { Mnemonics = new[] { "SLL r1,(IY)", "SLL (IY),r1" }, OPCode = new[] { "11111101", "11001011", "00000000", "00110DDD" }, M = 6, T = 23, UnDocumented = true, ErrorCode = Error.ErrorCodeEnum.W9002 },

                // 右シフト
                new InstructionItem { Mnemonics = new[] { "SRA r1" }, OPCode = new[] { "11001011", "00101DDD" }, M = 2, T = 8 },
                new InstructionItem { Mnemonics = new[] { "SRA (HL)" }, OPCode = new[] { "11001011", "00101110" }, M = 4, T = 15 },
                new InstructionItem { Mnemonics = new[] { "SRA (IX+d)" }, OPCode = new[] { "11011101", "11001011", "IIIIIIII", "00101110" }, M = 6, T = 23 },
                new InstructionItem { Mnemonics = new[] { "SRA (IY+d)" }, OPCode = new[] { "11111101", "11001011", "IIIIIIII", "00101110" }, M = 6, T = 23 },
                new InstructionItem { Mnemonics = new[] { "SRA (IX)" }, OPCode = new[] { "11011101", "11001011", "00000000", "00101110" }, M = 6, T = 23, ErrorCode = Error.ErrorCodeEnum.W9001 },
                new InstructionItem { Mnemonics = new[] { "SRA (IY)" }, OPCode = new[] { "11111101", "11001011", "00000000", "00101110" }, M = 6, T = 23, ErrorCode = Error.ErrorCodeEnum.W9002 },
                new InstructionItem { Mnemonics = new[] { "SRA r1,(IX+d)", "SRA (IX+d),r1" }, OPCode = new[] { "11011101", "11001011", "IIIIIIII", "00101DDD" }, M = 6, T = 23, UnDocumented = true },
                new InstructionItem { Mnemonics = new[] { "SRA r1,(IY+d)", "SRA (IY+d),r1"}, OPCode = new[] { "11111101", "11001011", "IIIIIIII", "00101DDD" }, M = 6, T = 23, UnDocumented = true },
                new InstructionItem { Mnemonics = new[] { "SRA r1,(IX)", "SRA (IX),r1" }, OPCode = new[] { "11011101", "11001011", "00000000", "00101DDD" }, M = 6, T = 23, UnDocumented = true,ErrorCode = Error.ErrorCodeEnum.W9001 },
                new InstructionItem { Mnemonics = new[] { "SRA r1,(IY)", "SRA (IY),r1"}, OPCode = new[] { "11111101", "11001011", "00000000", "00101DDD" }, M = 6, T = 23, UnDocumented = true,ErrorCode = Error.ErrorCodeEnum.W9002 },
                new InstructionItem { Mnemonics = new[] { "SRL r1" }, OPCode = new[] { "11001011", "00111DDD" }, M = 2, T = 8 },
                new InstructionItem { Mnemonics = new[] { "SRL (HL)" }, OPCode = new[] { "11001011", "00111110" }, M = 4, T = 15 },
                new InstructionItem { Mnemonics = new[] { "SRL (IX+d)" }, OPCode = new[] { "11011101", "11001011", "IIIIIIII", "00111110" }, M = 6, T = 23 },
                new InstructionItem { Mnemonics = new[] { "SRL (IY+d)" }, OPCode = new[] { "11111101", "11001011", "IIIIIIII", "00111110" }, M = 6, T = 23 },
                new InstructionItem { Mnemonics = new[] { "SRL (IX)" }, OPCode = new[] { "11011101", "11001011", "00000000", "00111110" }, M = 6, T = 23, ErrorCode = Error.ErrorCodeEnum.W9001 },
                new InstructionItem { Mnemonics = new[] { "SRL (IY)" }, OPCode = new[] { "11111101", "11001011", "00000000", "00111110" }, M = 6, T = 23, ErrorCode = Error.ErrorCodeEnum.W9002 },
                new InstructionItem { Mnemonics = new[] { "SRL r1,(IX+d)", "SRL (IX+d),r1" }, OPCode = new[] { "11011101", "11001011", "IIIIIIII", "00111DDD" }, M = 6, T = 23, UnDocumented = true },
                new InstructionItem { Mnemonics = new[] { "SRL r1,(IY+d)", "SRL (IY+d),r1" }, OPCode = new[] { "11111101", "11001011", "IIIIIIII", "00111DDD" }, M = 6, T = 23, UnDocumented = true },
                new InstructionItem { Mnemonics = new[] { "SRL r1,(IX)", "SRL (IX),r1" }, OPCode = new[] { "11011101", "11001011", "00000000", "00111DDD" }, M = 6, T = 23, UnDocumented = true, ErrorCode = Error.ErrorCodeEnum.W9001 },
                new InstructionItem { Mnemonics = new[] { "SRL r1,(IY)", "SRL (IY),r1" }, OPCode = new[] { "11111101", "11001011", "00000000", "00111DDD" }, M = 6, T = 23, UnDocumented = true, ErrorCode = Error.ErrorCodeEnum.W9002 },

                // 8ビットの加算
                new InstructionItem { Mnemonics = new[] { "ADD A,r2" }, OPCode = new[] { "10000SSS" }, M = 1, T = 4 },
                new InstructionItem { Mnemonics = new[] { "ADD A,n" }, OPCode = new[] { "11000110", "NNNNNNNN" }, M = 2, T = 7 },
                new InstructionItem { Mnemonics = new[] { "ADD A,(HL)" }, OPCode = new[] { "10000110" }, M = 2, T = 7 },
                new InstructionItem { Mnemonics = new[] { "ADD A,(IX+d)" }, OPCode = new[] { "11011101", "10000110", "IIIIIIII" }, M = 5, T = 19 },
                new InstructionItem { Mnemonics = new[] { "ADD A,(IY+d)" }, OPCode = new[] { "11111101", "10000110", "IIIIIIII" }, M = 5, T = 19 },
                new InstructionItem { Mnemonics = new[] { "ADD A,(IX)" }, OPCode = new[] { "11011101", "10000110", "00000000" }, M = 5, T = 19, ErrorCode = Error.ErrorCodeEnum.W9001 },
                new InstructionItem { Mnemonics = new[] { "ADD A,(IY)" }, OPCode = new[] { "11111101", "10000110", "00000000" }, M = 5, T = 19, ErrorCode = Error.ErrorCodeEnum.W9002 },
                new InstructionItem { Mnemonics = new[] { "ADD A,ixr2" }, OPCode = new[] { "11011101", "10000SSS" }, M = 2, T = 10, UnDocumented = true },
                new InstructionItem { Mnemonics = new[] { "ADD A,iyr2" }, OPCode = new[] { "11111101", "10000SSS" }, M = 2, T = 10, UnDocumented = true },
                new InstructionItem { Mnemonics = new[] { "ADC A,r2" }, OPCode = new[] { "10001SSS" }, M = 1, T = 4 },
                new InstructionItem { Mnemonics = new[] { "ADC A,n" }, OPCode = new[] { "11001110", "NNNNNNNN" }, M = 2, T = 7 },
                new InstructionItem { Mnemonics = new[] { "ADC A,(HL)" }, OPCode = new[] { "10001110" }, M = 2, T = 7 },
                new InstructionItem { Mnemonics = new[] { "ADC A,(IX+d)" }, OPCode = new[] { "11011101", "10001110", "IIIIIIII" }, M = 5, T = 19 },
                new InstructionItem { Mnemonics = new[] { "ADC A,(IY+d)" }, OPCode = new[] { "11111101", "10001110", "IIIIIIII" }, M = 5, T = 19 },
                new InstructionItem { Mnemonics = new[] { "ADC A,(IX)" }, OPCode = new[] { "11011101", "10001110", "00000000" }, M = 5, T = 19, ErrorCode = Error.ErrorCodeEnum.W9001 },
                new InstructionItem { Mnemonics = new[] { "ADC A,(IY)" }, OPCode = new[] { "11111101", "10001110", "00000000" }, M = 5, T = 19, ErrorCode = Error.ErrorCodeEnum.W9002 },
                new InstructionItem { Mnemonics = new[] { "ADC A,ixr2" }, OPCode = new[] { "11011101", "10001SSS" }, M = 2, T = 10, UnDocumented = true },
                new InstructionItem { Mnemonics = new[] { "ADC A,iyr2" }, OPCode = new[] { "11111101", "10001SSS" }, M = 2, T = 10, UnDocumented = true },
                new InstructionItem { Mnemonics = new[] { "INC r1" }, OPCode = new[] { "00DDD100" }, M = 1, T = 4 },
                new InstructionItem { Mnemonics = new[] { "INC (HL)" }, OPCode = new[] { "00110100" }, M = 3, T = 11 },
                new InstructionItem { Mnemonics = new[] { "INC (IX+d)" }, OPCode = new[] { "11011101", "00110100", "IIIIIIII" }, M = 6, T = 23 },
                new InstructionItem { Mnemonics = new[] { "INC (IY+d)" }, OPCode = new[] { "11111101", "00110100", "IIIIIIII" }, M = 6, T = 23 },
                new InstructionItem { Mnemonics = new[] { "INC (IX)" }, OPCode = new[] { "11011101", "00110100", "00000000" }, M = 6, T = 23, ErrorCode = Error.ErrorCodeEnum.W9001 },
                new InstructionItem { Mnemonics = new[] { "INC (IY)" }, OPCode = new[] { "11111101", "00110100", "00000000" }, M = 6, T = 23, ErrorCode = Error.ErrorCodeEnum.W9002 },
                new InstructionItem { Mnemonics = new[] { "INC ixr1" }, OPCode = new[] { "11011101", "00DDD100" }, M = 2, T = 10, UnDocumented = true },
                new InstructionItem { Mnemonics = new[] { "INC iyr1" }, OPCode = new[] { "11111101", "00DDD100" }, M = 2, T = 10, UnDocumented = true },
                // 8ビットの減算
                new InstructionItem { Mnemonics = new[] { "SUB r2" }, OPCode = new[] { "10010SSS" }, M = 1, T = 4 },
                new InstructionItem { Mnemonics = new[] { "SUB n" }, OPCode = new[] { "11010110", "NNNNNNNN" }, M = 2, T = 7 },
                new InstructionItem { Mnemonics = new[] { "SUB (HL)" }, OPCode = new[] { "10010110" }, M = 2, T = 7 },
                new InstructionItem { Mnemonics = new[] { "SUB (IX+d)" }, OPCode = new[] { "11011101", "10010110", "IIIIIIII" }, M = 5, T = 19 },
                new InstructionItem { Mnemonics = new[] { "SUB (IY+d)" }, OPCode = new[] { "11111101", "10010110", "IIIIIIII" }, M = 5, T = 19 },
                new InstructionItem { Mnemonics = new[] { "SUB (IX)" }, OPCode = new[] { "11011101", "10010110", "00000000" }, M = 5, T = 19, ErrorCode = Error.ErrorCodeEnum.W9001 },
                new InstructionItem { Mnemonics = new[] { "SUB (IY)" }, OPCode = new[] { "11111101", "10010110", "00000000" }, M = 5, T = 19, ErrorCode = Error.ErrorCodeEnum.W9002 },
                new InstructionItem { Mnemonics = new[] { "SUB ixr1" }, OPCode = new[] { "11011101", "10010DDD" }, M = 2, T = 10, UnDocumented = true },
                new InstructionItem { Mnemonics = new[] { "SUB iyr1" }, OPCode = new[] { "11111101", "10010DDD" }, M = 2, T = 10, UnDocumented = true },

                new InstructionItem { Mnemonics = new[] { "SUB A,r2" }, OPCode = new[]{ "10010SSS" }, M = 1, T = 4, ErrorCode = Error.ErrorCodeEnum.W9003 },
                new InstructionItem { Mnemonics = new[] { "SUB A,n" }, OPCode = new[]{ "11010110", "NNNNNNNN" }, M = 2, T = 7, ErrorCode = Error.ErrorCodeEnum.W9003 },
                new InstructionItem { Mnemonics = new[] { "SUB A,(HL)" }, OPCode = new[]{ "10010110" }, M = 2, T = 7, ErrorCode = Error.ErrorCodeEnum.W9003 },
                new InstructionItem { Mnemonics = new[] { "SUB A,(IX+d)" }, OPCode = new[]{ "11011101", "10010110", "IIIIIIII" }, M = 5, T = 19, ErrorCode = Error.ErrorCodeEnum.W9003 },
                new InstructionItem { Mnemonics = new[] { "SUB A,(IY+d)" }, OPCode = new[]{ "11111101", "10010110", "IIIIIIII" }, M = 5, T = 19, ErrorCode = Error.ErrorCodeEnum.W9003 },
                new InstructionItem { Mnemonics = new[] { "SUB A,(IX)" }, OPCode = new[]{ "11011101", "10010110", "00000000" }, M = 5, T = 19, ErrorCode = Error.ErrorCodeEnum.W9003 },
                new InstructionItem { Mnemonics = new[] { "SUB A,(IY)" }, OPCode = new[]{ "11111101", "10010110", "00000000" }, M = 5, T = 19, ErrorCode = Error.ErrorCodeEnum.W9003 },
                new InstructionItem { Mnemonics = new[] { "SUB A,ixr1" }, OPCode = new[] { "11011101", "10010DDD" }, M = 2, T = 10, UnDocumented = true, ErrorCode = Error.ErrorCodeEnum.W9003 },
                new InstructionItem { Mnemonics = new[] { "SUB A,iyr1" }, OPCode = new[] { "11111101", "10010DDD" }, M = 2, T = 10, UnDocumented = true, ErrorCode = Error.ErrorCodeEnum.W9003 },

                new InstructionItem { Mnemonics = new[] { "SBC A,r2" }, OPCode = new[] { "10011SSS" }, M = 1, T = 4 },
                new InstructionItem { Mnemonics = new[] { "SBC A,n" }, OPCode = new[] { "11011110", "NNNNNNNN" }, M = 2, T = 7 },
                new InstructionItem { Mnemonics = new[] { "SBC A,(HL)" }, OPCode = new[] { "10011110" }, M = 2, T = 7 },
                new InstructionItem { Mnemonics = new[] { "SBC A,(IX+d)" }, OPCode = new[] { "11011101", "10011110", "IIIIIIII" }, M = 5, T = 19 },
                new InstructionItem { Mnemonics = new[] { "SBC A,(IY+d)" }, OPCode = new[] { "11111101", "10011110", "IIIIIIII" }, M = 5, T = 19 },
                new InstructionItem { Mnemonics = new[] { "SBC A,(IX)" }, OPCode = new[] { "11011101", "10011110", "00000000" }, M = 5, T = 19, ErrorCode = Error.ErrorCodeEnum.W9001 },
                new InstructionItem { Mnemonics = new[] { "SBC A,(IY)" }, OPCode = new[] { "11111101", "10011110", "00000000" }, M = 5, T = 19, ErrorCode = Error.ErrorCodeEnum.W9002 },
                new InstructionItem { Mnemonics = new[] { "SBC A,ixr2" }, OPCode = new[] { "11011101", "10011SSS" }, M = 2, T = 10, UnDocumented = true },
                new InstructionItem { Mnemonics = new[] { "SBC A,iyr2" }, OPCode = new[] { "11111101", "10011SSS" }, M = 2, T = 10, UnDocumented = true },
                new InstructionItem { Mnemonics = new[] { "DEC r1" }, OPCode = new[] { "00DDD101" }, M = 1, T = 4 },
                new InstructionItem { Mnemonics = new[] { "DEC (HL)" }, OPCode = new[] { "00110101" }, M = 3, T = 11 },
                new InstructionItem { Mnemonics = new[] { "DEC (IX+d)" }, OPCode = new[] { "11011101", "00110101", "IIIIIIII" }, M = 6, T = 23 },
                new InstructionItem { Mnemonics = new[] { "DEC (IY+d)" }, OPCode = new[] { "11111101", "00110101", "IIIIIIII" }, M = 6, T = 23 },
                new InstructionItem { Mnemonics = new[] { "DEC (IX)" }, OPCode = new[] { "11011101", "00110101", "00000000" }, M = 6, T = 23, ErrorCode = Error.ErrorCodeEnum.W9001 },
                new InstructionItem { Mnemonics = new[] { "DEC (IY)" }, OPCode = new[] { "11111101", "00110101", "00000000" }, M = 6, T = 23, ErrorCode = Error.ErrorCodeEnum.W9002 },
                new InstructionItem { Mnemonics = new[] { "DEC ixr1" }, OPCode = new[] { "11011101", "00DDD101" }, M = 2, T = 10, UnDocumented = true },
                new InstructionItem { Mnemonics = new[] { "DEC iyr1" }, OPCode = new[] { "11111101", "00DDD101" }, M = 2, T = 10, UnDocumented = true },
                // 16ビットの加算
                new InstructionItem { Mnemonics = new[] { "ADD HL,rp" }, OPCode = new[] { "00RP1001" }, M = 3, T = 11 },
                new InstructionItem { Mnemonics = new[] { "ADC HL,rp" }, OPCode = new[] { "11101101", "01RP1010" }, M = 4, T = 15 },
                new InstructionItem { Mnemonics = new[] { "ADD IX,rps" }, OPCode = new[] { "11011101", "00RP1001" }, M = 4, T = 15 },
                new InstructionItem { Mnemonics = new[] { "ADD IY,rps" }, OPCode = new[] { "11111101", "00RP1001" }, M = 4, T = 15 },
                new InstructionItem { Mnemonics = new[] { "INC rp" }, OPCode = new[] { "00RP0011" }, M = 1, T = 6 },
                new InstructionItem { Mnemonics = new[] { "INC IX" }, OPCode = new[] { "11011101", "00100011" }, M = 2, T = 10 },
                new InstructionItem { Mnemonics = new[] { "INC IY" }, OPCode = new[] { "11111101", "00100011" }, M = 2, T = 10 },

                // 16ビットの減算
                new InstructionItem { Mnemonics = new[] { "SBC HL,rp" }, OPCode = new[] { "11101101", "01RP0010" }, M = 4, T = 15 },
                new InstructionItem { Mnemonics = new[] { "DEC rp" }, OPCode = new[] { "00RP1011" }, M = 1, T = 6 },
                new InstructionItem { Mnemonics = new[] { "DEC IX" }, OPCode = new[] { "11011101", "00101011" }, M = 2, T = 10 },
                new InstructionItem { Mnemonics = new[] { "DEC IY" }, OPCode = new[] { "11111101", "00101011" }, M = 2, T = 10 },
                // 論理演算
                new InstructionItem { Mnemonics = new[] { "AND r2" }, OPCode = new[] { "10100SSS" }, M = 1, T = 4 },
                new InstructionItem { Mnemonics = new[] { "AND n" }, OPCode = new[] { "11100110", "NNNNNNNN" }, M = 2, T = 7 },
                new InstructionItem { Mnemonics = new[] { "AND (HL)" }, OPCode = new[] { "10100110" }, M = 2, T = 7 },
                new InstructionItem { Mnemonics = new[] { "AND (IX+d)" }, OPCode = new[] { "11011101", "10100110", "IIIIIIII" }, M = 5, T = 19 },
                new InstructionItem { Mnemonics = new[] { "AND (IY+d)" }, OPCode = new[] { "11111101", "10100110", "IIIIIIII" }, M = 5, T = 19 },
                new InstructionItem { Mnemonics = new[] { "AND (IX)" }, OPCode = new[] { "11011101", "10100110", "00000000" }, M = 5, T = 19, ErrorCode = Error.ErrorCodeEnum.W9001 },
                new InstructionItem { Mnemonics = new[] { "AND (IY)" }, OPCode = new[] { "11111101", "10100110", "00000000" }, M = 5, T = 19, ErrorCode = Error.ErrorCodeEnum.W9002 },
                new InstructionItem { Mnemonics = new[] { "AND ixr2" }, OPCode = new[] { "11011101", "10100SSS" }, M = 2, T = 10, UnDocumented = true },
                new InstructionItem { Mnemonics = new[] { "AND iyr2" }, OPCode = new[] { "11111101", "10100SSS" }, M = 2, T = 10, UnDocumented = true },
                new InstructionItem { Mnemonics = new[] { "OR r2" }, OPCode = new[] { "10110SSS" }, M = 1, T = 4 },
                new InstructionItem { Mnemonics = new[] { "OR n" }, OPCode = new[] { "11110110", "NNNNNNNN" }, M = 2, T = 7 },
                new InstructionItem { Mnemonics = new[] { "OR (HL)" }, OPCode = new[] { "10110110" }, M = 2, T = 7 },
                new InstructionItem { Mnemonics = new[] { "OR (IX+d)" }, OPCode = new[] { "11011101", "10110110", "IIIIIIII" }, M = 5, T = 19 },
                new InstructionItem { Mnemonics = new[] { "OR (IY+d)" }, OPCode = new[] { "11111101", "10110110", "IIIIIIII" }, M = 5, T = 19 },
                new InstructionItem { Mnemonics = new[] { "OR (IX)" }, OPCode = new[] { "11011101", "10110110", "00000000" }, M = 5, T = 19, ErrorCode = Error.ErrorCodeEnum.W9001 },
                new InstructionItem { Mnemonics = new[] { "OR (IY)" }, OPCode = new[] { "11111101", "10110110", "00000000" }, M = 5, T = 19, ErrorCode = Error.ErrorCodeEnum.W9002 },
                new InstructionItem { Mnemonics = new[] { "OR ixr2" }, OPCode = new[] { "11011101", "10110SSS" }, M = 2, T = 10, UnDocumented = true },
                new InstructionItem { Mnemonics = new[] { "OR iyr2" }, OPCode = new[] { "11111101", "10110SSS" }, M = 2, T = 10, UnDocumented = true },
                new InstructionItem { Mnemonics = new[] { "XOR r2" }, OPCode = new[] { "10101SSS" }, M = 1, T = 4 },
                new InstructionItem { Mnemonics = new[] { "XOR n" }, OPCode = new[] { "11101110", "NNNNNNNN" }, M = 2, T = 7 },
                new InstructionItem { Mnemonics = new[] { "XOR (HL)" }, OPCode = new[] { "10101110" }, M = 2, T = 7 },
                new InstructionItem { Mnemonics = new[] { "XOR (IX+d)" }, OPCode = new[] { "11011101", "10101110", "IIIIIIII" }, M = 5, T = 19 },
                new InstructionItem { Mnemonics = new[] { "XOR (IY+d)" }, OPCode = new[] { "11111101", "10101110", "IIIIIIII" }, M = 5, T = 19 },
                new InstructionItem { Mnemonics = new[] { "XOR (IX)" }, OPCode = new[] { "11011101", "10101110", "00000000" }, M = 5, T = 19, ErrorCode = Error.ErrorCodeEnum.W9001 },
                new InstructionItem { Mnemonics = new[] { "XOR (IY)" }, OPCode = new[] { "11111101", "10101110", "00000000" }, M = 5, T = 19, ErrorCode = Error.ErrorCodeEnum.W9002 },
                new InstructionItem { Mnemonics = new[] { "XOR ixr2" }, OPCode = new[] { "11011101", "10101SSS" }, M = 2, T = 10, UnDocumented = true },
                new InstructionItem { Mnemonics = new[] { "XOR iyr2" }, OPCode = new[] { "11111101", "10101SSS" }, M = 2, T = 10, UnDocumented = true },
                new InstructionItem { Mnemonics = new[] { "CPL" }, OPCode = new[] { "00101111" }, M = 1, T = 4 },
                new InstructionItem { Mnemonics = new[] { "NEG" }, OPCode = new[] { "11101101", "01000100" }, M = 2, T = 8 },
                // ビット操作
                new InstructionItem { Mnemonics = new[] { "CCF" }, OPCode = new[] { "00111111" }, M = 1, T = 4 },
                new InstructionItem { Mnemonics = new[] { "SCF" }, OPCode = new[] { "00110111" }, M = 1, T = 4 },
                new InstructionItem { Mnemonics = new[] { "BIT b,r2" }, OPCode = new[] { "11001011", "01BBBSSS" }, M = 2, T = 8 },
                new InstructionItem { Mnemonics = new[] { "BIT b,(HL)" }, OPCode = new[] { "11001011", "01BBB110" }, M = 3, T = 13 },
                new InstructionItem { Mnemonics = new[] { "BIT b,(IX+d)" }, OPCode = new[] { "11011101", "11001011", "IIIIIIII", "01BBB110" }, M = 5, T = 20 },
                new InstructionItem { Mnemonics = new[] { "BIT b,(IY+d)" }, OPCode = new[] { "11111101", "11001011", "IIIIIIII", "01BBB110" }, M = 5, T = 20 },
                new InstructionItem { Mnemonics = new[] { "BIT b,(IX)" }, OPCode = new[] { "11011101", "11001011", "00000000", "01BBB110" }, M = 5, T = 20, ErrorCode = Error.ErrorCodeEnum.W9001 },
                new InstructionItem { Mnemonics = new[] { "BIT b,(IY)" }, OPCode = new[] { "11111101", "11001011", "00000000", "01BBB110" }, M = 5, T = 20, ErrorCode = Error.ErrorCodeEnum.W9002 },
                new InstructionItem { Mnemonics = new[] { "SET b,r2" }, OPCode = new[] { "11001011", "11BBBSSS" }, M = 2, T = 8 },
                new InstructionItem { Mnemonics = new[] { "SET b,(HL)" }, OPCode = new[] { "11001011", "11BBB110" }, M = 4, T = 15 },
                new InstructionItem { Mnemonics = new[] { "SET b,(IX+d)" }, OPCode = new[] { "11011101", "11001011", "IIIIIIII", "11BBB110" }, M = 6, T = 23 },
                new InstructionItem { Mnemonics = new[] { "SET b,(IY+d)" }, OPCode = new[] { "11111101", "11001011", "IIIIIIII", "11BBB110" }, M = 6, T = 23 },
                new InstructionItem { Mnemonics = new[] { "SET b,(IX)" }, OPCode = new[] { "11011101", "11001011", "00000000", "11BBB110" }, M = 6, T = 23, ErrorCode = Error.ErrorCodeEnum.W9001 },
                new InstructionItem { Mnemonics = new[] { "SET b,(IY)" }, OPCode = new[] { "11111101", "11001011", "00000000", "11BBB110" }, M = 6, T = 23, ErrorCode = Error.ErrorCodeEnum.W9002 },
                new InstructionItem { Mnemonics = new[] { "SET r1,b,(IX+d)", "SET b,(IX+d),r1" }, OPCode = new[] { "11011101", "11001011", "IIIIIIII", "11BBBDDD" }, M = 6, T = 23, UnDocumented = true },
                new InstructionItem { Mnemonics = new[] { "SET r1,b,(IY+d)", "SET b,(IY+d),r1" }, OPCode = new[] { "11111101", "11001011", "IIIIIIII", "11BBBDDD" }, M = 6, T = 23, UnDocumented = true },
                new InstructionItem { Mnemonics = new[] { "SET r1,b,(IX)", "SET b,(IX),r1" }, OPCode = new[] { "11011101", "11001011", "00000000", "11BBBDDD" }, M = 6, T = 23, UnDocumented = true, ErrorCode = Error.ErrorCodeEnum.W9001 },
                new InstructionItem { Mnemonics = new[] { "SET r1,b,(IY)", "SET b,(IY),r1" }, OPCode = new[] { "11111101", "11001011", "00000000", "11BBBDDD" }, M = 6, T = 23, UnDocumented = true, ErrorCode = Error.ErrorCodeEnum.W9002 },
                new InstructionItem { Mnemonics = new[] { "RES b,r2" }, OPCode = new[] { "11001011", "10BBBSSS" }, M = 2, T = 8 },
                new InstructionItem { Mnemonics = new[] { "RES b,(HL)" }, OPCode = new[] { "11001011", "10BBB110" }, M = 4, T = 15 },
                new InstructionItem { Mnemonics = new[] { "RES b,(IX+d)" }, OPCode = new[] { "11011101", "11001011", "IIIIIIII", "10BBB110" }, M = 6, T = 23 },
                new InstructionItem { Mnemonics = new[] { "RES b,(IY+d)" }, OPCode = new[] { "11111101", "11001011", "IIIIIIII", "10BBB110" }, M = 6, T = 23 },
                new InstructionItem { Mnemonics = new[] { "RES b,(IX)" }, OPCode = new[] { "11011101", "11001011", "00000000", "10BBB110" }, M = 6, T = 23, ErrorCode = Error.ErrorCodeEnum.W9001 },
                new InstructionItem { Mnemonics = new[] { "RES b,(IY)" }, OPCode = new[] { "11111101", "11001011", "00000000", "10BBB110" }, M = 6, T = 23, ErrorCode = Error.ErrorCodeEnum.W9002 },
                new InstructionItem { Mnemonics = new[] { "RES r1,b,(IX+d)", "RES b,(IX+d),r1" }, OPCode = new[] { "11011101", "11001011", "IIIIIIII", "10BBBDDD" }, M = 6, T = 23, UnDocumented = true },
                new InstructionItem { Mnemonics = new[] { "RES r1,b,(IY+d)", "RES b,(IY+d),r1" }, OPCode = new[] { "11111101", "11001011", "IIIIIIII", "10BBBDDD" }, M = 6, T = 23, UnDocumented = true },
                new InstructionItem { Mnemonics = new[] { "RES r1,b,(IX)", "RES b,(IX),r1" }, OPCode = new[] { "11011101", "11001011", "00000000", "10BBBDDD" }, M = 6, T = 23, UnDocumented = true, ErrorCode = Error.ErrorCodeEnum.W9001 },
                new InstructionItem { Mnemonics = new[] { "RES r1,b,(IY)", "RES b,(IY),r1" }, OPCode = new[] { "11111101", "11001011", "00000000", "10BBBDDD" }, M = 6, T = 23, UnDocumented = true, ErrorCode = Error.ErrorCodeEnum.W9002 },

                // サーチ
                new InstructionItem { Mnemonics = new[] { "CPI" }, OPCode = new[] { "11101101", "10100001" }, M = 4, T = 16 },
                new InstructionItem { Mnemonics = new[] { "CPIR" }, OPCode = new[] { "11101101", "10110001" }, M = 0, T = 0 },
                new InstructionItem { Mnemonics = new[] { "CPD" }, OPCode = new[] { "11101101", "10101001" }, M = 4, T = 16 },
                new InstructionItem { Mnemonics = new[] { "CPDR" }, OPCode = new[] { "11101101", "10111001" }, M = 0, T = 0 },
                // 比較
                new InstructionItem { Mnemonics = new[] { "CP r2" }, OPCode = new[] { "10111SSS" }, M = 1, T = 4 },
                new InstructionItem { Mnemonics = new[] { "CP n" }, OPCode = new[] { "11111110", "NNNNNNNN" }, M = 2, T = 7 },
                new InstructionItem { Mnemonics = new[] { "CP (HL)" }, OPCode = new[] { "10111110" }, M = 2, T = 7 },
                new InstructionItem { Mnemonics = new[] { "CP (IX+d)" }, OPCode = new[] { "11011101", "10111110", "IIIIIIII" }, M = 2, T = 7 },
                new InstructionItem { Mnemonics = new[] { "CP (IY+d)" }, OPCode = new[] { "11111101", "10111110", "IIIIIIII" }, M = 2, T = 7 },
                new InstructionItem { Mnemonics = new[] { "CP (IX)" }, OPCode = new[] { "11011101", "10111110", "00000000" }, M = 2, T = 7, ErrorCode = Error.ErrorCodeEnum.W9001 },
                new InstructionItem { Mnemonics = new[] { "CP (IY)" }, OPCode = new[] { "11111101", "10111110", "00000000" }, M = 2, T = 7, ErrorCode = Error.ErrorCodeEnum.W9002 },
                new InstructionItem { Mnemonics = new[] { "CP ixr2" }, OPCode = new[] { "11011101", "10111SSS" }, M = 2, T = 10, UnDocumented = true },
                new InstructionItem { Mnemonics = new[] { "CP iyr2" }, OPCode = new[] { "11111101", "10111SSS" }, M = 2, T = 10, UnDocumented = true },
                // ジャンプ
                new InstructionItem { Mnemonics = new[] { "JP nn" }, OPCode = new[] { "11000011", "LLLLLLLL", "HHHHHHHH" }, M = 3, T = 10 },
                new InstructionItem { Mnemonics = new[] { "JP cc,nn" }, OPCode = new[] { "11CCC010", "LLLLLLLL", "HHHHHHHH" }, M = 3, T = 10 },
                new InstructionItem { Mnemonics = new[] { "JR e" }, OPCode = new[] { "00011000", "EEEEEEEE" }, M = 3, T = 12 },
                new InstructionItem { Mnemonics = new[] { "JR C,e" }, OPCode = new[] { "00111000", "EEEEEEEE" }, M = 3, T = 12 },
                new InstructionItem { Mnemonics = new[] { "JR NC,e" }, OPCode = new[] { "00110000", "EEEEEEEE" }, M = 3, T = 12 },
                new InstructionItem { Mnemonics = new[] { "JR Z,e" }, OPCode = new[] { "00101000", "EEEEEEEE" }, M = 3, T = 12 },
                new InstructionItem { Mnemonics = new[] { "JR NZ,e" }, OPCode = new[] { "00100000", "EEEEEEEE" }, M = 3, T = 12 },
                new InstructionItem { Mnemonics = new[] { "JP (HL)" }, OPCode = new[] { "11101001" }, M = 1, T = 4 },
                new InstructionItem { Mnemonics = new[] { "JP (IX)" }, OPCode = new[] { "11011101", "11101001" }, M = 2, T = 8 },
                new InstructionItem { Mnemonics = new[] { "JP (IY)" }, OPCode = new[] { "11111101", "11101001" }, M = 2, T = 8 },
                new InstructionItem { Mnemonics = new[] { "DJNZ e" }, OPCode = new[] { "00010000", "EEEEEEEE" }, M = 3, T = 13 },
                // コール・リターン
                new InstructionItem { Mnemonics = new[] { "CALL nn" }, OPCode = new[] { "11001101", "LLLLLLLL", "HHHHHHHH" }, M = 5, T = 17 },
                new InstructionItem { Mnemonics = new[] { "CALL cc,nn" }, OPCode = new[] { "11CCC100", "LLLLLLLL", "HHHHHHHH" }, M = 5, T = 17 },
                new InstructionItem { Mnemonics = new[] { "RET" }, OPCode = new[] { "11001001" }, M = 3, T = 10 },
                new InstructionItem { Mnemonics = new[] { "RET cc" }, OPCode = new[] { "11CCC000" }, M = 3, T = 11 },
                new InstructionItem { Mnemonics = new[] { "RETI" }, OPCode = new[] { "11101101", "01001101" }, M = 4, T = 15 },
                new InstructionItem { Mnemonics = new[] { "RETN" }, OPCode = new[] { "11101101", "01000101" }, M = 4, T = 14 },
                new InstructionItem { Mnemonics = new[] { "RST p" }, OPCode = new[] { "11TTT111" }, M = 3, T = 12 },
                // CPU 制御命令:動作・割り込み設定
                new InstructionItem { Mnemonics = new[] { "NOP" }, OPCode = new[] { "00000000" }, M = 1, T = 4 },
                new InstructionItem { Mnemonics = new[] { "HALT" }, OPCode = new[] { "01110110" }, M = 1, T = 4 },
                new InstructionItem { Mnemonics = new[] { "DI" }, OPCode = new[] { "11110011" }, M = 1, T = 4 },
                new InstructionItem { Mnemonics = new[] { "EI" }, OPCode = new[] { "11111011" }, M = 1, T = 4 },
                new InstructionItem { Mnemonics = new[] { "IM imv" }, OPCode = new[] { "11101101", "010IV110" }, M = 2, T = 8 },
                // CPU 制御命令:動作・入力
                new InstructionItem { Mnemonics = new[] { "IN A,(n)" }, OPCode = new[] { "11011011", "NNNNNNNN" }, M = 3, T = 11 },
                new InstructionItem { Mnemonics = new[] { "IN r1,(C)" }, OPCode = new[] { "11101101", "01DDD000" }, M = 3, T = 12 },
                new InstructionItem { Mnemonics = new[] { "INI" }, OPCode = new[] { "11101101", "10100010" }, M = 4, T = 16 },
                new InstructionItem { Mnemonics = new[] { "INIR" }, OPCode = new[] { "11101101", "10110010" }, M = 0, T = 0 },
                new InstructionItem { Mnemonics = new[] { "IND" }, OPCode = new[] { "11101101", "10101010" }, M = 4, T = 16 },
                new InstructionItem { Mnemonics = new[] { "INDR" }, OPCode = new[] { "11101101", "10111010" }, M = 0, T = 0 },
                new InstructionItem { Mnemonics = new[] { "IN (C)", "IN F,(C)" }, OPCode = new[] { "11101101", "01110000" }, M = 3, T = 12, UnDocumented = true },
                // CPU 制御命令:動作・出力
                new InstructionItem { Mnemonics = new[] { "OUT (n),A" }, OPCode = new[] { "11010011", "NNNNNNNN" }, M = 3, T = 11 },
                new InstructionItem { Mnemonics = new[] { "OUT (C),r2" }, OPCode = new[] { "11101101", "01SSS001" }, M = 3, T = 12 },
                new InstructionItem { Mnemonics = new[] { "OUTI" }, OPCode = new[] { "11101101", "10100011" }, M = 4, T = 16 },
                new InstructionItem { Mnemonics = new[] { "OTIR" }, OPCode = new[] { "11101101", "10110011" }, M = 0, T = 0 },
                new InstructionItem { Mnemonics = new[] { "OUTD" }, OPCode = new[] { "11101101", "10101011" }, M = 4, T = 16 },
                new InstructionItem { Mnemonics = new[] { "OTDR" }, OPCode = new[] { "11101101", "10111011" }, M = 0, T = 0 },
                new InstructionItem { Mnemonics = new[] { "OUT (C),zero" }, OPCode = new[] { "11101101", "01110001" }, M = 3, T = 12, UnDocumented = true },
                // CPU 制御命令:二進化十進 (BCD) 用命令
                new InstructionItem { Mnemonics = new[] { "DAA" }, OPCode = new[] { "00100111" }, M = 1, T = 4 },
                new InstructionItem { Mnemonics = new[] { "RLD" }, OPCode = new[] { "11101101", "01101111" }, M = 5, T = 18 },
                new InstructionItem { Mnemonics = new[] { "RRD" }, OPCode = new[] { "11101101", "01100111" }, M = 5, T = 18 },
            }
        };

        static Z80()
        {
            Z80InstructionSet.MakeDataSet();
        }

        public Z80()
            : base(Z80InstructionSet)
        {
            Endianness = EndiannessEnum.LittleEndian;
        }

        /// <summary>
        /// Z80の命令を分解して、再度組み立てる
        /// </summary>
        /// <param name="instuction"></param>
        /// <returns></returns>
        public override AssembleParseResult ParseInstruction(string instuction)
        {
            var assembleParseResult = new AssembleParseResult();
            instuction = instuction.Replace("\t", " ");
            var index = instuction.IndexOf(" ");
            if (index == -1)
            {
                assembleParseResult.Instruction = instuction;
            }
            else
            {
                assembleParseResult.Instruction = instuction.Substring(0, index + 1);
                instuction = instuction.Substring(index + 1).TrimStart();

                var argument = "";
                var args = AIName.ParseArguments(instuction);
                foreach (var item in args)
                {
                    if (!string.IsNullOrEmpty(argument))
                    {
                        argument += ",";
                    }

                    if (IsMatchRegisterName(item) || IsMatchInstructionName(item))
                    {
                        argument += item;
                    }
                    else
                    {
                        if (item.StartsWith("(") && item.EndsWith(")"))
                        {
                            var tmpValue = item.Substring(1, item.Length - 2).Trim();
                            if (IsMatchRegisterName(tmpValue))
                            {
                                argument += $"({tmpValue})";
                            }
                            else if (tmpValue.StartsWith("IX", StringComparison.OrdinalIgnoreCase) ||
                                     tmpValue.StartsWith("IY", StringComparison.OrdinalIgnoreCase))
                            {
                                var indexPlus = tmpValue.IndexOf("+");
                                if (indexPlus == -1)
                                {
                                    // ここにはふつう来ない。ユーザーの記述ミス
                                    argument += $"({tmpValue})";
                                }
                                else
                                {
                                    // ここで変数を積む
                                    var valueString = tmpValue.Substring(indexPlus + 1).Trim();
                                    var argumentKey = $"{InstructionSet.NumberReplaseChar}{assembleParseResult.ArgumentDic.Count + 1}";
                                    assembleParseResult.ArgumentDic.Add(argumentKey, valueString);

                                    argument += $"({tmpValue.Substring(0, 2)}+{argumentKey})";
                                }
                            }
                            else
                            {
                                // ここで変数を積む
                                var valueString = item;
                                var argumentKey = $"{InstructionSet.NumberReplaseChar}{assembleParseResult.ArgumentDic.Count + 1}";
                                assembleParseResult.ArgumentDic.Add(argumentKey, valueString);

                                argument += $"({argumentKey})";
                            }
                        }
                        else
                        {
                            // ここで変数を積む
                            var valueString = item;
                            var argumentKey = $"{InstructionSet.NumberReplaseChar}{assembleParseResult.ArgumentDic.Count + 1}";
                            assembleParseResult.ArgumentDic.Add(argumentKey, valueString);

                            argument += $"{argumentKey}";
                        }
                    }
                }
                assembleParseResult.Instruction += argument;
            }

            return assembleParseResult;
        }
    }
}
