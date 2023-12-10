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
        [TestMethod]
        public void Issue_141()
        {
            Lib.Assemble_AreSame(Path.Combine("Issues", "141"));
        }

        [TestMethod]
        public void Issue_142_1()
        {
            var errors = Lib.Assemble(Path.Combine("Issues", "142_1"), "Test.Z80");

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
            var errors = Lib.Assemble(Path.Combine("Issues", "145"), "Test.Z80");

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
            var errors = Lib.Assemble(Path.Combine("Issues", "172"), "Test.Z80");

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
            var errors = Lib.Assemble(Path.Combine("Issues", "181"), "Test.Z80");

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

        [TestMethod]
        public void Issue_201()
        {
            Lib.Assemble_AreSame(Path.Combine("Issues", "201"));
        }

        [TestMethod]
        public void Issue_207()
        {
            var result = Program.Main(@"Test.Z80", "-f", "-bin", "Issue207.bin", "-sym", "Issue207.sym", "-cd", "./Test/Issues/207");
            Assert.AreEqual(0, result);

            Lib.AreSameLst(File.OpenRead("./Test/Issues/207/Issue207.sym"), File.OpenRead("./Test/Issues/207/Test.SYM"), Assembler.AsmEnum.FileTypeEnum.SYM);
            Lib.AreSameBin(File.OpenRead("./Test/Issues/207/Issue207.bin"), File.OpenRead("./Test/Issues/207/Test.BIN"), Assembler.AsmEnum.FileTypeEnum.BIN);
        }

        [TestMethod]
        public void Issue_208()
        {
            Lib.Assemble_AreSame(Path.Combine("Issues", "208"));
        }

        [TestMethod]
        public void Issue_214()
        {
            Lib.Assemble_AreSame(Path.Combine("Issues", "214"));
        }

        [TestMethod]
        public void Issue_215()
        {
            var result = Program.Main(@"Test.Z80", "-f", "-bin", "Issue215.bin", "-sym", "Issue215.sym", "-cd", "./Test/Issues/215");
            Assert.AreEqual(0, result);
            Lib.AreSameBin(File.OpenRead("./Test/Issues/215/Issue215.bin"), File.OpenRead("./Test/Issues/215/Test.BIN"), Assembler.AsmEnum.FileTypeEnum.BIN);
            Lib.AreSameLst(File.OpenRead("./Test/Issues/215/Issue215.sym"), File.OpenRead("./Test/Issues/215/Test.SYM"), Assembler.AsmEnum.FileTypeEnum.SYM);
        }

        [TestMethod]
        public void Issue_222()
        {
            var result = Program.Main(@"Test.Z80", "-f", "-bin", "Issue222.bin", "-lst", "Issue222.lst", "-cd", "./Test/Issues/222");
            Assert.AreEqual(0, result);
            Lib.AreSameBin(File.OpenRead("./Test/Issues/222/Issue222.bin"), File.OpenRead("./Test/Issues/222/Test.BIN"), Assembler.AsmEnum.FileTypeEnum.BIN);
            Lib.AreSameLst(File.OpenRead("./Test/Issues/222/Issue222.lst"), File.OpenRead("./Test/Issues/222/Test.LST"), Assembler.AsmEnum.FileTypeEnum.LST);
        }

        [TestMethod]
        public void Issue_223()
        {
            Lib.Assemble_AreSame(Path.Combine("Issues", "223"));
        }

        [TestMethod]
        public void Issue_230()
        {
            var result = Program.Main(@"Test.Z80", "-f", "-bin", "Issue230.bin", "-lst", "Issue230.lst", "-lob", "-cd", "./Test/Issues/230");
            Assert.AreEqual(0, result);
            Lib.AreSameBin(File.OpenRead("./Test/Issues/230/Issue230.bin"), File.OpenRead("./Test/Issues/230/Test.BIN"), Assembler.AsmEnum.FileTypeEnum.BIN);
            Lib.AreSameLst(File.OpenRead("./Test/Issues/230/Issue230.lst"), File.OpenRead("./Test/Issues/230/Test.LST"), Assembler.AsmEnum.FileTypeEnum.LST);
        }

        [TestMethod]
        public void Issue_231()
        {
            var result = Program.Main(@"Test.Z80", "-f", "-bin", "Issue231.bin", "-lst", "Issue231.lst", "-cd", "./Test/Issues/231");
            Assert.AreEqual(1, result);
            Lib.AreSameLst(File.OpenRead("./Test/Issues/231/Issue231.lst"), File.OpenRead("./Test/Issues/231/Test.LST"), Assembler.AsmEnum.FileTypeEnum.LST);
        }

        [TestMethod]
        public void Issue_232()
        {
            var result = Program.Main(@"Test.Z80", "-f", "-bin", "Issue232.bin", "-cmt", "Issue232.cmt", "-cd", "./Test/Issues/232");
            Assert.AreEqual(0, result);
            Lib.AreSameBin(File.OpenRead("./Test/Issues/232/Issue232.bin"), File.OpenRead("./Test/Issues/232/Test.BIN"), Assembler.AsmEnum.FileTypeEnum.BIN);
            Lib.AreSameBin(File.OpenRead("./Test/Issues/232/Issue232.cmt"), File.OpenRead("./Test/Issues/232/Test.CMT"), Assembler.AsmEnum.FileTypeEnum.CMT);
        }

        [TestMethod]
        public void Issue_257()
        {
            Lib.Assemble_AreSame(Path.Combine("Issues", "257"));
        }

        [TestMethod]
        public void Issue_260()
        {
            Lib.Assemble_AreSame(Path.Combine("Issues", "260"));
        }

        [TestMethod]
        public void Issue_262()
        {
            Lib.Assemble_AreSame(Path.Combine("Issues", "260"));
        }

        [TestMethod]
        public void Issue_273()
        {
            Lib.Assemble_AreSame(Path.Combine("Issues", "273"));
        }

        [TestMethod]
        public void Issue_276()
        {
            Lib.Assemble_AreSame(Path.Combine("Issues", "276"));
        }

        [TestMethod]
        public void Issue_281()
        {
            Lib.Assemble_AreSame(Path.Combine("Issues", "281"));
        }
    }
}
