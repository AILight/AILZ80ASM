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

    }
}
