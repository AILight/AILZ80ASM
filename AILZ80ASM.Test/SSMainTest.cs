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

            Assert.AreEqual(result, 0);
            foreach (var item in outputFiles)
            {
                Assert.IsTrue(File.Exists(item), item);
            }
        }

        [TestMethod]
        public void TestMainSuccess_SJIS()
        {
            var result_AUTO = Program.Main(@"Success_SJIS.Z80", "-lst", "Success_SJIS_AUTO.lst", "-cd", "./Test/TestSS_Main/");
            Assert.AreEqual(result_AUTO, 0);

            var result_TAB8 = Program.Main(@"Success_SJIS.Z80", "-ts", "8", "-lst", "Success_SJIS_AUTO_TAB8.lst", "-cd", "./Test/TestSS_Main/");
            Assert.AreEqual(result_TAB8, 0);

            var result_SJIS = Program.Main(@"Success_SJIS.Z80", "-ie", "shift_jis", "-oe", "shift_jis", "-ts", "8", "-lst", "Success_SJIS_SJIS_TAB8.lst", "-cd", "./Test/TestSS_Main/");
            Assert.AreEqual(result_SJIS, 0);

            Lib.AreSameLst(File.OpenRead("./Test/TestSS_Main/Success_SJIS_AUTO.lst"), File.OpenRead("./Test/TestSS_Main/Success_SJIS_AUTO_ORG.lst"), Assembler.AsmEnum.FileTypeEnum.LST);
            Lib.AreSameLst(File.OpenRead("./Test/TestSS_Main/Success_SJIS_AUTO_TAB8.lst"), File.OpenRead("./Test/TestSS_Main/Success_SJIS_AUTO_TAB8_ORG.lst"), Assembler.AsmEnum.FileTypeEnum.LST);
            Lib.AreSameLst(File.OpenRead("./Test/TestSS_Main/Success_SJIS_SJIS_TAB8.lst"), File.OpenRead("./Test/TestSS_Main/Success_SJIS_SJIS_TAB8_ORG.lst"), Assembler.AsmEnum.FileTypeEnum.LST);

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

            Assert.AreEqual(result, 0);
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
            Assert.AreEqual(result, 0);
            foreach (var item in outputFiles)
            {
                Assert.IsTrue(File.Exists(item), item);
            }
        }

        [TestMethod]
        public void TestMainNoOption()
        {
            var result = Program.Main();
            Assert.AreEqual(result, 2);
        }

        [TestMethod]
        public void TestReadMeTest()
        {
            {
                var result = Program.Main(@"-??");
                Assert.AreEqual(result, 2);
            }

            {
                var result = Program.Main(@"--readme");
                Assert.AreEqual(result, 2);
            }
        }

        [TestMethod]
        public void TestErrorSuccess()
        {
            var result = Program.Main(@"./Test/TestSS_Main/Error.Z80", "-bin", "TestErrorSuccess.bin", "-err", "TestErrorSuccess.err");
            Assert.AreEqual(result, 1);
        }

        [TestMethod]
        public void TestErrorArgument()
        {
            var result = Program.Main(@"./Test/TestSS_Main/Error.Z80", "-bin1", "TestErrorArgument.bin");
            Assert.AreEqual(result, 2);
        }

        [TestMethod]
        public void TestErrorWrtiteFile()
        {
            using (var stream = new FileStream("TestErrorWrtiteFile.bin", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read))
            {
                var result = Program.Main(@"./Test/TestSS_Main/Success.Z80", "-bin", "TestErrorWrtiteFile.bin", "-err", "TestErrorWrtiteFile.err", "-lst", "TestErrorWrtiteFile.lst", "-equ", "TestErrorWrtiteFile.equ");
                Assert.AreEqual(result, 1);
            }
        }

        [TestMethod]
        public void TestDiffFile()
        {
            var result_A = Program.Main(@"./Test/TestSS_Main/Success_df1.Z80", "-bin", "TestDiffFile.bin", "-lst", "TestDiffFile.lst");
            Assert.AreEqual(result_A, 0);
            
            var result_B = Program.Main(@"./Test/TestSS_Main/Success_df1.Z80", "-bin", "TestDiffFile.bin", "-lst", "TestDiffFile.lst", "-df");
            Assert.AreEqual(result_B, 0);
            
            var result_C = Program.Main(@"./Test/TestSS_Main/Success_df2.Z80", "-bin", "TestDiffFile.bin", "-lst", "TestDiffFile.lst", "-df");
            Assert.AreEqual(result_C, 1);
        }

        [TestMethod]
        public void TestList()
        {
            var result_simple = Program.Main(@"Success.Z80", "-lst", "List_simple.lst", "-lm", "simple", "-cd", "./Test/TestSS_Main/");
            Assert.AreEqual(result_simple, 0);

            var result_middle = Program.Main(@"Success.Z80", "-lst", "List_middle.lst", "-lm", "middle", "-cd", "./Test/TestSS_Main/");
            Assert.AreEqual(result_middle, 0);

            var result_full = Program.Main(@"Success.Z80", "-lst", "List_full.lst", "-lm", "full", "-cd", "./Test/TestSS_Main/");
            Assert.AreEqual(result_full, 0);

            Lib.AreSameLst(File.OpenRead("./Test/TestSS_Main/List_full.lst"), File.OpenRead("./Test/TestSS_Main/List_full_ORG.lst"), Assembler.AsmEnum.FileTypeEnum.LST);
            Lib.AreSameLst(File.OpenRead("./Test/TestSS_Main/List_middle.lst"), File.OpenRead("./Test/TestSS_Main/List_middle_ORG.lst"), Assembler.AsmEnum.FileTypeEnum.LST);
            Lib.AreSameLst(File.OpenRead("./Test/TestSS_Main/List_simple.lst"), File.OpenRead("./Test/TestSS_Main/List_simple_ORG.lst"), Assembler.AsmEnum.FileTypeEnum.LST);
        }


        [TestMethod]
        public void TestSymbol()
        {
            var result_simple = Program.Main(@"Success.Z80", "-sym", "-cd", "./Test/TestSS_Main/");
            Assert.AreEqual(result_simple, 0);
            Lib.AreSameLst(File.OpenRead("./Test/TestSS_Main/Success.sym"), File.OpenRead("./Test/TestSS_Main/Success_ORG.sym"), Assembler.AsmEnum.FileTypeEnum.SYM);
        }

        [TestMethod]
        public void TestEqual()
        {
            var result_simple = Program.Main(@"Success.Z80", "-equ", "-cd", "./Test/TestSS_Main/");
            Assert.AreEqual(result_simple, 0);
            Lib.AreSameLst(File.OpenRead("./Test/TestSS_Main/Success.equ"), File.OpenRead("./Test/TestSS_Main/Success_ORG.equ"), Assembler.AsmEnum.FileTypeEnum.EQU);
        }

        [TestMethod]
        public void TestAddress()
        {
            var result_simple = Program.Main(@"Success.Z80", "-adr", "-cd", "./Test/TestSS_Main/");
            Assert.AreEqual(result_simple, 0);
            Lib.AreSameLst(File.OpenRead("./Test/TestSS_Main/Success.adr"), File.OpenRead("./Test/TestSS_Main/Success_ORG.adr"), Assembler.AsmEnum.FileTypeEnum.ADR);
        }

        [TestMethod]
        public void TestCMT()
        {
            var result_simple = Program.Main(@"Success.Z80", "-cmt", "-cd", "./Test/TestSS_Main/");
            Assert.AreEqual(result_simple, 0);
            Lib.AreSameBin(File.OpenRead("./Test/TestSS_Main/Success.cmt"), File.OpenRead("./Test/TestSS_Main/Success_ORG.cmt"), Assembler.AsmEnum.FileTypeEnum.CMT);
        }

        [TestMethod]
        public void TestT88()
        {
            var result_simple = Program.Main(@"Success.Z80", "-t88", "-cd", "./Test/TestSS_Main/");
            Assert.AreEqual(result_simple, 0);
            Lib.AreSameBin(File.OpenRead("./Test/TestSS_Main/Success.t88"), File.OpenRead("./Test/TestSS_Main/Success_ORG.t88"), Assembler.AsmEnum.FileTypeEnum.T88);
        }

        [TestMethod]
        public void TestBIN()
        {
            var result_simple = Program.Main(@"Success.Z80", "-bin", "-cd", "./Test/TestSS_Main/");
            Assert.AreEqual(result_simple, 0);
            Lib.AreSameBin(File.OpenRead("./Test/TestSS_Main/Success.bin"), File.OpenRead("./Test/TestSS_Main/Success_ORG.bin"), Assembler.AsmEnum.FileTypeEnum.BIN);
        }

        [TestMethod]
        public void TestErr()
        {
            var result_simple = Program.Main(@"Error.Z80", "-err", "-cd", "./Test/TestSS_Main/");
            Assert.AreEqual(result_simple, 1);
            Lib.AreSameLst(File.OpenRead("./Test/TestSS_Main/Error.err"), File.OpenRead("./Test/TestSS_Main/Error_ORG.err"), Assembler.AsmEnum.FileTypeEnum.ERR);
        }

        [TestMethod]
        public void TestTAG()
        {
            var result_simple = Program.Main(@"Success.Z80", "-bin", "-tag", "-cd", "./Test/TestSS_Main/");
            Assert.AreEqual(result_simple, 0);
            Lib.AreSameBin(File.OpenRead("./Test/TestSS_Main/Success.bin"), File.OpenRead("./Test/TestSS_Main/Success_ORG.bin"), Assembler.AsmEnum.FileTypeEnum.BIN);
            Lib.AreSameBin(File.OpenRead("./Test/TestSS_Main/tags"), File.OpenRead("./Test/TestSS_Main/Success_ORG.tag"), Assembler.AsmEnum.FileTypeEnum.TAG);
        }

        [TestMethod]
        public void TestErrList()
        {
            var result_simple = Program.Main(@"ErrorList.Z80", "-err", "-lst", "-cd", "./Test/TestSS_Main/");
            Assert.AreEqual(result_simple, 1);
            Lib.AreSameLst(File.OpenRead("./Test/TestSS_Main/ErrorList.err"), File.OpenRead("./Test/TestSS_Main/ErrorList_ORG.err"), Assembler.AsmEnum.FileTypeEnum.ERR);
            Lib.AreSameLst(File.OpenRead("./Test/TestSS_Main/ErrorList.lst"), File.OpenRead("./Test/TestSS_Main/ErrorList_ORG.lst"), Assembler.AsmEnum.FileTypeEnum.LST);
        }
    }
}
