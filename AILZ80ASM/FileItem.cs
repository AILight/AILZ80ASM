﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AILZ80ASM
{
    public class FileItem
    {
        private AsmLoad AssembleLoad { get; set; }
        public FileInfo FileInfo { get; private set; }
        public List<LineItem> Items { get; private set; } = new List<LineItem>();

        public FileItem(FileInfo fileInfo, AsmLoad asmLoad)
        {
            // 重複読み込みチェック
            if (asmLoad.LoadFiles.Any(m => m.GetFullNameCaseSensitivity() == fileInfo.GetFullNameCaseSensitivity()))
            {
                throw new ErrorAssembleException(Error.ErrorCodeEnum.E2003, fileInfo.Name);
            }

            // スタックに読み込みファイルを積む
            asmLoad.LoadFiles.Push(fileInfo);

            AssembleLoad = asmLoad;
            FileInfo = fileInfo;

            using var streamReader = fileInfo.OpenText();
            Read(streamReader);
            streamReader.Close();

            asmLoad.LoadFiles.Pop();

        }

        private void Read(StreamReader streamReader)
        {
            string line;
            var lineIndex = 1;

            while ((line = streamReader.ReadLine()) != default(string))
            {
                var item = new LineItem(line, lineIndex, FileInfo);
                lineIndex++;
                try
                {
                    item.CreateLineDetailItem(AssembleLoad);
                    Items.Add(item);
                }
                catch (ErrorAssembleException ex)
                {
                    AssembleLoad.Errors.Add(new ErrorLineItem(item, ex));
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
                catch (ErrorAssembleException ex)
                {
                    AssembleLoad.Errors.Add(new ErrorLineItem(item, ex));
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
                catch (ErrorAssembleException ex)
                {
                    AssembleLoad.Errors.Add(new ErrorLineItem(item, ex));
                }
            }
        }

        public void BuildArgumentLabel()
        {
            foreach (var item in Items)
            {
                try
                {
                    item.BuildArgumentLabel();
                }
                catch (ErrorAssembleException ex)
                {
                    AssembleLoad.Errors.Add(new ErrorLineItem(item, ex));
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
                catch (ErrorAssembleException ex)
                {
                    AssembleLoad.Errors.Add(new ErrorLineItem(item, ex));
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
                catch (ErrorAssembleException ex)
                {
                    AssembleLoad.Errors.Add(new ErrorLineItem(item, ex));
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
                catch (ErrorAssembleException ex)
                {
                    AssembleLoad.Errors.Add(new ErrorLineItem(item, ex));
                }
            }
        }

        /// <summary>
        /// バイナリーを保存
        /// </summary>
        /// <param name="stream"></param>
        public void SaveBin(Stream stream)
        {
            var bin = this.Bin;
            if (bin.Length > 0)
            {
                stream.Write(bin, 0, bin.Length);
            }

        }

        /// <summary>
        /// リストファイルを保存
        /// </summary>
        /// <param name="stream"></param>
        public void SaveList(StreamWriter streamWriter)
        {
            streamWriter.WriteLineFileInfoBOF(FileInfo);

            foreach (var item in Items)
            {
                item.SaveList(streamWriter);
            }
            
            streamWriter.WriteLineFileInfoEOF(FileInfo);
        }
    }
}
