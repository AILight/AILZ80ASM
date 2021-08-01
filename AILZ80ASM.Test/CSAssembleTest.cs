using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Reflection;

namespace AILZ80ASM.Test
{
    [TestClass]
    public class CSAssembleTest
    {
        [TestMethod]
        public void TestCS_ORG()
        {
            Lib.Assemble_AreSame(MethodBase.GetCurrentMethod().Name);
        }
    }
}