using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Reflection;

namespace AILZ80ASM.Test
{
    [TestClass]
    public class ERAssembleTest
    {
        [TestMethod]
        public void TestER_LD_Address()
        {
            var targetDirectoryName = Path.Combine(".", "Test", "TestER_Address");

            var inputFiles = new[] { new FileInfo(Path.Combine(targetDirectoryName, "Test.Z80")) };

            using (var memoryStream = new MemoryStream())
            {
                Lib.Assemble(inputFiles, memoryStream);
            }
        }
    }
}
