﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;

namespace AILZ80ASM
{
    public static class OPCodeTable
    {
        public static readonly string RegexPatternOP = @"(?<op1>^\S+)?\s*(?<op2>[A-Z|a-z|0-9|$|\.|\-|\+|\(|\)|_]+)*\s*,*\s*(?<op3>.+)*";
        private static readonly string RegexPatternIXReg = @"^\(IX\+(?<value>.+)\)";
        private static readonly string RegexPatternIYReg = @"^\(IY\+(?<value>.+)\)";
        private static readonly string RegexPatternAddress = @"^\((?<addr>.+)\)$";

        private static OPCodeItem[] OPCodeItems =
            {
                // 8ビットの転送命令
                new OPCodeItem { Operation = "LD r1,r2", OPCode = new[] { "01DDDSSS" }, M = 1, T = 4 },
                new OPCodeItem { Operation = "LD r,n", OPCode = new[] { "00DDD110", "NNNNNNNN" }, M = 2, T = 7 },
                new OPCodeItem { Operation = "LD r,(HL)", OPCode = new[] { "01DDD110" }, M = 2, T = 7 },
                new OPCodeItem { Operation = "LD r,(IX+d)", OPCode = new[] { "11011101", "01DDD110", "IIIIIIII" }, M = 5, T = 19 },
                new OPCodeItem { Operation = "LD r,(IY+d)", OPCode = new[] { "11111101", "01DDD110", "IIIIIIII" }, M = 5, T = 19 },
                new OPCodeItem { Operation = "LD (HL),r", OPCode = new[] { "01110SSS" }, M = 2, T = 7 },
                new OPCodeItem { Operation = "LD (IX+d),r", OPCode = new[] { "11011101", "01110SSS", "IIIIIIII" }, M = 5, T = 19 },
                new OPCodeItem { Operation = "LD (IY+d),r", OPCode = new[] { "11111101", "01110SSS", "IIIIIIII" }, M = 5, T = 19 },
                new OPCodeItem { Operation = "LD (HL),n", OPCode = new[] { "00110110", "NNNNNNNN" }, M = 3, T = 10 },
                new OPCodeItem { Operation = "LD (IX+d),n", OPCode = new[] { "11011101", "00110110", "IIIIIIII", "NNNNNNNN" }, M = 5, T = 19 },
                new OPCodeItem { Operation = "LD (IY+d),n", OPCode = new[] { "11111101", "00110110", "IIIIIIII", "NNNNNNNN" }, M = 5, T = 19 },
                new OPCodeItem { Operation = "LD A,(BC)", OPCode = new[] { "00001010" }, M = 2, T = 7 },
                new OPCodeItem { Operation = "LD A,(DE)", OPCode = new[] { "00011010" }, M = 2, T = 7 },
                new OPCodeItem { Operation = "LD A,(nn)", OPCode = new[] { "00111010", "LLLLLLLL", "HHHHHHHH" }, M = 4, T = 13 },
                new OPCodeItem { Operation = "LD (BC),A", OPCode = new[] { "00000010" }, M = 2, T = 7 },
                new OPCodeItem { Operation = "LD (DE),A", OPCode = new[] { "00010010" }, M = 2, T = 7 },
                new OPCodeItem { Operation = "LD (nn),A", OPCode = new[] { "00110010", "LLLLLLLL", "HHHHHHHH" }, M = 4, T = 13 },
                new OPCodeItem { Operation = "LD A,I", OPCode = new[] { "11101101", "01010111" }, M = 2, T = 9 },
                new OPCodeItem { Operation = "LD I,A", OPCode = new[] { "11101101", "01000111" }, M = 2, T = 9 },
                new OPCodeItem { Operation = "LD A,R", OPCode = new[] { "11101101", "01011111" }, M = 2, T = 9 },
                new OPCodeItem { Operation = "LD R,A", OPCode = new[] { "11101101", "01001111" }, M = 2, T = 9 },
                // 16ビットの転送命令
                new OPCodeItem { Operation = "LD rp,nn", OPCode = new[] { "00RP0001", "LLLLLLLL", "HHHHHHHH" }, M = 3, T = 10 },
                new OPCodeItem { Operation = "LD IX,nn", OPCode = new[] { "11011101", "00100001", "LLLLLLLL", "HHHHHHHH" }, M = 4, T = 14 },
                new OPCodeItem { Operation = "LD IY,nn", OPCode = new[] { "11111101", "00100001", "LLLLLLLL", "HHHHHHHH" }, M = 4, T = 14 },
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
                // ブロック転送命令
                new OPCodeItem { Operation = "LDI", OPCode = new[] { "11101101", "10100000" }, M = 4, T = 16 },
                new OPCodeItem { Operation = "LDIR", OPCode = new[] { "11101101", "10110000" }, M = 0, T = 0 },
                new OPCodeItem { Operation = "LDD", OPCode = new[] { "11101101", "10101000" }, M = 4, T = 16 },
                new OPCodeItem { Operation = "LDDR", OPCode = new[] { "11101101", "10111000" }, M = 0, T = 0 },
                // 交換
                new OPCodeItem { Operation = "EX DE,HL", OPCode = new[] { "11101011" }, M = 1, T = 4 },
                new OPCodeItem { Operation = "EX AF,AF'", OPCode = new[] { "00001000" }, M = 1, T = 4 },
                new OPCodeItem { Operation = "EXX", OPCode = new[] { "11011001" }, M = 1, T = 4 },
                new OPCodeItem { Operation = "EX (SP),HL", OPCode = new[] { "11100011" }, M = 5, T = 19 },
                new OPCodeItem { Operation = "EX (SP),IX", OPCode = new[] { "11011101", "11100011" }, M = 6, T = 23 },
                new OPCodeItem { Operation = "EX (SP),IY", OPCode = new[] { "11111101", "11100011" }, M = 6, T = 23 },
                // スタック操作命令
                new OPCodeItem { Operation = "PUSH rp", OPCode = new[] { "11RP0101" }, M = 3, T = 11 },
                new OPCodeItem { Operation = "PUSH AF", OPCode = new[] { "11110101" }, M = 3, T = 11 },
                new OPCodeItem { Operation = "PUSH IX", OPCode = new[] { "11011101", "11100101" }, M = 4, T = 15 },
                new OPCodeItem { Operation = "PUSH IY", OPCode = new[] { "11111101", "11100101" }, M = 4, T = 15 },
                new OPCodeItem { Operation = "POP rp", OPCode = new[] { "11RP0001" }, M = 3, T = 10 },
                new OPCodeItem { Operation = "POP AF", OPCode = new[] { "11110001" }, M = 3, T = 10 },
                new OPCodeItem { Operation = "POP IX", OPCode = new[] { "11011101", "11100001" }, M = 4, T = 14 },
                new OPCodeItem { Operation = "POP IY", OPCode = new[] { "11111101", "11100001" }, M = 4, T = 14 },
                // 左ローテート
                new OPCodeItem { Operation = "RLCA", OPCode = new[] { "00000111" }, M = 1, T = 4 },
                new OPCodeItem { Operation = "RLA", OPCode = new[] { "00010111" }, M = 1, T = 4 },
                new OPCodeItem { Operation = "RLC r", OPCode = new[] { "11001011", "00000DDD" }, M = 1, T = 4 },
                new OPCodeItem { Operation = "RLC (HL)", OPCode = new[] { "11001011", "00000110" }, M = 4, T = 15 },
                new OPCodeItem { Operation = "RLC (IX+d)", OPCode = new[] { "11011101", "11001011", "IIIIIIII", "00000110" }, M = 6, T = 23 },
                new OPCodeItem { Operation = "RLC (IY+d)", OPCode = new[] { "11111101", "11001011", "IIIIIIII", "00000110" }, M = 6, T = 23 },
                new OPCodeItem { Operation = "RL r", OPCode = new[] { "11001011", "00010DDD" }, M = 2, T = 8 },
                new OPCodeItem { Operation = "RL (HL)", OPCode = new[] { "11001011", "00010110" }, M = 4, T = 15 },
                new OPCodeItem { Operation = "RL (IX+d)", OPCode = new[] { "11011101", "11001011", "IIIIIIII", "00010110" }, M = 6, T = 23 },
                new OPCodeItem { Operation = "RL (IY+d)", OPCode = new[] { "11111101", "11001011", "IIIIIIII", "00010110" }, M = 6, T = 23 },
                // 右ローテート
                new OPCodeItem { Operation = "RRCA", OPCode = new[] { "00001111" }, M = 1, T = 4 },
                new OPCodeItem { Operation = "RRA", OPCode = new[] { "00011111" }, M = 1, T = 4 },
                new OPCodeItem { Operation = "RRC r", OPCode = new[] { "11001011", "00001DDD" }, M = 1, T = 4 },
                new OPCodeItem { Operation = "RRC (HL)", OPCode = new[] { "11001011", "00001110" }, M = 4, T = 15 },
                new OPCodeItem { Operation = "RRC (IX+d)", OPCode = new[] { "11011101", "11001011", "IIIIIIII", "00001110" }, M = 6, T = 23 },
                new OPCodeItem { Operation = "RRC (IY+d)", OPCode = new[] { "11111101", "11001011", "IIIIIIII", "00001110" }, M = 6, T = 23 },
                new OPCodeItem { Operation = "RR r", OPCode = new[] { "11001011", "00011DDD" }, M = 2, T = 8 },
                new OPCodeItem { Operation = "RR (HL)", OPCode = new[] { "11001011", "00011110" }, M = 4, T = 15 },
                new OPCodeItem { Operation = "RR (IX+d)", OPCode = new[] { "11011101", "11001011", "IIIIIIII", "00011110" }, M = 6, T = 23 },
                new OPCodeItem { Operation = "RR (IY+d)", OPCode = new[] { "11111101", "11001011", "IIIIIIII", "00011110" }, M = 6, T = 23 },
                // 左シフト
                new OPCodeItem { Operation = "SLA r", OPCode = new[] { "11001011", "00100DDD" }, M = 2, T = 8 },
                new OPCodeItem { Operation = "SLA (HL)", OPCode = new[] { "11001011", "00100110" }, M = 4, T = 15 },
                new OPCodeItem { Operation = "SLA (IX+d)", OPCode = new[] { "11011101", "11001011", "IIIIIIII", "00100110" }, M = 6, T = 23 },
                new OPCodeItem { Operation = "SLA (IY+d)", OPCode = new[] { "11111101", "11001011", "IIIIIIII", "00100110" }, M = 6, T = 23 },
                // 右シフト
                new OPCodeItem { Operation = "SRA r", OPCode = new[] { "11001011", "00101DDD" }, M = 2, T = 8 },
                new OPCodeItem { Operation = "SRA (HL)", OPCode = new[] { "11001011", "00101110" }, M = 4, T = 15 },
                new OPCodeItem { Operation = "SRA (IX+d)", OPCode = new[] { "11011101", "11001011", "IIIIIIII", "00101110" }, M = 6, T = 23 },
                new OPCodeItem { Operation = "SRA (IY+d)", OPCode = new[] { "11111101", "11001011", "IIIIIIII", "00101110" }, M = 6, T = 23 },
                new OPCodeItem { Operation = "SRL r", OPCode = new[] { "11001011", "00111DDD" }, M = 2, T = 8 },
                new OPCodeItem { Operation = "SRL (HL)", OPCode = new[] { "11001011", "00111110" }, M = 4, T = 15 },
                new OPCodeItem { Operation = "SRL (IX+d)", OPCode = new[] { "11011101", "11001011", "IIIIIIII", "00111110" }, M = 6, T = 23 },
                new OPCodeItem { Operation = "SRL (IY+d)", OPCode = new[] { "11111101", "11001011", "IIIIIIII", "00111110" }, M = 6, T = 23 },
                // 8ビットの加算
                new OPCodeItem { Operation = "ADD A,r", OPCode = new[] { "10000SSS" }, M = 1, T = 4 },
                new OPCodeItem { Operation = "ADD A,n", OPCode = new[] { "11000110", "NNNNNNNN" }, M = 2, T = 7 },
                new OPCodeItem { Operation = "ADD A,(HL)", OPCode = new[] { "10000110" }, M = 2, T = 7 },
                new OPCodeItem { Operation = "ADD A,(IX+d)", OPCode = new[] { "11011101", "10000110", "IIIIIIII" }, M = 5, T = 19 },
                new OPCodeItem { Operation = "ADD A,(IY+d)", OPCode = new[] { "11111101", "10000110", "IIIIIIII" }, M = 5, T = 19 },
                new OPCodeItem { Operation = "ADC A,r", OPCode = new[] { "10001SSS" }, M = 1, T = 4 },
                new OPCodeItem { Operation = "ADC A,n", OPCode = new[] { "11001110", "NNNNNNNN" }, M = 2, T = 7 },
                new OPCodeItem { Operation = "ADC A,(HL)", OPCode = new[] { "10001110" }, M = 2, T = 7 },
                new OPCodeItem { Operation = "ADC A,(IX+d)", OPCode = new[] { "11011101", "10001110", "IIIIIIII" }, M = 5, T = 19 },
                new OPCodeItem { Operation = "ADC A,(IY+d)", OPCode = new[] { "11111101", "10001110", "IIIIIIII" }, M = 5, T = 19 },
                new OPCodeItem { Operation = "INC r", OPCode = new[] { "00DDD100" }, M = 1, T = 4 },
                new OPCodeItem { Operation = "INC (HL)", OPCode = new[] { "00110100" }, M = 3, T = 11 },
                new OPCodeItem { Operation = "INC (IX+d)", OPCode = new[] { "11011101", "00110100", "IIIIIIII" }, M = 6, T = 23 },
                new OPCodeItem { Operation = "INC (IY+d)", OPCode = new[] { "11111101", "00110100", "IIIIIIII" }, M = 6, T = 23 },
                // 8ビットの減算
                new OPCodeItem { Operation = "SUB r", OPCode = new[] { "10010DDD" }, M = 1, T = 4 },
                new OPCodeItem { Operation = "SUB n", OPCode = new[] { "11010110", "NNNNNNNN" }, M = 2, T = 7 },
                new OPCodeItem { Operation = "SUB (HL)", OPCode = new[] { "10010110" }, M = 2, T = 7 },
                new OPCodeItem { Operation = "SUB (IX+d)", OPCode = new[] { "11011101", "10010110", "IIIIIIII" }, M = 5, T = 19 },
                new OPCodeItem { Operation = "SUB (IY+d)", OPCode = new[] { "11111101", "10010110", "IIIIIIII" }, M = 5, T = 19 },
                /*
                new OPCodeItem { Operation = "SUB A,r", OPCode = new[] { "10010SSS" }, M = 1, T = 4, AccumulatorExtra = true },
                new OPCodeItem { Operation = "SUB A,n", OPCode = new[] { "11010110", "NNNNNNNN" }, M = 2, T = 7, AccumulatorExtra = true },
                new OPCodeItem { Operation = "SUB A,(HL)", OPCode = new[] { "10010110" }, M = 2, T = 7, AccumulatorExtra = true },
                new OPCodeItem { Operation = "SUB A,(IX+d)", OPCode = new[] { "11011101", "10010110", "IIIIIIII" }, M = 5, T = 19, AccumulatorExtra = true },
                new OPCodeItem { Operation = "SUB A,(IY+d)", OPCode = new[] { "11111101", "10010110", "IIIIIIII" }, M = 5, T = 19, AccumulatorExtra = true },
                */
                new OPCodeItem { Operation = "SBC A,r", OPCode = new[] { "10011SSS" }, M = 1, T = 4 },
                new OPCodeItem { Operation = "SBC A,n", OPCode = new[] { "11011110", "NNNNNNNN" }, M = 2, T = 7 },
                new OPCodeItem { Operation = "SBC A,(HL)", OPCode = new[] { "10011110" }, M = 2, T = 7 },
                new OPCodeItem { Operation = "SBC A,(IX+d)", OPCode = new[] { "11011101", "10011110", "IIIIIIII" }, M = 5, T = 19 },
                new OPCodeItem { Operation = "SBC A,(IY+d)", OPCode = new[] { "11111101", "10011110", "IIIIIIII" }, M = 5, T = 19 },
                new OPCodeItem { Operation = "DEC r", OPCode = new[] { "00DDD101" }, M = 1, T = 4 },
                new OPCodeItem { Operation = "DEC (HL)", OPCode = new[] { "00110101" }, M = 3, T = 11 },
                new OPCodeItem { Operation = "DEC (IX+d)", OPCode = new[] { "11011101", "00110101", "IIIIIIII" }, M = 6, T = 23 },
                new OPCodeItem { Operation = "DEC (IY+d)", OPCode = new[] { "11111101", "00110101", "IIIIIIII" }, M = 6, T = 23 },
                // 16ビットの加算
                new OPCodeItem { Operation = "ADD HL,rp", OPCode = new[] { "00RP1001" }, M = 3, T = 13 },
                new OPCodeItem { Operation = "ADC HL,rp", OPCode = new[] { "11101101", "01RP1010" }, M = 4, T = 15 },
                new OPCodeItem { Operation = "ADD IX,rp", OPCode = new[] { "11011101", "00RP1001" }, M = 4, T = 15 },
                new OPCodeItem { Operation = "ADD IY,rp", OPCode = new[] { "11111101", "00RP1001" }, M = 4, T = 15 },
                new OPCodeItem { Operation = "INC rp", OPCode = new[] { "00RP0011" }, M = 1, T = 6 },
                new OPCodeItem { Operation = "INC IX", OPCode = new[] { "11011101", "00100011" }, M = 2, T = 10 },
                new OPCodeItem { Operation = "INC IY", OPCode = new[] { "11111101", "00100011" }, M = 2, T = 10 },
                // 16ビットの減算
                new OPCodeItem { Operation = "SBC HL,rp", OPCode = new[] { "11101101", "01RP0010" }, M = 4, T = 15 },
                new OPCodeItem { Operation = "DEC rp", OPCode = new[] { "00RP1011" }, M = 1, T = 6 },
                new OPCodeItem { Operation = "DEC IX", OPCode = new[] { "11011101", "00101011" }, M = 2, T = 10 },
                new OPCodeItem { Operation = "DEC IY", OPCode = new[] { "11111101", "00101011" }, M = 2, T = 10 },
                // 論理演算
                new OPCodeItem { Operation = "AND r", OPCode = new[] { "10100DDD" }, M = 1, T = 4 },
                new OPCodeItem { Operation = "AND n", OPCode = new[] { "11100110", "NNNNNNNN" }, M = 2, T = 7 },
                new OPCodeItem { Operation = "AND (HL)", OPCode = new[] { "10100110" }, M = 2, T = 7 },
                new OPCodeItem { Operation = "AND (IX+d)", OPCode = new[] { "11011101", "10100110", "IIIIIIII" }, M = 5, T = 19 },
                new OPCodeItem { Operation = "AND (IY+d)", OPCode = new[] { "11111101", "10100110", "IIIIIIII" }, M = 5, T = 19 },
                new OPCodeItem { Operation = "OR r", OPCode = new[] { "10110DDD" }, M = 1, T = 4 },
                new OPCodeItem { Operation = "OR n", OPCode = new[] { "11110110", "NNNNNNNN" }, M = 2, T = 7 },
                new OPCodeItem { Operation = "OR (HL)", OPCode = new[] { "10110110" }, M = 2, T = 7 },
                new OPCodeItem { Operation = "OR (IX+d)", OPCode = new[] { "11011101", "10110110", "IIIIIIII" }, M = 5, T = 19 },
                new OPCodeItem { Operation = "OR (IY+d)", OPCode = new[] { "11111101", "10110110", "IIIIIIII" }, M = 5, T = 19 },
                new OPCodeItem { Operation = "XOR r", OPCode = new[] { "10101DDD" }, M = 1, T = 4 },
                new OPCodeItem { Operation = "XOR n", OPCode = new[] { "11101110", "NNNNNNNN" }, M = 2, T = 7 },
                new OPCodeItem { Operation = "XOR (HL)", OPCode = new[] { "10101110" }, M = 2, T = 7 },
                new OPCodeItem { Operation = "XOR (IX+d)", OPCode = new[] { "11011101", "10101110", "IIIIIIII" }, M = 5, T = 19 },
                new OPCodeItem { Operation = "XOR (IY+d)", OPCode = new[] { "11111101", "10101110", "IIIIIIII" }, M = 5, T = 19 },
                new OPCodeItem { Operation = "CPL", OPCode = new[] { "00101111" }, M = 1, T = 4 },
                new OPCodeItem { Operation = "NEG", OPCode = new[] { "11101101", "01000100" }, M = 2, T = 8 },
                // ビット操作
                new OPCodeItem { Operation = "CCF", OPCode = new[] { "00111111" }, M = 1, T = 4 },
                new OPCodeItem { Operation = "SCF", OPCode = new[] { "00110111" }, M = 1, T = 4 },
                new OPCodeItem { Operation = "BIT b,r", OPCode = new[] { "11001011", "01BBBSSS" }, M = 2, T = 8 },
                new OPCodeItem { Operation = "BIT b,(HL)", OPCode = new[] { "11001011", "01BBB110" }, M = 3, T = 13 },
                new OPCodeItem { Operation = "BIT b,(IX+d)", OPCode = new[] { "11011101", "11001011", "IIIIIIII", "01BBB110" }, M = 5, T = 20 },
                new OPCodeItem { Operation = "BIT b,(IY+d)", OPCode = new[] { "11111101", "11001011", "IIIIIIII", "01BBB110" }, M = 5, T = 20 },
                new OPCodeItem { Operation = "SET b,r", OPCode = new[] { "11001011", "11BBBSSS" }, M = 2, T = 8 },
                new OPCodeItem { Operation = "SET b,(HL)", OPCode = new[] { "11001011", "11BBB110" }, M = 4, T = 15 },
                new OPCodeItem { Operation = "SET b,(IX+d)", OPCode = new[] { "11011101", "11001011", "IIIIIIII", "11BBB110" }, M = 6, T = 23 },
                new OPCodeItem { Operation = "SET b,(IY+d)", OPCode = new[] { "11111101", "11001011", "IIIIIIII", "11BBB110" }, M = 6, T = 23 },
                new OPCodeItem { Operation = "RES b,r", OPCode = new[] { "11001011", "10BBBSSS" }, M = 2, T = 8 },
                new OPCodeItem { Operation = "RES b,(HL)", OPCode = new[] { "11001011", "10BBB110" }, M = 4, T = 15 },
                new OPCodeItem { Operation = "RES b,(IX+d)", OPCode = new[] { "11011101", "11001011", "IIIIIIII", "10BBB110" }, M = 6, T = 23 },
                new OPCodeItem { Operation = "RES b,(IY+d)", OPCode = new[] { "11111101", "11001011", "IIIIIIII", "10BBB110" }, M = 6, T = 23 },
                // サーチ
                new OPCodeItem { Operation = "CPI", OPCode = new[] { "11101101", "10100001" }, M = 4, T = 16 },
                new OPCodeItem { Operation = "CPIR", OPCode = new[] { "11101101", "10110001" }, M = 0, T = 0 },
                new OPCodeItem { Operation = "CPD", OPCode = new[] { "11101101", "10101001" }, M = 4, T = 16 },
                new OPCodeItem { Operation = "CPDR", OPCode = new[] { "11101101", "10111001" }, M = 0, T = 0 },
                // 比較
                new OPCodeItem { Operation = "CP r", OPCode = new[] { "10111DDD" }, M = 1, T = 4 },
                new OPCodeItem { Operation = "CP n", OPCode = new[] { "11111110", "NNNNNNNN" }, M = 2, T = 7 },
                new OPCodeItem { Operation = "CP (HL)", OPCode = new[] { "10111110" }, M = 2, T = 7 },
                new OPCodeItem { Operation = "CP (IX+d)", OPCode = new[] { "11011101", "10111110", "IIIIIIII" }, M = 2, T = 7 },
                new OPCodeItem { Operation = "CP (IY+d)", OPCode = new[] { "11111101", "10111110", "IIIIIIII" }, M = 2, T = 7 },
                // ジャンプ
                new OPCodeItem { Operation = "JP nn", OPCode = new[] { "11000011", "LLLLLLLL", "HHHHHHHH" }, M = 3, T = 10 },
                new OPCodeItem { Operation = "JP cc,nn", OPCode = new[] { "11CCC010", "LLLLLLLL", "HHHHHHHH" }, M = 3, T = 10 },
                new OPCodeItem { Operation = "JR e", OPCode = new[] { "00011000", "EEEEEEEE" }, M = 3, T = 12 },
                new OPCodeItem { Operation = "JR C,e", OPCode = new[] { "00111000", "EEEEEEEE" }, M = 3, T = 12 },
                new OPCodeItem { Operation = "JR NC,e", OPCode = new[] { "00110000", "EEEEEEEE" }, M = 3, T = 12 },
                new OPCodeItem { Operation = "JR Z,e", OPCode = new[] { "00101000", "EEEEEEEE" }, M = 3, T = 12 },
                new OPCodeItem { Operation = "JR NZ,e", OPCode = new[] { "00100000", "EEEEEEEE" }, M = 3, T = 12 },
                new OPCodeItem { Operation = "JP (HL)", OPCode = new[] { "11101001" }, M = 1, T = 4 },
                new OPCodeItem { Operation = "JP (IX)", OPCode = new[] { "11011101", "11101001" }, M = 2, T = 8 },
                new OPCodeItem { Operation = "JP (IY)", OPCode = new[] { "11111101", "11101001" }, M = 2, T = 8 },
                new OPCodeItem { Operation = "DJNZ e", OPCode = new[] { "00010000", "EEEEEEEE" }, M = 3, T = 13 },
                // コール・リターン
                new OPCodeItem { Operation = "CALL nn", OPCode = new[] { "11001101", "LLLLLLLL", "HHHHHHHH" }, M = 5, T = 17 },
                new OPCodeItem { Operation = "CALL cc,nn", OPCode = new[] { "11CCC100", "LLLLLLLL", "HHHHHHHH" }, M = 5, T = 17 },
                new OPCodeItem { Operation = "RET", OPCode = new[] { "11001001" }, M = 3, T = 10 },
                new OPCodeItem { Operation = "RET cc", OPCode = new[] { "11CCC000" }, M = 3, T = 11 },
                new OPCodeItem { Operation = "RETI", OPCode = new[] { "11101101", "01001101" }, M = 4, T = 15 },
                new OPCodeItem { Operation = "RETN", OPCode = new[] { "11101101", "01000101" }, M = 4, T = 14 },
                new OPCodeItem { Operation = "RST p", OPCode = new[] { "11TTT111" }, M = 3, T = 12 },
                // CPU 制御命令:動作・割り込み設定
                new OPCodeItem { Operation = "NOP", OPCode = new[] { "00000000" }, M = 1, T = 4 },
                new OPCodeItem { Operation = "HALT", OPCode = new[] { "01110110" }, M = 1, T = 4 },
                new OPCodeItem { Operation = "DI", OPCode = new[] { "11110011" }, M = 1, T = 4 },
                new OPCodeItem { Operation = "EI", OPCode = new[] { "11111011" }, M = 1, T = 4 },
                new OPCodeItem { Operation = "IM 0", OPCode = new[] { "11101101", "01000110" }, M = 2, T = 8 },
                new OPCodeItem { Operation = "IM 1", OPCode = new[] { "11101101", "01010110" }, M = 2, T = 8 },
                new OPCodeItem { Operation = "IM 2", OPCode = new[] { "11101101", "01011110" }, M = 2, T = 8 },
                // CPU 制御命令:動作・入力
                new OPCodeItem { Operation = "IN A,(n)", OPCode = new[] { "11011011", "NNNNNNNN" }, M = 3, T = 11 },
                new OPCodeItem { Operation = "IN r,(C)", OPCode = new[] { "11101101", "01DDD000" }, M = 3, T = 12 },
                new OPCodeItem { Operation = "INI", OPCode = new[] { "11101101", "10100010" }, M = 4, T = 16 },
                new OPCodeItem { Operation = "INIR", OPCode = new[] { "11101101", "10110010" }, M = 0, T = 0 },
                new OPCodeItem { Operation = "IND", OPCode = new[] { "11101101", "10101010" }, M = 4, T = 16 },
                new OPCodeItem { Operation = "INDR", OPCode = new[] { "11101101", "10111010" }, M = 0, T = 0 },
                // CPU 制御命令:動作・出力
                new OPCodeItem { Operation = "OUT (n),A", OPCode = new[] { "11010011", "NNNNNNNN" }, M = 3, T = 11 },
                new OPCodeItem { Operation = "OUT (C),r", OPCode = new[] { "11101101", "01SSS001" }, M = 3, T = 12 },
                new OPCodeItem { Operation = "OUTI", OPCode = new[] { "11101101", "10100011" }, M = 4, T = 16 },
                new OPCodeItem { Operation = "OTIR", OPCode = new[] { "11101101", "10110011" }, M = 0, T = 0 },
                new OPCodeItem { Operation = "OUTD", OPCode = new[] { "11101101", "10101011" }, M = 4, T = 16 },
                new OPCodeItem { Operation = "OTDR", OPCode = new[] { "11101101", "10111011" }, M = 0, T = 0 },
                // CPU 制御命令:二進化十進 (BCD) 用命令
                new OPCodeItem { Operation = "DAA", OPCode = new[] { "00100111" }, M = 1, T = 4 },
                new OPCodeItem { Operation = "RLD", OPCode = new[] { "11101101", "01101111" }, M = 5, T = 18 },
                new OPCodeItem { Operation = "RRD", OPCode = new[] { "11101101", "01100111" }, M = 5, T = 18 },
            };

