using AILZ80ASM.AILight;
using AILZ80ASM.Assembler;
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
            var asmLoad = new AsmLoad(new AsmOption(), new InstructionSet.Z80());

            Assert.IsTrue(AIString.IsChar("'0'", asmLoad));
            Assert.IsTrue(AIString.IsChar("'1'", asmLoad));
            Assert.IsTrue(AIString.IsChar("'2'", asmLoad));
            Assert.IsTrue(AIString.IsChar("'3'", asmLoad));
            Assert.IsTrue(AIString.IsChar("'a'", asmLoad));
            Assert.IsTrue(AIString.IsChar("'b'", asmLoad));
            Assert.IsTrue(AIString.IsChar("'X'", asmLoad));
            Assert.IsTrue(AIString.IsChar("'Y'", asmLoad));
            Assert.IsTrue(AIString.IsChar("'\0'", asmLoad));
            Assert.IsTrue(AIString.IsChar("'石'", asmLoad));
            Assert.IsTrue(AIString.IsChar("@SJIS:'Y'", asmLoad));
        }

        [TestMethod]
        public void IsCharFalseTest()
        {
            var asmLoad = new AsmLoad(new AsmOption(), new InstructionSet.Z80());

            Assert.IsFalse(AIString.IsChar("'", asmLoad));
            Assert.IsFalse(AIString.IsChar("0'", asmLoad));
            Assert.IsFalse(AIString.IsChar("'00'", asmLoad));
            Assert.IsFalse(AIString.IsChar("'石野'", asmLoad));
            Assert.IsFalse(AIString.IsChar("'\r\n'", asmLoad));
            Assert.IsFalse(AIString.IsChar("@JIS'3'", asmLoad));
            Assert.IsFalse(AIString.IsChar("JIS::'3'", asmLoad));
            Assert.IsFalse(AIString.IsChar("\0\"", asmLoad));
            Assert.IsFalse(AIString.IsChar("'''", asmLoad));

            Assert.IsFalse(AIString.IsChar("'YYY'", asmLoad));
            Assert.IsFalse(AIString.IsChar(":'Y'", asmLoad));
            Assert.IsFalse(AIString.IsChar("@:'Y'", asmLoad));
            Assert.IsFalse(AIString.IsChar("JIS:'Y'", asmLoad));
            Assert.IsFalse(AIString.IsChar("@SJIS'Y'", asmLoad));
            Assert.IsFalse(AIString.IsChar("@!!!:'Y'", asmLoad));
        }

        [TestMethod]
        public void IsStringTrueTest()
        {
            var asmLoad = new AsmLoad(new AsmOption(), new InstructionSet.Z80());

            Assert.IsTrue(AIString.IsString("\"0\"", asmLoad));
            Assert.IsTrue(AIString.IsString("\"1\"", asmLoad));
            Assert.IsTrue(AIString.IsString("\"2\"", asmLoad));
            Assert.IsTrue(AIString.IsString("\"3\"", asmLoad));
            Assert.IsTrue(AIString.IsString("\"a\"", asmLoad));
            Assert.IsTrue(AIString.IsString("\"b\"", asmLoad));
            Assert.IsTrue(AIString.IsString("\"X\"", asmLoad));
            Assert.IsTrue(AIString.IsString("\"Y\"", asmLoad));
            Assert.IsTrue(AIString.IsString("\"\n\r\"", asmLoad));
            Assert.IsTrue(AIString.IsString("\"\0\"", asmLoad));
            Assert.IsTrue(AIString.IsString("\"石\"", asmLoad));
            Assert.IsTrue(AIString.IsString("@SJIS:\"ABC\"", asmLoad));
            Assert.IsTrue(AIString.IsString("'ABC'", asmLoad));
            Assert.IsTrue(AIString.IsString("'A\"B\"C'", asmLoad));
            Assert.IsTrue(AIString.IsString("\"A'B'C\"", asmLoad));

            Assert.IsTrue(AIString.IsString("\"00\"", asmLoad));
            Assert.IsTrue(AIString.IsString("\"石野\"", asmLoad));
            Assert.IsTrue(AIString.IsString("\"\r\n\"", asmLoad));
        }

        [TestMethod]
        public void IsStringFalseTest()
        {
            var asmLoad = new AsmLoad(new AsmOption(), new InstructionSet.Z80());

            Assert.IsFalse(AIString.IsString("\"", asmLoad));
            Assert.IsFalse(AIString.IsString("ABC\"", asmLoad));
            Assert.IsFalse(AIString.IsString("\"'''''", asmLoad));
            Assert.IsFalse(AIString.IsString("\"'''''\\\"", asmLoad));

            Assert.IsFalse(AIString.IsString(":\"ABC\"", asmLoad));
            Assert.IsFalse(AIString.IsString("@:\"ABC\"", asmLoad));
            Assert.IsFalse(AIString.IsString("SJIS:\"ABC\"", asmLoad));
            Assert.IsFalse(AIString.IsString("@SJIS\"ABC\"", asmLoad));
            Assert.IsFalse(AIString.IsString("@!!!:\"ABC\"", asmLoad));
        }

        [TestMethod]
        public void IsCorrectOperation()
        {
            Assert.IsTrue(AIString.IsCorrectOperation("LD A, (123)"));
            Assert.IsTrue(AIString.IsCorrectOperation("DB 123, \"全角\", 123"));
            Assert.IsFalse(AIString.IsCorrectOperation("LD A, （123)"));
            Assert.IsFalse(AIString.IsCorrectOperation("LD A, (123）"));
            Assert.IsFalse(AIString.IsCorrectOperation("LD A,　‘(123）"));
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
        public void ValidEscapeSequenceTest()
        {
            Assert.IsFalse(AIString.ValidEscapeSequence("\\"));
            Assert.IsFalse(AIString.ValidEscapeSequence("\\c"));
            Assert.IsFalse(AIString.ValidEscapeSequence("\\w"));
        }

        [TestMethod]
        public void TryParseCharMapTrueTest()
        {
            {
                var asmLoad = new AsmLoad(new AsmOption(), new InstructionSet.Z80());
                Assert.IsTrue(AIString.TryParseCharMap("\'A\'", asmLoad, out var charMap, out var resultString, out var validEscapeSequence));
                Assert.AreEqual(charMap, "");
                Assert.AreEqual(resultString, "A");
            }

            {
                var asmLoad = new AsmLoad(new AsmOption(), new InstructionSet.Z80());
                Assert.IsTrue(AIString.TryParseCharMap("\"ABC\"", asmLoad, out var charMap, out var resultString, out var validEscapeSequence));
                Assert.AreEqual(charMap, "");
                Assert.AreEqual(resultString, "ABC");
            }

            {
                var asmLoad = new AsmLoad(new AsmOption(), new InstructionSet.Z80());
                asmLoad.CharMapConverter_ReadCharMapFromResource("@SJIS");

                Assert.IsTrue(AIString.TryParseCharMap("@SJIS:\"ABC\"", asmLoad, out var charMap, out var resultString, out var validEscapeSequence));
                Assert.AreEqual(charMap, "@SJIS");
                Assert.AreEqual(resultString, "ABC");
            }
        }

        [TestMethod]
        public void GetBytesByCharTest()
        {
            {
                var asmLoad = new AsmLoad(new AsmOption(), new InstructionSet.Z80());

                var bytes = AIString.GetBytesByChar("'A'", asmLoad);
                Assert.AreEqual(bytes[0], 0x41);
            }

            {
                var asmLoad = new AsmLoad(new AsmOption(), new InstructionSet.Z80());

                var bytes = AIString.GetBytesByChar("'石'", asmLoad);
                Assert.AreEqual(bytes[0], 0x90);
                Assert.AreEqual(bytes[1], 0xCE);
            }

            {
                var asmLoad = new AsmLoad(new AsmOption(), new InstructionSet.Z80());

                var bytes = AIString.GetBytesByChar("@SJIS:'野'", asmLoad);
                Assert.AreEqual(bytes[0], 0x96);
                Assert.AreEqual(bytes[1], 0xEC);
            }
        }

        [TestMethod]
        public void GetBytesByStringTest()
        {
            // ダブルクオーテーション
            {
                var asmLoad = new AsmLoad(new AsmOption(), new InstructionSet.Z80());

                var bytes = AIString.GetBytesByString("\"A\"", asmLoad);
                Assert.AreEqual(bytes[0], 0x41);
            }


            {
                var asmLoad = new AsmLoad(new AsmOption(), new InstructionSet.Z80());

                var bytes = AIString.GetBytesByString("\"石野\"", asmLoad);
                Assert.AreEqual(bytes[0], 0x90);
                Assert.AreEqual(bytes[1], 0xCE);
                Assert.AreEqual(bytes[2], 0x96);
                Assert.AreEqual(bytes[3], 0xEC);
            }

            {
                var asmLoad = new AsmLoad(new AsmOption(), new InstructionSet.Z80());

                var bytes = AIString.GetBytesByString("@SJIS:\"石野\"", asmLoad);
                Assert.AreEqual(bytes[0], 0x90);
                Assert.AreEqual(bytes[1], 0xCE);
                Assert.AreEqual(bytes[2], 0x96);
                Assert.AreEqual(bytes[3], 0xEC);
            }


            // シングルクオーテーション
            {
                var asmLoad = new AsmLoad(new AsmOption(), new InstructionSet.Z80());

                var bytes = AIString.GetBytesByString("\'A\'", asmLoad);
                Assert.AreEqual(bytes[0], 0x41);
            }

            {
                var asmLoad = new AsmLoad(new AsmOption(), new InstructionSet.Z80());

                var bytes = AIString.GetBytesByString("\'石野\'", asmLoad);
                Assert.AreEqual(bytes[0], 0x90);
                Assert.AreEqual(bytes[1], 0xCE);
                Assert.AreEqual(bytes[2], 0x96);
                Assert.AreEqual(bytes[3], 0xEC);
            }

            {
                var asmLoad = new AsmLoad(new AsmOption(), new InstructionSet.Z80());

                var bytes = AIString.GetBytesByString("@SJIS:\'石野\'", asmLoad);
                Assert.AreEqual(bytes[0], 0x90);
                Assert.AreEqual(bytes[1], 0xCE);
                Assert.AreEqual(bytes[2], 0x96);
                Assert.AreEqual(bytes[3], 0xEC);
            }
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
            Assert.AreEqual(AIString.IndexOfSkipString("ABC \"aaa,bbb\"", ','), -1);
            Assert.AreEqual(AIString.IndexOfSkipString("EX AF,AF'  ; ABC", ';'), 11);
            Assert.AreEqual(AIString.IndexOfSkipString("LD A,'\0'  ; ABC", ';'), 10);

            // 開始位置のテスト
            Assert.AreEqual(AIString.IndexOfSkipString("ABC \"A\\\"BC\", A,B,C", 'A', 0), 0);
            Assert.AreEqual(AIString.IndexOfSkipString("ABC \"A\\\"BC\", A,B,C", 'A', 1), 13);
        }

        [TestMethod]
        public void IndexOfAnySkipStringTest()
        {
            Assert.AreEqual(AIString.IndexOfAnySkipString("ABC \"aaa,bbb\"", new[] { ',', '(' }), -1);
            Assert.AreEqual(AIString.IndexOfAnySkipString("EX AF,AF'  ; ABC", new[] { ';', '(' }), 11);
            Assert.AreEqual(AIString.IndexOfAnySkipString("LD A,'\0'  ; ABC", new[] { ';', '(' }), 10);

            // 開始位置のテスト
            Assert.AreEqual(AIString.IndexOfAnySkipString("ABC \"A\\\"BC\", A,B,C", new[] { 'C', 'B' }, 0, out var _), 1);
            Assert.AreEqual(AIString.IndexOfAnySkipString("ABC \"A\\\"BC\", A,B,C", new[] { 'C', 'B' }, 1, out var _), 1);
            Assert.AreEqual(AIString.IndexOfAnySkipString("ABC \"A\\\"BC\", A,B,C", new[] { 'C', 'B' }, 3, out var _), 15);
        }


    }
}
