using AILZ80ASM.Assembler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AILZ80ASM.SuperAssembles
{
    public class SP_E0010 : ISuperAssemble
    {
        public string Title => nameof(Error.ErrorCodeEnum.E0010);

        private ErrorLineItem SaveErrorLineItem { get; set; }
        private AsmAddress SaveAsmAddress { get; set; }

        public bool CheckSuperAssemble(AsmLoad asmLoad)
        {
            var result = false;

            var error0010 = asmLoad.AssembleErrors.FirstOrDefault(m => m.ErrorCode == Error.ErrorCodeEnum.E0010);
            if (error0010 != default)
            {
                // ROM出力モードには未対応
                if (asmLoad.Share.AsmORGs.All(m => asmLoad.Share.AsmSuperAssembleMode.E0010_AddressList.Any(n => n.Program == m.ProgramAddress) ||
                                                             !m.SavedOutputAddress.HasValue && string.IsNullOrEmpty(m.OutputAddressLabel)))
                {
                    // エラーが含まれるORGを取得
                    var asmORGs = asmLoad.Share.AsmORGs.Select((v, i) => new { Item = v, Index = i }).ToArray();
                    var asmORG = asmORGs.FirstOrDefault(m => m.Item.LineDetailItems.Any(n => n.LineItem == error0010.LineItem) ||
                                                             m.Item.ErrorLineDetailItems.Any(n => n.LineItem == error0010.LineItem));

                    if (asmORG != default)
                    {
                        // 自分以降に自分より前に出力対象が無いことが条件
                        if (asmORGs.Where(m => m.Index > asmORG.Index).All(m => m.Item.ProgramAddress > asmORG.Item.ProgramAddress))
                        {
                            // ORGの計算を行う
                            var asmAddress = new AsmAddress();

                            var beforeAsmORG = asmORG.Index == 0 ? default : asmORGs[asmORG.Index - 1];
                            var beforeHasBinResult = beforeAsmORG == default ? false : beforeAsmORG.Item.HasBinResult;
                            var beforeOutputAddress = beforeAsmORG == default ? 0 : beforeAsmORG.Item.OutputAddress ?? 0;

                            asmAddress.Program = asmORG.Item.ProgramAddress;
                            if (beforeHasBinResult)
                            {
                                var beforeProgramAddress = beforeAsmORG == default ? (ushort)0 : beforeAsmORG.Item.ProgramAddress;
                                var nextOutputAddress = (ushort)(asmORG.Item.ProgramAddress - beforeProgramAddress);

                                asmAddress.Output = nextOutputAddress;
                            }
                            else
                            {
                                asmAddress.Output = beforeOutputAddress;
                            }

                            // スーバーアセンブルモードに移行
                            asmLoad.Share.AsmSuperAssembleMode.E0010_AddressList.Add(asmAddress);
                            asmLoad.Share.AsmSuperAssembleMode.AddError(error0010);
                            SaveErrorLineItem = error0010;
                            SaveAsmAddress = asmAddress;
                            result = true;
                        }
                    }
                }
            }
            return result;
        }

        public bool TarminateSuperAssemble(AsmLoad asmLoad)
        {
            // 失敗した場合は、AsmORG_AddressListを元に戻す
            if (asmLoad.HasSameError(SaveErrorLineItem))
            {
                asmLoad.Share.AsmSuperAssembleMode.E0010_AddressList.Remove(SaveAsmAddress);
            }

            return false;
        }
    }
}
