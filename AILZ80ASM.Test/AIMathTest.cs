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
            Assert.IsTrue(AIMath.IsNumber("123"));
        }

        [TestMethod]
        public void Calc_01()
        {
            Assert.AreEqual(AIMath.Calc<UInt16>("1+2*((2+1))+6/2"), 1 + 2 * ((2 + 1)) + 6 / 2);
            Assert.AreEqual(AIMath.Calc<UInt16>("6%5"), 6 % 5);
            Assert.AreEqual(AIMath.Calc<UInt16>("5<<1"), 5 << 1);
            Assert.AreEqual(AIMath.Calc<UInt16>("5<<5"), 5 << 5);
            Assert.AreEqual(AIMath.Calc<UInt16>("5>>1"), 5 >> 1);
            Assert.AreEqual(AIMath.Calc<UInt16>("5>>5"), 5 >> 5);
        }

        [TestMethod]
        public void Calc_02()
        {
            Assert.AreEqual(AIMath.Calc<UInt16>("2 == 2 ? (1 + 1) * 2: (2 + 2) * 3"), 2 == 2 ? (1 + 1) * 2 : (2 + 2) * 3);
            Assert.AreEqual(AIMath.Calc<UInt16>("2 == 3 ? (1 + 1) * 2: (2 + 2) * 3"), 2 == 3 ? (1 + 1) * 2 : (2 + 2) * 3);
        }

        [TestMethod]
        public void Calc_03()
        {
            Assert.AreEqual(AIMath.Calc<bool>("2 == 2"), 2 == 2);
            Assert.AreEqual(AIMath.Calc<bool>("2 == 3"), 2 == 3);
            Assert.AreEqual(AIMath.Calc<bool>("2 != 2"), 2 != 2);
            Assert.AreEqual(AIMath.Calc<bool>("2 != 3"), 2 != 3);
            Assert.AreEqual(AIMath.Calc<bool>("2 >= 2"), 2 >= 2);
            Assert.AreEqual(AIMath.Calc<bool>("2 >= 3"), 2 >= 3);
            Assert.AreEqual(AIMath.Calc<bool>("1 >= 2"), 1 >= 2);
            Assert.AreEqual(AIMath.Calc<bool>("2 <= 2"), 2 <= 2);
            Assert.AreEqual(AIMath.Calc<bool>("2 <= 3"), 2 <= 3);
            Assert.AreEqual(AIMath.Calc<bool>("1 <= 2"), 1 <= 2);
            Assert.AreEqual(AIMath.Calc<bool>("2 >  2"), 2 > 2);
            Assert.AreEqual(AIMath.Calc<bool>("2 >  3"), 2 > 3);
            Assert.AreEqual(AIMath.Calc<bool>("1 >  2"), 1 > 2);
            Assert.AreEqual(AIMath.Calc<bool>("2 <  2"), 2 < 2);
            Assert.AreEqual(AIMath.Calc<bool>("2 <  3"), 2 < 3);
            Assert.AreEqual(AIMath.Calc<bool>("1 <  2"), 1 < 2);
        }

        [TestMethod]
        public void Calc_04()
        {
            Assert.AreEqual(AIMath.Calc<UInt16>("8&1"), 8 & 1);
            Assert.AreEqual(AIMath.Calc<UInt16>("8&2"), 8 & 2);
            Assert.AreEqual(AIMath.Calc<UInt16>("1|6"), 1 | 6);
            Assert.AreEqual(AIMath.Calc<UInt16>("8|6"), 8 | 6);
            Assert.AreEqual(AIMath.Calc<UInt16>("8^1"), 8 ^ 1);
            Assert.AreEqual(AIMath.Calc<UInt16>("8^2"), 8 ^ 2);
        }

        [TestMethod]
        public void Calc_05()
        {
            Assert.AreEqual(AIMath.Calc<bool>("1 == 1 || 2 == 2"), 1 == 1 || 2 == 2);
            Assert.AreEqual(AIMath.Calc<bool>("1 != 1 || 2 != 2"), 1 != 1 || 2 != 2);
            Assert.AreEqual(AIMath.Calc<bool>("1 == 1 || 2 != 2"), 1 == 1 || 2 != 2);
            Assert.AreEqual(AIMath.Calc<bool>("1 != 1 || 2 == 2"), 1 != 1 || 2 == 2);

            Assert.AreEqual(AIMath.Calc<bool>("1 == 1 && 2 == 2"), 1 == 1 && 2 == 2);
            Assert.AreEqual(AIMath.Calc<bool>("1 != 1 && 2 != 2"), 1 != 1 && 2 != 2);
            Assert.AreEqual(AIMath.Calc<bool>("1 == 1 && 2 != 2"), 1 == 1 && 2 != 2);
            Assert.AreEqual(AIMath.Calc<bool>("1 != 1 && 2 == 2"), 1 != 1 && 2 == 2);
        }

    }
}
