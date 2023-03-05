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
    public class Z80ZZIssuesTest
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
            Lib.Assemble_AreSame(Path.Combine("Issues", "141"));
        }

        [TestMethod]
        public void Issue_142_1()
        {
            var errors = Assemble(Path.Combine("Issues", "142_1"), "Test.Z80");

            Assert.AreEqual(1, errors.Length);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0004, 10, "Test.Z80", errors);
        }

        [TestMethod]
        public void Issue_142_2()
        {
            Lib.Assemble_AreSame(Path.Combine("Issues", "142_2"));
        }

        [TestMethod]
        public void Issue_145()
        {
            var errors = Assemble(Path.Combine("Issues", "145"), "Test.Z80");

            Assert.AreEqual(1, errors.Length);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0009, 108, "Test.Z80", errors);
        }

        [TestMethod]
        public void Issue_171()
        {
            var result = Program.Main(@"Test.Z80", "-f", "-lst", "Issue171.lst", "-err", "Issue171.err", "-cd", "./Test/Issues/171");
            Assert.AreEqual(0, result);

            Lib.AreSameLst(File.OpenRead("./Test/Issues/171/Issue171.lst"), File.OpenRead("./Test/Issues/171/Test.LST"), Assembler.AsmEnum.FileTypeEnum.LST);
            Lib.AreSameLst(File.OpenRead("./Test/Issues/171/Issue171.err"), File.OpenRead("./Test/Issues/171/Test.ERR"), Assembler.AsmEnum.FileTypeEnum.ERR);
        }

        [TestMethod]
        public void Issue_172()
        {
            var errors = Assemble(Path.Combine("Issues", "172"), "Test.Z80");

            Assert.AreEqual(2, errors.Length);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0013, 4, "Test.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0013, 5, "Test.Z80", errors);
        }

        [TestMethod]
        public void Issue_175()
        {
            Lib.Assemble_AreSame(Path.Combine("Issues", "175"));
        }

        [TestMethod]
        public void Issue_181()
        {
            var errors = Assemble(Path.Combine("Issues", "181"), "Test.Z80");

            Assert.AreEqual(3, errors.Length);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0007, 3, "Test.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0007, 5, "Test.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0007, 6, "Test.Z80", errors);
        }

        [TestMethod]
        public void Issue_187()
        {
            Lib.Assemble_AreSame(Path.Combine("Issues", "187"));
        }
    }
}
