using AILZ80ASM.Assembler;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AILZ80ASM
{
    public partial class Package
    {
        // CMT フォーマット
        // ヘッダー(1): 3A [HI] [LO] [CK]
        // データー(N): 3A [LN] [データ] [CK]
        // フッター(1): 3A 00 00
        // HI: データ読み込みアドレス上位
        // LO: データ読み込みアドレス下位
        // CK: チェックサム
        // データ: データ
        // LN: データレングス
        public void SaveCMT(Stream stream)
        {
            using var memoryStream = new MemoryStream();
            SaveBin(memoryStream);

            var address = default(UInt16?);
            if (this.AssembleLoad.Share.LoadAddress.HasValue && 
                this.AssembleLoad.Share.LoadAddress.SetBy == AsmDefinedAddress.SetByEnum.Defined)
            {
                address = this.AssembleLoad.Share.LoadAddress.Value;
            }
            
            if (!address.HasValue)
            {
                address = this.AssembleLoad.Share.EntryPoint.Value;
            }
            address = address ?? default(UInt16);

            var binaryWriter = new IO.CMTBinaryWriter(address.Value, memoryStream.ToArray(), stream);
            binaryWriter.Write();
        }
    }
}
