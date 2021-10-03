using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AILZ80ASM.Test
{
    public class AIMathTest
    {
        [TestMethod]
        public void IsNumber()
        {
            Assert.IsTrue(AIMath.IsNumber("ABC"));
        }
    }
}
