using AILZ80ASM.AILight;
using AILZ80ASM.Assembler;
using AILZ80ASM.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Data;
using System.Formats.Asn1;
using System.Formats.Tar;

namespace AILZ80ASM.Test
{
    [TestClass]
    public class AIStringTest
    {
        [TestMethod]
        [DataRow("'0'")]
        [DataRow("'1'")]
        [DataRow("'2'")]
        [DataRow("'3'")]
        [DataRow("'a'")]
        [DataRow("'b'")]
        [DataRow("'X'")]
        [DataRow("'Y'")]
        [DataRow("'\0'")]
        [DataRow("'石'")]
        [DataRow("@SJIS:'Y'")]
        [DataRow("@SJIS:@'\\'")]
        public void IsCharTrueTest(string target)
        {
            var asmLoad = new AsmLoad(new AsmOption(), new InstructionSet.Z80());
            Assert.IsTrue(AIString.IsChar(target, asmLoad));
        }

        [TestMethod]
        [DataRow("'")]
        [DataRow("0'")]
        [DataRow("'00'")]
        [DataRow("'石野'")]
        [DataRow("'\r\n'")]
        [DataRow("@JIS'3'")]
        [DataRow("JIS::'3'")]
        [DataRow("\0\"")]
        [DataRow("'''")]
        [DataRow("'YYY'")]
        [DataRow(":'Y'")]
        [DataRow("@:'Y'")]
        [DataRow("JIS:'Y'")]
        [DataRow("@SJIS'Y'")]
        [DataRow("@!!!:'Y'")]
        [DataRow("@SJIS:'\\'")]
        public void IsCharFalseTest(string target)
        {
            var asmLoad = new AsmLoad(new AsmOption(), new InstructionSet.Z80());
            Assert.IsFalse(AIString.IsChar(target, asmLoad));
        }

        [TestMethod]
        [DataRow("\"0\"")]
        [DataRow("\"1\"")]
        [DataRow("\"2\"")]
        [DataRow("\"3\"")]
        [DataRow("\"a\"")]
        [DataRow("\"b\"")]
        [DataRow("\"X\"")]
        [DataRow("\"Y\"")]
        [DataRow("\"\n\r\"")]
        [DataRow("\"\0\"")]
        [DataRow("\"石\"")]
        [DataRow("@SJIS:\"ABC\"")]
        [DataRow("@SJIS:@\"ABC\\\"")]
        [DataRow("'ABC'")]
        [DataRow("'A\"B\"C'")]
        [DataRow("\"A'B'C\"")]
        [DataRow("\"00\"")]
        [DataRow("\"石野\"")]
        [DataRow("\"\r\n\"")]
        [DataRow("\"ABC \\\"DEF\\\" GHI\"")]
        [DataRow("@SJIS:\"012:\"")]
        [DataRow("@\"012:\"")]
        [DataRow("@'012:'")]
        [DataRow("@'012:\\'")]
        public void IsStringTrueTest(string target)
        {
            var asmLoad = new AsmLoad(new AsmOption(), new InstructionSet.Z80());
            Assert.IsTrue(AIString.IsString(target, asmLoad));
        }

        [TestMethod]
        [DataRow("'\\q12'")]
        [DataRow("@'\\q12'")]
        [DataRow("\"\\q12\"")]
        [DataRow("@\"\\q12\"")]
        public void IsStringTrueTestForCompat(string target)
        {
            var asmLoad = new AsmLoad(new AsmOption() { CompatRawString = true }, new InstructionSet.Z80());
            Assert.IsTrue(AIString.IsString(target, asmLoad));
        }

        [TestMethod]
        [DataRow("ABC")]
        [DataRow("\"")]
        [DataRow("\"1\"2\"")]
        [DataRow("ABC\"")]
        [DataRow("\"'''''")]
        [DataRow("\"'''''\\\"")]
        [DataRow(":\"ABC\"")]
        [DataRow("@:\"ABC\"")]
        [DataRow("SJIS:\"ABC\"")]
        [DataRow("@SJIS\"ABC\"")]
        [DataRow("@!!!:\"ABC\"")]
        [DataRow("@\"012\":\"")]
        [DataRow("@\"012\\\":\"")]
        [DataRow("@'012\\':'")]
        [DataRow("@'012:\\''")]
        [DataRow("@\"012:\\\"\"")]
        public void IsStringFalseTest(string target)
        {
            var asmLoad = new AsmLoad(new AsmOption(), new InstructionSet.Z80());
            Assert.IsFalse(AIString.IsString(target, asmLoad));
        }

