using AILZ80ASM.Assembler;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;

namespace AILZ80ASM.Test
{
    [TestClass]
    public class Z80ECAssembleTest
    {
        private ErrorLineItem[] Assemble(string fileName)
        {
            var targetDirectoryName = Path.Combine(".", "Test", "TestEC");
            var inputFiles = new[] { new FileInfo(Path.Combine(targetDirectoryName, fileName)) };
            var outputFiles = new System.Collections.Generic.Dictionary<MemoryStream, System.Collections.Generic.KeyValuePair<Assembler.AsmEnum.FileTypeEnum, FileInfo>>();

            return Lib.Assemble(inputFiles, outputFiles, true);
        }

        [TestMethod]
        public void TestER_E0001()
        {
            var errors = Assemble("E0001.Z80");

            Assert.AreEqual(errors.Where(m => m.ErrorType == Error.ErrorTypeEnum.Error).Count(), 16);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0001, 1, "E0001.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0001, 2, "E0001.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0001, 3, "E0001.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0001, 4, "E0001.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0001, 5, "E0001.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0001, 6, "E0001.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0001, 7, "E0001.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0001, 8, "E0001.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0001, 9, "E0001.Z80", errors);

            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0001, 11, "E0001.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0001, 12, "E0001.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0001, 13, "E0001.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0001, 14, "E0001.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0001, 15, "E0001.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0001, 16, "E0001.Z80", errors);

            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0001, 18, "E0001.Z80", errors);
        }

        [TestMethod]
        public void TestER_E0002()
        {
            var errors = Assemble("E0002.Z80");

            Assert.AreEqual(errors.Where(m => m.ErrorType == Error.ErrorTypeEnum.Error).Count(), 2);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0002, 1, "E0002.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0002, 2, "E0002.Z80", errors);
        }

        [TestMethod]
        public void TestER_E0003()
        {
            var errors = Assemble("E0003.Z80");

            Assert.AreEqual(errors.Where(m => m.ErrorType == Error.ErrorTypeEnum.Error).Count(), 2);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0003, 4, "E0003.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0003, 5, "E0003.Z80", errors);
        }

        [TestMethod]
        public void TestER_E0004()
        {
            var errors = Assemble("E0004.Z80");

            Assert.AreEqual(errors.Where(m => m.ErrorType == Error.ErrorTypeEnum.Error).Count(), 27);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0004, 3, "E0004.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0004, 5, "E0004.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0004, 7, "E0004.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0004, 8, "E0004.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0004, 9, "E0004.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0004, 11, "E0004.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0004, 13, "E0004.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0004, 14, "E0004.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0004, 16, "E0004.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0004, 17, "E0004.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0004, 19, "E0004.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0004, 20, "E0004.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0004, 22, "E0004.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0004, 23, "E0004.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0004, 25, "E0004.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0004, 26, "E0004.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0004, 28, "E0004.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0004, 29, "E0004.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0004, 31, "E0004.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0004, 32, "E0004.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0004, 35, "E0004.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0004, 39, "E0004.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0004, 43, "E0004.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0004, 45, "E0004.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0004, 49, "E0004.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0004, 50, "E0004.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0004, 59, "E0004.Z80", errors);
        }

        [TestMethod]
        public void TestER_E0009_1()
        {
            var errors = Assemble("E0009_1.Z80");

            Assert.AreEqual(errors.Where(m => m.ErrorType == Error.ErrorTypeEnum.Error).Count(), 1);

            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0009, 6, "E0009_1.Z80", errors);
        }

        [TestMethod]
        public void TestER_E0009_2()
        {
            var errors = Assemble("E0009_2.Z80");

            Assert.AreEqual(errors.Where(m => m.ErrorType == Error.ErrorTypeEnum.Error).Count(), 1);

            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0009, 8, "E0009_2.Z80", errors);
        }

        [TestMethod]
        public void TestER_E0012()
        {
            var errors = Assemble("E0012.Z80");

            Assert.AreEqual(errors.Where(m => m.ErrorType == Error.ErrorTypeEnum.Error).Count(), 6);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0012, 2, "E0012.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0012, 3, "E0012.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0012, 5, "E0012.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0012, 6, "E0012.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0012, 8, "E0012.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0012, 9, "E0012.Z80", errors);
        }

        [TestMethod]
        public void TestER_E0013()
        {
            var errors = Assemble("E0013.Z80");

            Assert.AreEqual(errors.Where(m => m.ErrorType == Error.ErrorTypeEnum.Error).Count(), 36);

            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0013, 2, "E0013.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0013, 3, "E0013.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0013, 4, "E0013.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0013, 5, "E0013.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0013, 7, "E0013.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0013, 8, "E0013.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0013, 9, "E0013.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0013, 10, "E0013.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0013, 11, "E0013.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0013, 12, "E0013.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0013, 13, "E0013.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0013, 14, "E0013.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0013, 15, "E0013.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0013, 16, "E0013.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0013, 17, "E0013.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0013, 18, "E0013.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0013, 19, "E0013.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0013, 20, "E0013.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0013, 21, "E0013.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0013, 22, "E0013.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0013, 23, "E0013.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0013, 24, "E0013.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0013, 25, "E0013.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0013, 26, "E0013.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0013, 27, "E0013.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0013, 28, "E0013.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0013, 29, "E0013.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0013, 30, "E0013.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0013, 31, "E0013.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0013, 32, "E0013.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0013, 33, "E0013.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0013, 34, "E0013.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0013, 35, "E0013.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0013, 53, "E0013.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0013, 57, "E0013.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0013, 76, "E0013.Z80", errors);
        }

        [TestMethod]
        public void TestER_E0014()
        {
            var errors = Assemble("E0014.Z80");

            Assert.AreEqual(errors.Where(m => m.ErrorType == Error.ErrorTypeEnum.Error).Count(), 3);

            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0014, 3, "E0014.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0014, 5, "E0014.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0014, 7, "E0014.Z80", errors);
        }


        [TestMethod]
        public void TestER_E0015()
        {
            var errors = Assemble("E0015.Z80");

            Assert.AreEqual(errors.Where(m => m.ErrorType == Error.ErrorTypeEnum.Error).Count(), 14);

            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0015, 3, "E0015.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0015, 5, "E0015.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0015, 6, "E0015.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0015, 7, "E0015.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0015, 9, "E0015.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0015, 10, "E0015.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0015, 11, "E0015.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0015, 12, "E0015.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0015, 13, "E0015.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0015, 14, "E0015.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0015, 15, "E0015.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0015, 17, "E0015.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0015, 19, "E0015.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0015, 20, "E0015.Z80", errors);
        }

        [TestMethod]
        public void TestER_E0016()
        {
            var errors = Assemble("E0016.Z80");

            Assert.AreEqual(errors.Where(m => m.ErrorType == Error.ErrorTypeEnum.Error).Count(), 2);

            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0002, 2, "E0016.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0002, 4, "E0016.Z80", errors);
        }

        [TestMethod]
        public void TestER_E0017()
        {
            var errors = Assemble("E0017.Z80");

            Assert.AreEqual(errors.Where(m => m.ErrorType == Error.ErrorTypeEnum.Error).Count(), 1);

            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0017, 3, "E0017.Z80", errors);
        }

        [TestMethod]
        public void TestER_E0018()
        {
            var errors = Assemble("E0018.Z80");

            Assert.AreEqual(errors.Where(m => m.ErrorType == Error.ErrorTypeEnum.Error).Count(), 1);

            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0018, 3, "E0018.Z80", errors);
        }

        [TestMethod]
        public void TestER_E0019()
        {
            var errors = Assemble("E0019.Z80");

            Assert.AreEqual(errors.Where(m => m.ErrorType == Error.ErrorTypeEnum.Error).Count(), 2);

            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0019, 2, "E0019.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0019, 4, "E0019.Z80", errors);
        }

        [TestMethod]
        public void TestER_E0020()
        {
            var errors = Assemble("E0020.Z80");

            Assert.AreEqual(errors.Where(m => m.ErrorType == Error.ErrorTypeEnum.Error).Count(), 3);

            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0020, 3, "E0020.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0020, 11, "E0020.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0020, 12, "E0020.Z80", errors);
        }

        [TestMethod]
        public void TestER_E0021()
        {
            var errors = Assemble("E0021.Z80");

            Assert.AreEqual(errors.Where(m => m.ErrorType == Error.ErrorTypeEnum.Error).Count(), 1);

            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0021, 2, "E0021.Z80", errors);
        }

        [TestMethod]
        public void TestER_E0022()
        {
            var errors = Assemble("E0022.Z80");

            Assert.AreEqual(errors.Where(m => m.ErrorType == Error.ErrorTypeEnum.Error).Count(), 1);

            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0022, 2, "E0022.Z80", errors);
        }

        [TestMethod]
        public void TestER_E0023()
        {
            var errors = Assemble("E0023.Z80");

            Assert.AreEqual(errors.Where(m => m.ErrorType == Error.ErrorTypeEnum.Error).Count(), 3);

            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0023, 2, "E0023.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0023, 3, "E0023.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0023, 4, "E0023.Z80", errors);
        }


        [TestMethod]
        public void TestER_E0024()
        {
            var errors = Assemble("E0024.Z80");

            Assert.AreEqual(errors.Where(m => m.ErrorType == Error.ErrorTypeEnum.Error).Count(), 2);

            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0024, 2, "E0024.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0024, 3, "E0024.Z80", errors);
        }
    }
}
