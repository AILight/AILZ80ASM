using AILZ80ASM.AILight;
using AILZ80ASM.Assembler;
using AILZ80ASM.Exceptions;
using AILZ80ASM.InstructionSet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace AILZ80ASM.CharMaps
{
    public class CharMapInfo
    {
        public string Name { get; set; }
        private Dictionary<string, byte[]> CharToBytesDictionary { get; set; }
        private int[] KeyParsingLengths { get; set; }
        private char[] KeyLeadingCharacters { get; set; }

        public CharMapInfo(string name, string content)
        {
            var result = MakeJsonResult(content);
            this.Name = name;
            this.CharToBytesDictionary = result;
            this.KeyParsingLengths = result.Keys.Select(m => m.Length).Distinct().OrderDescending().ToArray();
            this.KeyLeadingCharacters = result.Keys.Where(m => m.Length > 1).Select(m => m[0]).Distinct().ToArray();
        }

        public byte[] ConvertToBytes(string target)
        {
            var result = new List<byte>();

            for (var index = 0; index < target.Length; index++)
            {
                var item = target[index];
                if (!KeyLeadingCharacters.Contains(item))
                {
                    if (CharToBytesDictionary.TryGetValue(item.ToString(), out var bytes))
                    {
                        result.AddRange(bytes);
                    }
                    else
                    {
                        throw new CharMapConvertException($"{item}");
                    }
                }
                else
                {
                    var found = false;
                    foreach (var parseLength in KeyParsingLengths)
                    {
                        var checkItem = target.Substring(index, parseLength);
                        if (CharToBytesDictionary.TryGetValue(checkItem, out var bytes))
                        {
                            result.AddRange(bytes);
                            index += parseLength;
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                    {
                        throw new CharMapConvertException($"{item}");
                    }
                }

            }

            return result.ToArray();
        }

        /// <summary>
        /// Jsonからデータを積む
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        /// <exception cref="CharMapJsonReadException"></exception>
        private static Dictionary<string, byte[]> MakeJsonResult(string content)
        {
            var result = new Dictionary<string, byte[]>();
            var jsonResult = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, int[]>>(content);

            foreach (var kvp in jsonResult.OrderByDescending(m => m.Key.Length))
            {
                if (kvp.Key.Length == 0)
                {
                    throw new CharMapJsonReadException($"変換元に空のデータは設定できません");
                }

                if (kvp.Value.Length == 0)
                {
                    throw new CharMapJsonReadException($"変換先に空のデータは設定できません。[{kvp.Key}]");
                }

                foreach (var item in kvp.Value)
                {
                    if (item < 0 || item > 255)
                    {
                        throw new CharMapJsonReadException($"変換先に指定できる値は、0～255までです。[{kvp.Key}]");
                    }
                }

                var key = "";
                var escapeMode = false;
                foreach (var item in kvp.Key.ToArray())
                {
                    if (escapeMode)
                    {
                        key += item switch
                        {
                            '0' => '\0',
                            'a' => '\a',
                            'b' => '\b',
                            'f' => '\f',
                            'n' => '\n',
                            'r' => '\r',
                            't' => '\t',
                            'v' => '\v',
                            '\\' => '\\',
                            _ => throw new CharMapJsonReadException($"未対応のエスケープシーケンスです。{kvp.Key}"),
                        };
                        escapeMode = false;
                    }
                    else if (item == '\\')
                    {
                        escapeMode = true;
                    }
                    else
                    {
                        key += item;
                    }
                }
                if (escapeMode)
                {
                    key += '\\';
                }

                result.Add(key, kvp.Value.Select(m => (byte)m).ToArray());
            }

            return result;
        }
    }
}
