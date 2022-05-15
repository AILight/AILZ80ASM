using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AILZ80ASM.Assembler
{
    public class AsmReservedWord
    {
        public string Name { get; set; }
        public bool IsArgumentPath { get; set; }

        public static AsmReservedWord[] GetReservedWords()
        {
            var result = new[]
            {
                new AsmReservedWord { Name = "ORG", IsArgumentPath = false},
                new AsmReservedWord { Name = "ALIGN", IsArgumentPath = false},
                new AsmReservedWord { Name = "DS", IsArgumentPath = false},
                new AsmReservedWord { Name = "DEFS", IsArgumentPath = false},
                new AsmReservedWord { Name = "EQU", IsArgumentPath = false},
                new AsmReservedWord { Name = "CHARMAP", IsArgumentPath = true},
                new AsmReservedWord { Name = "INCLUDE", IsArgumentPath = true},
                new AsmReservedWord { Name = "DB", IsArgumentPath = false},
                new AsmReservedWord { Name = "DEFB", IsArgumentPath = false},
                new AsmReservedWord { Name = "DW", IsArgumentPath = false},
                new AsmReservedWord { Name = "DEFW", IsArgumentPath = false},
                new AsmReservedWord { Name = "DBFIL", IsArgumentPath = false},
                new AsmReservedWord { Name = "DWFIL", IsArgumentPath = false},
                new AsmReservedWord { Name = "MACRO", IsArgumentPath = false},
                new AsmReservedWord { Name = "ENDM", IsArgumentPath = false},
                new AsmReservedWord { Name = "REPT", IsArgumentPath = false},
                new AsmReservedWord { Name = "FUNCTION", IsArgumentPath = false},
                new AsmReservedWord { Name = "END", IsArgumentPath = false},
            };

            return result;
        }

        public static AsmReservedWord[] GetReservedWordsForLabel()
        {
            return GetReservedWords().Where(m => m.Name != "MACRO").ToArray();
        }
    }
}