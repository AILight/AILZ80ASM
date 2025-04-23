using AILZ80ASM.AILight;
using AILZ80ASM.Assembler;
using AILZ80ASM.Exceptions;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using static AILZ80ASM.Assembler.Error;

namespace AILZ80ASM.LineDetailItems
{
    public class LineDetailItemPreProcList : LineDetailItem
    {
        // TODO: ラベルにLASTが使えない仕様になってしまっているので、あとでパーサーを強化して使えるようにする
        private static readonly string RegexPatternList = @"^#LIST";
        private static readonly Regex CompiledRegexPatternList = new Regex(
            RegexPatternList,
            RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase
        );

        private static readonly string RegexPatternListWithArgument = @"^#LIST\s+(?<condition>.+)$";
        private static readonly Regex CompiledRegexPatternListWithArgument = new Regex(
            RegexPatternListWithArgument,
            RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase
        );

        public string Condition { get; set; }
        public bool IsOutputList { get; set; }

        private LineDetailItemPreProcList(LineItem lineItem, AsmLoad asmLoad)
            : base(lineItem, asmLoad)
        {

        }

        public static LineDetailItemPreProcList Create(LineItem lineItem, AsmLoad asmLoad)
        {
            if (!lineItem.IsCollectOperationString)
            {
                return default(LineDetailItemPreProcList);
            }

            var matched = CompiledRegexPatternList.Match(lineItem.OperationString);

            // 開始条件チェック
            if (matched.Success)
            {
                // ラベルが存在していたらエラー
                if (!string.IsNullOrEmpty(lineItem.LabelString))
                {
                    throw new ErrorAssembleException(Error.ErrorCodeEnum.E1052);
                }
                var matchedWithArgument = CompiledRegexPatternListWithArgument.Match(lineItem.OperationString);
                if (matchedWithArgument.Success)
                {
                    var lineDetailItemPreProcList = new LineDetailItemPreProcList(lineItem, asmLoad)
                    {
                        Condition = matchedWithArgument.Groups["condition"].Value
                    };
                    return lineDetailItemPreProcList;
                }
                else
                {
                    throw new ErrorAssembleException(Error.ErrorCodeEnum.E1051, "条件(bool型)を設定してください。");
                }
            }

            return default;
        }

        public override void PreAssemble(ref AsmAddress asmAddress)
        {
            //base.PreAssemble(ref asmAddress);
            if (AIMath.TryParse(Condition, AsmLoad, out var resultValue))
            {
                IsOutputList = resultValue.ConvertTo<bool>();
            }
            else
            {
                throw new ErrorAssembleException(Error.ErrorCodeEnum.E1051, "条件を設定してください。");
            }
        }

        public override AsmList[] Lists
        {
            get
            {
                this.AsmLoad.Share.IsOutputList = IsOutputList;
                // #LISTは、出力しない
                return new AsmList[] { };
            }
        }
    }
}
