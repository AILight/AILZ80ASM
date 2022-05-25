using System;
using System.Linq;
using AILZ80ASM.Assembler;
using AILZ80ASM.Exceptions;

namespace AILZ80ASM.AILight
{
    public static class AIString
    {
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
            // エスケープシーケンスの置き換え
            foreach (var item in EscapeSequenceCharTables)
            {
                target = target.Replace(item[0], item[1]);
            }

            return target;
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
            charMap = "";
            resultString = "";
            validEscapeSequence = true;

            // スペシャル対応
            if (target.Length == 3 && target[0] == encloseChar && target[1] == '\\' && target[2] == encloseChar)
            {
                resultString = "\\";
                validEscapeSequence = false;
                return true;
            }

            if (target.EndsWith(encloseChar))
            {
                var startIndex = target.IndexOf(encloseChar);
                if (startIndex == target.Length - 1)
                {
                    return false;
                }
                resultString = target.Substring(startIndex + 1, target.Length - (startIndex + 1) - 1);

                if (target.StartsWith("@"))
                {
                    var colonIndex = target.IndexOf(":");
                    if (colonIndex == -1 || colonIndex > startIndex)
                    {
                        return false;
                    }

                    charMap = target.Substring(0, colonIndex).Trim();
                    if (string.IsNullOrEmpty(charMap))
                    {
                        return false;
                    }
                    
                    if (!AIName.ValidateCharMapName(charMap, asmLoad))
                    {
                        return false;
                    }
                }
                else if (startIndex != 0)
                {
                    // @が無いのに、文字の囲み記号も無い場合は文字列としては不正
                    return false;
                }

                // 文字列の終端が正しく設定されているか確認
                for (var index = 0; index < resultString.Length; index++)
                {
                    if (resultString[index] == encloseChar)
                    {
                        return false;
                    }
                    else if (resultString[index] == '\\')
                    {
                        index++;
                        if (index >= resultString.Length)
                        {
                            return false;
                        }
                    }
                }

                validEscapeSequence = ValidEscapeSequence(resultString);
                resultString = EscapeSequence(resultString);

                return true;
            }

            return false;
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
            if (!encloseChars.Any(m => target.EndsWith(m)) ||
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
