using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AILZ80ASM.IO
{
    public class MZTBinaryWriter
    {
        private UInt32 Elapse { get; set; } = 0;
        private string Filename { get; set; }
        private UInt16 StartAddress { get; set; }
        private UInt16 EntryAddress { get; set; }
        private Stream Stream { get; set; }
        private byte[] Buffer { get; set; }

        public MZTBinaryWriter(string filename, UInt16 startAddress, UInt16 entryAddress, byte[] buffer, Stream stream)
        {
            Filename = filename;
            StartAddress = startAddress;
            EntryAddress = entryAddress;

            Buffer = buffer;
            Stream = stream;
        }

        public void Write()
        {
            WriteHeader();
            WriteStream(Buffer);
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


        private void WriteHeader()
        {
            // (0x01)File Mode
            WriteStream((byte)0x01);
           
            // (0x11)Filename + CR
            var filenameString = (Filename.ToUpper() + new string('\x0d', 17)).Substring(0, 17);
            WriteStream(Encoding.ASCII.GetBytes(filenameString));

            // (0x06)size, offset, start
            WriteStream((ushort)(Buffer.Length));
            WriteStream((ushort)(StartAddress));
            WriteStream((ushort)(EntryAddress));

            // (0x68)rest 
            WriteStream(new byte[0x68]);
        }
    }
}
