using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static AILZ80ASM.Instructions.InstructionRegister;

namespace AILZ80ASM.Instructions
{
    public class InstructionItem
    {
        public string[] Mnemonics { get; set; }
        public string[] OPCode { get; set; }
        public int M { get; set; }
        public int T { get; set; }
        public bool AccumulatorExtra { get; set; }
        public bool UnDocumented { get; set; }
        private string[] RegexPatterns { get; set; }

        public void MakeDataSet(char[] splitChars, InstructionRegister[] instructionRegisters)
        {
            var patternList = new List<string>();
            foreach (var mnemonic in Mnemonics)
            {
                var result = "^";
                var tmpMnemonic = mnemonic;
                do
                {
                    var index = tmpMnemonic.IndexOfAny(splitChars);
                    // 文字列を切り出し
                    var target = tmpMnemonic.Substring(0, index == -1 ? tmpMnemonic.Length : index);

                    // 検索文字列作成
                    var instructionRegister = instructionRegisters.FirstOrDefault(m => m.MnemonicRegisterName == target);
                    if (instructionRegister == default)
                    {
                        result += target;
                    }
                    else
                    {
                        var matchPattern = instructionRegister.InstructionRegisterMode switch
                        {
                            InstructionRegisterModeEnum.Register => string.Join('|', instructionRegister.InstructionRegisterItems.Select(m => m.RegisterName)),
                            _ => $"[^" + string.Concat(splitChars.Where(m => m != ' ')) + "]+"
                        };
                        result += $"(?<{instructionRegister.MnemonicRegisterName}>{matchPattern})";
                    }

                    // 区切り文字の調整
                    if (index != -1)
                    {
                        var tmpSplitCahr = tmpMnemonic[index];
                        var matchPattern = tmpSplitCahr switch
                        {
                            ' ' => $"\\s+",
                            _   => $"\\s*{tmpSplitCahr}\\s*"
                        };
                        result += matchPattern;

                        tmpMnemonic = tmpMnemonic.Substring(index + 1);
                    }
                    else
                    {
                        tmpMnemonic = "";
                    }
                }
                while (!string.IsNullOrEmpty(tmpMnemonic));
                result += "$";

                patternList.Add(result);
            }
            RegexPatterns = patternList.ToArray();
        }


        public Match Match(string target)
        {
            if (RegexPatterns.Length == 0)
            {
                MakeRegexPatterns();
            }

            var mateched = default(Match);

            foreach (var regexPattern in RegexPatterns)
            {
                mateched = Regex.Match(target, regexPattern, RegexOptions.Singleline | RegexOptions.IgnoreCase);
                if (mateched.Success)
                {
                    break;
                }
            }

            return mateched;
        }

        private void MakeRegexPatterns()
        {
        }

        private static string MakeRegexPattern(string value)
        {
            /*
            var terms = new List<string>();
            var tmpValue = value.Trim();

            // 数値と演算子に分解する
            while (!string.IsNullOrEmpty(tmpValue))
            {
                var matched = Regex.Match(tmpValue, RegexPatternOperation, RegexOptions.Singleline | RegexOptions.IgnoreCase);
                if (matched.Success)
                {
                    terms.Add(matched.Value);
                    tmpValue = tmpValue.Substring(matched.Length).TrimStart();
                }
                else
                {
                    throw new Exception("演算に使えない文字が検出されました。");
                }
            }
            */
            return "";
        }


    }
}
