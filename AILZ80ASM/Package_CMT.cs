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
        public void SaveCMT(Stream stream)
        {
            using var memoryStream = new MemoryStream();
            SaveBin(memoryStream);

            var address = AssembleLoad.Share.EntryPoint ?? default(UInt16);
            var binaryWriter = new IO.CMTBinaryWriter(address, memoryStream.ToArray(), stream);
            binaryWriter.Write();
        }
    }
}
