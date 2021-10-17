using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace AILZ80ASM
{
    public class LineDetailItemInclude : LineDetailItem
    {
        public enum FileTypeEnum
        {
            Text,
            Binary
        }

        public FileInfo FileInfo { get; private set; }
        public List<LineItem> LineItems { get; private set; } = new List<LineItem>();
        public override byte[] Bin => FileType == FileTypeEnum.Text ? FileItem.Bin : base.Bin;

        private static readonly string RegexPatternInclude = @"^include\s*\""(?<Filename>.+)\""\s*,?\s*(?<Filetype>[^,]*)\s*,?\s*(?<StartAddress>[^,]*)\s*,?\s*(?<Length>[^,]*)";
        private FileTypeEnum FileType { get; set; } = FileTypeEnum.Text;
        private string FileStart { get; set; }
        private string FileLength { get; set; }

        private FileItem FileItem { get; set; }
        private LineDetailExpansionItem LineDetailExpansionItem { get; set; }

        private LineDetailItemInclude(LineItem lineItem, FileInfo fileInfo, FileTypeEnum fileType, string fileStart, string fileLength, AsmLoad asmLoad)
            : base(lineItem, asmLoad)
        {
            FileInfo = fileInfo;
            FileType = fileType;
            FileStart = fileStart;
            FileLength = fileLength;

            // ファイルの存在チェック
            if (!fileInfo.Exists)
            {
                throw new ErrorAssembleException(Error.ErrorCodeEnum.E2002, fileInfo.Name);
            }

            switch (FileType)
            {
                case FileTypeEnum.Text:
                    FileItem = new FileItem(fileInfo, asmLoad);
                    break;
                case FileTypeEnum.Binary:
                    LineDetailExpansionItem = new LineDetailExpansionItemBinaryFile(LineItem, FileInfo, FileStart, FileLength);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        public static LineDetailItemInclude Create(LineItem lineItem, AsmLoad asmLoad)
        {
            var matched = Regex.Match(lineItem.OperationString, RegexPatternInclude, RegexOptions.Singleline | RegexOptions.IgnoreCase);
            if (matched.Success)
            {
                if (!string.IsNullOrEmpty(lineItem.LabelString))
                {
                    throw new ErrorAssembleException(Error.ErrorCodeEnum.E2007);
                }

                var filename = matched.Groups["Filename"].Value;
                var fileTypeString = matched.Groups["Filetype"].Value;
                var startAddressString = matched.Groups["StartAddress"].Value;
                var lengthString = matched.Groups["Length"].Value;

                var fileInfo = new FileInfo(filename);
                var fileType = LineDetailItemInclude.FileTypeEnum.Text;

                if ((new[] { "B", "Binary" }).Any(m => string.Compare(m, fileTypeString, StringComparison.OrdinalIgnoreCase) == 0))
                {
                    fileType = LineDetailItemInclude.FileTypeEnum.Binary;
                }

                return new LineDetailItemInclude(lineItem, fileInfo, fileType, startAddressString, lengthString, asmLoad);
            }
            
            return default;
        }

        public override void ExpansionItem()
        {
            switch (FileType)
            {
                case FileTypeEnum.Text:
                    FileItem.ExpansionItem();
                    break;
                case FileTypeEnum.Binary:
                    LineDetailScopeItems = new[] { new LineDetailScopeItem(new LineDetailExpansionItem[] { LineDetailExpansionItem }, AsmLoad) };
                    break;
                default:
                    throw new NotImplementedException();
            }

            base.ExpansionItem();
        }

        public override void PreAssemble(ref AsmAddress asmAddress)
        {
            switch (FileType)
            {
                case FileTypeEnum.Text:
                    FileItem.PreAssemble(ref asmAddress);
                    break;
                case FileTypeEnum.Binary:
                    base.PreAssemble(ref asmAddress);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        public override void BuildValueLabel()
        {
            switch (FileType)
            {
                case FileTypeEnum.Text:
                    FileItem.BuildValueLabel();
                    break;
                case FileTypeEnum.Binary:
                    base.BuildValueLabel();
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        public override void BuildAddressLabel()
        {
            switch (FileType)
            {
                case FileTypeEnum.Text:
                    FileItem.BuildAddressLabel();
                    break;
                case FileTypeEnum.Binary:
                    base.BuildAddressLabel();
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        public override void BuildArgumentLabel()
        {
            switch (FileType)
            {
                case FileTypeEnum.Text:
                    FileItem.BuildArgumentLabel();
                    break;
                case FileTypeEnum.Binary:
                    base.BuildArgumentLabel();
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        public override void Assemble()
        {
            switch (FileType)
            {
                case FileTypeEnum.Text:
                    FileItem.Assemble();
                    break;
                case FileTypeEnum.Binary:
                    base.Assemble();
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
