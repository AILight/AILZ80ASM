using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;

namespace AILZ80ASM.Test
{
    [TestClass]
    public class CSAssembleTest
    {
        [TestMethod]
        public void TestCS_ALIGN()
        {
            Lib.Assemble_AreSame(MethodBase.GetCurrentMethod().Name);
        }

        [TestMethod]
        public void TestCS_ORG()
        {
            Lib.Assemble_AreSame(MethodBase.GetCurrentMethod().Name);
        }

        [TestMethod]
        public void TestCS_ORG_Rom()
        {
            Lib.Assemble_AreSame(MethodBase.GetCurrentMethod().Name);
        }
    }
}
