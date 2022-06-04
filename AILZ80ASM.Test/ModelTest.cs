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
    public class ModelTest
    {
        [TestMethod]
        public void ProfileTest()
        {
            var defaultProfile = JsonSerializer.Deserialize<AILZ80ASM.Models.Profile>(
                @"
{
  ""default-options"": [
    ""-e"",
    ""-t""
  ],
  ""disable-warnings"": [
    ""W0001"",
    ""W9001"",
    ""W9002""
  ]
}
                ");
            var profileArguments = new List<string>();
            profileArguments.AddRange(defaultProfile.DefaultOptions);
            if (defaultProfile.DisableWarnings != default && defaultProfile.DisableWarnings.Count() > 0)
            {
                profileArguments.Add("-dw");
                profileArguments.AddRange(defaultProfile.DisableWarnings);
            }

            Assert.AreEqual("-e",   profileArguments[0]);
            Assert.AreEqual("-t",   profileArguments[1]);
            Assert.AreEqual("-dw",  profileArguments[2]);
            Assert.AreEqual("W0001", profileArguments[3]);
            Assert.AreEqual("W9001", profileArguments[4]);
            Assert.AreEqual("W9002", profileArguments[5]);
        }
    }
}
