using AILZ80ASM.Assembler;
using AILZ80ASM.OperationItems;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace AILZ80ASM.Test
{
    [TestClass]
    public class OperationItemOPCodeTest
    {
        [TestMethod]
        public void AssembleTest()
        {
            var asmLoad = new AsmLoad(new AsmOption(), new InstructionSet.Z80());
            var operationItem = OperationItemOPCode.Create(new LineItem("XOR A", 0, default(FileInfo)), asmLoad);

            Assert.ThrowsException<NullReferenceException>(() =>
            {
                operationItem.Assemble(asmLoad);
            });
        }


        [TestMethod]
        public void GetBinTest()
        {
            var asmLoad = new AsmLoad(new AsmOption(), new InstructionSet.Z80());
            var operationItem = OperationItemOPCode.Create(new LineItem("XOR A", 0, default(FileInfo)), asmLoad);

            Assert.AreEqual(0, operationItem.Bin.Length);
        }

        [TestMethod]
        public void GetLengthTest()
        {
            var asmLoad = new AsmLoad(new AsmOption(), new InstructionSet.Z80());
            var operationItem = OperationItemOPCode.Create(new LineItem("XOR A", 0, default(FileInfo)), asmLoad);

            Assert.AreEqual(1, operationItem.Length.Program);
        }
    }
}
