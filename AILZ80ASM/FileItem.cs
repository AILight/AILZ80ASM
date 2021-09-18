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
        public List<ErrorLineItemMessage> ErrorMessages { get; private set; } = new List<ErrorLineItemMessage>();

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
                var item = new LineItem(line, lineIndex, FileInfo);
                lineIndex++;
                try
                {
                    item.CreateLineDetailItem(Package.AssembleLoad);
                    Items.Add(item);

                    // 内部エラーを積む
                    if (item?.LineDetailItem?.InternalErrorMessageException != default)
                    {
                        throw item.LineDetailItem.InternalErrorMessageException;
                    }
                }
                catch (ErrorMessageException ex)
                {
                    ErrorMessages.Add(new ErrorLineItemMessage(ex, item));
                }
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
                    ErrorMessages.Add(new ErrorLineItemMessage(ex, item));
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
                    ErrorMessages.Add(new ErrorLineItemMessage(ex, item));
                }
            }
        }


        public void BuildValueLabel()
        {
            foreach (var item in Items)
            {
                try
                {
                    item.BuildValueLabel();
                }
                catch (ErrorMessageException ex)
                {
                    ErrorMessages.Add(new ErrorLineItemMessage(ex, item));
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
                    ErrorMessages.Add(new ErrorLineItemMessage(ex, item));
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
                    ErrorMessages.Add(new ErrorLineItemMessage(ex, item));
                }
            }
        }

    }
}
