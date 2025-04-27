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
        public void SaveTAG(FileInfo tags, bool omitHeader)
        {
            using var fileStream = tags.OpenWrite();

            SaveTAG(fileStream, omitHeader);

            fileStream.Close();
        }

        public void SaveTAG(Stream stream, bool omitHeader)
        {
            using var memoryStream = new MemoryStream();
            using var streamWriter = new StreamWriter(memoryStream, AsmLoad.GetEncoding(AssembleLoad.AssembleOption.DecidedOutputEncodeMode));

            if (!omitHeader)
            {
                var title = $";{ProductInfo.ProductLongName}, TAG";
                streamWriter.WriteLine(title);
            }

            AssembleLoad.OutputTags(streamWriter);

            streamWriter.Flush();
            memoryStream.Position = 0;
            memoryStream.CopyTo(stream);
        }
    }
}
