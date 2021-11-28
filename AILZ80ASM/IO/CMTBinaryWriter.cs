using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AILZ80ASM.IO
{
    public class CMTBinaryWriter : BinaryWriter
    {
        private UInt32 Elapse { get; set; } = 0;
        private UInt16 StartAddress { get; set; }
        private byte[] Buffer { get; set; }

        public CMTBinaryWriter(UInt16 startAddress, byte[] buffer, Stream stream)
            : base(stream)
        {
            StartAddress = startAddress;
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
        private void WriteDataBlock(byte[] buffer, bool withLength)
        {
            var sum = buffer.Sum(m => m);
            var check = 0x100 - (sum & 0xff);   // チェックバイト
            WriteStream((byte)0x3a);
            if (withLength)
            {
                WriteStream((byte)buffer.Length);
            }
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
                var readBuffer = Buffer.Skip(readPointer).Take(255).ToArray();
                WriteDataBlock(readBuffer, true);

                readPointer += readBuffer.Count();
            }
        }

        private void WriteHeader()
        {
            WriteDataBlock(new byte[] { (byte)(StartAddress >> 8), (byte)(StartAddress & 0xFF) }, false);
        }

        private void WriteEnd()
        {
            WriteStream((byte)0x3a);
            WriteStream((byte)0x00);
            WriteStream((byte)0x00);
        }
    }
}
