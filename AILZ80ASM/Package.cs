using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AILZ80ASM
{
    public class Package
    {
        private List<FileItem> FileItems { get; set; } = new List<FileItem>();

        public Package(FileInfo[] Files)
        {
            foreach (var fileInfo in Files)
            {
                FileItems.Add(new FileItem(fileInfo, this));
            }
        }

        public void Assemble()
        {
            //var labelList = new List<Lable>();
            //labelList.Add(new Lable { LabelName = "$", DataLength = Lable.DataLengthEnum.DW, Value = 0 });
            
            var address = default(UInt16);

            foreach (var fileItem in FileItems)
            {
                fileItem.PreAssemble(ref address);
            }

            foreach (var fileItem in FileItems)
            {
                fileItem.Assemble();
            }
        }

        public void Save(FileInfo output)
        {
            var fileStream = output.OpenWrite();

            Save(fileStream);

            fileStream.Close();
        }

        public void Save(Stream stream)
        {

            foreach (var item in FileItems)
            {
                var bin = item.Bin;
                stream.Write(bin, 0, bin.Length);
            }
        }
    }
}
