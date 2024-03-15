using AILZ80ASM.Assembler;
using AILZ80ASM.Exceptions;
using System.Text.RegularExpressions;

namespace AILZ80ASM.LineDetailItems
{
    public class LineDetailItemPreProcError : LineDetailItem
    {
        // TODO: ラベルにLASTが使えない仕様になってしまっているので、あとでパーサーを強化して使えるようにする
        private static readonly string RegexPatternError = @"^#ERROR\s+""(?<message>.+)""$";
        public string Message { get; set; }

        private LineDetailItemPreProcError(LineItem lineItem, AsmLoad asmLoad)
            : base(lineItem, asmLoad)
        {

        }

        public static LineDetailItemPreProcError Create(LineItem lineItem, AsmLoad asmLoad)
        {
            if (!lineItem.IsCollectOperationString)
            {
                return default(LineDetailItemPreProcError);
            }

            var matched = Regex.Match(lineItem.OperationString, RegexPatternError, RegexOptions.Singleline | RegexOptions.IgnoreCase);

            // 開始条件チェック
            if (matched.Success)
            {
                // ラベルが存在していたらエラー
                if (!string.IsNullOrEmpty(lineItem.LabelString))
                {
                    throw new ErrorAssembleException(Error.ErrorCodeEnum.E1032);
                }

                var lineDetailItemError = new LineDetailItemPreProcError(lineItem, asmLoad)
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
