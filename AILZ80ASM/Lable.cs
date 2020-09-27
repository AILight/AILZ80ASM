using System;
using System.Collections.Generic;
using System.Text;

namespace AILZ80ASM
{
    public class Lable
    {
        public enum DataLengthEnum
        {
            DB,
            DW,
        }

        public string LabelName { get; set; }
        public UInt16 Value { get; set; }
        public DataLengthEnum DataLength { get; set; }
    }
}
