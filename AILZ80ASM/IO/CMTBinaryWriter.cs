using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AILZ80ASM.IO
{
    public class CMTBinaryWriter : AIBinaryWriter
    {
        private UInt16 EntryAddress { get; set; }
        private byte[] Buffer { get; set; }

        public CMTBinaryWriter(UInt16 entryAddress, byte[] buffer, Stream stream)
            : base(stream)
        {
            EntryAddress = entryAddress;
            Buffer = buffer;
        }

        public void Write()
        {
            WriteHeader();
            WriteData();
            WriteEnd();
        }

        /// <summary>
        /// ブロックを書き込み
        /// </summary>
        /// <param name="buffer"></param>
        private void WriteDataBlock(byte[] buffer)
        {
            var sum = buffer.Sum(m => m);
            var check = 0x100 - (sum & 0xff);   // チェックバイト
            WriteStream((byte)0x3a);
            WriteStream(buffer);
            WriteStream((byte)check);

        }

        /// <summary>
        /// データ出力
        /// </summary>
        /// <param name="startAddress"></param>
        /// <param name="buffer"></param>
        private void WriteData()
        {
            // データ
            var readPointer = 0;
            while (readPointer < Buffer.Length)
            {
                var readBuffer = Buffer.Skip(readPointer).Take(255);
                var tmpBuffer = (new byte[1]).Concat(readBuffer).ToArray();
                tmpBuffer[0] = (byte)readBuffer.Count();
                WriteDataBlock(tmpBuffer);

                readPointer += readBuffer.Count();
            }
        }

        private void WriteHeader()
        {
            WriteDataBlock(new byte[] { (byte)(EntryAddress >> 8), (byte)(EntryAddress & 0xFF) });
        }

        private void WriteEnd()
        {
            WriteStream((byte)0x3a);
            WriteStream((byte)0x00);
            WriteStream((byte)0x00);
        }
    }
}
