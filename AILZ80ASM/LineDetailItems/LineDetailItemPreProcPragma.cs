using AILZ80ASM.Assembler;
using AILZ80ASM.Exceptions;
using System.IO;
using System.Text.RegularExpressions;

namespace AILZ80ASM.LineDetailItems
{
    public class LineDetailItemPreProcPragma : LineDetailItem
    {
        private static readonly string RegexPatternPragma = @"^\s*#PRAGMA\s+(?<name>[0-9a-zA-Z]+)\s*(?<argument>.*)$";
        private static readonly Regex CompiledRegexPatternPragma = new Regex(
            RegexPatternPragma,
            RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase
        );

        public override AsmList[] Lists
        {
            get
            {
                if (!AsmLoad.Share.IsOutputList)
                {
                    return new AsmList[] { };
                }

                return new[]
                {
                    AsmList.CreateLineItem(LineItem)
                };
            }
        }

        private LineDetailItemPreProcPragma(LineItem lineItem, AsmLoad asmLoad)
            : base(lineItem, asmLoad)
        {

        }

        public static LineDetailItemPreProcPragma Create(LineItem lineItem, AsmLoad asmLoad)
        {
            if (!lineItem.IsCollectOperationString)
            {
                return default(LineDetailItemPreProcPragma);
            }

            var matched = CompiledRegexPatternPragma.Match(lineItem.OperationString);
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

                return new LineDetailItemPreProcPragma(lineItem, asmLoad);
            }

            return default(LineDetailItemPreProcPragma);
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
