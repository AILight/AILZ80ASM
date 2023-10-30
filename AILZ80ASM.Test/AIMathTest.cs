using AILZ80ASM.AILight;
using AILZ80ASM.Assembler;
using AILZ80ASM.Exceptions;
using AILZ80ASM.LineDetailItems;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace AILZ80ASM.Test
{
    [TestClass]
    public class AIMathTest
    {
        [TestMethod]
        public void BoolType_Test()
        {
            Assert.IsTrue(AIMath.Calculation("#TRUE").ConvertTo<bool>());
            Assert.IsFalse(AIMath.Calculation("#FALSE").ConvertTo<bool>());
        }

        [TestMethod]
        public void Calc_01()
        {
            Assert.AreEqual(1 + 2 * ((2 + 1)) + 6 / 2, AIMath.Calculation("1+2*((2+1))+6/2").ConvertTo<UInt16>());
            Assert.AreEqual((UInt16)((-2 + 1) & 0xFFFF), AIMath.Calculation("-2 + 1").ConvertTo<UInt16>());
            Assert.AreEqual(1 + 2 - 3, AIMath.Calculation("1 + 2 - 3").ConvertTo<UInt16>());
            Assert.AreEqual(1 + 2 * 3, AIMath.Calculation("1 + 2 * 3").ConvertTo<UInt16>());
            Assert.AreEqual(1 * 2 + 3, AIMath.Calculation("1 * 2 + 3").ConvertTo<UInt16>());
            Assert.AreEqual(-1 + 1, AIMath.Calculation("-1 + 1").ConvertTo<UInt16>());
            Assert.AreEqual(+1 + 1, AIMath.Calculation("+1 + 1").ConvertTo<UInt16>());
            Assert.AreEqual(+1 + -1, AIMath.Calculation("+1 + -1").ConvertTo<UInt16>());
            Assert.AreEqual(6 % 1, AIMath.Calculation("6%1").ConvertTo<UInt16>());
            Assert.AreEqual(6 % 5, AIMath.Calculation("6%5").ConvertTo<UInt16>());
            Assert.AreEqual(6 % 6, AIMath.Calculation("6%6").ConvertTo<UInt16>());
            Assert.AreEqual(5 << 1, AIMath.Calculation("5<<1").ConvertTo<UInt16>());
            Assert.AreEqual(5 << 5, AIMath.Calculation("5<<5").ConvertTo<UInt16>());
            Assert.AreEqual(5 >> 1, AIMath.Calculation("5>>1").ConvertTo<UInt16>());
            Assert.AreEqual(5 >> 5, AIMath.Calculation("5>>5").ConvertTo<UInt16>());
            Assert.AreEqual(-1 + 1, AIMath.Calculation("-1 + 1").ConvertTo<UInt16>());
            Assert.AreEqual(10 + 20 % 5, AIMath.Calculation("10 + 20 % 5").ConvertTo<UInt16>());
            Assert.AreEqual(10 + 20 / 5, AIMath.Calculation("10 + 20 / 5").ConvertTo<UInt16>());
            Assert.AreEqual(0x01 & 0xAA, AIMath.Calculation("0x01 & 0xAA").ConvertTo<UInt16>());
            Assert.AreEqual(0x02 & 0xAA, AIMath.Calculation("0x02 & 0xAA").ConvertTo<UInt16>());
            Assert.AreEqual(1 + 0x02 & 0xAA + 2, AIMath.Calculation("1 + 0x02 & 0xAA + 2").ConvertTo<UInt16>());
            Assert.AreEqual(1 + 0x02 ^ 0xAA + 2, AIMath.Calculation("1 + 0x02 ^ 0xAA + 2").ConvertTo<UInt16>());
            Assert.AreEqual(1 + 0x03 | 0xAA + 2, AIMath.Calculation("1 + 0x03 | 0xAA + 2").ConvertTo<UInt16>());
            Assert.AreEqual(2 == 2 && 1 == 1, AIMath.Calculation("2 == 2 && 1 == 1").ConvertTo<bool>());
            Assert.AreEqual(2 == 2 && 2 == 1, AIMath.Calculation("2 == 2 && 2 == 1").ConvertTo<bool>());
            Assert.AreEqual(2 == 2 || 1 == 1, AIMath.Calculation("2 == 2 || 1 == 1").ConvertTo<bool>());
            Assert.AreEqual(2 == 2 || 2 == 1, AIMath.Calculation("2 == 2 || 2 == 1").ConvertTo<bool>());
            Assert.AreEqual(2 == 1 || 2 == 1, AIMath.Calculation("2 == 1 || 2 == 1").ConvertTo<bool>());
            Assert.AreEqual(1 == 1 ? 1 + 1 : 2 + 2, AIMath.Calculation("1 == 1 ? 1 + 1 : 2 + 2").ConvertTo<UInt16>());
            Assert.AreEqual(1 == 2 ? 1 + 1 : 2 + 2, AIMath.Calculation("1 == 2 ? 1 + 1 : 2 + 2").ConvertTo<UInt16>());
            Assert.AreEqual(0x55AA, AIMath.Calculation("!0xAA55").ConvertTo<UInt16>());
            Assert.AreEqual(0x55AA, AIMath.Calculation("~0xAA55").ConvertTo<UInt16>());
        }

        [TestMethod]
        public void Calc_02()
        {
            Assert.AreEqual(2 == 2 ? (1 + 1) * 2 : (2 + 2) * 3, AIMath.Calculation("2 == 2 ? (1 + 1) * 2: (2 + 2) * 3").ConvertTo<UInt16>());
            Assert.AreEqual(2 == 3 ? (1 + 1) * 2 : (2 + 2) * 3, AIMath.Calculation("2 == 3 ? (1 + 1) * 2: (2 + 2) * 3").ConvertTo<UInt16>());
            Assert.AreEqual(1 + 2 == 2 ? (1 + 1) * 2 : (2 + 2) * 3, AIMath.Calculation("1 + 2 == 2 ? (1 + 1) * 2: (2 + 2) * 3").ConvertTo<UInt16>());
            Assert.AreEqual(1 + 2 == 3 ? (1 + 1) * 2 : (2 + 2) * 3, AIMath.Calculation("1 + 2 == 3 ? (1 + 1) * 2: (2 + 2) * 3").ConvertTo<UInt16>());
            Assert.AreEqual(1 + (2 == 2 ? (1 + 1) * 2 : (2 + 2) * 3), AIMath.Calculation("1 + (2 == 2 ? (1 + 1) * 2: (2 + 2) * 3)").ConvertTo<UInt16>());
            Assert.AreEqual(1 + (2 == 3 ? (1 + 1) * 2 : (2 + 2) * 3), AIMath.Calculation("1 + (2 == 3 ? (1 + 1) * 2: (2 + 2) * 3)").ConvertTo<UInt16>());
        }

        [TestMethod]
        public void Calc_03()
        {
            Assert.AreEqual(2 == 2, AIMath.Calculation("2 == 2").ConvertTo<bool>());
            Assert.AreEqual(2 == 3, AIMath.Calculation("2 == 3").ConvertTo<bool>());
            Assert.AreEqual(2 != 2, AIMath.Calculation("2 != 2").ConvertTo<bool>());
            Assert.AreEqual(2 != 3, AIMath.Calculation("2 != 3").ConvertTo<bool>());
            Assert.AreEqual(2 >= 2, AIMath.Calculation("2 >= 2").ConvertTo<bool>());
            Assert.AreEqual(2 >= 3, AIMath.Calculation("2 >= 3").ConvertTo<bool>());
            Assert.AreEqual(1 >= 2, AIMath.Calculation("1 >= 2").ConvertTo<bool>());
            Assert.AreEqual(2 <= 2, AIMath.Calculation("2 <= 2").ConvertTo<bool>());
            Assert.AreEqual(2 <= 3, AIMath.Calculation("2 <= 3").ConvertTo<bool>());
            Assert.AreEqual(1 <= 2, AIMath.Calculation("1 <= 2").ConvertTo<bool>());
            Assert.AreEqual(2 > 2,  AIMath.Calculation("2 >  2").ConvertTo<bool>());
            Assert.AreEqual(2 > 3,  AIMath.Calculation("2 >  3").ConvertTo<bool>());
            Assert.AreEqual(1 > 2,  AIMath.Calculation("1 >  2").ConvertTo<bool>());
            Assert.AreEqual(2 < 2,  AIMath.Calculation("2 <  2").ConvertTo<bool>());
            Assert.AreEqual(2 < 3,  AIMath.Calculation("2 <  3").ConvertTo<bool>());
            Assert.AreEqual(1 < 2,  AIMath.Calculation("1 <  2").ConvertTo<bool>());
        }

        [TestMethod]
        public void Calc_04()
        {
            Assert.AreEqual(8 & 1, AIMath.Calculation("8&1").ConvertTo<UInt16>());
            Assert.AreEqual(8 & 2, AIMath.Calculation("8&2").ConvertTo<UInt16>());
            Assert.AreEqual(1 | 6, AIMath.Calculation("1|6").ConvertTo<UInt16>());
            Assert.AreEqual(8 | 6, AIMath.Calculation("8|6").ConvertTo<UInt16>());
            Assert.AreEqual(8 ^ 1, AIMath.Calculation("8^1").ConvertTo<UInt16>());
            Assert.AreEqual(8 ^ 2, AIMath.Calculation("8^2").ConvertTo<UInt16>());
        }

        [TestMethod]
        public void Calc_05()
        {
            Assert.AreEqual(1 == 1 || 2 == 2, AIMath.Calculation("1 == 1 || 2 == 2").ConvertTo<bool>());
            Assert.AreEqual(1 != 1 || 2 != 2, AIMath.Calculation("1 != 1 || 2 != 2").ConvertTo<bool>());
            Assert.AreEqual(1 == 1 || 2 != 2, AIMath.Calculation("1 == 1 || 2 != 2").ConvertTo<bool>());
            Assert.AreEqual(1 != 1 || 2 == 2, AIMath.Calculation("1 != 1 || 2 == 2").ConvertTo<bool>());

            Assert.AreEqual(1 == 1 && 2 == 2, AIMath.Calculation("1 == 1 && 2 == 2").ConvertTo<bool>());
            Assert.AreEqual(1 != 1 && 2 != 2, AIMath.Calculation("1 != 1 && 2 != 2").ConvertTo<bool>());
            Assert.AreEqual(1 == 1 && 2 != 2, AIMath.Calculation("1 == 1 && 2 != 2").ConvertTo<bool>());
            Assert.AreEqual(1 != 1 && 2 == 2, AIMath.Calculation("1 != 1 && 2 == 2").ConvertTo<bool>());
        }

        [TestMethod]
        public void Calc_06()
        {
            Assert.AreEqual(UInt16.MaxValue /*~0*/, AIMath.Calculation("~0").ConvertTo<UInt16>());
            Assert.AreEqual(default(UInt16) /*~65535 */, AIMath.Calculation("~65535").ConvertTo<UInt16>());
            Assert.AreEqual(UInt16.MaxValue /*~0*/, AIMath.Calculation("!0").ConvertTo<UInt16>());
            Assert.AreEqual(default(UInt16) /*~65535 */, AIMath.Calculation("!65535").ConvertTo<UInt16>());
        }

        [TestMethod]
        public void Calc_07()
        {
            Assert.AreEqual(0xFFFF, AIMath.Calculation("FFFFH").ConvertTo<UInt16>());
            Assert.AreEqual(0xFFFF, AIMath.Calculation("0xFFFF").ConvertTo<UInt16>());
            Assert.AreEqual(126, AIMath.Calculation("176O").ConvertTo<UInt16>());
            Assert.AreEqual(0x5A, AIMath.Calculation("0101_1010b").ConvertTo<UInt16>());
            Assert.AreEqual(0x5A, AIMath.Calculation("%0101_1010").ConvertTo<UInt16>());
        }

        [TestMethod]
        public void Calc_08()
        {
            var asmAddress = new AsmAddress();
            asmAddress.Program = 0x8000;
            asmAddress.Output = 0x8000;

            var asmLoad = new AsmLoad(new AsmOption(), new InstructionSet.Z80());
            asmLoad.AddLabel(new LabelAdr("[NAME_SPACE_DEFAULT]", asmLoad));
            var label = new LabelAdr("LB", "0xAA01", asmLoad);

            asmLoad.AddLabel(label);

            Assert.AreEqual(0xAA02, AIMath.Calculation("LB + 1", asmLoad).ConvertTo<UInt16>());
            Assert.AreEqual(0x8001, AIMath.Calculation("$ + 1", asmLoad, asmAddress).ConvertTo<UInt16>());
        }

        [TestMethod]
        public void Calc_09()
        {
            var asmLoad = new AsmLoad(new AsmOption(), new InstructionSet.Z80());
            
            Assert.AreEqual((byte)'0', AIMath.Calculation("'0'", asmLoad).ConvertTo<byte>());
            Assert.AreEqual(256 + ((byte)' ' - (byte)'0'), AIMath.Calculation("' ' - '0'", asmLoad).ConvertTo<byte>());
            Assert.AreEqual((byte)':', AIMath.Calculation("':'", asmLoad).ConvertTo<byte>());
            Assert.AreEqual((byte)'\'', AIMath.Calculation("'\\''", asmLoad).ConvertTo<byte>());
            Assert.AreEqual((byte)0, AIMath.Calculation("'\\0'", asmLoad).ConvertTo<byte>());
            Assert.AreEqual((byte)'\\', AIMath.Calculation("'\\\\'", asmLoad).ConvertTo<byte>());

            foreach (var item in System.Linq.Enumerable.Range(0x20, 95))
            {
                if (item == 0x27 || item == 0x5c)
                {
                    continue;
                }

                var character = (char)(byte)item;

                Assert.AreEqual((byte)item, AIMath.Calculation($"'{character}'", asmLoad).ConvertTo<byte>());
            }
        }

        [TestMethod]
        public void Calc_10()
        {
            var asmLoad = new AsmLoad(new AsmOption(), new InstructionSet.Z80());

            foreach (var item in System.Linq.Enumerable.Range(0x20, 95))
            {
                var escape = "";
                if (item == 0x22 || item == 0x27 || item == 0x5C)
                {
                    escape = "\\";
                }

                var character = (char)(byte)item;
                Assert.AreEqual((byte)(item + 0x80), AIMath.Calculation($"'{escape}{character}'+80H", asmLoad).ConvertTo<byte>(), $"{character}:{item}");
            }
        }

        [TestMethod]
        public void Calc_11()
        {
            Assert.AreEqual(0xEE, AIMath.Calculation("low $FFEE").ConvertTo<byte>());
            Assert.AreEqual(0xFF, AIMath.Calculation("high $FFEE").ConvertTo<byte>());
            Assert.AreEqual(0xEF, AIMath.Calculation("low $FFEE + 1").ConvertTo<byte>());
            Assert.AreEqual(0xFE, AIMath.Calculation("high $FFEE - 1").ConvertTo<byte>());

            Assert.AreEqual(0x11, AIMath.Calculation("low !$FFEE").ConvertTo<byte>());
            Assert.AreEqual(0x00, AIMath.Calculation("high !$FFEE").ConvertTo<byte>());

            Assert.AreEqual(0xFF, AIMath.Calculation("low +65535").ConvertTo<byte>());
            Assert.AreEqual(0xFF, AIMath.Calculation("high +65535").ConvertTo<byte>());

            Assert.AreEqual(0xEE, AIMath.Calculation("LOW $FFEE").ConvertTo<byte>());
            Assert.AreEqual(0xFF, AIMath.Calculation("HIGH $FFEE").ConvertTo<byte>());
            Assert.AreEqual(0xEF, AIMath.Calculation("LOW $FFEE + 1").ConvertTo<byte>());
            Assert.AreEqual(0xFE, AIMath.Calculation("HIGH $FFEE - 1").ConvertTo<byte>());

            Assert.AreEqual(0x11, AIMath.Calculation("LOW !$FFEE").ConvertTo<byte>());
            Assert.AreEqual(0x00, AIMath.Calculation("HIGH !$FFEE").ConvertTo<byte>());

            Assert.AreEqual(0xFF, AIMath.Calculation("LOW +65535").ConvertTo<byte>());
            Assert.AreEqual(0xFF, AIMath.Calculation("HIGH +65535").ConvertTo<byte>());
        }

        [TestMethod]
        public void Calc_12()
        {
            var asmLoad = new AsmLoad(new AsmOption(), new InstructionSet.Z80());
            asmLoad.AddLabel(new LabelAdr("[NAME_SPACE_DEFAULT]", asmLoad));
            var label = new LabelAdr("LB01", "0xAA01", asmLoad);

            asmLoad.AddLabel(label);

            Assert.IsTrue(AIMath.Calculation("exists LB01", asmLoad).ConvertTo<bool>());
            Assert.IsFalse(AIMath.Calculation("exists LB02", asmLoad).ConvertTo<bool>());
            
            Assert.IsTrue(AIMath.Calculation("EXISTS LB01", asmLoad).ConvertTo<bool>());
            Assert.IsFalse(AIMath.Calculation("EXISTS LB02", asmLoad).ConvertTo<bool>());
        }
        
        [TestMethod]
        public void Calc_13()
        {
            Assert.AreEqual((2 - 1) + 4, AIMath.Calculation("(2 - 1) + 4").ConvertTo<UInt16>());
            Assert.AreEqual(2 - 1 + 1, AIMath.Calculation("2 - 1 + 1").ConvertTo<UInt16>());
            Assert.AreEqual((1) * 3, AIMath.Calculation("(1) * 3").ConvertTo<UInt16>());
            Assert.AreEqual((1) / 4, AIMath.Calculation("(1) / 4").ConvertTo<UInt16>());

            Assert.AreEqual((1) + 3, AIMath.Calculation("(1) + 3").ConvertTo<UInt16>());
            Assert.AreEqual((2 - 1) + 4, AIMath.Calculation("(2 - 1) + 4").ConvertTo<UInt16>());
            Assert.AreEqual((1) - 5, AIMath.Calculation("(1) - 5").ConvertTo<int>());
        }

        [TestMethod]
        public void Calc_UInt32()
        {
            Assert.AreEqual((UInt32)(1 + 2 * ((2 + 1)) + 6 / 2), AIMath.Calculation("1+2*((2+1))+6/2").ConvertTo<UInt32>());
            Assert.AreEqual((UInt32)((-2 + 1) & 0xFFFFFFFF), AIMath.Calculation("-2 + 1").ConvertTo<UInt32>());
            Assert.AreEqual((UInt32)(-1 + 1),  AIMath.Calculation("-1 + 1").ConvertTo<UInt32>());
            Assert.AreEqual((UInt32)(+1 + 1),  AIMath.Calculation("+1 + 1").ConvertTo<UInt32>());
            Assert.AreEqual((UInt32)(+1 + -1), AIMath.Calculation("+1 + -1").ConvertTo<UInt32>());
            Assert.AreEqual((UInt32)(6 % 1),   AIMath.Calculation("6%1").ConvertTo<UInt32>());
            Assert.AreEqual((UInt32)(6 % 5),   AIMath.Calculation("6%5").ConvertTo<UInt32>());
            Assert.AreEqual((UInt32)(6 % 6),   AIMath.Calculation("6%6").ConvertTo<UInt32>());
            Assert.AreEqual((UInt32)(5 << 1),  AIMath.Calculation("5<<1").ConvertTo<UInt32>());
            Assert.AreEqual((UInt32)(5 << 5),  AIMath.Calculation("5<<5").ConvertTo<UInt32>());
            Assert.AreEqual((UInt32)(5 >> 1),  AIMath.Calculation("5>>1").ConvertTo<UInt32>());
            Assert.AreEqual((UInt32)(5 >> 5),  AIMath.Calculation("5>>5").ConvertTo<UInt32>());
        }

        [TestMethod]
        public void Calc_Label()
        {
            var asmAddress = new AsmAddress();
            asmAddress.Program = 0x8000;
            asmAddress.Output = 0x8100;

            var asmLoad = new AsmLoad(new AsmOption(), new InstructionSet.Z80());

            asmLoad.AddLabel(new LabelAdr("[NAME_SPACE_DEFAULT]", "", asmLoad));
            {
                var label = new LabelAdr("LB", "0xAA02", asmLoad);
                asmLoad.AddLabel(label);
            }
            {
                var label = new LabelAdr(".LLB", "0xBB03", asmLoad);
                asmLoad.AddLabel(label);
            }
            {
                var label = new LabelAdr(".@0", "0xCC04", asmLoad);
                asmLoad.AddLabel(label);
            }

            {
                var label = new LabelAdr("LB2", "LABEL", asmLoad);
                asmLoad.AddLabel(label);
            }
            Assert.AreEqual(0xFF, AIMath.Calculation("-1", asmLoad).ConvertTo<byte>());
            Assert.AreEqual(1, AIMath.Calculation("-1 * -1", asmLoad).ConvertTo<byte>());
            Assert.AreEqual(0x55FE, AIMath.Calculation("-LB", asmLoad).ConvertTo<UInt16>());

            Assert.AreEqual(0xAA, AIMath.Calculation("LB.@H", asmLoad).ConvertTo<byte>());
            Assert.AreEqual(0xAA, AIMath.Calculation("LB.@HIGH", asmLoad).ConvertTo<byte>());
            Assert.AreEqual(0x00, AIMath.Calculation("LB.@H.@H", asmLoad).ConvertTo<byte>());
            Assert.AreEqual(0x02, AIMath.Calculation("LB.@L", asmLoad).ConvertTo<byte>());
            Assert.AreEqual(0x02, AIMath.Calculation("LB.@LOW", asmLoad).ConvertTo<byte>());
            Assert.AreEqual(0x02, AIMath.Calculation("LB.@L.@L", asmLoad).ConvertTo<byte>());

            Assert.AreEqual(0xAA, AIMath.Calculation("LB.@h", asmLoad).ConvertTo<byte>());
            Assert.AreEqual(0xAA, AIMath.Calculation("LB.@high", asmLoad).ConvertTo<byte>());
            Assert.AreEqual(0x00, AIMath.Calculation("LB.@h.@h", asmLoad).ConvertTo<byte>());
            Assert.AreEqual(0x02, AIMath.Calculation("LB.@l", asmLoad).ConvertTo<byte>());
            Assert.AreEqual(0x02, AIMath.Calculation("LB.@low", asmLoad).ConvertTo<byte>());
            Assert.AreEqual(0x02, AIMath.Calculation("LB.@l.@l", asmLoad).ConvertTo<byte>());

            Assert.AreEqual(0xBB, AIMath.Calculation("LB.LLB.@H", asmLoad).ConvertTo<byte>());
            Assert.AreEqual(0xBB, AIMath.Calculation("LB.LLB.@HIGH", asmLoad).ConvertTo<byte>());
            Assert.AreEqual(0x00, AIMath.Calculation("LB.LLB.@H.@H", asmLoad).ConvertTo<byte>());
            Assert.AreEqual(0x03, AIMath.Calculation("LB.LLB.@L", asmLoad).ConvertTo<byte>());
            Assert.AreEqual(0x03, AIMath.Calculation("LB.LLB.@LOW", asmLoad).ConvertTo<byte>());
            Assert.AreEqual(0x03, AIMath.Calculation("LB.LLB.@L.@L", asmLoad).ConvertTo<byte>());

            Assert.AreEqual(0xBB, AIMath.Calculation("LB.LLB.@h", asmLoad).ConvertTo<byte>());
            Assert.AreEqual(0xBB, AIMath.Calculation("LB.LLB.@high", asmLoad).ConvertTo<byte>());
            Assert.AreEqual(0x00, AIMath.Calculation("LB.LLB.@h.@h", asmLoad).ConvertTo<byte>());
            Assert.AreEqual(0x03, AIMath.Calculation("LB.LLB.@l", asmLoad).ConvertTo<byte>());
            Assert.AreEqual(0x03, AIMath.Calculation("LB.LLB.@low", asmLoad).ConvertTo<byte>());
            Assert.AreEqual(0x03, AIMath.Calculation("LB.LLB.@l.@l", asmLoad).ConvertTo<byte>());

            Assert.AreEqual(0xCC, AIMath.Calculation("LB.LLB.@0.@H", asmLoad).ConvertTo<byte>());
            Assert.AreEqual(0xCC, AIMath.Calculation("LB.LLB.@0.@HIGH", asmLoad).ConvertTo<byte>());
            Assert.AreEqual(0x00, AIMath.Calculation("LB.LLB.@0.@H.@H", asmLoad).ConvertTo<byte>());
            Assert.AreEqual(0x04, AIMath.Calculation("LB.LLB.@0.@L", asmLoad).ConvertTo<byte>());
            Assert.AreEqual(0x04, AIMath.Calculation("LB.LLB.@0.@LOW", asmLoad).ConvertTo<byte>());
            Assert.AreEqual(0x04, AIMath.Calculation("LB.LLB.@0.@L.@L", asmLoad).ConvertTo<byte>());

            Assert.AreEqual(0xCC, AIMath.Calculation("LB.LLB.@0.@h", asmLoad).ConvertTo<byte>());
            Assert.AreEqual(0xCC, AIMath.Calculation("LB.LLB.@0.@high", asmLoad).ConvertTo<byte>());
            Assert.AreEqual(0x00, AIMath.Calculation("LB.LLB.@0.@h.@h", asmLoad).ConvertTo<byte>());
            Assert.AreEqual(0x04, AIMath.Calculation("LB.LLB.@0.@l", asmLoad).ConvertTo<byte>());
            Assert.AreEqual(0x04, AIMath.Calculation("LB.LLB.@0.@low", asmLoad).ConvertTo<byte>());
            Assert.AreEqual(0x04, AIMath.Calculation("LB.LLB.@0.@l.@l", asmLoad).ConvertTo<byte>());

            Assert.IsTrue(AIMath.Calculation("LB.@T == \"0xAA02\"", asmLoad).ConvertTo<bool>());
            Assert.IsTrue(AIMath.Calculation("LB.@TEXT == \"0xAA02\"", asmLoad).ConvertTo<bool>());
            Assert.IsTrue(AIMath.Calculation("TEXT LB == \"0xAA02\"", asmLoad).ConvertTo<bool>());
            Assert.IsTrue(AIMath.Calculation("LB2.@T == \"LABEL\"", asmLoad).ConvertTo<bool>());
            Assert.IsTrue(AIMath.Calculation("LB2.@TEXT == \"LABEL\"", asmLoad).ConvertTo<bool>());
            Assert.IsTrue(AIMath.Calculation("TEXT LB2 == \"LABEL\"", asmLoad).ConvertTo<bool>());
            Assert.IsFalse(AIMath.Calculation("LB2.@T == \"LABEL1\"", asmLoad).ConvertTo<bool>());

            Assert.IsTrue(AIMath.Calculation("LB.@t == \"0xAA02\"", asmLoad).ConvertTo<bool>());
            Assert.IsTrue(AIMath.Calculation("LB.@text == \"0xAA02\"", asmLoad).ConvertTo<bool>());
            Assert.IsTrue(AIMath.Calculation("text LB == \"0xAA02\"", asmLoad).ConvertTo<bool>());
            Assert.IsTrue(AIMath.Calculation("LB2.@t == \"LABEL\"", asmLoad).ConvertTo<bool>());
            Assert.IsTrue(AIMath.Calculation("LB2.@text == \"LABEL\"", asmLoad).ConvertTo<bool>());
            Assert.IsTrue(AIMath.Calculation("text LB2 == \"LABEL\"", asmLoad).ConvertTo<bool>());
            Assert.IsFalse(AIMath.Calculation("LB2.@t == \"LABEL1\"", asmLoad).ConvertTo<bool>());

            Assert.AreEqual(0x8000, AIMath.Calculation("$", asmLoad, asmAddress).ConvertTo<UInt16>());
            Assert.AreEqual(0x8100, AIMath.Calculation("$$", asmLoad, asmAddress).ConvertTo<UInt16>());
        }

        [TestMethod]
        public void Calc_Exception()
        {
            var asmLoad = new AsmLoad(new AsmOption(), new InstructionSet.Z80());

            Assert.ThrowsException<ErrorAssembleException>(() => { AIMath.Calculation("", asmLoad).ConvertTo<UInt16>(); });
            Assert.ThrowsException<ErrorAssembleException>(() => { AIMath.Calculation("1**1", asmLoad).ConvertTo<UInt16>(); });
            Assert.ThrowsException<ErrorAssembleException>(() => { AIMath.Calculation("1++", asmLoad).ConvertTo<UInt16>(); });
            Assert.ThrowsException<ErrorAssembleException>(() => { AIMath.Calculation("1@1", asmLoad).ConvertTo<UInt16>(); });
            Assert.ThrowsException<ErrorAssembleException>(() => { AIMath.Calculation("(1+)", asmLoad).ConvertTo<UInt16>(); });
            Assert.ThrowsException<ErrorAssembleException>(() => { AIMath.Calculation("(1)*(", asmLoad).ConvertTo<UInt16>(); });
            Assert.ThrowsException<ErrorAssembleException>(() => { AIMath.Calculation("*3+2", asmLoad).ConvertTo<UInt16>(); });
            Assert.ThrowsException<ErrorAssembleException>(() => { AIMath.Calculation("ABC+2", asmLoad).ConvertTo<UInt16>(); });
            Assert.ThrowsException<ErrorAssembleException>(() => { AIMath.Calculation("0xG0", asmLoad).ConvertTo<UInt16>(); });
            Assert.ThrowsException<ErrorAssembleException>(() => { AIMath.Calculation("$G0", asmLoad).ConvertTo<UInt16>(); });
            Assert.ThrowsException<ErrorAssembleException>(() => { AIMath.Calculation("0000222b", asmLoad).ConvertTo<UInt16>(); });
            Assert.ThrowsException<ErrorAssembleException>(() => { AIMath.Calculation("\"0", asmLoad).ConvertTo<UInt16>(); });
            Assert.ThrowsException<ErrorAssembleException>(() => { AIMath.Calculation("(1+1 * (3 + 1)", asmLoad).ConvertTo<UInt16>(); });
            Assert.ThrowsException<ErrorAssembleException>(() => { AIMath.Calculation("(1+1 * (3 + 1)))", asmLoad).ConvertTo<UInt16>(); });
            Assert.ThrowsException<ErrorAssembleException>(() => { AIMath.Calculation("0", asmLoad).ConvertTo<Double>(); });
            Assert.ThrowsException<ErrorAssembleException>(() => { AIMath.Calculation("LB + 1", asmLoad).ConvertTo<UInt16>(); });
            Assert.ThrowsException<ErrorAssembleException>(() => { AIMath.Calculation("0 == 1 ? 5", asmLoad).ConvertTo<UInt16>(); });
            Assert.ThrowsException<ErrorAssembleException>(() => { AIMath.Calculation("\"ABC\" = \"ABC\"", asmLoad).ConvertTo<UInt16>(); });
            Assert.ThrowsException<ErrorAssembleException>(() => { AIMath.Calculation("\"ABC\" == 0", asmLoad).ConvertTo<UInt16>(); });
            Assert.ThrowsException<ErrorAssembleException>(() => { AIMath.Calculation("0 == \"ABC\"", asmLoad).ConvertTo<UInt16>(); });
            Assert.ThrowsException<ErrorAssembleException>(() => { AIMath.Calculation("1 + FUNC(0)", asmLoad).ConvertTo<UInt16>(); });
            Assert.ThrowsException<ErrorAssembleException>(() => { AIMath.Calculation("LB", asmLoad).ConvertTo<UInt16>(); });
            Assert.ThrowsException<ErrorAssembleException>(() => { AIMath.Calculation("$", asmLoad).ConvertTo<UInt16>(); });
            Assert.ThrowsException<ErrorAssembleException>(() => { AIMath.Calculation("$$", asmLoad).ConvertTo<UInt16>(); });
            Assert.ThrowsException<ErrorAssembleException>(() => { AIMath.Calculation("low * 10", asmLoad).ConvertTo<UInt16>(); });
        }

        [TestMethod]
        public void Calc_String()
        {
            var asmLoad = new AsmLoad(new AsmOption(), new InstructionSet.Z80());

            Assert.AreEqual(0x31, AIMath.Calculation("'0' + 1", asmLoad).ConvertTo<byte>());
            Assert.AreEqual(true, AIMath.Calculation("\"012\" + \"3\" == \"0123\"", asmLoad).ConvertTo<bool>());
            Assert.AreEqual(true, AIMath.Calculation("\"012\" + \"4\" != \"0123\"", asmLoad).ConvertTo<bool>());
        }

        [TestMethod]
        public void TryParse_Test()
        {
            {
                Assert.IsTrue(AIMath.TryParse("0 + 1", out var result));
                Assert.AreEqual(1, result.ConvertTo<byte>());
            }
            {
                Assert.IsFalse(AIMath.TryParse("0 ** 1", out var result));
            }
            {
                var asmAddress = new AsmAddress();
                asmAddress.Program = 0x8000;
                asmAddress.Output = 0x8000;

                var asmLoad = new AsmLoad(new AsmOption(), new InstructionSet.Z80());
                asmLoad.AddLabel(new LabelAdr("[NAME_SPACE_DEFAULT]", "", asmLoad));

                var label = new LabelAdr("LB", "0xAA02", asmLoad);
                asmLoad.AddLabel(label);

                Assert.IsTrue(AIMath.TryParse("LB + 1", asmLoad, asmAddress, out var result));
                Assert.AreEqual(0xAA02 + 1, result.ConvertTo<UInt16>());

            }
            {
                var asmLoad = new AsmLoad(new AsmOption(), new InstructionSet.Z80());

                AIMath.TryParse("1+2", asmLoad, out var result);
                Assert.AreEqual(3, result.ConvertTo<UInt16>());
            }
        }
    }
}
