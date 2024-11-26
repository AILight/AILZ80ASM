using AILZ80ASM.Assembler;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AILZ80ASM.Test
{
    [TestClass]
    public class Z80ZZDiscussionTest
    {
        [TestMethod]
        public void Discussion_203()
        {
            {
                var result = Program.Main(@"Test.Z80", "-f", "-lst", "-bin", "-cd", "./Test/Discussions/203");
                Assert.AreEqual(1, result);
            }

            {
                var result = Program.Main(@"Test.Z80", "-f", "-lst", "Discussion203.LST", "-bin", "Discussion203.BIN", "-cd", "./Test/Discussions/203", "-ip", "./lib");
                Assert.AreEqual(0, result);

                Lib.AreSameLst(File.OpenRead("./Test/Discussions/203/Discussion203.LST"), File.OpenRead("./Test/Discussions/203/Test.LST"), Assembler.AsmEnum.FileTypeEnum.LST);
                Lib.AreSameLst(File.OpenRead("./Test/Discussions/203/Discussion203.BIN"), File.OpenRead("./Test/Discussions/203/Test.BIN"), Assembler.AsmEnum.FileTypeEnum.BIN);
            }
        }
    }
}
