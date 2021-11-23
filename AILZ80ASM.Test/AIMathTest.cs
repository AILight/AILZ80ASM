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

            Assert.IsFalse(AIMath.IsNumber("O123"));
            Assert.IsFalse(AIMath.IsNumber("FFF"));
            Assert.IsFalse(AIMath.IsNumber("0000_1111%"));
            Assert.IsFalse(AIMath.IsNumber("0000_1111"));
            Assert.IsFalse(AIMath.IsNumber("0FFF"));
        }

        [TestMethod]
        public void Calc_01()
        {
            Assert.AreEqual(AIMath.ConvertTo<UInt16>("1+2*((2+1))+6/2"), 1 + 2 * ((2 + 1)) + 6 / 2);
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

            var asmLoad = new AsmLoad();
            var label = new Label("LB", "0xAA01", asmLoad);
            label.SetValue(asmLoad);

            asmLoad.AddLabel(label);

            Assert.AreEqual(AIMath.ConvertTo<UInt16>("LB + 1", asmLoad), 0xAA02);
            Assert.AreEqual(AIMath.ConvertTo<UInt16>("$ + 1", asmLoad, asmAddress), 0x8001);
        }

    }
}
