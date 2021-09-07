using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AILZ80ASM
{
    public class FileItem
    {
        private Package Package { get; set; }
        public FileInfo FileInfo { get; private set; }
        public List<LineItem> Items { get; private set; } = new List<LineItem>();
        public List<LineItemErrorMessage> ErrorMessages { get; private set; } = new List<LineItemErrorMessage>();

        public FileItem(FileInfo fileInfo, Package package)
        {
            Package = package;
            FileInfo = fileInfo;

            using var streamReader = fileInfo.OpenText();
            Read(streamReader);
            streamReader.Close();
        }

        private void Read(StreamReader streamReader)
        {
            string line;
            var lineIndex = 0;
            var loadFileName = Path.GetFileNameWithoutExtension(FileInfo.Name);
            
            Package.AssembleLoad.GlobalLableName = loadFileName.Replace(".", "_");

            while ((line = streamReader.ReadLine()) != default(string))
            {
                var item = new LineItem(line, lineIndex, Package.AssembleLoad);
                Items.Add(item);

                lineIndex++;
            }

        }

        /// <summary>
        /// マクロその他命令の展開
        /// </summary>
        /// <param name="macros"></param>
        public void ExpansionItem()
        {
            foreach (var item in Items)
            {
                try
                {
                    item.ExpansionItem();
                }
                catch (ErrorMessageException ex)
                {
                    ErrorMessages.Add(new LineItemErrorMessage(ex, item));
                }
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

        public void BuildAddressLabel()
        {
            foreach (var item in Items)
            {
                try
                {
                    item.BuildAddressLabel();
                }
                catch (ErrorMessageException ex)
                {
                    ErrorMessages.Add(new LineItemErrorMessage(ex, item));
                }
            }
        }

        public void Assemble()
        {
            // アセンブルを実行する
            foreach (var item in Items)
            {
                try
                {
                    item.Assemble();
                }
                catch (ErrorMessageException ex)
                {
                    ErrorMessages.Add(new LineItemErrorMessage(ex, item));
                }
            }
        }

        public void PreAssemble(ref AsmAddress address)
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
    }
}
