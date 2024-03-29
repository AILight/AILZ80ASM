﻿using System;
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

            var address = AssembleLoad.Share.EntryPoint ?? default(UInt16);
            var binaryWriter = new IO.CMTBinaryWriter(address, memoryStream.ToArray(), stream);
            binaryWriter.Write();
        }
    }
}
