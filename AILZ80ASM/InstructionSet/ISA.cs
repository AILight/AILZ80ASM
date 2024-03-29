﻿using AILZ80ASM.AILight;
using AILZ80ASM.Assembler;
using AILZ80ASM.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AILZ80ASM.InstructionSet
{
    public class ISA
    {
        public enum EndiannessEnum
        {
            LittleEndian,
            BigEndian,
        }

        protected InstructionSet InstructionSet { get; private set; }
        public EndiannessEnum Endianness { get; protected set; } = EndiannessEnum.LittleEndian;
        private Dictionary<string, AssembleCacheResult> AssembleCacheResultDic { get; set; } = new Dictionary<string, AssembleCacheResult>();

        protected ISA(InstructionSet instructionSet)
        {
            InstructionSet = instructionSet;
        }

        /// <summary>
        /// レジスター名と一致するか？
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public bool IsMatchRegisterName(string target)
        {
            return InstructionSet.RegisterAndFlagNames.Where(m => string.Compare(m, target, true) == 0).Any();
        }

        /// <summary>
        /// 命令名と一致するか？
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public bool IsMatchInstructionName(string target)
        {
            return InstructionSet.InstructionNames.Where(m => string.Compare(m, target, true) == 0).Any();
        }


        public virtual AssembleParseResult ParseInstruction(string instuction)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// プレアセンブルを行う
        /// </summary>
        /// <returns></returns>
        public virtual AssembleResult PreAssemble(string instruction)
        {
            // 多段で判別する
            // 正規表現で命令を判別する
            // 判別不能時に、分解したもので再度処理を行う
            if (instruction.Length == 0)
            {
                return default;
            }

            var parseResult = ParseInstruction(instruction);
            if (AssembleCacheResultDic.TryGetValue(parseResult.Instruction, out var assembleCacheResult))
            {
                return new AssembleResult { InstructionItem = assembleCacheResult.InstructionItem, ParseResult = parseResult, PreAssembleMatched = assembleCacheResult.PreAssembleMatched };
            }
            else
            {
                if (InstructionSet.InstructionDic.TryGetValue(parseResult.InstructionForDic, out var instructionItem))
                {
                    foreach (var item in instructionItem)
                    {
                        var matched = item.Match(parseResult.Instruction);
                        if (matched.Success)
                        {
                            AssembleCacheResultDic.Add(parseResult.Instruction, new AssembleCacheResult { InstructionItem = item, PreAssembleMatched = matched });
                            return new AssembleResult { InstructionItem = item, ParseResult = parseResult, PreAssembleMatched = matched };
                        }
                    }
                }
            }
            return default;
        }

        public virtual void Assemble(AssembleResult assembleResult, AsmLoad asmLoad, AsmAddress asmAddress)
        {
            if (assembleResult == default)
            {
                throw new ArgumentNullException(nameof(assembleResult));
            }

            if (asmLoad == default)
            {
                throw new ArgumentNullException(nameof(asmLoad));
            }

            var replaceDic = new Dictionary<string, string>();

            foreach (var key in assembleResult.PreAssembleMatched.Groups.Keys)
            {
                var value = assembleResult.PreAssembleMatched.Groups[key].Value;
                if (assembleResult.InstructionItem.InstructionRegisterDic.ContainsKey(key) &&
                    assembleResult.InstructionItem.InstructionRegisterDic[key].InstructionRegisterMode == InstructionRegister.InstructionRegisterModeEnum.Register)
                {
                    var instructionRegister = assembleResult.InstructionItem.InstructionRegisterDic[key];
                    var instructionRegisterItem = instructionRegister.InstructionRegisterItems.First(m => string.Compare(m.RegisterName, value, true) == 0);
                    replaceDic.Add(instructionRegister.MnemonicBitName, instructionRegisterItem.BitCode);
                }
                else if (assembleResult.InstructionItem.InstructionValueDic.ContainsKey(value))
                {
                    var instructionRegister = assembleResult.InstructionItem.InstructionValueDic[value];
                    var stringValue = assembleResult.ParseResult.ArgumentDic[value];
                    switch (instructionRegister.InstructionRegisterMode)
                    {
                        case InstructionRegister.InstructionRegisterModeEnum.RelativeAddress8Bit:
                            {
                                var tmpValue16 = AIMath.Calculation(stringValue, asmLoad, asmAddress).ConvertTo<UInt16>();
                                var offsetAddress = tmpValue16 - asmAddress.Program - 2;
                                if (offsetAddress < SByte.MinValue || offsetAddress > SByte.MaxValue)
                                {
                                    throw new AssembleOutOfRangeException(instructionRegister.InstructionRegisterMode, offsetAddress, $"指定値: 0x{offsetAddress:x2}:{offsetAddress}");
                                }
                                var e8 = ConvertTo2BaseString(offsetAddress, 8);
                                replaceDic.Add(instructionRegister.MnemonicBitName, e8);
                            }
                            break;
                        case InstructionRegister.InstructionRegisterModeEnum.Value0Bit:
                            {
                                var tmpValue16 = AIMath.Calculation(stringValue, asmLoad, asmAddress).ConvertTo<UInt16>();
                                replaceDic.Add(instructionRegister.MnemonicBitName, "");
                                if (tmpValue16 != 0)
                                {
                                    throw new AssembleOutOfRangeException(instructionRegister.InstructionRegisterMode, tmpValue16, $"指定値: 0x{tmpValue16:x2}:{tmpValue16}です。");
                                }
                            }
                            break;
                        case InstructionRegister.InstructionRegisterModeEnum.Value3Bit:
                            {
                                var tmpValue16 = AIMath.Calculation(stringValue, asmLoad, asmAddress).ConvertTo<UInt16>();
                                var value3 = ConvertTo2BaseString(tmpValue16 /*& 0x07*/, 3);
                                replaceDic.Add(instructionRegister.MnemonicBitName, value3);
                                /* 3ビットのマスクは不要
                                if (tmpValue16 > 7)
                                {
                                    throw new AssembleOutOfRangeException(instructionRegister.InstructionRegisterMode, tmpValue16, $"指定値: 0x{tmpValue16:x2}:{tmpValue16}");
                                }
                                */
                            }
                            break;
                        case InstructionRegister.InstructionRegisterModeEnum.Value8Bit:
                            {
                                var result = AIMath.Calculation(stringValue, asmLoad, asmAddress);
                                var tmpValue = result.ConvertTo<int>();
                                if (tmpValue > 255 || tmpValue < -255)
                                {
                                    assembleResult.InnerAssembleException = new AssembleOutOfRangeException(instructionRegister.InstructionRegisterMode, tmpValue, $"指定値: 0x{tmpValue:x2}:{tmpValue}");
                                }

                                var tmpValue16 = result.ConvertTo<UInt16>();
                                var value8 = ConvertTo2BaseString(tmpValue16 & 0xFF, 8);
                                replaceDic.Add(instructionRegister.MnemonicBitName, value8);
                            }
                            break;
                        case InstructionRegister.InstructionRegisterModeEnum.Value8BitSigned:
                            {
                                var tmpValue = AIMath.Calculation(stringValue, asmLoad, asmAddress).ConvertTo<int>();
                                if (tmpValue > 127 || tmpValue < -128)
                                {
                                    assembleResult.InnerAssembleException = new AssembleOutOfRangeException(instructionRegister.InstructionRegisterMode, tmpValue, $"指定値: 0x{tmpValue:x2}:{tmpValue}");
                                }

                                var value8 = ConvertTo2BaseString(tmpValue & 0xFF, 8);
                                replaceDic.Add(instructionRegister.MnemonicBitName, value8);
                            }
                            break;
                        case InstructionRegister.InstructionRegisterModeEnum.Value16Bit:
                            {
                                var result = AIMath.Calculation(stringValue, asmLoad, asmAddress);
                                var tmpValue = result.ConvertTo<int>();
                                if (tmpValue > 65535 || tmpValue < -65535)
                                {
                                    assembleResult.InnerAssembleException = new AssembleOutOfRangeException(instructionRegister.InstructionRegisterMode, tmpValue, $"指定値: 0x{tmpValue:x4}:{tmpValue}");
                                }

                                var tmpValue32 = result.ConvertTo<UInt32>();
                                var tmpValue16String = ConvertTo2BaseString((int)(tmpValue32 & 0xFFFF), 16);
                                var value16 = new[] { "", "" };
                                var mnemonicBitNames = instructionRegister.MnemonicBitName.Split(",");
                                replaceDic.Add(mnemonicBitNames[0], tmpValue16String.Substring(0, 8));
                                replaceDic.Add(mnemonicBitNames[1], tmpValue16String.Substring(8));
                            }
                            break;
                        case InstructionRegister.InstructionRegisterModeEnum.InterruptModeValue:
                            {
                                var tmpValue16 = AIMath.Calculation(stringValue, asmLoad, asmAddress).ConvertTo<UInt16>();
                                var value8 = tmpValue16 switch
                                { 
                                    0 => "00",
                                    1 => "10",
                                    2 => "11",
                                    _ => throw new AssembleOutOfRangeException(instructionRegister.InstructionRegisterMode, tmpValue16, $"指定値: 0x{tmpValue16:x4}:{tmpValue16}")
                                };
                                replaceDic.Add(instructionRegister.MnemonicBitName, value8);
                            }
                            break;
                        case InstructionRegister.InstructionRegisterModeEnum.RestartValue:
                            {
                                var tmpValue16 = AIMath.Calculation(stringValue, asmLoad, asmAddress).ConvertTo<UInt16>();
                                var value8 = tmpValue16 switch
                                {
                                    0x00 => "000",
                                    0x08 => "001",
                                    0x10 => "010",
                                    0x18 => "011",
                                    0x20 => "100",
                                    0x28 => "101",
                                    0x30 => "110",
                                    0x38 => "111",
                                    _ => throw new AssembleOutOfRangeException(instructionRegister.InstructionRegisterMode, tmpValue16, $"指定値: 0x{tmpValue16:x4}:{tmpValue16}")
                                };
                                replaceDic.Add(instructionRegister.MnemonicBitName, value8);
                            }
                            break;
                        default:
                            throw new InvalidOperationException();
                    }
                }
            }

            assembleResult.AssembledOPCodes = assembleResult.InstructionItem.OPCode.Select(m =>
            {
                foreach (var item in replaceDic)
                {
                    if (!string.IsNullOrEmpty(item.Key))
                    {
                        m = m.Replace(item.Key, item.Value);
                    }
                }
                return m;
            }).ToArray();
        }

        public virtual byte[] ToBin(AssembleResult assembleResult)
        {
            return assembleResult?.AssembledOPCodes?.Select(m => Convert.ToByte(m, 2)).ToArray();
        }

        private static string ConvertTo2BaseString(int value, int length)
        {
            var returnValue = Convert.ToString(value, 2).PadLeft(length, '0');
            var overString = returnValue.Substring(0, returnValue.Length - length);
            if ((value > 0 && overString.Contains("1")) ||
                (value < 0 && overString.Contains("0")))
            {
                throw new ErrorAssembleException(Error.ErrorCodeEnum.E0002, $"{length:0}", $"0x{value:x2}:{value}");
            }

            return returnValue.Substring(overString.Length);
        }
    }
}
