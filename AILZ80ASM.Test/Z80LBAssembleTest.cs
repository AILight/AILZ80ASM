using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;

namespace AILZ80ASM.Test
{
    [TestClass]
    public class Z80LBAssembleTest
    {
        [TestMethod]
        public void TestLB_CALL_Address()
        {
            Lib.Assemble_AreSame(MethodBase.GetCurrentMethod().Name);
        }

        [TestMethod]
        public void TestLB_CharMap()
        {
            Lib.Assemble_AreSame(MethodBase.GetCurrentMethod().Name);
        }

        [TestMethod]
        public void TestLB_DBDW()
        {
            Lib.Assemble_AreSame(MethodBase.GetCurrentMethod().Name);
        }

        [TestMethod]
        public void TestLB_DS()
        {
            Lib.Assemble_AreSame(MethodBase.GetCurrentMethod().Name);
        }

        [TestMethod]
        public void TestLB_DSDBSDWS()
        {
            Lib.Assemble_AreSame(MethodBase.GetCurrentMethod().Name);
        }

        [TestMethod]
        public void TestLB_DSDBSDWS_Trim()
        {
            Lib.Assemble_AreSame(MethodBase.GetCurrentMethod().Name, true);
        }

        [TestMethod]
        public void TestLB_Enhance_EQU()
        {
            Lib.Assemble_AreSame(MethodBase.GetCurrentMethod().Name);
        }

        [TestMethod]
        public void TestLB_EQU()
        {
            Lib.Assemble_AreSame(MethodBase.GetCurrentMethod().Name);
        }

        [TestMethod]
        public void TestLB_GlobalLabel()
        {
            Lib.Assemble_AreSame(MethodBase.GetCurrentMethod().Name);
        }

        [TestMethod]
        public void TestLB_JP_And_JR_And_Call()
        {
            Lib.Assemble_AreSame(MethodBase.GetCurrentMethod().Name);
        }

        [TestMethod]
        public void TestLB_Label()
        {
            Lib.Assemble_AreSame(MethodBase.GetCurrentMethod().Name);
        }

        [TestMethod]
        public void TestLB_LD_Address()
        {
            Lib.Assemble_AreSame(MethodBase.GetCurrentMethod().Name);
        }

        [TestMethod]
        public void TestLB_LD_String()
        {
            Lib.Assemble_AreSame(MethodBase.GetCurrentMethod().Name);
        }
    }
}
