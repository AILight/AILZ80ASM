using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AILZ80ASM.Test
{
    [TestClass]
    public class AIMathTest
    {
        [TestMethod]
        public void IsNumber()
        {
            Assert.IsTrue(AIMath.IsNumber("ABC"));
        }

        [TestMethod]
        public void Calc_01()
        {
            Assert.AreEqual(AIMath.Calc<int>("1+2*((2+1))+6/2"), 10);
        }
        
        [TestMethod]
        public void Calc_02()
        {
            Assert.AreEqual(AIMath.Calc<int>("6%5"), 1);
        }

        [TestMethod]
        public void Calc_03()
        {
            Assert.AreEqual(AIMath.Calc<int>("5<<1"), 10);
        }

        [TestMethod]
        public void Calc_04()
        {
            Assert.AreEqual(AIMath.Calc<int>("5>>1"), 2);
        }

        [TestMethod]
        public void Calc_05()
        {
            Assert.AreEqual(AIMath.Calc<int>("2 == 2 ? 1 : 2"), 2);
        }
    }
}
