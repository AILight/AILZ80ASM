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
        internal InstructionRegister[] InstructionRegisters { get; set; }

        public void MakeDataSet(char[] splitChars, string[] brackets, InstructionRegister[] instructionRegisters)
        {
            var patternList = new List<string>();
            var instructionRegisterList = new List<InstructionRegister>();
            //var numberArgumentPattern = "\\w\\d\\s\\-" + string.Concat("!~*/%+<>=&^|?:$_".ToArray().Select(m => Regex.Escape(m.ToString())));

            foreach (var mnemonic in Mnemonics)
            {
                var result = "^";
                var tmpMnemonic = mnemonic;


                /*
                // 括弧の除外を設定
                var leftExclusionBranketPattern = "";
                var rightExclusionBranketPattern = "";
                foreach (var bracket in brackets)
                {
#if DEBUG
                    if (bracket.Length != 2)
                    {
                        throw new InvalidOperationException("Brankeの文字長は2です");
                    }
#endif
                    if (tmpMnemonic.IndexOf(bracket[0]) == -1 || tmpMnemonic.IndexOf(bracket[1]) == -1)
                    {
                        leftExclusionBranketPattern += $"[^{Regex.Escape(bracket[0].ToString())}]";
                        rightExclusionBranketPattern += $"[^{Regex.Escape(bracket[1].ToString())}]";
                    }
                }
                */


                do
                {
                    var index = tmpMnemonic.IndexOfAny(splitChars);
                    // 文字列を切り出し
                    var target = tmpMnemonic.Substring(0, index == -1 ? tmpMnemonic.Length : index);
                    var searchRegister = target;

                    // 括弧検索
                    var leftBranketPattern = "";
                    var rightBranketPattern = "";
                    foreach (var bracket in brackets)
                    {
#if DEBUG
                        if (bracket.Length != 2)
                        {
                            throw new InvalidOperationException("Brankeの文字長は2です");
                        }
#endif

                        if (searchRegister.StartsWith(bracket[0]))
                        {
                            searchRegister = searchRegister.Substring(1);
                            leftBranketPattern = $"{Regex.Escape(bracket[0].ToString())}\\s*";
                        }

                        if (searchRegister.EndsWith(bracket[1]))
                        {
                            searchRegister = searchRegister.Substring(0, searchRegister.Length - 1);
                            rightBranketPattern = $"\\s*{Regex.Escape(bracket[1].ToString())}";
                        }
                    }

                    // 検索文字列作成
                    var instructionRegister = instructionRegisters.FirstOrDefault(m => m.MnemonicRegisterName == searchRegister);
                    if (instructionRegister == default)
                    {
                        result += $"{leftBranketPattern}{Regex.Escape(searchRegister)}{rightBranketPattern}";
                    }
                    else
                    {
                        instructionRegisterList.Add(instructionRegister);

                        var matchPattern = instructionRegister.InstructionRegisterMode switch
                        {
                            InstructionRegisterModeEnum.Register => string.Join('|', instructionRegister.InstructionRegisterItems.Select(m => m.RegisterName)),
                            _ => $"[^" + string.Concat(splitChars.Where(m => m != ' ')) + "]+"
                            //_ => $"([{numberArgumentPattern}][{numberArgumentPattern}\\(\\)]*)"
                        };
                        var registerMatchPattern = $"(?<{instructionRegister.MnemonicRegisterName}>{matchPattern})";
                        result += $"{leftBranketPattern}{registerMatchPattern}{rightBranketPattern}";
                    }

                    // 区切り文字の調整
                    if (index != -1)
                    {
                        var tmpSplitCahr = tmpMnemonic[index];
                        var matchPattern = tmpSplitCahr switch
                        {
                            ' ' => $"\\s+",
                            _   => $"\\s*{Regex.Escape(tmpSplitCahr.ToString())}\\s*"
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
            InstructionRegisters = instructionRegisterList.Distinct().ToArray();
        }


        public Match Match(string target)
        {
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
    }
}
