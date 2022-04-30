using AILZ80ASM.AILight;
using AILZ80ASM.Assembler;
using AILZ80ASM.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace AILZ80ASM.Test
{
    [TestClass]
    public class AIValueTest
    {
        [TestMethod]
        public void IsNumber()
        {
            Assert.IsTrue(AIValue.IsNumber("01"));
            Assert.IsTrue(AIValue.IsNumber("123"));
            Assert.IsTrue(AIValue.IsNumber("FFFH"));
            Assert.IsTrue(AIValue.IsNumber("0FFFH"));
            Assert.IsTrue(AIValue.IsNumber("%0000_1111"));
            Assert.IsTrue(AIValue.IsNumber("0000_1111b"));
            Assert.IsTrue(AIValue.IsNumber("$FFFF"));
            Assert.IsTrue(AIValue.IsNumber("0xFFFF"));
            Assert.IsTrue(AIValue.IsNumber("777o"));

            Assert.IsFalse(AIValue.IsNumber("O123"));
            Assert.IsFalse(AIValue.IsNumber("FFF"));
            Assert.IsFalse(AIValue.IsNumber("0000_1111%"));
            Assert.IsFalse(AIValue.IsNumber("0000_1111"));
            Assert.IsFalse(AIValue.IsNumber("0FFF"));
            Assert.IsFalse(AIValue.IsNumber("0xXFFF"));
            Assert.IsFalse(AIValue.IsNumber("888o"));
        }
    }
}
