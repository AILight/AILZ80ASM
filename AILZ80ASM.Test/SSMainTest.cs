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
            var outputFiles = new []{ "test1.bin", ".\\Test\\TestSS_Main\\Success.CMT", "test1.err" };
            foreach (var item in outputFiles)
            {
                if (File.Exists(item))
                {
                    File.Delete(item);
                }
            }

            var result = Program.Main(@"./Test/TestSS_Main/Success.Z80", "-bin", "test1.bin", "-err", "test1.err");

            Assert.AreEqual(result, 0);
            foreach (var item in outputFiles)
            {
                Assert.IsTrue(File.Exists(item), item);
            }
        }

        [TestMethod]
        public void TestMainSuccessAndChangeDirectory()
        {
            var outputFiles = new[] { ".\\Test\\TestSS_Main\\test1.bin", ".\\Test\\TestSS_Main\\Success.CMT", ".\\Test\\TestSS_Main\\test1.err" };
            foreach (var item in outputFiles)
            {
                if (File.Exists(item))
                {
                    File.Delete(item);
                }
            }

            var result = Program.Main(@"Success.Z80", "-bin", "test1.bin", "-err", "test1.err", "-cd", "./Test/TestSS_Main/");

            Assert.AreEqual(result, 0);
            foreach (var item in outputFiles)
            {
                Assert.IsTrue(File.Exists(item), item);
            }
        }

        [TestMethod]
        public void TestMainSuccessAllOption()
        {
            var outputFiles = new[] { "test2.bin", ".\\Test\\TestSS_Main\\Success.CMT", "test2.err", "test2.lst", "test2.equ" };
            foreach (var item in outputFiles)
            {
                if (File.Exists(item))
                {
                    File.Delete(item);
                }
            }

            var result = Program.Main(@"./Test/TestSS_Main/Success.Z80", "-bin", "test2.bin", "-err", "test2.err", "-lst", "test2.lst", "-equ", "test2.equ");
            Assert.AreEqual(result, 0);
            foreach (var item in outputFiles)
            {
                Assert.IsTrue(File.Exists(item), item);
            }
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
            var result = Program.Main(@"./Test/TestSS_Main/Error.Z80", "-bin", "test3.bin", "-err", "test3.err");
            Assert.AreEqual(result, 1);
        }

        [TestMethod]
        public void TestErrorArgument()
        {
            var result = Program.Main(@"./Test/TestSS_Main/Error.Z80", "-bin1", "test3.bin");
            Assert.AreEqual(result, 2);
        }

        [TestMethod]
        public void TestErrorWrtiteFile()
        {
            using (var stream = new FileStream("test5.bin", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read))
            {
                var result = Program.Main(@"./Test/TestSS_Main/Success.Z80", "-bin", "test5.bin", "-err", "test5.err", "-lst", "test5.lst", "-equ", "test5.equ");
                Assert.AreEqual(result, 1);
            }
        }

        [TestMethod]
        public void TestDiffFile()
        {
            var result_A = Program.Main(@"./Test/TestSS_Main/Success_df1.Z80", "-bin", "test4.bin");
            Assert.AreEqual(result_A, 0);
            
            var result_B = Program.Main(@"./Test/TestSS_Main/Success_df1.Z80", "-bin", "test4.bin", "-df");
            Assert.AreEqual(result_B, 0);
            
            var result_C = Program.Main(@"./Test/TestSS_Main/Success_df2.Z80", "-bin", "test4.bin", "-df");
            Assert.AreEqual(result_C, 1);
        }
    }
}
