using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AILZ80ASM
{
    public class FileItem
    {
        private Package Package { get; set; }

        public string LoadFileName {get ; private set;}
        public FileInfo FileInfo { get; private set; }
        public List<LineItem> Items { get; private set; } = new List<LineItem>();
        public string WorkGlobalLabelName { get; set; }
        public string WorkLabelName { get; set; }
        public List<LineItemErrorMessage> ErrorMessages { get; private set; } = new List<LineItemErrorMessage>();

        public FileItem(FileInfo fileInfo, Package package)
        {
            Package = package;
            FileInfo = fileInfo;

            using var streamReader = fileInfo.OpenText();
            Read(streamReader);
            streamReader.Close();
        }

        public FileItem(StreamReader streamReader)
        {
            Read(streamReader);
        }

        private void Read(StreamReader streamReader)
        {
            string line;
            var lineIndex = 0;
            LoadFileName = Path.GetFileNameWithoutExtension(FileInfo.Name);
            WorkGlobalLabelName = LoadFileName.Replace(".", "_");
            WorkLabelName = "";

            while ((line = streamReader.ReadLine()) != default(string))
            {
                var item = new LineItem(line, lineIndex, this);
                Items.Add(item);

                lineIndex++;
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

        public void PreAssemble(ref UInt16 address)
        {
            foreach (var item in Items)
            {
                try
                {
                    item.PreAssemble(ref address);
                }
                catch (ErrorMessageException ex)
                {
                    ErrorMessages.Add(new LineItemErrorMessage(ex, item));
                }
            }
        }

        public void SetValueLabel(Label[] labels)
        {
            foreach (var item in Items)
            {
                try
                {
                    item.SetValueLabel(labels);
                }
                catch (ErrorMessageException ex)
                {
                    ErrorMessages.Add(new LineItemErrorMessage(ex, item));
                }
            }
        }

        public void Assemble(Label[] labels)
        {
            // アセンブルを実行する
            foreach (var item in Items)
            {
                try
                {
                    item.Assemble(labels);
                }
                catch (ErrorMessageException ex)
                {
                    ErrorMessages.Add(new LineItemErrorMessage(ex, item));
                }
            }
        }

    }
}
