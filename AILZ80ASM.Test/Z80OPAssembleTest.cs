using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;

namespace AILZ80ASM.Test
{
    [TestClass]
    public class Z80OPAssembleTest
    {
        [TestMethod]
        public void TestLN_Comment()
        {
            Lib.Assemble_AreSame(MethodBase.GetCurrentMethod().Name);
        }

        [TestMethod]
        public void TestOP_ADC_16BitR_16BitR()
        {
            Lib.Assemble_AreSame(MethodBase.GetCurrentMethod().Name);
        }

        [TestMethod]
        public void TestOP_ADC_Acc_All()
        {
            Lib.Assemble_AreSame(MethodBase.GetCurrentMethod().Name);
        }

        [TestMethod]
        public void TestOP_ADD_16BitR_16BitR()
        {
            Lib.Assemble_AreSame(MethodBase.GetCurrentMethod().Name);
        }

        [TestMethod]
        public void TestOP_ADD_Acc_All()
        {
            Lib.Assemble_AreSame(MethodBase.GetCurrentMethod().Name);
        }

        [TestMethod]
        public void TestOP_AND_All()
        {
            Lib.Assemble_AreSame(MethodBase.GetCurrentMethod().Name);
        }

        [TestMethod]
        public void TestOP_BCD()
        {
            Lib.Assemble_AreSame(MethodBase.GetCurrentMethod().Name);
        }

        [TestMethod]
        public void TestOP_BIT()
        {
            Lib.Assemble_AreSame(MethodBase.GetCurrentMethod().Name);
        }

        [TestMethod]
        public void TestOP_CALL()
        {
            Lib.Assemble_AreSame(MethodBase.GetCurrentMethod().Name);
        }

        [TestMethod]
        public void TestOP_CCF_And_SCF()
        {
            Lib.Assemble_AreSame(MethodBase.GetCurrentMethod().Name);
        }

        [TestMethod]
        public void TestOP_CP_All()
        {
            Lib.Assemble_AreSame(MethodBase.GetCurrentMethod().Name);
        }

        [TestMethod]
        public void TestOP_CPI_And_CPD()
        {
            Lib.Assemble_AreSame(MethodBase.GetCurrentMethod().Name);
        }

        [TestMethod]
        public void TestOP_CPL_And_NEG()
        {
            Lib.Assemble_AreSame(MethodBase.GetCurrentMethod().Name);
        }

        [TestMethod]
        public void TestOP_DEC_AllR()
        {
            Lib.Assemble_AreSame(MethodBase.GetCurrentMethod().Name);
        }

        [TestMethod]
        public void TestOP_EXX()
        {
            Lib.Assemble_AreSame(MethodBase.GetCurrentMethod().Name);
        }

        [TestMethod]
        public void TestOP_INC_AllR()
        {
            Lib.Assemble_AreSame(MethodBase.GetCurrentMethod().Name);
        }

        [TestMethod]
        public void TestOP_INDEX_Register()
        {
            Lib.Assemble_AreSame(MethodBase.GetCurrentMethod().Name);
        }

        [TestMethod]
        public void TestOP_INT()
        {
            Lib.Assemble_AreSame(MethodBase.GetCurrentMethod().Name);
        }

        [TestMethod]
        public void TestOP_IN_And_OUT()
        {
            Lib.Assemble_AreSame(MethodBase.GetCurrentMethod().Name);
        }

        [TestMethod]
        public void TestOP_JP()
        {
            Lib.Assemble_AreSame(MethodBase.GetCurrentMethod().Name);
        }

        [TestMethod]
        public void TestOP_JR()
        {
            Lib.Assemble_AreSame(MethodBase.GetCurrentMethod().Name);
        }

        [TestMethod]
        public void TestOP_LDI_And_LDD()
        {
            Lib.Assemble_AreSame(MethodBase.GetCurrentMethod().Name);
        }

        [TestMethod]
        public void TestOP_LD_16BitR_16BitN()
        {
            Lib.Assemble_AreSame(MethodBase.GetCurrentMethod().Name);
        }

