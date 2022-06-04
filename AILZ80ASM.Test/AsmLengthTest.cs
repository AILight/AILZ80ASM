using AILZ80ASM.Assembler;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace AILZ80ASM.Test
{
    [TestClass]
    public class AsmLengthTest
    {
        [TestMethod]
        public void AsmLengthTest01()
        {
            var asmLength = new AsmLength(0x8000);

            Assert.AreEqual((UInt16)0x8000, asmLength.Program);
            Assert.AreEqual((UInt32)0x00008000, asmLength.Output);
        }

        [TestMethod]
        public void AsmLengthTest02()
        {
            var asmLength = new AsmLength(0x8000, 0x00108000);

            Assert.AreEqual((UInt16)0x8000, asmLength.Program);
            Assert.AreEqual((UInt32)0x00108000, asmLength.Output);
        }
    }
}
