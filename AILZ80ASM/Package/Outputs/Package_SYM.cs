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
        public void SaveSYM(FileInfo symbol, bool omitHeader)
        {
            using var fileStream = symbol.OpenWrite();

            SaveSYM(fileStream, omitHeader);

            fileStream.Close();
        }

        public void SaveSYM(Stream stream, bool omitHeader)
        {
            using var memoryStream = new MemoryStream();
            using var streamWriter = new StreamWriter(memoryStream, AsmLoad.GetEncoding(AssembleLoad.AssembleOption.DecidedOutputEncodeMode));

            if (!omitHeader)
            {
                var title = $";{ProductInfo.ProductLongName}, SYM:{AssembleLoad.AssembleOption.SymbolMode}";
                streamWriter.WriteLine(title);
            }

            AssembleLoad.OutputLabels(streamWriter, AssembleLoad.AssembleOption.SymbolMode);

            streamWriter.Flush();
            memoryStream.Position = 0;
            memoryStream.CopyTo(stream);
        }
    }
}
