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
            ReadCharMap(map);

            var charMap = CharMaps[map];
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

        private static void ReadCharMap(string map)
        {
            if (!CharMaps.ContainsKey(map))
            {
                var targetFileName = $"{map}.json";
                var targetFilePath = Path.Combine(System.Environment.CurrentDirectory, targetFileName);
                var result = GetFileContent(targetFilePath);
                if (!result.IsSuccess)
                {
                    var processDirectory = Path.GetDirectoryName(Environment.ProcessPath);
                    targetFilePath = Path.Combine(processDirectory, targetFileName);

                    result = GetFileContent(targetFilePath);
                    if (!result.IsSuccess)
                    {
                        result = GetResourceContent(targetFileName);
                        if (!result.IsSuccess)
                        {
                            throw new FileNotFoundException();
                        }
                    }
                }
                try
                {
                    var jsonResult = MakeJsonResult(result.Content);

                    CharMaps.Add(map, jsonResult);
                }
                catch (System.Text.Json.JsonException ex)
                {
                    throw new CharMapJsonReadException(ex.LineNumber);
                }
            }
        }

        private static Dictionary<char, byte[]> MakeJsonResult(string content)
        {
            var result = new Dictionary<char, byte[]>();
            var jsonResult = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, int[]>>(content);

            foreach (var kvp in jsonResult)
            {
                if (kvp.Value.Length == 0)
                {
                    throw new CharMapJsonReadException($"空のデータは設定できません。{kvp.Key}");
                }

                if (kvp.Key.Length > 1 && !(kvp.Key.Length == 2 && kvp.Key[0] == '\\'))
                {
                    throw new CharMapJsonReadException($"変換元は１文字しか設定できません。{kvp.Key}");
                }

                foreach (var item in kvp.Value)
                {
                    if (item < 0 || item > 255)
                    {
                        throw new CharMapJsonReadException($"指定できる値は、0～255までです。{kvp.Key}");
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

        private static (bool IsSuccess, string Content) GetFileContent(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return (false, "");
            }
            var content = File.ReadAllText(filePath);

            return (true, content);
        }

        private static (bool IsSuccess, string Content) GetResourceContent(string resourceName)
        {
            using (var stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream($"AILZ80ASM.CharMaps.{resourceName}"))
            {
                if (stream == default)
                {
                    throw new CharMapJsonReadException($"CharMap用のファイルが見つかりませんでした。{resourceName}");
                }

                using (var reader = new StreamReader(stream))
                {
                    if (stream == default)
                    {
                        return (false, "");
                    }

                    return (true, reader.ReadToEnd());
                }
            }
        }
    }
}
