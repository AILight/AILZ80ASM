using AILZ80ASM.AILight;
using AILZ80ASM.Assembler;
using AILZ80ASM.Exceptions;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AILZ80ASM.LineDetailItems
{
    public class LineDetailItemPreProcPrint : LineDetailItem
    {
        private static readonly string RegexPatternPrint = @"^#PRINT";
        private static readonly string RegexPatternPrintWithArgument = @"^#PRINT\s+(?<message>.+)$";
        public string Message { get; set; }

        private LineDetailItemPreProcPrint(LineItem lineItem, AsmLoad asmLoad)
            : base(lineItem, asmLoad)
        {

        }

        public static LineDetailItemPreProcPrint Create(LineItem lineItem, AsmLoad asmLoad)
        {
            if (!lineItem.IsCollectOperationString)
            {
                return default(LineDetailItemPreProcPrint);
            }

            var matched = Regex.Match(lineItem.OperationString, RegexPatternPrint, RegexOptions.Singleline | RegexOptions.IgnoreCase);

            // 開始条件チェック
            if (matched.Success)
            {
                // ラベルが存在していたらエラー
                if (!string.IsNullOrEmpty(lineItem.LabelString))
                {
                    throw new ErrorAssembleException(Error.ErrorCodeEnum.E1042);
                }
                var matchedWithArgument = Regex.Match(lineItem.OperationString, RegexPatternPrintWithArgument, RegexOptions.Singleline | RegexOptions.IgnoreCase);
                if (matchedWithArgument.Success)
                {
                    var lineDetailItemPreProcPrint = new LineDetailItemPreProcPrint(lineItem, asmLoad)
                    {
                        Message = matchedWithArgument.Groups["message"].Value
                    };
                    return lineDetailItemPreProcPrint;
                }
                else
                {
                    throw new ErrorAssembleException(Error.ErrorCodeEnum.E1041, "メッセージを設定してください。");
                }
            }

            return default;
        }

        public override void PreAssemble(ref AsmAddress asmAddress)
        {
            //base.PreAssemble(ref asmAddress);
            var messages = AIName.ParseArguments(this.Message);
            if (!AIString.IsString(messages[0], AsmLoad))
            {
                throw new ErrorAssembleException(Error.ErrorCodeEnum.E1041, "メッセージは文字列として設定して下さい。「\"」もしくは[']で囲う");
            }

            // メッセージFormatを作成
            var messageFormat = messages[0].Substring(1, messages[0].Length - 2);

            // 引数追加
            var args = new List<object>();
            foreach (var item in messages.Skip(1))
            {
                var calc = AIMath.Calculation(item, AsmLoad, asmAddress);

                args.Add(calc.ConvertTo<object>());
            }

            var message = string.Format(messageFormat, args.ToArray());
            AsmLoad.AddError(new ErrorLineItem(this.LineItem, Error.ErrorCodeEnum.I0001, message));
        }
    }
}
