using AILZ80ASM.Assembler;
using AILZ80ASM.Exceptions;
using AILZ80ASM.LineDetailItems.ScopeItem;
using AILZ80ASM.LineDetailItems.ScopeItem.ExpansionItems;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace AILZ80ASM.LineDetailItems
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
        public override AsmResult[] BinResults => FileType == FileTypeEnum.Text ? FileItem.BinResults : base.BinResults;

        private static readonly string RegexPatternInclude = @"^include\s*\""(?<Filename>.+)\""\s*,?\s*(?<Filetype>[^,]*)\s*,?\s*(?<StartAddress>[^,]*)\s*,?\s*(?<Length>[^,]*)";
        private static readonly Regex CompiledRegexPatternInclude = new Regex(
            RegexPatternInclude,
            RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase
        );

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
            if (!lineItem.IsCollectOperationString)
            {
                return default(LineDetailItemInclude);
            }

            var matched = CompiledRegexPatternInclude.Match(lineItem.OperationString);
            if (matched.Success)
            {
                var filename = matched.Groups["Filename"].Value;
                var fileTypeString = matched.Groups["Filetype"].Value;
                var startAddressString = matched.Groups["StartAddress"].Value;
                var lengthString = matched.Groups["Length"].Value;
                var fileFullPath = Path.Combine(lineItem.FileInfo.Directory.FullName, filename);
                var fileInfo = new FileInfo(fileFullPath);

                // ファイルを検索する
                if (asmLoad.AssembleOption.IncludePaths != default && !fileInfo.Exists)
                {
                    foreach (var item in asmLoad.AssembleOption.IncludePaths)
                    {
                        var localFileFullPath = Path.Combine(item.FullName, filename);
                        if (Path.Exists(localFileFullPath))
                        {
                            fileInfo = new FileInfo(localFileFullPath);
                            break;
                        }
                    }
                }

                var fileType = LineDetailItemInclude.FileTypeEnum.Text;

                if (string.IsNullOrEmpty(fileTypeString) || (new[] { "T", "Text" }).Any(m => string.Equals(m, fileTypeString, StringComparison.OrdinalIgnoreCase)))
                {
                    fileType = LineDetailItemInclude.FileTypeEnum.Text;
                    if (!string.IsNullOrEmpty(startAddressString) || !string.IsNullOrEmpty(lengthString))
                    {
                        throw new ErrorAssembleException(Error.ErrorCodeEnum.E2009);
                    }
                }
                else if ((new[] { "B", "Binary" }).Any(m => string.Equals(m, fileTypeString, StringComparison.OrdinalIgnoreCase)))
                {
                    fileType = LineDetailItemInclude.FileTypeEnum.Binary;
                }
                else
                {
                    throw new ErrorAssembleException(Error.ErrorCodeEnum.E2008);
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
            Address = asmAddress;
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

        public override AsmList[] Lists
        {
            get
            {
                if (!AsmLoad.Share.IsOutputList)
                {
                    return new AsmList[] { };
                }

                var lists = new List<AsmList>();
                if (this.AsmLoad.AssembleOption.ListOmitBinaryFile && FileType == FileTypeEnum.Binary)
                {
                    // バイナリー出力を省略して出力する場合
                    lists.Add(AsmList.CreateLineItem(LineDetailExpansionItem.Address, LineDetailExpansionItem.BinResults.SelectMany(m => m.Data ?? new byte[] { }).ToArray(), "", LineItem));
                }
                else
                {
                    lists.Add(AsmList.CreateLineItem(LineItem));

                    switch (FileType)
                    {
                        case FileTypeEnum.Text:
                            lists.AddRange(FileItem.Lists);
                            break;
                        case FileTypeEnum.Binary:
                            if (this.AsmLoad.AssembleOption.ListOmitBinaryFile)
                            {
                                lists.Add(LineDetailExpansionItem.List);
                            }
                            else
                            {
                                lists.Add(LineDetailExpansionItem.List);
                                lists.Add(AsmList.CreateFileInfoEOF(FileInfo, LineDetailExpansionItem.BinResults.Sum(m => m.Data?.Length ?? 0)));
                            }
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                }

                return lists.ToArray();
            }
        }
    }
}
