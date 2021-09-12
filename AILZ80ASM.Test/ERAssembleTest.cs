using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;
using System.Reflection;

namespace AILZ80ASM.Test
{
    [TestClass]
    public class ERAssembleTest
    {
        private FileInfoErrorMessage[] Assemble(string fileName)
        {
            var targetDirectoryName = Path.Combine(".", "Test", "TestER");
            var inputFiles = new[] { new FileInfo(Path.Combine(targetDirectoryName, fileName)) };
            using var memoryStream = new MemoryStream();

            return Lib.Assemble(inputFiles, memoryStream);
        }

        [TestMethod]
        public void TestER_Address()
        {
            var errors = Assemble("Address.Z80");

            Assert.AreEqual(errors.Length, 1);
            Assert.AreEqual(errors[0].ErrorLineItemMessages.Length, 2);
            AssertErrorItemMessage(Error.ErrorCodeEnum.E0001, 7, errors[0].ErrorLineItemMessages);
            AssertErrorItemMessage(Error.ErrorCodeEnum.E0001, 9, errors[0].ErrorLineItemMessages);
        }

        [TestMethod]
        public void TestER_Macro()
        {
            var errors = Assemble("Macro.Z80");

            Assert.AreEqual(errors.Length, 2);
            Assert.AreEqual(errors[0].ErrorLineItemMessages.Length, 1);
            AssertErrorItemMessage(Error.ErrorCodeEnum.E1001, 27, errors[0].ErrorLineItemMessages);
            Assert.AreEqual(errors[1].ErrorLineItemMessages.Length, 4);
            AssertErrorItemMessage(Error.ErrorCodeEnum.E1006, 9, errors[1].ErrorLineItemMessages);
            AssertErrorItemMessage(Error.ErrorCodeEnum.E1005, 22, errors[1].ErrorLineItemMessages);
            AssertErrorItemMessage(Error.ErrorCodeEnum.E1004, 2, errors[1].ErrorLineItemMessages);
            AssertErrorItemMessage(Error.ErrorCodeEnum.E1004, 4, errors[1].ErrorLineItemMessages);
        }

        private static void AssertErrorItemMessage(Error.ErrorCodeEnum errorCode, int lineIndex, ErrorLineItemMessage[] errors)
        {
            if (!errors.Any(m => m.ErrorMessageException.ErrorCode == errorCode && m.LineItem.LineIndex == lineIndex))
            {
                Assert.Fail($"エラーが見つかりませんでした。　ErrorCode:{errorCode} LineIndex:{lineIndex}");
            }
        }
    }
}
