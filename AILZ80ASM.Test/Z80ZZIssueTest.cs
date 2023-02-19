using AILZ80ASM.Assembler;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AILZ80ASM.Test
{
    [TestClass]
    public class Z80ZZIssueTest
    {
        private ErrorLineItem[] Assemble(string direcotryName, string fileName)
        {
            var targetDirectoryName = Path.Combine(".", "Test", direcotryName);
            var inputFiles = new[] { new FileInfo(Path.Combine(targetDirectoryName, fileName)) };
            var outputFiles = new System.Collections.Generic.Dictionary<MemoryStream, System.Collections.Generic.KeyValuePair<Assembler.AsmEnum.FileTypeEnum, FileInfo>>();

            return Lib.Assemble(inputFiles, outputFiles, true);
        }

        [TestMethod]
        public void Issue_141()
        {
            Lib.Assemble_AreSame(Path.Combine("Issue", "141"));
        }

        [TestMethod]
        public void Issue_142()
        {
            var errors = Assemble(Path.Combine("Issue", "142"), "Test.Z80");

            Assert.AreEqual(1, errors.Length);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0004, 10, "Test.Z80", errors);
        }

        [TestMethod]
        public void Issue_145()
        {
            var errors = Assemble(Path.Combine("Issue", "145"), "Test.Z80");

            Assert.AreEqual(1, errors.Length);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0009, 108, "Test.Z80", errors);
        }

        [TestMethod]
        public void Issue_181()
        {
            var errors = Assemble(Path.Combine("Issue", "181"), "Test.Z80");

            Assert.AreEqual(3, errors.Length);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0007, 3, "Test.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0007, 5, "Test.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0007, 6, "Test.Z80", errors);
        }
    }
}
