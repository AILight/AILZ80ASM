using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Reflection;

namespace AILZ80ASM.Test
{
    [TestClass]
    public class SSMainTest
    {
        [TestMethod]
        public void TestMainSuccess()
        {
            var outputFiles = new []{ "./Test/TestSS_Main/TestMainSuccess.bin", "./Test/TestSS_Main/TestMainSuccess.cmt", "./Test/TestSS_Main/TestMainSuccess.err" };
            foreach (var item in outputFiles)
            {
                if (File.Exists(item))
                {
                    File.Delete(item);
                }
            }

            var result = Program.Main(@"Success.Z80", "-bin", "TestMainSuccess.bin", "-cmt", "TestMainSuccess.cmt", "-err", "TestMainSuccess.err", "-cd", "./Test/TestSS_Main/");

            Assert.AreEqual(0, result);
            foreach (var item in outputFiles)
            {
                Assert.IsTrue(File.Exists(item), item);
            }
        }

        [TestMethod]
        public void TestMainSuccess_SJIS()
        {
            var result_AUTO = Program.Main(@"Success_SJIS.Z80", "-lst", "Success_SJIS_AUTO.lst", "-cd", "./Test/TestSS_Main/");
            Assert.AreEqual(0, result_AUTO);

            var result_TAB8 = Program.Main(@"Success_SJIS.Z80", "-ts", "8", "-lst", "Success_SJIS_AUTO_TAB8.lst", "-cd", "./Test/TestSS_Main/");
            Assert.AreEqual(0, result_TAB8);

            var result_SJIS = Program.Main(@"Success_SJIS.Z80", "-ie", "shift_jis", "-oe", "shift_jis", "-ts", "8", "-lst", "Success_SJIS_SJIS_TAB8.lst", "-cd", "./Test/TestSS_Main/");
            Assert.AreEqual(0, result_SJIS);

            Lib.AreSameLst(File.OpenRead("./Test/TestSS_Main/Success_SJIS_AUTO_ORG.lst"), File.OpenRead("./Test/TestSS_Main/Success_SJIS_AUTO.lst"), Assembler.AsmEnum.FileTypeEnum.LST);
            Lib.AreSameLst(File.OpenRead("./Test/TestSS_Main/Success_SJIS_AUTO_TAB8_ORG.lst"), File.OpenRead("./Test/TestSS_Main/Success_SJIS_AUTO_TAB8.lst"), Assembler.AsmEnum.FileTypeEnum.LST);
            Lib.AreSameLst(File.OpenRead("./Test/TestSS_Main/Success_SJIS_SJIS_TAB8_ORG.lst"), File.OpenRead("./Test/TestSS_Main/Success_SJIS_SJIS_TAB8.lst"), Assembler.AsmEnum.FileTypeEnum.LST);

        }

        [TestMethod]
        public void TestMainSuccessAndChangeDirectory()
        {
            var outputFiles = new[] { ".\\Test\\TestSS_Main\\TestMainSuccessAndChangeDirectory.bin", ".\\Test\\TestSS_Main\\TestMainSuccessAndChangeDirectory.err" };
            foreach (var item in outputFiles)
            {
                if (File.Exists(item))
                {
                    File.Delete(item);
                }
            }

            var result = Program.Main(@"Success.Z80", "-bin", "TestMainSuccessAndChangeDirectory.bin", "-err", "TestMainSuccessAndChangeDirectory.err", "-cd", "./Test/TestSS_Main/");

            Assert.AreEqual(0, result);
            foreach (var item in outputFiles)
            {
                Assert.IsTrue(File.Exists(item), item);
            }
        }

        [TestMethod]
        public void TestMainSuccessAllOption()
        {
            var outputFiles = new[] { "./Test/TestSS_Main/TestMainSuccessAllOption.bin", "./Test/TestSS_Main/TestMainSuccessAllOption.err", "./Test/TestSS_Main/TestMainSuccessAllOption.lst", "./Test/TestSS_Main/TestMainSuccessAllOption.equ", "./Test/TestSS_Main/TestMainSuccessAllOption.adr", "./Test/TestSS_Main/TestMainSuccessAllOption.tag" };
            foreach (var item in outputFiles)
            {
                if (File.Exists(item))
                {
                    File.Delete(item);
                }
            }

            var result = Program.Main(@"Success.Z80", "-bin", "TestMainSuccessAllOption.bin", "-err", "TestMainSuccessAllOption.err", "-lst", "TestMainSuccessAllOption.lst", "-equ", "TestMainSuccessAllOption.equ", "-adr", "TestMainSuccessAllOption.adr", "-tag", "TestMainSuccessAllOption.tag", "-cd", "./Test/TestSS_Main/");
            Assert.AreEqual(0, result);
            foreach (var item in outputFiles)
            {
                Assert.IsTrue(File.Exists(item), item);
            }
        }

