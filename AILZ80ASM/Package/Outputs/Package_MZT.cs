using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AILZ80ASM
{ 
    public partial class Package
    {
        // MZT フォーマット
        // ヘッダー (128バイト)
        //  アトリビュート: 01 (マシン語ファイル)
        //  ファイル名: 16バイト + 0DH (終端文字以降は20Hで埋める)
        //  ファイルサイズ: [LO] [HI]
        //  ローディングアドレス: [LO] [HI]
        //  エントリーアドレス: [LO] [HI]
        // データ
        //  本体データ
        public void SaveMZT(Stream stream, string outputFilename)
        {
            using var memoryStream = new MemoryStream();
            SaveBin(memoryStream);

            var loadAddress = AssembleLoad.Share.LoadAddress ?? default(UInt16);
            var entryAddress = AssembleLoad.Share.EntryPoint ?? default(UInt16);

            var binaryWriter = new IO.MZTBinaryWriter(outputFilename, loadAddress, entryAddress, memoryStream.ToArray(), stream);
            binaryWriter.Write();
        }
    }
}
