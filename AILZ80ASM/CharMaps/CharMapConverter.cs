using AILZ80ASM.AILight;
using AILZ80ASM.Assembler;
using AILZ80ASM.Exceptions;
using AILZ80ASM.InstructionSet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AILZ80ASM.CharMaps
{
    public static class CharMapConverter
    {
        private static Dictionary<string, Dictionary<char, byte[]>> CharMaps { get; set; } = new Dictionary<string, Dictionary<char, byte[]>>();

        public static byte[] ConvertToBytes(string map, string target, ISA.EndiannessEnum endianness)
        {
            if (!CharMaps.ContainsKey(map))
            {
                throw new CharMapNotFoundException(map);
            }

            var charMap = CharMaps[map.ToUpper()];
            var result = new List<byte>();
            foreach (var item in target.ToArray())
            {
                if (charMap.TryGetValue(item, out var bytes))
                {
                    switch (endianness)
                    {
                        case ISA.EndiannessEnum.LittleEndian:
                            result.AddRange(bytes.Reverse());
                            break;
                        case ISA.EndiannessEnum.BigEndian:
                            result.AddRange(bytes);
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                }
                else
                {
                    throw new CharMapConvertException($"{item}");
                }
            }

            return result.ToArray();
        }

        /// <summary>
        /// CharMapの読み込み
        /// </summary>
        /// <param name="map"></param>
        /// <param name="filePath"></param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="CharMapJsonReadException"></exception>
        public static void ReadCharMapFromFile(string map, string filePath, AsmLoad asmLoad)
        {
            if (string.IsNullOrEmpty(map))
            {
                throw new ArgumentException(nameof(map));
            }

            if (!AIName.ValidateCharMapName(map, asmLoad))
            {
                throw new ArgumentException(nameof(map));
            }

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("ファイルが見つかりませんでした", filePath);
            }

            var mapName = map.ToUpper();
            if (CharMaps.ContainsKey(mapName))
            {
                CharMaps.Remove(mapName);
            }

            try
            {
                var content = File.ReadAllText(filePath);
                var jsonResult = MakeJsonResult(content);

                CharMaps.Add(mapName, jsonResult);
            }
            catch (System.Text.Json.JsonException ex)
            {
                throw new CharMapJsonReadException(ex.LineNumber + 1);
            }
        }

        /// <summary>
        /// リソースの読み濃いm
        /// </summary>
        /// <param name="map"></param>
        /// <param name="asmLoad"></param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="CharMapJsonReadException"></exception>
        public static void ReadCharMapFromResource(string map, AsmLoad asmLoad)
        {
            if (string.IsNullOrEmpty(map))
            {
                throw new ArgumentException(nameof(map));
            }

            if (!AIName.ValidateCharMapName(map, asmLoad))
            {
                throw new ArgumentException(nameof(map));
            }

            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var resourceName = $"AILZ80ASM.CharMaps.{map.Substring(1)}.json";

            if (!assembly.GetManifestResourceNames().Any(m => m == resourceName))
            {
                throw new FileNotFoundException("リソースが見つかりませんでした", resourceName);
            }

            var mapName = map.ToUpper();
            if (CharMaps.ContainsKey(mapName))
            {
                CharMaps.Remove(mapName);
            }

            try
            {
                using (var stream = assembly.GetManifestResourceStream(resourceName))
                {
                    if (stream == default)
                    {
                        throw new CharMapJsonReadException($"CharMap用のファイルが見つかりませんでした。{resourceName}");
                    }

                    using (var reader = new StreamReader(stream))
                    {
                        if (reader == default)
                        {
                            throw new CharMapJsonReadException($"CharMap用のファイルが読み込めませんでした。{resourceName}");
                        }
                        var jsonResult = MakeJsonResult(reader.ReadToEnd());
                        CharMaps.Add(mapName, jsonResult);
                    }
                }
            }
            catch (System.Text.Json.JsonException ex)
            {
                throw new CharMapJsonReadException(ex.LineNumber);
            }
        }

        /// <summary>
        /// コンテンツが含まれているか確認
        /// </summary>
        /// <param name="map"></param>
        /// <returns></returns>
        public static bool IsContains(string map)
        {
            return CharMaps.ContainsKey(map);
        }

        private static Dictionary<char, byte[]> MakeJsonResult(string content)
        {
            var result = new Dictionary<char, byte[]>();
            var jsonResult = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, int[]>>(content);

            foreach (var kvp in jsonResult)
            {
                if (kvp.Key.Length == 0)
                {
                    throw new CharMapJsonReadException($"変換元に空のデータは設定できません");
                }

                if (kvp.Value.Length == 0)
                {
                    throw new CharMapJsonReadException($"変換先に空のデータは設定できません。[{kvp.Key}]");
                }

                if (kvp.Key.Length > 1 && !(kvp.Key.Length == 2 && kvp.Key[0] == '\\'))
                {
                    throw new CharMapJsonReadException($"変換元は１文字しか設定できません。[{kvp.Key}]");
                }

                foreach (var item in kvp.Value)
                {
                    if (item < 0 || item > 255)
                    {
                        throw new CharMapJsonReadException($"変換先に指定できる値は、0～255までです。[{kvp.Key}]");
                    }
                }
                var key = kvp.Key[0];
                if (key == '\\' && kvp.Key.Length > 1)
                {
                    key = kvp.Key[1] switch
                    {
                        '0' => '\0',
                        'a' => '\a',
                        'b' => '\b',
                        'f' => '\f',
                        'n' => '\n',
                        'r' => '\r',
                        't' => '\t',
                        'v' => '\v',
                        _ => throw new CharMapJsonReadException($"未対応のエスケープシーケンスです。{kvp.Key}"),
                    };
                }

                result.Add(key, kvp.Value.Select(m => (byte)m).ToArray());
            }

            return result;
        }
    }
}
