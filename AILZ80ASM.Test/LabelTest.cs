using AILZ80ASM.Assembler;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AILZ80ASM.Test
{
    [TestClass]
    public class LabelTest
    {
        [TestMethod]
        public void LabelNameTest()
        {
            var asmLoad = new AsmLoad(new AsmOption(), new InstructionSet.Z80());
            var globalLabel = new LabelAdr("[NS_Main]", asmLoad);
            asmLoad.AddLabel(globalLabel);

            Assert.AreEqual(new LabelAdr("ABC:", "", asmLoad).LabelFullName, "NS_Main.ABC");
            Assert.AreEqual(new LabelAdr("ABC::", "", asmLoad).LabelFullName, "NS_Main.ABC");
            Assert.AreEqual(new LabelAdr("ABC.DEF", "", asmLoad).LabelFullName, "NS_Main.ABC.DEF");
            Assert.AreEqual(new LabelAdr("ABC.DEF.GHI", "", asmLoad).LabelFullName, "ABC.DEF.GHI");

        }

    }
}
