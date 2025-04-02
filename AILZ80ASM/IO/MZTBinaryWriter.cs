using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AILZ80ASM.IO
{
    public class MZTBinaryWriter : AIBinaryWriter
    {
        private string Filename { get; set; }
        private UInt16 LoadAddress { get; set; }
        private UInt16 EntryAddress { get; set; }
        private byte[] Buffer { get; set; }

        public MZTBinaryWriter(string filename, UInt16 loadAddress, UInt16 entryAddress, byte[] buffer, Stream stream)
            : base(stream)
        {
            Filename = filename;
            LoadAddress = loadAddress;
            EntryAddress = entryAddress;

            Buffer = buffer;
        }

        public void Write()
        {
            WriteHeader();
            WriteDetail();
            //WriteStream(Buffer);
        }

        private void WriteHeader()
        {
            // 0x0000: 01 File Mode
            WriteStream((byte)0x01);

            // 0x0001 - 0x0011: Filename
            WriteFilename();

            // 0x0012 - 0x0013 ファイルサイズ(データサイズ)
            WriteStream((UInt16)Buffer.Length);

            // 0x0014 - 0x0015 読み込むアドレス
            WriteStream((UInt16)(LoadAddress));

            // 0x0016 - 0x0017 実行するアドレス
            WriteStream((UInt16)(EntryAddress));

            // 0x0018 - 0x007F 予約
            WriteStream(new byte[0x0080 - 0x0018]);
        }

        private void WriteFilename()
        {
            // ファイルは16バイト、終端は0x0d、残りは0x20で埋める
            var filenameBytes = Encoding.ASCII.GetBytes(Filename.ToUpper()).Take(16).ToArray();
            var gapBytes = Enumerable.Repeat((byte)0x20, 16 - filenameBytes.Length).ToArray();
            WriteStream(filenameBytes);
            WriteStream(0x0d);
            WriteStream(gapBytes);
        }

        private void WriteDetail()
        {
            WriteStream(Buffer);
        }
    }
}
