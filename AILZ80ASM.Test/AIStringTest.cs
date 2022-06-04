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
            Assert.AreEqual("'", AIString.EscapeSequence("\\'"));
            Assert.AreEqual("\"", AIString.EscapeSequence("\\\""));
            Assert.AreEqual("\\", AIString.EscapeSequence("\\\\"));
            Assert.AreEqual("\0", AIString.EscapeSequence("\\0"));
            Assert.AreEqual("\a", AIString.EscapeSequence("\\a"));
            Assert.AreEqual("\b", AIString.EscapeSequence("\\b"));
            Assert.AreEqual("\f", AIString.EscapeSequence("\\f"));
            Assert.AreEqual("\n", AIString.EscapeSequence("\\n"));
            Assert.AreEqual("\r", AIString.EscapeSequence("\\r"));
            Assert.AreEqual("\t", AIString.EscapeSequence("\\t"));
            Assert.AreEqual("\v", AIString.EscapeSequence("\\v"));

            Assert.AreEqual("['\"\\\0\a\b\f\n\r\t\v]", AIString.EscapeSequence("[\\'\\\"\\\\\\0\\a\\b\\f\\n\\r\\t\\v]"));
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
        public void GetBytesByStringTest()
        {
            // ダブルクオーテーション
            {
                var asmLoad = new AsmLoad(new AsmOption(), new InstructionSet.Z80());

                var bytes = AIString.GetBytesByString("\"A\"", asmLoad);
                Assert.AreEqual(0x41, bytes[0]);
            }


            {
                var asmLoad = new AsmLoad(new AsmOption(), new InstructionSet.Z80());

                var bytes = AIString.GetBytesByString("\"石野\"", asmLoad);
                Assert.AreEqual(0x90, bytes[0]);
                Assert.AreEqual(0xCE, bytes[1]);
                Assert.AreEqual(0x96, bytes[2]);
                Assert.AreEqual(0xEC, bytes[3]);
            }

            {
                var asmLoad = new AsmLoad(new AsmOption(), new InstructionSet.Z80());

                var bytes = AIString.GetBytesByString("@SJIS:\"石野\"", asmLoad);
                Assert.AreEqual(0x90, bytes[0]);
                Assert.AreEqual(0xCE, bytes[1]);
                Assert.AreEqual(0x96, bytes[2]);
                Assert.AreEqual(0xEC, bytes[3]);
            }


            // シングルクオーテーション
            {
                var asmLoad = new AsmLoad(new AsmOption(), new InstructionSet.Z80());

                var bytes = AIString.GetBytesByString("\'A\'", asmLoad);
                Assert.AreEqual(0x41, bytes[0]);
            }

            {
                var asmLoad = new AsmLoad(new AsmOption(), new InstructionSet.Z80());

                var bytes = AIString.GetBytesByString("\'石野\'", asmLoad);
                Assert.AreEqual(0x90, bytes[0]);
                Assert.AreEqual(0xCE, bytes[1]);
                Assert.AreEqual(0x96, bytes[2]);
                Assert.AreEqual(0xEC, bytes[3]);
            }

            {
                var asmLoad = new AsmLoad(new AsmOption(), new InstructionSet.Z80());

                var bytes = AIString.GetBytesByString("@SJIS:\'石野\'", asmLoad);
                Assert.AreEqual(0x90, bytes[0]);
                Assert.AreEqual(0xCE, bytes[1]);
                Assert.AreEqual(0x96, bytes[2]);
                Assert.AreEqual(0xEC, bytes[3]);
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
