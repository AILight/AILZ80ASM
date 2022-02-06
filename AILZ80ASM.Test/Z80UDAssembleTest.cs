using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;

namespace AILZ80ASM.Test
{
    [TestClass]
    public class Z80UDAssembleTest
    {

        [TestMethod]
        public void TestUD_AO()
        {
            Lib.Assemble_AreSame(MethodBase.GetCurrentMethod().Name);
        }
        [TestMethod]
        public void TestUD_BIT_RES()
        {
            Lib.Assemble_AreSame(MethodBase.GetCurrentMethod().Name);
        }
        [TestMethod]
        public void TestUD_BIT_SET()
        {
            Lib.Assemble_AreSame(MethodBase.GetCurrentMethod().Name);
        }
        [TestMethod]
        public void TestUD_CB_SLL()
        {
            Lib.Assemble_AreSame(MethodBase.GetCurrentMethod().Name);
        }
        [TestMethod]
        public void TestUD_IN_And_OUT()
        {
            Lib.Assemble_AreSame(MethodBase.GetCurrentMethod().Name);
        }

        [TestMethod]
        public void TestUD_INC_And_DEC()
        {
            Lib.Assemble_AreSame(MethodBase.GetCurrentMethod().Name);
        }

        [TestMethod]
        public void TestUD_LD()
        {
            Lib.Assemble_AreSame(MethodBase.GetCurrentMethod().Name);
        }
        [TestMethod]
        public void TestUD_RL()
        {
            Lib.Assemble_AreSame(MethodBase.GetCurrentMethod().Name);
        }
        [TestMethod]
        public void TestUD_RLC()
        {
            Lib.Assemble_AreSame(MethodBase.GetCurrentMethod().Name);
        }
        [TestMethod]
        public void TestUD_RR()
        {
            Lib.Assemble_AreSame(MethodBase.GetCurrentMethod().Name);
        }
        [TestMethod]
        public void TestUD_RRC()
        {
            Lib.Assemble_AreSame(MethodBase.GetCurrentMethod().Name);
        }
        [TestMethod]
        public void TestUD_SLA()
        {
            Lib.Assemble_AreSame(MethodBase.GetCurrentMethod().Name);
        }
        [TestMethod]
        public void TestUD_SRA()
        {
            Lib.Assemble_AreSame(MethodBase.GetCurrentMethod().Name);
        }
        [TestMethod]
        public void TestUD_SLL()
        {
            Lib.Assemble_AreSame(MethodBase.GetCurrentMethod().Name);
        }
        [TestMethod]
        public void TestUD_SRL()
        {
            Lib.Assemble_AreSame(MethodBase.GetCurrentMethod().Name);
        }
    }
}
