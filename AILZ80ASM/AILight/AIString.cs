using System;
using System.Linq;
using AILZ80ASM.Assembler;
using AILZ80ASM.Exceptions;

namespace AILZ80ASM.AILight
{
    public static class AIString
    {
        /// <summary>
        /// 文字列の宣言かを調べる
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static bool IsString(string target, AsmLoad asmLoad)
        {
            if (target.EndsWith('\"'))
            {
                return TryParseCharMap(target, asmLoad, out var dummy1, out var dummy2);
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
                return TryParseCharMap(target, asmLoad, out var dummy1, out var dummy2);
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
            var tables = new[]
            {
                new [] { "\\\'", "\'" },
                new [] { "\\\"", "\"" },
                new [] { "\\\\", "\\" },
                new [] { "\\0", "\0" },
                new [] { "\\a", "\a" },
                new [] { "\\b", "\b" },
                new [] { "\\f", "\f" },
                new [] { "\\n", "\n" },
                new [] { "\\r", "\r" },
                new [] { "\\t", "\t" },
                new [] { "\\v", "\v" },
            };

            foreach (var item in tables)
            {
                target = target.Replace(item[0], item[1]);
            }

            return target;
        }

        /// <summary>
        /// 文字列からCharMap情報を取得する
        /// </summary>
        /// <param name="target"></param>
        /// <param name="charpMap"></param>
        /// <param name="resultString"></param>
        /// <returns></returns>
        public static bool TryParseCharMap(string target, AsmLoad asmLoad, out string charMap, out string resultString)
        {
            var result = InternalTryParseCharMap(target, '\"', asmLoad, out charMap, out resultString);
            if (!result)
            {
                result = InternalTryParseCharMap(target, '\'', asmLoad, out charMap, out resultString);
                if (result && resultString.Length != 1)
                {
                    return false;
                }
            }

            return result;
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
            return IndexOfAnySkipString(target, new []{ value }, startIndex);
        }

        /// <summary>
        /// 文字列をスキップしてその中でvalueの文字位置を調べる
        /// </summary>
        /// <param name="target"></param>
        /// <param name="anyOf"></param>
        /// <returns></returns>
        public static int IndexOfAnySkipString(string target, char[] anyOf)
        {
            return IndexOfAnySkipString(target, anyOf, 0);
        }

        /// <summary>
        /// 文字列をスキップしてその中でvalueの文字位置を調べる
        /// </summary>
        /// <param name="target"></param>
        /// <param name="anyOf"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        public static int IndexOfAnySkipString(string target, char[] anyOf, int startIndex)
        {
            var skip = false;
            var mode = 0;
            var result = -1;

            for (var index = startIndex; index < target.Length; index++)
            {
                if (skip)
                {
                    skip = false;
                }
                else if (mode == 0 && anyOf.Any(m => m == target[index]))
                {
                    return index;
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
        private static bool InternalTryParseCharMap(string target, char encloseChar, AsmLoad asmLoad, out string charMap, out string resultString)
        {
            charMap = "";
            resultString = "";

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
            return InternalGetBytesByString(target, '\'', asmLoad);
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
            return InternalGetBytesByString(target, '\"', asmLoad);
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
        private static byte[] InternalGetBytesByString(string target, char encloseChar, AsmLoad asmLoad)
        {
            if (!target.EndsWith(encloseChar) ||
                !TryParseCharMap(target, asmLoad, out var charMap, out var resultString))
            {
                switch (encloseChar)
                {
                    case '\'':
                        throw new InvalidAIStringException("正しい１文字を指定してください");
                    case '\"':
                        throw new InvalidAIStringException("正しい文字列を指定してください");
                    default:
                        throw new NotImplementedException();
                }
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
