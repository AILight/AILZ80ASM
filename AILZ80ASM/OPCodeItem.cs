using System;
using System.Collections.Generic;
using System.Text;

namespace AILZ80ASM
{
    public class OPCodeItem
    {
        public string Operation { get; set; }
        public string[] OPCode { get; set; }
        public int M { get; set; }
        public int T { get; set; }

    }
}
