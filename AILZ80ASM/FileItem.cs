using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AILZ80ASM
{
    public class FileItem
    {
        private Package Package { get; set; }
        
        internal string LoadFileName { get; set; }
        internal List<LineItem> Items { get; set; } = new List<LineItem>();
        internal string WorkGlobalLabelName { get; set; }
        internal string WorkLabelName { get; set; }
        internal List<LineItemErrorMessage> ErrorMessages { get; set; } = new List<LineItemErrorMessage>();

        public FileItem(FileInfo fileInfo, Package package)
        {
            Package = package;

            using var streamReader = fileInfo.OpenText();
            Read(streamReader, fileInfo.Name);
            streamReader.Close();
        }

        public FileItem(StreamReader streamReader, string fileName)
        {
            Read(streamReader, fileName);
        }

        private void Read(StreamReader streamReader, string fileName)
        {
            string line;
            var lineIndex = 0;
            WorkGlobalLabelName = fileName;
            WorkLabelName = "";

            while (!string.IsNullOrEmpty(line = streamReader.ReadLine()))
            {
                var item = new LineItem(line, lineIndex, this);
                Items.Add(item);

                lineIndex++;
            }

            LoadFileName = Path.GetFileNameWithoutExtension(fileName);
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
                    ErrorMessages.Add(new LineItemErrorMessage(ex.ErrorType, ex.Message, item));
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
                    ErrorMessages.Add(new LineItemErrorMessage(ex.ErrorType, ex.Message, item));
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
                    ErrorMessages.Add(new LineItemErrorMessage(ex.ErrorType, ex.Message, item));
                }
            }
        }

    }
}
