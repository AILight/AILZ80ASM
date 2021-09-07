using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Reflection;

namespace AILZ80ASM.Test
{
    [TestClass]
    public class ERAssembleTest
    {
        [TestMethod]
        public void TestER_Address()
        {
            var targetDirectoryName = Path.Combine(".", "Test", "TestER");

            var inputFiles = new[] { new FileInfo(Path.Combine(targetDirectoryName, "Address.Z80")) };

            using (var memoryStream = new MemoryStream())
            {
                var errors = Lib.Assemble(inputFiles, memoryStream);

                Assert.AreEqual(errors.Length, 1);
                Assert.AreEqual(errors[0].LineItemErrorMessages.Length, 2);
                Assert.AreEqual(errors[0].LineItemErrorMessages[0].ErrorMessageException.ErrorCode, Error.ErrorCodeEnum.E0001);
                Assert.AreEqual(errors[0].LineItemErrorMessages[0].LineItem.LineIndex, 7);
                Assert.AreEqual(errors[0].LineItemErrorMessages[1].ErrorMessageException.ErrorCode, Error.ErrorCodeEnum.E0001);
                Assert.AreEqual(errors[0].LineItemErrorMessages[1].LineItem.LineIndex, 9);
            }
        }
    }
}
