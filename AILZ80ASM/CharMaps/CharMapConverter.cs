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
    public class CharMapConverter
    {
        private Dictionary<string, CharMapInfo> CharMaps = new Dictionary<string, CharMapInfo>();

        public byte[] ConvertToBytes(string map, string target)
        {
            if (!CharMaps.ContainsKey(map))
            {
                throw new CharMapNotFoundException(map);
            }

            return CharMaps[map.ToUpper()].ConvertToBytes(target);
        }

        /// <summary>
        /// CharMapの読み込み
        /// </summary>
        /// <param name="map"></param>
        /// <param name="fileInfo"></param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="CharMapJsonReadException"></exception>
        public void ReadCharMapFromFile(string map, FileInfo fileInfo, AsmLoad asmLoad)
        {
            if (string.IsNullOrEmpty(map))
            {
                throw new ArgumentException(nameof(map));
            }

            if (!AIName.ValidateCharMapName(map, asmLoad))
            {
                throw new ArgumentException(nameof(map));
            }

            if (!fileInfo.Exists)
            {
                throw new FileNotFoundException("ファイルが見つかりませんでした", fileInfo.FullName);
            }

            var mapName = map.ToUpper();
            if (CharMaps.ContainsKey(mapName))
            {
                throw new CharMapAlreadyDefinedException(mapName, $"既に設定済みです。[{mapName}]");
            }
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var resourceName = $"AILZ80ASM.CharMaps.{mapName.Substring(1)}.json";

            if (assembly.GetManifestResourceNames().Any(m => m == resourceName))
            {
                throw new CharMapAlreadyDefinedException(mapName, $"内蔵CharMapです。[{mapName}]");
            }

            try
            {
                var content = File.ReadAllText(fileInfo.FullName);
                MakeCharMap(mapName, content);
            }
            catch (System.Text.Json.JsonException ex)
            {
                throw new CharMapJsonReadException(ex.LineNumber + 1);
            }
        }

        /// <summary>
        /// リソースの読み込み
        /// </summary>
        /// <param name="map"></param>
        /// <param name="asmLoad"></param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="CharMapJsonReadException"></exception>
        public void ReadCharMapFromResource(string map, AsmLoad asmLoad)
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
                        MakeCharMap(mapName, reader.ReadToEnd());
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
        public bool IsContains(string map)
        {
            return CharMaps.ContainsKey(map);
        }

        /// <summary>
        /// CharMapを作成する
        /// </summary>
        /// <param name="mapName"></param>
        /// <param name="content"></param>
        private void MakeCharMap(string mapName, string content)
        {
            CharMaps.Add(mapName, new CharMapInfo(mapName, content));
        }
    }
}