        [TestMethod]
        public void TestOP_LD_16BitR_16BitR()
        {
            Lib.Assemble_AreSame(MethodBase.GetCurrentMethod().Name);
        }

        [TestMethod]
        public void TestOP_LD_16BitR_Address()
        {
            Lib.Assemble_AreSame(MethodBase.GetCurrentMethod().Name);
        }

        [TestMethod]
        public void TestOP_LD_8BitR_8BitN()
        {
            Lib.Assemble_AreSame(MethodBase.GetCurrentMethod().Name);
        }

        [TestMethod]
        public void TestOP_LD_8BitR_8BitR()
        {
            Lib.Assemble_AreSame(MethodBase.GetCurrentMethod().Name);
        }

        [TestMethod]
        public void TestOP_LD_8BitR_Address()
        {
            Lib.Assemble_AreSame(MethodBase.GetCurrentMethod().Name);
        }

        [TestMethod]
        public void TestOP_LD_Address_16BitR()
        {
            Lib.Assemble_AreSame(MethodBase.GetCurrentMethod().Name);
        }

        [TestMethod]
        public void TestOP_LD_Address_8BitN()
        {
            Lib.Assemble_AreSame(MethodBase.GetCurrentMethod().Name);
        }

        [TestMethod]
        public void TestOP_LD_Address_8BitR()
        {
            Lib.Assemble_AreSame(MethodBase.GetCurrentMethod().Name);
        }

        [TestMethod]
        public void TestOP_OR_All()
        {
            Lib.Assemble_AreSame(MethodBase.GetCurrentMethod().Name);
        }

        [TestMethod]
        public void TestOP_POP_16BitR()
        {
            Lib.Assemble_AreSame(MethodBase.GetCurrentMethod().Name);
        }

        [TestMethod]
        public void TestOP_PUSH_16BitR()
        {
            Lib.Assemble_AreSame(MethodBase.GetCurrentMethod().Name);
        }

        [TestMethod]
        public void TestOP_RES()
        {
            Lib.Assemble_AreSame(MethodBase.GetCurrentMethod().Name);
        }

        [TestMethod]
        public void TestOP_RET()
        {
            Lib.Assemble_AreSame(MethodBase.GetCurrentMethod().Name);
        }

        [TestMethod]
        public void TestOP_RL()
        {
            Lib.Assemble_AreSame(MethodBase.GetCurrentMethod().Name);
        }

        [TestMethod]
        public void TestOP_RR()
        {
            Lib.Assemble_AreSame(MethodBase.GetCurrentMethod().Name);
        }

        [TestMethod]
        public void TestOP_RST()
        {
            Lib.Assemble_AreSame(MethodBase.GetCurrentMethod().Name);
        }

        [TestMethod]
        public void TestOP_SBC_16BitR_16BitR()
        {
            Lib.Assemble_AreSame(MethodBase.GetCurrentMethod().Name);
        }

        [TestMethod]
        public void TestOP_SBC_Acc_All()
        {
            Lib.Assemble_AreSame(MethodBase.GetCurrentMethod().Name);
        }

        [TestMethod]
        public void TestOP_SET()
        {
            Lib.Assemble_AreSame(MethodBase.GetCurrentMethod().Name);
        }

        [TestMethod]
        public void TestOP_SL()
        {
            Lib.Assemble_AreSame(MethodBase.GetCurrentMethod().Name);
        }

        [TestMethod]
        public void TestOP_SR()
        {
            Lib.Assemble_AreSame(MethodBase.GetCurrentMethod().Name);
        }

        [TestMethod]
        public void TestOP_SUB_Acc_All()
        {
            Lib.Assemble_AreSame(MethodBase.GetCurrentMethod().Name);
        }

        [TestMethod]
        public void TestOP_XOR_All()
        {
            Lib.Assemble_AreSame(MethodBase.GetCurrentMethod().Name);
        }
    }
}
