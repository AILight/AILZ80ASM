using AILZ80ASM.Assembler;
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
            var outputFiles = new System.Collections.Generic.Dictionary<MemoryStream, System.Collections.Generic.KeyValuePair<Assembler.AsmEnum.FileTypeEnum, FileInfo>>();

            return Lib.Assemble(inputFiles, outputFiles, true);
        }

        [TestMethod]
        public void TestER_Address1()
        {
            var errors = Assemble("Address1.Z80");

            Assert.AreEqual(4, errors.Length);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0013, 8, "Address1.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0014, 7, "Address1.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0013, 12, "Address1.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0013, 13, "Address1.Z80", errors);
        }

        [TestMethod]
        public void TestER_Address2()
        {
            var errors = Assemble("Address2.Z80");

            Assert.AreEqual(1, errors.Length);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0001, 10, "Address2.Z80", errors);
        }

        [TestMethod]
        public void TestER_Align()
        {
            var errors = Assemble("Align.Z80");

            Assert.AreEqual(1, errors.Length);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0015, 3, "Align.Z80", errors);
        }

        [TestMethod]
        public void TestER_CharMap1()
        {
            var errors = Assemble("CharMap1.Z80");

            Assert.AreEqual(9, errors.Length);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E2101, 2, "CharMap1.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E2101, 6, "CharMap1.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E2104, 7, "CharMap1.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E2104, 8, "CharMap1.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E2104, 9, "CharMap1.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E2104, 10, "CharMap1.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E2104, 11, "CharMap1.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E2104, 12, "CharMap1.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E2104, 13, "CharMap1.Z80", errors);
        }

        [TestMethod]
        public void TestER_CharMap2()
        {
            var errors = Assemble("CharMap2.Z80");

            Assert.AreEqual(3, errors.Length);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E2105, 17, "CharMap2.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E2106, 19, "CharMap2.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0005, 21, "CharMap2.Z80", errors);
        }

        [TestMethod]
        public void TestER_Conditional()
        {
            var errors = Assemble("Conditional.Z80");

            Assert.AreEqual(1, errors.Length);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E1022, 11, "Conditional.Z80", errors);
        }

        [TestMethod]
        public void TestER_Conditional_Label()
        {
            var errors = Assemble("Conditional_Label.Z80");

            Assert.AreEqual(3, errors.Length);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E1024, 9, "Conditional_Label.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E1024, 14, "Conditional_Label.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E1024, 16, "Conditional_Label.Z80", errors);
        }

        [TestMethod]
        public void TestER_DBDW()
        {
            var errors = Assemble("DBDW.Z80");

            Assert.AreEqual(5, errors.Length);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0021, 3, "DBDW.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0004, 4, "DBDW.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0022, 5, "DBDW.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E2106, 6, "DBDW.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E2105, 7, "DBDW.Z80", errors);
        }

        [TestMethod]
        public void TestER_Equ()
        {
            var errors = Assemble("Equ.Z80");

            Assert.AreEqual(1, errors.Length);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0013, 2, "Equ.Z80", errors);
        }

        [TestMethod]
        public void TestER_Error1()
        {
            var errors = Assemble("Error1.Z80");

            Assert.AreEqual(1, errors.Length);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E1032, 3, "Error1.Z80", errors);
        }

        [TestMethod]
        public void TestER_Error2()
        {
            var errors = Assemble("Error2.Z80");

            Assert.AreEqual(1, errors.Length);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E1031, 2, "Error2.Z80", errors);
        }

        [TestMethod]
        public void TestER_Include1()
        {
            var errors = Assemble("Include1.Z80");

            Assert.AreEqual(2, errors.Length);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E2002, 3, "Include1.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E2003, 5, "Include1.Z80", errors);
        }

        [TestMethod]
        public void TestER_Include2()
        {
            var errors = Assemble("Include2.Z80");

            Assert.AreEqual(1, errors.Length);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0001, 1, "Include_error.z80", errors);
        }

        [TestMethod]
        public void TestER_JR()
        {
            var errors = Assemble("JR.Z80");

            Assert.AreEqual(1, errors.Length);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0001, 5, "JR.Z80", errors);
        }

        [TestMethod]
        public void TestER_Label1()
        {
            var errors = Assemble("Label1.Z80");

            Assert.AreEqual(8, errors.Length);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0013, 2, "Label1.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0013, 3, "Label1.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0013, 4, "Label1.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0013, 5, "Label1.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0013, 6, "Label1.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0014, 8, "Label1.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0018, 11, "Label1.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0017, 14, "Label1.Z80", errors);
        }

        [TestMethod]
        public void TestER_Label2()
        {
            var errors = Assemble("Label2.Z80");

            Assert.AreEqual(3, errors.Length);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0001, 1, "Label2.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0004, 17, "Label2.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0006, 20, "Label2.Z80", errors);
        }

        [TestMethod]
        public void TestER_Label3()
        {
            var errors = Assemble("Label3.Z80");

            Assert.AreEqual(1, errors.Length);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0008, 4, "Label3.Z80", errors);
        }

        [TestMethod]
        public void TestER_Macro1()
        {
            var errors = Assemble("Macro1.Z80");

            Assert.AreEqual(10, errors.Length);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E3010, 12, "Macro1.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E3006, 17, "Macro1.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E3006, 20, "Macro1.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E3005, 33, "Macro1.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E3007, 38, "Macro1.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E3007, 44, "Macro1.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E3007, 49, "Macro1.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E3007, 54, "Macro1.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E3007, 59, "Macro1.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E3001, 63, "Macro1.Z80", errors);
        }

        [TestMethod]
        public void TestER_Macro2()
        {
            var errors = Assemble("Macro2.Z80");

            Assert.AreEqual(2, errors.Length);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E3004, 3, "Macro2.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E3004, 5, "Macro2.Z80", errors);
        }

        [TestMethod]
        public void TestER_MacroCircularError()
        {
            var errors = Assemble("Macro_CircularError.Z80");

            Assert.AreEqual(1, errors.Length);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E3008, 13, "Macro_CircularError.Z80", errors);
        }

        [TestMethod]
        public void TestER_MacroInsideError()
        {
            var errors = Assemble("Macro_InsideError.Z80");

            Assert.AreEqual(1, errors.Length);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0001, 4, "Macro_InsideError.Z80", errors);
        }

        [TestMethod]
        public void TestER_MacroInsideAssembleError()
        {
            var errors = Assemble("Macro_InsideAssembleError.Z80");

            Assert.AreEqual(1, errors.Length);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0004, 8, "Macro_InsideAssembleError.Z80", errors);
        }

        [TestMethod]
        public void TestER_MissingOperandError()
        {
            var errors = Assemble("MissingOperand.Z80");

            var maxLine = 16;
            Assert.AreEqual(maxLine, errors.Length);
            foreach (var index in Enumerable.Range(1, maxLine))
            {
                Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0001, index, "MissingOperand.Z80", errors);
            }
        }

        [TestMethod]
        public void TestER_Org1()
        {
            var errors = Assemble("Org1.Z80");

            Assert.AreEqual(1, errors.Length);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0009, 20, "Org1.Z80", errors);
        }

        [TestMethod]
        public void TestER_Org2()
        {
            var errors = Assemble("Org2.Z80");

            Assert.AreEqual(1, errors.Length);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0009, 20, "Org2.Z80", errors);
        }

        [TestMethod]
        public void TestER_Org3()
        {
            var errors = Assemble("Org3.Z80");

            Assert.AreEqual(2, errors.Length);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0010, 2, "Org3.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0010, 9, "Org3.Z80", errors);
        }

        [TestMethod]
        public void TestER_Org4()
        {
            var errors = Assemble("Org4.Z80");

            Assert.AreEqual(1, errors.Length);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0010, 2, "Org4.Z80", errors);
        }

        [TestMethod]
        public void TestER_PreProcList1()
        {
            var errors = Assemble("PreProcList1.Z80");

            Assert.AreEqual(2, errors.Length);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E1051, 4, "PreProcList1.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E1052, 5, "PreProcList1.Z80", errors);
        }

        [TestMethod]
        public void TestER_PreProcList2()
        {
            var errors = Assemble("PreProcList2.Z80");

            Assert.AreEqual(3, errors.Length);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0004, 4, "PreProcList2.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0004, 5, "PreProcList2.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E1051, 6, "PreProcList2.Z80", errors);
        }

        [TestMethod]
        public void TestER_PreProcPrint()
        {
            var errors = Assemble("PreProcPrint.Z80");

            Assert.AreEqual(1, errors.Length);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.I0001, 4, "PreProcPrint.Z80", errors);
            Assert.AreEqual("#PRINT: ABC:1", errors[0].ErrorMessage);
        }

        [TestMethod]
        public void TestER_Repeat()
        {
            var errors = Assemble("Repeat.Z80");

            Assert.AreEqual(4, errors.Length);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E1014, 7, "Repeat.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E1015, 14, "Repeat.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E3002, 16, "Repeat.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E1011, 18, "Repeat.Z80", errors);
        }

        [TestMethod]
        public void TestER_Repeat_Last()
        {
            var errors = Assemble("Repeat_Last.Z80");

            Assert.AreEqual(3, errors.Length);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0004, 37, "Repeat_Last.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E1013, 41, "Repeat_Last.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E1016, 50, "Repeat_Last.Z80", errors);
        }

        [TestMethod]
        public void TestER_Warning()
        {
            var errors = Assemble("Warning.Z80");

            Assert.AreEqual(4, errors.Length);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.W0001, 3, "Warning.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.W0002, 4, "Warning.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0003, 7, "Warning.Z80", errors);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E0003, 8, "Warning.Z80", errors);
        }
    }
}
