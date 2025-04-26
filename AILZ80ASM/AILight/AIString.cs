using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Formats.Tar;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using AILZ80ASM.Assembler;
using AILZ80ASM.Exceptions;

namespace AILZ80ASM.AILight
{
    public static class AIString
    {
        private static readonly string RegexPatternCharMapLabel = @"^((?<charMap>@.*\:)(?<label>[a-zA-Z0-9_]+))";
        private static readonly Regex CompiledRegexPatternCharMapLabel = new Regex(
            RegexPatternCharMapLabel,
            RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase
        );

        private static string[][] EscapeSequenceCharTables = new string[][]
        {

                new string[] { "\\\'", "\'" },
                new string[] { "\\\"", "\"" },
                new string[] { "\\\\", "\\" },
                new string[] { "\\0", "\0" },
                new string[] { "\\a", "\a" },
                new string[] { "\\b", "\b" },
                new string[] { "\\f", "\f" },
                new string[] { "\\n", "\n" },
                new string[] { "\\r", "\r" },
                new string[] { "\\t", "\t" },
                new string[] { "\\v", "\v" },
        };
        private static readonly Dictionary<string, char> EscapeLookup = EscapeSequenceCharTables.ToDictionary(x => x[0], x => x[1][0]);

        /// <summary>
        /// 文字列の宣言かを調べる
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static bool IsString(string target, AsmLoad asmLoad)
        {
            if (target.EndsWith('\"'))
            {
                return InternalTryParseCharMap(target, '\"', asmLoad, out var _, out var _, out var _);
            }
            else if (target.EndsWith('\''))
            {
                return InternalTryParseCharMap(target, '\'', asmLoad, out var _, out var _, out var _);
            }

            return false;
        }

