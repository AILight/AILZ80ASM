using AILZ80ASM.Assembler;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Reflection;

namespace AILZ80ASM.Test
{
    [TestClass]
    public class Z80PPAssembleTest
    {
        [TestMethod]
        public void TestPP_CheckAlign()
        {
            {
                var errors = Lib.Assemble("TestPP_CheckAlign", "Test1.Z80");

                Assert.AreEqual(1, errors.Length);
                Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E6012, 9, "Test1.Z80", errors);
            }

            {
                var errors = Lib.Assemble("TestPP_CheckAlign", "Test2.Z80");

                Assert.AreEqual(1, errors.Length);
                Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.E6012, 9, "Test2.Z80", errors);
            }

            {
                var errors = Lib.Assemble("TestPP_CheckAlign", "Test3.Z80");

                Assert.AreEqual(0, errors.Length);
            }
        }

        [TestMethod]
        public void TestPP_Conditional()
        {
            Lib.Assemble_AreSame(MethodBase.GetCurrentMethod().Name);
        }

        [TestMethod]
        public void TestPP_ConditionalEnhanced()
        {
            Lib.Assemble_AreSame(MethodBase.GetCurrentMethod().Name);
        }

        [TestMethod]
        public void TestPP_Enum()
        {
            Lib.Assemble_AreSame(MethodBase.GetCurrentMethod().Name);
        }

        [TestMethod]
        public void TestPP_Function()
        {
            Lib.Assemble_AreSame(MethodBase.GetCurrentMethod().Name);
        }

        [TestMethod]
        public void TestPP_Include()
        {
            Lib.Assemble_AreSame(MethodBase.GetCurrentMethod().Name);
        }

        [TestMethod]
        public void TestPP_List()
        {
            Lib.Assemble_AreSame(MethodBase.GetCurrentMethod().Name);
        }

        [TestMethod]
        public void TestPP_MacroAndFunction()
        {
            Lib.Assemble_AreSame(MethodBase.GetCurrentMethod().Name);
        }

        [TestMethod]
        public void TestPP_MacroArgumentRegister()
        {
            Lib.Assemble_AreSame(MethodBase.GetCurrentMethod().Name);
        }

        [TestMethod]
        public void TestPP_Macro()
        {
            Lib.Assemble_AreSame(MethodBase.GetCurrentMethod().Name);
        }

        [TestMethod]
        public void TestPP_MacroEx()
        {
            Lib.Assemble_AreSame(MethodBase.GetCurrentMethod().Name);
        }

        [TestMethod]
        public void TestPP_MacroProgramAddress()
        {
            Lib.Assemble_AreSame(MethodBase.GetCurrentMethod().Name);
        }

        [TestMethod]
        public void TestPP_MacroRegister()
        {
            Lib.Assemble_AreSame(MethodBase.GetCurrentMethod().Name);
        }

        [TestMethod]
        public void TestPP_Pragma_Once()
        {
            Lib.Assemble_AreSame(MethodBase.GetCurrentMethod().Name);
        }

        [TestMethod]
        public void TestPP_Print()
        {
            Lib.Assemble_AreSame(MethodBase.GetCurrentMethod().Name);
        }

        [TestMethod]
        public void TestPP_RepeatAlign()
        {
            Lib.Assemble_AreSame(MethodBase.GetCurrentMethod().Name);
        }

        [TestMethod]
        public void TestPP_Repeat()
        {
            Lib.Assemble_AreSame(MethodBase.GetCurrentMethod().Name);
        }
    }
}
