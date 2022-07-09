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

                var charMap = matched.Groups["Charmap"].Value;
                var filePath = matched.Groups["Filename"].Value;
                if (string.IsNullOrEmpty(filePath))
                {
                    throw new ErrorAssembleException(Error.ErrorCodeEnum.E2101, "");
                }
                else
                {
                    if (!filePath.StartsWith('\"') || !filePath.EndsWith('\"'))
                    {
                        throw new ErrorAssembleException(Error.ErrorCodeEnum.E2103, filePath);
                    }

                    try
                    {
                        var filename = filePath.Substring(1, filePath.Length - 2);
                        var fileFullPath = Path.Combine(lineItem.FileInfo.Directory.FullName, filename);

                        var fileInfo = new FileInfo(fileFullPath);
                        asmLoad.CharMapConverter_ReadCharMapFromFile(charMap, fileInfo);
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

                var lineDetailItemCharMap = new LineDetailItemCharMap(lineItem, asmLoad)
                {
                    CharMapName = charMap
                };

                return lineDetailItemCharMap;

            }

            return default;
        }
    }
}
