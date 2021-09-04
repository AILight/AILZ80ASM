using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace AILZ80ASM
{
    public class LineDetailExpansionItem
    {


        public Label Label { get; set; }
        public bool IsAssembled { get; set; }

        public LineDetailExpansionItem()
        {

        }

        public virtual byte[] Bin
        {
            get
            {
                return new byte[] { };
            }
        }
    }
}
