using AILZ80ASM.Assembler;
using AILZ80ASM.Exceptions;
using System.IO;
using System.Text.RegularExpressions;

namespace AILZ80ASM.LineDetailItems
{
    public class LineDetailItemPragma : LineDetailItem
    {
        private static readonly string RegexPatternPragma = @"^\s*#PRAGMA\s+(?<name>[0-9a-zA-Z]+)\s*(?<argument>.*)$";

        public override AsmList[] Lists
        {
            get
            {
                return new[]
                {
                    AsmList.CreateLineItem(LineItem)
                };
            }
        }

        private LineDetailItemPragma(LineItem lineItem, AsmLoad asmLoad)
            : base(lineItem, asmLoad)
        {

        }

        public static LineDetailItemPragma Create(LineItem lineItem, AsmLoad asmLoad)
        {
            if (!lineItem.IsCollectOperationString)
            {
                return default(LineDetailItemPragma);
            }

            var matched = Regex.Match(lineItem.OperationString, RegexPatternPragma, RegexOptions.Singleline | RegexOptions.IgnoreCase);
            if (matched.Success)
            {
                var name = matched.Groups["name"].Value;
                var argument = matched.Groups["argument"].Value;

                switch (name.ToUpper())
                {
                    case "ONCE":
                        asmLoad.AddPragmaOnceFileInfo(lineItem.FileInfo);
                        break;
                    /* コマンドライン引数は、こちらでオプション設定をする
                    case "OPTION":
                        switch (argument.ToUpper())
                        {
                            case "OUTPUT-TRIM":
                                break;
                            default:
                                return default;
                        }
                        break;
                    */
                    default:
                        return default;
                }

                return new LineDetailItemPragma(lineItem, asmLoad);
            }

            return default(LineDetailItemPragma);
        }

        public override void Assemble()
        {
        }

        public override void PreAssemble(ref AsmAddress asmAddress)
        {
        }

        public override void ExpansionItem()
        {
        }
    }
}
