using AILZ80ASM.Assembler;
using AILZ80ASM.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
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
            Low,                // low
            High,               // high
            Exists,             // exists
            Text,               // text
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
            ["("] =      OperationTypeEnum.LeftParenthesis,
            [")"] =      OperationTypeEnum.RightParenthesis,
            ["!"] =      OperationTypeEnum.Negation,
            ["~"] =      OperationTypeEnum.BitwiseComplement,
            ["low"] =    OperationTypeEnum.Low,
            ["high"] =   OperationTypeEnum.High,
            ["exists"] = OperationTypeEnum.Exists,
            ["text"] =   OperationTypeEnum.Text,
            ["*"] =      OperationTypeEnum.Multiplication,
            ["/"] =      OperationTypeEnum.Division,
            ["%"] =      OperationTypeEnum.Remainder,
            ["+"] =      OperationTypeEnum.Plus,
            ["-"] =      OperationTypeEnum.Minus,
            ["<<"] =     OperationTypeEnum.LeftShift,
            [">>"] =     OperationTypeEnum.RightShift,
            ["<"] =      OperationTypeEnum.Less,
            [">"] =      OperationTypeEnum.Greater,
            ["<="] =     OperationTypeEnum.LessEqual,
            [">="] =     OperationTypeEnum.GreaterEqual,
            ["=="] =     OperationTypeEnum.Equal,
            ["!="] =     OperationTypeEnum.NotEqual,
            ["&"] =      OperationTypeEnum.And,
            ["^"] =      OperationTypeEnum.Xor,
            ["|"] =      OperationTypeEnum.Or,
            ["&&"] =     OperationTypeEnum.ConditionalAnd,
            ["||"] =     OperationTypeEnum.ConditionalOr,
            ["?"] =      OperationTypeEnum.Ternary_Question,
            [":"] =      OperationTypeEnum.Ternary_Colon,
        };

        private static readonly Dictionary<OperationTypeEnum, int> FormulaPriority = new()
        {
            [OperationTypeEnum.RightParenthesis] = 1,   // )
            [OperationTypeEnum.Negation] = 3,           // !
            [OperationTypeEnum.BitwiseComplement] = 3,  // ~ 単項演算子は別で処理する ["+"] = 2,  ["-"] = 2,
            [OperationTypeEnum.Low] = 2,                // low
            [OperationTypeEnum.High] = 2,               // high
            [OperationTypeEnum.Exists] = 2,             // exists
            [OperationTypeEnum.Text] = 2,               // text
            [OperationTypeEnum.Multiplication] = 4,     // *
            [OperationTypeEnum.Division] = 4,           // /
            [OperationTypeEnum.Remainder] = 4,          // %
            [OperationTypeEnum.Plus] = 5,               // +
            [OperationTypeEnum.Minus] = 5,              // -
            [OperationTypeEnum.LeftShift] = 6,          // <<
            [OperationTypeEnum.RightShift] = 6,         // >>
            [OperationTypeEnum.Less] = 7,               // <
            [OperationTypeEnum.Greater] = 7,            // >
            [OperationTypeEnum.LessEqual] = 7,          // <=
            [OperationTypeEnum.GreaterEqual] = 7,       // >=
            [OperationTypeEnum.Equal] = 8,              // ==
            [OperationTypeEnum.NotEqual] = 8,           // !=
            [OperationTypeEnum.And] = 9,                // &
            [OperationTypeEnum.Xor] = 10,                // ^
            [OperationTypeEnum.Or] = 11,                // |
            [OperationTypeEnum.ConditionalAnd] = 12,    // &&
            [OperationTypeEnum.ConditionalOr] = 13,     // ||
            [OperationTypeEnum.Ternary_Question] = 15,  // ?
            [OperationTypeEnum.Ternary_Colon] = 14,     // :
            [OperationTypeEnum.LeftParenthesis] = 16,   // (
        };

        private static readonly Dictionary<OperationTypeEnum, ArgumentTypeEnum> OperationArgumentType = new()
        {
            [OperationTypeEnum.RightParenthesis] = ArgumentTypeEnum.None,               // )
            [OperationTypeEnum.Negation] = ArgumentTypeEnum.SingleArgument,             // !
            [OperationTypeEnum.BitwiseComplement] = ArgumentTypeEnum.SingleArgument,    // ~ 単項演算子は別で処理する ["+"] = 2,  ["-"] = 2,
            [OperationTypeEnum.Low] = ArgumentTypeEnum.SingleArgument,                  // low
            [OperationTypeEnum.High] = ArgumentTypeEnum.SingleArgument,                 // high
            [OperationTypeEnum.Exists] = ArgumentTypeEnum.SingleArgument,               // exists
            [OperationTypeEnum.Text] = ArgumentTypeEnum.SingleArgument,                 // text
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

        /// <summary>
        /// オペレーション判別用（文字列のみ）
        /// </summary>
        private static readonly string[] WordOperationKeys = OperationTypeTable.Keys.Where(m => char.IsLetter(m[0])).OrderByDescending(m => m.Length).ToArray();

        /// <summary>
        /// オペレーション判別用（記号のみ）
        /// </summary>
        private static readonly string[] SymbolOperationKeys = OperationTypeTable.Keys.Where(m => !char.IsLetter(m[0])).OrderByDescending(m => m.Length).ToArray();

        /// <summary>
        /// オペレーション判別・正規表現用（記号のみ）
        /// </summary>
        private static readonly string SymbolOperationKeysString = string.Join("|", SymbolOperationKeys.Select(m => Regex.Escape(m)));

        private static readonly string RegexPatternHexadecimal_H = @"^(?<value>([0-9A-Fa-f]+))H$";
        private static readonly string RegexPatternHexadecimal_X = @"^0x(?<value>([0-9A-Fa-f]+))$";
        private static readonly string RegexPatternHexadecimal_D = @"^\$(?<value>([0-9A-Fa-f]+))$";
        private static readonly string RegexPatternOctal_O = @"^(?<value>([0-7]+))O$";
        private static readonly string RegexPatternBinaryNumber_B = @"^(?<value>([01_]+))B$";
        private static readonly string RegexPatternBinaryNumber_P = @"^%(?<value>([01_]+))$";
        private static readonly string RegexPatternDigit = @"^(?<value>(\+|\-|)(\d+))$";
        private static readonly string RegexPatternValue = @"^(?<value>[0-9a-zA-Z_\$#@\.]+)";
        private static readonly string RegexPatternFunction = @"^(?<function>[0-9a-zA-Z_]+\s*\()";

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
                case ValueTypeEnum.Char:
                case ValueTypeEnum.String:
                case ValueTypeEnum.Function:
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// 演算子を抜き出す
        /// </summary>
        /// <param name="target"></param>
        /// <param name="resultFormula"></param>
        /// <returns></returns>
        public static bool TryParseFormula(ref string target, out string resultFormula)
        {
            var localTarget = target;
            resultFormula = "";

            // 記号のみのチェック
            foreach (var key in SymbolOperationKeys)
            {
                if (Regex.IsMatch(localTarget, $"^{Regex.Escape(key)}", RegexOptions.IgnoreCase))
                {
                    resultFormula = key;
                    target = localTarget.Substring(key.Length).TrimStart();
                    return true;
                }
            }

            // 文字列演算子のチェック
            if (!WordOperationKeys.Any(m => Regex.IsMatch(localTarget, $"^{Regex.Escape(m)}($|\\s+)", RegexOptions.IgnoreCase)))
            {
                return false;
            }

            // この文字の次の演算子を調べる
            foreach (var key in WordOperationKeys)
            {
                if (Regex.IsMatch(localTarget, $"^{Regex.Escape(key)}($|\\s+|{SymbolOperationKeysString})", RegexOptions.IgnoreCase))
                {
                    // 次の演算子を調査する
                    var nextTarget = localTarget.Substring(key.Length).TrimStart();
                    if (!string.IsNullOrEmpty(nextTarget))
                    {
                        foreach (var ope in SymbolOperationKeys)
                        {
                            if (Regex.IsMatch(nextTarget, $"^{Regex.Escape(ope)}", RegexOptions.IgnoreCase))
                            {
                                var operationType = OperationTypeTable[ope];
                                var operationArgument = OperationArgumentType[operationType];
                                switch (operationArgument)
                                {
                                    case ArgumentTypeEnum.None:
                                    case ArgumentTypeEnum.SingleArgument:
                                        // 文字の演算子が使える事を判別した
                                        resultFormula = key;
                                        target = localTarget.Substring(key.Length).TrimStart();
                                        return true;
                                    case ArgumentTypeEnum.DoubleArgument:
                                    case ArgumentTypeEnum.TripleArgument:
                                    default:
                                        return false;
                                }
                            }
                        }
                        // 文字の演算子が使える事を判別した
                        resultFormula = key;
                        target = localTarget.Substring(key.Length).TrimStart();
                        return true;
                    }
                    return false;
                }
            }
            return false;
        }

        /// <summary>
        /// Functionを抜き出す
        /// </summary>
        /// <param name="target"></param>
        /// <param name="resultFunction"></param>
        /// <returns></returns>
        /// <exception cref="InvalidAIMathException"></exception>
        public static bool TryParseFunction(ref string target, out string resultFunction)
        {
            resultFunction = "";

            var matched = Regex.Match(target, RegexPatternFunction, RegexOptions.Singleline | RegexOptions.IgnoreCase);
            if (matched.Success)
            {
                var localTarget = target;

                var function = matched.Groups["function"].Value;
                var targetIndex = function.Length;
                var startIndex = AIString.IndexOfAnySkipString(localTarget, '(', targetIndex);
                var endIndex = AIString.IndexOfAnySkipString(localTarget, ')', targetIndex);
                var counter = 1;
                while (endIndex != -1)
                {
                    if (startIndex != -1 && startIndex < endIndex)
                    {
                        counter++;
                        startIndex = AIString.IndexOfAnySkipString(localTarget, '(', startIndex + 1);
                    }
                    else
                    {
                        counter--;
                        if (counter == 0)
                        {
                            break;
                        }
                        endIndex = AIString.IndexOfAnySkipString(localTarget, ')', endIndex + 1);
                    }
                }

                if (endIndex == -1)
                {
                    throw new InvalidAIMathException("閉じ括弧が見つかりませんでした。");
                }

                if (counter != 0)
                {
                    throw new InvalidAIMathException("括弧の数が一致しませんでした。");
                }

                target = localTarget.Substring(endIndex + 1).TrimStart();
                resultFunction = localTarget.Substring(0, endIndex + 1);
                return true;
            }
            /*
            var localTarget = target;
            resultFunction = "";
            var matched = Regex.Match(target, $"(?<formula>({SymbolOperationKeysString}|\"|'))", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            if (matched.Success)
            {
                var formula = matched.Groups["formula"].Value;
                if (formula.StartsWith("("))
                {
                    var targetIndex = localTarget.IndexOf('(') + 1;
                    var startIndex = AIString.IndexOfAnySkipString(localTarget, '(', targetIndex);
                    var endIndex= AIString.IndexOfAnySkipString(localTarget, ')', targetIndex);

                    var counter = 0;
                    var endFlgForLeftParenthesis = false;
                    while ((startIndex != -1 || endFlgForLeftParenthesis) && endIndex != -1)
                    {
                        if (!endFlgForLeftParenthesis && startIndex < endIndex)
                        {
                            counter++;
                            startIndex = AIString.IndexOfAnySkipString(localTarget, '(', startIndex + 1);
                            if (startIndex == -1)
                            {
                                endFlgForLeftParenthesis = true;
                            }
                        }
                        else
                        {
                            counter--;
                            endIndex = AIString.IndexOfAnySkipString(localTarget, ')', endIndex + 1);

                            if (counter == 0)
                            {
                                break;
                            }
                        }
                    }

                    if (endIndex == -1)
                    {
                        throw new InvalidAIMathException("閉じ括弧が見つかりませんでした。");
                    }

                    if (counter != 0)
                    {
                        throw new InvalidAIMathException("括弧の数が一致しませんでした。");
                    }

                    if (startIndex == - 1)
                    {
                        target = localTarget.Substring(endIndex + 1).TrimStart();
                        resultFunction = localTarget.Substring(0, endIndex + 1);
                        return true;
                    }
                    
                }
            }
            */
            return false;
        }

        /// <summary>
        /// 値を抜き出す
        /// </summary>
        /// <param name="target"></param>
        /// <param name="resultValue"></param>
        /// <returns></returns>
        public static bool TryParseValue(ref string target, out string resultValue)
        { 
            var localTarget = target;
            resultValue = "";
            var matched = Regex.Match(target, RegexPatternValue, RegexOptions.Singleline | RegexOptions.IgnoreCase);
            if (matched.Success)
            {
                resultValue = matched.Groups["value"].Value;
                target = localTarget.Substring(resultValue.Length).TrimStart();
                return true;
            }

            return false;
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

        public AIValue(char value)
        {
            ValueType = ValueTypeEnum.Char;
            Value = value.ToString();
            ValueString = Value;
        }

        public AIValue(AIValue value1, AIValue value2)
            : this(value1.Value + value2.Value)
        { 
        }

        public bool TryParse<T>(out T resultValue)
            where T : struct
        {
            try
            {
                resultValue = ConvertTo<T>();
                return true;
            }
            catch
            {
                resultValue = default(T);
                return false;
            }
        }

        public string OriginalValue => Value;

        public T ConvertTo<T>()
        {
            return AsmException.TryCatch(Error.ErrorCodeEnum.E0004, Value, () =>
            {
                if (typeof(T) == typeof(int))
                {
                    if (ValueType.HasFlag(ValueTypeEnum.Int32))
                    {
                        return (T)(object)ValueInt32;
                    }
                    throw new InvalidAIValueException("int型に変換できません。");
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
                    throw new InvalidAIValueException("32ビット数値型に変換できません。");
                }
                else if (typeof(T) == typeof(UInt16))
                {
                    if (ValueType.HasFlag(ValueTypeEnum.Int32))
                    {
                        //有効範囲チェック
                        if (ValueInt32 < ((UInt16.MaxValue + 1) * -1) || ValueInt32 > UInt16.MaxValue)
                        {
                            throw new InvalidAIValueException("16ビット数値型に変換できません。");
                        }

                        if (ValueInt32 < 0)
                        {
                            return (T)(object)(UInt16)(UInt16.MaxValue + ValueInt32 + 1);
                        }
                        else
                        {
                            return (T)(object)(UInt16)(ValueInt32 & UInt16.MaxValue);
                        }
                    }
                    throw new InvalidAIValueException("16ビット数値型に変換できません。");
                }
                else if (typeof(T) == typeof(byte))
                {
                    if (ValueType.HasFlag(ValueTypeEnum.Int32))
                    {
                        //有効範囲チェック
                        if (ValueInt32 < ((byte.MaxValue + 1) * -1) || ValueInt32 > byte.MaxValue)
                        {
                            throw new InvalidAIValueException("8ビット数値型に変換できません。");
                        }

                        if (ValueInt32 < 0)
                        {
                            return (T)(object)(byte)(byte.MaxValue + ValueInt32 + 1);
                        }
                        else
                        {
                            return (T)(object)(byte)(ValueInt32 & byte.MaxValue);
                        }
                    }
                    throw new InvalidAIValueException("8ビット数値型に変換できません。");
                }
                else if (typeof(T) == typeof(bool))
                {
                    if (ValueType.HasFlag(ValueTypeEnum.Bool))
                    {
                        return (T)(object)ValueBool;
                    }
                    throw new InvalidAIValueException("Bool型に変換できません。");
                }
                else if (typeof(T) == typeof(string))
                {
                    if (ValueType.HasFlag(ValueTypeEnum.String))
                    {
                        return (T)(object)ValueString;
                    }
                    throw new InvalidAIValueException("String型に変換できません。");
                }
                else if (typeof(T) == typeof(char))
                {
                    if (ValueType.HasFlag(ValueTypeEnum.Char) && ValueString.Length == 1)
                    {
                        return (T)(object)ValueString[0];
                    }
                    throw new InvalidAIValueException("Char型に変換できません。");
                }
                else if (typeof(T) == typeof(byte[]))
                {
                    if (ValueType.HasFlag(ValueTypeEnum.Bytes))
                    {
                        return (T)(object)ValueBytes;
                    }
                    else if (ValueType.HasFlag(ValueTypeEnum.Int32))
                    {
                        if (ValueInt32 < 0)
                        {
                            return (T)(object)new[] { (byte)(byte.MaxValue + ValueInt32 + 1) };
                        }
                        else
                        {
                            return (T)(object)new[] { (byte)(ValueInt32 & byte.MaxValue) };
                        }
                    }
                    throw new InvalidAIValueException("8ビット数値型の配列に変換できません。");
                }
                else if (typeof(T) == typeof(UInt16[]))
                {
                    if (ValueType.HasFlag(ValueTypeEnum.Bytes))
                    {
                        return (T)(object)ValueBytes.Select(m => (UInt16)m).ToArray();
                    }
                    else if (ValueType.HasFlag(ValueTypeEnum.Int32))
                    {
                        if (ValueInt32 < 0)
                        {
                            return (T)(object)new[] { (UInt16)(UInt16.MaxValue + ValueInt32 + 1) };
                        }
                        else
                        {
                            return (T)(object)new[] { (UInt16)(ValueInt32 & UInt16.MaxValue) };
                        }
                    }
                    throw new InvalidAIValueException("16ビット数値型の配列に変換できません。");
                }
                else if ((typeof(T) == typeof(object)))
                {
                    if (ValueType.HasFlag(ValueTypeEnum.Int32))
                    {
                        return (T)(object)ValueInt32;
                    }
                    else if (ValueType.HasFlag(ValueTypeEnum.Bytes))
                    {
                        return (T)(object)string.Join(',', ValueBytes.Select(m => $"{m:2X}"));
                    }
                    else if (ValueType.HasFlag(ValueTypeEnum.String))
                    {
                        return (T)(object)ValueString;
                    }
                    else if (ValueType.HasFlag(ValueTypeEnum.Bool))
                    {
                        return (T)(object)(ValueBool ? "#TRUE" : "#FALSE");
                    }
                    else
                    {
                        throw new InvalidAIValueException("値に変換できません。");
                    }
                }
                else
                {
                    throw new NotImplementedException();
                }
            });
        }

        /// <summary>
        /// イコールの判断
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Equals(AIValue value)
        {
            var aiValue = AIValue.Equal(this, value, default(AsmLoad), default(AsmAddress?));
            return aiValue.ValueBool;
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
            switch (ValueType)
            {
                case ValueTypeEnum.None:
                    SetValueForNone(asmLoad, asmAddress);
                    break;
                case ValueTypeEnum.Function:
                    SetValueForFunction(asmLoad, asmAddress);
                    break;
            }
        }

        /// <summary>
        /// 値が確定できるか確認をする
        /// </summary>
        /// <param name="asmLoad"></param>
        /// <param name="asmAddress"></param>
        /// <returns></returns>
        public bool TrySetValue(AsmLoad asmLoad, AsmAddress? asmAddress)
        {
            try
            {
                SetValue(asmLoad, asmAddress);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void SetValueForNone(AsmLoad asmLoad, AsmAddress? asmAddress)
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
                    throw new InvalidAIValueException($"数値に変換できませんでした。[{Value}]");
                }
            }
            else if (AIString.TryParseCharMap(Value, asmLoad, out var charMap, out var resultString, out var validEscapeSequence))
            {
                if (!validEscapeSequence)
                {
                    throw new InvalidAIStringEscapeSequenceException($"有効なエスケープシーケンスではありません。[{Value}]", Value);
                }

                ValueCharMap = charMap;
                ValueString = resultString;
                ValueType |= ValueTypeEnum.Bytes;
                if (AIString.IsChar(Value, asmLoad))
                {
                    ValueType |= ValueTypeEnum.Char;
                    ValueBytes = AIString.GetBytesByChar(Value, asmLoad);
                }
                else
                {
                    ValueType |= ValueTypeEnum.String;
                    ValueBytes = AIString.GetBytesByString(Value, asmLoad);
                }

                if (ValueBytes.Length > 0 && ValueBytes.Length <= 2)
                {
                    ValueType |= ValueTypeEnum.Int32;
                    if (ValueBytes.Length == 2)
                    {
                        ValueInt32 = ValueBytes[0] * 256 + ValueBytes[1];
                    }
                    else
                    {
                        ValueInt32 = ValueBytes[0];
                    }
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
                //if (asmLoad != default && asmLoad.Share.AsmStep == AsmLoadShare.AsmStepEnum.PreAssemble)
                if (asmLoad != default && !asmAddress.Value.Output.HasValue)
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

                // Label
                var macroValue = MacroValueEnum.None;
                var tmpLabel = Value;
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
                        throw new InvalidAIValueException($"未定義:{Value}");
                    }
                    else
                    {
                        if (macroValue != MacroValueEnum.Text)
                        {
                            label.Calculation();
                        }
                    }

                    var value = label.Value;
                    ValueType = value.ValueType;
                    ValueInt32 = value.ValueInt32;
                    ValueBool = value.ValueBool;
                    ValueString = value.ValueString;
                    ValueOperation = value.ValueOperation;
                }
            }
        }

        /// <summary>
        /// ファンクションの値を求める
        /// </summary>
        /// <param name="asmLoad"></param>
        /// <param name="asmAddress"></param>
        /// <exception cref="InvalidAIValueException"></exception>
        private void SetValueForFunction(AsmLoad asmLoad, AsmAddress? asmAddress)
        {
            var startIndex = Value.IndexOf('(');
            var functionName = Value.Substring(0, startIndex).Trim();
            var function = asmLoad.FindFunction(functionName);

            var lastIndex = Value.LastIndexOf(')');
            if (function == default || lastIndex == -1)
            {
                throw new InvalidAIValueException($"Functionが見つかりませんでした。{functionName}");
            }
            var arguments = AIName.ParseArguments(Value.Substring(startIndex + 1, lastIndex - startIndex - 1));

            var calcValue = function.Calculation(arguments, asmLoad, asmAddress);

            ValueType = calcValue.ValueType;
            ValueInt32 = calcValue.ValueInt32;
            ValueBool = calcValue.ValueBool;
            ValueString = calcValue.ValueString;
            ValueOperation = calcValue.ValueOperation;
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
        public static AIValue Calculation(AIValue operation, AIValue firstValue, AsmLoad asmLoad, AsmAddress? asmAddress)
        {
            if (operation.ValueType != ValueTypeEnum.Operation)
            {
                throw new InvalidOperationException("演算子が必要な場所に、それ以外の値が指定されました。");
            }

            switch (operation.ValueOperation)
            {
                case OperationTypeEnum.Negation:
                    return AIValue.Negation(firstValue, asmLoad, asmAddress);
                case OperationTypeEnum.BitwiseComplement:
                    return AIValue.BitwiseComplement(firstValue, asmLoad, asmAddress);
                case OperationTypeEnum.Low:
                    return AIValue.Low(firstValue, asmLoad, asmAddress);
                case OperationTypeEnum.High:
                    return AIValue.High(firstValue, asmLoad, asmAddress);
                case OperationTypeEnum.Exists:
                    return AIValue.Exists(firstValue, asmLoad, asmAddress);
                case OperationTypeEnum.Text:
                    return AIValue.Text(firstValue, asmLoad, asmAddress);
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
        public static AIValue Calculation(AIValue operation, AIValue firstValue, AIValue secondPopValue, AsmLoad asmLoad, AsmAddress? asmAddress)
        {
            if (operation.ValueType != ValueTypeEnum.Operation)
            {
                throw new InvalidOperationException("演算子が必要な場所に、それ以外の値が指定されました。");
            }

            switch (operation.ValueOperation)
            {
                case OperationTypeEnum.Plus:
                    return AIValue.Plus(firstValue, secondPopValue, asmLoad, asmAddress);
                case OperationTypeEnum.Minus:
                    return AIValue.Minus(firstValue, secondPopValue, asmLoad, asmAddress);
                case OperationTypeEnum.Multiplication:
                    return AIValue.Multiplication(firstValue, secondPopValue, asmLoad, asmAddress);
                case OperationTypeEnum.Division:
                    return AIValue.Division(firstValue, secondPopValue, asmLoad, asmAddress);
                case OperationTypeEnum.Remainder:
                    return AIValue.Remainder(firstValue, secondPopValue, asmLoad, asmAddress);
                case OperationTypeEnum.LeftShift:
                    return AIValue.LeftShift(firstValue, secondPopValue, asmLoad, asmAddress);
                case OperationTypeEnum.RightShift:
                    return AIValue.RightShift(firstValue, secondPopValue, asmLoad, asmAddress);
                case OperationTypeEnum.Less:
                    return AIValue.Less(firstValue, secondPopValue, asmLoad, asmAddress);
                case OperationTypeEnum.Greater:
                    return AIValue.Greater(firstValue, secondPopValue, asmLoad, asmAddress);
                case OperationTypeEnum.LessEqual:
                    return AIValue.LessEqual(firstValue, secondPopValue, asmLoad, asmAddress);
                case OperationTypeEnum.GreaterEqual:
                    return AIValue.GreaterEqual(firstValue, secondPopValue, asmLoad, asmAddress);
                case OperationTypeEnum.Equal:
                    return AIValue.Equal(firstValue, secondPopValue, asmLoad, asmAddress);
                case OperationTypeEnum.NotEqual:
                    return AIValue.NotEqual(firstValue, secondPopValue, asmLoad, asmAddress);
                case OperationTypeEnum.And:
                    return AIValue.And(firstValue, secondPopValue, asmLoad, asmAddress);
                case OperationTypeEnum.Xor:
                    return AIValue.Xor(firstValue, secondPopValue, asmLoad, asmAddress);
                case OperationTypeEnum.Or:
                    return AIValue.Or(firstValue, secondPopValue, asmLoad, asmAddress);
                case OperationTypeEnum.ConditionalAnd:
                    return AIValue.ConditionalAnd(firstValue, secondPopValue, asmLoad, asmAddress);
                case OperationTypeEnum.ConditionalOr:
                    return AIValue.ConditionalOr(firstValue, secondPopValue, asmLoad, asmAddress);
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
        public static AIValue Calculation(AIValue operation, AIValue firstValue, AIValue secondPopValue, AIValue thirdPopValue, AsmLoad asmLoad, AsmAddress? asmAddress)
        {
            if (operation.ValueType != ValueTypeEnum.Operation)
            {
                throw new InvalidOperationException("演算子が必要な場所に、それ以外の値が指定されました。");
            }

            switch (operation.ValueOperation)
            {
                case OperationTypeEnum.Ternary_Question:
                    firstValue.SetValue(asmLoad, asmAddress);
                    secondPopValue.SetValue(asmLoad, asmAddress);
                    thirdPopValue.SetValue(asmLoad, asmAddress);
                    
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
        private static AIValue Negation(AIValue firstValue, AsmLoad asmLoad, AsmAddress? asmAddress)
        {
            firstValue.SetValue(asmLoad, asmAddress);

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
        private static AIValue BitwiseComplement(AIValue firstValue, AsmLoad asmLoad, AsmAddress? asmAddress)
        {
            firstValue.SetValue(asmLoad, asmAddress);

            if (firstValue.ValueType.HasFlag(ValueTypeEnum.Int32))
            {
                return new AIValue(~firstValue.ValueInt32);
            }

            throw new InvalidAIValueException($"指定できる型は、数値型です。{firstValue.Value}");
        }

        /// <summary>
        /// low:下位バイト
        /// </summary>
        /// <param name="firstValue"></param>
        /// <returns></returns>
        private static AIValue Low(AIValue firstValue, AsmLoad asmLoad, AsmAddress? asmAddress)
        {
            firstValue.SetValue(asmLoad, asmAddress);

            if (firstValue.ValueType.HasFlag(ValueTypeEnum.Int32))
            {
                return new AIValue(firstValue.ValueInt32 & 0xFF);
            }

            throw new InvalidAIValueException($"指定できる型は、数値型です。{firstValue.Value}");
        }

        /// <summary>
        /// high:上位バイト
        /// </summary>
        /// <param name="firstValue"></param>
        /// <returns></returns>
        private static AIValue High(AIValue firstValue, AsmLoad asmLoad, AsmAddress? asmAddress)
        {
            firstValue.SetValue(asmLoad, asmAddress);

            if (firstValue.ValueType.HasFlag(ValueTypeEnum.Int32))
            {
                return new AIValue((firstValue.ValueInt32 & 0xFF00) / 256);
            }

            throw new InvalidAIValueException($"指定できる型は、数値型です。{firstValue.Value}");
        }

        /// <summary>
        /// exists:ラベルの存在チェック
        /// </summary>
        /// <param name="firstValue"></param>
        /// <returns></returns>
        private static AIValue Exists(AIValue firstValue, AsmLoad asmLoad, AsmAddress? asmAddress)
        {
            if (firstValue.TrySetValue(asmLoad, asmAddress))
            {
                return new AIValue(true);
            }
            else
            {
                return new AIValue(false);
            }
        }

        /// <summary>
        /// text:ラベルの設定値を取得
        /// </summary>
        /// <param name="firstValue"></param>
        /// <returns></returns>
        private static AIValue Text(AIValue firstValue, AsmLoad asmLoad, AsmAddress? asmAddress)
        {
            var label = asmLoad.FindLabel(firstValue.OriginalValue);
            if (label == default)
            {
                throw new InvalidAIValueException($"未定義:{firstValue.OriginalValue}");
            }
            return new AIValue(label.ValueString, ValueTypeEnum.String);

        }

        /// <summary>
        /// +:プラス
        /// </summary>
        /// <param name="firstValue"></param>
        /// <param name="secondValue"></param>
        /// <returns></returns>
        private static AIValue Plus(AIValue firstValue, AIValue secondValue, AsmLoad asmLoad, AsmAddress? asmAddress)
        {
            firstValue.SetValue(asmLoad, asmAddress);
            secondValue.SetValue(asmLoad, asmAddress);

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
        private static AIValue Minus(AIValue firstValue, AIValue secondValue, AsmLoad asmLoad, AsmAddress? asmAddress)
        {
            firstValue.SetValue(asmLoad, asmAddress);
            secondValue.SetValue(asmLoad, asmAddress);

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
        private static AIValue Multiplication(AIValue firstValue, AIValue secondValue, AsmLoad asmLoad, AsmAddress? asmAddress)
        {
            firstValue.SetValue(asmLoad, asmAddress);
            secondValue.SetValue(asmLoad, asmAddress);

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
        private static AIValue Division(AIValue firstValue, AIValue secondValue, AsmLoad asmLoad, AsmAddress? asmAddress)
        {
            firstValue.SetValue(asmLoad, asmAddress);
            secondValue.SetValue(asmLoad, asmAddress);

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
        private static AIValue Remainder(AIValue firstValue, AIValue secondValue, AsmLoad asmLoad, AsmAddress? asmAddress)
        {
            firstValue.SetValue(asmLoad, asmAddress);
            secondValue.SetValue(asmLoad, asmAddress);

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
        private static AIValue LeftShift(AIValue firstValue, AIValue secondValue, AsmLoad asmLoad, AsmAddress? asmAddress)
        {
            firstValue.SetValue(asmLoad, asmAddress);
            secondValue.SetValue(asmLoad, asmAddress);

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
        private static AIValue RightShift(AIValue firstValue, AIValue secondValue, AsmLoad asmLoad, AsmAddress? asmAddress)
        {
            firstValue.SetValue(asmLoad, asmAddress);
            secondValue.SetValue(asmLoad, asmAddress);

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
        private static AIValue Less(AIValue firstValue, AIValue secondValue, AsmLoad asmLoad, AsmAddress? asmAddress)
        {
            firstValue.SetValue(asmLoad, asmAddress);
            secondValue.SetValue(asmLoad, asmAddress);

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
        private static AIValue Greater(AIValue firstValue, AIValue secondValue, AsmLoad asmLoad, AsmAddress? asmAddress)
        {
            firstValue.SetValue(asmLoad, asmAddress);
            secondValue.SetValue(asmLoad, asmAddress);

            if (firstValue.ValueType.HasFlag(ValueTypeEnum.Int32) &&
                secondValue.ValueType.HasFlag(ValueTypeEnum.Int32))
            {
                return new AIValue(firstValue.ValueInt32 > secondValue.ValueInt32);
            }

            throw new InvalidAIValueException($"指定できる型は、同じ型で数値型です。{firstValue.Value},{secondValue.Value}");
        }

        /// <summary>
        /// <=:以下
        /// </summary>
        /// <param name="firstValue"></param>
        /// <param name="secondPopValue"></param>
        /// <returns></returns>
        private static AIValue LessEqual(AIValue firstValue, AIValue secondValue, AsmLoad asmLoad, AsmAddress? asmAddress)
        {
            firstValue.SetValue(asmLoad, asmAddress);
            secondValue.SetValue(asmLoad, asmAddress);

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
        private static AIValue GreaterEqual(AIValue firstValue, AIValue secondValue, AsmLoad asmLoad, AsmAddress? asmAddress)
        {
            firstValue.SetValue(asmLoad, asmAddress);
            secondValue.SetValue(asmLoad, asmAddress);

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
        private static AIValue Equal(AIValue firstValue, AIValue secondValue, AsmLoad asmLoad, AsmAddress? asmAddress)
        {
            firstValue.SetValue(asmLoad, asmAddress);
            secondValue.SetValue(asmLoad, asmAddress);

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
            else if (firstValue.ValueType.HasFlag(ValueTypeEnum.Bool) &&
                     secondValue.ValueType.HasFlag(ValueTypeEnum.Bool))
            {
                return new AIValue(firstValue.ValueBool == secondValue.ValueBool);
            }

            throw new InvalidAIValueException($"指定できる型は、同じ型で数値型、Bool型もしくは文字列型です。{firstValue.Value},{secondValue.Value}");
        }

        /// <summary>
        /// !=:Notイコール
        /// </summary>
        /// <param name="firstValue"></param>
        /// <param name="secondValue"></param>
        /// <returns></returns>
        private static AIValue NotEqual(AIValue firstValue, AIValue secondValue, AsmLoad asmLoad, AsmAddress? asmAddress)
        {
            firstValue.SetValue(asmLoad, asmAddress);
            secondValue.SetValue(asmLoad, asmAddress);

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
            else if (firstValue.ValueType.HasFlag(ValueTypeEnum.Bool) &&
                     secondValue.ValueType.HasFlag(ValueTypeEnum.Bool))
            {
                return new AIValue(firstValue.ValueBool != secondValue.ValueBool);
            }

            throw new InvalidAIValueException($"指定できる型は、同じ型で数値型、Bool型もしくは文字列型です。{firstValue.Value},{secondValue.Value}");
        }

        /// <summary>
        /// &:And
        /// </summary>
        /// <param name="firstValue"></param>
        /// <param name="secondValue"></param>
        /// <returns></returns>
        private static AIValue And(AIValue firstValue, AIValue secondValue, AsmLoad asmLoad, AsmAddress? asmAddress)
        {
            firstValue.SetValue(asmLoad, asmAddress);
            secondValue.SetValue(asmLoad, asmAddress);

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
        private static AIValue Xor(AIValue firstValue, AIValue secondValue, AsmLoad asmLoad, AsmAddress? asmAddress)
        {
            firstValue.SetValue(asmLoad, asmAddress);
            secondValue.SetValue(asmLoad, asmAddress);

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
        private static AIValue Or(AIValue firstValue, AIValue secondValue, AsmLoad asmLoad, AsmAddress? asmAddress)
        {
            firstValue.SetValue(asmLoad, asmAddress);
            secondValue.SetValue(asmLoad, asmAddress);

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
        private static AIValue ConditionalOr(AIValue firstValue, AIValue secondValue, AsmLoad asmLoad, AsmAddress? asmAddress)
        {
            firstValue.SetValue(asmLoad, asmAddress);
            secondValue.SetValue(asmLoad, asmAddress);

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
        private static AIValue ConditionalAnd(AIValue firstValue, AIValue secondValue, AsmLoad asmLoad, AsmAddress? asmAddress)
        {
            firstValue.SetValue(asmLoad, asmAddress);
            secondValue.SetValue(asmLoad, asmAddress);

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
