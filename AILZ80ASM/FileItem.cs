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

        public string LoadFileName {get ; private set;}
        public FileInfo FileInfo { get; private set; }
        public List<LineItem> Items { get; private set; } = new List<LineItem>();
        public List<Macro> Macros { get; private set; } = new List<Macro>();
        public Label[] Labels => Items.SelectMany(m => m.Labels).ToArray();

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

        /// <summary>
        /// マクロをロードする
        /// </summary>
        public void LoadMacro()
        {
            var whileMacro = false;
            var macroItems = new List<LineItem>();

            foreach (var item in Items)
            {
                if (item.OperationString.TrimStart().ToUpper().StartsWith("MACRO"))
                {
                    if (whileMacro)
                    {
                        ErrorMessages.Add(new LineItemErrorMessage(new ErrorMessageException(Error.ErrorCodeEnum.E0010), item));
                    }
                    whileMacro = true;
                    macroItems.Clear();
                }

                if (whileMacro)
                {
                    macroItems.Add(item);
                }

                if (item.OperationString.TrimStart().ToUpper().StartsWith("END MACRO"))
                {
                    Macros.Add(new Macro(macroItems, this));
                    whileMacro = false;
                }
            }
            if (whileMacro)
            {
                ErrorMessages.Add(new LineItemErrorMessage(new ErrorMessageException(Error.ErrorCodeEnum.E0010), Items.Last()));
            }
        }

        public void ProcessLabelValue(Label[] labels)
        {
            foreach (var item in Items)
            {
                item.ProcessLabelValue(labels);
            }

        }


        public void ProcessLabelValueAndAddress(Label[] labels)
        {
            foreach (var item in Items)
            {
                item.ProcessLabelValueAndAddress(labels);
            }
        }

        /// <summary>
        /// マクロその他命令の展開
        /// </summary>
        /// <param name="macros"></param>
        public void ExpansionItem(List<Macro> macros)
        {
            foreach (var item in Items)
            {
                try
                {
                    // マクロの展開（グローバルマクロ）
                    {
                        var foundMacros = Macros.Where(row => row.FullName == item.InstructionText);
                        if (foundMacros.Count() > 2)
                        {
                            throw new ErrorMessageException(Error.ErrorCodeEnum.E0011);
                        }
                        if (foundMacros.Count() == 1)
                        {
                            item.ExpansionMacro(foundMacros.First());
                            continue;
                        }
                    }
                    // ローカルマクロ
                    {
                        var foundMacros = Macros.Where(row => row.Name == item.InstructionText);
                        if (foundMacros.Count() > 2)
                        {
                            throw new ErrorMessageException(Error.ErrorCodeEnum.E0011);
                        }
                        if (foundMacros.Count() == 1)
                        {
                            item.ExpansionMacro(foundMacros.First());
                            continue;
                        }
                    }

                    // 通常命令の展開
                    item.ExpansionItem();

                }
                catch (ErrorMessageException ex)
                {
                    ErrorMessages.Add(new LineItemErrorMessage(ex, item));
                }
            }
        }

        public void PreAssemble(ref AsmAddress address, Label[] labels)
        {
            foreach (var item in Items)
            {
                try
                {
                    item.PreAssemble(ref address, labels);
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
