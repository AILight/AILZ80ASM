using AILZ80ASM.AILight;
using AILZ80ASM.Assembler;
using AILZ80ASM.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace AILZ80ASM.Test
{
    [TestClass]
    public class AIMathTest
    {
        [TestMethod]
        public void IsNumber()
        {
            Assert.IsTrue(AIMath.IsNumber("01"));
            Assert.IsTrue(AIMath.IsNumber("123"));
            Assert.IsTrue(AIMath.IsNumber("FFFH"));
            Assert.IsTrue(AIMath.IsNumber("0FFFH"));
            Assert.IsTrue(AIMath.IsNumber("%0000_1111"));
            Assert.IsTrue(AIMath.IsNumber("0000_1111b"));
            Assert.IsTrue(AIMath.IsNumber("$FFFF"));
            Assert.IsTrue(AIMath.IsNumber("0xFFFF"));
            Assert.IsTrue(AIMath.IsNumber("777o"));

            Assert.IsFalse(AIMath.IsNumber("O123"));
            Assert.IsFalse(AIMath.IsNumber("FFF"));
            Assert.IsFalse(AIMath.IsNumber("0000_1111%"));
            Assert.IsFalse(AIMath.IsNumber("0000_1111"));
            Assert.IsFalse(AIMath.IsNumber("0FFF"));
            Assert.IsFalse(AIMath.IsNumber("0xXFFF"));
            Assert.IsFalse(AIMath.IsNumber("888o"));
        }

        [TestMethod]
        public void Calc_01()
        {
            Assert.AreEqual(AIMath.ConvertTo<UInt16>("1+2*((2+1))+6/2"), 1 + 2 * ((2 + 1)) + 6 / 2);
            Assert.AreEqual(AIMath.ConvertTo<UInt16>("-2 + 1"), (UInt16)((-2 + 1) & 0xFFFF));
            Assert.AreEqual(AIMath.ConvertTo<UInt16>("-1 + 1"), -1 + 1);
            Assert.AreEqual(AIMath.ConvertTo<UInt16>("+1 + 1"), +1 + 1);
            Assert.AreEqual(AIMath.ConvertTo<UInt16>("+1 + -1"), +1 + -1);
            Assert.AreEqual(AIMath.ConvertTo<UInt16>("6%1"), 6 % 1);
            Assert.AreEqual(AIMath.ConvertTo<UInt16>("6%5"), 6 % 5);
            Assert.AreEqual(AIMath.ConvertTo<UInt16>("6%6"), 6 % 6);
            Assert.AreEqual(AIMath.ConvertTo<UInt16>("5<<1"), 5 << 1);
            Assert.AreEqual(AIMath.ConvertTo<UInt16>("5<<5"), 5 << 5);
            Assert.AreEqual(AIMath.ConvertTo<UInt16>("5>>1"), 5 >> 1);
            Assert.AreEqual(AIMath.ConvertTo<UInt16>("5>>5"), 5 >> 5);
        }

        [TestMethod]
        public void Calc_02()
        {
            Assert.AreEqual(AIMath.ConvertTo<UInt16>("2 == 2 ? (1 + 1) * 2: (2 + 2) * 3"), 2 == 2 ? (1 + 1) * 2 : (2 + 2) * 3);
            Assert.AreEqual(AIMath.ConvertTo<UInt16>("2 == 3 ? (1 + 1) * 2: (2 + 2) * 3"), 2 == 3 ? (1 + 1) * 2 : (2 + 2) * 3);
            Assert.AreEqual(AIMath.ConvertTo<UInt16>("1 + 2 == 2 ? (1 + 1) * 2: (2 + 2) * 3"), 1 + 2 == 2 ? (1 + 1) * 2 : (2 + 2) * 3);
            Assert.AreEqual(AIMath.ConvertTo<UInt16>("1 + 2 == 3 ? (1 + 1) * 2: (2 + 2) * 3"), 1 + 2 == 3 ? (1 + 1) * 2 : (2 + 2) * 3);
            Assert.AreEqual(AIMath.ConvertTo<UInt16>("1 + (2 == 2 ? (1 + 1) * 2: (2 + 2) * 3)"), 1 + (2 == 2 ? (1 + 1) * 2 : (2 + 2) * 3));
            Assert.AreEqual(AIMath.ConvertTo<UInt16>("1 + (2 == 3 ? (1 + 1) * 2: (2 + 2) * 3)"), 1 + (2 == 3 ? (1 + 1) * 2 : (2 + 2) * 3));
        }

        [TestMethod]
        public void Calc_03()
        {
            Assert.AreEqual(AIMath.ConvertTo<bool>("2 == 2"), 2 == 2);
            Assert.AreEqual(AIMath.ConvertTo<bool>("2 == 3"), 2 == 3);
            Assert.AreEqual(AIMath.ConvertTo<bool>("2 != 2"), 2 != 2);
            Assert.AreEqual(AIMath.ConvertTo<bool>("2 != 3"), 2 != 3);
            Assert.AreEqual(AIMath.ConvertTo<bool>("2 >= 2"), 2 >= 2);
            Assert.AreEqual(AIMath.ConvertTo<bool>("2 >= 3"), 2 >= 3);
            Assert.AreEqual(AIMath.ConvertTo<bool>("1 >= 2"), 1 >= 2);
            Assert.AreEqual(AIMath.ConvertTo<bool>("2 <= 2"), 2 <= 2);
            Assert.AreEqual(AIMath.ConvertTo<bool>("2 <= 3"), 2 <= 3);
            Assert.AreEqual(AIMath.ConvertTo<bool>("1 <= 2"), 1 <= 2);
            Assert.AreEqual(AIMath.ConvertTo<bool>("2 >  2"), 2 > 2);
            Assert.AreEqual(AIMath.ConvertTo<bool>("2 >  3"), 2 > 3);
            Assert.AreEqual(AIMath.ConvertTo<bool>("1 >  2"), 1 > 2);
            Assert.AreEqual(AIMath.ConvertTo<bool>("2 <  2"), 2 < 2);
            Assert.AreEqual(AIMath.ConvertTo<bool>("2 <  3"), 2 < 3);
            Assert.AreEqual(AIMath.ConvertTo<bool>("1 <  2"), 1 < 2);
        }

        [TestMethod]
        public void Calc_04()
        {
            Assert.AreEqual(AIMath.ConvertTo<UInt16>("8&1"), 8 & 1);
            Assert.AreEqual(AIMath.ConvertTo<UInt16>("8&2"), 8 & 2);
            Assert.AreEqual(AIMath.ConvertTo<UInt16>("1|6"), 1 | 6);
            Assert.AreEqual(AIMath.ConvertTo<UInt16>("8|6"), 8 | 6);
            Assert.AreEqual(AIMath.ConvertTo<UInt16>("8^1"), 8 ^ 1);
            Assert.AreEqual(AIMath.ConvertTo<UInt16>("8^2"), 8 ^ 2);
        }

        [TestMethod]
        public void Calc_05()
        {
            Assert.AreEqual(AIMath.ConvertTo<bool>("1 == 1 || 2 == 2"), 1 == 1 || 2 == 2);
            Assert.AreEqual(AIMath.ConvertTo<bool>("1 != 1 || 2 != 2"), 1 != 1 || 2 != 2);
            Assert.AreEqual(AIMath.ConvertTo<bool>("1 == 1 || 2 != 2"), 1 == 1 || 2 != 2);
            Assert.AreEqual(AIMath.ConvertTo<bool>("1 != 1 || 2 == 2"), 1 != 1 || 2 == 2);

            Assert.AreEqual(AIMath.ConvertTo<bool>("1 == 1 && 2 == 2"), 1 == 1 && 2 == 2);
            Assert.AreEqual(AIMath.ConvertTo<bool>("1 != 1 && 2 != 2"), 1 != 1 && 2 != 2);
            Assert.AreEqual(AIMath.ConvertTo<bool>("1 == 1 && 2 != 2"), 1 == 1 && 2 != 2);
            Assert.AreEqual(AIMath.ConvertTo<bool>("1 != 1 && 2 == 2"), 1 != 1 && 2 == 2);
        }

        [TestMethod]
        public void Calc_06()
        {
            Assert.AreEqual(AIMath.ConvertTo<UInt16>("~0"), UInt16.MaxValue /*~0*/);
            Assert.AreEqual(AIMath.ConvertTo<UInt16>("~65535"), default(UInt16) /*~65535 */);
            Assert.AreEqual(AIMath.ConvertTo<UInt16>("!0"), UInt16.MaxValue /*~0*/);
            Assert.AreEqual(AIMath.ConvertTo<UInt16>("!65535"), default(UInt16) /*~65535 */);
        }

        [TestMethod]
        public void Calc_07()
        {
            Assert.AreEqual(AIMath.ConvertTo<UInt16>("FFFFH"), 0xFFFF);
            Assert.AreEqual(AIMath.ConvertTo<UInt16>("0xFFFF"), 0xFFFF);
            Assert.AreEqual(AIMath.ConvertTo<UInt16>("176O"), 126);
            Assert.AreEqual(AIMath.ConvertTo<UInt16>("0101_1010b"), 0x5A);
            Assert.AreEqual(AIMath.ConvertTo<UInt16>("%0101_1010"), 0x5A);
        }

        [TestMethod]
        public void Calc_08()
        {
            var asmAddress = new AsmAddress();
            asmAddress.Program = 0x8000;
            asmAddress.Output = 0x8000;

            var asmLoad = new AsmLoad(new AsmOption(), new InstructionSet.Z80());
            asmLoad.AddLabel(new Label("[NS_Main]", "", asmLoad));
            var label = new Label("LB", "0xAA01", asmLoad);

            asmLoad.AddLabel(label);

            Assert.AreEqual(AIMath.ConvertTo<UInt16>("LB + 1", asmLoad), 0xAA02);
            Assert.AreEqual(AIMath.ConvertTo<UInt16>("$ + 1", asmLoad, asmAddress), 0x8001);
        }

        [TestMethod]
        public void Calc_09()
        {
            Assert.AreEqual(AIMath.ConvertTo<byte>("'0'"), (byte)'0');
            Assert.AreEqual(AIMath.ConvertTo<byte>("' ' - '0'"), 256 + ((byte)' ' - (byte)'0'));
            Assert.AreEqual(AIMath.ConvertTo<byte>("':'"), (byte)':');
            Assert.AreEqual(AIMath.ConvertTo<byte>("'\\''"), (byte)'\'');
            Assert.AreEqual(AIMath.ConvertTo<byte>("'\\0'"), (byte)0);

            foreach (var item in System.Linq.Enumerable.Range(0x20, 95))
            {
                if (item == 0x27)
                {
                    continue;
                }

                var character = (char)(byte)item;
                Assert.AreEqual(AIMath.ConvertTo<byte>($"'{character}'"), (byte)item);
            }
        }

        [TestMethod]
        public void Calc_UInt32()
        {
            Assert.AreEqual(AIMath.ConvertTo<UInt32>("1+2*((2+1))+6/2"), (UInt32)(1 + 2 * ((2 + 1)) + 6 / 2));
            Assert.AreEqual(AIMath.ConvertTo<UInt32>("-2 + 1"), (UInt32)((-2 + 1) & 0xFFFFFFFF));
            Assert.AreEqual(AIMath.ConvertTo<UInt32>("-1 + 1"), (UInt32)(-1 + 1));
            Assert.AreEqual(AIMath.ConvertTo<UInt32>("+1 + 1"), (UInt32)(+1 + 1));
            Assert.AreEqual(AIMath.ConvertTo<UInt32>("+1 + -1"), (UInt32)(+1 + -1));
            Assert.AreEqual(AIMath.ConvertTo<UInt32>("6%1"), (UInt32)(6 % 1));
            Assert.AreEqual(AIMath.ConvertTo<UInt32>("6%5"), (UInt32)(6 % 5));
            Assert.AreEqual(AIMath.ConvertTo<UInt32>("6%6"), (UInt32)(6 % 6));
            Assert.AreEqual(AIMath.ConvertTo<UInt32>("5<<1"), (UInt32)(5 << 1));
            Assert.AreEqual(AIMath.ConvertTo<UInt32>("5<<5"), (UInt32)(5 << 5));
            Assert.AreEqual(AIMath.ConvertTo<UInt32>("5>>1"), (UInt32)(5 >> 1));
            Assert.AreEqual(AIMath.ConvertTo<UInt32>("5>>5"), (UInt32)(5 >> 5));
        }

        [TestMethod]
        public void Calc_Label()
        {
            var asmAddress = new AsmAddress();
            asmAddress.Program = 0x8000;
            asmAddress.Output = 0x8100;

            var asmLoad = new AsmLoad(new AsmOption(), new InstructionSet.Z80());
            asmLoad.AddLabel(new Label("[NS_Main]", "", asmLoad));
            {
                var label = new Label("LB", "0xAA02", asmLoad);
                asmLoad.AddLabel(label);
            }
            {
                var label = new Label("LB2", "LABEL", asmLoad);
                asmLoad.AddLabel(label);
            }
            Assert.AreEqual(AIMath.ConvertTo<byte>("-1", asmLoad), 0xFF);
            Assert.AreEqual(AIMath.ConvertTo<byte>("-1 * -1", asmLoad), 1);
            Assert.AreEqual(AIMath.ConvertTo<UInt16>("-LB", asmLoad), 0x55FE);
            Assert.AreEqual(AIMath.ConvertTo<byte>("LB.@H", asmLoad), 0xAA);
            Assert.AreEqual(AIMath.ConvertTo<byte>("LB.@HIGH", asmLoad), 0xAA);
            Assert.AreEqual(AIMath.ConvertTo<byte>("LB.@L", asmLoad), 0x02);
            Assert.AreEqual(AIMath.ConvertTo<byte>("LB.@LOW", asmLoad), 0x02);
            Assert.IsTrue(AIMath.ConvertTo<bool>("LB2.@T == \"LABEL\"", asmLoad));
            Assert.IsTrue(AIMath.ConvertTo<bool>("LB2.@TEXT == \"LABEL\"", asmLoad));
            Assert.IsFalse(AIMath.ConvertTo<bool>("LB2.@T == \"LABEL1\"", asmLoad));
            Assert.AreEqual(AIMath.ConvertTo<UInt16>("$", asmLoad, asmAddress), 0x8000);
            Assert.AreEqual(AIMath.ConvertTo<UInt16>("$$", asmLoad, asmAddress), 0x8100);
        }

        [TestMethod]
        public void Calc_Exception()
        {
            Assert.ThrowsException<ErrorAssembleException>(() => { AIMath.ConvertTo<UInt16>(""); });
            Assert.ThrowsException<ErrorAssembleException>(() => { AIMath.ConvertTo<UInt16>("1**1"); });
            Assert.ThrowsException<ErrorAssembleException>(() => { AIMath.ConvertTo<UInt16>("1++"); });
            Assert.ThrowsException<ErrorAssembleException>(() => { AIMath.ConvertTo<UInt16>("1@1"); });
            Assert.ThrowsException<ErrorAssembleException>(() => { AIMath.ConvertTo<UInt16>("(1+)"); });
            Assert.ThrowsException<ErrorAssembleException>(() => { AIMath.ConvertTo<UInt16>("(1)*("); });
            Assert.ThrowsException<ErrorAssembleException>(() => { AIMath.ConvertTo<UInt16>("*3+2"); });
            Assert.ThrowsException<ErrorAssembleException>(() => { AIMath.ConvertTo<UInt16>("ABC+2"); });
            Assert.ThrowsException<ErrorAssembleException>(() => { AIMath.ConvertTo<UInt16>("0xG0"); });
            Assert.ThrowsException<ErrorAssembleException>(() => { AIMath.ConvertTo<UInt16>("$G0"); });
            Assert.ThrowsException<ErrorAssembleException>(() => { AIMath.ConvertTo<UInt16>("0000222b"); });
            Assert.ThrowsException<ErrorAssembleException>(() => { AIMath.ConvertTo<UInt16>("\"0\""); });
            Assert.ThrowsException<ErrorAssembleException>(() => { AIMath.ConvertTo<UInt16>("\"0"); });
            Assert.ThrowsException<ErrorAssembleException>(() => { AIMath.ConvertTo<UInt16>("(1+1 * (3 + 1)"); });
            Assert.ThrowsException<ErrorAssembleException>(() => { AIMath.ConvertTo<UInt16>("(1+1 * (3 + 1)))"); });
            Assert.ThrowsException<ErrorAssembleException>(() => { AIMath.ConvertTo<UInt16>("\"'\""); });
            Assert.ThrowsException<ErrorAssembleException>(() => { AIMath.ConvertTo<UInt16>("'AB'"); });
            Assert.ThrowsException<ErrorAssembleException>(() => { AIMath.ConvertTo<Double>("0"); });
            Assert.ThrowsException<ErrorAssembleException>(() => { AIMath.ConvertTo<UInt16>("LB + 1"); });
            Assert.ThrowsException<ErrorAssembleException>(() => { AIMath.ConvertTo<UInt16>("0 == 1 ? 5"); });
            Assert.ThrowsException<ErrorAssembleException>(() => { AIMath.ConvertTo<UInt16>("\"ABC\" = \"ABC\""); });
            Assert.ThrowsException<ErrorAssembleException>(() => { AIMath.ConvertTo<UInt16>("\"ABC\" == 0"); });
            Assert.ThrowsException<ErrorAssembleException>(() => { AIMath.ConvertTo<UInt16>("0 == \"ABC\""); });
            Assert.ThrowsException<ErrorAssembleException>(() => { AIMath.ConvertTo<UInt16>("1 + FUNC(0)"); });
            Assert.ThrowsException<ErrorAssembleException>(() => { AIMath.ConvertTo<UInt16>("LB"); });
            Assert.ThrowsException<ErrorAssembleException>(() => { AIMath.ConvertTo<UInt16>("$"); });
            Assert.ThrowsException<ErrorAssembleException>(() => { AIMath.ConvertTo<UInt16>("$$"); });
        }

        [TestMethod]
        public void Calc_String()
        {
            Assert.AreEqual(AIMath.ConvertTo<byte>("'0' + 1"), 0x31);
            Assert.AreEqual(AIMath.ConvertTo<bool>("\"012\" + \"3\" == \"0123\""), true);
            Assert.AreEqual(AIMath.ConvertTo<bool>("\"012\" + \"4\" != \"0123\""), true);
        }

        [TestMethod]
        public void TryParse_Test()
        {
            {
                Assert.IsTrue(AIMath.TryParse<byte>("0 + 1", out var result));
                Assert.AreEqual(result, 1);
            }
            {
                Assert.IsFalse(AIMath.TryParse<byte>("0 ** 1", out var result));
            }
            {
                var asmAddress = new AsmAddress();
                asmAddress.Program = 0x8000;
                asmAddress.Output = 0x8000;

                var asmLoad = new AsmLoad(new AsmOption(), new InstructionSet.Z80());
                asmLoad.AddLabel(new Label("[NS_Main]", "", asmLoad));

                var label = new Label("LB", "0xAA02", asmLoad);
                asmLoad.AddLabel(label);

                Assert.IsTrue(AIMath.TryParse<UInt16>("LB + 1", asmLoad, asmAddress, out var result));
                Assert.AreEqual(result, 0xAA02 + 1);
            }
        }
    }
}