        /// <summary>
        /// １文字の宣言かを調べる
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static bool IsChar(string target, AsmLoad asmLoad)
        {
            if (target.EndsWith('\''))
            {
                var result = InternalTryParseCharMap(target, '\'', asmLoad, out var _ , out var charString, out var _);

                if (result && charString.Length == 1)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// エスケープシーケンス
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static string EscapeSequence(string target)
        {
            if (string.IsNullOrEmpty(target)) return target;

            var sb = new StringBuilder(target.Length);

            for (var index = 0; index < target.Length; index++)
            {
                // バックスラッシュで始まり、かつ次の文字がある場合のみ判定
                if (target[index] == '\\' && index + 1 < target.Length)
                {
                    // テーブル上は 2 文字固定なので Slice(2) で OK
                    var seq = target.Substring(index, 2);

                    if (EscapeLookup.TryGetValue(seq, out var repl))
                    {
                        sb.Append(repl);
                        index++;                // 2 文字読んだので追加でインクリメント
                        continue;
                    }
                }
                sb.Append(target[index]);
            }
            return sb.ToString();
        }
 
        /// <summary>
        /// 無効なエスケープシーケンスが含まれているか確認
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static bool ValidEscapeSequence(string target)
        {
            // エスケープシーケンスの問題があるか確認
            var index = 0;
            while ((index = target.IndexOf('\\', index)) != -1)
            {
                if (index >= target.Length - 1)
                {
                    return false;
                }

                var checkString = target.Substring(index, 2);
                if (!EscapeSequenceCharTables.Any(m => string.Compare(m[0], checkString, true) == 0))
                {
                    return false;
                }
                index += 2;
            }
            return true;
        }

        /// <summary>
        /// 文字列を抜かして有効な命令文字かを判定
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static bool IsCorrectOperation(string target)
        {
            var result = default(bool);

            IndexOfAnySkipString(target, new char[] { }, 0, out result);

            return result;
        }

        /// <summary>
        /// 文字列からCharMap情報を取得する
        /// </summary>
        /// <param name="target"></param>
        /// <param name="charpMap"></param>
        /// <param name="resultString"></param>
        /// <returns></returns>
        public static bool TryParseCharMap(string target, AsmLoad asmLoad, out string charMap, out string resultString, out bool validEscapeSequence)
        {
            if (target.EndsWith('\"'))
            {
                return InternalTryParseCharMap(target, '\"', asmLoad, out charMap, out resultString, out validEscapeSequence);
            }
            else if (target.EndsWith('\''))
            {
                return InternalTryParseCharMap(target, '\'', asmLoad, out charMap, out resultString, out validEscapeSequence);
            }
            else
            {
                var matchedLabel = CompiledRegexPatternCharMapLabel.Match(target);
                if (matchedLabel.Success)
                {
                    var charMapName = matchedLabel.Groups["charMap"].Value;
                    var labelName = matchedLabel.Groups["label"].Value;
                    var label = asmLoad.FindLabel(labelName);
                    if (label == default)
                    {
                        throw new InvalidAIValueException($"未定義:{labelName}");
                    }
                    
                    return InternalTryParseCharMap($"{charMapName}{label.ValueString.Replace("\\\"", "\"")}", '\"', asmLoad, out charMap, out resultString, out validEscapeSequence);
                }
            }

            charMap = "";
            resultString = "";
            validEscapeSequence = true;

            return false;
        }

        /// <summary>
        /// 文字列をスキップしてその中でvalueの文字位置を調べる
        /// </summary>
        /// <param name="target"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int IndexOfSkipString(string target, char value)
        {
            return IndexOfSkipString(target, value, 0);
        }

        /// <summary>
        /// 文字列をスキップしてその中でvalueの文字位置を調べる
        /// </summary>
        /// <param name="target"></param>
        /// <param name="value"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        public static int IndexOfSkipString(string target, char value, int startIndex)
        {
            return IndexOfAnySkipString(target, new []{ value }, startIndex, out var _);
        }

        /// <summary>
        /// 文字列をスキップしてその中でvalueの文字位置を調べる
        /// </summary>
        /// <param name="target"></param>
        /// <param name="value"></param>
        /// <param name="startIndex"></param>
        /// <param name="isAllAsciiChars"></param>
        /// <returns></returns>
        public static int IndexOfSkipString(string target, char value, int startIndex, out bool isAllAsciiChars)
        {
            return IndexOfAnySkipString(target, new[] { value }, startIndex, out isAllAsciiChars);
        }

        /// <summary>
        /// 文字列をスキップしてその中でvalueの文字位置を調べる
        /// </summary>
        /// <param name="target"></param>
        /// <param name="anyOf"></param>
        /// <returns></returns>
        public static int IndexOfAnySkipString(string target, char[] anyOf)
        {
            return IndexOfAnySkipString(target, anyOf, 0, out var _);
        }

        /// <summary>
        /// 文字列をスキップしてその中でvalueの文字位置を調べる
        /// </summary>
        /// <param name="target"></param>
        /// <param name="value"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static int IndexOfAnySkipString(string target, char value, int index)
        {
            return IndexOfAnySkipString(target, new[] { value }, index, out var _);
        }

        /// <summary>
        /// 文字列をスキップしてその中でvalueの文字位置を調べる
        /// </summary>
        /// <param name="target"></param>
        /// <param name="anyOf"></param>
        /// <param name="startIndex"></param>
        /// <param name="IsAllAsciiChars">全ての文字がASCII文字か？</param>
        /// <returns></returns>
        public static int IndexOfAnySkipString(string target, char[] anyOf, int startIndex, out bool isAllAsciiChars)
        {
            var skip = false;
            var mode = 0;
            var result = -1;
            isAllAsciiChars = true;

            for (var index = startIndex; index < target.Length; index++)
            {
                if (skip)
                {
                    skip = false;
                }
                else if (target[index] == '\'' && (mode == 0 || mode == 1))
                {
                    if (mode == 0 && (index > 0 && char.IsLetterOrDigit(target[index - 1])))
                    {
                        // アルファベットと数字に続く文字列開始記号は、無効とする
                    }
                    else
                    {
                        mode = (mode == 0 ? 1 : 0);
                    }
                }
                else if (target[index] == '\"' && (mode == 0 || mode == 2))
                {
                    if (mode == 0 && (index > 0 && char.IsLetterOrDigit(target[index - 1])))
                    {
                        // アルファベットと数字に続く文字列開始記号は、無効とする
                    }
                    else
                    {
                        mode = (mode == 0 ? 2 : 0);
                    }
                }
                else if (mode > 0 && target[index] == '\\')
                {
                    skip = true;
                }
                else if (mode == 0)
                {
                    if (anyOf.Any(m => m == target[index]))
                    {
                        return index;
                    }
                    isAllAsciiChars &= char.IsAscii(target[index]);
                }
            }

            return result;
            
        }

        /// <summary>
        /// 文字列からCharMap情報を取得する（内部処理用）
        /// </summary>
        /// <param name="target"></param>
        /// <param name="encloseChar"></param>
        /// <param name="charpMap"></param>
        /// <param name="resultString"></param>
        /// <returns></returns>
        private static bool InternalTryParseCharMap(string target, char encloseChar, AsmLoad asmLoad, out string charMap, out string resultString, out bool validEscapeSequence)
        {
            // 初期化
            charMap = "";
            resultString = "";
            validEscapeSequence = false;

            // 逐語的文字列リテラル
            var isVerbatimLiteral = asmLoad.AssembleOption.CompatRawString;

            //charpMapを確認する
            if (target.StartsWith("@"))
            {
                var coronIndex = target.IndexOf(':');
                var encloseIndex = target.IndexOf(encloseChar);
                if (coronIndex != -1 && coronIndex < encloseIndex)
                {
                    charMap = target.Substring(0, coronIndex);
                    target = target.Substring(coronIndex + 1);

                    if (!AIName.ValidateCharMapName(charMap, asmLoad))
                    {
                        return false;
                    }
                }
            }

            // 逐語的文字列リテラル
            if (target.StartsWith("@"))
            {
                target = target.Substring(1);
                isVerbatimLiteral = true;
            }

            if (target.Length >= 2 && target.StartsWith(encloseChar) && target.EndsWith(encloseChar))
            {
                resultString = target.Substring(1, target.Length - 2);

                if (isVerbatimLiteral)
                {
                    // encloseCharがシングルで指定されている場合はエラー
                    var checkEncloseCharIndex = 0;
                    while (checkEncloseCharIndex < resultString.Length)
                    {
                        var index = resultString.IndexOf(encloseChar, checkEncloseCharIndex);
                        if (index == -1)
                        {
                            break;
                        }
                        else if (index == resultString.Length - 1)
                        {
                            // 最後にある場合エスケープできないので、エラー文字列
                            return false;
                        }
                        else
                        {
                            if (resultString[index + 1] != '\\')
                            {
                                // encloseCharをエスケープしないとエラー
                                return false;
                            }

                            checkEncloseCharIndex = index + 2;
                        }
                    }

                    validEscapeSequence = true;
                }
                else
                {
                    // encloseCharが使われているか確認
                    var checkEncloseCharIndex = 0;
                    while (checkEncloseCharIndex < resultString.Length)
                    {
                        var index = resultString.IndexOf(encloseChar, checkEncloseCharIndex);
                        if (index == -1)
                        {
                            break;
                        }
                        else if (index == 0)
                        {
                            // 先頭にある場合エスケープできないので、エラー文字列
                            return false;
                        }
                        else
                        {
                            if (resultString[index - 1] != '\\')
                            {
                                // encloseCharをエスケープしないとエラー
                                return false;
                            }

                            checkEncloseCharIndex = index + 1;
                        }
                    }

                    //最後の文字が\の場合エラー
                    var checkEscapeCharIndex = 0;
                    while (checkEscapeCharIndex < resultString.Length)
                    {
                        var index = resultString.IndexOf('\\', checkEscapeCharIndex);
                        if (index == -1)
                        {
                            break;
                        }
                        else if (index == resultString.Length - 1)
                        {
                            // 最後の文字が\の場合
                            return false;
                        }
                        else
                        {
                            checkEscapeCharIndex += (index + 2);
                        }
                    }

                    validEscapeSequence = ValidEscapeSequence(resultString);
                    resultString = EscapeSequence(resultString);
                }
            }
            else
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// １文字の変換
        /// </summary>
        /// <param name="target"></param>
        /// <param name="asmLoad"></param>
        /// <returns></returns>
        /// <exception cref="InvalidAIStringException"></exception>
        public static byte[] GetBytesByChar(string target, AsmLoad asmLoad)
        {
            return InternalGetBytesByString(target, new[] { '\'' }, asmLoad);
        }

        /// <summary>
        /// 文字列の変換
        /// </summary>
        /// <param name="target"></param>
        /// <param name="asmLoad"></param>
        /// <returns></returns>
        /// <exception cref="InvalidAIStringException"></exception>
        public static byte[] GetBytesByString(string target, AsmLoad asmLoad)
        {
            return InternalGetBytesByString(target, new[] { '\'', '\"' }, asmLoad);
        }

        /// <summary>
        /// 文字、文字列の変換（内部処理）
        /// </summary>
        /// <param name="target"></param>
        /// <param name="encloseChar"></param>
        /// <param name="asmLoad"></param>
        /// <returns></returns>
        /// <exception cref="InvalidAIStringException"></exception>
        /// <exception cref="NotImplementedException"></exception>
        private static byte[] InternalGetBytesByString(string target, char[] encloseChars, AsmLoad asmLoad)
        {
            if ((!encloseChars.Any(m => target.EndsWith(m)) && !target.StartsWith("@")) ||
                !TryParseCharMap(target, asmLoad, out var charMap, out var resultString, out var invalidEscapeSequence))
            {
                throw new InvalidAIStringException("正しい文字列を指定してください");
            }

            if (string.IsNullOrEmpty(charMap))
            {
                charMap = "@SJIS";
            }

            if (!asmLoad.CharMapConverter_IsContains(charMap))
            {
                try
                {
                    asmLoad.CharMapConverter_ReadCharMapFromResource(charMap);
                }
                catch
                {
                    throw new CharMapNotFoundException(charMap);
                }
            }

            var bytes = asmLoad.CharMapConverter_ConvertToBytes(charMap, resultString);

            return bytes;
        }
    }
}
