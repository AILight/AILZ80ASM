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

            Assert.AreEqual(asmLength.Program, (UInt16)0x8000);
            Assert.AreEqual(asmLength.Output, (UInt32)0x00008000);
        }

        [TestMethod]
        public void AsmLengthTest02()
        {
            var asmLength = new AsmLength(0x8000, 0x00108000);

            Assert.AreEqual(asmLength.Program, (UInt16)0x8000);
            Assert.AreEqual(asmLength.Output, (UInt32)0x00108000);
        }
    }
}