        [TestMethod]
        public void TestMainNoOption()
        {
            var result = Program.Main();
            Assert.AreEqual(2, result);
        }

        [TestMethod]
        public void TestReadMeTest()
        {
            {
                var result = Program.Main(@"-??");
                Assert.AreEqual(2, result);
            }

            {
                var result = Program.Main(@"--readme");
                Assert.AreEqual(2, result);
            }
        }

        [TestMethod]
        public void TestErrorSuccess()
        {
            var result = Program.Main(@"./Test/TestSS_Main/Error.Z80", "-bin", "TestErrorSuccess.bin", "-err", "TestErrorSuccess.err");
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void TestErrorArgument()
        {
            var result = Program.Main(@"./Test/TestSS_Main/Error.Z80", "-bin1", "TestErrorArgument.bin");
            Assert.AreEqual(2, result);
        }

        [TestMethod]
        public void TestErrorWrtiteFile()
        {
            using (var stream = new FileStream("TestErrorWrtiteFile.bin", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read))
            {
                var result = Program.Main(@"./Test/TestSS_Main/Success.Z80", "-bin", "TestErrorWrtiteFile.bin", "-err", "TestErrorWrtiteFile.err", "-lst", "TestErrorWrtiteFile.lst", "-equ", "TestErrorWrtiteFile.equ");
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestArgumentValidate()
        {
            {
                var result = Program.Main("TestArgumentValidate.z80", "-bin", "TestArgumentValidate.bin");
                Assert.AreEqual(3, result);
            }

            {
                var result = Program.Main("-bin", "TestArgumentValidate.bin");
                Assert.AreEqual(2, result);
            }

            {
                var result = Program.Main("Success.Z80", "-bin", "Success.Z80");
                Assert.AreEqual(3, result);
            }

            {
                var result = Program.Main("Success.Z80", "-bin", "Success.Z80", "-dw", "E0001");
                Assert.AreEqual(2, result);
            }
        }

        [TestMethod]
        public void TestDiffFile()
        {
            {
                var result = Program.Main(@"./Test/TestSS_Main/Success_df1.Z80", "-bin", "TestDiffFile.bin", "-lst", "TestDiffFile.lst");
                Assert.AreEqual(0, result);
            }

            {
                var result = Program.Main(@"./Test/TestSS_Main/Success_df1.Z80", "-bin", "TestDiffFile.bin", "-lst", "TestDiffFile.lst", "-df");
                Assert.AreEqual(0, result);
            }

            {
                var result = Program.Main(@"./Test/TestSS_Main/Success_df2.Z80", "-bin", "TestDiffFile.bin", "-lst", "TestDiffFile.lst", "-df");
                Assert.AreEqual(1, result);
            }

            {
                var result = Program.Main(@"./Test/TestSS_Main/Success_df3.Z80", "-bin", "TestDiffFile.bin", "-lst", "TestDiffFile.lst", "-df");
                Assert.AreEqual(1, result);
            }

            {
                var result = Program.Main(@"./Test/TestSS_Main/Success_df1.Z80", "-bin", "TestDiffFile_NotFound.bin", "-lst", "TestDiffFile_NotFound.lst", "-df");
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestList()
        {
            var result_simple = Program.Main(@"Success.Z80", "-lst", "List_simple.lst", "-lm", "simple", "-cd", "./Test/TestSS_Main/");
            Assert.AreEqual(0, result_simple);

            var result_middle = Program.Main(@"Success.Z80", "-lst", "List_middle.lst", "-lm", "middle", "-cd", "./Test/TestSS_Main/");
            Assert.AreEqual(0, result_middle);

            var result_full = Program.Main(@"Success.Z80", "-lst", "List_full.lst", "-lm", "full", "-cd", "./Test/TestSS_Main/");
            Assert.AreEqual(0, result_full);

            Lib.AreSameLst(File.OpenRead("./Test/TestSS_Main/List_full_ORG.lst"),   File.OpenRead("./Test/TestSS_Main/List_full.lst"), Assembler.AsmEnum.FileTypeEnum.LST);
            Lib.AreSameLst(File.OpenRead("./Test/TestSS_Main/List_middle_ORG.lst"), File.OpenRead("./Test/TestSS_Main/List_middle.lst"), Assembler.AsmEnum.FileTypeEnum.LST);
            Lib.AreSameLst(File.OpenRead("./Test/TestSS_Main/List_simple_ORG.lst"), File.OpenRead("./Test/TestSS_Main/List_simple.lst"), Assembler.AsmEnum.FileTypeEnum.LST);
        }


        [TestMethod]
        public void TestSymbol()
        {
            var result = Program.Main(@"Success.Z80", "-sym", "-cd", "./Test/TestSS_Main/");
            Assert.AreEqual(0, result);
            Lib.AreSameLst(File.OpenRead("./Test/TestSS_Main/Success_ORG.sym"), File.OpenRead("./Test/TestSS_Main/Success.sym"), Assembler.AsmEnum.FileTypeEnum.SYM);
        }

        [TestMethod]
        public void TestEqual()
        {
            var result = Program.Main(@"Success.Z80", "-equ", "-cd", "./Test/TestSS_Main/");
            Assert.AreEqual(0, result);
            Lib.AreSameLst(File.OpenRead("./Test/TestSS_Main/Success_ORG.equ"), File.OpenRead("./Test/TestSS_Main/Success.equ"), Assembler.AsmEnum.FileTypeEnum.EQU);
        }

        [TestMethod]
        public void TestAddress()
        {
            var result = Program.Main(@"Success.Z80", "-adr", "-cd", "./Test/TestSS_Main/");
            Assert.AreEqual(0, result);
            Lib.AreSameLst(File.OpenRead("./Test/TestSS_Main/Success_ORG.adr"), File.OpenRead("./Test/TestSS_Main/Success.adr"), Assembler.AsmEnum.FileTypeEnum.ADR);
        }

        [TestMethod]
        public void TestCMT()
        {
            var result = Program.Main(@"Success.Z80", "-cmt", "-cd", "./Test/TestSS_Main/");
            Assert.AreEqual(0, result);
            Lib.AreSameBin(File.OpenRead("./Test/TestSS_Main/Success_ORG.cmt"), File.OpenRead("./Test/TestSS_Main/Success.cmt"), Assembler.AsmEnum.FileTypeEnum.CMT);
        }

        [TestMethod]
        public void TestT88()
        {
            var result = Program.Main(@"Success.Z80", "-t88", "-cd", "./Test/TestSS_Main/");
            Assert.AreEqual(0, result);
            Lib.AreSameBin(File.OpenRead("./Test/TestSS_Main/Success_ORG.t88"), File.OpenRead("./Test/TestSS_Main/Success.t88"), Assembler.AsmEnum.FileTypeEnum.T88);
        }

        [TestMethod]
        public void TestBIN()
        {
            var result = Program.Main(@"Success.Z80", "-bin", "-cd", "./Test/TestSS_Main/");
            Assert.AreEqual(0, result);
            Lib.AreSameBin(File.OpenRead("./Test/TestSS_Main/Success_ORG.bin"), File.OpenRead("./Test/TestSS_Main/Success.bin"), Assembler.AsmEnum.FileTypeEnum.BIN);
        }

        [TestMethod]
        public void TestErr()
        {
            var result = Program.Main(@"Error.Z80", "-err", "-cd", "./Test/TestSS_Main/");
            Assert.AreEqual(1, result);
            Lib.AreSameLst(File.OpenRead("./Test/TestSS_Main/Error_ORG.err"), File.OpenRead("./Test/TestSS_Main/Error.err"), Assembler.AsmEnum.FileTypeEnum.ERR);
        }

        [TestMethod]
        public void TestTAG()
        {
            var result = Program.Main(@"Success.Z80", "-tag", "-cd", "./Test/TestSS_Main/");
            Assert.AreEqual(0, result);
            Lib.AreSameLst(File.OpenRead("./Test/TestSS_Main/Success_ORG.tag"), File.OpenRead("./Test/TestSS_Main/tags"), Assembler.AsmEnum.FileTypeEnum.TAG);
        }

        [TestMethod]
        public void TestErrList()
        {
            var result = Program.Main(@"ErrorList.Z80", "-err", "-lst", "-cd", "./Test/TestSS_Main/");
            Assert.AreEqual(1, result);
            Lib.AreSameLst(File.OpenRead("./Test/TestSS_Main/ErrorList_ORG.err"), File.OpenRead("./Test/TestSS_Main/ErrorList.err"), Assembler.AsmEnum.FileTypeEnum.ERR);
            Lib.AreSameLst(File.OpenRead("./Test/TestSS_Main/ErrorList_ORG.lst"), File.OpenRead("./Test/TestSS_Main/ErrorList.lst"), Assembler.AsmEnum.FileTypeEnum.LST);
        }

        [TestMethod]
        public void TestGapTest()
        {
            var result = Program.Main(@"GapTest.Z80", "-bin", "-gap", "$AA", "-cd", "./Test/TestSS_Main/");
            Assert.AreEqual(0, result);
            Lib.AreSameBin(File.OpenRead("./Test/TestSS_Main/GapTest.bin"), File.OpenRead("./Test/TestSS_Main/GapTest.bin"), Assembler.AsmEnum.FileTypeEnum.BIN);
        }

        [TestMethod]
        public void TestHexTest()
        {
            var result = Program.Main(@"HEXTest.Z80", "-hex", "-cd", "./Test/TestSS_Main/");
            Assert.AreEqual(0, result);
            Lib.AreSameLst(File.OpenRead("./Test/TestSS_Main/HEXTest_ORG.hex"), File.OpenRead("./Test/TestSS_Main/HEXTest.hex"), Assembler.AsmEnum.FileTypeEnum.HEX);
        }
    }
}
