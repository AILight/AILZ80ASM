using AILZ80ASM.Assembler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AILZ80ASM.SuperAssembles
{
    public class SP_Reset : ISuperAssemble
    {
        public string Title => "Reset";

        public bool IsLoop => true;

        public SP_Reset()
        {

        }

        public bool CheckSuperAssemble(AsmLoad asmLoad)
        {
            return false;
        }

        public bool TarminateSuperAssemble(AsmLoad asmLoad)
        {
            return false;
        }
    }
}
