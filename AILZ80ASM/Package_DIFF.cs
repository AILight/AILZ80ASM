using AILZ80ASM.Assembler;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AILZ80ASM
{
    public partial class Package
    {

        public bool DiffOutput(Dictionary<AsmEnum.FileTypeEnum, FileInfo> outputFiles)
        {
            var result = true;
            foreach (var item in outputFiles)
            {
                if (!item.Value.Exists)
                {
                    Trace.WriteLine($"{item.Value.Name}: 不一致 ファイルが見つかりません。");
                    continue;
                }

                using var fileStream = item.Value.OpenRead();

                try
                {
                    result &= DiffOutput(fileStream, item);
                }
                catch (Exception ex)
                {
                    result = false;
                    Trace.TraceError(ex.Message);
                }
                fileStream.Close();
            }
            Trace.WriteLine("");
            return result;
        }

        public bool DiffOutput(Stream stream, KeyValuePair<AsmEnum.FileTypeEnum, FileInfo> outputFile)
        {
            using var assembledStream = new MemoryStream();
            using var originalStream = new MemoryStream();
            stream.CopyTo(originalStream);

            SaveOutput(assembledStream, outputFile);
            return DiffOutput(originalStream.ToArray(), assembledStream.ToArray(), outputFile);
        }

        public bool DiffOutput(byte[] original, byte[] assembled, KeyValuePair<AsmEnum.FileTypeEnum, FileInfo> outputFile)
        {
            var result = true;
            var resultString = "一致";

            Trace.Write($"{outputFile.Value.Name}: ");

            if (outputFile.Key == AsmEnum.FileTypeEnum.LST)
            {
                // テキスト比較
                var originals = AILight.AIEncode.GetString(original).Replace("\r", "").Split('\n');
                var assembleds = AILight.AIEncode.GetString(assembled).Replace("\r", "").Split('\n');
                if (originals.Length != assembleds.Length)
                {
                    resultString = $"不一致 {originals.Length:0} -> {assembleds.Length:0} 行数";
                    resultString += $"{Environment.NewLine}";
                    result = false;
                }
                else
                {
                    var byteDiffCounter = 0;
                    var tmpResultStream = "";

                    for (var index = 0; index < originals.Length && index < assembleds.Length; index++)
                    {
                        if (originals[index] != assembleds[index])
                        {
                            if (byteDiffCounter < 100)
                            {
                                tmpResultStream += $"{Environment.NewLine}{index + 1:#0}: {originals[index]}";
                                tmpResultStream += $"{Environment.NewLine}{index + 1:#0}: {assembleds[index]}";
                            }
                            byteDiffCounter++;
                        }
                    }
                    if (byteDiffCounter > 0)
                    {
                        resultString = $"不一致 ( {byteDiffCounter:0}件 / 全体:{originals.Length:0}行 )" + tmpResultStream;
                        resultString += $"{Environment.NewLine}";
                        result = false;
                    }
                }


                AILight.AIEncode.IsSHIFT_JIS(assembled);

            }
            else
            {
                // バイナリー比較
                if (original.Length != assembled.Length)
                {
                    resultString = $"不一致 {original.Length:0} -> {assembled.Length:0} bytes";
                    resultString += $"{Environment.NewLine}";
                    result = false;
                }
                else
                {
                    var byteDiffCounter = 0;
                    var tmpResultStream = "";
                    for (var index = 0; index < original.Length; index++)
                    {
                        if (original[index] != assembled[index])
                        {
                            if (byteDiffCounter < 100)
                            {
                                tmpResultStream += $"{Environment.NewLine}{index:X6}: {original[index]:X2} -> {assembled[index]:X2}";
                            }
                            byteDiffCounter++;
                        }
                    }
                    if (byteDiffCounter > 0)
                    {
                        resultString = $"不一致 ( {byteDiffCounter:0}件 / 全体:{original.Length:0} bytes )" + tmpResultStream;
                        resultString += $"{Environment.NewLine}";
                        result = false;
                    }
                }
            }

            Trace.WriteLine(resultString);
            return result;
        }

    }
}