        [TestMethod]
        [DataRow("LD A, (123)")]
        [DataRow("DB 123, \"全角\", 123")]
        [DataRow("DB \"ABC \\\"DEF\\\" GHI\", $00")]
        public void IsCorrectOperationForIsTrue(string target)
        {
            Assert.IsTrue(AIString.IsCorrectOperation(target));
        }

        [TestMethod]
        [DataRow("LD A, （123)")]
        [DataRow("LD A, (123）")]
        [DataRow("LD A,　‘(123）")]
        public void IsCorrectOperationForIsFalse(string target)
        {
            Assert.IsFalse(AIString.IsCorrectOperation(target));
        }

        [TestMethod]
        [DataRow("'", "\\'")]
        [DataRow("\"", "\\\"")]
        [DataRow("\\", "\\\\")]
        [DataRow("\0", "\\0")]
        [DataRow("\a", "\\a")]
        [DataRow("\b", "\\b")]
        [DataRow("\f", "\\f")]
        [DataRow("\n", "\\n")]
        [DataRow("\r", "\\r")]
        [DataRow("\t", "\\t")]
        [DataRow("\v", "\\v")]
        [DataRow("['\"\\\0\a\b\f\n\r\t\v]", "[\\'\\\"\\\\\\0\\a\\b\\f\\n\\r\\t\\v]")]
        [DataRow("ABC \"DEF\" GHI", "ABC \\\"DEF\\\" GHI")]
        public void EscapeSequenceTest(string expected, string actual)
        {
            Assert.AreEqual(expected, AIString.EscapeSequence(actual));
        }

        [TestMethod]
        [DataRow("\\")]
        [DataRow("\\c")]
        [DataRow("\\w")]
        [DataRow("AB\\LCD")]
        public void ValidEscapeSequenceTest(string target)
        {
            Assert.IsFalse(AIString.ValidEscapeSequence(target));
        }

        [TestMethod]
        [DataRow("ABC", "\"ABC\"")]
        [DataRow("ABC", "'ABC'")]
        [DataRow("ABC\n", "\"ABC\n\"")]
        [DataRow("ABC\n", "'ABC\n'")]
        [DataRow(@"ABC\n", @"@""ABC\n""")]
        [DataRow(@"ABC\n", @"@'ABC\n'")]
        [DataRow(@"ABC""\n", @"@""ABC""\n""")]
        public void TryParseCharMapResultStringTest(string expected, string target)
        {
            var asmLoad = new AsmLoad(new AsmOption(), new InstructionSet.Z80());
            Assert.IsTrue(AIString.TryParseCharMap(target, asmLoad, out var charMap, out var resultString, out var validEscapeSequence));
            Assert.IsTrue(validEscapeSequence);
            Assert.AreEqual(expected, resultString);
        }

        [TestMethod]
        public void TryParseCharMapTrueTest()
        {
            {
                var asmLoad = new AsmLoad(new AsmOption(), new InstructionSet.Z80());
                Assert.IsTrue(AIString.TryParseCharMap("\'A\'", asmLoad, out var charMap, out var resultString, out var validEscapeSequence));
                Assert.AreEqual("", charMap);
                Assert.AreEqual("A", resultString);
            }

            {
                var asmLoad = new AsmLoad(new AsmOption(), new InstructionSet.Z80());
                Assert.IsTrue(AIString.TryParseCharMap("\"ABC\"", asmLoad, out var charMap, out var resultString, out var validEscapeSequence));
                Assert.AreEqual("", charMap);
                Assert.AreEqual("ABC", resultString);
            }

            {
                var asmLoad = new AsmLoad(new AsmOption(), new InstructionSet.Z80());
                Assert.IsTrue(AIString.TryParseCharMap("\"ABC \\\"DEF\\\" GHI\"", asmLoad, out var charMap, out var resultString, out var validEscapeSequence));
                Assert.AreEqual("", charMap);
                Assert.AreEqual("ABC \"DEF\" GHI", resultString);
            }

            {
                var asmLoad = new AsmLoad(new AsmOption(), new InstructionSet.Z80());
                asmLoad.CharMapConverter_ReadCharMapFromResource("@SJIS");

                Assert.IsTrue(AIString.TryParseCharMap("@SJIS:\"ABC\"", asmLoad, out var charMap, out var resultString, out var validEscapeSequence));
                Assert.AreEqual("@SJIS", charMap);
                Assert.AreEqual("ABC", resultString);
            }
        }

