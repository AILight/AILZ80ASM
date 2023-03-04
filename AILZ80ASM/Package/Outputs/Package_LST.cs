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
        public void SaveLST(FileInfo list)
        {
            using var fileStream = list.OpenWrite();

            SaveLST(fileStream);

            fileStream.Close();
        }

        public void SaveLST(Stream stream)
        {
            using var memoryStream = new MemoryStream();
            using var streamWriter = new StreamWriter(memoryStream, AsmLoad.GetEncoding(AssembleLoad.AssembleOption.DecidedOutputEncodeMode));

            AssembleLoad.ListedFileClear();
            var lineIndex = 2;
            var title = $";{ProductInfo.ProductLongName}, LST:{AssembleLoad.AssembleOption.ListMode}:{AssembleLoad.AssembleOption.TabSize}";
            streamWriter.WriteLine(AsmList.CreateSource(title).ToString(AssembleLoad.AssembleOption.ListMode, AssembleLoad.AssembleOption.TabSize));

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
