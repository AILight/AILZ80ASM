using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;

namespace AILZ80ASM.Test
{
    [TestClass]
    public class Z80PPAssembleTest
    {
        [TestMethod]
        public void TestPP_Conditional()
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
        public void TestPP_MacroCompatible()
        {
            Lib.Assemble_AreSame(MethodBase.GetCurrentMethod().Name);
        }

        [TestMethod]
        public void TestPP_MacroEx()
        {
            Lib.Assemble_AreSame(MethodBase.GetCurrentMethod().Name);
        }

        [TestMethod]
        public void TestPP_MacroModern()
        {
            Lib.Assemble_AreSame(MethodBase.GetCurrentMethod().Name);
        }

        [TestMethod]
        public void TestPP_MacroProgramAddress()
        {
            Lib.Assemble_AreSame(MethodBase.GetCurrentMethod().Name);
        }

        [TestMethod]
        public void TestPP_Pragma_Once()
        {
            Lib.Assemble_AreSame(MethodBase.GetCurrentMethod().Name);
        }

        [TestMethod]
        public void TestPP_PreproPrint()
        {
            Lib.Assemble_AreSame(MethodBase.GetCurrentMethod().Name);
        }

        [TestMethod]
        public void TestPP_RepeatAlign()
        {
            Lib.Assemble_AreSame(MethodBase.GetCurrentMethod().Name);
        }

        [TestMethod]
        public void TestPP_RepeatCompatible()
        {
            Lib.Assemble_AreSame(MethodBase.GetCurrentMethod().Name);
        }

        [TestMethod]
        public void TestPP_RepeatModern()
        {
            Lib.Assemble_AreSame(MethodBase.GetCurrentMethod().Name);
        }
    }
}
