using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AILZ80ASM
{
    public interface ILineDetailItemMacroDefine
    {
        public string MacroName { get; set; }
        public string[] MacroArgs { get; set; }
        public List<LineItem> MacroLines { get; set; }
    }
}
