using AILZ80ASM.Assembler;
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
        public void SaveEQU(FileInfo equal)
        {
            using var fileStream = equal.OpenWrite();

            SaveEQU(fileStream);

            fileStream.Close();
        }

        public void SaveEQU(Stream stream)
        {
            using var memoryStream = new MemoryStream();
            using var streamWriter = new StreamWriter(memoryStream, AsmLoad.GetEncoding(AssembleLoad.AssembleOption.DecidedOutputEncodeMode));

            var title = $";{ProductInfo.ProductLongName}, EQU";
            streamWriter.WriteLine(title);

            AssembleLoad.OutputEqualLabels(streamWriter);

            streamWriter.Flush();
            memoryStream.Position = 0;
            memoryStream.CopyTo(stream);
        }
    }
}
