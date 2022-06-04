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
            var globalLabel = new LabelAdr("[NAME_SPACE_DEFAULT]", asmLoad);
            asmLoad.AddLabel(globalLabel);

            Assert.AreEqual("NAME_SPACE_DEFAULT.ABC", new LabelAdr("ABC:", "", asmLoad).LabelFullName);
            Assert.AreEqual("NAME_SPACE_DEFAULT.ABC", new LabelAdr("ABC::", "", asmLoad).LabelFullName);
            Assert.AreEqual("NAME_SPACE_DEFAULT.ABC.DEF", new LabelAdr("ABC.DEF", "", asmLoad).LabelFullName);
            Assert.AreEqual("ABC.DEF.GHI", new LabelAdr("ABC.DEF.GHI", "", asmLoad).LabelFullName);

        }

        [TestMethod]
        public void LocalLabelNameTest()
        {
            var asmLoad = new AsmLoad(new AsmOption(), new InstructionSet.Z80());
            var globalLabel = new LabelAdr("[NAME_SPACE_DEFAULT]", asmLoad);
            asmLoad.AddLabel(globalLabel);

            Assert.AreEqual("NAME_SPACE_DEFAULT.TEST.1CH", new LabelAdr("TEST.1CH", "", asmLoad).LabelFullName);
            Assert.AreEqual("NAME_SPACE_DEFAULT.TEST.2CH", new LabelAdr("TEST.2CH", "", asmLoad).LabelFullName);
            Assert.AreEqual("NAME_SPACE_DEFAULT.TEST.3CH", new LabelAdr("TEST.3CH", "", asmLoad).LabelFullName);
            Assert.AreEqual("NAME_SPACE_DEFAULT.TEST.0", new LabelAdr("TEST.0", "", asmLoad).LabelFullName);
            Assert.AreEqual("NAME_SPACE_DEFAULT.TEST.A", new LabelAdr("TEST.A", "", asmLoad).LabelFullName);
            Assert.AreEqual("NAME_SPACE_DEFAULT.TEST.LD", new LabelAdr("TEST.LD", "", asmLoad).LabelFullName);

        }

        [TestMethod]
        public void BuildLabelTest()
        {
            var asmLoad = new AsmLoad(new AsmOption(), new InstructionSet.Z80());
            var globalLabel = new LabelAdr("[NAME_SPACE_DEFAULT]", asmLoad);
            asmLoad.AddLabel(globalLabel);

            var labelAdr = new LabelAdr("TEST", "123", asmLoad);
            labelAdr.SetLineDetailExpansionItem(new LineDetailItems.ScopeItem.ExpansionItems.LineDetailExpansionItem(new LineItem("", 1, default(FileInfo))));
            asmLoad.AddLabel(labelAdr);

            asmLoad.BuildLabel();

            Assert.AreEqual(1, asmLoad.AssembleInformation.Length);
            Assert.AreEqual(Error.ErrorCodeEnum.I0002, asmLoad.AssembleInformation[0].ErrorCode);
        }

        [TestMethod]
        public void GlobalLabelTest()
        {
            var asmLoad = new AsmLoad(new AsmOption(), new InstructionSet.Z80());
            Assert.ThrowsException<ErrorAssembleException>(() =>
            {
                try
                {
                    var globalLabel = new LabelAdr("[NAME_SPACE_DEFAULT]", asmLoad);
                    asmLoad.AddLabel(globalLabel);

                    var labelAdr = new LabelAdr("NAME_SPACE_DEFAULT", "123", asmLoad);
                    asmLoad.AddLabel(labelAdr);
                }
                catch (ErrorAssembleException ex)
                {
                    Assert.AreEqual(Error.ErrorCodeEnum.E0018, ex.ErrorCode);
                    throw;
                }
            });

            Assert.ThrowsException<ErrorAssembleException>(() =>
            {
                try
                {
                    var labelAdr = new LabelAdr("NAME_SPACE_DEFAULT_A", "123", asmLoad);
                    asmLoad.AddLabel(labelAdr);
                    
                    var globalLabel = new LabelAdr("[NAME_SPACE_DEFAULT_A]", asmLoad);
                    asmLoad.AddLabel(globalLabel);
                }
                catch (ErrorAssembleException ex)
                {
                    Assert.AreEqual(Error.ErrorCodeEnum.E0017, ex.ErrorCode);
                    throw;
                }
            });

        }
    }
}
