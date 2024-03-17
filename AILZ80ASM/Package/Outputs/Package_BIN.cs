using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
            // 出力アドレスが指定された場合
            if (this.AssembleOption.StartAddress.HasValue)
            {
                // 開始のオフセットがあれば処理を行う
                var asmORG = this.AssembleLoad.Share.AsmORGs.Where(m => m.HasBinResult || m.IsRomMode).OrderBy(m => m.OutputAddress).FirstOrDefault();
                // ロムモードの出力場合には処理しない
                if (!asmORG.IsRomMode)
                {
                    var length = (int)(asmORG.ProgramAddress - this.AssembleOption.StartAddress.Value);
                    if (length < 0)
                    {
                        throw new Exception($"オプション: --start-address:0x{this.AssembleOption.StartAddress.Value:X4} の指定に問題があります。指定アドレス以前に出力データがあります。");
                    }
                    var bytes = Enumerable.Repeat<byte>(this.AssembleOption.GapByte, (int)length).ToArray();
                    stream.Write(bytes, 0, bytes.Length);
                }
            }

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
