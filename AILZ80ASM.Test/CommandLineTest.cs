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
                Assert.AreEqual(outputFiles[AsmLoad.OutputModeEnum.BIN].Name, "Main.bin");
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
                Assert.AreEqual(outputFiles[AsmLoad.OutputModeEnum.BIN].Name, "Main.bin");
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
                Assert.AreEqual(outputFiles[AsmLoad.OutputModeEnum.BIN].Name, "Main.bin");
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
                Assert.AreEqual(outputFiles[AsmLoad.OutputModeEnum.BIN].Name, "Main.bin");
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
            Assert.AreEqual(outputFiles[AsmLoad.OutputModeEnum.BIN].Name, "Main.bin");
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
                Assert.AreEqual(rootCommand.GetValue<string>("encodeMode"), "auto");

                var outputFiles = rootCommand.GetOutputFiles();
                Assert.AreEqual(outputFiles.Count, 1);
                Assert.AreEqual(outputFiles[AsmLoad.OutputModeEnum.BIN].Name, "Main.bin");
                Assert.AreEqual(rootCommand.GetEncodeMode(), AsmLoad.EncodeModeEnum.AUTO);
            }

            {
                var rootCommand = AsmCommandLine.SettingRootCommand();
                var arguments = new[] { "Main.z80", "-o", "Main.bin", "-en", "UTF-8" };

                Assert.IsTrue(rootCommand.Parse(arguments));
                var fileInfos = rootCommand.GetValue<FileInfo[]>("input");

                Assert.AreEqual(fileInfos.Length, 1);
                Assert.AreEqual(fileInfos.First().Name, "Main.z80");
                Assert.AreEqual(rootCommand.GetValue<FileInfo>("output").Name, "Main.bin");
                Assert.AreEqual(rootCommand.GetValue<string>("outputMode"), "bin");
                Assert.AreEqual(rootCommand.GetValue<string>("encodeMode"), "utf-8");

                var outputFiles = rootCommand.GetOutputFiles();
                Assert.AreEqual(outputFiles.Count, 1);
                Assert.AreEqual(outputFiles[AsmLoad.OutputModeEnum.BIN].Name, "Main.bin");
                Assert.AreEqual(rootCommand.GetEncodeMode(), AsmLoad.EncodeModeEnum.UTF_8);
            }

            {
                var rootCommand = AsmCommandLine.SettingRootCommand();
                var arguments = new[] { "Main.z80", "-o", "Main.bin", "-en", "SHIFT_JIS" };

                Assert.IsTrue(rootCommand.Parse(arguments));
                var fileInfos = rootCommand.GetValue<FileInfo[]>("input");

                Assert.AreEqual(fileInfos.Length, 1);
                Assert.AreEqual(fileInfos.First().Name, "Main.z80");
                Assert.AreEqual(rootCommand.GetValue<FileInfo>("output").Name, "Main.bin");
                Assert.AreEqual(rootCommand.GetValue<string>("outputMode"), "bin");
                Assert.AreEqual(rootCommand.GetValue<string>("encodeMode"), "shift_jis");

                var outputFiles = rootCommand.GetOutputFiles();
                Assert.AreEqual(outputFiles.Count, 1);
                Assert.AreEqual(outputFiles[AsmLoad.OutputModeEnum.BIN].Name, "Main.bin");
                Assert.AreEqual(rootCommand.GetEncodeMode(), AsmLoad.EncodeModeEnum.SHIFT_JIS);
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
                Assert.AreEqual(outputFiles[AsmLoad.OutputModeEnum.BIN].Name, "test.bin");
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
                Assert.AreEqual(outputFiles[AsmLoad.OutputModeEnum.BIN].Name, "Main.bin");
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
                Assert.AreEqual(outputFiles[AsmLoad.OutputModeEnum.BIN].Name, "Main.bin");
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
                Assert.AreEqual(outputFiles[AsmLoad.OutputModeEnum.BIN].Name, "Test.bin");
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
                Assert.AreEqual(outputFiles[AsmLoad.OutputModeEnum.CMT].Name, "test.cmt");
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
                Assert.AreEqual(outputFiles[AsmLoad.OutputModeEnum.CMT].Name, "Main.cmt");
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
                Assert.AreEqual(outputFiles[AsmLoad.OutputModeEnum.CMT].Name, "Main.cmt");
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
                Assert.AreEqual(outputFiles[AsmLoad.OutputModeEnum.T88].Name, "test.t88");
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
                Assert.AreEqual(outputFiles[AsmLoad.OutputModeEnum.T88].Name, "Main.t88");
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
                Assert.AreEqual(outputFiles[AsmLoad.OutputModeEnum.T88].Name, "Main.t88");
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
                Assert.AreEqual(outputFiles[AsmLoad.OutputModeEnum.HEX].Name, "test.hex");
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
                Assert.AreEqual(outputFiles[AsmLoad.OutputModeEnum.HEX].Name, "Main.hex");
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
                Assert.AreEqual(outputFiles[AsmLoad.OutputModeEnum.HEX].Name, "Main.hex");
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
                Assert.AreEqual(outputFiles[AsmLoad.OutputModeEnum.BIN].Name, "Main.bin");
                Assert.AreEqual(outputFiles[AsmLoad.OutputModeEnum.CMT].Name, "Main.cmt");
                Assert.AreEqual(outputFiles[AsmLoad.OutputModeEnum.T88].Name, "Main.t88");
            }
        }

        [TestMethod]
        public void Test_CommandLine_List()
        {
            {
                var rootCommand = AsmCommandLine.SettingRootCommand();
                var arguments = new[] { "Main.z80", "-bin", "-l" };

                Assert.IsTrue(rootCommand.Parse(arguments));
                var fileInfos = rootCommand.GetValue<FileInfo[]>("input");

                Assert.AreEqual(fileInfos.Length, 1);
                Assert.AreEqual(fileInfos.First().Name, "Main.z80");

                var outputFiles = rootCommand.GetOutputFiles();
                Assert.AreEqual(outputFiles.Count, 2);
                Assert.AreEqual(outputFiles[AsmLoad.OutputModeEnum.BIN].Name, "Main.bin");
                Assert.AreEqual(outputFiles[AsmLoad.OutputModeEnum.LST].Name, "Main.lst");

                var listMode = rootCommand.GetListMode();
                Assert.AreEqual(listMode, AsmLoad.ListModeEnum.Full);
            }

            {
                var rootCommand = AsmCommandLine.SettingRootCommand();
                var arguments = new[] { "Main.z80", "-bin", "--list", "-lm", "simple" };

                Assert.IsTrue(rootCommand.Parse(arguments));
                var fileInfos = rootCommand.GetValue<FileInfo[]>("input");

                Assert.AreEqual(fileInfos.Length, 1);
                Assert.AreEqual(fileInfos.First().Name, "Main.z80");

                var outputFiles = rootCommand.GetOutputFiles();
                Assert.AreEqual(outputFiles.Count, 2);
                Assert.AreEqual(outputFiles[AsmLoad.OutputModeEnum.BIN].Name, "Main.bin");
                Assert.AreEqual(outputFiles[AsmLoad.OutputModeEnum.LST].Name, "Main.lst");

                var listMode = rootCommand.GetListMode();
                Assert.AreEqual(listMode, AsmLoad.ListModeEnum.Simple);
            }

            {
                var rootCommand = AsmCommandLine.SettingRootCommand();
                var arguments = new[] { "Main.z80", "-bin", "--list", "List.lst", "--list-mode", "middle" };

                Assert.IsTrue(rootCommand.Parse(arguments));
                var fileInfos = rootCommand.GetValue<FileInfo[]>("input");

                Assert.AreEqual(fileInfos.Length, 1);
                Assert.AreEqual(fileInfos.First().Name, "Main.z80");

                var outputFiles = rootCommand.GetOutputFiles();
                Assert.AreEqual(outputFiles.Count, 2);
                Assert.AreEqual(outputFiles[AsmLoad.OutputModeEnum.BIN].Name, "Main.bin");
                Assert.AreEqual(outputFiles[AsmLoad.OutputModeEnum.LST].Name, "List.lst");

                var listMode = rootCommand.GetListMode();
                Assert.AreEqual(listMode, AsmLoad.ListModeEnum.Middle);
            }

        }

        [TestMethod]
        public void Test_CommandLine_Symbol()
        {
            {
                var rootCommand = AsmCommandLine.SettingRootCommand();
                var arguments = new[] { "Main.z80", "-bin", "-s" };

                Assert.IsTrue(rootCommand.Parse(arguments));
                var fileInfos = rootCommand.GetValue<FileInfo[]>("input");

                Assert.AreEqual(fileInfos.Length, 1);
                Assert.AreEqual(fileInfos.First().Name, "Main.z80");

                var outputFiles = rootCommand.GetOutputFiles();
                Assert.AreEqual(outputFiles.Count, 2);
                Assert.AreEqual(outputFiles[AsmLoad.OutputModeEnum.BIN].Name, "Main.bin");
                Assert.AreEqual(outputFiles[AsmLoad.OutputModeEnum.SYM].Name, "Main.sym");
            }

            {
                var rootCommand = AsmCommandLine.SettingRootCommand();
                var arguments = new[] { "Main.z80", "-bin", "--symbol" };

                Assert.IsTrue(rootCommand.Parse(arguments));
                var fileInfos = rootCommand.GetValue<FileInfo[]>("input");

                Assert.AreEqual(fileInfos.Length, 1);
                Assert.AreEqual(fileInfos.First().Name, "Main.z80");

                var outputFiles = rootCommand.GetOutputFiles();
                Assert.AreEqual(outputFiles.Count, 2);
                Assert.AreEqual(outputFiles[AsmLoad.OutputModeEnum.BIN].Name, "Main.bin");
                Assert.AreEqual(outputFiles[AsmLoad.OutputModeEnum.SYM].Name, "Main.sym");
            }

            {
                var rootCommand = AsmCommandLine.SettingRootCommand();
                var arguments = new[] { "Main.z80", "-bin", "--symbol", "Symbol.sym" };

                Assert.IsTrue(rootCommand.Parse(arguments));
                var fileInfos = rootCommand.GetValue<FileInfo[]>("input");

                Assert.AreEqual(fileInfos.Length, 1);
                Assert.AreEqual(fileInfos.First().Name, "Main.z80");

                var outputFiles = rootCommand.GetOutputFiles();
                Assert.AreEqual(outputFiles.Count, 2);
                Assert.AreEqual(outputFiles[AsmLoad.OutputModeEnum.BIN].Name, "Main.bin");
                Assert.AreEqual(outputFiles[AsmLoad.OutputModeEnum.SYM].Name, "Symbol.sym");
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
                Assert.AreEqual(outputFiles[AsmLoad.OutputModeEnum.BIN].Name, "Main.bin");
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
                Assert.AreEqual(outputFiles[AsmLoad.OutputModeEnum.BIN].Name, "Main.bin");
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
                Assert.AreEqual(outputFiles[AsmLoad.OutputModeEnum.BIN].Name, "Main.bin");
            }

        }
        */
    }
}
