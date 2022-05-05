using AILZ80ASM.AILight;
using AILZ80ASM.Assembler;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace AILZ80ASM.Test
{
    [TestClass]
    public class AINameTest
    {
        [TestMethod]
        public void ArgumentTest_01()
        {
            var arguments = AIName.ParseArguments("");

            Assert.AreEqual(arguments.Length, 0);
        }

        [TestMethod]
        public void ArgumentTest_02()
        {
            var arguments = AIName.ParseArguments("ABC,FUNC(),DEF");

            Assert.AreEqual(arguments[0], "ABC");
            Assert.AreEqual(arguments[1], "FUNC()");
            Assert.AreEqual(arguments[2], "DEF");
        }

        [TestMethod]
        public void ArgumentTest_03()
        {
            var arguments = AIName.ParseArguments("ABC,FUNC(GHI),DEF");

            Assert.AreEqual(arguments[0], "ABC");
            Assert.AreEqual(arguments[1], "FUNC(GHI)");
            Assert.AreEqual(arguments[2], "DEF");
        }

        [TestMethod]
        public void ArgumentTest_04()
        {
            var arguments = AIName.ParseArguments("ABC,FUNC(GHI,JKL),DEF");

            Assert.AreEqual(arguments[0], "ABC");
            Assert.AreEqual(arguments[1], "FUNC(GHI,JKL)");
            Assert.AreEqual(arguments[2], "DEF");
        }

        [TestMethod]
        public void ArgumentTest_05()
        {
            var arguments = AIName.ParseArguments("ABC,FUNC(GHI,JKL,FUNC(GHI,JKL)),DEF");

            Assert.AreEqual(arguments[0], "ABC");
            Assert.AreEqual(arguments[1], "FUNC(GHI,JKL,FUNC(GHI,JKL))");
            Assert.AreEqual(arguments[2], "DEF");
        }

        [TestMethod]
        public void ArgumentTest_06()
        {
            var arguments = AIName.ParseArguments("ABC + TTT, FUNC(GHI,JKL,FUNC(GHI,JKL)) , DEF");

            Assert.AreEqual(arguments[0], "ABC + TTT");
            Assert.AreEqual(arguments[1], "FUNC(GHI,JKL,FUNC(GHI,JKL))");
            Assert.AreEqual(arguments[2], "DEF");
        }

        [TestMethod]
        public void ArgumentTest_07()
        {
            var arguments = AIName.ParseArguments("\"ABC\", 123, \"ABC,DEF\", \"ABC\\\"DEF\"");

            Assert.AreEqual(arguments[0], "\"ABC\"");
            Assert.AreEqual(arguments[1], "123");
            Assert.AreEqual(arguments[2], "\"ABC,DEF\"");
            Assert.AreEqual(arguments[3], "\"ABC\\\"DEF\"");
        }

        [TestMethod]
        public void ArgumentTest_08()
        {
            var arguments = AIName.ParseArguments("\"0123456789:;<=>? \"");

            Assert.AreEqual(arguments[0], "\"0123456789:;<=>? \"");
        }

        [TestMethod]
        public void ArgumentTest_09()
        {
            var arguments = AIName.ParseArguments("ABC(A)(B),DEF(G,H)");

            Assert.AreEqual(arguments[0], "ABC(A)(B)");
            Assert.AreEqual(arguments[1], "DEF(G,H)");
        }

        [TestMethod]
        public void ArgumentTest_10()
        {
            {
                var arguments = AIName.ParseArguments("ABC(A)(B),DEF(G,H),");

                Assert.AreEqual(arguments[0], "ABC(A)(B)");
                Assert.AreEqual(arguments[1], "DEF(G,H)");
                Assert.AreEqual(arguments[2], "");
            }

            {
                var arguments = AIName.ParseArguments("A,B,C,");

                Assert.AreEqual(arguments[0], "A");
                Assert.AreEqual(arguments[1], "B");
                Assert.AreEqual(arguments[2], "C");
                Assert.AreEqual(arguments[3], "");
            }
        }

        [TestMethod]
        public void CharMapNameValidateTest()
        {
            var asmLoad = new AsmLoad(new AsmOption(), new InstructionSet.Z80());
            Assert.IsTrue(AIName.ValidateCharMapName("@ABC", asmLoad));
            Assert.IsFalse(AIName.ValidateCharMapName("", asmLoad));
            Assert.IsFalse(AIName.ValidateCharMapName("@@ABC", asmLoad));
            Assert.IsFalse(AIName.ValidateCharMapName("@漢字", asmLoad));
        }

        [TestMethod]
        public void ReservedWordTest()
        {
            var reservedWords = new[] { "LD" };
            var asmLoad = new AsmLoad(new AsmOption(), new InstructionSet.Z80());

            foreach (var reservedWord in reservedWords)
            {
                Assert.IsFalse(AIName.ValidateCharMapName(reservedWord, asmLoad));
                // Assert.IsFalse(AIName.ValidateFunctionName(reservedWord, asmLoad)); 対応を決めてから有効にする
                Assert.IsFalse(AIName.ValidateFunctionArgument(reservedWord, asmLoad));
                Assert.IsFalse(AIName.ValidateMacroName(reservedWord, asmLoad));
                Assert.IsFalse(AIName.ValidateMacroArgument(reservedWord, asmLoad));
                Assert.IsFalse(AIName.DeclareLabelValidate(reservedWord, asmLoad));
            }

        }
    }
}
