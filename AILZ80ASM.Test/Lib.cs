using AILZ80ASM.Assembler;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AILZ80ASM.Test
{
    public static class Lib
    {
        private const int READ_BUFFER_LENGTH = 3;

        public static void AreSameBin(Stream expectedStream, Stream actualStream, AsmLoad.OutputModeEnum outputMode)
        {
            var expectedBytes = new byte[READ_BUFFER_LENGTH];
            var actualBytes = new byte[READ_BUFFER_LENGTH];
            var address = default(UInt32);

            Read(expectedStream, actualStream, expectedBytes, actualBytes, out var expectedReadLength, out var actualReadLength, out var index);

            while (expectedReadLength > 0 || actualReadLength > 0)
            {
                while (expectedReadLength > index || actualReadLength > index)
                {
                    var expected = default(byte?);
                    var actual = default(byte?);
                    if (expectedReadLength > index)
                    {
                        expected = expectedBytes[index];
                    }
                    if (actualReadLength > index)
                    {
                        actual = actualBytes[index];
                    }
                    Assert.AreEqual(expected, actual, $"{outputMode} Address:{address:X8} expect:{expected:X2} actual:{actual:X2}");
                    address++;
                    index++;
                }

                Read(expectedStream, actualStream, expectedBytes, actualBytes, out expectedReadLength, out actualReadLength, out index);
            }

            static void Read(Stream expectedStream, Stream actualStream, byte[] expectedBytes, byte[] actualBytes, out int expectedReadLength, out int actualReadLength, out int index)
            {
                expectedReadLength = expectedStream.Read(expectedBytes, 0, expectedBytes.Length);
                actualReadLength = actualStream.Read(actualBytes, 0, actualBytes.Length);
                index = 0;
            }
        }

        public static void AreSameLst(Stream expectedStream, Stream actualStream, AsmLoad.OutputModeEnum outputMode)
        {
            using var expectedStreamReader = new StreamReader(expectedStream);
            using var actualStreamReader = new StreamReader(actualStream);
            var expected = expectedStreamReader.ReadLine();
            var actual = actualStreamReader.ReadLine();
            var line = 1;
            while (!string.IsNullOrEmpty(expected) && !string.IsNullOrEmpty(actual))
            {
                if (line > 1)
                {
                    Assert.AreEqual(expected, actual, $"{outputMode} Line:{line} expect:{expected} actual:{actual}");
                }

                expected = expectedStreamReader.ReadLine();
                actual = actualStreamReader.ReadLine();
                line++;
            }

            if (!string.IsNullOrEmpty(expected) || !string.IsNullOrEmpty(actual))
            {
                Assert.AreEqual(expected, actual, $"Line:{line} expect:{expected} actual:{actual}");
            }
        }

        public static ErrorLineItem[] Assemble(FileInfo[] Files, Dictionary<MemoryStream, KeyValuePair<AsmLoad.OutputModeEnum, FileInfo>> outputFiles, bool dataTrim, bool testError)
        {
            var package = new Package(Files, AsmLoad.EncodeModeEnum.UTF_8, AsmLoad.ListModeEnum.Full, dataTrim, default(Error.ErrorCodeEnum[]), AsmISA.Z80);

            package.Assemble();

            if (package.Errors.Length == 0)
            {
                foreach (var item in outputFiles)
                {
                    package.SaveOutput(item.Key, item.Value);
                }
            }
            else if (!testError)
            {
                throw new Exception(package.Errors[0].ErrorMessage);
            }

            return package.Errors.Union(package.Warnings).Union(package.Information).ToArray();
        }


        public static ErrorLineItem[] Assemble_AreSame(FileInfo[] inputFiles, Dictionary<AsmLoad.OutputModeEnum, FileInfo> outputFiles)
        {
            return Assemble_AreSame(inputFiles, outputFiles, false);
        }

        public static ErrorLineItem[] Assemble_AreSame(FileInfo[] inputFiles, Dictionary<AsmLoad.OutputModeEnum, FileInfo> outputFiles, bool dataTrim)
        {
            var memoryStreamFiles = new Dictionary<MemoryStream, KeyValuePair<AsmLoad.OutputModeEnum, FileInfo>>();
            try
            {
                // テスト項目の積み込み
                foreach (var item in outputFiles)
                {
                    var memoryStream = new MemoryStream();
                    if (item.Value.Exists)
                    {
                        memoryStreamFiles.Add(memoryStream, item);
                    }
                }

                // アセンブル
                var errors = Lib.Assemble(inputFiles, memoryStreamFiles, dataTrim, false);

                // アセンブル結果の比較
                foreach (var file in memoryStreamFiles)
                {
                    file.Key.Position = 0;
                    switch (AsmLoad.GetFileType(file.Value.Key))
                    {
                        case AsmLoad.OutputModeFileTypeEnum.Binary:
                            using (var outputStream = file.Value.Value.OpenRead())
                            {
                                Lib.AreSameBin(outputStream, file.Key, file.Value.Key);
                            }
                            break;
                        case AsmLoad.OutputModeFileTypeEnum.Text:
                            using (var outputStream = file.Value.Value.OpenRead())
                            {
                                Lib.AreSameLst(outputStream, file.Key, file.Value.Key);
                            }
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                }

                return errors;
            }
            finally
            {
                foreach (var item in memoryStreamFiles)
                {
                    item.Key.Dispose();
                }
            }
        }

        public static ErrorLineItem[] Assemble_AreSame(string directoryName)
        {
            return Assemble_AreSame(directoryName, false);
        }

        public static ErrorLineItem[] Assemble_AreSame(string directoryName, bool dataTrim)
        {
            var targetDirectoryName = Path.Combine(".", "Test", directoryName);
            var inputFiles = new[] { new FileInfo(Path.Combine(targetDirectoryName, "Test.Z80")) };
            var outputFiles = Enum.GetValues<AsmLoad.OutputModeEnum>()
                                  .ToDictionary(m => m, n => new FileInfo(Path.Combine(targetDirectoryName, $"Test.{n}")));

            return Lib.Assemble_AreSame(inputFiles, outputFiles, dataTrim);
        }

        public static void AssertErrorItemMessage(Error.ErrorCodeEnum errorCode, int lineIndex, string fileName, ErrorLineItem[] errors)
        {
            if (!errors.Any(m => m.ErrorCode == errorCode && m.LineItem.LineIndex == lineIndex && m.LineItem.FileInfo.Name == fileName))
            {
                Assert.Fail($"指定のエラーが見つかりませんでした。　ErrorCode:{errorCode} LineIndex:{lineIndex}");
            }
        }
    }
}
