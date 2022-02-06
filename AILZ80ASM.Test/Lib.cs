using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace AILZ80ASM.Test
{
    public static class Lib
    {
        private const int READ_BUFFER_LENGTH = 3;

        public static void AreSameBin(Stream expectedStream, Stream actualStream)
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
                    Assert.AreEqual(expected, actual, $"Address:{address:X8} expect:{expected:X2} actual:{actual:X2}");
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

        public static void AreSameLst(Stream expectedStream, Stream actualStream)
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
                    Assert.AreEqual(expected, actual, $"Line:{line} expect:{expected} actual:{actual}");
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

        public static ErrorLineItem[] Assemble(FileInfo[] Files, Stream assebledStream, Stream assembledList, bool dataTrim, bool testError)
        {
            var package = new Package(Files, AsmLoad.EncodeModeEnum.UTF_8, AsmLoad.ListModeEnum.Full, dataTrim, default(Error.ErrorCodeEnum[]), AsmISA.Z80);

            package.Assemble();

            if (package.Errors.Length == 0)
            {
                package.SaveOutput(assebledStream, new System.Collections.Generic.KeyValuePair<AsmLoad.OutputModeEnum, FileInfo>(AsmLoad.OutputModeEnum.BIN, new FileInfo("Main.bin")));
                package.SaveOutput(assembledList, new System.Collections.Generic.KeyValuePair<AsmLoad.OutputModeEnum, FileInfo>(AsmLoad.OutputModeEnum.LST, new FileInfo("Main.lst")));
            }
            else if (!testError)
            {
                throw new Exception(package.Errors[0].ErrorMessage);
            }

            return package.Errors;
        }


        public static void Assemble_AreSame(FileInfo[] inputFiles, FileInfo outputFile, FileInfo listFile)
        {
            Assemble_AreSame(inputFiles, outputFile, listFile, false);
        }

        public static void Assemble_AreSame(FileInfo[] inputFiles, FileInfo outputFile, FileInfo listFile, bool dataTrim)
        {
            using var outputBinMemoryStream = new MemoryStream();
            using var outputLstMemoryStream = new MemoryStream();
            using var outputStream = outputFile.OpenRead();

            Lib.Assemble(inputFiles, outputBinMemoryStream, outputLstMemoryStream, dataTrim, false);
            outputBinMemoryStream.Position = 0;
            outputLstMemoryStream.Position = 0;
            Lib.AreSameBin(outputStream, outputBinMemoryStream);

            /*
            if (listFile.Exists)
            {
                using var listStream = listFile.OpenRead();
                Lib.AreSameLst(listStream, outputLstMemoryStream);
            }
            else
            {
                using var listStream = listFile.OpenWrite();
                outputLstMemoryStream.CopyTo(listStream);
            }
            */
        }

        public static void Assemble_AreSame(string directoryName)
        {
            Assemble_AreSame(directoryName, false);
        }

        public static void Assemble_AreSame(string directoryName, bool dataTrim)
        {
            var targetDirectoryName = Path.Combine(".", "Test", directoryName);

            var inputFiles = new[] { new FileInfo(Path.Combine(targetDirectoryName, "Test.Z80")) };
            var outputFile = new FileInfo(Path.Combine(targetDirectoryName, "Test.BIN"));

            var listFile = new FileInfo(Path.Combine(targetDirectoryName, "Test.LST"));

            Lib.Assemble_AreSame(inputFiles, outputFile, listFile, dataTrim);
        }
    }
}
