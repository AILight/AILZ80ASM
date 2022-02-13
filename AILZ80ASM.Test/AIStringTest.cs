using AILZ80ASM.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace AILZ80ASM.Test
{
    [TestClass]
    public class AIStringTest
    {
        [TestMethod]
        public void IsCharTrueTest()
        {
            Assert.IsTrue(AIString.IsChar("'0'"));
            Assert.IsTrue(AIString.IsChar("'1'"));
            Assert.IsTrue(AIString.IsChar("'2'"));
            Assert.IsTrue(AIString.IsChar("'3'"));
            Assert.IsTrue(AIString.IsChar("'a'"));
            Assert.IsTrue(AIString.IsChar("'b'"));
            Assert.IsTrue(AIString.IsChar("'X'"));
            Assert.IsTrue(AIString.IsChar("'Y'"));
            Assert.IsTrue(AIString.IsChar("'\0'"));
            Assert.IsTrue(AIString.IsChar("'石'"));
            Assert.IsTrue(AIString.IsChar("@SJIS:'Y'"));
        }

        [TestMethod]
        public void IsCharFalseTest()
        {
            Assert.IsFalse(AIString.IsChar("'"));
            Assert.IsFalse(AIString.IsChar("0'"));
            Assert.IsFalse(AIString.IsChar("'00'"));
            Assert.IsFalse(AIString.IsChar("'石野'"));
            Assert.IsFalse(AIString.IsChar("'\r\n'"));
            Assert.IsFalse(AIString.IsChar("@JIS'3'"));
            Assert.IsFalse(AIString.IsChar("JIS::'3'"));
            Assert.IsFalse(AIString.IsChar("\0\""));
            Assert.IsFalse(AIString.IsChar("'''"));

            Assert.IsFalse(AIString.IsChar(":'Y'"));
            Assert.IsFalse(AIString.IsChar("@:'Y'"));
            Assert.IsFalse(AIString.IsChar("JIS:'Y'"));
            Assert.IsFalse(AIString.IsChar("@SJIS'Y'"));
            Assert.IsFalse(AIString.IsChar("@!!!:'Y'"));
        }

        [TestMethod]
        public void IsStringTrueTest()
        {
            Assert.IsTrue(AIString.IsString("\"0\""));
            Assert.IsTrue(AIString.IsString("\"1\""));
            Assert.IsTrue(AIString.IsString("\"2\""));
            Assert.IsTrue(AIString.IsString("\"3\""));
            Assert.IsTrue(AIString.IsString("\"a\""));
            Assert.IsTrue(AIString.IsString("\"b\""));
            Assert.IsTrue(AIString.IsString("\"X\""));
            Assert.IsTrue(AIString.IsString("\"Y\""));
            Assert.IsTrue(AIString.IsString("\"\n\r\""));
            Assert.IsTrue(AIString.IsString("\"\0\""));
            Assert.IsTrue(AIString.IsString("\"石\""));
            Assert.IsTrue(AIString.IsString("@SJIS:\"ABC\""));

            Assert.IsTrue(AIString.IsString("\"00\""));
            Assert.IsTrue(AIString.IsString("\"石野\""));
            Assert.IsTrue(AIString.IsString("\"\r\n\""));
        }

        [TestMethod]
        public void IsStringFalseTest()
        {
            Assert.IsFalse(AIString.IsString("\""));
            Assert.IsFalse(AIString.IsString("ABC\""));
            Assert.IsFalse(AIString.IsString("\"'''''"));
            Assert.IsFalse(AIString.IsString("\"'''''\\\""));

            Assert.IsFalse(AIString.IsString(":\"ABC\""));
            Assert.IsFalse(AIString.IsString("@:\"ABC\""));
            Assert.IsFalse(AIString.IsString("SJIS:\"ABC\""));
            Assert.IsFalse(AIString.IsString("@SJIS\"ABC\""));
            Assert.IsFalse(AIString.IsString("@!!!:\"ABC\""));
        }

        [TestMethod]
        public void EscapeSequenceTest()
        {
            Assert.AreEqual(AIString.EscapeSequence("\\'"), "'");
            Assert.AreEqual(AIString.EscapeSequence("\\\""), "\"");
            Assert.AreEqual(AIString.EscapeSequence("\\\\"), "\\");
            Assert.AreEqual(AIString.EscapeSequence("\\0"), "\0");
            Assert.AreEqual(AIString.EscapeSequence("\\a"), "\a");
            Assert.AreEqual(AIString.EscapeSequence("\\b"), "\b");
            Assert.AreEqual(AIString.EscapeSequence("\\f"), "\f");
            Assert.AreEqual(AIString.EscapeSequence("\\n"), "\n");
            Assert.AreEqual(AIString.EscapeSequence("\\r"), "\r");
            Assert.AreEqual(AIString.EscapeSequence("\\t"), "\t");
            Assert.AreEqual(AIString.EscapeSequence("\\v"), "\v");

            Assert.AreEqual(AIString.EscapeSequence("[\\'\\\"\\\\\\0\\a\\b\\f\\n\\r\\t\\v]"), "['\"\\\0\a\b\f\n\r\t\v]");
        }

        [TestMethod]
        public void TryParseCharMapTrueTest()
        {
            {
                Assert.IsTrue(AIString.TryParseCharMap("\'A\'", out var charMap, out var resultString));
                Assert.AreEqual(charMap, "");
                Assert.AreEqual(resultString, "A");
            }

            {
                Assert.IsTrue(AIString.TryParseCharMap("\"ABC\"", out var charMap, out var resultString));
                Assert.AreEqual(charMap, "");
                Assert.AreEqual(resultString, "ABC");
            }

            {
                Assert.IsTrue(AIString.TryParseCharMap("@SJIS:\"ABC\"", out var charMap, out var resultString));
                Assert.AreEqual(charMap, "SJIS");
                Assert.AreEqual(resultString, "ABC");
            }
        }

        [TestMethod]
        public void GetBytesByCharTest()
        {
            {
                var asmLoad = new AsmLoad(new InstructionSet.Z80());
                var bytes = AIString.GetBytesByChar("'A'", asmLoad);
                Assert.AreEqual(bytes[0], 0x41);
            }

            {
                var asmLoad = new AsmLoad(new InstructionSet.Z80());
                var bytes = AIString.GetBytesByChar("'石'", asmLoad);
                Assert.AreEqual(bytes[0], 0xCE);
                Assert.AreEqual(bytes[1], 0x90);
            }

            {
                var asmLoad = new AsmLoad(new InstructionSet.Z80());
                var bytes = AIString.GetBytesByChar("@SJIS:'野'", asmLoad);
                Assert.AreEqual(bytes[0], 0xEC);
                Assert.AreEqual(bytes[1], 0x96);
            }
        }

        [TestMethod]
        public void GetBytesByStringTest()
        {
            {
                var asmLoad = new AsmLoad(new InstructionSet.Z80());
                var bytes = AIString.GetBytesByString("\"A\"", asmLoad);
                Assert.AreEqual(bytes[0], 0x41);
            }

            {
                var asmLoad = new AsmLoad(new InstructionSet.Z80());
                var bytes = AIString.GetBytesByString("\"石野\"", asmLoad);
                Assert.AreEqual(bytes[0], 0xCE);
                Assert.AreEqual(bytes[1], 0x90);
                Assert.AreEqual(bytes[2], 0xEC);
                Assert.AreEqual(bytes[3], 0x96);
            }

            {
                var asmLoad = new AsmLoad(new InstructionSet.Z80());
                var bytes = AIString.GetBytesByString("@SJIS:\"石野\"", asmLoad);
                Assert.AreEqual(bytes[0], 0xCE);
                Assert.AreEqual(bytes[1], 0x90);
                Assert.AreEqual(bytes[2], 0xEC);
                Assert.AreEqual(bytes[3], 0x96);
            }
        }

        [TestMethod]
        public void GetBytesByStringThrowTest()
        {
            {
                Assert.ThrowsException<InvalidAIStringException>(() => {
                    var asmLoad = new AsmLoad(new InstructionSet.Z80());
                    AIString.GetBytesByString("@SJIS:\'石野\'", asmLoad);
                });
            }

            {
                Assert.ThrowsException<InvalidAIStringException>(() => {
                    var asmLoad = new AsmLoad(new InstructionSet.Z80());
                    AIString.GetBytesByString("@SJIS \"石野\"", asmLoad);
                });
            }
        }
    }
}
