using AILZ80ASM.Assembler;
using AILZ80ASM.Exceptions;
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
                if (!string.IsNullOrEmpty(filePath))
                {
                    if (!filePath.StartsWith("\"") ||
                        !filePath.EndsWith("\""))
                    {
                        throw new ErrorAssembleException(Error.ErrorCodeEnum.E2103, filePath);
                    }

                    try
                    {
                        asmLoad.CharMapConverter_ReadCharMapFromFile(charMap, filePath.Substring(1, filePath.Length - 2));
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
                        throw new ErrorAssembleException(Error.ErrorCodeEnum.E2101, ex.MapName);
                    }
                }
                else
                {
                    if (!asmLoad.CharMapConverter_IsContains(charMap))
                    {
                        try
                        {
                            asmLoad.CharMapConverter_ReadCharMapFromResource(charMap);
                        }
                        catch (FileNotFoundException ex)
                        {
                            throw new ErrorAssembleException(Error.ErrorCodeEnum.E2102, ex.Message);
                        }
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
