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

                Assert.AreEqual(fileInfos.Length, 1);
                Assert.AreEqual(fileInfos.First().Name, "Main.z80");
                Assert.AreEqual(rootCommand.GetValue<FileInfo>("output").Name, "Main.bin");
                Assert.AreEqual(rootCommand.GetValue<string>("outputMode"), "bin");

                var outputFiles = rootCommand.GetOutputFiles();
                Assert.AreEqual(outputFiles.Count, 1);
                Assert.AreEqual(outputFiles[AsmEnum.FileTypeEnum.BIN].Name, "Main.bin");
            }

            {
                var rootCommand = AsmCommandLine.SettingRootCommand();
                var arguments = new[] { "--input", "Main.z80", "--output", "Main.bin" };

                Assert.IsTrue(rootCommand.Parse(arguments));
                var fileInfos = rootCommand.GetValue<FileInfo[]>("input");

                Assert.AreEqual(fileInfos.Length, 1);
                Assert.AreEqual(fileInfos.First().Name, "Main.z80");
                Assert.AreEqual(rootCommand.GetValue<FileInfo>("output").Name, "Main.bin");
                Assert.AreEqual(rootCommand.GetValue<string>("outputMode"), "bin");

                var outputFiles = rootCommand.GetOutputFiles();
                Assert.AreEqual(outputFiles.Count, 1);
                Assert.AreEqual(outputFiles[AsmEnum.FileTypeEnum.BIN].Name, "Main.bin");
            }
        }

        [TestMethod]
        public void Test_CommandLine_Version()
        {
            var rootCommand = AsmCommandLine.SettingRootCommand();
            var arguments = new[] { "-v" };

            Assert.IsFalse(rootCommand.Parse(arguments));
            Assert.IsTrue(Version.TryParse(rootCommand.ParseMessage, out var dummy));
        }

        [TestMethod]
        public void Test_CommandLine_Input()
        {
            {
                var rootCommand = AsmCommandLine.SettingRootCommand();
                var arguments = new[] { "Main.z80", "-o", "Main.bin" };

                Assert.IsTrue(rootCommand.Parse(arguments));
                var fileInfos = rootCommand.GetValue<FileInfo[]>("input");

                Assert.AreEqual(fileInfos.Length, 1);
                Assert.AreEqual(fileInfos.First().Name, "Main.z80");
                Assert.AreEqual(rootCommand.GetValue<FileInfo>("output").Name, "Main.bin");
                Assert.AreEqual(rootCommand.GetValue<string>("outputMode"), "bin");

                var outputFiles = rootCommand.GetOutputFiles();
                Assert.AreEqual(outputFiles.Count, 1);
                Assert.AreEqual(outputFiles[AsmEnum.FileTypeEnum.BIN].Name, "Main.bin");
            }

            {
                var rootCommand = AsmCommandLine.SettingRootCommand();
                var arguments = new[] { "-i", "Main.z80", "-o", "Main.bin" };

                Assert.IsTrue(rootCommand.Parse(arguments));
                var fileInfos = rootCommand.GetValue<FileInfo[]>("input");

                Assert.AreEqual(fileInfos.Length, 1);
                Assert.AreEqual(fileInfos.First().Name, "Main.z80");
                Assert.AreEqual(rootCommand.GetValue<FileInfo>("output").Name, "Main.bin");
                Assert.AreEqual(rootCommand.GetValue<string>("outputMode"), "bin");

                var outputFiles = rootCommand.GetOutputFiles();
                Assert.AreEqual(outputFiles.Count, 1);
                Assert.AreEqual(outputFiles[AsmEnum.FileTypeEnum.BIN].Name, "Main.bin");
            }
        }

        [TestMethod]
        public void Test_CommandLine_Inputs()
        {
            var rootCommand = AsmCommandLine.SettingRootCommand();
            var arguments = new[] { "Main.z80", "Main2.z80", "Main3.z80", "-o", "Main.bin" };

            Assert.IsTrue(rootCommand.Parse(arguments));
            var fileInfos = rootCommand.GetValue<FileInfo[]>("input");

            Assert.AreEqual(fileInfos.Length, 3);
            Assert.IsTrue(fileInfos.Any(m => m.Name == "Main.z80"));
            Assert.IsTrue(fileInfos.Any(m => m.Name == "Main2.z80"));
            Assert.IsTrue(fileInfos.Any(m => m.Name == "Main3.z80"));
            Assert.AreEqual(rootCommand.GetValue<FileInfo>("output").Name, "Main.bin");
            Assert.AreEqual(rootCommand.GetValue<string>("outputMode"), "bin");

            var outputFiles = rootCommand.GetOutputFiles();
            Assert.AreEqual(outputFiles.Count, 1);
            Assert.AreEqual(outputFiles[AsmEnum.FileTypeEnum.BIN].Name, "Main.bin");
        }

        [TestMethod]
        public void Test_CommandLine_Encode()
        {
            {
                var rootCommand = AsmCommandLine.SettingRootCommand();
                var arguments = new[] { "Main.z80", "-o", "Main.bin" };

                Assert.IsTrue(rootCommand.Parse(arguments));
                var fileInfos = rootCommand.GetValue<FileInfo[]>("input");

                Assert.AreEqual(fileInfos.Length, 1);
                Assert.AreEqual(fileInfos.First().Name, "Main.z80");
                Assert.AreEqual(rootCommand.GetValue<FileInfo>("output").Name, "Main.bin");
                Assert.AreEqual(rootCommand.GetValue<string>("outputMode"), "bin");
                Assert.AreEqual(rootCommand.GetValue<string>("inputEncode"), "auto");

                var outputFiles = rootCommand.GetOutputFiles();
                Assert.AreEqual(outputFiles.Count, 1);
                Assert.AreEqual(outputFiles[AsmEnum.FileTypeEnum.BIN].Name, "Main.bin");
                Assert.AreEqual(rootCommand.GetInputEncodeMode(), AsmEnum.EncodeModeEnum.AUTO);
            }

            {
                var rootCommand = AsmCommandLine.SettingRootCommand();
                var arguments = new[] { "Main.z80", "-o", "Main.bin", "-ie", "UTF-8", "-oe", "SHIFT_JIS" };

                Assert.IsTrue(rootCommand.Parse(arguments));
                var fileInfos = rootCommand.GetValue<FileInfo[]>("input");

                Assert.AreEqual(fileInfos.Length, 1);
                Assert.AreEqual(fileInfos.First().Name, "Main.z80");
                Assert.AreEqual(rootCommand.GetValue<string>("inputEncode"), "utf-8");
                Assert.AreEqual(rootCommand.GetValue<FileInfo>("output").Name, "Main.bin");
                Assert.AreEqual(rootCommand.GetValue<string>("outputMode"), "bin");
                Assert.AreEqual(rootCommand.GetValue<string>("outputEncode"), "shift_jis");

                var outputFiles = rootCommand.GetOutputFiles();
                Assert.AreEqual(outputFiles.Count, 1);
                Assert.AreEqual(outputFiles[AsmEnum.FileTypeEnum.BIN].Name, "Main.bin");
                Assert.AreEqual(rootCommand.GetInputEncodeMode(), AsmEnum.EncodeModeEnum.UTF_8);
                Assert.AreEqual(rootCommand.GetOutputEncodeMode(), AsmEnum.EncodeModeEnum.SHIFT_JIS);
            }

            {
                var rootCommand = AsmCommandLine.SettingRootCommand();
                var arguments = new[] { "Main.z80", "-o", "Main.bin", "-oe", "SHIFT_JIS" };

                Assert.IsTrue(rootCommand.Parse(arguments));
                var fileInfos = rootCommand.GetValue<FileInfo[]>("input");

                Assert.AreEqual(fileInfos.Length, 1);
                Assert.AreEqual(fileInfos.First().Name, "Main.z80");
                Assert.AreEqual(rootCommand.GetValue<FileInfo>("output").Name, "Main.bin");
                Assert.AreEqual(rootCommand.GetValue<string>("outputMode"), "bin");
                Assert.AreEqual(rootCommand.GetValue<string>("outputEncode"), "shift_jis");

                var outputFiles = rootCommand.GetOutputFiles();
                Assert.AreEqual(outputFiles.Count, 1);
                Assert.AreEqual(outputFiles[AsmEnum.FileTypeEnum.BIN].Name, "Main.bin");
                Assert.AreEqual(rootCommand.GetOutputEncodeMode(), AsmEnum.EncodeModeEnum.SHIFT_JIS);
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

                Assert.AreEqual(fileInfos.Length, 1);
                Assert.AreEqual(fileInfos.First().Name, "Main.z80");
                Assert.AreEqual(rootCommand.GetValue<FileInfo>("output").Name, "test.bin");
                Assert.AreEqual(rootCommand.GetValue<string>("outputMode"), "bin");

                var outputFiles = rootCommand.GetOutputFiles();
                Assert.AreEqual(outputFiles.Count, 1);
                Assert.AreEqual(outputFiles[AsmEnum.FileTypeEnum.BIN].Name, "test.bin");
            }

            {
                var rootCommand = AsmCommandLine.SettingRootCommand();
                var arguments = new[] { "Main.z80" };

                Assert.IsTrue(rootCommand.Parse(arguments));
                var fileInfos = rootCommand.GetValue<FileInfo[]>("input");

                Assert.AreEqual(fileInfos.Length, 1);
                Assert.AreEqual(fileInfos.First().Name, "Main.z80");
                Assert.AreEqual(rootCommand.GetValue<FileInfo>("output").Name, "Main.bin");
                Assert.AreEqual(rootCommand.GetValue<string>("outputMode"), "bin");

                var outputFiles = rootCommand.GetOutputFiles();
                Assert.AreEqual(outputFiles.Count, 1);
                Assert.AreEqual(outputFiles[AsmEnum.FileTypeEnum.BIN].Name, "Main.bin");
            }

            {
                var rootCommand = AsmCommandLine.SettingRootCommand();
                var arguments = new[] { "Main.z80", "-bin" };

                Assert.IsTrue(rootCommand.Parse(arguments));
                var fileInfos = rootCommand.GetValue<FileInfo[]>("input");

                Assert.AreEqual(fileInfos.Length, 1);
                Assert.AreEqual(fileInfos.First().Name, "Main.z80");
                Assert.AreEqual(rootCommand.GetValue<FileInfo>("output").Name, "Main.bin");
                Assert.AreEqual(rootCommand.GetValue<string>("outputMode"), "bin");

                var outputFiles = rootCommand.GetOutputFiles();
                Assert.AreEqual(outputFiles.Count, 1);
                Assert.AreEqual(outputFiles[AsmEnum.FileTypeEnum.BIN].Name, "Main.bin");
            }

            {
                var rootCommand = AsmCommandLine.SettingRootCommand();
                var arguments = new[] { "Main.z80", "-bin", "Test.bin" };

                Assert.IsTrue(rootCommand.Parse(arguments));
                var fileInfos = rootCommand.GetValue<FileInfo[]>("input");

                Assert.AreEqual(fileInfos.Length, 1);
                Assert.AreEqual(fileInfos.First().Name, "Main.z80");
                Assert.AreEqual(rootCommand.GetValue<FileInfo>("output").Name, "Main.bin");
                Assert.AreEqual(rootCommand.GetValue<string>("outputMode"), "bin");             // デフォルト値が設定
                Assert.AreEqual(rootCommand.GetValue<FileInfo>("outputBin").Name, "Test.bin");
                Assert.IsFalse(rootCommand.GetSelected("output"));
                Assert.IsFalse(rootCommand.GetSelected("outputMode"));

                var outputFiles = rootCommand.GetOutputFiles();
                Assert.AreEqual(outputFiles.Count, 1);
                Assert.AreEqual(outputFiles[AsmEnum.FileTypeEnum.BIN].Name, "Test.bin");
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

                Assert.AreEqual(fileInfos.Length, 1);
                Assert.AreEqual(fileInfos.First().Name, "Main.z80");
                Assert.AreEqual(rootCommand.GetValue<FileInfo>("output").Name, "test.cmt");
                Assert.AreEqual(rootCommand.GetValue<string>("outputMode"), "cmt");

                var outputFiles = rootCommand.GetOutputFiles();
                Assert.AreEqual(outputFiles.Count, 1);
                Assert.AreEqual(outputFiles[AsmEnum.FileTypeEnum.CMT].Name, "test.cmt");
            }

            {
                var rootCommand = AsmCommandLine.SettingRootCommand();
                var arguments = new[] { "Main.z80", "-cmt" };

                Assert.IsTrue(rootCommand.Parse(arguments));
                var fileInfos = rootCommand.GetValue<FileInfo[]>("input");

                Assert.AreEqual(fileInfos.Length, 1);
                Assert.AreEqual(fileInfos.First().Name, "Main.z80");
                Assert.AreEqual(rootCommand.GetValue<FileInfo>("output").Name, "Main.cmt");
                Assert.AreEqual(rootCommand.GetValue<string>("outputMode"), "cmt");

                var outputFiles = rootCommand.GetOutputFiles();
                Assert.AreEqual(outputFiles.Count, 1);
                Assert.AreEqual(outputFiles[AsmEnum.FileTypeEnum.CMT].Name, "Main.cmt");
            }

            {
                var rootCommand = AsmCommandLine.SettingRootCommand();
                var arguments = new[] { "Main.z80", "-cmt", "Main.cmt" };

                Assert.IsTrue(rootCommand.Parse(arguments));
                var fileInfos = rootCommand.GetValue<FileInfo[]>("input");

                Assert.AreEqual(fileInfos.Length, 1);
                Assert.AreEqual(fileInfos.First().Name, "Main.z80");
                Assert.AreEqual(rootCommand.GetValue<FileInfo>("output").Name, "Main.bin");
                Assert.AreEqual(rootCommand.GetValue<string>("outputMode"), "bin");             // デフォルト値が設定
                Assert.AreEqual(rootCommand.GetValue<FileInfo>("outputCMT").Name, "Main.cmt");
                Assert.IsFalse(rootCommand.GetSelected("output"));
                Assert.IsFalse(rootCommand.GetSelected("outputMode"));

                var outputFiles = rootCommand.GetOutputFiles();
                Assert.AreEqual(outputFiles.Count, 1);
                Assert.AreEqual(outputFiles[AsmEnum.FileTypeEnum.CMT].Name, "Main.cmt");
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

                Assert.AreEqual(fileInfos.Length, 1);
                Assert.AreEqual(fileInfos.First().Name, "Main.z80");
                Assert.AreEqual(rootCommand.GetValue<FileInfo>("output").Name, "test.bin");
                Assert.AreEqual(rootCommand.GetValue<string>("outputMode"), "bin");

                var outputFiles = rootCommand.GetOutputFiles();
                Assert.AreEqual(outputFiles.Count, 1);
                Assert.AreEqual(outputFiles[AsmEnum.FileTypeEnum.BIN].Name, "test.bin");
            }

            {
                var rootCommand = AsmCommandLine.SettingRootCommand();
                var arguments = new[] { "Main.z80", "-bin", "-tag" };

                Assert.IsTrue(rootCommand.Parse(arguments));
                var fileInfos = rootCommand.GetValue<FileInfo[]>("input");

                Assert.AreEqual(fileInfos.Length, 1);
                Assert.AreEqual(fileInfos.First().Name, "Main.z80");
                Assert.AreEqual(rootCommand.GetValue<FileInfo>("output").Name, "Main.bin");
                Assert.AreEqual(rootCommand.GetValue<string>("outputMode"), "bin");

                var outputFiles = rootCommand.GetOutputFiles();
                Assert.AreEqual(outputFiles.Count, 2);
                Assert.AreEqual(outputFiles[AsmEnum.FileTypeEnum.BIN].Name, "Main.bin");
                Assert.AreEqual(outputFiles[AsmEnum.FileTypeEnum.TAG].Name, "tags");
            }

            {
                var rootCommand = AsmCommandLine.SettingRootCommand();
                var arguments = new[] { "Main.z80", "-bin", "-tag"};

                Assert.IsTrue(rootCommand.Parse(arguments));
                var fileInfos = rootCommand.GetValue<FileInfo[]>("input");

                Assert.AreEqual(fileInfos.Length, 1);
                Assert.AreEqual(fileInfos.First().Name, "Main.z80");
                Assert.AreEqual(rootCommand.GetValue<FileInfo>("output").Name, "Main.bin");
                Assert.AreEqual(rootCommand.GetValue<string>("outputMode"), "bin");

                var outputFiles = rootCommand.GetOutputFiles();
                Assert.AreEqual(outputFiles.Count, 2);
                Assert.AreEqual(outputFiles[AsmEnum.FileTypeEnum.BIN].Name, "Main.bin");
                Assert.AreEqual(outputFiles[AsmEnum.FileTypeEnum.TAG].Name, "tags");
            }

            {
                var rootCommand = AsmCommandLine.SettingRootCommand();
                var arguments = new[] { "Main.z80", "-bin", "Test.bin", "-tag", "Test.tag" };

                Assert.IsTrue(rootCommand.Parse(arguments));
                var fileInfos = rootCommand.GetValue<FileInfo[]>("input");

                Assert.AreEqual(fileInfos.Length, 1);
                Assert.AreEqual(fileInfos.First().Name, "Main.z80");
                Assert.AreEqual(rootCommand.GetValue<FileInfo>("output").Name, "Main.bin");
                Assert.AreEqual(rootCommand.GetValue<string>("outputMode"), "bin");             // デフォルト値が設定
                Assert.AreEqual(rootCommand.GetValue<FileInfo>("outputBin").Name, "Test.bin");
                Assert.IsFalse(rootCommand.GetSelected("output"));
                Assert.IsFalse(rootCommand.GetSelected("outputMode"));

                var outputFiles = rootCommand.GetOutputFiles();
                Assert.AreEqual(outputFiles.Count, 2);
                Assert.AreEqual(outputFiles[AsmEnum.FileTypeEnum.BIN].Name, "Test.bin");
                Assert.AreEqual(outputFiles[AsmEnum.FileTypeEnum.TAG].Name, "Test.tag");
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

                Assert.AreEqual(fileInfos.Length, 1);
                Assert.AreEqual(fileInfos.First().Name, "Main.z80");
                Assert.AreEqual(rootCommand.GetValue<FileInfo>("output").Name, "test.t88");
                Assert.AreEqual(rootCommand.GetValue<string>("outputMode"), "t88");

                var outputFiles = rootCommand.GetOutputFiles();
                Assert.AreEqual(outputFiles.Count, 1);
                Assert.AreEqual(outputFiles[AsmEnum.FileTypeEnum.T88].Name, "test.t88");
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

                Assert.AreEqual(fileInfos.Length, 1);
                Assert.AreEqual(fileInfos.First().Name, "Main.z80");
                Assert.AreEqual(rootCommand.GetValue<FileInfo>("output").Name, "Main.bin");
                Assert.AreEqual(rootCommand.GetValue<string>("outputMode"), "bin");             // デフォルト値が設定
                Assert.AreEqual(rootCommand.GetValue<FileInfo>("outputT88").Name, "Main.t88");
                Assert.IsFalse(rootCommand.GetSelected("output"));
                Assert.IsFalse(rootCommand.GetSelected("outputMode"));

                var outputFiles = rootCommand.GetOutputFiles();
                Assert.AreEqual(outputFiles.Count, 1);
                Assert.AreEqual(outputFiles[AsmEnum.FileTypeEnum.T88].Name, "Main.t88");
            }
        }

        /*
        [TestMethod]
        public void Test_CommandLine_Hex()
        {
            {
                var rootCommand = AsmCommandLine.SettingRootCommand();
                var arguments = new[] { "Main.z80", "-o", "test.hex", "-om", "HEX" };

                Assert.IsTrue(rootCommand.Parse(arguments));
                var fileInfos = rootCommand.GetValue<FileInfo[]>("input");

                Assert.AreEqual(fileInfos.Length, 1);
                Assert.AreEqual(fileInfos.First().Name, "Main.z80");
                Assert.AreEqual(rootCommand.GetValue<FileInfo>("output").Name, "test.hex");
                Assert.AreEqual(rootCommand.GetValue<string>("outputMode"), "hex");

                var outputFiles = rootCommand.GetOutputFiles();
                Assert.AreEqual(outputFiles.Count, 1);
                Assert.AreEqual(outputFiles[AsmEnum.FileTypeEnum.HEX].Name, "test.hex");
            }

            {
                var rootCommand = AsmCommandLine.SettingRootCommand();
                var arguments = new[] { "Main.z80", "-hex" };

                Assert.IsTrue(rootCommand.Parse(arguments));
                var fileInfos = rootCommand.GetValue<FileInfo[]>("input");

                Assert.AreEqual(fileInfos.Length, 1);
                Assert.AreEqual(fileInfos.First().Name, "Main.z80");
                Assert.AreEqual(rootCommand.GetValue<FileInfo>("output").Name, "Main.hex");
                Assert.AreEqual(rootCommand.GetValue<string>("outputMode"), "hex");

                var outputFiles = rootCommand.GetOutputFiles();
                Assert.AreEqual(outputFiles.Count, 1);
                Assert.AreEqual(outputFiles[AsmEnum.FileTypeEnum.HEX].Name, "Main.hex");
            }

            {
                var rootCommand = AsmCommandLine.SettingRootCommand();
                var arguments = new[] { "Main.z80", "-hex", "Main.hex" };

                Assert.IsTrue(rootCommand.Parse(arguments));
                var fileInfos = rootCommand.GetValue<FileInfo[]>("input");

                Assert.AreEqual(fileInfos.Length, 1);
                Assert.AreEqual(fileInfos.First().Name, "Main.z80");
                Assert.AreEqual(rootCommand.GetValue<FileInfo>("output").Name, "Main.bin");
                Assert.AreEqual(rootCommand.GetValue<string>("outputMode"), "bin");             // デフォルト値が設定
                Assert.AreEqual(rootCommand.GetValue<FileInfo>("outputHex").Name, "Main.hex");
                Assert.IsFalse(rootCommand.GetSelected("output"));
                Assert.IsFalse(rootCommand.GetSelected("outputMode"));

                var outputFiles = rootCommand.GetOutputFiles();
                Assert.AreEqual(outputFiles.Count, 1);
                Assert.AreEqual(outputFiles[AsmEnum.FileTypeEnum.HEX].Name, "Main.hex");
            }
        }
        */

        [TestMethod]
        public void Test_CommandLine_Bin_CMT_T88()
        {
            {
                var rootCommand = AsmCommandLine.SettingRootCommand();
                var arguments = new[] { "Main.z80", "-bin", "-cmt", "-t88" };

                Assert.IsTrue(rootCommand.Parse(arguments));
                var fileInfos = rootCommand.GetValue<FileInfo[]>("input");

                Assert.AreEqual(fileInfos.Length, 1);
                Assert.AreEqual(fileInfos.First().Name, "Main.z80");
                Assert.AreEqual(rootCommand.GetValue<FileInfo>("output").Name, "Main.bin");
                Assert.AreEqual(rootCommand.GetValue<string>("outputMode"), "bin");
                Assert.AreEqual(rootCommand.GetValue<FileInfo>("outputCMT").Name, "Main.cmt");
                Assert.AreEqual(rootCommand.GetValue<FileInfo>("outputT88").Name, "Main.t88");
                Assert.IsFalse(rootCommand.GetSelected("output"));
                Assert.IsTrue(rootCommand.GetSelected("outputMode"));

                var outputFiles = rootCommand.GetOutputFiles();
                Assert.AreEqual(outputFiles.Count, 3);
                Assert.AreEqual(outputFiles[AsmEnum.FileTypeEnum.BIN].Name, "Main.bin");
                Assert.AreEqual(outputFiles[AsmEnum.FileTypeEnum.CMT].Name, "Main.cmt");
                Assert.AreEqual(outputFiles[AsmEnum.FileTypeEnum.T88].Name, "Main.t88");
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

                Assert.AreEqual(fileInfos.Length, 1);
                Assert.AreEqual(fileInfos.First().Name, "Main.z80");

                var outputFiles = rootCommand.GetOutputFiles();
                Assert.AreEqual(outputFiles.Count, 2);
                Assert.AreEqual(outputFiles[AsmEnum.FileTypeEnum.BIN].Name, "Main.bin");
                Assert.AreEqual(outputFiles[AsmEnum.FileTypeEnum.LST].Name, "Main.lst");

                var listMode = rootCommand.GetListMode();
                Assert.AreEqual(listMode, AsmEnum.ListFormatEnum.Full);
            }

            {
                var rootCommand = AsmCommandLine.SettingRootCommand();
                var arguments = new[] { "Main.z80", "-bin", "-lst", "-lm", "simple" };

                Assert.IsTrue(rootCommand.Parse(arguments));
                var fileInfos = rootCommand.GetValue<FileInfo[]>("input");

                Assert.AreEqual(fileInfos.Length, 1);
                Assert.AreEqual(fileInfos.First().Name, "Main.z80");

                var outputFiles = rootCommand.GetOutputFiles();
                Assert.AreEqual(outputFiles.Count, 2);
                Assert.AreEqual(outputFiles[AsmEnum.FileTypeEnum.BIN].Name, "Main.bin");
                Assert.AreEqual(outputFiles[AsmEnum.FileTypeEnum.LST].Name, "Main.lst");

                var listMode = rootCommand.GetListMode();
                Assert.AreEqual(listMode, AsmEnum.ListFormatEnum.Simple);
            }

            {
                var rootCommand = AsmCommandLine.SettingRootCommand();
                var arguments = new[] { "Main.z80", "-bin", "-lst", "List.lst", "--list-mode", "middle" };

                Assert.IsTrue(rootCommand.Parse(arguments));
                var fileInfos = rootCommand.GetValue<FileInfo[]>("input");

                Assert.AreEqual(fileInfos.Length, 1);
                Assert.AreEqual(fileInfos.First().Name, "Main.z80");

                var outputFiles = rootCommand.GetOutputFiles();
                Assert.AreEqual(outputFiles.Count, 2);
                Assert.AreEqual(outputFiles[AsmEnum.FileTypeEnum.BIN].Name, "Main.bin");
                Assert.AreEqual(outputFiles[AsmEnum.FileTypeEnum.LST].Name, "List.lst");

                var listMode = rootCommand.GetListMode();
                Assert.AreEqual(listMode, AsmEnum.ListFormatEnum.Middle);
            }

            {
                var rootCommand = AsmCommandLine.SettingRootCommand();
                var arguments = new[] { "Main.z80", "-bin", "-lst", "List.lst", "--list-mode", "middle", "--output-encode", "SHIFT_JIS", "-ts", "8" };

                Assert.IsTrue(rootCommand.Parse(arguments));
                var fileInfos = rootCommand.GetValue<FileInfo[]>("input");

                Assert.AreEqual(fileInfos.Length, 1);
                Assert.AreEqual(fileInfos.First().Name, "Main.z80");

                var outputFiles = rootCommand.GetOutputFiles();
                Assert.AreEqual(outputFiles.Count, 2);
                Assert.AreEqual(outputFiles[AsmEnum.FileTypeEnum.BIN].Name, "Main.bin");
                Assert.AreEqual(outputFiles[AsmEnum.FileTypeEnum.LST].Name, "List.lst");

                var listMode = rootCommand.GetListMode();
                Assert.AreEqual(listMode, AsmEnum.ListFormatEnum.Middle);

                var outputEncodeMode = rootCommand.GetOutputEncodeMode();
                Assert.AreEqual(outputEncodeMode, AsmEnum.EncodeModeEnum.SHIFT_JIS);

                var tabSize = rootCommand.GetTabSize();
                Assert.AreEqual(tabSize, 8);
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

                Assert.AreEqual(fileInfos.Length, 1);
                Assert.AreEqual(fileInfos.First().Name, "Main.z80");

                var outputFiles = rootCommand.GetOutputFiles();
                Assert.AreEqual(outputFiles.Count, 2);
                Assert.AreEqual(outputFiles[AsmEnum.FileTypeEnum.BIN].Name, "Main.bin");
                Assert.AreEqual(outputFiles[AsmEnum.FileTypeEnum.SYM].Name, "Main.sym");
            }

            {
                var rootCommand = AsmCommandLine.SettingRootCommand();
                var arguments = new[] { "Main.z80", "-bin", "-sym" };

                Assert.IsTrue(rootCommand.Parse(arguments));
                var fileInfos = rootCommand.GetValue<FileInfo[]>("input");

                Assert.AreEqual(fileInfos.Length, 1);
                Assert.AreEqual(fileInfos.First().Name, "Main.z80");

                var outputFiles = rootCommand.GetOutputFiles();
                Assert.AreEqual(outputFiles.Count, 2);
                Assert.AreEqual(outputFiles[AsmEnum.FileTypeEnum.BIN].Name, "Main.bin");
                Assert.AreEqual(outputFiles[AsmEnum.FileTypeEnum.SYM].Name, "Main.sym");
            }

            {
                var rootCommand = AsmCommandLine.SettingRootCommand();
                var arguments = new[] { "Main.z80", "-bin", "-sym", "Symbol.sym" };

                Assert.IsTrue(rootCommand.Parse(arguments));
                var fileInfos = rootCommand.GetValue<FileInfo[]>("input");

                Assert.AreEqual(fileInfos.Length, 1);
                Assert.AreEqual(fileInfos.First().Name, "Main.z80");

                var outputFiles = rootCommand.GetOutputFiles();
                Assert.AreEqual(outputFiles.Count, 2);
                Assert.AreEqual(outputFiles[AsmEnum.FileTypeEnum.BIN].Name, "Main.bin");
                Assert.AreEqual(outputFiles[AsmEnum.FileTypeEnum.SYM].Name, "Symbol.sym");
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

                Assert.AreEqual(fileInfos.Length, 1);
                Assert.AreEqual(fileInfos.First().Name, "Main.z80");

                var outputFiles = rootCommand.GetOutputFiles();
                Assert.AreEqual(outputFiles.Count, 2);
                Assert.AreEqual(outputFiles[AsmEnum.FileTypeEnum.BIN].Name, "Main.bin");
                Assert.AreEqual(outputFiles[AsmEnum.FileTypeEnum.EQU].Name, "Main.equ");
            }

            {
                var rootCommand = AsmCommandLine.SettingRootCommand();
                var arguments = new[] { "Main.z80", "-bin", "-equ" };

                Assert.IsTrue(rootCommand.Parse(arguments));
                var fileInfos = rootCommand.GetValue<FileInfo[]>("input");

                Assert.AreEqual(fileInfos.Length, 1);
                Assert.AreEqual(fileInfos.First().Name, "Main.z80");

                var outputFiles = rootCommand.GetOutputFiles();
                Assert.AreEqual(outputFiles.Count, 2);
                Assert.AreEqual(outputFiles[AsmEnum.FileTypeEnum.BIN].Name, "Main.bin");
                Assert.AreEqual(outputFiles[AsmEnum.FileTypeEnum.EQU].Name, "Main.equ");
            }

            {
                var rootCommand = AsmCommandLine.SettingRootCommand();
                var arguments = new[] { "Main.z80", "-bin", "-equ", "Address.z80" };

                Assert.IsTrue(rootCommand.Parse(arguments));
                var fileInfos = rootCommand.GetValue<FileInfo[]>("input");

                Assert.AreEqual(fileInfos.Length, 1);
                Assert.AreEqual(fileInfos.First().Name, "Main.z80");

                var outputFiles = rootCommand.GetOutputFiles();
                Assert.AreEqual(outputFiles.Count, 2);
                Assert.AreEqual(outputFiles[AsmEnum.FileTypeEnum.BIN].Name, "Main.bin");
                Assert.AreEqual(outputFiles[AsmEnum.FileTypeEnum.EQU].Name, "Address.z80");
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

                Assert.AreEqual(fileInfos.Length, 1);
                Assert.AreEqual(fileInfos.First().Name, "Main.z80");

                var outputFiles = rootCommand.GetOutputFiles();
                Assert.AreEqual(outputFiles.Count, 2);
                Assert.AreEqual(outputFiles[AsmEnum.FileTypeEnum.BIN].Name, "Main.bin");
                Assert.AreEqual(outputFiles[AsmEnum.FileTypeEnum.ERR].Name, "Main.err");
            }

            {
                var rootCommand = AsmCommandLine.SettingRootCommand();
                var arguments = new[] { "Main.z80", "-bin", "-err" };

                Assert.IsTrue(rootCommand.Parse(arguments));
                var fileInfos = rootCommand.GetValue<FileInfo[]>("input");

                Assert.AreEqual(fileInfos.Length, 1);
                Assert.AreEqual(fileInfos.First().Name, "Main.z80");

                var outputFiles = rootCommand.GetOutputFiles();
                Assert.AreEqual(outputFiles.Count, 2);
                Assert.AreEqual(outputFiles[AsmEnum.FileTypeEnum.BIN].Name, "Main.bin");
                Assert.AreEqual(outputFiles[AsmEnum.FileTypeEnum.ERR].Name, "Main.err");
            }

            {
                var rootCommand = AsmCommandLine.SettingRootCommand();
                var arguments = new[] { "Main.z80", "-bin", "-err", "Error.txt" };

                Assert.IsTrue(rootCommand.Parse(arguments));
                var fileInfos = rootCommand.GetValue<FileInfo[]>("input");

                Assert.AreEqual(fileInfos.Length, 1);
                Assert.AreEqual(fileInfos.First().Name, "Main.z80");

                var outputFiles = rootCommand.GetOutputFiles();
                Assert.AreEqual(outputFiles.Count, 2);
                Assert.AreEqual(outputFiles[AsmEnum.FileTypeEnum.BIN].Name, "Main.bin");
                Assert.AreEqual(outputFiles[AsmEnum.FileTypeEnum.ERR].Name, "Error.txt");
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
                Assert.AreEqual(argments[0], "-err");
                Assert.AreEqual(argments[1], "-dw");
                Assert.AreEqual(argments[2], "W0001");
                Assert.AreEqual(argments[3], "W9001");
                Assert.AreEqual(argments[4], "W9002");
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
                Assert.AreEqual(argments[0], "-ts");
                Assert.AreEqual(argments[1], "8");
                Assert.AreEqual(argments[2], "-dw");
                Assert.AreEqual(argments[3], "W0001");
                Assert.AreEqual(argments[4], "W9001");
                Assert.AreEqual(argments[5], "W9002");
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
                Assert.AreEqual(argments[0], "-ts");
                Assert.AreEqual(argments[1], "8");
                Assert.AreEqual(argments[2], "-dw");
                Assert.AreEqual(argments[3], "W0001");
                Assert.AreEqual(argments[4], "W9001");
                Assert.AreEqual(argments[5], "W9002");
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
                    Assert.AreEqual(ex.Message, "8行目に問題があります。");
                }

            }
        }
    }
}
