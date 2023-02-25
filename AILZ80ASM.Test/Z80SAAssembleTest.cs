using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;

namespace AILZ80ASM.Test
{
    [TestClass]
    public class Z80SAAssembleTest
    {
        [TestMethod]
        public void InitializeOutputAddress()
        {
            Lib.Assemble_AreSame("TestSA", MethodBase.GetCurrentMethod().Name);
        }
    }
}
