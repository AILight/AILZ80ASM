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

            Assert.AreEqual(new LabelAdr("ABC:", "", asmLoad).LabelFullName, "NAME_SPACE_DEFAULT.ABC");
            Assert.AreEqual(new LabelAdr("ABC::", "", asmLoad).LabelFullName, "NAME_SPACE_DEFAULT.ABC");
            Assert.AreEqual(new LabelAdr("ABC.DEF", "", asmLoad).LabelFullName, "NAME_SPACE_DEFAULT.ABC.DEF");
            Assert.AreEqual(new LabelAdr("ABC.DEF.GHI", "", asmLoad).LabelFullName, "ABC.DEF.GHI");

        }

        [TestMethod]
        public void LocalLabelNameTest()
        {
            var asmLoad = new AsmLoad(new AsmOption(), new InstructionSet.Z80());
            var globalLabel = new LabelAdr("[NAME_SPACE_DEFAULT]", asmLoad);
            asmLoad.AddLabel(globalLabel);

            Assert.AreEqual(new LabelAdr("TEST.1CH", "", asmLoad).LabelFullName, "NAME_SPACE_DEFAULT.TEST.1CH");
            Assert.AreEqual(new LabelAdr("TEST.2CH", "", asmLoad).LabelFullName, "NAME_SPACE_DEFAULT.TEST.2CH");
            Assert.AreEqual(new LabelAdr("TEST.3CH", "", asmLoad).LabelFullName, "NAME_SPACE_DEFAULT.TEST.3CH");
            Assert.AreEqual(new LabelAdr("TEST.0", "", asmLoad).LabelFullName, "NAME_SPACE_DEFAULT.TEST.0");
            Assert.AreEqual(new LabelAdr("TEST.A", "", asmLoad).LabelFullName, "NAME_SPACE_DEFAULT.TEST.A");
            Assert.AreEqual(new LabelAdr("TEST.LD", "", asmLoad).LabelFullName, "NAME_SPACE_DEFAULT.TEST.LD");

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
                    var globalLabel = new LabelAdr("[NAME_SPACE_DEFAULT]", asmLoad);
                    asmLoad.AddLabel(globalLabel);

                    var labelAdr = new LabelAdr("NAME_SPACE_DEFAULT", "123", asmLoad);
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
                    var labelAdr = new LabelAdr("NAME_SPACE_DEFAULT_A", "123", asmLoad);
                    asmLoad.AddLabel(labelAdr);
                    
                    var globalLabel = new LabelAdr("[NAME_SPACE_DEFAULT_A]", asmLoad);
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
