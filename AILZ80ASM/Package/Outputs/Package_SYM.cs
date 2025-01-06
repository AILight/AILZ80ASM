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
        public void SaveSYM(FileInfo symbol)
        {
            using var fileStream = symbol.OpenWrite();

            SaveSYM(fileStream);

            fileStream.Close();
        }

        public void SaveSYM(Stream stream)
        {
            using var memoryStream = new MemoryStream();
            using var streamWriter = new StreamWriter(memoryStream, AsmLoad.GetEncoding(AssembleLoad.AssembleOption.DecidedOutputEncodeMode));

            var title = $";{ProductInfo.ProductLongName}, SYM:{AssembleLoad.AssembleOption.SymbolMode}";
            streamWriter.WriteLine(title);

            AssembleLoad.OutputLabels(streamWriter, AssembleLoad.AssembleOption.SymbolMode);

            streamWriter.Flush();
            memoryStream.Position = 0;
            memoryStream.CopyTo(stream);
        }
    }
}
