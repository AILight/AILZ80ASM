using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AILZ80ASM
{
    public class FileItem
    {
        private List<LineItem> Items { get; set; } = new List<LineItem>();

        public FileItem(FileInfo fileinfo)
        {
            var fileStream = fileinfo.OpenText();
            string line;
            while (!string.IsNullOrEmpty(line = fileStream.ReadLine()))
            {
                var item = new LineItem(line);
                Items.Add(item);
            }
        }

        public byte[] Bin
        {
            get
            {
                var bytes = new List<byte>();

                foreach (var item in Items)
                {
                    if (item.Bin != default(byte[]))
                    {
                        bytes.AddRange(item.Bin);
                    }
                }

                return bytes.ToArray();
            }
        }

        public void SetLabel(ref UInt16 address, IList<Lable> labelList)
        {
            // ラベルを整理する
            foreach (var item in Items)
            {
                item.SetLabel(ref address, labelList);
            }
        }

        public void Assemble(ref UInt16 address, Lable[] labels)
        {
            // アセンブルを実行する
            foreach (var item in Items)
            {
                item.Assemble(ref address, labels);
            }
        }

    }
}
