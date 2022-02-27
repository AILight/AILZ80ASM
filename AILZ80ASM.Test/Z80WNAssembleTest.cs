using AILZ80ASM.Assembler;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;

namespace AILZ80ASM.Test
{
    [TestClass]
    public class Z80WNAssembleTest
    {
        [TestMethod]
        public void TestWN_EXHLDE()
        {
            var warnings = Lib.Assemble_AreSame(MethodBase.GetCurrentMethod().Name);

            Assert.AreEqual(warnings.Length, 1);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.W9004, 1, "Test.Z80", warnings);
        }

        [TestMethod]
        public void TestWN_INDEX_Register()
        {
            var warnings = Lib.Assemble_AreSame(MethodBase.GetCurrentMethod().Name);
            
            Assert.AreEqual(warnings.Length, 4);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.W0003, 2, "Test.Z80", warnings);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.W0003, 3, "Test.Z80", warnings);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.W0003, 7, "Test.Z80", warnings);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.W0003, 8, "Test.Z80", warnings);
        }

        [TestMethod]
        public void TestWN_IXIY()
        {
            var warnings = Lib.Assemble_AreSame(MethodBase.GetCurrentMethod().Name);

            Assert.AreEqual(warnings.Length, 402);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.W9001, 422, "Test.Z80", warnings);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.W9002, 423, "Test.Z80", warnings);
        }

        [TestMethod]
        public void TestWN_Local_Label()
        {
            var warnings = Lib.Assemble_AreSame(MethodBase.GetCurrentMethod().Name);

            Assert.AreEqual(warnings.Length, 1);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.W9005, 23, "Test.Z80", warnings);
        }

        [TestMethod]
        public void TestWN_SUB()
        {
            var warnings = Lib.Assemble_AreSame(MethodBase.GetCurrentMethod().Name);

            Assert.AreEqual(warnings.Length, 17);
            Lib.AssertErrorItemMessage(Error.ErrorCodeEnum.W9003, 23, "Test.Z80", warnings);
        }
    }
}
