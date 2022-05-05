using AILZ80ASM.AILight;
using AILZ80ASM.Assembler;
using AILZ80ASM.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

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

        [TestMethod]
        public void BuildLabelTest()
        {
            var asmLoad = new AsmLoad(new AsmOption(), new InstructionSet.Z80());
            var globalLabel = new LabelAdr("[NS_Main]", asmLoad);
            asmLoad.AddLabel(globalLabel);

            var labelAdr = new LabelAdr("TEST", "123", asmLoad);
            labelAdr.SetLineDetailExpansionItem(new LineDetailItems.ScopeItem.ExpansionItems.LineDetailExpansionItem(new LineItem("", 1, default(FileInfo))));
            asmLoad.AddLabel(labelAdr);

            asmLoad.BuildLabel();

            Assert.AreEqual(asmLoad.AssembleInformation.Length, 1);
            Assert.AreEqual(asmLoad.AssembleInformation[0].ErrorCode, Error.ErrorCodeEnum.I0001);
        }

        [TestMethod]
        public void GlobalLabelTest()
        {
            var asmLoad = new AsmLoad(new AsmOption(), new InstructionSet.Z80());
            Assert.ThrowsException<ErrorAssembleException>(() =>
            {
                try
                {
                    var globalLabel = new LabelAdr("[NS_Main]", asmLoad);
                    asmLoad.AddLabel(globalLabel);

                    var labelAdr = new LabelAdr("NS_Main", "123", asmLoad);
                    asmLoad.AddLabel(labelAdr);
                }
                catch (ErrorAssembleException ex)
                {
                    Assert.AreEqual(ex.ErrorCode, Error.ErrorCodeEnum.E0018);
                    throw;
                }
            });

            Assert.ThrowsException<ErrorAssembleException>(() =>
            {
                try
                {
                    var labelAdr = new LabelAdr("NS_Main_A", "123", asmLoad);
                    asmLoad.AddLabel(labelAdr);
                    
                    var globalLabel = new LabelAdr("[NS_Main_A]", asmLoad);
                    asmLoad.AddLabel(globalLabel);
                }
                catch (ErrorAssembleException ex)
                {
                    Assert.AreEqual(ex.ErrorCode, Error.ErrorCodeEnum.E0017);
                    throw;
                }
            });

        }
    }
}
