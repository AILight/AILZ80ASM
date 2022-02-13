using System.Text.RegularExpressions;
using AILZ80ASM.Exceptions;

namespace AILZ80ASM
{
    public class LineDetailItemCharMap : LineDetailItem
    {
        // TODO: ラベルにLASTが使えない仕様になってしまっているので、あとでパーサーを強化して使えるようにする
        private static readonly string RegexPatternCharMap = @"^\s*CHARMAP\s+(?<charmap>.+)$";
        public string CharMapName { get; set; }

        private LineDetailItemCharMap(LineItem lineItem, AsmLoad asmLoad)
            : base(lineItem, asmLoad)
        {

        }

        public static LineDetailItemCharMap Create(LineItem lineItem, AsmLoad asmLoad)
        {
            var matched = Regex.Match(lineItem.OperationString, RegexPatternCharMap, RegexOptions.Singleline | RegexOptions.IgnoreCase);

            // 開始条件チェック
            if (matched.Success)
            {
                // ラベルが存在していたらエラー
                if (!string.IsNullOrEmpty(lineItem.LabelString))
                {
                    throw new ErrorAssembleException(Error.ErrorCodeEnum.E1032);
                }

                var lineDetailItemCharMap = new LineDetailItemCharMap(lineItem, asmLoad)
                {
                    CharMapName = matched.Groups["charmap"].Value
                };
                asmLoad.CharMap = matched.Groups["charmap"].Value;

                return lineDetailItemCharMap;
            }

            return default;
        }
    }
}
