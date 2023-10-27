using AILZ80ASM.AILight;
using AILZ80ASM.Assembler;
using AILZ80ASM.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using static AILZ80ASM.Assembler.Error;

namespace AILZ80ASM.LineDetailItems
{
    public abstract class LineDetailItemCheck : LineDetailItem
    {
        public LineDetailItemCheck LineDetailItemCheck_ENDC { get; set; } = default;

        protected LineDetailItemCheck(LineItem lineItem, AsmLoad asmLoad)
            : base(lineItem, asmLoad)
        {
        }

        public void SetENDC(LineDetailItemCheck lineDetailItemCheck)
        {
            LineDetailItemCheck_ENDC = lineDetailItemCheck;
        }
    }
}
