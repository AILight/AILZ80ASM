using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AILZ80ASM.Assembler
{
    public class AsmLoadScope
    {
        // デフォルトキャラクターマップ
        public string DefaultCharMap { get; set; }

        public AsmLoadScope Clone()
        {
            var asmLoadScope = new AsmLoadScope();

            asmLoadScope.DefaultCharMap = DefaultCharMap;

            return asmLoadScope;
        }

        public void Restore(AsmLoadScope scope)
        {
            this.DefaultCharMap = scope.DefaultCharMap;
        }

    }
}