        public static OPCodeResult GetOPCodeItem(string code)
        {
            var matched = Regex.Match(code, RegexPatternOP, RegexOptions.Singleline);

            var op1 = matched.Groups["op1"].Value.ToUpper();
            var op2 = matched.Groups["op2"].Value.ToUpper();
            var op3 = matched.Groups["op3"].Value.ToUpper();

            // 無効命令
            if ((op1 == "PUSH" && op2 == "SP" && op3 == "") ||
                (op1 == "POP" && op2 == "SP" && op3 == ""))
            {
                throw new ErrorMessageException(Error.ErrorCodeEnum.E0001, $"{code}");
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
                        var bbb = "";
                        var ccc = "";
                        var ttt = "";
                        var opCodeLabelList = new List<OPCodeLabel>();

                        if (!ProcessMark(tableOp1, op2, 0, tableOp2, ref dddd, ref ssss, ref rp, ref bbb, ref ccc, ref ttt, opCodeLabelList))
                            continue;

                        if (!ProcessMark(tableOp1, op3, 1, tableOp3, ref dddd, ref ssss, ref rp, ref bbb, ref ccc, ref ttt, opCodeLabelList))
                            continue;

                        var opcodes = opCodeItem.OPCode;
                        opcodes = opcodes.Select(m => m.Replace("DDD", dddd)
                                                        .Replace("SSS", ssss)
                                                        .Replace("RP", rp)
                                                        .Replace("CCC", ccc)
                                                        .Replace("BBB", bbb)
                                                        .Replace("TTT", ttt)).ToArray();

                        return new OPCodeResult(opcodes, opCodeItem.M, opCodeItem.T, opCodeLabelList.ToArray());
                    }
                }
            }
            return null;
        }

        private static bool ProcessMark (string op, string arg, int index, string tableOp, ref string dddd, ref string ssss, ref string rp, ref string bbb, ref string ccc, ref string ttt, IList<OPCodeLabel> opCodeLabelList)
        {
            switch (tableOp)
            {
                case "A":
                    if (!IsAccumulatorRegister(arg))
                        return false;
                    break;
                case "HL":
                    if (!IsHLRegister(arg))
                        return false;
                    break;
                case "SP":
                    if (!IsSPRegister(arg))
                        return false;
                    break;
                case "IX":
                    if (!IsIXRegister(arg))
                        return false;
                    break;
                case "IY":
                    if (!IsIYRegister(arg))
                        return false;
                    break;
                case "r":
                case "r1":
                case "r2":
                    if (!Is8BitRegister(arg))
                        return false;

                    if (index == 0)
                    {
                        dddd = GetDDDSSS(arg);
                    }
                    else
                    {
                        ssss = GetDDDSSS(arg);
                    }
                    break;
                case "rp":
                    if (!Is16BitRegister(arg))
                        return false;

                    rp = GetRP(arg);
                    break;
                case "cc":
                    if (!IsConditionSymbol(arg))
                        return false;

                    ccc = GetCCC(arg);
                    break;
                case "b":
                    if (!IsNumber3Bit(arg))
                        return false;

                    bbb = GetNumber3Bit(arg);
                    break;
                case "p":
                    if (!IsNumberRst(arg))
                        return false;

                    ttt = GetNumberRst(arg);
                    break;
                case "n":
                    if (!IsNumber8(arg))
                        return false;

                    opCodeLabelList.Add(new OPCodeLabel(OPCodeLabel.ValueTypeEnum.Value8, arg));
                    break;
                case "e":
                    if (!IsNumber8(arg) || IsConditionSymbol(arg))
                        return false;

                    opCodeLabelList.Add(new OPCodeLabel(OPCodeLabel.ValueTypeEnum.e8, arg));
                    break;
                case "nn":
                    if (!IsNumber16(arg))
                        return false;

                    if ((IsJPOpecode(op) || IsCALLOpecode(op)) && IsConditionSymbol(arg))
                        return false;

                    opCodeLabelList.Add(new OPCodeLabel(OPCodeLabel.ValueTypeEnum.Value16, arg));
                    break;
                case "(nn)":
                    if (!IsAddrNumber16(arg))
                        return false;

                    {
                        var matchedAddr = Regex.Match(arg, RegexPatternAddress);
                        var value = matchedAddr.Groups["addr"].Value;
                        opCodeLabelList.Add(new OPCodeLabel(OPCodeLabel.ValueTypeEnum.Value16, value));
                    }
                    break;
                case "(HL)":
                    if (!IsAddrHLRegister(arg))
                        return false;

                    break;
                case "(BC)":
                    if (!IsAddrBCRegister(arg))
                        return false;

                    break;
                case "(DE)":
                    if (!IsAddrDERegister(arg))
                        return false;

                    break;
                case "(IX+d)":
                    if (!IsAddrIXPlusDRegister(arg))
                        return false;

                    {
                        var matchedIndex = Regex.Match(arg, RegexPatternIXReg);
                        var value = matchedIndex.Groups["value"].Value;
                        opCodeLabelList.Add(new OPCodeLabel(OPCodeLabel.ValueTypeEnum.IndexOffset, value));
                    }
                    break;
                case "(IY+d)":
                    if (!IsAddrIYPlusDRegister(arg))
                        return false;
                    {
                        var matchedIndex = Regex.Match(arg, RegexPatternIYReg);
                        var value = matchedIndex.Groups["value"].Value;
                        opCodeLabelList.Add(new OPCodeLabel(OPCodeLabel.ValueTypeEnum.IndexOffset, value));
                    }
                    break;
                case "(n)":
                    if (!IsPortNumber(arg))
                        return false;

                    {
                        var matchedAddr = Regex.Match(arg, RegexPatternAddress);
                        var value = matchedAddr.Groups["addr"].Value;
                        opCodeLabelList.Add(new OPCodeLabel(OPCodeLabel.ValueTypeEnum.Value8, value));
                    }

                    break;
                case "(C)":
                    return arg == tableOp;
                case "":
                    return arg == tableOp;
                default:
                    return arg == tableOp;
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

        private static bool IsNumber8(string source)
        {
            if (IsAllRegister(source))
                return false;

            // ()で囲まれた値は外す
            if (Regex.Match(source, @"^\(.+\)$").Success)
                return false;

            return true;
        }

        private static bool IsPortNumber(string source)
        {
            return Regex.IsMatch(source, RegexPatternAddress) && source != "(C)";
        }

        private static bool IsNumber3Bit(string source)
        {
            return IsNumber8(source);
        }

        private static string GetNumber3Bit(string source)
        {
            if (source.IndexOf("$") == 0)
            {
                return Convert.ToString(Convert.ToByte(source.Replace("$", ""), 16), 2).PadLeft(3, '0');
            }
            else
            {
                return Convert.ToString(Convert.ToByte(source), 2).PadLeft(3, '0');
            }
        }

        private static bool IsNumberRst(string source)
        {
            switch (source)
            {
                case "00H":
                case "08H":
                case "10H":
                case "18H":
                case "20H":
                case "28H":
                case "30H":
                case "38H":
                    return true;
                default:
                    return false;
            }
        }

        private static string GetNumberRst(string source)
        {
            switch (source)
            {
                case "00H":
                    return "000";
                case "08H":
                    return "001";
                case "10H":
                    return "010";
                case "18H":
                    return "011";
                case "20H":
                    return "100";
                case "28H":
                    return "101";
                case "30H":
                    return "110";
                case "38H":
                    return "111";
                default:
                    return "";
            }
        }

        private static bool IsAllRegister(string source)
        {
            return Is8BitRegister(source) ||
                   Is8BitIndexRegister(source) ||
                   Is16BitRegister(source) ||
                   Is16BitIndexRegister(source) ||
                   IsAddrIndexRegister(source) ||
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

        private static bool IsIXRegister(string source)
        {
            return source == "IX";
        }

        private static bool IsIYRegister(string source)
        {
            return source == "IY";
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

        private static bool Is16BitIndexRegister(string source)
        {
            return Regex.IsMatch(source, @"^(IX|IY)$");
        }

        private static bool IsAddrIXPlusDRegister(string source)
        {
            return Regex.IsMatch(source, RegexPatternIXReg);
        }

        private static bool IsAddrIYPlusDRegister(string source)
        {
            return Regex.IsMatch(source, RegexPatternIYReg);
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

        private static bool IsAddrIXRegister(string source)
        {
            return source == "(IX)";
        }

        private static bool IsAddrIYRegister(string source)
        {
            return source == "(IY)";
        }

        private static bool IsAddrRegister(string source)
        {
            return Regex.IsMatch(source, @"^(\(BC\)|\(DE\)|\(HL\)|\(SP\))$");
        }

        private static bool IsAddrIndexRegister(string source)
        {
            return IsAddrIXPlusDRegister(source) || IsAddrIYPlusDRegister(source);
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

        private static bool IsConditionSymbol(string source)
        {
            return Regex.IsMatch(source, @"^(NZ|Z|NC|C|PO|PE|P|M)$");
        }

        private static bool IsJPOpecode(string source)
        {
            return source == "JP";
        }

        private static bool IsCALLOpecode(string source)
        {
            return source == "CALL";
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

        private static string GetCCC(string source)
        {
            switch (source)
            {
                case "NZ":
                    return "000";
                case "Z":
                    return "001";
                case "NC":
                    return "010";
                case "C":
                    return "011";
                case "PO":
                    return "100";
                case "PE":
                    return "101";
                case "P":
                    return "110";
                case "M":
                    return "111";
                default:
                    throw new Exception($"レジスタの指定が間違っています。{source}");
            }
        }
    }
}
