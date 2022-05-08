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
    }
}
