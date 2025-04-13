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

        public static void AreSameBin(Stream expectedStream, Stream actualStream, AsmEnum.FileTypeEnum fileType)
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
                    Assert.AreEqual(expected, actual, $"{fileType} Address:{address:X8} expect:{expected:X2} actual:{actual:X2}");
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

        public static void AreSameLst(Stream expectedStream, Stream actualStream, AsmEnum.FileTypeEnum fileType)
        {
            switch (fileType)
            {
                case AsmEnum.FileTypeEnum.BIN:
                case AsmEnum.FileTypeEnum.T88:
                case AsmEnum.FileTypeEnum.CMT:
                case AsmEnum.FileTypeEnum.MZT:
                    Assert.Fail("テキストファイルではありません");
                    break;
                default:
                    break;
            }

            if (fileType == AsmEnum.FileTypeEnum.HEX)
            {
                var expectedReadByte = expectedStream.ReadByte();
                var actualReadByte = actualStream.ReadByte();
                if ((expectedReadByte != -1 && !char.IsAscii((char)expectedReadByte)) ||
                    (actualReadByte != -1 && !char.IsAscii((char)actualReadByte)))
                {
                    Assert.Fail("ASCIIエンコードのファイルではありません");
                }

                expectedStream.Seek(0, SeekOrigin.Begin);
                actualStream.Seek(0, SeekOrigin.Begin);
            }

            using var expectedStreamReader = new StreamReader(expectedStream);
            using var actualStreamReader = new StreamReader(actualStream);

            var line = 1;
            while (!expectedStreamReader.EndOfStream && !actualStreamReader.EndOfStream)
            {
                var expected = expectedStreamReader.ReadLine();
                var actual = actualStreamReader.ReadLine();

                if ((fileType == AsmEnum.FileTypeEnum.HEX) ||
                    (line > 1 && fileType != AsmEnum.FileTypeEnum.ERR) ||
                    (line > 2 && fileType == AsmEnum.FileTypeEnum.ERR))
                {
                    Assert.AreEqual(expected, actual, $"{fileType} Line:{line} expect:{expected} actual:{actual}");
                }

                line++;
            }

            if (!expectedStreamReader.EndOfStream || !expectedStreamReader.EndOfStream)
            {
                var expected = expectedStreamReader.EndOfStream ? "" : expectedStreamReader.ReadLine();
                var actual = expectedStreamReader.EndOfStream ? "" : actualStreamReader.ReadLine();

                Assert.AreEqual(expected, actual, $"Line:{line} expect:{expected} actual:{actual}");
            }
        }

        public static ErrorLineItem[] Assemble(FileInfo[] files, Dictionary<MemoryStream, KeyValuePair<AsmEnum.FileTypeEnum, FileInfo>> outputFiles, bool testError)
        {
            var asmOption = new AsmOption();
            asmOption.InputFiles = new Dictionary<AsmEnum.FileTypeEnum, FileInfo[]>()
            {
                [AsmEnum.FileTypeEnum.Z80] = files,
            };
            asmOption.DisableWarningCodes = new[]
            {
                Error.ErrorCodeEnum.I0002
            };
            asmOption.InputEncodeMode = AsmEnum.EncodeModeEnum.UTF_8;
            asmOption.ListMode = AsmEnum.ListFormatEnum.Full;

            var package = new Package(asmOption, AsmISA.Z80);
            if (package.Errors.Length == 0)
            {
                package.Assemble();

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

        public static ErrorLineItem[] Assemble(string direcotryName, string fileName)
        {
            var targetDirectoryName = Path.Combine(".", "Test", direcotryName);
            var inputFiles = new[] { new FileInfo(Path.Combine(targetDirectoryName, fileName)) };
            var outputFiles = new System.Collections.Generic.Dictionary<MemoryStream, System.Collections.Generic.KeyValuePair<Assembler.AsmEnum.FileTypeEnum, FileInfo>>();

            return Lib.Assemble(inputFiles, outputFiles, true);
        }

        public static ErrorLineItem[] Assemble_AreSame(FileInfo[] inputFiles, Dictionary<AsmEnum.FileTypeEnum, FileInfo> outputFiles)
        {
            var memoryStreamFiles = new Dictionary<MemoryStream, KeyValuePair<AsmEnum.FileTypeEnum, FileInfo>>();
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
                var errors = Lib.Assemble(inputFiles, memoryStreamFiles, false);

                // アセンブル結果の比較
                foreach (var file in memoryStreamFiles)
                {
                    file.Key.Position = 0;
                    switch (AsmLoad.GetFileType(file.Value.Key))
                    {
                        case AsmEnum.FileDataTypeEnum.Binary:
                            using (var outputStream = file.Value.Value.OpenRead())
                            {
                                Lib.AreSameBin(file.Key, outputStream, file.Value.Key);
                            }
                            break;
                        case AsmEnum.FileDataTypeEnum.Text:
                            using (var outputStream = file.Value.Value.OpenRead())
                            {
                                Lib.AreSameLst(file.Key, outputStream, file.Value.Key);
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

        public static ErrorLineItem[] Assemble_AreSame(params string[] directoryNames)
        {
            var directories = (new[] { ".", "Test" }).Union(directoryNames).ToArray();
            var targetDirectoryName = Path.Combine(directories);
            var inputFiles = new[] { new FileInfo(Path.Combine(targetDirectoryName, "Test.Z80")) };
            var outputFiles = Enum.GetValues<AsmEnum.FileTypeEnum>().Where(m => m != AsmEnum.FileTypeEnum.Z80)
                                  .ToDictionary(m => m, n => new FileInfo(Path.Combine(targetDirectoryName, $"Test.{n}")));

            return Lib.Assemble_AreSame(inputFiles, outputFiles);
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
