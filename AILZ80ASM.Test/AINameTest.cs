using AILZ80ASM.AILight;
using AILZ80ASM.Assembler;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace AILZ80ASM.Test
{
    [TestClass]
    public class AINameTest
    {
        [DataTestMethod]
        [DataRow("")]
        [DataRow("ABC,FUNC(),DEF", ["ABC", "FUNC()", "DEF"])]
        [DataRow("ABC,FUNC(GHI),DEF", ["ABC", "FUNC(GHI)", "DEF"])]
        [DataRow("ABC,FUNC(GHI,JKL),DEF", ["ABC", "FUNC(GHI,JKL)", "DEF"])]
        [DataRow("ABC,FUNC(GHI,JKL,FUNC(GHI,JKL)),DEF", ["ABC", "FUNC(GHI,JKL,FUNC(GHI,JKL))", "DEF"])]
        [DataRow("ABC + TTT, FUNC(GHI,JKL,FUNC(GHI,JKL)) , DEF", ["ABC + TTT", "FUNC(GHI,JKL,FUNC(GHI,JKL))", "DEF"])]
        [DataRow("\"ABC\", 123, \"ABC,DEF\", \"ABC\\\"DEF\"", ["\"ABC\"", "123", "\"ABC,DEF\"", "\"ABC\\\"DEF\""])]
        [DataRow("\"0123456789:;<=>? \"", ["\"0123456789:;<=>? \""])]
        [DataRow("ABC(A)(B),DEF(G,H)", ["ABC(A)(B)", "DEF(G,H)"])]
        [DataRow("ABC(A)(B),DEF(G,H),", ["ABC(A)(B)", "DEF(G,H)", ""])]
        [DataRow("A,B,C,", ["A", "B", "C", ""])]
        [DataRow("(2 - 1) + 1, 4", ["(2 - 1) + 1", "4"])]
        [DataRow("2 - 1 + 1, 4", ["2 - 1 + 1", "4"])]
        [DataRow("(1) * 3, (4)", ["(1) * 3", "(4)"])]
        [DataRow("(1) / 4, 4", ["(1) / 4", "4"])]
        [DataRow("(1) + 3, (4)", ["(1) + 3", "(4)"])]
        [DataRow("(2 - 1) + 4, (5 - 1)", ["(2 - 1) + 4", "(5 - 1)"])]
        [DataRow("(1) - 5, (4)", ["(1) - 5", "(4)"])]
        [DataRow("(1) * 3, (4)", ["(1) * 3", "(4)"])]
        [DataRow("(1) / 4, (4)", ["(1) / 4", "(4)"])]
        [DataRow("hl, abc(1, 1)", ["hl", "abc(1, 1)"])]
        [DataRow("de, abc(1, 1) + abc(2, 2)", ["de", "abc(1, 1) + abc(2, 2)"])]
        [DataRow("bc, xyofs(1, 1) - 2", ["bc", "xyofs(1, 1) - 2"])]
        public void ArgumentTest(string input, params string[] expected)
        {
            var actual = AIName.ParseArguments(input);
            CollectionAssert.AreEqual(expected, actual);
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
            var reservedWords = new[] { "LD", "A" };
            var asmLoad = new AsmLoad(new AsmOption(), new InstructionSet.Z80());

            foreach (var reservedWord in reservedWords)
            {
                Assert.IsFalse(AIName.ValidateCharMapName(reservedWord, asmLoad));
                Assert.IsTrue(AIName.ValidateFunctionName(reservedWord, asmLoad));
                Assert.IsTrue(AIName.ValidateFunctionArgument(reservedWord, asmLoad));
                Assert.IsFalse(AIName.ValidateMacroName(reservedWord, asmLoad));
                Assert.IsFalse(AIName.ValidateMacroArgument(reservedWord, asmLoad));
                Assert.IsFalse(AIName.DeclareLabelValidate(reservedWord, asmLoad));
            }

        }

        [TestMethod]
        public void DeclareLabelValidateTest()
        {
            var asmLoad = new AsmLoad(new AsmOption(), new InstructionSet.Z80());
            var label = new LabelAdr("[NAME_SPACE_DEFAULT]", asmLoad);
            asmLoad.AddLabel(label);

            Assert.IsTrue(AIName.DeclareLabelValidate("ABC:", asmLoad));
            Assert.IsTrue(AIName.DeclareLabelValidate("A_B_C:", asmLoad));
            Assert.IsTrue(AIName.DeclareLabelValidate("ABC.DEF", asmLoad));
            Assert.IsTrue(AIName.DeclareLabelValidate(".A", asmLoad));
            Assert.IsTrue(AIName.DeclareLabelValidate(".0", asmLoad));
            Assert.IsTrue(AIName.DeclareLabelValidate("ABC.0", asmLoad));
            Assert.IsTrue(AIName.DeclareLabelValidate("ABC.0:", asmLoad));
            Assert.IsTrue(AIName.DeclareLabelValidate(".ABC:", asmLoad));
            Assert.IsTrue(AIName.DeclareLabelValidate(".ABC::", asmLoad));
            Assert.IsTrue(AIName.DeclareLabelValidate("NAME_SPACE_DEFAULT.ABC.DEF", asmLoad));

            Assert.IsFalse(AIName.DeclareLabelValidate("$0000:", asmLoad));
            Assert.IsFalse(AIName.DeclareLabelValidate("0000:", asmLoad));
            Assert.IsFalse(AIName.DeclareLabelValidate("LD:", asmLoad));
            Assert.IsFalse(AIName.DeclareLabelValidate("ABC!:", asmLoad));
            Assert.IsFalse(AIName.DeclareLabelValidate("ABC.0!0", asmLoad));
            Assert.IsFalse(AIName.DeclareLabelValidate("A!BC.0", asmLoad));
            Assert.IsFalse(AIName.DeclareLabelValidate("NAME_SPACE_DEFAULT.A!B", asmLoad));
            Assert.IsFalse(AIName.DeclareLabelValidate("NAME_SPACE_DEFAULT.A!B.0", asmLoad));
            Assert.IsFalse(AIName.DeclareLabelValidate("NAME_SPACE_DEFAULT.AB.0!0", asmLoad));
            Assert.IsFalse(AIName.DeclareLabelValidate("NAME_SPACE_DEFAULT.A.B.C", asmLoad));
        }

        [TestMethod]
        public void ValidateMacroNameTest()
        {
            var asmLoad = new AsmLoad(new AsmOption(), new InstructionSet.Z80());
            var label = new LabelAdr("[NAME_SPACE_DEFAULT]", asmLoad);
            asmLoad.AddLabel(label);

            Assert.IsTrue(AIName.ValidateMacroName("ABC", asmLoad));
            Assert.IsTrue(AIName.ValidateMacroName("ABC()", asmLoad));
            Assert.IsTrue(AIName.ValidateMacroName("ABC_DEF", asmLoad));
            Assert.IsTrue(AIName.ValidateMacroName("ABC_DEF()", asmLoad));

            Assert.IsFalse(AIName.ValidateMacroName("0ABC", asmLoad));
            Assert.IsFalse(AIName.ValidateMacroName("ABC!", asmLoad));
            Assert.IsFalse(AIName.ValidateMacroName("ABC:", asmLoad));
            Assert.IsFalse(AIName.ValidateMacroName("$0000", asmLoad));
            Assert.IsFalse(AIName.ValidateMacroName("0H", asmLoad));
            Assert.IsFalse(AIName.ValidateMacroName("LD", asmLoad));
            Assert.IsFalse(AIName.ValidateMacroName("AB.CD", asmLoad));
            Assert.IsFalse(AIName.ValidateMacroName("AB:", asmLoad));
        }

        [TestMethod]
        public void ValidateMacroArgumentTest()
        {
            var asmLoad = new AsmLoad(new AsmOption(), new InstructionSet.Z80());
            var label = new LabelAdr("[NAME_SPACE_DEFAULT]", asmLoad);
            asmLoad.AddLabel(label);

            Assert.IsTrue(AIName.ValidateMacroArgument("ABC", asmLoad));
            Assert.IsTrue(AIName.ValidateMacroArgument("A_B_C", asmLoad));
            Assert.IsTrue(AIName.ValidateMacroArgument("A_B_C0", asmLoad));

            Assert.IsFalse(AIName.ValidateMacroArgument("LD", asmLoad));
            Assert.IsFalse(AIName.ValidateMacroArgument("A", asmLoad));
            Assert.IsFalse(AIName.ValidateMacroArgument("A!B", asmLoad));
            Assert.IsFalse(AIName.ValidateMacroArgument("AB.CD", asmLoad));
            Assert.IsFalse(AIName.ValidateMacroArgument("AB:", asmLoad));
        }

        [TestMethod]
        public void ValidateFunctionNameTest()
        {
            var asmLoad = new AsmLoad(new AsmOption(), new InstructionSet.Z80());
            var label = new LabelAdr("[NAME_SPACE_DEFAULT]", asmLoad);
            asmLoad.AddLabel(label);

            Assert.IsTrue(AIName.ValidateFunctionName("ABC", asmLoad));
            Assert.IsTrue(AIName.ValidateFunctionName("A_B_C", asmLoad));
            Assert.IsTrue(AIName.ValidateFunctionName("A_B_C0", asmLoad));
            Assert.IsTrue(AIName.ValidateFunctionName("LD", asmLoad));
            Assert.IsTrue(AIName.ValidateFunctionName("A", asmLoad));

            Assert.IsFalse(AIName.ValidateFunctionName("A!B", asmLoad));
            Assert.IsFalse(AIName.ValidateFunctionName("FUNC()", asmLoad));
            Assert.IsFalse(AIName.ValidateFunctionName("AB.CD", asmLoad));
            Assert.IsFalse(AIName.ValidateFunctionName("AB:", asmLoad));
        }

        [TestMethod]
        public void ValidateFunctionArgumentTest()
        {
            var asmLoad = new AsmLoad(new AsmOption(), new InstructionSet.Z80());
            var label = new LabelAdr("[NAME_SPACE_DEFAULT]", asmLoad);
            asmLoad.AddLabel(label);

            Assert.IsTrue(AIName.ValidateFunctionArgument("ABC", asmLoad));
            Assert.IsTrue(AIName.ValidateFunctionArgument("A_B_C", asmLoad));
            Assert.IsTrue(AIName.ValidateFunctionArgument("A_B_C0", asmLoad));
            Assert.IsTrue(AIName.ValidateFunctionArgument("LD", asmLoad));
            Assert.IsTrue(AIName.ValidateFunctionArgument("A", asmLoad));

            Assert.IsFalse(AIName.ValidateFunctionArgument("A!B", asmLoad));
            Assert.IsFalse(AIName.ValidateFunctionArgument("AB.CD", asmLoad));
            Assert.IsFalse(AIName.ValidateFunctionArgument("AB:", asmLoad));
        }
    }
}
