using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AILZ80ASM.Instructions
{
    public class ISA
    {
        public enum EndiannessEnum
        {
            LittleEndian,
            BigEndian,
        }

        protected InstructionSet InstructionSet { get; set; }
        protected string InternalInstruction { get; set; }
        protected Match PreAssembleMatched { get; set; }
        protected InstructionItem PreAssembleMatchedInstructionItem { get; set; }
        protected string[] AssembledOPCodes { get; set; }


        public string Instruction { get; set; }
        public EndiannessEnum Endianness { get; protected set; } = EndiannessEnum.LittleEndian;
        public InstructionItem InstructionItem => this.PreAssembleMatchedInstructionItem;
        public int OPCodeLength => AssembledOPCodes == default ? 0 : AssembledOPCodes.Length;

        public ISA()
        {
        }

        /// <summary>
        /// レジスター名と一致するか？
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool IsMatchRegisterName(string name)
        {
            return InstructionSet.RegisterNames.Where(m => string.Compare(m, name) == 0).Any();
        }

        /// <summary>
        /// プレアセンブルを行う
        /// </summary>
        /// <returns></returns>
        public virtual bool PreAssemble()
        {
            InternalInstruction = Instruction;
            foreach (var item in InstructionSet.InstructionItems)
            {
                var matched = item.Match(InternalInstruction);
                if (matched.Success)
                {
                    var success = true;

                    // 除外キーワードを調査
                    foreach (var key in matched.Groups.Keys)
                    {
                        var instructionRegister = this.InstructionSet.InstructionRegisters.FirstOrDefault(m => m.MnemonicRegisterName == key);
                        if (instructionRegister != default)
                        {
                            var value = matched.Groups[key].Value;
                            if (instructionRegister.ExclusionItems != default && instructionRegister.ExclusionItems.Any(m => string.Compare(m, value, true) == 0))
                            {
                                success = false;
                                break;
                            }

                            switch (instructionRegister.InstructionRegisterMode)
                            {
                                case InstructionRegister.InstructionRegisterModeEnum.Register:
                                    // 何もしない
                                    break;
                                case InstructionRegister.InstructionRegisterModeEnum.RelativeAddress8Bit:
                                case InstructionRegister.InstructionRegisterModeEnum.Value3Bit:
                                case InstructionRegister.InstructionRegisterModeEnum.Value8Bit:
                                case InstructionRegister.InstructionRegisterModeEnum.Value16Bit:
                                    // Bracketsで囲まれていたら値ではない
                                    foreach (var bracket in this.InstructionSet.Brackets)
                                    {
                                        if (value.StartsWith(bracket[0]) && value.EndsWith(bracket[1]))
                                        {
                                            success = false;
                                            break;
                                        }
                                    }
                                    break;
                                default:
                                    throw new InvalidOperationException();
                            }
                        }
                    }

                    if (success)
                    {
                        PreAssembleMatched = matched;
                        PreAssembleMatchedInstructionItem = item;
                        AssembledOPCodes = item.OPCode.Select(m => new string(m)).ToArray();
                        return true;
                    }
                }
            }
            PreAssembleMatched = default;
            PreAssembleMatchedInstructionItem = default;
            AssembledOPCodes = default;

            return false;
        }

        public virtual void Assemble(LineDetailExpansionItemOperation lineDetailExpansionItemOperation, AsmLoad asmLoad)
        {
            if (PreAssembleMatched == default)
            {
                throw new ArgumentNullException(nameof(PreAssembleMatched));
            }

            if (PreAssembleMatchedInstructionItem == default)
            {
                throw new ArgumentNullException(nameof(PreAssembleMatchedInstructionItem));
            }

            var replaceDic = new Dictionary<string, string>();

            foreach (var instructionRegister in PreAssembleMatchedInstructionItem.InstructionRegisters)
            {
                var value = PreAssembleMatched.Groups[instructionRegister.MnemonicRegisterName].Value;
                switch (instructionRegister.InstructionRegisterMode)
                {
                    case InstructionRegister.InstructionRegisterModeEnum.Register:
                        {
                            var instructionRegisterItem = instructionRegister.InstructionRegisterItems.First(m => string.Compare(m.RegisterName, value, true) == 0);
                            replaceDic.Add(instructionRegister.MnemonicBitName, instructionRegisterItem.BitCode);
                        }
                        break;
                    case InstructionRegister.InstructionRegisterModeEnum.RelativeAddress8Bit:
                        {
                            var tmpValue16 = AIMath.ConvertTo<UInt16>(value, lineDetailExpansionItemOperation, asmLoad);
                            var offsetAddress = tmpValue16 - lineDetailExpansionItemOperation.Address.Program - 2;
                            if (offsetAddress < SByte.MinValue || offsetAddress > SByte.MaxValue)
                            {
                                throw new ErrorAssembleException(Error.ErrorCodeEnum.E0003, $"指定された値は、{offsetAddress}でした。");
                            }
                            var e8 = ConvertTo2BaseString(offsetAddress, 8);
                            replaceDic.Add(instructionRegister.MnemonicBitName, e8);
                        }
                        break;
                    case InstructionRegister.InstructionRegisterModeEnum.Value3Bit:
                        {
                            var tmpValue3 = AIMath.ConvertTo<byte>(value, lineDetailExpansionItemOperation, asmLoad);
                            var value3 = ConvertTo2BaseString(tmpValue3, 3);
                            replaceDic.Add(instructionRegister.MnemonicBitName, value3);
                        }
                        break;
                    case InstructionRegister.InstructionRegisterModeEnum.Value8Bit:
                        {
                            var tmpValue8 = AIMath.ConvertTo<byte>(value, lineDetailExpansionItemOperation, asmLoad);
                            var value8 = ConvertTo2BaseString(tmpValue8, 8);
                            replaceDic.Add(instructionRegister.MnemonicBitName, value8);
                        }
                        break;
                    case InstructionRegister.InstructionRegisterModeEnum.Value16Bit:
                        {
                            var tmpValue16 = AIMath.ConvertTo<UInt16>(value, lineDetailExpansionItemOperation, asmLoad);
                            var tmpValue16String = ConvertTo2BaseString(tmpValue16, 16);
                            var value16 = new[] { "", "" };
                            var mnemonicBitNames = instructionRegister.MnemonicBitName.Split(",");
                            replaceDic.Add(mnemonicBitNames[0], tmpValue16String.Substring(0, 8));
                            replaceDic.Add(mnemonicBitNames[1], tmpValue16String.Substring(8));
                        }
                        break;
                    default:
                        break;
                }
            }

            foreach (var index in Enumerable.Range(0, AssembledOPCodes.Length))
            {
                foreach (var item in replaceDic)
                {
                    AssembledOPCodes[index] = AssembledOPCodes[index].Replace(item.Key, item.Value);
                }
            }
        }

        public virtual byte[] ToBin()
        {
            return AssembledOPCodes.Select(m => Convert.ToByte(m, 2)).ToArray();
        }

        private static string ConvertTo2BaseString(int value, int length)
        {
            var returnValue = Convert.ToString(value, 2).PadLeft(length, '0'); ;
            var overString = returnValue.Substring(0, returnValue.Length - length);
            if ((value > 0 && overString.Contains("1")) ||
                (value < 0 && overString.Contains("0")))
            {
                throw new ErrorAssembleException(Error.ErrorCodeEnum.E0002, $"{value:x}");
            }

            return returnValue.Substring(overString.Length);
        }
    }
}
