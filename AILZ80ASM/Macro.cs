using System;
using System.Collections.Generic;
using System.Text;

namespace AILZ80ASM
{
    public class Macro
    {
        public Macro(LineItem lineItem)
        {
            MacroStatus = MacroStatusEnum.None;
        }

        public enum MacroStatusEnum
        {
            None,
            Macro
        }

        public string NameSpace { get; private set; }
        public string MacroName { get; private set; }
        public MacroStatusEnum MacroStatus { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lineString"></param>
        /// <returns>false:コードブロックの終了</returns>
        public bool AddLine(string lineString)
        {
            return false;
        }
    }
}
