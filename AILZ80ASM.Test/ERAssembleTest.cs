using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;
using System.Reflection;

namespace AILZ80ASM.Test
{
    [TestClass]
    public class ERAssembleTest
    {
        private ErrorFileInfoMessage[] Assemble(string fileName)
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
        public void TestER_Error()
        {
            var errors = Assemble("Error.Z80");

            Assert.AreEqual(errors.Length, 1);
            Assert.AreEqual(errors[0].ErrorLineItemMessages.Length, 1);
            AssertErrorItemMessage(Error.ErrorCodeEnum.E1031, 1, errors[0].ErrorLineItemMessages);
            Assert.AreEqual(errors[0].ErrorLineItemMessages[0].ErrorMessageException.Parameters[0], "エラーテスト");
        }

        [TestMethod]
        public void TestER_Include()
        {
            var errors = Assemble("Include.Z80");

            Assert.AreEqual(errors.Length, 1);
            Assert.AreEqual(errors[0].ErrorLineItemMessages.Length, 3);
            AssertErrorItemMessage(Error.ErrorCodeEnum.E2002, 2, errors[0].ErrorLineItemMessages);
            AssertErrorItemMessage(Error.ErrorCodeEnum.E2001, 4, errors[0].ErrorLineItemMessages);
            AssertErrorItemMessage(Error.ErrorCodeEnum.E2002, 2, errors[0].ErrorLineItemMessages[1].ErrorMessageException.ErrorFileInfoMessage.ErrorLineItemMessages);
            AssertErrorItemMessage(Error.ErrorCodeEnum.E2003, 4, errors[0].ErrorLineItemMessages[1].ErrorMessageException.ErrorFileInfoMessage.ErrorLineItemMessages);
            AssertErrorItemMessage(Error.ErrorCodeEnum.E2001, 3, errors[0].ErrorLineItemMessages);
            AssertErrorItemMessage(Error.ErrorCodeEnum.E0001, 0, errors[0].ErrorLineItemMessages[2].ErrorMessageException.ErrorFileInfoMessage.ErrorLineItemMessages);
        }

        [TestMethod]
        public void TestER_Label()
        {
            var errors = Assemble("Label.Z80");

            Assert.AreEqual(errors.Length, 1);
            Assert.AreEqual(errors[0].ErrorLineItemMessages.Length, 6);
            AssertErrorItemMessage(Error.ErrorCodeEnum.E0013, 0, errors[0].ErrorLineItemMessages);
            AssertErrorItemMessage(Error.ErrorCodeEnum.E0013, 1, errors[0].ErrorLineItemMessages);
            AssertErrorItemMessage(Error.ErrorCodeEnum.E0013, 2, errors[0].ErrorLineItemMessages);
            AssertErrorItemMessage(Error.ErrorCodeEnum.E0013, 3, errors[0].ErrorLineItemMessages);
            AssertErrorItemMessage(Error.ErrorCodeEnum.E0013, 4, errors[0].ErrorLineItemMessages);
            AssertErrorItemMessage(Error.ErrorCodeEnum.E0013, 5, errors[0].ErrorLineItemMessages);
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

        [TestMethod]
        public void TestER_Repeat()
        {
            var errors = Assemble("Repeat.Z80");

            Assert.AreEqual(errors.Length, 2);
            Assert.AreEqual(errors[0].ErrorLineItemMessages.Length, 1);
            AssertErrorItemMessage(Error.ErrorCodeEnum.E1011, 17, errors[0].ErrorLineItemMessages);
            Assert.AreEqual(errors[1].ErrorLineItemMessages.Length, 3);
            AssertErrorItemMessage(Error.ErrorCodeEnum.E1014, 6, errors[1].ErrorLineItemMessages);
            AssertErrorItemMessage(Error.ErrorCodeEnum.E1012, 15, errors[1].ErrorLineItemMessages);
            AssertErrorItemMessage(Error.ErrorCodeEnum.E0001, 13, errors[1].ErrorLineItemMessages);
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
