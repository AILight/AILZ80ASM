using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace AILZ80ASM
{
    public class LineDetailItemEqual : LineDetailItem
    {
        public FileInfo FileInfo { get; private set; }
        public List<LineItem> Items { get; private set; } = new List<LineItem>();
        //GREEN_ADDR equ 
        private static readonly string RegexPatternEqual = @"\s*(?<label>\.?[a-zA-Z0-9_]+)\s+equ\s+(?<value>.+)";


        public LineDetailItemEqual()
        {

        }

        public static LineDetailItemEqual Create(string lineString)
        {
            var matched = Regex.Match(lineString, RegexPatternEqual, RegexOptions.Singleline | RegexOptions.IgnoreCase);
            if (matched.Success)
            {
                return new LineDetailItemEqual();
            }
            
            return default;
        }
    }
}
