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
        public void SaveLST(FileInfo list, bool omitHeader)
        {
            using var fileStream = list.OpenWrite();

            SaveLST(fileStream, omitHeader);

            fileStream.Close();
        }

        public void SaveLST(Stream stream, bool omitHeader)
        {
            using var memoryStream = new MemoryStream();
            using var streamWriter = new StreamWriter(memoryStream, AsmLoad.GetEncoding(AssembleLoad.AssembleOption.DecidedOutputEncodeMode));

            AssembleLoad.ListedFileClear();
            var lineIndex = 2;
            if (!omitHeader)
            {
                var title = $";{ProductInfo.ProductLongName}, LST:{AssembleLoad.AssembleOption.ListMode}:{AssembleLoad.AssembleOption.TabSize}";
                streamWriter.WriteLine(AsmList.CreateSource(title).ToString(AssembleLoad.AssembleOption.ListMode, AssembleLoad.AssembleOption.TabSize, AssembleLoad.AssembleOption.ListOmitBinaryFile));
            }

            AssembleLoad.AssociateError();

            foreach (var item in FileItems)
            {
                item.SaveList(streamWriter, ref lineIndex);
            }
            streamWriter.Flush();
            memoryStream.Position = 0;
            memoryStream.CopyTo(stream);
        }
    }
}
