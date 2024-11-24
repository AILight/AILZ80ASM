using AILZ80ASM.Assembler;
using AILZ80ASM.Exceptions;
using System;
using System.IO;
using System.Text.RegularExpressions;

namespace AILZ80ASM.LineDetailItems
{
    public class LineDetailItemCharMap : LineDetailItem
    {
        private static readonly string RegexPatternCharMap = @"^\s*CHARMAP\s+(?<Charmap>@[a-zA-Z0-9_]+)\s*,?\s*(?<Filename>.*)$";
        public string CharMapName { get; set; }
        public string FilePath { get; set; }

        private LineDetailItemCharMap(LineItem lineItem, AsmLoad asmLoad)
            : base(lineItem, asmLoad)
        {

        }

        public static LineDetailItemCharMap Create(LineItem lineItem, AsmLoad asmLoad)
        {
            if (!lineItem.IsCollectOperationString)
            {
                return default(LineDetailItemCharMap);
            }

            var matched = Regex.Match(lineItem.OperationString, RegexPatternCharMap, RegexOptions.Singleline | RegexOptions.IgnoreCase);

            // 開始条件チェック
            if (matched.Success)
            {
                // ラベルが存在していたらエラー
                if (!string.IsNullOrEmpty(lineItem.LabelString))
                {
                    throw new ErrorAssembleException(Error.ErrorCodeEnum.E2107);
                }

                var lineDetailItemCharMap = new LineDetailItemCharMap(lineItem, asmLoad)
                {
                    CharMapName = matched.Groups["Charmap"].Value,
                    FilePath = matched.Groups["Filename"].Value
                };

                return lineDetailItemCharMap;

            }

            return default;
        }

        public override void PreAssemble(ref AsmAddress asmAddress)
        {
            base.PreAssemble(ref asmAddress);

            if (string.IsNullOrEmpty(FilePath))
            {
                throw new ErrorAssembleException(Error.ErrorCodeEnum.E2101, "");
            }
            else
            {
                if (!FilePath.StartsWith('\"') || !FilePath.EndsWith('\"'))
                {
                    throw new ErrorAssembleException(Error.ErrorCodeEnum.E2103, FilePath);
                }

                try
                {
                    var filename = FilePath.Substring(1, FilePath.Length - 2);
                    var fileFullPath = Path.Combine(this.LineItem.FileInfo.Directory.FullName, filename);

                    var fileInfo = new FileInfo(fileFullPath);
                    this.AsmLoad.CharMapConverter_ReadCharMapFromFile(CharMapName, fileInfo);
                }
                catch (CharMapJsonReadException ex)
                {
                    if (ex.LineNumber.HasValue)
                    {
                        throw new ErrorAssembleException(Error.ErrorCodeEnum.E2104, $"行番号: {ex.LineNumber}");
                    }
                    else
                    {
                        throw new ErrorAssembleException(Error.ErrorCodeEnum.E2104, ex.Message);
                    }
                }
                catch (FileNotFoundException ex)
                {
                    throw new ErrorAssembleException(Error.ErrorCodeEnum.E2101, ex.FileName);
                }
                catch (CharMapAlreadyDefinedException ex)
                {
                    throw new ErrorAssembleException(Error.ErrorCodeEnum.E2108, ex.MapName);
                }
            }
        }
    }
}
