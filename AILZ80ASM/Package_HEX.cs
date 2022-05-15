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
        public void SaveHEX(FileInfo symbol)
        {
            using var fileStream = symbol.OpenWrite();

            SaveHEX(fileStream);

            fileStream.Close();
        }

        public void SaveHEX(Stream stream)
        {
            using var memoryStream = new MemoryStream();
            using var streamWriter = new StreamWriter(memoryStream, AsmLoad.GetEncoding(AssembleLoad.AssembleOption.DecidedOutputEncodeMode));

            var outputAddress = default(UInt32);
            var binResults = FileItems.SelectMany(m => m.BinResults).OrderBy(m => m.Address.Output);
            var programKey = binResults.FirstOrDefault()?.Address.Program ?? default(UInt16);
            var outputKey = programKey + binResults.FirstOrDefault()?.Address.Output ?? default(UInt32);
            var dictionaryBytes = new Dictionary<UInt32, List<byte>>();
            dictionaryBytes.Add(outputKey, new List<byte>());

            foreach (var item in binResults)
            {
                if ((item.Address.Output ?? 0) < outputAddress)
                {
                    throw new Exception("出力先アドレスが重複したため、HEXファイルの出力に失敗ました");
                }
                else if ((item.Address.Output ?? 0) != outputAddress)
                {
                    // 新しいアドレス
                    outputKey = programKey + (item.Address.Output ?? 0);
                    dictionaryBytes.Add(outputKey, new List<byte>());
                }

                // 出力
                if (item.Data.Length > 0)
                {
                    dictionaryBytes[outputKey].AddRange(item.Data);
                    outputAddress += (UInt32)item.Data.Length;
                }
            }
            foreach (var key in dictionaryBytes.Keys)
            {
                var hexOutputAddress = key;
                foreach (var item in dictionaryBytes[key].Chunk(16))
                {
                    var dataType = 0;
                    var checkSum = (byte)((~(item.Length + (hexOutputAddress >> 8) + (hexOutputAddress & 0xFF) + dataType + item.Sum(m => m))) + 1);
                    var dataString = $":{item.Length:X2}{hexOutputAddress:X4}{dataType:X2}{string.Concat(item.Select(m => $"{m:X2}"))}{checkSum:X2}";
                    hexOutputAddress += (UInt32)item.Length;

                    streamWriter.WriteLine(dataString);
                }
            }
            // 終端
            streamWriter.WriteLine(":00000001FF");

            streamWriter.Flush();
            memoryStream.Position = 0;
            memoryStream.CopyTo(stream);
        }
    }
}
