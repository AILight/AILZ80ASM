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
        public void SaveT88(Stream stream, string outputFilename)
        {
            using var memoryStream = new MemoryStream();
            SaveBin(memoryStream);

            var address = default(UInt16);
            if (AssembleLoad.Share.AsmORGs.Count >= 2)
            {
                address = AssembleLoad.Share.AsmORGs.Skip(1).First().ProgramAddress;
            }

            var binaryWriter = new IO.T88BinaryWriter(outputFilename, address, memoryStream.ToArray(), stream);
            binaryWriter.Write();
        }
    }
}
