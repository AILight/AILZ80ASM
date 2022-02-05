using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;

namespace AILZ80ASM.Test
{
    [TestClass]
    public class SSMainTest
    {
        [TestMethod]
        public void TestMainSuccess()
        {
            var result = Program.Main(@"./Test/TestSS_Main/Success.Z80");
            Assert.AreEqual(result, 0);
        }

        [TestMethod]
        public void TestErrorSuccess()
        {
            var result = Program.Main(@"./Test/TestSS_Main/Error.Z80");
            Assert.AreEqual(result, 1);
        }
    }
}
