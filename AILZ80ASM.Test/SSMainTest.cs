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
            var result = Program.Main(@"./Test/TestSS_Main/Success.Z80", "-bin", "test1.bin", "-err", "test1.err");
            Assert.AreEqual(result, 0);
        }

        [TestMethod]
        public void TestMainSuccessAllOption()
        {
            var result = Program.Main(@"./Test/TestSS_Main/Success.Z80", "-bin", "test2.bin", "-err", "test2.err", "-lst", "test2.lst", "-equ", "test2.equ");
            Assert.AreEqual(result, 0);
        }

        [TestMethod]
        public void TestErrorSuccess()
        {
            var result = Program.Main(@"./Test/TestSS_Main/Error.Z80", "-bin", "test3.bin", "-err", "test3.err");
            Assert.AreEqual(result, 1);
        }
    }
}
