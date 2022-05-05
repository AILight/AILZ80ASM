using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AILZ80ASM.IO
{
    public abstract class AIBinaryWriter
    {
        private Stream Stream { get; set; }

        public AIBinaryWriter(Stream stream)
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
    }
}
