using AILZ80ASM.AILight;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace AILZ80ASM.Test
{
    [TestClass]
    public class AIEncodeTest
    {
        [TestMethod]
        public void SHIFT_JIS_TEST()
        {
            var path = Path.Combine(".", "Test", "AIEncode", "SHIFT_JIS.txt");
            var bytes = File.ReadAllBytes(path);
            Assert.IsTrue(AIEncode.IsSHIFT_JIS(bytes));
            Assert.IsFalse(AIEncode.IsUTF8(bytes));
        }

        [TestMethod]
        public void UTF_8_TEST()
        {
            var path = Path.Combine(".", "Test", "AIEncode", "UTF-8.txt");
            var bytes = File.ReadAllBytes(path);
            Assert.IsFalse(AIEncode.IsSHIFT_JIS(bytes));
            Assert.IsTrue(AIEncode.IsUTF8(bytes));
        }


        [TestMethod]
        public void UTF_8_W_BOM_TEST()
        {
            var path = Path.Combine(".", "Test", "AIEncode", "UTF-8-W-BOM.txt");
            var bytes = File.ReadAllBytes(path);
            Assert.IsFalse(AIEncode.IsSHIFT_JIS(bytes));
            Assert.IsTrue(AIEncode.IsUTF8(bytes));
        }

        [TestMethod]
        public void GetStringUTF8Test()
        {
            var path = Path.Combine(".", "Test", "AIEncode", "UTF-8.txt");
            var bytes = File.ReadAllBytes(path);
            var originalString = File.ReadAllText(path, AIEncode.GetEncodingUTF8());
            var testString = AIEncode.GetString(bytes);
            
            Assert.AreEqual(originalString, testString);
        }

        [TestMethod]
        public void GetStringSJISTest()
        {
            var path = Path.Combine(".", "Test", "AIEncode", "SHIFT_JIS.txt");
            var bytes = File.ReadAllBytes(path);
            var originalString = File.ReadAllText(path, AIEncode.GetEncodingSJIS());
            var testString = AIEncode.GetString(bytes);

            Assert.AreEqual(originalString, testString);
        }
    }
}
