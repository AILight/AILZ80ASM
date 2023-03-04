using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;

namespace AILZ80ASM.Test
{
    [TestClass]
    public class Z80SPAssembleTest
    {
        [TestMethod]
        public void E0010()
        {
            Lib.Assemble_AreSame("TestSP", MethodBase.GetCurrentMethod().Name);
        }

        [TestMethod]
        public void E0011()
        {
            Lib.Assemble_AreSame("TestSP", MethodBase.GetCurrentMethod().Name);
        }
    }
}
