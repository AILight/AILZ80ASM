using System.Text.RegularExpressions;

namespace AILZ80ASM
{
    public class LineDetailItemError : LineDetailItem
    {
        // TODO: ラベルにLASTが使えない仕様になってしまっているので、あとでパーサーを強化して使えるようにする
        private static readonly string RegexPatternError = @"^\s*#ERROR\s+""(?<message>.+)""$";
        public string Message { get; set; }

        private LineDetailItemError(LineItem lineItem, AsmLoad asmLoad)
            : base(lineItem, asmLoad)
        {

        }

        public static LineDetailItemError Create(LineItem lineItem, AsmLoad asmLoad)
        {
            var matched = Regex.Match(lineItem.OperationString, RegexPatternError, RegexOptions.Singleline | RegexOptions.IgnoreCase);

            // 開始条件チェック
            if (matched.Success)
            {
                // ラベルが存在していたらエラー
                if (!string.IsNullOrEmpty(lineItem.LabelString))
                {
                    throw new ErrorAssembleException(Error.ErrorCodeEnum.E1032);
                }

                var lineDetailItemError = new LineDetailItemError(lineItem, asmLoad)
                {
                    Message = matched.Groups["message"].Value
                };

                return lineDetailItemError;
            }

            return default;
        }

        public override void ExpansionItem()
        {
            throw new ErrorAssembleException(Error.ErrorCodeEnum.E1031, this.Message);
        }
    }
}
