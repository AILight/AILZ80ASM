using AILZ80ASM.AILight;
using AILZ80ASM.Assembler;
using AILZ80ASM.Exceptions;
using System.IO;
using System.Runtime.CompilerServices;
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
                var name = matched.Groups["name"].Value.ToUpper();
                var argument = matched.Groups["argument"].Value;

                switch (name)
                {
                    case "ONCE":
                        asmLoad.AddPragmaOnceFileInfo(lineItem.FileInfo);
                        break;
                    case "SET":
                        var item = ArgumentParse(argument);
                        var setName = item.Name.ToUpper();
                        var setValue = item.Value;
                        switch (setName)
                        {
                            case "LOADNAME":
                                if (AIMath.TryParse(setValue, asmLoad, out var resultValue))
                                {
                                    if (asmLoad.Share.LoadName != default)
                                    {
                                        asmLoad.AddError(new ErrorLineItem(lineItem, Error.ErrorCodeEnum.W8001, setName, setValue, setValue));
                                    }
                                    asmLoad.Share.LoadName = setValue;
                                }
                                else
                                {
                                    throw new ErrorAssembleException(Error.ErrorCodeEnum.E6203, setValue);
                                }
                                break;
                            default:
                                throw new ErrorAssembleException(Error.ErrorCodeEnum.E6202, setName);
                        }
                        break;
                    default:
                        throw new ErrorAssembleException(Error.ErrorCodeEnum.E6201, name);
                }

                return new LineDetailItemPreProcPragma(lineItem, asmLoad);
            }

            return default(LineDetailItemPreProcPragma);
        }

        private static (string Name, string Value) ArgumentParse(string target)
        {
            var index = target.IndexOf(" ");
            if (index == -1)
            {
                return ("", "");
            }
            var name = target.Substring(0, index).Trim();
            var value = target.Substring(index).Trim();

            return (name, value);
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
