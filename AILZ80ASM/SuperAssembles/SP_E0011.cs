using AILZ80ASM.Assembler;
using AILZ80ASM.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AILZ80ASM.SuperAssembles
{
    public class SP_E0011 : ISuperAssemble
    {
        public string Title => nameof(Error.ErrorCodeEnum.E0011);

        private string SaveLabelName { get; set; }


        public bool CheckSuperAssemble(AsmLoad asmLoad)
        {
            var result = false;

            var error0011 = asmLoad.AssembleErrors.FirstOrDefault(m => m.ErrorCode == Error.ErrorCodeEnum.E0011);
            if (error0011 != default)
            {
                if (error0011.InnerErrorAssembleException.InnerException is UnresolvedProgramAddressException ex &&
                    ex.TargetLabel is Assembler.Label label)
                {
                    var labelName = label.LabelFullName;
                    if (label.LineItem.LineDetailItem.Address.HasValue)
                    {
                        var programAddress = label.LineItem.LineDetailItem.Address.Value.Program;

                        if (!asmLoad.Share.AsmSuperAssembleMode.E0011_AddressDictionary.ContainsKey(labelName))
                        {
                            asmLoad.Share.AsmSuperAssembleMode.E0011_AddressDictionary.Add(labelName, (UInt16)programAddress);
                            SaveLabelName = labelName;
                            result = true;
                        }
                    }
                }
            }

            return result;
        }

        public bool TarminateSuperAssemble(AsmLoad asmLoad)
        {
            var address = asmLoad.Share.AsmSuperAssembleMode.E0011_AddressDictionary[SaveLabelName];

            // アドレス処理が成功したかを確認
            var label = asmLoad.FindLabel(SaveLabelName);
            if (label != default &&
                label.LineItem.LineDetailItem.Address.HasValue)
            {
                if (label.LineItem.LineDetailItem.Address.Value.Program == address)
                {
                    return false;
                }
            }
            
            if (label == default ||
                address == UInt16.MaxValue || 
                !asmLoad.Share.AsmSuperAssembleMode.IsLoop)
            {
                asmLoad.Share.AsmSuperAssembleMode.E0011_AddressDictionary.Remove(SaveLabelName);

                return false;
            }

            address++;
            asmLoad.Share.AsmSuperAssembleMode.E0011_AddressDictionary[SaveLabelName] = address;

            return true;
        }
    }
}
