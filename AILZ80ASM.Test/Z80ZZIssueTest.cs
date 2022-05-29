using AILZ80ASM.Assembler;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AILZ80ASM.Test
{
    [TestClass]
    public class Z80ZZIssueTest
    {
        [TestMethod]
        public void Issue_141()
        {
            Lib.Assemble_AreSame(Path.Combine("Issue", "141"));
        }
    }
}
