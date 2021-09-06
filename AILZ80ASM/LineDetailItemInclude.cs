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
        private static readonly string RegexPatternInclude = @"\s*include\s*\""(?<Filename>.+)\""\s*,?\s*(?<Filetype>[^,]*)\s*,?\s*(?<StartAddress>[^,]*)\s*,?\s*(?<Length>[^,]*)";

        public LineDetailItemInclude(LineItem lineItem, FileInfo fileInfo, FileTypeEnum fileType, int start, int length, AsmLoad asmLoad)
            : base(lineItem, asmLoad)
        {
            FileInfo = fileInfo;

            // ファイルの存在チェック
            if (!fileInfo.Exists)
            {
                throw new ErrorMessageException(Error.ErrorCodeEnum.E0001, $"Filename:{fileInfo.Name}");
            }

            // 重複読み込みチェック
            if (asmLoad.LoadFiles.Any(m => m.FullName == fileInfo.FullName))
            {
                throw new ErrorMessageException(Error.ErrorCodeEnum.E0002, $"Filename:{fileInfo.Name}");
            }

            // スタックに読み込みファイルを積む
            asmLoad.LoadFiles.Push(fileInfo);

            using var streamReader = fileInfo.OpenText();

            var line = default(string);
            var lineIndex = 0;

            while ((line = streamReader.ReadLine()) != default(string))
            {
                var localLineItem = new LineItem(line, lineIndex, asmLoad);
                LineItems.Add(localLineItem);

                lineIndex++;
            }
            asmLoad.LoadFiles.Pop();
        }

        public static LineDetailItemInclude Create(LineItem lineItem, AsmLoad asmLoad)
        {
            var matched = Regex.Match(lineItem.OperationString, RegexPatternInclude, RegexOptions.Singleline | RegexOptions.IgnoreCase);
            if (matched.Success)
            {
                var filename = matched.Groups["Filename"].Value;
                var fileTypeString = matched.Groups["Filetype"].Value;
                var startAddressString = matched.Groups["StartAddress"].Value;
                var lengthString = matched.Groups["Length"].Value;

                var fileInfo = new FileInfo(filename);
                var fileType = LineDetailItemInclude.FileTypeEnum.Text;
                var start = 0;
                var length = int.MaxValue;

                if ((new[] { "B", "Binary" }).Any(m => string.Compare(m, fileTypeString, StringComparison.OrdinalIgnoreCase) == 0))
                {
                    fileType = LineDetailItemInclude.FileTypeEnum.Binary;
                }

                if (int.TryParse(startAddressString, out var resultStart))
                {
                    start = resultStart;
                }

                if (int.TryParse(startAddressString, out var resultLength))
                {
                    length = resultLength;
                }

                return new LineDetailItemInclude(lineItem, fileInfo, fileType, start, length, asmLoad);
            }
            
            return default;
        }

        public override void ExpansionItem()
        {
            var lineDetailExpansionItems = new List<LineDetailExpansionItem>();

            foreach (var lineItem in LineItems)
            {
                lineItem.ExpansionItem();
                lineDetailExpansionItems.AddRange(lineItem.LineDetailItem.LineDetailExpansionItems);
            }
            this.LineDetailExpansionItems = lineDetailExpansionItems.ToArray();

            base.ExpansionItem();
        }
    }
}
