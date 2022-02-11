using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;

namespace AILZ80ASM.Test
{
    [TestClass]
    public class Z80ERAssembleTest
    {
        private ErrorLineItem[] Assemble(string fileName)
        {
            var targetDirectoryName = Path.Combine(".", "Test", "TestER");
            var inputFiles = new[] { new FileInfo(Path.Combine(targetDirectoryName, fileName)) };
            using var memoryStream1 = new MemoryStream();
            using var memoryStream2 = new MemoryStream();

            return Lib.Assemble(inputFiles, memoryStream1, memoryStream2, false, true);
        }

        [TestMethod]
        public void TestER_Address()
        {
            var errors = Assemble("Address.Z80");

            Assert.AreEqual(errors.Length, 5);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0001, 8, "Address.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0001, 10, "Address.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0014, 7, "Address.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0013, 12, "Address.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0013, 13, "Address.Z80", errors);
        }

        [TestMethod]
        public void TestER_Align()
        {
            var errors = Assemble("Align.Z80");

            Assert.AreEqual(errors.Length, 1);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0015, 3, "Align.Z80", errors);
        }

        [TestMethod]
        public void TestER_Conditional()
        {
            var errors = Assemble("Conditional.Z80");
            Assert.AreEqual(errors.Length, 4);

            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E1022, 11, "Conditional.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0001, 4, "Conditional.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0001, 9, "Conditional.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E1031, 10, "Conditional.Z80", errors);
        }

        [TestMethod]
        public void TestER_Conditional_Label()
        {
            var errors = Assemble("Conditional_Label.Z80");
            Assert.AreEqual(errors.Length, 5);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E1024, 5, "Conditional_Label.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E1024, 9, "Conditional_Label.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E1024, 14, "Conditional_Label.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E1024, 16, "Conditional_Label.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E1031, 15, "Conditional_Label.Z80", errors);
        }

        [TestMethod]
        public void TestER_DBDW()
        {
            var errors = Assemble("DBDW.Z80");

            Assert.AreEqual(errors.Length, 2);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0021, 3, "DBDW.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0022, 4, "DBDW.Z80", errors);
        }

        [TestMethod]
        public void TestER_Equ()
        {
            var errors = Assemble("Equ.Z80");

            Assert.AreEqual(errors.Length, 1);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0013, 2, "Equ.Z80", errors);
        }

        [TestMethod]
        public void TestER_Error()
        {
            var errors = Assemble("Error.Z80");

            Assert.AreEqual(errors.Length, 2);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E1031, 2, "Error.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E1032, 3, "Error.Z80", errors);
            Assert.AreEqual(errors[1].ErrorMessage, "#ERROR:エラーテスト");
        }

        [TestMethod]
        public void TestER_Include()
        {
            var errors = Assemble("Include.Z80");

            Assert.AreEqual(errors.Length, 4);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E2002, 3, "Include.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E2003, 5, "Include.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0001, 1, "Include_error.z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E2007, 6, "Include.Z80", errors);
        }

        [TestMethod]
        public void TestER_Label()
        {
            var errors = Assemble("Label.Z80");

            Assert.AreEqual(errors.Length, 9);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0001, 1, "Label.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0013, 2, "Label.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0013, 3, "Label.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0013, 4, "Label.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0013, 5, "Label.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0013, 6, "Label.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0014, 8, "Label.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0017, 14, "Label.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0018, 11, "Label.Z80", errors);
        }

        [TestMethod]
        public void TestER_Macro()
        {
            var errors = Assemble("Macro.Z80");

            Assert.AreEqual(errors.Length, 13);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0001, 2, "Macro.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E3006, 17, "Macro.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E3006, 20, "Macro.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E3005, 33, "Macro.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E3007, 38, "Macro.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E3004, 3, "Macro.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E3004, 5, "Macro.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E3010, 12, "Macro.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E3007, 44, "Macro.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E3007, 49, "Macro.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E3007, 54, "Macro.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E3007, 59, "Macro.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E3001, 63, "Macro.Z80", errors);
        }

        [TestMethod]
        public void TestER_MacroCircularError()
        {
            var errors = Assemble("Macro_CircularError.Z80");

            Assert.AreEqual(errors.Length, 1);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E3008, 13, "Macro_CircularError.Z80", errors);
        }

        [TestMethod]
        public void TestER_MacroInsideError()
        {
            var errors = Assemble("Macro_InsideError.Z80");

            Assert.AreEqual(errors.Length, 1);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0001, 4, "Macro_InsideError.Z80", errors);
        }

        [TestMethod]
        public void TestER_MacroInsideAssembleError()
        {
            var errors = Assemble("Macro_InsideAssembleError.Z80");

            Assert.AreEqual(errors.Length, 1);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0004, 8, "Macro_InsideAssembleError.Z80", errors);
        }

        [TestMethod]
        public void TestER_MissingOperandError()
        {
            var errors = Assemble("MissingOperand.Z80");

            var maxLine = 16;
            Assert.AreEqual(errors.Length, maxLine);
            foreach (var index in Enumerable.Range(1, maxLine))
            {
                Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0001, index, "MissingOperand.Z80", errors);
            }
        }

        [TestMethod]
        public void TestER_Repeat()
        {
            var errors = Assemble("Repeat.Z80");

            Assert.AreEqual(errors.Length, 4);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E1014, 7, "Repeat.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E1015, 14, "Repeat.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E1012, 16, "Repeat.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E1011, 18, "Repeat.Z80", errors);
        }
    }
}
