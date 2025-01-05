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
        public void Issue_205()
        {
            {
                var result = Program.Main(@"Test.Z80", "-f", "-bin", "Issue205.bin", "-hex", "Issue205.hex", "-cd", "./Test/Issues/205");
                Assert.AreEqual(0, result);
            }
            {
                var result = Program.Main(@"Test.Z80", "-f", "-bin", "Issue205.0000.bin", "-sa", "0", "-cd", "./Test/Issues/205");
                Assert.AreEqual(0, result);
            }
            {
                var result = Program.Main(@"Test.Z80", "-f", "-bin", "Issue205.0001.bin", "-sa", "0x0001", "-cd", "./Test/Issues/205");
                Assert.AreEqual(0, result);
            }
            {
                var result = Program.Main(@"Test.Z80", "-f", "-bin", "Issue205.0100.bin", "-sa", "0x0100", "-cd", "./Test/Issues/205");
                Assert.AreEqual(0, result);
            }

            Lib.AreSameLst(File.OpenRead("./Test/Issues/205/Issue205.hex"), File.OpenRead("./Test/Issues/205/Test.HEX"), Assembler.AsmEnum.FileTypeEnum.HEX);
            Lib.AreSameBin(File.OpenRead("./Test/Issues/205/Issue205.bin"), File.OpenRead("./Test/Issues/205/Test.BIN"), Assembler.AsmEnum.FileTypeEnum.BIN);
            Lib.AreSameBin(File.OpenRead("./Test/Issues/205/Issue205.0000.bin"), File.OpenRead("./Test/Issues/205/Test.0000.BIN"), Assembler.AsmEnum.FileTypeEnum.BIN);
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
        public void Issue_221()
        {
            Lib.Assemble_AreSame(Path.Combine("Issues", "221"));
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
        public void Issue_237()
        {
            var result = Program.Main(@"Test.Z80", "-f", "-bin", "Issue237.bin", "-lst", "Issue237.lst", "-sym", "Issue237.sym", "-equ", "Issue237.equ", "-cd", "./Test/Issues/237");
            Assert.AreEqual(0, result);
            Lib.AreSameBin(File.OpenRead("./Test/Issues/237/Issue237.bin"), File.OpenRead("./Test/Issues/237/Test.BIN"), Assembler.AsmEnum.FileTypeEnum.BIN);
            Lib.AreSameLst(File.OpenRead("./Test/Issues/237/Issue237.lst"), File.OpenRead("./Test/Issues/237/Test.LST"), Assembler.AsmEnum.FileTypeEnum.LST);
            Lib.AreSameLst(File.OpenRead("./Test/Issues/237/Issue237.sym"), File.OpenRead("./Test/Issues/237/Test.SYM"), Assembler.AsmEnum.FileTypeEnum.SYM);
            Lib.AreSameLst(File.OpenRead("./Test/Issues/237/Issue237.equ"), File.OpenRead("./Test/Issues/237/Test.EQU"), Assembler.AsmEnum.FileTypeEnum.EQU);
        }

        [TestMethod]
        public void Issue_238()
        {
            {
                var result = Program.Main(@"Test1.Z80", "-f", "-sym", "Issue238_min1.sym", "-cd", "./Test/Issues/238", "-sm", "minimal-equ");
                Assert.AreEqual(0, result);
                Lib.AreSameLst(File.OpenRead("./Test/Issues/238/Issue238_min1.sym"), File.OpenRead("./Test/Issues/238/Test_min1.SYM"), Assembler.AsmEnum.FileTypeEnum.SYM);
            }
            {
                var result = Program.Main(@"Test1.Z80", "-f", "-sym", "Issue238_nor1.sym", "-cd", "./Test/Issues/238", "-sm", "normal");
                Assert.AreEqual(0, result);
                Lib.AreSameLst(File.OpenRead("./Test/Issues/238/Issue238_nor1.sym"), File.OpenRead("./Test/Issues/238/Test_nor1.SYM"), Assembler.AsmEnum.FileTypeEnum.SYM);
            }
            {
                var result = Program.Main(@"Test2.Z80", "-f", "-sym", "Issue238_min2.sym", "-cd", "./Test/Issues/238", "-sm", "minimal-equ");
                Assert.AreEqual(0, result);
                Lib.AreSameLst(File.OpenRead("./Test/Issues/238/Issue238_min2.sym"), File.OpenRead("./Test/Issues/238/Test_min2.SYM"), Assembler.AsmEnum.FileTypeEnum.SYM);
            }
            {
                var result = Program.Main(@"Test2.Z80", "-f", "-sym", "Issue238_nor2.sym", "-cd", "./Test/Issues/238", "-sm", "normal");
                Assert.AreEqual(0, result);
                Lib.AreSameLst(File.OpenRead("./Test/Issues/238/Issue238_nor2.sym"), File.OpenRead("./Test/Issues/238/Test_nor2.SYM"), Assembler.AsmEnum.FileTypeEnum.SYM);
            }
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
        public void Issue_271()
        {
            var result = Program.Main(@"Test.Z80", "-f", "-bin", "Issue271.bin", "-lst", "Issue271.lst", "-sym", "Issue271.sym", "-equ", "Issue271.equ", "-cd", "./Test/Issues/271", "-dl", "BASE.TEST.OFFSET=1", "TEST1=10", "TEST2=20", "TEST3");
            Assert.AreEqual(0, result);
            Lib.AreSameBin(File.OpenRead("./Test/Issues/271/Issue271.bin"), File.OpenRead("./Test/Issues/271/Test.BIN"), Assembler.AsmEnum.FileTypeEnum.BIN);
            Lib.AreSameLst(File.OpenRead("./Test/Issues/271/Issue271.lst"), File.OpenRead("./Test/Issues/271/Test.LST"), Assembler.AsmEnum.FileTypeEnum.LST);
            Lib.AreSameLst(File.OpenRead("./Test/Issues/271/Issue271.sym"), File.OpenRead("./Test/Issues/271/Test.SYM"), Assembler.AsmEnum.FileTypeEnum.SYM);
            Lib.AreSameLst(File.OpenRead("./Test/Issues/271/Issue271.equ"), File.OpenRead("./Test/Issues/271/Test.EQU"), Assembler.AsmEnum.FileTypeEnum.EQU);
        }

        [TestMethod]
        public void Issue_272()
        {
            Lib.Assemble_AreSame(Path.Combine("Issues", "272"));
        }

        [TestMethod]
        public void Issue_273()
        {
            Lib.Assemble_AreSame(Path.Combine("Issues", "273"));
        }

        [TestMethod]
        public void Issue_274()
        {
            Lib.Assemble_AreSame(Path.Combine("Issues", "274"));
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

        [TestMethod]
        public void Issue_291()
        {
            var result = Program.Main(@"Test.Z80", "-f", "-bin", "Issue291.bin", "-lst", "Issue291.LST", "-err", "Issue291.ERR", "-cd", "./Test/Issues/291");
            Assert.AreEqual(1, result);
            Lib.AreSameLst(File.OpenRead("./Test/Issues/291/Issue291.LST"), File.OpenRead("./Test/Issues/291/Test.LST"), Assembler.AsmEnum.FileTypeEnum.LST);
            Lib.AreSameLst(File.OpenRead("./Test/Issues/291/Issue291.ERR"), File.OpenRead("./Test/Issues/291/Test.ERR"), Assembler.AsmEnum.FileTypeEnum.ERR);
        }

        [TestMethod]
        public void Issue_294()
        {
            var result = Program.Main(@"Test.Z80", "-f", "-bin", "Issue294.bin", "-lst", "Issue294.LST", "-err", "Issue294.ERR", "-cd", "./Test/Issues/294");
            Assert.AreEqual(1, result);
            Lib.AreSameLst(File.OpenRead("./Test/Issues/294/Issue294.LST"), File.OpenRead("./Test/Issues/294/Test.LST"), Assembler.AsmEnum.FileTypeEnum.LST);
            Lib.AreSameLst(File.OpenRead("./Test/Issues/294/Issue294.ERR"), File.OpenRead("./Test/Issues/294/Test.ERR"), Assembler.AsmEnum.FileTypeEnum.ERR);
        }

        [TestMethod]
        public void Issue_305()
        {
            Lib.Assemble_AreSame(Path.Combine("Issues", "305"));
        }

        [TestMethod]
        public void Issue_311()
        {
            Lib.Assemble_AreSame(Path.Combine("Issues", "311"));
        }

        [TestMethod]
        public void Issue_322_1()
        {
            var result = Program.Main(@"Test.Z80", "-f", "-bin", "Issue322_1.BIN", "-lst", "Issue322_1.LST", "-equ", "Issue322_1.EQU", "-dl", "LOADER.SCRSIZE=$1234", "ABC=123", "-cd", "./Test/Issues/322_1");
            Assert.AreEqual(0, result);
            Lib.AreSameBin(File.OpenRead("./Test/Issues/322_1/Issue322_1.BIN"), File.OpenRead("./Test/Issues/322_1/Test.BIN"), Assembler.AsmEnum.FileTypeEnum.BIN);
            Lib.AreSameLst(File.OpenRead("./Test/Issues/322_1/Issue322_1.LST"), File.OpenRead("./Test/Issues/322_1/Test.LST"), Assembler.AsmEnum.FileTypeEnum.LST);
            Lib.AreSameLst(File.OpenRead("./Test/Issues/322_1/Issue322_1.EQU"), File.OpenRead("./Test/Issues/322_1/Test.EQU"), Assembler.AsmEnum.FileTypeEnum.EQU);
        }

        [TestMethod]
        public void Issue_322_2()
        {
            var result = Program.Main(@"Test.Z80", "-f", "-bin", "Issue322_2.BIN", "-lst", "Issue322_2.LST", "-equ", "Issue322_2.EQU", "-dl", "LOADER.SCRSIZE=$1234", "-cd", "./Test/Issues/322_2");
            Assert.AreEqual(0, result);
            Lib.AreSameBin(File.OpenRead("./Test/Issues/322_2/Issue322_2.BIN"), File.OpenRead("./Test/Issues/322_2/Test.BIN"), Assembler.AsmEnum.FileTypeEnum.BIN);
            Lib.AreSameLst(File.OpenRead("./Test/Issues/322_2/Issue322_2.LST"), File.OpenRead("./Test/Issues/322_2/Test.LST"), Assembler.AsmEnum.FileTypeEnum.LST);
            Lib.AreSameLst(File.OpenRead("./Test/Issues/322_2/Issue322_2.EQU"), File.OpenRead("./Test/Issues/322_2/Test.EQU"), Assembler.AsmEnum.FileTypeEnum.EQU);
        }

        [TestMethod]
        public void Issue_323()
        {
            Lib.Assemble_AreSame(Path.Combine("Issues", "323"));
        }

        [TestMethod]
        public void Issue_327()
        {
            var result = Program.Main(@"Test.Z80", "-f", "-bin", "Issue327.BIN", "-lst", "Issue327.LST", "-err", "Issue327.ERR", "-ul", "-cd", "./Test/Issues/327");
            Assert.AreEqual(0, result);
            Lib.AreSameBin(File.OpenRead("./Test/Issues/327/Issue327.BIN"), File.OpenRead("./Test/Issues/327/Test.BIN"), Assembler.AsmEnum.FileTypeEnum.BIN);
            Lib.AreSameLst(File.OpenRead("./Test/Issues/327/Issue327.LST"), File.OpenRead("./Test/Issues/327/Test.LST"), Assembler.AsmEnum.FileTypeEnum.LST);
            Lib.AreSameLst(File.OpenRead("./Test/Issues/327/Issue327.ERR"), File.OpenRead("./Test/Issues/327/Test.ERR"), Assembler.AsmEnum.FileTypeEnum.ERR);
        }

        [TestMethod]
        public void Issue_333()
        {
            var result = Program.Main(@"Test.Z80", "-f", "-bin", "Issue333.BIN", "-lst", "Issue333.LST", "-err", "Issue333.ERR", "-cd", "./Test/Issues/333");
            Assert.AreEqual(1, result);
            Lib.AreSameLst(File.OpenRead("./Test/Issues/333/Issue333.LST"), File.OpenRead("./Test/Issues/333/Test.LST"), Assembler.AsmEnum.FileTypeEnum.LST);
            Lib.AreSameLst(File.OpenRead("./Test/Issues/333/Issue333.ERR"), File.OpenRead("./Test/Issues/333/Test.ERR"), Assembler.AsmEnum.FileTypeEnum.ERR);
        }

        [TestMethod]
        public void Issue_334()
        {
            Lib.Assemble_AreSame(Path.Combine("Issues", "334"));
        }

        [TestMethod]
        public void Issue_336()
        {
            Lib.Assemble_AreSame(Path.Combine("Issues", "336"));
        }

        [TestMethod]
        public void Issue_338()
        {
            Lib.Assemble_AreSame(Path.Combine("Issues", "338"));
        }

        [TestMethod]
        public void Issue_355()
        {
            Lib.Assemble_AreSame(Path.Combine("Issues", "355"));
        }

        [TestMethod]
        public void Issue_356()
        {
            Lib.Assemble_AreSame(Path.Combine("Issues", "356"));
        }

        [TestMethod]
        public void Issue_365()
        {
            Lib.Assemble_AreSame(Path.Combine("Issues", "365"));
        }

        [TestMethod]
        public void Issue_368()
        {
            Lib.Assemble_AreSame(Path.Combine("Issues", "368"));
        }

        [TestMethod]
        public void Issue_369()
        {
            var result = Program.Main(@"Test.Z80", "-f", "-bin", "Issue369.BIN", "-lst", "Issue369.LST", "-cd", "./Test/Issues/369");
            Assert.AreEqual(0, result);
            Lib.AreSameLst(File.OpenRead("./Test/Issues/369/Issue369.LST"), File.OpenRead("./Test/Issues/369/Test.LST"), Assembler.AsmEnum.FileTypeEnum.LST);
        }

        [TestMethod]
        public void Issue_370()
        {
            Lib.Assemble_AreSame(Path.Combine("Issues", "370"));
        }

        [TestMethod]
        public void Issue_376()
        {
            Lib.Assemble_AreSame(Path.Combine("Issues", "376"));
        }
    }
}
