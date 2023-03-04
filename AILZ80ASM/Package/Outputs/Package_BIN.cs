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
        public void SaveBin(Stream stream)
        {
            var outputAddress = default(UInt32);
            var binResults = FileItems.SelectMany(m => m.BinResults).OrderBy(m => m.Address.Output);

            // 余白を調整して出力をする
            foreach (var item in binResults)
            {
                if ((item.Address.Output ?? 0) < outputAddress)
                {
                    throw new Exception("出力先アドレスが重複したため、BINファイルの出力に失敗ました");
                }
                else if ((item.Address.Output ?? 0) != outputAddress)
                {
                    // 余白調整
                    this.AssembleLoad.OutputORGSpace(ref outputAddress, item.Address.Output ?? 0, stream);
                }

                // 通常出力
                if (item.Data.Length > 0)
                {
                    stream.Write(item.Data, 0, item.Data.Length);
                    outputAddress += (UInt32)item.Data.Length;
                }
            }
        }
    }
}
