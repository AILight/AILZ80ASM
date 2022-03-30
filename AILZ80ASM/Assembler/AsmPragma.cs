using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AILZ80ASM.Assembler
{
    public class AsmPragma
    {
        public List<FileInfo> OnceFiles { get; set; } = new List<FileInfo>();
    }
}