        [TestMethod]
        public void GetBytesByCharTest()
        {
            {
                var asmLoad = new AsmLoad(new AsmOption(), new InstructionSet.Z80());

                var bytes = AIString.GetBytesByChar("'A'", asmLoad);
                Assert.AreEqual(0x41, bytes[0]);
            }

            {
                var asmLoad = new AsmLoad(new AsmOption(), new InstructionSet.Z80());

                var bytes = AIString.GetBytesByChar("'石'", asmLoad);
                Assert.AreEqual(0x90, bytes[0]);
                Assert.AreEqual(0xCE, bytes[1]);
            }

            {
                var asmLoad = new AsmLoad(new AsmOption(), new InstructionSet.Z80());

                var bytes = AIString.GetBytesByChar("@SJIS:'野'", asmLoad);
                Assert.AreEqual(0x96, bytes[0]);
                Assert.AreEqual(0xEC, bytes[1]);
            }
        }

        [TestMethod]
        [DataRow(new byte[] { 0x41 }, "\"A\"")]
        [DataRow(new byte[] { 0x90, 0xCE, 0x96, 0xEC }, "\"石野\"")]
        [DataRow(new byte[] { 0x90, 0xCE, 0x96, 0xEC }, "@SJIS:\"石野\"")]
        [DataRow(new byte[] { 0x5C, 0x30, 0x30, 0x30, 0x30 }, "\"\\\\0000\"")]
        [DataRow(new byte[] { 0x41 }, "'A'")]
        [DataRow(new byte[] { 0x90, 0xCE, 0x96, 0xEC }, "'石野'")]
        [DataRow(new byte[] { 0x90, 0xCE, 0x96, 0xEC }, "@SJIS:'石野'")]
        [DataRow(new byte[] { 0x5C, 0x30, 0x30, 0x30, 0x30 }, "'\\\\0000'")]
        [DataRow(new byte[] { 0x41 }, "\'A\'")]
        [DataRow(new byte[] { 0x90, 0xCE, 0x96, 0xEC }, "\'石野\'")]
        [DataRow(new byte[] { 0x90, 0xCE, 0x96, 0xEC }, "@SJIS:\'石野\'")]
        [DataRow(new byte[] { 0x5C, 0x30, 0x30, 0x30, 0x30 }, "\'\\\\0000\'")]
        public void GetBytesByStringTest(byte[] expected, string target)
        {
            var asmLoad = new AsmLoad(new AsmOption(), new InstructionSet.Z80());

            var actual = AIString.GetBytesByString(target, asmLoad);

            CollectionAssert.AreEqual(expected, actual);

        }

        [TestMethod]
        public void GetBytesByStringThrowTest()
        {
            {
                Assert.ThrowsException<InvalidAIStringException>(() => {
                    var asmLoad = new AsmLoad(new AsmOption(), new InstructionSet.Z80());
                    asmLoad.CharMapConverter_ReadCharMapFromResource("@SJIS");

                    AIString.GetBytesByString("@SJIS \"石野\"", asmLoad);
                });
            }
        }

        [TestMethod]
        public void IndexOfSkipStringTest()
        {
            Assert.AreEqual(-1, AIString.IndexOfSkipString("ABC \"aaa,bbb\"", ','));
            Assert.AreEqual(11, AIString.IndexOfSkipString("EX AF,AF'  ; ABC", ';'));
            Assert.AreEqual(10, AIString.IndexOfSkipString("LD A,'\0'  ; ABC", ';'));

            // 開始位置のテスト
            Assert.AreEqual(0,  AIString.IndexOfSkipString("ABC \"A\\\"BC\", A,B,C", 'A', 0));
            Assert.AreEqual(13, AIString.IndexOfSkipString("ABC \"A\\\"BC\", A,B,C", 'A', 1));
        }

        [TestMethod]
        public void IndexOfAnySkipStringTest()
        {
            Assert.AreEqual(-1, AIString.IndexOfAnySkipString("ABC \"aaa,bbb\"", new[] { ',', '(' }));
            Assert.AreEqual(11, AIString.IndexOfAnySkipString("EX AF,AF'  ; ABC", new[] { ';', '(' }));
            Assert.AreEqual(10, AIString.IndexOfAnySkipString("LD A,'\0'  ; ABC", new[] { ';', '(' }));

            // 開始位置のテスト
            Assert.AreEqual(1,  AIString.IndexOfAnySkipString("ABC \"A\\\"BC\", A,B,C", new[] { 'C', 'B' }, 0, out var _));
            Assert.AreEqual(1,  AIString.IndexOfAnySkipString("ABC \"A\\\"BC\", A,B,C", new[] { 'C', 'B' }, 1, out var _));
            Assert.AreEqual(15, AIString.IndexOfAnySkipString("ABC \"A\\\"BC\", A,B,C", new[] { 'C', 'B' }, 3, out var _));
        }
    }
}
