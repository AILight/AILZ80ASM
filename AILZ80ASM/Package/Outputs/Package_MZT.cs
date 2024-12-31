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
        public void SaveMZT(Stream stream, string outputFilename)
        {
            using var memoryStream = new MemoryStream();
            SaveBin(memoryStream);

            var startAddress = default(UInt16);
            if (AssembleLoad.Share.AsmORGs.Count >= 2) {
                startAddress = AssembleLoad.Share.AsmORGs.Skip(1).First().ProgramAddress;
            }

            var entryAddress = AssembleLoad.Share.EntryPoint ?? startAddress;


            var binaryWriter = new IO.MZTBinaryWriter(outputFilename, startAddress, entryAddress, memoryStream.ToArray(), stream);
            binaryWriter.Write();
        }
    }
}
