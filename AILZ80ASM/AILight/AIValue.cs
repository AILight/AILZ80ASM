﻿using AILZ80ASM.Assembler;
using AILZ80ASM.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace AILZ80ASM.AILight
{
    public class AIValue
    {
        private enum MacroValueEnum
        {
            None,
            High,
            Low,
            Text,
            Exists,
        }

        [Flags]
        public enum ValueTypeEnum
        {
            None,
            Int32 = 1,
            Bool = 2,
            Char = 4,
            String = 8,
            Bytes = 16,
            Operation = 32,
            Function = 64,
        }

        public enum OperationTypeEnum
        {
            None,
            LeftParenthesis,    // (
            RightParenthesis,   // )
            Plus,               // +
            Minus,              // -
            Negation,           // !
            BitwiseComplement,  // ~
            Multiplication,     // *
            Division,           // /
            Remainder,          // %
            LeftShift,          // <<
            RightShift,         // >>
            Less,               // <
            Greater,            // >
            LessEqual,          // <=
            GreaterEqual,       // >=
            Equal,              // ==
            NotEqual,           // !=
            And,                // &
            Xor,                // ^
            Or,                 // |
            ConditionalAnd,     // &&
            ConditionalOr,      // ||
            Ternary_Colon,      // :
            Ternary_Question,   // ?
        }

        public enum ArgumentTypeEnum
        {
            None,
            SingleArgument,     // 引数 1
            DoubleArgument,     // 引数 2
            TripleArgument,     // 引数 3
        }

        private static readonly Dictionary<string, OperationTypeEnum> OperationTypeTable = new()
        {
            ["("] =  OperationTypeEnum.LeftParenthesis,
            [")"] =  OperationTypeEnum.RightParenthesis,
            ["!"] =  OperationTypeEnum.Negation,
            ["~"] =  OperationTypeEnum.BitwiseComplement,
            ["*"] =  OperationTypeEnum.Multiplication,
            ["/"] =  OperationTypeEnum.Division,
            ["%"] =  OperationTypeEnum.Remainder,
            ["+"] =  OperationTypeEnum.Plus,
            ["-"] =  OperationTypeEnum.Minus,
            ["<<"] = OperationTypeEnum.LeftShift,
            [">>"] = OperationTypeEnum.RightShift,
            ["<"] =  OperationTypeEnum.Less,
            [">"] =  OperationTypeEnum.Greater,
            ["<="] = OperationTypeEnum.LessEqual,
            [">="] = OperationTypeEnum.GreaterEqual,
            ["=="] = OperationTypeEnum.Equal,
            ["!="] = OperationTypeEnum.NotEqual,
            ["&"] =  OperationTypeEnum.And,
            ["^"] =  OperationTypeEnum.Xor,
            ["|"] =  OperationTypeEnum.Or,
            ["&&"] = OperationTypeEnum.ConditionalAnd,
            ["||"] = OperationTypeEnum.ConditionalOr,
            ["?"] =  OperationTypeEnum.Ternary_Question,
            [":"] =  OperationTypeEnum.Ternary_Colon,
        };

        private static readonly Dictionary<OperationTypeEnum, int> FormulaPriority = new()
        {
            [OperationTypeEnum.RightParenthesis] = 1,   // )
            [OperationTypeEnum.Negation] = 2,           // !
            [OperationTypeEnum.BitwiseComplement] = 2,  // ~ 単項演算子は別で処理する ["+"] = 2,  ["-"] = 2,
            [OperationTypeEnum.Multiplication] = 3,     // *
            [OperationTypeEnum.Division] = 3,           // /
            [OperationTypeEnum.Remainder] = 3,          // %
            [OperationTypeEnum.Plus] = 4,               // +
            [OperationTypeEnum.Minus] = 4,              // -
            [OperationTypeEnum.LeftShift] = 5,          // <<
            [OperationTypeEnum.RightShift] = 5,         // >>
            [OperationTypeEnum.Less] = 6,               // <
            [OperationTypeEnum.Greater] = 6,            // >
            [OperationTypeEnum.LessEqual] = 6,          // <=
            [OperationTypeEnum.GreaterEqual] = 6,       // >=
            [OperationTypeEnum.Equal] = 7,              // ==
            [OperationTypeEnum.NotEqual] = 7,           // !=
            [OperationTypeEnum.And] = 8,                // &
            [OperationTypeEnum.Xor] = 9,                // ^
            [OperationTypeEnum.Or] = 10,                // |
            [OperationTypeEnum.ConditionalAnd] = 11,    // &&
            [OperationTypeEnum.ConditionalOr] = 12,     // ||
            [OperationTypeEnum.Ternary_Question] = 14,  // ?
            [OperationTypeEnum.Ternary_Colon] = 13,     // :
            [OperationTypeEnum.LeftParenthesis] = 15,   // (
        };

        private static readonly Dictionary<OperationTypeEnum, ArgumentTypeEnum> OperationArgumentType = new()
        {
            [OperationTypeEnum.RightParenthesis] = ArgumentTypeEnum.None,               // )
            [OperationTypeEnum.Negation] = ArgumentTypeEnum.SingleArgument,             // !
            [OperationTypeEnum.BitwiseComplement] = ArgumentTypeEnum.SingleArgument,    // ~ 単項演算子は別で処理する ["+"] = 2,  ["-"] = 2,
            [OperationTypeEnum.Multiplication] = ArgumentTypeEnum.DoubleArgument,       // *
            [OperationTypeEnum.Division] = ArgumentTypeEnum.DoubleArgument,             // /
            [OperationTypeEnum.Remainder] = ArgumentTypeEnum.DoubleArgument,            // %
            [OperationTypeEnum.Plus] = ArgumentTypeEnum.DoubleArgument,                 // +
            [OperationTypeEnum.Minus] = ArgumentTypeEnum.DoubleArgument,                // -
            [OperationTypeEnum.LeftShift] = ArgumentTypeEnum.DoubleArgument,            // <<
            [OperationTypeEnum.RightShift] = ArgumentTypeEnum.DoubleArgument,           // >>
            [OperationTypeEnum.Less] = ArgumentTypeEnum.DoubleArgument,                 // <
            [OperationTypeEnum.Greater] = ArgumentTypeEnum.DoubleArgument,              // >
            [OperationTypeEnum.LessEqual] = ArgumentTypeEnum.DoubleArgument,            // <=
            [OperationTypeEnum.GreaterEqual] = ArgumentTypeEnum.DoubleArgument,         // >=
            [OperationTypeEnum.Equal] = ArgumentTypeEnum.DoubleArgument,                // ==
            [OperationTypeEnum.NotEqual] = ArgumentTypeEnum.DoubleArgument,             // !=
            [OperationTypeEnum.And] = ArgumentTypeEnum.DoubleArgument,                  // &
            [OperationTypeEnum.Xor] = ArgumentTypeEnum.DoubleArgument,                  // ^
            [OperationTypeEnum.Or] = ArgumentTypeEnum.DoubleArgument,                   // |
            [OperationTypeEnum.ConditionalAnd] = ArgumentTypeEnum.DoubleArgument,       // &&
            [OperationTypeEnum.ConditionalOr] = ArgumentTypeEnum.DoubleArgument,        // ||
            [OperationTypeEnum.Ternary_Question] = ArgumentTypeEnum.TripleArgument,     // ?
            [OperationTypeEnum.Ternary_Colon] = ArgumentTypeEnum.None,                  // :
            [OperationTypeEnum.LeftParenthesis] = ArgumentTypeEnum.None,                // (
        };
        private static readonly string RegexPatternHexadecimal_H = @"^(?<value>([0-9A-Fa-f_]+))H$";
        private static readonly string RegexPatternHexadecimal_X = @"^0x(?<value>([0-9A-Fa-f_]+))$";
        private static readonly string RegexPatternHexadecimal_D = @"^\$(?<value>([0-9A-Fa-f_]+))$";
        private static readonly string RegexPatternOctal_O = @"^(?<value>([0-7_]+))O$";
        private static readonly string RegexPatternBinaryNumber_B = @"^(?<value>([01_]+))B$";
        private static readonly string RegexPatternBinaryNumber_P = @"^%(?<value>([01_]+))$";
        //private static readonly string RegexPatternChar = @"^'(?<value>(.|\\.))'$";
        private static readonly string RegexPatternDigit = @"^(?<value>(\+|\-|)(\d+))$";


        private string Value { get; set; } = "";
        private int ValueInt32 { get; set; } = default(int);
        private bool ValueBool { get; set; } = default(bool);
        private string ValueString { get; set; } = default(string);
        private string ValueCharMap { get; set; } = default(string);
        private byte[] ValueBytes { get; set; } = default(byte[]);
        private OperationTypeEnum ValueOperation { get; set; } = OperationTypeEnum.None;

        /// <summary>
        /// 演算順位
        /// </summary>
        public int OperationPriority => FormulaPriority[ValueOperation];
        public ArgumentTypeEnum ArgumentType => OperationArgumentType[ValueOperation];
        public ValueTypeEnum ValueType { get; set; } = ValueTypeEnum.None;

        public AIValue(string value)
            : this(value, ValueTypeEnum.None)
        {
            ValueType = ValueTypeEnum.None;
            Value = value;
        }

        public AIValue(string value, ValueTypeEnum valueType)
        {
            ValueType = valueType;
            Value = value;
            if (valueType.HasFlag(ValueTypeEnum.Int32))
            {
                ValueInt32 = Convert.ToInt32(value);
            }
            if (valueType.HasFlag(ValueTypeEnum.Operation))
            {
                ValueOperation = OperationTypeTable[value];
            }
            if (valueType.HasFlag(ValueTypeEnum.Char) ||
                valueType.HasFlag(ValueTypeEnum.String))
            {
                ValueString = value;
            }

            switch (valueType)
            {
                case ValueTypeEnum.None:
                case ValueTypeEnum.Int32:
                case ValueTypeEnum.Operation:
                case ValueTypeEnum.String:
                case ValueTypeEnum.Function:
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        public AIValue(AIValue value1, AIValue value2)
            : this(value1.Value + value2.Value)
        { 
        }

        public T ConvertTo<T>()
            where T : struct
        {
            try
            {
                if (typeof(T) == typeof(int))
                {
                    if (ValueType.HasFlag(ValueTypeEnum.Int32))
                    {
                        return (T)(object)ValueInt32;
                    }
                    throw new InvalidOperationException();
                }
                else if (typeof(T) == typeof(UInt32))
                {
                    if (ValueType.HasFlag(ValueTypeEnum.Int32))
                    {
                        if (ValueInt32 < 0)
                        {
                            return (T)(object)(UInt32)(UInt32.MaxValue + ValueInt32 + 1);
                        }
                        else
                        {
                            return (T)(object)(UInt32)(ValueInt32 & UInt32.MaxValue);
                        }
                    }
                    throw new InvalidOperationException();
                }
                else if (typeof(T) == typeof(UInt16))
                {
                    if (ValueType.HasFlag(ValueTypeEnum.Int32))
                    {
                        if (ValueInt32 < 0)
                        {
                            return (T)(object)(UInt16)(UInt16.MaxValue + ValueInt32 + 1);
                        }
                        else
                        {
                            return (T)(object)(UInt16)(ValueInt32 & UInt16.MaxValue);
                        }
                    }
                    throw new InvalidOperationException();
                }
                else if (typeof(T) == typeof(byte))
                {
                    if (ValueType.HasFlag(ValueTypeEnum.Int32))
                    {
                        if (ValueInt32 < 0)
                        {
                            return (T)(object)(byte)(byte.MaxValue + ValueInt32 + 1);
                        }
                        else
                        {
                            return (T)(object)(byte)(ValueInt32 & byte.MaxValue);
                        }
                    }
                    throw new InvalidOperationException();
                }
                else if (typeof(T) == typeof(bool))
                {
                    if (ValueType.HasFlag(ValueTypeEnum.Bool))
                    {
                        return (T)(object)ValueBool;
                    }
                    throw new InvalidOperationException();
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
            catch (ErrorAssembleException)
            {
                throw;
            }
            catch (InvalidAIValueException ex)
            {
                throw new ErrorAssembleException(Error.ErrorCodeEnum.E0004, $"演算対象：{Value} エラー:{ex.Message}");
            }
            catch (Exception)
            {
                throw new ErrorAssembleException(Error.ErrorCodeEnum.E0004, $"演算対象：{Value}");
            }
        }

        /// <summary>
        /// Bool型
        /// </summary>
        /// <param name="value"></param>
        public AIValue(bool value)
        {
            ValueType = ValueTypeEnum.Bool;
            Value = value.ToString();
            ValueBool = value;
        }

        /// <summary>
        /// Int型
        /// </summary>
        /// <param name="value"></param>
        public AIValue(int value)
        {
            ValueType = ValueTypeEnum.Int32;
            Value = value.ToString();
            ValueInt32 = value;
        }

        /// <summary>
        /// オペレーションかを判断する
        /// </summary>
        /// <returns></returns>
        public bool IsOperation()
        {
            return ValueType.HasFlag(ValueTypeEnum.Operation);
        }

        /// <summary>
        /// オペレーションを判断する
        /// </summary>
        /// <param name="operationType"></param>
        /// <returns></returns>
        public bool IsOperation(OperationTypeEnum operationType)
        {
            return ValueType.HasFlag(ValueTypeEnum.Operation) && ValueOperation == operationType;
        }

        /// <summary>
        /// 数値の前に付けられる記号　+, -, %
        /// </summary>
        /// <returns></returns>
        public bool IsOperationSignOrBNumber()
        {
            return ValueType.HasFlag(ValueTypeEnum.Operation) &&
                   (ValueOperation == OperationTypeEnum.Plus || 
                    ValueOperation == OperationTypeEnum.Minus || 
                    ValueOperation == OperationTypeEnum.Remainder);
        }

        /// <summary>
        /// 値を確定する
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public void SetValue(AsmLoad asmLoad, AsmAddress? asmAddress)
        {
            if (TryParse16(Value, out var result16))
            {
                // 16進数
                try
                {
                    ValueType = ValueTypeEnum.Int32;
                    ValueInt32 = Convert.ToInt32(result16, 16);
                }
                catch
                {
                    throw new InvalidAIValueException($"数値に変換できませんでした。[{Value}]");
                }
            }
            else if (TryParse2(Value, out var result2))
            {
                // 2進数
                try
                {
                    ValueType = ValueTypeEnum.Int32;
                    ValueInt32 = Convert.ToInt32(result2, 2);
                }
                catch
                {
                    throw new InvalidAIValueException($"数値に変換できませんでした。[{Value}]");
                }
            }
            else if (TryParse8(Value, out var result8))
            {
                // 8進数
                try
                {
                    ValueType = ValueTypeEnum.Int32;
                    ValueInt32 = Convert.ToInt32(result8, 8);
                }
                catch
                {
                    throw new InvalidAIValueException($"数値に変換できませんでした。[{Value}]");
                }
            }
            else if (TryParse10(Value, out var result10))
            {
                // 10進数
                try
                {
                    ValueType = ValueTypeEnum.Int32;
                    ValueInt32 = Convert.ToInt32(result10, 10);
                }
                catch
                {
                    throw new InvalidAIValueException($"数値に変換できませんでした。]{Value}]");
                }
            }
            else if (AIString.TryParseCharMap(Value, asmLoad, out var charMap, out var resultString))
            {
                ValueCharMap = charMap;
                ValueString = resultString;
                ValueType |= ValueTypeEnum.Bytes;
                if (AIString.IsChar(Value, asmLoad))
                {
                    ValueType = ValueTypeEnum.Char;
                    ValueBytes = AIString.GetBytesByChar(Value, asmLoad);
                }
                else
                {
                    ValueType = ValueTypeEnum.String;
                    ValueBytes = AIString.GetBytesByString(Value, asmLoad);
                }

                if (ValueBytes.Length > 0 && ValueBytes.Length <= 2)
                {
                    ValueType |= ValueTypeEnum.Int32;
                    ValueInt32 = ValueBytes[0] + (ValueBytes.Length > 1 ? ValueBytes[1] * 256 : 0);
                }
            }
            else if (Value == "$")
            {
                if (!asmAddress.HasValue)
                {
                    throw new ArgumentNullException(nameof(asmAddress));
                }
                // プログラム・ロケーションカウンター
                ValueType = ValueTypeEnum.Int32;
                ValueInt32 = (int)asmAddress.Value.Program;
            }
            else if (Value == "$$")
            {
                if (!asmAddress.HasValue)
                {
                    throw new ArgumentNullException(nameof(asmAddress));
                }

                // PreAssemble中のアウトプット・ロケーションカウンターは参照できない
                if (asmLoad != default && asmLoad.Share.AsmStep == AsmLoadShare.AsmStepEnum.PreAssemble)
                {
                    throw new InvalidAIValueException("出力アドレスに影響する場所では$$は使えません。");
                }

                // アウトプット・ロケーションカウンター
                ValueType = ValueTypeEnum.Int32;
                ValueInt32 = (int)asmAddress.Value.Output;
            }
            else if (string.Compare(Value, "#TRUE", true) == 0)
            {
                ValueType = ValueTypeEnum.Bool;
                ValueBool = true;
            }
            else if (string.Compare(Value, "#FALSE", true) == 0)
            {
                ValueType = ValueTypeEnum.Bool;
                ValueBool = false;
            }
            else
            {
                if (asmLoad == default)
                {
                    throw new ArgumentNullException(nameof(asmLoad));
                }

                var startIndex = Value.IndexOf('(');
                if (startIndex != -1)
                {
                    // Function
                    var functionName = Value.Substring(0, startIndex).Trim();
                    var function = asmLoad.FindFunction(functionName);

                    var lastIndex = Value.LastIndexOf(')');
                    if (function == default || lastIndex == -1)
                    {
                        throw new InvalidAIValueException($"Functionが見つかりませんでした。{functionName}");
                    }
                    var arguments = AIName.ParseArguments(Value.Substring(startIndex + 1, lastIndex - startIndex - 1));

                    var calcValue = function.Calculation2(arguments, asmLoad, asmAddress);

                    ValueType = calcValue.ValueType;
                    ValueInt32 = calcValue.ValueInt32;
                    ValueBool = calcValue.ValueBool;
                    ValueString = calcValue.ValueString;
                    ValueOperation = calcValue.ValueOperation;
                }
                else
                {
                    // Label
                    var macroValue = MacroValueEnum.None;
                    var tmpLabel = Value;
                    var optionIndex = Value.IndexOf(".@");
                    if (optionIndex > 0)
                    {
                        var option = Value.Substring(optionIndex);
                        if (string.Compare(option, ".@H", true) == 0 ||
                            string.Compare(option, ".@HIGH", true) == 0)
                        {
                            tmpLabel = Value.Substring(0, optionIndex);
                            macroValue = MacroValueEnum.High;
                        }
                        else if (string.Compare(option, ".@L", true) == 0 ||
                                 string.Compare(option, ".@LOW", true) == 0)
                        {
                            tmpLabel = Value.Substring(0, optionIndex);
                            macroValue = MacroValueEnum.Low;
                        }
                        else if (string.Compare(option, ".@T", true) == 0 ||
                                 string.Compare(option, ".@TEXT", true) == 0)
                        {
                            tmpLabel = Value.Substring(0, optionIndex);
                            macroValue = MacroValueEnum.Text;
                        }
                        else if (string.Compare(option, ".@E", true) == 0 ||
                                 string.Compare(option, ".@EXISTS", true) == 0)
                        {
                            tmpLabel = Value.Substring(0, optionIndex);
                            macroValue = MacroValueEnum.Exists;
                        }
                    }

                    var label = asmLoad.FindLabel(tmpLabel);

                    if (macroValue == MacroValueEnum.Exists)
                    {
                        // ラベルの存在チェックなので、それに合わせて応答を返す
                        ValueType = ValueTypeEnum.Bool;
                        ValueBool = (label != default);
                    }
                    else
                    {
                        if (label == default)
                        {
                            throw new InvalidAIValueException($"未定義のラベルが指定されています。{Value}");
                        }
                        else
                        {
                            if (macroValue != MacroValueEnum.Text)
                            {
                                label.Calculation();
                            }
                        }

                        var value = label.Value2;

                        switch (macroValue)
                        {
                            case MacroValueEnum.High:
                                ValueType = ValueTypeEnum.Int32;
                                ValueInt32 = value.ValueInt32 / 256;
                                break;
                            case MacroValueEnum.Low:
                                ValueType = ValueTypeEnum.Int32;
                                ValueInt32 = value.ValueInt32 % 256;
                                break;
                            case MacroValueEnum.Text:
                                ValueType = ValueTypeEnum.String;
                                ValueString = label.ValueString;
                                break;
                            default:
                                ValueType = value.ValueType;
                                ValueInt32 = value.ValueInt32;
                                ValueBool = value.ValueBool;
                                ValueString = value.ValueString;
                                ValueOperation = value.ValueOperation;
                                break;
                        }
                    }
                }

            } 
        }

        /// <summary>
        /// 数値項目の判断
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsNumber(string value)
        {
            if (AIValue.TryParse16(value, out var _))
            {
                return true;
            }

            if (AIValue.TryParse2(value, out var _))
            {
                return true;
            }

            if (AIValue.TryParse8(value, out var _))
            {
                return true;
            }

            if (AIValue.TryParse10(value, out var _))
            {
                return true;
            }

            return false;
        }


        /// <summary>
        /// 値を計算する
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="firstValue"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static AIValue Calculation(AIValue operation, AIValue firstValue)
        {
            if (operation.ValueType != ValueTypeEnum.Operation)
            {
                throw new InvalidOperationException("演算子が必要な場所に、それ以外の値が指定されました。");
            }

            switch (operation.ValueOperation)
            {
                case OperationTypeEnum.Negation:
                    return AIValue.Negation(firstValue);
                case OperationTypeEnum.BitwiseComplement:
                    return AIValue.BitwiseComplement(firstValue);
                default:
                    throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// 値を計算する
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="firstValue"></param>
        /// <param name="secondPopValue"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static AIValue Calculation(AIValue operation, AIValue firstValue, AIValue secondPopValue)
        {
            if (operation.ValueType != ValueTypeEnum.Operation)
            {
                throw new InvalidOperationException("演算子が必要な場所に、それ以外の値が指定されました。");
            }

            switch (operation.ValueOperation)
            {
                case OperationTypeEnum.Plus:
                    return AIValue.Plus(firstValue, secondPopValue);
                case OperationTypeEnum.Minus:
                    return AIValue.Minus(firstValue, secondPopValue);
                case OperationTypeEnum.Multiplication:
                    return AIValue.Multiplication(firstValue, secondPopValue);
                case OperationTypeEnum.Division:
                    return AIValue.Division(firstValue, secondPopValue);
                case OperationTypeEnum.Remainder:
                    return AIValue.Remainder(firstValue, secondPopValue);
                case OperationTypeEnum.LeftShift:
                    return AIValue.LeftShift(firstValue, secondPopValue);
                case OperationTypeEnum.RightShift:
                    return AIValue.RightShift(firstValue, secondPopValue);
                case OperationTypeEnum.Less:
                    return AIValue.Less(firstValue, secondPopValue);
                case OperationTypeEnum.Greater:
                    return AIValue.Greater(firstValue, secondPopValue);
                case OperationTypeEnum.LessEqual:
                    return AIValue.LessEqual(firstValue, secondPopValue);
                case OperationTypeEnum.GreaterEqual:
                    return AIValue.GreaterEqual(firstValue, secondPopValue);
                case OperationTypeEnum.Equal:
                    return AIValue.Equal(firstValue, secondPopValue);
                case OperationTypeEnum.NotEqual:
                    return AIValue.NotEqual(firstValue, secondPopValue);
                case OperationTypeEnum.And:
                    return AIValue.And(firstValue, secondPopValue);
                case OperationTypeEnum.Xor:
                    return AIValue.Xor(firstValue, secondPopValue);
                case OperationTypeEnum.Or:
                    return AIValue.Or(firstValue, secondPopValue);
                case OperationTypeEnum.ConditionalAnd:
                    return AIValue.ConditionalAnd(firstValue, secondPopValue);
                case OperationTypeEnum.ConditionalOr:
                    return AIValue.ConditionalOr(firstValue, secondPopValue);
                default:
                    throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// 値を計算する
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="firstValue"></param>
        /// <param name="secondPopValue"></param>
        /// <param name="thirdPopValue"></param>
        /// <returns></returns>
        public static AIValue Calculation(AIValue operation, AIValue firstValue, AIValue secondPopValue, AIValue thirdPopValue)
        {
            if (operation.ValueType != ValueTypeEnum.Operation)
            {
                throw new InvalidOperationException("演算子が必要な場所に、それ以外の値が指定されました。");
            }

            switch (operation.ValueOperation)
            {
                case OperationTypeEnum.Ternary_Question:
                    if (!firstValue.ValueType.HasFlag(ValueTypeEnum.Bool))
                    {
                        throw new InvalidAIValueException($"Bool型の指定が必要です。[{firstValue.Value}]");
                    }

                    if ((secondPopValue.ValueType & thirdPopValue.ValueType) == ValueTypeEnum.None)
                    {
                        throw new InvalidAIValueException($"同一の型の指定が必要です。[{secondPopValue.Value},{thirdPopValue.Value}]");
                    }

                    if (firstValue.ValueBool)
                    {
                        return secondPopValue;
                    }
                    else
                    {
                        return thirdPopValue;
                    }
                default:
                    throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// !:反転
        /// </summary>
        /// <param name="firstValue"></param>
        /// <returns></returns>
        private static AIValue Negation(AIValue firstValue)
        {
            if (firstValue.ValueType.HasFlag(ValueTypeEnum.Int32))
            {
                return new AIValue(~firstValue.ValueInt32);
            }
            else if (firstValue.ValueType.HasFlag(ValueTypeEnum.Bool))
            {
                return new AIValue(!firstValue.ValueBool);
            }
            else
            {
                throw new InvalidAIValueException($"指定できる型は、数値型もしくはBool型です。{firstValue.Value}");
            }
        }

        /// <summary>
        /// ~:ビット反転
        /// </summary>
        /// <param name="firstValue"></param>
        /// <returns></returns>
        private static AIValue BitwiseComplement(AIValue firstValue)
        {
            if (firstValue.ValueType.HasFlag(ValueTypeEnum.Int32))
            {
                return new AIValue(~firstValue.ValueInt32);
            }

            throw new InvalidAIValueException($"指定できる型は、数値型です。{firstValue.Value}");
        }

        /// <summary>
        /// +:プラス
        /// </summary>
        /// <param name="firstValue"></param>
        /// <param name="secondValue"></param>
        /// <returns></returns>
        private static AIValue Plus(AIValue firstValue, AIValue secondValue)
        {
            if (firstValue.ValueType.HasFlag(ValueTypeEnum.String) &&
                secondValue.ValueType.HasFlag(ValueTypeEnum.String))
            {
                return new AIValue(firstValue.ValueString + secondValue.ValueString, ValueTypeEnum.String);
            }
            else if (firstValue.ValueType.HasFlag(ValueTypeEnum.Int32) &&
                     secondValue.ValueType.HasFlag(ValueTypeEnum.Int32))
            {
                return new AIValue(firstValue.ValueInt32 + secondValue.ValueInt32);
            }

            throw new InvalidAIValueException($"指定できる型は、同じ型で数値型もしくは文字列型です。{firstValue.Value},{secondValue.Value}");
        }

        /// <summary>
        /// -:マイナス
        /// </summary>
        /// <param name="firstValue"></param>
        /// <param name="secondValue"></param>
        /// <returns></returns>
        private static AIValue Minus(AIValue firstValue, AIValue secondValue)
        {
            if (firstValue.ValueType.HasFlag(ValueTypeEnum.Int32) &&
                secondValue.ValueType.HasFlag(ValueTypeEnum.Int32))
            {
                return new AIValue(firstValue.ValueInt32 - secondValue.ValueInt32);
            }

            throw new InvalidAIValueException($"指定できる型は、同じ型で数値型です。{firstValue.Value},{secondValue.Value}");
        }

        /// <summary>
        /// *:掛け算
        /// </summary>
        /// <param name="firstValue"></param>
        /// <param name="secondValue"></param>
        /// <returns></returns>
        private static AIValue Multiplication(AIValue firstValue, AIValue secondValue)
        {
            if (firstValue.ValueType.HasFlag(ValueTypeEnum.Int32) &&
                secondValue.ValueType.HasFlag(ValueTypeEnum.Int32))
            {
                return new AIValue(firstValue.ValueInt32 * secondValue.ValueInt32);
            }

            throw new InvalidAIValueException($"指定できる型は、同じ型で数値型です。{firstValue.Value},{secondValue.Value}");
        }

        /// <summary>
        /// /:割り算
        /// </summary>
        /// <param name="firstValue"></param>
        /// <param name="secondValue"></param>
        /// <returns></returns>
        private static AIValue Division(AIValue firstValue, AIValue secondValue)
        {
            if (firstValue.ValueType.HasFlag(ValueTypeEnum.Int32) &&
                secondValue.ValueType.HasFlag(ValueTypeEnum.Int32))
            {
                return new AIValue(firstValue.ValueInt32 / secondValue.ValueInt32);
            }

            throw new InvalidAIValueException($"指定できる型は、同じ型で数値型です。{firstValue.Value},{secondValue.Value}");
        }

        /// <summary>
        /// %:余り
        /// </summary>
        /// <param name="firstValue"></param>
        /// <param name="secondValue"></param>
        /// <returns></returns>
        private static AIValue Remainder(AIValue firstValue, AIValue secondValue)
        {
            if (firstValue.ValueType.HasFlag(ValueTypeEnum.Int32) &&
                secondValue.ValueType.HasFlag(ValueTypeEnum.Int32))
            {
                return new AIValue(firstValue.ValueInt32 % secondValue.ValueInt32);
            }

            throw new InvalidAIValueException($"指定できる型は、同じ型で数値型です。{firstValue.Value},{secondValue.Value}");
        }


        /// <summary>
        /// <<:左シフト
        /// </summary>
        /// <param name="firstValue"></param>
        /// <param name="secondPopValue"></param>
        /// <returns></returns>
        private static AIValue LeftShift(AIValue firstValue, AIValue secondValue)
        {
            if (firstValue.ValueType.HasFlag(ValueTypeEnum.Int32) &&
                secondValue.ValueType.HasFlag(ValueTypeEnum.Int32))
            {
                return new AIValue(firstValue.ValueInt32 << secondValue.ValueInt32);
            }

            throw new InvalidAIValueException($"指定できる型は、同じ型で数値型です。{firstValue.Value},{secondValue.Value}");
        }

        /// <summary>
        /// >>:右シフト
        /// </summary>
        /// <param name="firstValue"></param>
        /// <param name="secondValue"></param>
        /// <returns></returns>
        private static AIValue RightShift(AIValue firstValue, AIValue secondValue)
        {
            if (firstValue.ValueType.HasFlag(ValueTypeEnum.Int32) &&
                secondValue.ValueType.HasFlag(ValueTypeEnum.Int32))
            {
                return new AIValue(firstValue.ValueInt32 >> secondValue.ValueInt32);
            }

            throw new InvalidAIValueException($"指定できる型は、同じ型で数値型です。{firstValue.Value},{secondValue.Value}");
        }

        /// <summary>
        /// <:より小さい
        /// </summary>
        /// <param name="firstValue"></param>
        /// <param name="secondValue"></param>
        /// <returns></returns>
        private static AIValue Less(AIValue firstValue, AIValue secondValue)
        {
            if (firstValue.ValueType.HasFlag(ValueTypeEnum.Int32) &&
                secondValue.ValueType.HasFlag(ValueTypeEnum.Int32))
            {
                return new AIValue(firstValue.ValueInt32 < secondValue.ValueInt32);
            }

            throw new InvalidAIValueException($"指定できる型は、同じ型で数値型です。{firstValue.Value},{secondValue.Value}");
        }

        /// <summary>
        /// >:より大きい
        /// </summary>
        /// <param name="firstValue"></param>
        /// <param name="secondPopValue"></param>
        /// <returns></returns>
        private static AIValue Greater(AIValue firstValue, AIValue secondValue)
        {
            if (firstValue.ValueType.HasFlag(ValueTypeEnum.Int32) &&
                secondValue.ValueType.HasFlag(ValueTypeEnum.Int32))
            {
                return new AIValue(firstValue.ValueInt32 > secondValue.ValueInt32);
            }

            throw new InvalidAIValueException($"指定できる型は、同じ型で数値型です。{firstValue.Value},{secondValue.Value}");
        }

        /// <summary>
        /// <=:以上
        /// </summary>
        /// <param name="firstValue"></param>
        /// <param name="secondPopValue"></param>
        /// <returns></returns>
        private static AIValue LessEqual(AIValue firstValue, AIValue secondValue)
        {
            if (firstValue.ValueType.HasFlag(ValueTypeEnum.Int32) &&
                secondValue.ValueType.HasFlag(ValueTypeEnum.Int32))
            {
                return new AIValue(firstValue.ValueInt32 <= secondValue.ValueInt32);
            }

            throw new InvalidAIValueException($"指定できる型は、同じ型で数値型です。{firstValue.Value},{secondValue.Value}");
        }

        /// <summary>
        /// >=:以上
        /// </summary>
        /// <param name="firstValue"></param>
        /// <param name="secondPopValue"></param>
        /// <returns></returns>
        private static AIValue GreaterEqual(AIValue firstValue, AIValue secondValue)
        {
            if (firstValue.ValueType.HasFlag(ValueTypeEnum.Int32) &&
                secondValue.ValueType.HasFlag(ValueTypeEnum.Int32))
            {
                return new AIValue(firstValue.ValueInt32 >= secondValue.ValueInt32);
            }

            throw new InvalidAIValueException($"指定できる型は、同じ型で数値型です。{firstValue.Value},{secondValue.Value}");
        }

        /// <summary>
        /// ==:イコール
        /// </summary>
        /// <param name="firstValue"></param>
        /// <param name="secondValue"></param>
        /// <returns></returns>
        private static AIValue Equal(AIValue firstValue, AIValue secondValue)
        {
            if (firstValue.ValueType.HasFlag(ValueTypeEnum.String) &&
                secondValue.ValueType.HasFlag(ValueTypeEnum.String))
            {
                return new AIValue(firstValue.ValueString == secondValue.ValueString);
            }
            else if (firstValue.ValueType.HasFlag(ValueTypeEnum.Int32) &&
                     secondValue.ValueType.HasFlag(ValueTypeEnum.Int32))
            {
                return new AIValue(firstValue.ValueInt32 == secondValue.ValueInt32);
            }

            throw new InvalidAIValueException($"指定できる型は、同じ型で数値型もしくは文字列型です。{firstValue.Value},{secondValue.Value}");
        }

        /// <summary>
        /// !=:Notイコール
        /// </summary>
        /// <param name="firstValue"></param>
        /// <param name="secondValue"></param>
        /// <returns></returns>
        private static AIValue NotEqual(AIValue firstValue, AIValue secondValue)
        {
            if (firstValue.ValueType.HasFlag(ValueTypeEnum.String) &&
                secondValue.ValueType.HasFlag(ValueTypeEnum.String))
            {
                return new AIValue(firstValue.ValueString != secondValue.ValueString);
            }
            else if (firstValue.ValueType.HasFlag(ValueTypeEnum.Int32) &&
                     secondValue.ValueType.HasFlag(ValueTypeEnum.Int32))
            {
                return new AIValue(firstValue.ValueInt32 != secondValue.ValueInt32);
            }

            throw new InvalidAIValueException($"指定できる型は、同じ型で数値型もしくは文字列型です。{firstValue.Value},{secondValue.Value}");
        }

        /// <summary>
        /// &:And
        /// </summary>
        /// <param name="firstValue"></param>
        /// <param name="secondValue"></param>
        /// <returns></returns>
        private static AIValue And(AIValue firstValue, AIValue secondValue)
        {
            if (firstValue.ValueType.HasFlag(ValueTypeEnum.Int32) &&
                secondValue.ValueType.HasFlag(ValueTypeEnum.Int32))
            {
                return new AIValue(firstValue.ValueInt32 & secondValue.ValueInt32);
            }

            throw new InvalidAIValueException($"指定できる型は、同じ型で数値型です。{firstValue.Value},{secondValue.Value}");
        }

        /// <summary>
        /// ^:Xor
        /// </summary>
        /// <param name="firstValue"></param>
        /// <param name="secondValue"></param>
        /// <returns></returns>
        private static AIValue Xor(AIValue firstValue, AIValue secondValue)
        {
            if (firstValue.ValueType.HasFlag(ValueTypeEnum.Int32) &&
                secondValue.ValueType.HasFlag(ValueTypeEnum.Int32))
            {
                return new AIValue(firstValue.ValueInt32 ^ secondValue.ValueInt32);
            }

            throw new InvalidAIValueException($"指定できる型は、同じ型で数値型です。{firstValue.Value},{secondValue.Value}");
        }

        /// <summary>
        /// |:Or
        /// </summary>
        /// <param name="firstValue"></param>
        /// <param name="secondValue"></param>
        /// <returns></returns>
        private static AIValue Or(AIValue firstValue, AIValue secondValue)
        {
            if (firstValue.ValueType.HasFlag(ValueTypeEnum.Int32) &&
                secondValue.ValueType.HasFlag(ValueTypeEnum.Int32))
            {
                return new AIValue(firstValue.ValueInt32 | secondValue.ValueInt32);
            }

            throw new InvalidAIValueException($"指定できる型は、同じ型で数値型です。{firstValue.Value},{secondValue.Value}");
        }

        /// <summary>
        /// ||:Or
        /// </summary>
        /// <param name="firstValue"></param>
        /// <param name="secondValue"></param>
        /// <returns></returns>
        private static AIValue ConditionalOr(AIValue firstValue, AIValue secondValue)
        {
            if (firstValue.ValueType.HasFlag(ValueTypeEnum.Bool) &&
                secondValue.ValueType.HasFlag(ValueTypeEnum.Bool))
            {
                return new AIValue(firstValue.ValueBool || secondValue.ValueBool);
            }

            throw new InvalidAIValueException($"指定できる型は、同じ型でBool型です。{firstValue.Value},{secondValue.Value}");
        }

        /// <summary>
        /// &&:And
        /// </summary>
        /// <param name="firstValue"></param>
        /// <param name="secondValue"></param>
        /// <returns></returns>
        private static AIValue ConditionalAnd(AIValue firstValue, AIValue secondValue)
        {
            if (firstValue.ValueType.HasFlag(ValueTypeEnum.Bool) &&
                secondValue.ValueType.HasFlag(ValueTypeEnum.Bool))
            {
                return new AIValue(firstValue.ValueBool && secondValue.ValueBool);
            }

            throw new InvalidAIValueException($"指定できる型は、同じ型でBool型です。{firstValue.Value},{secondValue.Value}");
        }

        /// <summary>
        /// 16進数をパースする
        /// </summary>
        /// <param name="value"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private static bool TryParse16(string value, out string result)
        {
            var matched = default(Match);

            if ((matched = Regex.Match(value, RegexPatternHexadecimal_H, RegexOptions.Singleline | RegexOptions.IgnoreCase)).Success ||
                (matched = Regex.Match(value, RegexPatternHexadecimal_X, RegexOptions.Singleline | RegexOptions.IgnoreCase)).Success ||
                (matched = Regex.Match(value, RegexPatternHexadecimal_D, RegexOptions.Singleline | RegexOptions.IgnoreCase)).Success)
            {
                result = matched.Groups["value"].Value.Replace("_", "");
                return true;
            }

            result = "";
            return false;
        }

        /// <summary>
        /// 2進数をパースする
        /// </summary>
        /// <param name="value"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private static bool TryParse2(string value, out string result)
        {
            var matched = default(Match);

            if ((matched = Regex.Match(value, RegexPatternBinaryNumber_B, RegexOptions.Singleline | RegexOptions.IgnoreCase)).Success ||
                (matched = Regex.Match(value, RegexPatternBinaryNumber_P, RegexOptions.Singleline | RegexOptions.IgnoreCase)).Success)
            {
                result = matched.Groups["value"].Value.Replace("_", "");
                return true;
            }

            result = "";
            return false;
        }
        

        /// <summary>
        /// 8進数をパースする
        /// </summary>
        /// <param name="value"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private static bool TryParse8(string value, out string result)
        {
            var matched = default(Match);

            if ((matched = Regex.Match(value, RegexPatternOctal_O, RegexOptions.Singleline | RegexOptions.IgnoreCase)).Success)
            {
                result = matched.Groups["value"].Value.Replace("_", "");
                return true;
            }

            result = "";
            return false;
        }


        /// <summary>
        /// 10進数をパースする
        /// </summary>
        /// <param name="value"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private static bool TryParse10(string value, out string result)
        {
            var matched = default(Match);

            if ((matched = Regex.Match(value, RegexPatternDigit, RegexOptions.Singleline | RegexOptions.IgnoreCase)).Success)
            {
                result = matched.Groups["value"].Value.Replace("_", "");
                return true;
            }

            result = "";
            return false;
        }
    }
}
