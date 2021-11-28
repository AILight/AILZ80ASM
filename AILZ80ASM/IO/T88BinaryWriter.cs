using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AILZ80ASM.IO
{
    public class T88BinaryWriter
    {
        private UInt32 Elapse { get; set; } = 0;
        private string Filename { get; set; }
        private UInt16 StartAddress { get; set; }
        private Stream Stream { get; set; }
        private byte[] Buffer { get; set; }

        public T88BinaryWriter(string filename, UInt16 startAddress, byte[] buffer, Stream stream)
        {
            Filename = filename;
            StartAddress = startAddress;
            Buffer = buffer;
            Stream = stream;
        }

        public void Write()
        {
            WriteHeader();
            WriteVersion();
            WriteSpace(7928);
            WriteMark(1322);
            WriteFilename();
            WriteMark(330);
            WriteData();
            WriteMark(3966);
            WriteSpace(9252);
            WriteTag(0x0000, 0);   // 終了タグ
        }

        private void WriteStream(byte target)
        {
            Stream.Write(new byte[] { (byte)target });
        }

        private void WriteStream(byte[] buffer)
        {
            WriteStream(buffer, 0, buffer.Length);
        }

        private void WriteStream(byte[] buffer, int offset, int count)
        {
            Stream.Write(buffer, offset, count);
        }

        private void WriteStream(UInt16 target)
        {
            Stream.Write(new byte[] { (byte)target, (byte)(target >> 8) });
        }

        private void WriteStream(UInt32 target)
        {
            Stream.Write(new byte[] { (byte)target, (byte)((target >> 8) & 0xFF), (byte)((target >> 16) & 0xFF), (byte)((target >> 24) & 0xFF) });
        }

        /// <summary>
        /// タグの出力
        /// </summary>
        /// <param name="id"></param>
        /// <param name="length"></param>
        private void WriteTag(UInt16 id, UInt16 length)
        {
            WriteStream(id);
            WriteStream(length);
        }

        /// <summary>
        /// 時間の出力
        /// </summary>
        /// <param name="tick"></param>
        private void WriteTick(UInt32 tick)
        {
            WriteStream(Elapse);
            WriteStream(tick);
            Elapse += tick;
        }

        /// <summary>
        /// マーク
        /// </summary>
        /// <param name="tick"></param>
        private void WriteMark(UInt16 tick)
        {
            WriteTag(0x0103, 8);
            WriteTick(tick);
        }

        /// <summary>
        /// データタグ出力
        /// </summary>
        /// <param name="length"></param>
        private void WriteDataTag(UInt16 length)
        {
            WriteTag(0x0101, (UInt16)(12 + length));
            WriteTick((UInt16)(44 * length));
            WriteStream(length);
            WriteStream((UInt16)0x01cc);
        }

        private void WriteDataCheck(byte[] buffer, int length)
        {
            var sum = buffer.Sum(m => m);
            var check = 0x100 - (sum & 0xff);   // チェックバイト
            WriteStream((byte)0x3a);
            WriteStream(buffer, 0, length);
            WriteStream((byte)check);
        }


        /// <summary>
        /// データ出力
        /// </summary>
        /// <param name="startAddress"></param>
        /// <param name="buffer"></param>
        private void WriteData()
        {
            var buf = new byte[256];
            var blk = (int)Math.Ceiling(Buffer.Length / 128.0);
            WriteDataTag((UInt16)(Buffer.Length + 7 + 3 * blk));

            // 開始アドレス
            buf[0] = (byte)(StartAddress >> 8);
            buf[1] = (byte)(StartAddress & 0xff);
            WriteDataCheck(buf, 2);

            // データ
            var readPointer = 0;
            while (readPointer < Buffer.Length)
            {
                var readBuffer = Buffer.Skip(readPointer).Take(128);
                var tmpBuffer = (new byte[1]).Concat(readBuffer).ToArray();
                tmpBuffer[0] = (byte)readBuffer.Count();

                WriteDataCheck(tmpBuffer, tmpBuffer.Length);

                readPointer += readBuffer.Count();
            }

            // 終了
            buf[0] = 0;
            WriteDataCheck(buf, 1);
        }

        private void WriteHeader()
        {
            WriteStream(Encoding.ASCII.GetBytes("PC-8801 Tape Image(T88)\0"));
        }

        private void WriteVersion()
        {
            WriteTag(0x0001, 2);
            WriteStream((ushort)0x0100);
        }

        private void WriteSpace(UInt16 tick)
        {
            WriteTag(0x0102, 8);
            WriteTick(tick);
        }

        private void WriteFilename()
        {
            var filenameString = $"$$${Filename}".PadRight(9).Substring(0, 9);
            WriteDataTag(9);
            WriteStream(Encoding.ASCII.GetBytes(filenameString));
        }
    }
}
