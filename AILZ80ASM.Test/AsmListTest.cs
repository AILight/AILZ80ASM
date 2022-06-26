using AILZ80ASM.Assembler;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace AILZ80ASM.Test
{
    [TestClass]
    public class AsmListTest
    {
        [TestMethod]
        public void ToStringTest()
        {
            var asmList = AsmList.CreateSource("LD A, 0");
            Assert.AreEqual("                                LD A, 0", asmList.ToString());
        }
    }
}
