using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AILZ80ASM.Test
{
    public static class Lib
    {
        private const int READ_BUFFER_LENGTH = 3;

        public static void AreSame(Stream expectedStream, Stream actualStream)
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

        public static ErrorFileInfoMessage[] Assemble(FileInfo[] Files, Stream assebledStream)
        {
            var package = new Package(Files);

            package.Assemble();

            package.Save(assebledStream);

            return package.Errors;
        }


        public static void Assemble_AreSame(FileInfo[] inputFiles, FileInfo outputFile)
        {
            using var memoryStream = new MemoryStream();
            using var outputStream = outputFile.OpenRead();

            Lib.Assemble(inputFiles, memoryStream);
            memoryStream.Position = 0;
            Lib.AreSame(outputStream, memoryStream);
        }


        public static void Assemble_AreSame(string directoryName)
        {
            var targetDirectoryName = Path.Combine(".", "Test", directoryName);

            var inputFiles = new[] { new FileInfo(Path.Combine(targetDirectoryName, "Test.Z80")) };
            var outputFile = new FileInfo(Path.Combine(targetDirectoryName, "Test.BIN"));

            Lib.Assemble_AreSame(inputFiles, outputFile);
        }
    }
}
