using AILZ80ASM.Assembler;
using AILZ80ASM.Exceptions;
using AILZ80ASM.LineDetailItems.ScopeItem.ExpansionItems;
using AILZ80ASM.OperationItems;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace AILZ80ASM.Test
{
    [TestClass]
    public class OperationItemTest
    {
        [TestMethod]
        public void AssembleTest()
        {
            var operationItem = new OperationItem();

            Assert.ThrowsException<NotImplementedException>(() =>
            {
                operationItem.Assemble(default(AsmLoad));
            });
        }


        [TestMethod]
        public void GetBinTest()
        {
            var operationItem = new OperationItem();

            Assert.ThrowsException<NotImplementedException>(() =>
            {
                var _ = operationItem.Bin;
            });
        }

        [TestMethod]
        public void GetLengthTest()
        {
            var operationItem = new OperationItem();

            Assert.ThrowsException<NotImplementedException>(() =>
            {
                var _ = operationItem.Length;
            });
        }
    }
}
