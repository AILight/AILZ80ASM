using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AILZ80ASM.IO
{
    public abstract class BinaryWriter
    {
        private Stream Stream { get; set; }

        public BinaryWriter(Stream stream)
        {
            Stream = stream;
        }

        public void WriteStream(byte target)
        {
            Stream.Write(new byte[] { (byte)target });
        }

        public void WriteStream(byte[] buffer)
        {
            WriteStream(buffer, 0, buffer.Length);
        }

        public void WriteStream(byte[] buffer, int offset, int count)
        {
            Stream.Write(buffer, offset, count);
        }

        public void WriteStream(UInt16 target)
        {
            Stream.Write(new byte[] { (byte)target, (byte)(target >> 8) });
        }

        public void WriteStream(UInt32 target)
        {
            Stream.Write(new byte[] { (byte)target, (byte)((target >> 8) & 0xFF), (byte)((target >> 16) & 0xFF), (byte)((target >> 24) & 0xFF) });
        }
    }
}
