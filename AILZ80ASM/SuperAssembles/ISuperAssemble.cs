using AILZ80ASM.Assembler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AILZ80ASM.SuperAssembles
{
    public interface ISuperAssemble
    {
        string Title { get; }

        bool CheckSuperAssemble(AsmLoad asmLoad);
        bool TarminateSuperAssemble(AsmLoad asmLoad);
    }
}
