using AILZ80ASM.Assembler;
using AILZ80ASM.CharMaps;
using AILZ80ASM.Exceptions;
using AILZ80ASM.LineDetailItems.ScopeItem.ExpansionItems;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace AILZ80ASM.Test
{
    [TestClass]
    public class CharMapsTest
    {
        [TestMethod]
        public void ConvertToBytesExceptionTest()
        {
            var charMapConverter = new CharMapConverter();
            Assert.ThrowsException<CharMapNotFoundException>(() => 
            {
                charMapConverter.ConvertToBytes("@ConvertToBytes", "石野");
            });
        }

        [TestMethod]
        public void ReadCharMapFromFileExceptionText()
        {
            var asmLoad = new AsmLoad(new AsmOption(), new InstructionSet.Z80());
            var charMapConverter = new CharMapConverter();

            Assert.ThrowsException<ArgumentException>(() =>
            {
                charMapConverter.ReadCharMapFromFile("", "", asmLoad);
            });

            Assert.ThrowsException<ArgumentException>(() =>
            {
                charMapConverter.ReadCharMapFromFile("+-", "", asmLoad);
            });

            Assert.ThrowsException<FileNotFoundException>(() =>
            {
                charMapConverter.ReadCharMapFromFile("@ReadCharMapFromFileExceptionText", "", asmLoad);
            });
        }

        [TestMethod]
        public void ReadCharMapFromResourceExceptionText()
        {
            var asmLoad = new AsmLoad(new AsmOption(), new InstructionSet.Z80());
            var charMapConverter = new CharMapConverter();

            Assert.ThrowsException<ArgumentException>(() =>
            {
                charMapConverter.ReadCharMapFromResource("", asmLoad);
            });

            Assert.ThrowsException<ArgumentException>(() =>
            {
                charMapConverter.ReadCharMapFromResource("+-", asmLoad);
            });
        }
    }
}
