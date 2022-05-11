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
            Assert.AreEqual(AIMath.Calculation("1+2*((2+1))+6/2").ConvertTo<UInt16>(), 1 + 2 * ((2 + 1)) + 6 / 2);
            Assert.AreEqual(AIMath.Calculation("-2 + 1").ConvertTo<UInt16>(), (UInt16)((-2 + 1) & 0xFFFF));
            Assert.AreEqual(AIMath.Calculation("-1 + 1").ConvertTo<UInt16>(), -1 + 1);
            Assert.AreEqual(AIMath.Calculation("+1 + 1").ConvertTo<UInt16>(), +1 + 1);
            Assert.AreEqual(AIMath.Calculation("+1 + -1").ConvertTo<UInt16>(), +1 + -1);
            Assert.AreEqual(AIMath.Calculation("6%1").ConvertTo<UInt16>(), 6 % 1);
            Assert.AreEqual(AIMath.Calculation("6%5").ConvertTo<UInt16>(), 6 % 5);
            Assert.AreEqual(AIMath.Calculation("6%6").ConvertTo<UInt16>(), 6 % 6);
            Assert.AreEqual(AIMath.Calculation("5<<1").ConvertTo<UInt16>(), 5 << 1);
            Assert.AreEqual(AIMath.Calculation("5<<5").ConvertTo<UInt16>(), 5 << 5);
            Assert.AreEqual(AIMath.Calculation("5>>1").ConvertTo<UInt16>(), 5 >> 1);
            Assert.AreEqual(AIMath.Calculation("5>>5").ConvertTo<UInt16>(), 5 >> 5);
        }

        [TestMethod]
        public void Calc_02()
        {
            Assert.AreEqual(AIMath.Calculation("2 == 2 ? (1 + 1) * 2: (2 + 2) * 3").ConvertTo<UInt16>(), 2 == 2 ? (1 + 1) * 2 : (2 + 2) * 3);
            Assert.AreEqual(AIMath.Calculation("2 == 3 ? (1 + 1) * 2: (2 + 2) * 3").ConvertTo<UInt16>(), 2 == 3 ? (1 + 1) * 2 : (2 + 2) * 3);
            Assert.AreEqual(AIMath.Calculation("1 + 2 == 2 ? (1 + 1) * 2: (2 + 2) * 3").ConvertTo<UInt16>(), 1 + 2 == 2 ? (1 + 1) * 2 : (2 + 2) * 3);
            Assert.AreEqual(AIMath.Calculation("1 + 2 == 3 ? (1 + 1) * 2: (2 + 2) * 3").ConvertTo<UInt16>(), 1 + 2 == 3 ? (1 + 1) * 2 : (2 + 2) * 3);
            Assert.AreEqual(AIMath.Calculation("1 + (2 == 2 ? (1 + 1) * 2: (2 + 2) * 3)").ConvertTo<UInt16>(), 1 + (2 == 2 ? (1 + 1) * 2 : (2 + 2) * 3));
            Assert.AreEqual(AIMath.Calculation("1 + (2 == 3 ? (1 + 1) * 2: (2 + 2) * 3)").ConvertTo<UInt16>(), 1 + (2 == 3 ? (1 + 1) * 2 : (2 + 2) * 3));
        }

        [TestMethod]
        public void Calc_03()
        {
            Assert.AreEqual(AIMath.Calculation("2 == 2").ConvertTo<bool>(), 2 == 2);
            Assert.AreEqual(AIMath.Calculation("2 == 3").ConvertTo<bool>(), 2 == 3);
            Assert.AreEqual(AIMath.Calculation("2 != 2").ConvertTo<bool>(), 2 != 2);
            Assert.AreEqual(AIMath.Calculation("2 != 3").ConvertTo<bool>(), 2 != 3);
            Assert.AreEqual(AIMath.Calculation("2 >= 2").ConvertTo<bool>(), 2 >= 2);
            Assert.AreEqual(AIMath.Calculation("2 >= 3").ConvertTo<bool>(), 2 >= 3);
            Assert.AreEqual(AIMath.Calculation("1 >= 2").ConvertTo<bool>(), 1 >= 2);
            Assert.AreEqual(AIMath.Calculation("2 <= 2").ConvertTo<bool>(), 2 <= 2);
            Assert.AreEqual(AIMath.Calculation("2 <= 3").ConvertTo<bool>(), 2 <= 3);
            Assert.AreEqual(AIMath.Calculation("1 <= 2").ConvertTo<bool>(), 1 <= 2);
            Assert.AreEqual(AIMath.Calculation("2 >  2").ConvertTo<bool>(), 2 > 2);
            Assert.AreEqual(AIMath.Calculation("2 >  3").ConvertTo<bool>(), 2 > 3);
            Assert.AreEqual(AIMath.Calculation("1 >  2").ConvertTo<bool>(), 1 > 2);
            Assert.AreEqual(AIMath.Calculation("2 <  2").ConvertTo<bool>(), 2 < 2);
            Assert.AreEqual(AIMath.Calculation("2 <  3").ConvertTo<bool>(), 2 < 3);
            Assert.AreEqual(AIMath.Calculation("1 <  2").ConvertTo<bool>(), 1 < 2);
        }

        [TestMethod]
        public void Calc_04()
        {
            Assert.AreEqual(AIMath.Calculation("8&1").ConvertTo<UInt16>(), 8 & 1);
            Assert.AreEqual(AIMath.Calculation("8&2").ConvertTo<UInt16>(), 8 & 2);
            Assert.AreEqual(AIMath.Calculation("1|6").ConvertTo<UInt16>(), 1 | 6);
            Assert.AreEqual(AIMath.Calculation("8|6").ConvertTo<UInt16>(), 8 | 6);
            Assert.AreEqual(AIMath.Calculation("8^1").ConvertTo<UInt16>(), 8 ^ 1);
            Assert.AreEqual(AIMath.Calculation("8^2").ConvertTo<UInt16>(), 8 ^ 2);
        }

        [TestMethod]
        public void Calc_05()
        {
            Assert.AreEqual(AIMath.Calculation("1 == 1 || 2 == 2").ConvertTo<bool>(), 1 == 1 || 2 == 2);
            Assert.AreEqual(AIMath.Calculation("1 != 1 || 2 != 2").ConvertTo<bool>(), 1 != 1 || 2 != 2);
            Assert.AreEqual(AIMath.Calculation("1 == 1 || 2 != 2").ConvertTo<bool>(), 1 == 1 || 2 != 2);
            Assert.AreEqual(AIMath.Calculation("1 != 1 || 2 == 2").ConvertTo<bool>(), 1 != 1 || 2 == 2);

            Assert.AreEqual(AIMath.Calculation("1 == 1 && 2 == 2").ConvertTo<bool>(), 1 == 1 && 2 == 2);
            Assert.AreEqual(AIMath.Calculation("1 != 1 && 2 != 2").ConvertTo<bool>(), 1 != 1 && 2 != 2);
            Assert.AreEqual(AIMath.Calculation("1 == 1 && 2 != 2").ConvertTo<bool>(), 1 == 1 && 2 != 2);
            Assert.AreEqual(AIMath.Calculation("1 != 1 && 2 == 2").ConvertTo<bool>(), 1 != 1 && 2 == 2);
        }

        [TestMethod]
        public void Calc_06()
        {
            Assert.AreEqual(AIMath.Calculation("~0").ConvertTo<UInt16>(), UInt16.MaxValue /*~0*/);
            Assert.AreEqual(AIMath.Calculation("~65535").ConvertTo<UInt16>(), default(UInt16) /*~65535 */);
            Assert.AreEqual(AIMath.Calculation("!0").ConvertTo<UInt16>(), UInt16.MaxValue /*~0*/);
            Assert.AreEqual(AIMath.Calculation("!65535").ConvertTo<UInt16>(), default(UInt16) /*~65535 */);
        }

        [TestMethod]
        public void Calc_07()
        {
            Assert.AreEqual(AIMath.Calculation("FFFFH").ConvertTo<UInt16>(), 0xFFFF);
            Assert.AreEqual(AIMath.Calculation("0xFFFF").ConvertTo<UInt16>(), 0xFFFF);
            Assert.AreEqual(AIMath.Calculation("176O").ConvertTo<UInt16>(), 126);
            Assert.AreEqual(AIMath.Calculation("0101_1010b").ConvertTo<UInt16>(), 0x5A);
            Assert.AreEqual(AIMath.Calculation("%0101_1010").ConvertTo<UInt16>(), 0x5A);
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

            Assert.AreEqual(AIMath.Calculation("LB + 1", asmLoad).ConvertTo<UInt16>(), 0xAA02);
            Assert.AreEqual(AIMath.Calculation("$ + 1", asmLoad, asmAddress).ConvertTo<UInt16>(), 0x8001);
        }

        [TestMethod]
        public void Calc_09()
        {
            var asmLoad = new AsmLoad(new AsmOption(), new InstructionSet.Z80());
            
            Assert.AreEqual(AIMath.Calculation("'0'", asmLoad).ConvertTo<byte>(), (byte)'0');
            Assert.AreEqual(AIMath.Calculation("' ' - '0'", asmLoad).ConvertTo<byte>(), 256 + ((byte)' ' - (byte)'0'));
            Assert.AreEqual(AIMath.Calculation("':'", asmLoad).ConvertTo<byte>(), (byte)':');
            Assert.AreEqual(AIMath.Calculation("'\\''", asmLoad).ConvertTo<byte>(), (byte)'\'');
            Assert.AreEqual(AIMath.Calculation("'\\0'", asmLoad).ConvertTo<byte>(), (byte)0);
            Assert.AreEqual(AIMath.Calculation("'\\\\'", asmLoad).ConvertTo<byte>(), (byte)'\\');

            foreach (var item in System.Linq.Enumerable.Range(0x20, 95))
            {
                if (item == 0x27 || item == 0x5c)
                {
                    continue;
                }

                var character = (char)(byte)item;

                Assert.AreEqual(AIMath.Calculation($"'{character}'", asmLoad).ConvertTo<byte>(), (byte)item);
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
                Assert.AreEqual(AIMath.Calculation($"'{escape}{character}'+80H", asmLoad).ConvertTo<byte>(), (byte)(item + 0x80), $"{character}:{item}");
            }
        }

        [TestMethod]
        public void Calc_UInt32()
        {
            Assert.AreEqual(AIMath.Calculation("1+2*((2+1))+6/2").ConvertTo<UInt32>(), (UInt32)(1 + 2 * ((2 + 1)) + 6 / 2));
            Assert.AreEqual(AIMath.Calculation("-2 + 1").ConvertTo<UInt32>(), (UInt32)((-2 + 1) & 0xFFFFFFFF));
            Assert.AreEqual(AIMath.Calculation("-1 + 1").ConvertTo<UInt32>(), (UInt32)(-1 + 1));
            Assert.AreEqual(AIMath.Calculation("+1 + 1").ConvertTo<UInt32>(), (UInt32)(+1 + 1));
            Assert.AreEqual(AIMath.Calculation("+1 + -1").ConvertTo<UInt32>(), (UInt32)(+1 + -1));
            Assert.AreEqual(AIMath.Calculation("6%1").ConvertTo<UInt32>(), (UInt32)(6 % 1));
            Assert.AreEqual(AIMath.Calculation("6%5").ConvertTo<UInt32>(), (UInt32)(6 % 5));
            Assert.AreEqual(AIMath.Calculation("6%6").ConvertTo<UInt32>(), (UInt32)(6 % 6));
            Assert.AreEqual(AIMath.Calculation("5<<1").ConvertTo<UInt32>(), (UInt32)(5 << 1));
            Assert.AreEqual(AIMath.Calculation("5<<5").ConvertTo<UInt32>(), (UInt32)(5 << 5));
            Assert.AreEqual(AIMath.Calculation("5>>1").ConvertTo<UInt32>(), (UInt32)(5 >> 1));
            Assert.AreEqual(AIMath.Calculation("5>>5").ConvertTo<UInt32>(), (UInt32)(5 >> 5));
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
                var label = new LabelAdr("LB2", "LABEL", asmLoad);
                asmLoad.AddLabel(label);
            }
            Assert.AreEqual(AIMath.Calculation("-1", asmLoad).ConvertTo<byte>(), 0xFF);
            Assert.AreEqual(AIMath.Calculation("-1 * -1", asmLoad).ConvertTo<byte>(), 1);
            Assert.AreEqual(AIMath.Calculation("-LB", asmLoad).ConvertTo<UInt16>(), 0x55FE);
            Assert.AreEqual(AIMath.Calculation("LB.@H", asmLoad).ConvertTo<byte>(), 0xAA);
            Assert.AreEqual(AIMath.Calculation("LB.@HIGH", asmLoad).ConvertTo<byte>(), 0xAA);
            Assert.AreEqual(AIMath.Calculation("LB.@L", asmLoad).ConvertTo<byte>(), 0x02);
            Assert.AreEqual(AIMath.Calculation("LB.@LOW", asmLoad).ConvertTo<byte>(), 0x02);
            Assert.IsTrue(AIMath.Calculation("LB2.@T == \"LABEL\"", asmLoad).ConvertTo<bool>());
            Assert.IsTrue(AIMath.Calculation("LB2.@TEXT == \"LABEL\"", asmLoad).ConvertTo<bool>());
            Assert.IsFalse(AIMath.Calculation("LB2.@T == \"LABEL1\"", asmLoad).ConvertTo<bool>());
            Assert.AreEqual(AIMath.Calculation("$", asmLoad, asmAddress).ConvertTo<UInt16>(), 0x8000);
            Assert.AreEqual(AIMath.Calculation("$$", asmLoad, asmAddress).ConvertTo<UInt16>(), 0x8100);
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
            Assert.ThrowsException<ErrorAssembleException>(() => { AIMath.Calculation("'AB'", asmLoad).ConvertTo<UInt16>(); });
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
        }

        [TestMethod]
        public void Calc_String()
        {
            var asmLoad = new AsmLoad(new AsmOption(), new InstructionSet.Z80());

            Assert.AreEqual(AIMath.Calculation("'0' + 1", asmLoad).ConvertTo<byte>(), 0x31);
            Assert.AreEqual(AIMath.Calculation("\"012\" + \"3\" == \"0123\"", asmLoad).ConvertTo<bool>(), true);
            Assert.AreEqual(AIMath.Calculation("\"012\" + \"4\" != \"0123\"", asmLoad).ConvertTo<bool>(), true);
        }

        [TestMethod]
        public void TryParse_Test()
        {
            {
                Assert.IsTrue(AIMath.TryParse("0 + 1", out var result));
                Assert.AreEqual(result.ConvertTo<byte>(), 1);
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
                Assert.AreEqual(result.ConvertTo<UInt16>(), 0xAA02 + 1);

            }
            {
                var asmLoad = new AsmLoad(new AsmOption(), new InstructionSet.Z80());

                AIMath.TryParse("1+2", asmLoad, out var result);
                Assert.AreEqual(result.ConvertTo<UInt16>(), 3);
            }
        }
    }
}
