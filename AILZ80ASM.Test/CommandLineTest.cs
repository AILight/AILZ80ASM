using AILZ80ASM.Assembler;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AILZ80ASM.Test
{
    [TestClass]
    public class CommandLineTest
    {
        [TestMethod]
        public void Test_CommandLine()
        {
            {
                var rootCommand = AsmCommandLine.SettingRootCommand();
                var arguments = new[] { "-i", "Main.z80", "-o", "Main.bin" };

                Assert.IsTrue(rootCommand.Parse(arguments));
                var fileInfos = rootCommand.GetValue<FileInfo[]>("input");

                Assert.AreEqual(1, fileInfos.Length);
                Assert.AreEqual("Main.z80", fileInfos.First().Name);
                Assert.AreEqual("Main.bin", rootCommand.GetValue<FileInfo>("output").Name);
                Assert.AreEqual("bin", rootCommand.GetValue<string>("outputMode"));

                var outputFiles = rootCommand.GetOutputFiles();
                Assert.AreEqual(1, outputFiles.Count);
                Assert.AreEqual("Main.bin", outputFiles[AsmEnum.FileTypeEnum.BIN].Name);
            }

            {
                var rootCommand = AsmCommandLine.SettingRootCommand();
                var arguments = new[] { "--input", "Main.z80", "--output", "Main.bin" };

                Assert.IsTrue(rootCommand.Parse(arguments));
                var fileInfos = rootCommand.GetValue<FileInfo[]>("input");

                Assert.AreEqual(1, fileInfos.Length);
                Assert.AreEqual("Main.z80", fileInfos.First().Name);
                Assert.AreEqual("Main.bin", rootCommand.GetValue<FileInfo>("output").Name);
                Assert.AreEqual("bin", rootCommand.GetValue<string>("outputMode"));

                var outputFiles = rootCommand.GetOutputFiles();
                Assert.AreEqual(1, outputFiles.Count);
                Assert.AreEqual("Main.bin", outputFiles[AsmEnum.FileTypeEnum.BIN].Name);
            }
        }

        [TestMethod]
        public void Test_CommandLine_Version()
        {
            {
                var rootCommand = AsmCommandLine.SettingRootCommand();
                var arguments = new[] { "-v" };

                Assert.IsFalse(rootCommand.Parse(arguments));
                Assert.IsTrue(Version.TryParse(rootCommand.ParseMessage, out var _));
            }
            {
                var rootCommand = AsmCommandLine.SettingRootCommand();
                var arguments = new[] { "--version" };

                Assert.IsFalse(rootCommand.Parse(arguments));
                Assert.IsTrue(Version.TryParse(rootCommand.ParseMessage, out var _));

            }
        }

        [TestMethod]
        public void Test_CommandLine_EntryPoint()
        {
            {
                var rootCommand = AsmCommandLine.SettingRootCommand();
                var arguments = new[] { "Main.z80" };

                Assert.IsTrue(rootCommand.Parse(arguments));
                Assert.IsNull(rootCommand.GetValue<ushort?>("entryPoint"));
            }

            {
                var rootCommand = AsmCommandLine.SettingRootCommand();
                var arguments = new[] { "Main.z80", "-ep" };

                Assert.IsFalse(rootCommand.Parse(arguments));
                Assert.IsNull(rootCommand.GetValue<ushort?>("entryPoint"));
            }

            {
                var rootCommand = AsmCommandLine.SettingRootCommand();
                var arguments = new[] { "Main.z80", "--entry-point" };

                Assert.IsFalse(rootCommand.Parse(arguments));
                Assert.IsNull(rootCommand.GetValue<ushort?>("entryPoint"));
            }

            {
                var rootCommand = AsmCommandLine.SettingRootCommand();
                var arguments = new[] { "Main.z80", "-ep", "1234" };

                Assert.IsTrue(rootCommand.Parse(arguments));
                Assert.AreEqual((UInt16)1234, rootCommand.GetValue<ushort?>("entryPoint"));
            }

            {
                var rootCommand = AsmCommandLine.SettingRootCommand();
                var arguments = new[] { "Main.z80", "--entry-point", "$1234" };

                Assert.IsTrue(rootCommand.Parse(arguments));
                Assert.AreEqual((UInt16)0x1234, rootCommand.GetValue<ushort?>("entryPoint"));
            }
        }

        [TestMethod]
        public void Test_CommandLine_Force()
        {
            {
                var rootCommand = AsmCommandLine.SettingRootCommand();
                var arguments = new[] { "-v" };

                Assert.IsFalse(rootCommand.Parse(arguments));
                Assert.IsFalse(rootCommand.GetValue<bool>("force"));
            }

            {
                var rootCommand = AsmCommandLine.SettingRootCommand();
                var arguments = new[] { "-f" };

                Assert.IsFalse(rootCommand.Parse(arguments));
                Assert.IsTrue(rootCommand.GetValue<bool>("force"));
            }

            {
                var rootCommand = AsmCommandLine.SettingRootCommand();
                var arguments = new[] { "--force" };

                Assert.IsFalse(rootCommand.Parse(arguments));
                Assert.IsTrue(rootCommand.GetValue<bool>("force"));
            }
        }

        [TestMethod]
        public void Test_CommandLine_NoSuperAssemble()
        {
            {
                var rootCommand = AsmCommandLine.SettingRootCommand();
                var arguments = new[] { "-v" };

                Assert.IsFalse(rootCommand.Parse(arguments));
                Assert.IsFalse(rootCommand.GetValue<bool>("noSuperAssemble"));
            }

            {
                var rootCommand = AsmCommandLine.SettingRootCommand();
                var arguments = new[] { "-nsa" };

                Assert.IsFalse(rootCommand.Parse(arguments));
                Assert.IsTrue(rootCommand.GetValue<bool>("noSuperAssemble"));
            }

            {
                var rootCommand = AsmCommandLine.SettingRootCommand();
                var arguments = new[] { "--no-super-asm" };

                Assert.IsFalse(rootCommand.Parse(arguments));
                Assert.IsTrue(rootCommand.GetValue<bool>("noSuperAssemble"));
            }
        }

        [TestMethod]
        public void Test_CommandLine_Input()
        {
            {
                var rootCommand = AsmCommandLine.SettingRootCommand();
                var arguments = new[] { "Main.z80", "-o", "Main.bin" };

                Assert.IsTrue(rootCommand.Parse(arguments));
                var fileInfos = rootCommand.GetValue<FileInfo[]>("input");

                Assert.AreEqual(1, fileInfos.Length);
                Assert.AreEqual("Main.z80", fileInfos.First().Name);
                Assert.AreEqual("Main.bin", rootCommand.GetValue<FileInfo>("output").Name);
                Assert.AreEqual("bin", rootCommand.GetValue<string>("outputMode"));

                var outputFiles = rootCommand.GetOutputFiles();
                Assert.AreEqual(1, outputFiles.Count);
                Assert.AreEqual("Main.bin", outputFiles[AsmEnum.FileTypeEnum.BIN].Name);
            }

            {
                var rootCommand = AsmCommandLine.SettingRootCommand();
                var arguments = new[] { "-i", "Main.z80", "-o", "Main.bin" };

                Assert.IsTrue(rootCommand.Parse(arguments));
                var fileInfos = rootCommand.GetValue<FileInfo[]>("input");

                Assert.AreEqual(1, fileInfos.Length);
                Assert.AreEqual("Main.z80", fileInfos.First().Name);
                Assert.AreEqual("Main.bin", rootCommand.GetValue<FileInfo>("output").Name);
                Assert.AreEqual("bin", rootCommand.GetValue<string>("outputMode"));

                var outputFiles = rootCommand.GetOutputFiles();
                Assert.AreEqual(1, outputFiles.Count);
                Assert.AreEqual("Main.bin", outputFiles[AsmEnum.FileTypeEnum.BIN].Name);
            }
        }

        [TestMethod]
        public void Test_CommandLine_Inputs()
        {
            var rootCommand = AsmCommandLine.SettingRootCommand();
            var arguments = new[] { "Main.z80", "Main2.z80", "Main3.z80", "-o", "Main.bin" };

            Assert.IsTrue(rootCommand.Parse(arguments));
            var fileInfos = rootCommand.GetValue<FileInfo[]>("input");

            Assert.AreEqual(3, fileInfos.Length);
            Assert.IsTrue(fileInfos.Any(m => m.Name == "Main.z80"));
            Assert.IsTrue(fileInfos.Any(m => m.Name == "Main2.z80"));
            Assert.IsTrue(fileInfos.Any(m => m.Name == "Main3.z80"));
            Assert.AreEqual("Main.bin", rootCommand.GetValue<FileInfo>("output").Name);
            Assert.AreEqual("bin", rootCommand.GetValue<string>("outputMode"));

            var outputFiles = rootCommand.GetOutputFiles();
            Assert.AreEqual(1, outputFiles.Count);
            Assert.AreEqual("Main.bin", outputFiles[AsmEnum.FileTypeEnum.BIN].Name);
        }

        [TestMethod]
        public void Test_CommandLine_Encode()
        {
            {
                var rootCommand = AsmCommandLine.SettingRootCommand();
                var arguments = new[] { "Main.z80", "-o", "Main.bin" };

                Assert.IsTrue(rootCommand.Parse(arguments));
                var fileInfos = rootCommand.GetValue<FileInfo[]>("input");

                Assert.AreEqual(1, fileInfos.Length);
                Assert.AreEqual("Main.z80", fileInfos.First().Name);
                Assert.AreEqual("Main.bin", rootCommand.GetValue<FileInfo>("output").Name);
                Assert.AreEqual("bin", rootCommand.GetValue<string>("outputMode"));
                Assert.AreEqual("auto", rootCommand.GetValue<string>("inputEncode"));

                var outputFiles = rootCommand.GetOutputFiles();
                Assert.AreEqual(1, outputFiles.Count);
                Assert.AreEqual("Main.bin", outputFiles[AsmEnum.FileTypeEnum.BIN].Name);
                Assert.AreEqual(AsmEnum.EncodeModeEnum.AUTO, rootCommand.GetInputEncodeMode());
            }

            {
                var rootCommand = AsmCommandLine.SettingRootCommand();
                var arguments = new[] { "Main.z80", "-o", "Main.bin", "-ie", "UTF-8", "-oe", "SHIFT_JIS" };

                Assert.IsTrue(rootCommand.Parse(arguments));
                var fileInfos = rootCommand.GetValue<FileInfo[]>("input");

                Assert.AreEqual(1, fileInfos.Length);
                Assert.AreEqual(fileInfos.First().Name, "Main.z80");
                Assert.AreEqual("utf-8", rootCommand.GetValue<string>("inputEncode"));
                Assert.AreEqual("Main.bin", rootCommand.GetValue<FileInfo>("output").Name);
                Assert.AreEqual("bin", rootCommand.GetValue<string>("outputMode"));
                Assert.AreEqual("shift_jis", rootCommand.GetValue<string>("outputEncode"));

                var outputFiles = rootCommand.GetOutputFiles();
                Assert.AreEqual(1, outputFiles.Count);
                Assert.AreEqual("Main.bin", outputFiles[AsmEnum.FileTypeEnum.BIN].Name);
                Assert.AreEqual(AsmEnum.EncodeModeEnum.UTF_8, rootCommand.GetInputEncodeMode());
                Assert.AreEqual(AsmEnum.EncodeModeEnum.SHIFT_JIS, rootCommand.GetOutputEncodeMode());
            }

            {
                var rootCommand = AsmCommandLine.SettingRootCommand();
                var arguments = new[] { "Main.z80", "-o", "Main.bin", "-oe", "SHIFT_JIS" };

                Assert.IsTrue(rootCommand.Parse(arguments));
                var fileInfos = rootCommand.GetValue<FileInfo[]>("input");

                Assert.AreEqual(1, fileInfos.Length);
                Assert.AreEqual("Main.z80", fileInfos.First().Name);
                Assert.AreEqual("Main.bin", rootCommand.GetValue<FileInfo>("output").Name);
                Assert.AreEqual("bin", rootCommand.GetValue<string>("outputMode"));
                Assert.AreEqual("shift_jis", rootCommand.GetValue<string>("outputEncode"));

                var outputFiles = rootCommand.GetOutputFiles();
                Assert.AreEqual(1, outputFiles.Count);
                Assert.AreEqual("Main.bin", outputFiles[AsmEnum.FileTypeEnum.BIN].Name);
                Assert.AreEqual(AsmEnum.EncodeModeEnum.SHIFT_JIS, rootCommand.GetOutputEncodeMode());
            }
        }

        [TestMethod]
        public void Test_CommandLine_Bin()
        {
            {
                var rootCommand = AsmCommandLine.SettingRootCommand();
                var arguments = new[] { "Main.z80", "-o", "test.bin", "-om", "BIN" };

                Assert.IsTrue(rootCommand.Parse(arguments));
                var fileInfos = rootCommand.GetValue<FileInfo[]>("input");

                Assert.AreEqual(1, fileInfos.Length);
                Assert.AreEqual("Main.z80", fileInfos.First().Name);
                Assert.AreEqual("test.bin", rootCommand.GetValue<FileInfo>("output").Name);
                Assert.AreEqual("bin", rootCommand.GetValue<string>("outputMode"));

                var outputFiles = rootCommand.GetOutputFiles();
                Assert.AreEqual(1, outputFiles.Count);
                Assert.AreEqual("test.bin", outputFiles[AsmEnum.FileTypeEnum.BIN].Name);
            }

            {
                var rootCommand = AsmCommandLine.SettingRootCommand();
                var arguments = new[] { "Main.z80" };

                Assert.IsTrue(rootCommand.Parse(arguments));
                var fileInfos = rootCommand.GetValue<FileInfo[]>("input");

                Assert.AreEqual(1, fileInfos.Length);
                Assert.AreEqual("Main.z80", fileInfos.First().Name);
                Assert.AreEqual("Main.bin", rootCommand.GetValue<FileInfo>("output").Name);
                Assert.AreEqual("bin", rootCommand.GetValue<string>("outputMode"));

                var outputFiles = rootCommand.GetOutputFiles();
                Assert.AreEqual(1, outputFiles.Count);
                Assert.AreEqual("Main.bin", outputFiles[AsmEnum.FileTypeEnum.BIN].Name);
            }

            {
                var rootCommand = AsmCommandLine.SettingRootCommand();
                var arguments = new[] { "Main.z80", "-bin" };

                Assert.IsTrue(rootCommand.Parse(arguments));
                var fileInfos = rootCommand.GetValue<FileInfo[]>("input");

                Assert.AreEqual(1, fileInfos.Length);
                Assert.AreEqual("Main.z80", fileInfos.First().Name);
                Assert.AreEqual("Main.bin", rootCommand.GetValue<FileInfo>("output").Name);
                Assert.AreEqual("bin", rootCommand.GetValue<string>("outputMode"));

                var outputFiles = rootCommand.GetOutputFiles();
                Assert.AreEqual(1, outputFiles.Count);
                Assert.AreEqual("Main.bin", outputFiles[AsmEnum.FileTypeEnum.BIN].Name);
            }

            {
                var rootCommand = AsmCommandLine.SettingRootCommand();
                var arguments = new[] { "Main.z80", "-bin", "Test.bin" };

                Assert.IsTrue(rootCommand.Parse(arguments));
                var fileInfos = rootCommand.GetValue<FileInfo[]>("input");

                Assert.AreEqual(1, fileInfos.Length);
                Assert.AreEqual("Main.z80", fileInfos.First().Name);
                Assert.AreEqual("Main.bin", rootCommand.GetValue<FileInfo>("output").Name);
                Assert.AreEqual("bin", rootCommand.GetValue<string>("outputMode"));             // デフォルト値が設定
                Assert.AreEqual("Test.bin", rootCommand.GetValue<FileInfo>("outputBin").Name);
                Assert.IsFalse(rootCommand.GetSelected("output"));
                Assert.IsFalse(rootCommand.GetSelected("outputMode"));

                var outputFiles = rootCommand.GetOutputFiles();
                Assert.AreEqual(1, outputFiles.Count);
                Assert.AreEqual("Test.bin", outputFiles[AsmEnum.FileTypeEnum.BIN].Name);
            }
        }

        [TestMethod]
        public void Test_CommandLine_CMT()
        {
            {
                var rootCommand = AsmCommandLine.SettingRootCommand();
                var arguments = new[] { "Main.z80", "-o", "test.cmt", "-om", "cmt" };

                Assert.IsTrue(rootCommand.Parse(arguments));
                var fileInfos = rootCommand.GetValue<FileInfo[]>("input");

                Assert.AreEqual(1, fileInfos.Length);
                Assert.AreEqual("Main.z80", fileInfos.First().Name);
                Assert.AreEqual("test.cmt", rootCommand.GetValue<FileInfo>("output").Name);
                Assert.AreEqual("cmt", rootCommand.GetValue<string>("outputMode"));

                var outputFiles = rootCommand.GetOutputFiles();
                Assert.AreEqual(1, outputFiles.Count);
                Assert.AreEqual("test.cmt", outputFiles[AsmEnum.FileTypeEnum.CMT].Name);
            }

            {
                var rootCommand = AsmCommandLine.SettingRootCommand();
                var arguments = new[] { "Main.z80", "-cmt" };

                Assert.IsTrue(rootCommand.Parse(arguments));
                var fileInfos = rootCommand.GetValue<FileInfo[]>("input");

                Assert.AreEqual(1, fileInfos.Length);
                Assert.AreEqual("Main.z80", fileInfos.First().Name);
                Assert.AreEqual("Main.cmt", rootCommand.GetValue<FileInfo>("output").Name);
                Assert.AreEqual("cmt", rootCommand.GetValue<string>("outputMode"));

                var outputFiles = rootCommand.GetOutputFiles();
                Assert.AreEqual(1, outputFiles.Count);
                Assert.AreEqual("Main.cmt", outputFiles[AsmEnum.FileTypeEnum.CMT].Name);
            }

            {
                var rootCommand = AsmCommandLine.SettingRootCommand();
                var arguments = new[] { "Main.z80", "-cmt", "Main.cmt" };

                Assert.IsTrue(rootCommand.Parse(arguments));
                var fileInfos = rootCommand.GetValue<FileInfo[]>("input");

                Assert.AreEqual(1, fileInfos.Length);
                Assert.AreEqual("Main.z80", fileInfos.First().Name);
                Assert.AreEqual("Main.bin", rootCommand.GetValue<FileInfo>("output").Name);
                Assert.AreEqual("bin", rootCommand.GetValue<string>("outputMode"));             // デフォルト値が設定
                Assert.AreEqual("Main.cmt", rootCommand.GetValue<FileInfo>("outputCMT").Name);
                Assert.IsFalse(rootCommand.GetSelected("output"));
                Assert.IsFalse(rootCommand.GetSelected("outputMode"));

                var outputFiles = rootCommand.GetOutputFiles();
                Assert.AreEqual(1, outputFiles.Count);
                Assert.AreEqual("Main.cmt", outputFiles[AsmEnum.FileTypeEnum.CMT].Name);
            }
        }

        [TestMethod]
        public void Test_CommandLine_Tag()
        {
            {
                var rootCommand = AsmCommandLine.SettingRootCommand();
                var arguments = new[] { "Main.z80", "-o", "test.bin", "-om", "BIN" };

                Assert.IsTrue(rootCommand.Parse(arguments));
                var fileInfos = rootCommand.GetValue<FileInfo[]>("input");

                Assert.AreEqual(1, fileInfos.Length);
                Assert.AreEqual("Main.z80", fileInfos.First().Name);
                Assert.AreEqual("test.bin", rootCommand.GetValue<FileInfo>("output").Name);
                Assert.AreEqual("bin", rootCommand.GetValue<string>("outputMode"));

                var outputFiles = rootCommand.GetOutputFiles();
                Assert.AreEqual(1, outputFiles.Count);
                Assert.AreEqual("test.bin", outputFiles[AsmEnum.FileTypeEnum.BIN].Name);
            }

            {
                var rootCommand = AsmCommandLine.SettingRootCommand();
                var arguments = new[] { "Main.z80", "-bin", "-tag" };

                Assert.IsTrue(rootCommand.Parse(arguments));
                var fileInfos = rootCommand.GetValue<FileInfo[]>("input");

                Assert.AreEqual(1, fileInfos.Length);
                Assert.AreEqual("Main.z80", fileInfos.First().Name);
                Assert.AreEqual("Main.bin", rootCommand.GetValue<FileInfo>("output").Name);
                Assert.AreEqual("bin", rootCommand.GetValue<string>("outputMode"));

                var outputFiles = rootCommand.GetOutputFiles();
                Assert.AreEqual(2, outputFiles.Count);
                Assert.AreEqual("Main.bin", outputFiles[AsmEnum.FileTypeEnum.BIN].Name);
                Assert.AreEqual("tags", outputFiles[AsmEnum.FileTypeEnum.TAG].Name);
            }

            {
                var rootCommand = AsmCommandLine.SettingRootCommand();
                var arguments = new[] { "Main.z80", "-bin", "-tag"};

                Assert.IsTrue(rootCommand.Parse(arguments));
                var fileInfos = rootCommand.GetValue<FileInfo[]>("input");

                Assert.AreEqual(fileInfos.Length, 1);
                Assert.AreEqual("Main.z80", fileInfos.First().Name);
                Assert.AreEqual("Main.bin", rootCommand.GetValue<FileInfo>("output").Name);
                Assert.AreEqual("bin", rootCommand.GetValue<string>("outputMode"));

                var outputFiles = rootCommand.GetOutputFiles();
                Assert.AreEqual(2, outputFiles.Count);
                Assert.AreEqual("Main.bin", outputFiles[AsmEnum.FileTypeEnum.BIN].Name);
                Assert.AreEqual("tags", outputFiles[AsmEnum.FileTypeEnum.TAG].Name);
            }

            {
                var rootCommand = AsmCommandLine.SettingRootCommand();
                var arguments = new[] { "Main.z80", "-bin", "Test.bin", "-tag", "Test.tag" };

                Assert.IsTrue(rootCommand.Parse(arguments));
                var fileInfos = rootCommand.GetValue<FileInfo[]>("input");

                Assert.AreEqual(1, fileInfos.Length);
                Assert.AreEqual("Main.z80", fileInfos.First().Name);
                Assert.AreEqual("Main.bin", rootCommand.GetValue<FileInfo>("output").Name);
                Assert.AreEqual("bin", rootCommand.GetValue<string>("outputMode"));             // デフォルト値が設定
                Assert.AreEqual("Test.bin", rootCommand.GetValue<FileInfo>("outputBin").Name);
                Assert.IsFalse(rootCommand.GetSelected("output"));
                Assert.IsFalse(rootCommand.GetSelected("outputMode"));

                var outputFiles = rootCommand.GetOutputFiles();
                Assert.AreEqual(2, outputFiles.Count);
                Assert.AreEqual("Test.bin", outputFiles[AsmEnum.FileTypeEnum.BIN].Name);
                Assert.AreEqual("Test.tag", outputFiles[AsmEnum.FileTypeEnum.TAG].Name);
            }
        }

        [TestMethod]
        public void Test_CommandLine_T88()
        {
            {
                var rootCommand = AsmCommandLine.SettingRootCommand();
                var arguments = new[] { "Main.z80", "-o", "test.t88", "-om", "T88" };

                Assert.IsTrue(rootCommand.Parse(arguments));
                var fileInfos = rootCommand.GetValue<FileInfo[]>("input");

                Assert.AreEqual(1, fileInfos.Length);
                Assert.AreEqual("Main.z80", fileInfos.First().Name);
                Assert.AreEqual("test.t88", rootCommand.GetValue<FileInfo>("output").Name);
                Assert.AreEqual("t88", rootCommand.GetValue<string>("outputMode"));

                var outputFiles = rootCommand.GetOutputFiles();
                Assert.AreEqual(1, outputFiles.Count);
                Assert.AreEqual("test.t88", outputFiles[AsmEnum.FileTypeEnum.T88].Name);
            }

            {
                var rootCommand = AsmCommandLine.SettingRootCommand();
                var arguments = new[] { "Main.z80", "-t88" };

                Assert.IsTrue(rootCommand.Parse(arguments));
                var fileInfos = rootCommand.GetValue<FileInfo[]>("input");

                Assert.AreEqual(fileInfos.Length, 1);
                Assert.AreEqual(fileInfos.First().Name, "Main.z80");
                Assert.AreEqual(rootCommand.GetValue<FileInfo>("output").Name, "Main.t88");
                Assert.AreEqual(rootCommand.GetValue<string>("outputMode"), "t88");

                var outputFiles = rootCommand.GetOutputFiles();
                Assert.AreEqual(outputFiles.Count, 1);
                Assert.AreEqual(outputFiles[AsmEnum.FileTypeEnum.T88].Name, "Main.t88");
            }

            {
                var rootCommand = AsmCommandLine.SettingRootCommand();
                var arguments = new[] { "Main.z80", "-t88", "Main.t88" };

                Assert.IsTrue(rootCommand.Parse(arguments));
                var fileInfos = rootCommand.GetValue<FileInfo[]>("input");

                Assert.AreEqual(1, fileInfos.Length);
                Assert.AreEqual("Main.z80", fileInfos.First().Name);
                Assert.AreEqual("Main.bin", rootCommand.GetValue<FileInfo>("output").Name);
                Assert.AreEqual("bin", rootCommand.GetValue<string>("outputMode"));             // デフォルト値が設定
                Assert.AreEqual("Main.t88", rootCommand.GetValue<FileInfo>("outputT88").Name);
                Assert.IsFalse(rootCommand.GetSelected("output"));
                Assert.IsFalse(rootCommand.GetSelected("outputMode"));

                var outputFiles = rootCommand.GetOutputFiles();
                Assert.AreEqual(1, outputFiles.Count);
                Assert.AreEqual("Main.t88", outputFiles[AsmEnum.FileTypeEnum.T88].Name);
            }
        }

        [TestMethod]
        public void Test_CommandLine_Hex()
        {
            {
                var rootCommand = AsmCommandLine.SettingRootCommand();
                var arguments = new[] { "Main.z80", "-o", "test.hex", "-om", "HEX" };

                Assert.IsTrue(rootCommand.Parse(arguments));
                var fileInfos = rootCommand.GetValue<FileInfo[]>("input");

                Assert.AreEqual(1, fileInfos.Length);
                Assert.AreEqual("Main.z80", fileInfos.First().Name);
                Assert.AreEqual("test.hex", rootCommand.GetValue<FileInfo>("output").Name);
                Assert.AreEqual("hex", rootCommand.GetValue<string>("outputMode"));

                var outputFiles = rootCommand.GetOutputFiles();
                Assert.AreEqual(1, outputFiles.Count);
                Assert.AreEqual("test.hex", outputFiles[AsmEnum.FileTypeEnum.HEX].Name);
            }

            {
                var rootCommand = AsmCommandLine.SettingRootCommand();
                var arguments = new[] { "Main.z80", "-hex" };

                Assert.IsTrue(rootCommand.Parse(arguments));
                var fileInfos = rootCommand.GetValue<FileInfo[]>("input");

                Assert.AreEqual(1, fileInfos.Length);
                Assert.AreEqual("Main.z80", fileInfos.First().Name);
                Assert.AreEqual("Main.hex", rootCommand.GetValue<FileInfo>("output").Name);
                Assert.AreEqual("hex", rootCommand.GetValue<string>("outputMode"));

                var outputFiles = rootCommand.GetOutputFiles();
                Assert.AreEqual(1, outputFiles.Count);
                Assert.AreEqual("Main.hex", outputFiles[AsmEnum.FileTypeEnum.HEX].Name);
            }

            {
                var rootCommand = AsmCommandLine.SettingRootCommand();
                var arguments = new[] { "Main.z80", "-hex", "Main.hex" };

                Assert.IsTrue(rootCommand.Parse(arguments));
                var fileInfos = rootCommand.GetValue<FileInfo[]>("input");

                Assert.AreEqual(1, fileInfos.Length);
                Assert.AreEqual("Main.z80", fileInfos.First().Name);
                Assert.AreEqual("Main.bin", rootCommand.GetValue<FileInfo>("output").Name);
                Assert.AreEqual("bin", rootCommand.GetValue<string>("outputMode"));             // デフォルト値が設定
                Assert.AreEqual("Main.hex", rootCommand.GetValue<FileInfo>("outputHex").Name);
                Assert.IsFalse(rootCommand.GetSelected("output"));
                Assert.IsFalse(rootCommand.GetSelected("outputMode"));

                var outputFiles = rootCommand.GetOutputFiles();
                Assert.AreEqual(1, outputFiles.Count);
                Assert.AreEqual("Main.hex", outputFiles[AsmEnum.FileTypeEnum.HEX].Name);
            }
        }

        [TestMethod]
        public void Test_CommandLine_Bin_CMT_T88()
        {
            {
                var rootCommand = AsmCommandLine.SettingRootCommand();
                var arguments = new[] { "Main.z80", "-bin", "-cmt", "-t88" };

                Assert.IsTrue(rootCommand.Parse(arguments));
                var fileInfos = rootCommand.GetValue<FileInfo[]>("input");

                Assert.AreEqual(1, fileInfos.Length);
                Assert.AreEqual("Main.z80", fileInfos.First().Name);
                Assert.AreEqual("Main.bin", rootCommand.GetValue<FileInfo>("output").Name);
                Assert.AreEqual("bin", rootCommand.GetValue<string>("outputMode"));
                Assert.AreEqual("Main.cmt", rootCommand.GetValue<FileInfo>("outputCMT").Name);
                Assert.AreEqual("Main.t88", rootCommand.GetValue<FileInfo>("outputT88").Name);
                Assert.IsFalse(rootCommand.GetSelected("output"));
                Assert.IsTrue(rootCommand.GetSelected("outputMode"));

                var outputFiles = rootCommand.GetOutputFiles();
                Assert.AreEqual(3, outputFiles.Count);
                Assert.AreEqual("Main.bin", outputFiles[AsmEnum.FileTypeEnum.BIN].Name);
                Assert.AreEqual("Main.cmt", outputFiles[AsmEnum.FileTypeEnum.CMT].Name);
                Assert.AreEqual("Main.t88", outputFiles[AsmEnum.FileTypeEnum.T88].Name);
            }
        }

        [TestMethod]
        public void Test_CommandLine_List()
        {
            {
                var rootCommand = AsmCommandLine.SettingRootCommand();
                var arguments = new[] { "Main.z80", "-bin", "-lst" };

                Assert.IsTrue(rootCommand.Parse(arguments));
                var fileInfos = rootCommand.GetValue<FileInfo[]>("input");

                Assert.AreEqual(1, fileInfos.Length);
                Assert.AreEqual("Main.z80", fileInfos.First().Name);

                var outputFiles = rootCommand.GetOutputFiles();
                Assert.AreEqual(2, outputFiles.Count);
                Assert.AreEqual("Main.bin", outputFiles[AsmEnum.FileTypeEnum.BIN].Name);
                Assert.AreEqual("Main.lst", outputFiles[AsmEnum.FileTypeEnum.LST].Name);

                var listMode = rootCommand.GetListMode();
                Assert.AreEqual(listMode, AsmEnum.ListFormatEnum.Full);
            }

            {
                var rootCommand = AsmCommandLine.SettingRootCommand();
                var arguments = new[] { "Main.z80", "-bin", "-lst", "-lm", "simple" };

                Assert.IsTrue(rootCommand.Parse(arguments));
                var fileInfos = rootCommand.GetValue<FileInfo[]>("input");

                Assert.AreEqual(1, fileInfos.Length);
                Assert.AreEqual("Main.z80", fileInfos.First().Name);

                var outputFiles = rootCommand.GetOutputFiles();
                Assert.AreEqual(2, outputFiles.Count);
                Assert.AreEqual("Main.bin", outputFiles[AsmEnum.FileTypeEnum.BIN].Name);
                Assert.AreEqual("Main.lst", outputFiles[AsmEnum.FileTypeEnum.LST].Name);

                var listMode = rootCommand.GetListMode();
                Assert.AreEqual(listMode, AsmEnum.ListFormatEnum.Simple);
            }

            {
                var rootCommand = AsmCommandLine.SettingRootCommand();
                var arguments = new[] { "Main.z80", "-bin", "-lst", "List.lst", "--list-mode", "middle" };

                Assert.IsTrue(rootCommand.Parse(arguments));
                var fileInfos = rootCommand.GetValue<FileInfo[]>("input");

                Assert.AreEqual(1, fileInfos.Length);
                Assert.AreEqual("Main.z80", fileInfos.First().Name);

                var outputFiles = rootCommand.GetOutputFiles();
                Assert.AreEqual(2, outputFiles.Count);
                Assert.AreEqual("Main.bin", outputFiles[AsmEnum.FileTypeEnum.BIN].Name);
                Assert.AreEqual("List.lst", outputFiles[AsmEnum.FileTypeEnum.LST].Name);

                var listMode = rootCommand.GetListMode();
                Assert.AreEqual(listMode, AsmEnum.ListFormatEnum.Middle);
            }

            {
                var rootCommand = AsmCommandLine.SettingRootCommand();
                var arguments = new[] { "Main.z80", "-bin", "-lst", "List.lst", "--list-mode", "middle", "--output-encode", "SHIFT_JIS", "-ts", "8" };

                Assert.IsTrue(rootCommand.Parse(arguments));
                var fileInfos = rootCommand.GetValue<FileInfo[]>("input");

                Assert.AreEqual(1, fileInfos.Length);
                Assert.AreEqual("Main.z80", fileInfos.First().Name);

                var outputFiles = rootCommand.GetOutputFiles();
                Assert.AreEqual(2, outputFiles.Count);
                Assert.AreEqual("Main.bin", outputFiles[AsmEnum.FileTypeEnum.BIN].Name);
                Assert.AreEqual("List.lst", outputFiles[AsmEnum.FileTypeEnum.LST].Name);

                var listMode = rootCommand.GetListMode();
                Assert.AreEqual(AsmEnum.ListFormatEnum.Middle, listMode);

                var outputEncodeMode = rootCommand.GetOutputEncodeMode();
                Assert.AreEqual(AsmEnum.EncodeModeEnum.SHIFT_JIS, outputEncodeMode);

                var tabSize = rootCommand.GetTabSize();
                Assert.AreEqual(8, tabSize);
            }

        }

        [TestMethod]
        public void Test_CommandLine_Symbol()
        {
            {
                var rootCommand = AsmCommandLine.SettingRootCommand();
                var arguments = new[] { "Main.z80", "-bin", "-sym" };

                Assert.IsTrue(rootCommand.Parse(arguments));
                var fileInfos = rootCommand.GetValue<FileInfo[]>("input");

                Assert.AreEqual(1, fileInfos.Length);
                Assert.AreEqual("Main.z80", fileInfos.First().Name);

                var outputFiles = rootCommand.GetOutputFiles();
                Assert.AreEqual(2, outputFiles.Count);
                Assert.AreEqual("Main.bin", outputFiles[AsmEnum.FileTypeEnum.BIN].Name);
                Assert.AreEqual("Main.sym", outputFiles[AsmEnum.FileTypeEnum.SYM].Name);
            }

            {
                var rootCommand = AsmCommandLine.SettingRootCommand();
                var arguments = new[] { "Main.z80", "-bin", "-sym" };

                Assert.IsTrue(rootCommand.Parse(arguments));
                var fileInfos = rootCommand.GetValue<FileInfo[]>("input");

                Assert.AreEqual(1, fileInfos.Length);
                Assert.AreEqual("Main.z80", fileInfos.First().Name);

                var outputFiles = rootCommand.GetOutputFiles();
                Assert.AreEqual(2, outputFiles.Count);
                Assert.AreEqual("Main.bin", outputFiles[AsmEnum.FileTypeEnum.BIN].Name);
                Assert.AreEqual("Main.sym", outputFiles[AsmEnum.FileTypeEnum.SYM].Name);
            }

            {
                var rootCommand = AsmCommandLine.SettingRootCommand();
                var arguments = new[] { "Main.z80", "-bin", "-sym", "Symbol.sym" };

                Assert.IsTrue(rootCommand.Parse(arguments));
                var fileInfos = rootCommand.GetValue<FileInfo[]>("input");

                Assert.AreEqual(1, fileInfos.Length);
                Assert.AreEqual("Main.z80", fileInfos.First().Name);

                var outputFiles = rootCommand.GetOutputFiles();
                Assert.AreEqual(2, outputFiles.Count);
                Assert.AreEqual("Main.bin", outputFiles[AsmEnum.FileTypeEnum.BIN].Name);
                Assert.AreEqual("Symbol.sym", outputFiles[AsmEnum.FileTypeEnum.SYM].Name);
            }

        }

        [TestMethod]
        public void Test_CommandLine_Equal()
        {
            {
                var rootCommand = AsmCommandLine.SettingRootCommand();
                var arguments = new[] { "Main.z80", "-bin", "-equ" };

                Assert.IsTrue(rootCommand.Parse(arguments));
                var fileInfos = rootCommand.GetValue<FileInfo[]>("input");

                Assert.AreEqual(1, fileInfos.Length);
                Assert.AreEqual("Main.z80", fileInfos.First().Name);

                var outputFiles = rootCommand.GetOutputFiles();
                Assert.AreEqual(2, outputFiles.Count);
                Assert.AreEqual("Main.bin", outputFiles[AsmEnum.FileTypeEnum.BIN].Name);
                Assert.AreEqual("Main.equ", outputFiles[AsmEnum.FileTypeEnum.EQU].Name);
            }

            {
                var rootCommand = AsmCommandLine.SettingRootCommand();
                var arguments = new[] { "Main.z80", "-bin", "-equ" };

                Assert.IsTrue(rootCommand.Parse(arguments));
                var fileInfos = rootCommand.GetValue<FileInfo[]>("input");

                Assert.AreEqual(1, fileInfos.Length);
                Assert.AreEqual("Main.z80", fileInfos.First().Name);

                var outputFiles = rootCommand.GetOutputFiles();
                Assert.AreEqual(2, outputFiles.Count);
                Assert.AreEqual("Main.bin", outputFiles[AsmEnum.FileTypeEnum.BIN].Name);
                Assert.AreEqual("Main.equ", outputFiles[AsmEnum.FileTypeEnum.EQU].Name);
            }

            {
                var rootCommand = AsmCommandLine.SettingRootCommand();
                var arguments = new[] { "Main.z80", "-bin", "-equ", "Address.z80" };

                Assert.IsTrue(rootCommand.Parse(arguments));
                var fileInfos = rootCommand.GetValue<FileInfo[]>("input");

                Assert.AreEqual(1, fileInfos.Length);
                Assert.AreEqual("Main.z80", fileInfos.First().Name);

                var outputFiles = rootCommand.GetOutputFiles();
                Assert.AreEqual(2, outputFiles.Count);
                Assert.AreEqual("Main.bin", outputFiles[AsmEnum.FileTypeEnum.BIN].Name);
                Assert.AreEqual("Address.z80", outputFiles[AsmEnum.FileTypeEnum.EQU].Name);
            }

        }

        [TestMethod]
        public void Test_CommandLine_Error()
        {
            {
                var rootCommand = AsmCommandLine.SettingRootCommand();
                var arguments = new[] { "Main.z80", "-bin", "-err" };

                Assert.IsTrue(rootCommand.Parse(arguments));
                var fileInfos = rootCommand.GetValue<FileInfo[]>("input");

                Assert.AreEqual(1, fileInfos.Length);
                Assert.AreEqual("Main.z80", fileInfos.First().Name);

                var outputFiles = rootCommand.GetOutputFiles();
                Assert.AreEqual(2, outputFiles.Count);
                Assert.AreEqual("Main.bin", outputFiles[AsmEnum.FileTypeEnum.BIN].Name);
                Assert.AreEqual("Main.err", outputFiles[AsmEnum.FileTypeEnum.ERR].Name);
            }

            {
                var rootCommand = AsmCommandLine.SettingRootCommand();
                var arguments = new[] { "Main.z80", "-bin", "-err" };

                Assert.IsTrue(rootCommand.Parse(arguments));
                var fileInfos = rootCommand.GetValue<FileInfo[]>("input");

                Assert.AreEqual(1, fileInfos.Length);
                Assert.AreEqual("Main.z80", fileInfos.First().Name);

                var outputFiles = rootCommand.GetOutputFiles();
                Assert.AreEqual(2, outputFiles.Count);
                Assert.AreEqual("Main.bin", outputFiles[AsmEnum.FileTypeEnum.BIN].Name);
                Assert.AreEqual("Main.err", outputFiles[AsmEnum.FileTypeEnum.ERR].Name);
            }

            {
                var rootCommand = AsmCommandLine.SettingRootCommand();
                var arguments = new[] { "Main.z80", "-bin", "-err", "Error.txt" };

                Assert.IsTrue(rootCommand.Parse(arguments));
                var fileInfos = rootCommand.GetValue<FileInfo[]>("input");

                Assert.AreEqual(1, fileInfos.Length);
                Assert.AreEqual("Main.z80", fileInfos.First().Name);

                var outputFiles = rootCommand.GetOutputFiles();
                Assert.AreEqual(2, outputFiles.Count);
                Assert.AreEqual("Main.bin", outputFiles[AsmEnum.FileTypeEnum.BIN].Name);
                Assert.AreEqual("Error.txt", outputFiles[AsmEnum.FileTypeEnum.ERR].Name);
            }

        }

        [TestMethod]
        public void Test_CommandLine_IncludePaths()
        {
            {
                var rootCommand = AsmCommandLine.SettingRootCommand();
                var arguments = new[] { "Main.z80", "-bin", "-ips", "./lib1" };

                Assert.IsTrue(rootCommand.Parse(arguments));
                var directoryInfos = rootCommand.GetValue<DirectoryInfo[]>("includePaths");

                Assert.AreEqual(1, directoryInfos.Length);
                Assert.AreEqual("./lib1", directoryInfos.First().ToString());
            }

            {
                var rootCommand = AsmCommandLine.SettingRootCommand();
                var arguments = new[] { "Main.z80", "-bin", "--include-paths", "./lib1" };

                Assert.IsTrue(rootCommand.Parse(arguments));
                var directoryInfos = rootCommand.GetValue<DirectoryInfo[]>("includePaths");

                Assert.AreEqual(1, directoryInfos.Length);
                Assert.AreEqual("./lib1", directoryInfos.First().ToString());
            }

            {
                var rootCommand = AsmCommandLine.SettingRootCommand();
                var arguments = new[] { "Main.z80", "-bin", "-ips", "./lib1", "./lib2" };

                Assert.IsTrue(rootCommand.Parse(arguments));
                var directoryInfos = rootCommand.GetValue<DirectoryInfo[]>("includePaths");

                Assert.AreEqual(2, directoryInfos.Length);
                Assert.AreEqual("./lib1", directoryInfos.First().ToString());
                Assert.AreEqual("./lib2", directoryInfos.Last().ToString());
            }

        }

        [TestMethod]
        public void Test_CommandLine_Help()
        {
            {
                var rootCommand = AsmCommandLine.SettingRootCommand();
                var arguments = new[] { "-h" };

                Assert.IsFalse(rootCommand.Parse(arguments));
                Assert.IsTrue(rootCommand.HelpMessage.IndexOf("AILZ80ASM:") >= 0);
            }

            {
                var rootCommand = AsmCommandLine.SettingRootCommand();
                var arguments = new[] { "-h", "--input-mode" };

                Assert.IsFalse(rootCommand.Parse(arguments));
                Assert.IsTrue(rootCommand.ParseMessage.IndexOf("入力ファイルのモードを選択します。") > 0);
            }

            {
                var rootCommand = AsmCommandLine.SettingRootCommand();
                var arguments = new[] { "-h", "-om" };

                Assert.IsFalse(rootCommand.Parse(arguments));
                Assert.IsTrue(rootCommand.ParseMessage.IndexOf("Shortcut Usage:") > 0);
            }

        }

        /*
        [TestMethod]
        public void Test_CommandLine_Debug()
        {
            {
                var rootCommand = AsmCommandLine.SettingRootCommand();
                var arguments = new[] { "Main.z80", "-bin", "-d" };

                Assert.IsTrue(rootCommand.Parse(arguments));
                var fileInfos = rootCommand.GetValue<FileInfo[]>("input");

                Assert.AreEqual(fileInfos.Length, 1);
                Assert.AreEqual(fileInfos.First().Name, "Main.z80");
                Assert.AreEqual(rootCommand.GetValue<FileInfo>("output").Name, "Main.bin");
                Assert.AreEqual(rootCommand.GetValue<string>("outputMode"), "bin");
                Assert.AreEqual(rootCommand.GetValue<FileInfo>("debug").Name, "Main.dbg");

                var outputFiles = rootCommand.GetOutputFiles();
                Assert.AreEqual(outputFiles.Count, 1);
                Assert.AreEqual(outputFiles[AsmEnum.FileTypeEnum.BIN].Name, "Main.bin");
            }

            {
                var rootCommand = AsmCommandLine.SettingRootCommand();
                var arguments = new[] { "Main.z80", "-bin", "--debug" };

                Assert.IsTrue(rootCommand.Parse(arguments));
                var fileInfos = rootCommand.GetValue<FileInfo[]>("input");

                Assert.AreEqual(fileInfos.Length, 1);
                Assert.AreEqual(fileInfos.First().Name, "Main.z80");
                Assert.AreEqual(rootCommand.GetValue<FileInfo>("output").Name, "Main.bin");
                Assert.AreEqual(rootCommand.GetValue<string>("outputMode"), "bin");
                Assert.AreEqual(rootCommand.GetValue<FileInfo>("debug").Name, "Main.dbg");

                var outputFiles = rootCommand.GetOutputFiles();
                Assert.AreEqual(outputFiles.Count, 1);
                Assert.AreEqual(outputFiles[AsmEnum.FileTypeEnum.BIN].Name, "Main.bin");
            }

            {
                var rootCommand = AsmCommandLine.SettingRootCommand();
                var arguments = new[] { "Main.z80", "-bin", "--debug", "debug.dbg" };

                Assert.IsTrue(rootCommand.Parse(arguments));
                var fileInfos = rootCommand.GetValue<FileInfo[]>("input");

                Assert.AreEqual(fileInfos.Length, 1);
                Assert.AreEqual(fileInfos.First().Name, "Main.z80");
                Assert.AreEqual(rootCommand.GetValue<FileInfo>("output").Name, "Main.bin");
                Assert.AreEqual(rootCommand.GetValue<string>("outputMode"), "bin");
                Assert.AreEqual(rootCommand.GetValue<FileInfo>("debug").Name, "debug.dbg");

                var outputFiles = rootCommand.GetOutputFiles();
                Assert.AreEqual(outputFiles.Count, 1);
                Assert.AreEqual(outputFiles[AsmEnum.FileTypeEnum.BIN].Name, "Main.bin");
            }

        }
        */

        [TestMethod]
        public void Test_CommandLine_Validate()
        {
            var rootCommand = AsmCommandLine.SettingRootCommand();

            Assert.ThrowsException<Exception>(() =>
            {
                rootCommand.AddOption(new CommandLine.Option<FileInfo[]>()
                {
                    Name = "input",
                    ArgumentName = "files",
                    Aliases = new[] { "-i", "--input" },
                    Description = "アセンブリ対象のファイルをスペース区切りで指定します。",
                    Required = true,
                    IsSimple = true,
                    IsDefineOptional = true,
                });
            });

            Assert.ThrowsException<Exception>(() =>
            {
                rootCommand.AddOption(new CommandLine.Option<FileInfo[]>()
                {
                    Name = "input_test",
                    ArgumentName = "files",
                    Aliases = new[] { "-i", "--input" },
                    Description = "アセンブリ対象のファイルをスペース区切りで指定します。",
                    Required = true,
                    IsSimple = true,
                    IsDefineOptional = true,
                });
            });
        }

        [TestMethod]
        public void Test_ParseArgumentsFromJsonString()
        {
            {
                var testJson = @"
{
  ""default-options"": [
    ""-err""
  ],
  ""disable-warnings"": [
    ""W0001"",
    ""W9001"",
    ""W9002""
  ]
}";
                var argments = AsmCommandLine.ParseArgumentsFromJsonString(testJson);
                Assert.AreEqual("-err", argments[0]);
                Assert.AreEqual("-dw",  argments[1]);
                Assert.AreEqual("W0001", argments[2]);
                Assert.AreEqual("W9001", argments[3]);
                Assert.AreEqual("W9002", argments[4]);
            }

            {
                 var testJson = @"
{
  ""default-options"": [
    ""-ts 8""
  ],
  ""disable-warnings"": [
    ""W0001"",
    ""W9001"",
    ""W9002""
  ]
}";
                var argments = AsmCommandLine.ParseArgumentsFromJsonString(testJson);
                Assert.AreEqual("-ts",   argments[0]);
                Assert.AreEqual("8",     argments[1]);
                Assert.AreEqual("-dw",   argments[2]);
                Assert.AreEqual("W0001", argments[3]);
                Assert.AreEqual("W9001", argments[4]);
                Assert.AreEqual("W9002", argments[5]);
            }

            {
                var testJson = @"
{
  ""default-options"": [
    ""-ts 8""
  ],
  ""disable-warnings"": [
    ""W0001"",
    ""W9001"",
    ""W9002"",
  ]
}";
                var argments = AsmCommandLine.ParseArgumentsFromJsonString(testJson);
                Assert.AreEqual("-ts",   argments[0]);
                Assert.AreEqual("8",     argments[1]);
                Assert.AreEqual("-dw",   argments[2]);
                Assert.AreEqual("W0001", argments[3]);
                Assert.AreEqual("W9001", argments[4]);
                Assert.AreEqual("W9002", argments[5]);
            }

            {
                var testJson = @"
{
  ""default-options"": [
    ""-ts 8"",
  ],
  ""disable-warnings"": [
    ""W0001"",
    ""W9001""
    ""W9002"",
  ]
}";
                try
                {
                    try
                    {
                        var argments = AsmCommandLine.ParseArgumentsFromJsonString(testJson);
                        Assert.Fail();
                    }
                    catch (System.Text.Json.JsonException ex)
                    {
                        throw new Exception($"{ex.LineNumber}行目に問題があります。");
                    }
                }
                catch (Exception ex)
                {
                    Assert.AreEqual("8行目に問題があります。", ex.Message);
                }

            }
        }
    }
}
