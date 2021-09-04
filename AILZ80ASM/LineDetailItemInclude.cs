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
            : base(lineItem)
        {
            FileInfo = fileInfo;
            using var streamReader = fileInfo.OpenText();

            string line;
            var lineIndex = 0;

            while ((line = streamReader.ReadLine()) != default(string))
            {
                var localLineItem = new LineItem(line, lineIndex, asmLoad);
                LineItems.Add(localLineItem);

                lineIndex++;
            }
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

        public override void ExpansionItem(AsmLoad asmLoad)
        {
            foreach (var lineItem in LineItems)
            {
                lineItem.ExpansionItem(asmLoad);
            }

            base.ExpansionItem(asmLoad);
        }
    }
}
