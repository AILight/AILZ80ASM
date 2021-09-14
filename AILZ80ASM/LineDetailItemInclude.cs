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

        private Dictionary<LineItem, LineDetailScopeItem[]> DicLineDetailScopeItem { get; set; } = new Dictionary<LineItem, LineDetailScopeItem[]>(); // 逆引き用
        private static readonly string RegexPatternInclude = @"\s*include\s*\""(?<Filename>.+)\""\s*,?\s*(?<Filetype>[^,]*)\s*,?\s*(?<StartAddress>[^,]*)\s*,?\s*(?<Length>[^,]*)";

        public LineDetailItemInclude(LineItem lineItem, FileInfo fileInfo, FileTypeEnum fileType, int start, int length, AsmLoad asmLoad)
            : base(lineItem, asmLoad)
        {
            FileInfo = fileInfo;

            // ファイルの存在チェック
            if (!fileInfo.Exists)
            {
                throw new ErrorMessageException(Error.ErrorCodeEnum.E2002, fileInfo.Name);
            }

            // 重複読み込みチェック
            if (asmLoad.LoadFiles.Any(m => m.FullName == fileInfo.FullName))
            {
                throw new ErrorMessageException(Error.ErrorCodeEnum.E2003, fileInfo.Name);
            }

            // スタックに読み込みファイルを積む
            asmLoad.LoadFiles.Push(fileInfo);

            using var streamReader = fileInfo.OpenText();

            var errorLineItemMessages = new List<ErrorLineItemMessage>();
            var line = default(string);
            var lineIndex = 0;

            while ((line = streamReader.ReadLine()) != default(string))
            {
                var localLineItem = new LineItem(line, lineIndex, fileInfo);
                lineIndex++;
                try
                {
                    localLineItem.CreateLineDetailItem(asmLoad);
                    LineItems.Add(localLineItem);

                    // 内部エラーを積む
                    if (localLineItem?.LineDetailItem?.InternalErrorMessageException != default)
                    {
                        throw localLineItem.LineDetailItem.InternalErrorMessageException;
                    }
                }
                catch (ErrorMessageException ex)
                {
                    errorLineItemMessages.Add(new ErrorLineItemMessage(ex, localLineItem));
                }
            }
            asmLoad.LoadFiles.Pop();

            // エラーを処理
            if (errorLineItemMessages.Count > 0)
            {
                throw new ErrorMessageException(Error.ErrorCodeEnum.E2001, new ErrorFileInfoMessage(errorLineItemMessages.ToArray(), FileInfo));
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

        public override void ExpansionItem()
        {
            var lineDetailScopeItem = new List<LineDetailScopeItem>();
            var errorLineItemMessages = new List<ErrorLineItemMessage>();

            foreach (var lineItem in LineItems)
            {
                try
                {
                    lineItem.ExpansionItem();
                    lineDetailScopeItem.AddRange(lineItem.LineDetailItem.LineDetailScopeItems);
                    DicLineDetailScopeItem.Add(lineItem, lineItem.LineDetailItem.LineDetailScopeItems);
                }
                catch (ErrorMessageException ex)
                {
                    errorLineItemMessages.Add(new ErrorLineItemMessage(ex, lineItem));
                }
            }

            LineDetailScopeItems = lineDetailScopeItem.ToArray();

            if (errorLineItemMessages.Count > 0)
            {
                throw new ErrorMessageException(Error.ErrorCodeEnum.E2001, new ErrorFileInfoMessage(errorLineItemMessages.ToArray(), FileInfo));
            }

            base.ExpansionItem();
        }

        public override void PreAssemble(ref AsmAddress asmAddress)
        {
            if (LineDetailScopeItems == default)
                return;

            var errorLineItemMessages = new List<ErrorLineItemMessage>();

            foreach (var item in LineDetailScopeItems)
            {
                try
                {
                    item.PreAssemble(ref asmAddress);
                }
                catch (ErrorMessageException ex)
                {
                    var lineItem = DicLineDetailScopeItem.Single(m => m.Value.Any(n => n == item)).Key;
                    errorLineItemMessages.Add(new ErrorLineItemMessage(ex, lineItem));
                }
            }

            if (errorLineItemMessages.Count > 0)
            {
                throw new ErrorMessageException(Error.ErrorCodeEnum.E2001, new ErrorFileInfoMessage(errorLineItemMessages.ToArray(), FileInfo));
            }
        }

        public override void BuildAddressLabel()
        {
            if (LineDetailScopeItems == default)
                return;

            var errorLineItemMessages = new List<ErrorLineItemMessage>();

            foreach (var item in LineDetailScopeItems)
            {
                try
                {
                    item.BuildAddressLabel();
                }
                catch (ErrorMessageException ex)
                {
                    var lineItem = DicLineDetailScopeItem.Single(m => m.Value.Any(n => n == item)).Key;
                    errorLineItemMessages.Add(new ErrorLineItemMessage(ex, lineItem));
                }
            }

            if (errorLineItemMessages.Count > 0)
            {
                throw new ErrorMessageException(Error.ErrorCodeEnum.E2001, new ErrorFileInfoMessage(errorLineItemMessages.ToArray(), FileInfo));
            }
        }

        public override void Assemble()
        {
            if (LineDetailScopeItems == default)
                return;

            var errorLineItemMessages = new List<ErrorLineItemMessage>();

            foreach (var item in LineDetailScopeItems)
            {
                try
                {
                    item.Assemble();
                }
                catch (ErrorMessageException ex)
                {
                    var lineItem = DicLineDetailScopeItem.Single(m => m.Value.Any(n => n == item)).Key;
                    errorLineItemMessages.Add(new ErrorLineItemMessage(ex, lineItem));
                }
            }

            if (errorLineItemMessages.Count > 0)
            {
                throw new ErrorMessageException(Error.ErrorCodeEnum.E2001, new ErrorFileInfoMessage(errorLineItemMessages.ToArray(), FileInfo));
            }
        }
    }
}
