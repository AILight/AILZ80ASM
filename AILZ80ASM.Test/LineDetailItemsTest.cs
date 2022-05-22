using AILZ80ASM.Assembler;
using AILZ80ASM.Exceptions;
using AILZ80ASM.LineDetailItems;
using AILZ80ASM.LineDetailItems.ScopeItem.ExpansionItems;
using AILZ80ASM.OperationItems;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace AILZ80ASM.Test
{
    [TestClass]
    public class LineDetailItemsTest
    {
        [TestMethod]
        public void LineDetailExpansionItem_get_BinResults()
        {
            var lineDetailExpansionItem = new LineDetailExpansionItem(default(LineItem));

            Assert.AreEqual(lineDetailExpansionItem.BinResults, Array.Empty<AsmResult>());
        }

        [TestMethod]
        public void LineDetailExpansionItem_get_List()
        {
            var lineDetailExpansionItem = new LineDetailExpansionItem(default(LineItem));

            Assert.AreEqual(lineDetailExpansionItem.List, default(AsmList));
        }

        [TestMethod]
        public void LineDetailExpansionItemOperation_PreAssemble()
        {
            var asmLoad = new AsmLoad(new AsmOption(), new InstructionSet.Z80());
            var lineItem = new LineItem("\tLD G,0", 1, default(FileInfo));
            var operationItem = LineDetailItemOperation.Create(new LineItem(lineItem), asmLoad);

            Assert.IsNull(operationItem);
        }
    }
}
