using AILZ80ASM.Assembler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AILZ80ASM.InstructionSet
{
    public class InstructionItem
    {
        public string[] Mnemonics { get; set; }
        public string[] OPCode { get; set; }
        public int M { get; set; }
        public int T { get; set; }
        //public bool AccumulatorExtra { get; set; }
        public bool UnDocumented { get; set; }
        public Error.ErrorCodeEnum? ErrorCode { get; set; }

        private Regex[] RegexPatterns { get; set; }
        internal Dictionary<string, InstructionRegister> InstructionRegisterDic { get; set; } = new Dictionary<string, InstructionRegister>();
        internal Dictionary<string, InstructionRegister> InstructionValueDic { get; set; } = new Dictionary<string, InstructionRegister>();

        public string[] MakeDataSet(char[] splitChars, InstructionRegister[] instructionRegisters)
        {
            var patternList = new List<Regex>();
            var instructionNameList = new List<string>();

            //var instructionRegisterList = new List<InstructionRegister>();

            foreach (var mnemonic in Mnemonics)
            {
                var result = "^";
                var mnemonicName = "";
                var startIndex = 0;
                var argumentIndex = 1;
                while (startIndex < mnemonic.Length)
                {
                    var splitIndex = mnemonic.IndexOfAny(splitChars, startIndex);
                    var length = splitIndex == -1 ? mnemonic.Length - startIndex : splitIndex - startIndex;
                    var target = mnemonic.Substring(startIndex, length);
                    startIndex += length + 1;
                    if (string.IsNullOrWhiteSpace(mnemonicName))
                    {
                        mnemonicName = target;
                    }

                    var splitChar = "";
                    if (splitIndex == -1)
                    {
                        splitChar = "";
                    }
                    else
                    {
                        splitChar = mnemonic[splitIndex].ToString();
                    }

                    if (!string.IsNullOrEmpty(target))
                    {
                        // 変数チェック
                        var instructionRegister = instructionRegisters.FirstOrDefault(m => m.MnemonicRegisterName == target);
                        if (instructionRegister != default)
                        {
                            switch (instructionRegister.InstructionRegisterMode)
                            {
                                case InstructionRegister.InstructionRegisterModeEnum.Register:
                                    target = string.Join('|', instructionRegister.InstructionRegisterItems.Select(m => Regex.Escape(m.RegisterName)));
                                    break;
                                case InstructionRegister.InstructionRegisterModeEnum.RelativeAddress8Bit:
                                case InstructionRegister.InstructionRegisterModeEnum.Value0Bit:
                                case InstructionRegister.InstructionRegisterModeEnum.Value3Bit:
                                case InstructionRegister.InstructionRegisterModeEnum.Value8Bit:
                                case InstructionRegister.InstructionRegisterModeEnum.Value8BitSigned:
                                case InstructionRegister.InstructionRegisterModeEnum.Value16Bit:
                                case InstructionRegister.InstructionRegisterModeEnum.InterruptModeValue:
                                case InstructionRegister.InstructionRegisterModeEnum.RestartValue:
                                    {
                                        var argumentName = $"${argumentIndex}";
                                        target = Regex.Escape(argumentName);
                                        argumentIndex++;

                                        if (!InstructionValueDic.ContainsKey(argumentName))
                                        {
                                            InstructionValueDic.Add(argumentName, instructionRegister);
                                        }
                                    }
                                    break;
                                default:
                                    throw new InvalidOperationException();
                            }
                            target = $"(?<{instructionRegister.MnemonicRegisterName}>{target})";
                            
                            if (!InstructionRegisterDic.ContainsKey(instructionRegister.MnemonicRegisterName))
                            {
                                InstructionRegisterDic.Add(instructionRegister.MnemonicRegisterName, instructionRegister);
                            }
                        }
                    }
                    result += target + Regex.Escape(splitChar);
                }
                result += "$";

                patternList.Add(new Regex(
                    result,
                    RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase
                ));

                if (!string.IsNullOrEmpty(mnemonicName))
                {
                    instructionNameList.Add(mnemonicName);
                }
                else
                {
#if DEBUG
                    throw new InvalidOperationException("命令名が取得できませんでした。");
#endif
                }
            }
            RegexPatterns = patternList.ToArray();

            return instructionNameList.ToArray();
        }


        public Match Match(string target)
        {
            var mateched = default(Match);

            foreach (var regexPattern in RegexPatterns)
            {
                mateched = regexPattern.Match(target);
                if (mateched.Success)
                {
                    break;
                }
            }

            return mateched;
        }
    }
}
