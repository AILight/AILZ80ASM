using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AILZ80ASM.CommandLine;

namespace AILZ80ASM
{
    class Program
    {
        public static int Main(params string[] args)
        {
            try
            {
                var rootCommand = AsmCommandLine.SettingRootCommand();

                OutputStart();

                // 引数の名前とRootCommand.Optionの名前が一致していないと変数展開されない
                if (rootCommand.Parse(args))
                {
                    return Assember(rootCommand) ? 0 : 1;
                }
                else
                {
                    Console.WriteLine(rootCommand.ParseMessage);

                    return 2;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 3;
            }
        }

        public static bool Assember(RootCommand rootCommand)
        {
            return Assember(rootCommand.GetValue<FileInfo[]>("input"),
                            rootCommand.GetEncodeMode(),
                            rootCommand.GetOutputFiles(),
                            rootCommand.GetListMode(),
                            rootCommand.GetValue<bool>("outputTrim"));
        }


        /// <summary>
        /// アセンブル
        /// </summary>
        /// <param name="inputs"></param>
        /// <param name="inputMode"></param>
        /// <param name="output"></param>
        /// <param name="outputMode"></param>
        /// <param name="symbol"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public static bool Assember(
                FileInfo[] inputs, AsmLoad.EncodeModeEnum encodeMode, Dictionary<AsmLoad.OutputModeEnum, FileInfo> outputFiles, AsmLoad.ListModeEnum listMode, bool outputTrim)
        {
            try
            {
                // デバッグ情報
                /*
                if (debugFileInfo != default)
                {
                    if (debugFileInfo.Exists)
                    {
                        Console.Write("デバッグファイルが既に存在しています。削除してから書き込みますか？ (y/n)");
                        if (Console.ReadKey().Key == ConsoleKey.Y)
                        {
                            debugFileInfo.Delete();
                        }
                    }
                    OutputStartForDebug(debugFileInfo);
                    //OutputFileInfo(inputs, "入力ファイル:");
                    //OutputFileInfo(output, "出力ファイル:");
                    //OutputFileInfo(symbol, "シンボルファイル:");
                    //OutputFileInfo(debugFileInfo, "デバッグファイル:");

                }
                */

                if (inputs == default || inputs.Length == 0)
                {
                    throw new ArgumentException($"入力ファイルが指定されていません。");
                }

                if (outputFiles == default)
                {
                    throw new ArgumentException($"出力ファイルが指定されていません。");
                }

                foreach (var item in outputFiles)
                {
                    if (inputs.Any(m => m.FullName == item.Value.FullName))
                    {
                        throw new ArgumentException($"出力ファイルに入力ファイルは指定できません。ファイル: {item.Value.Name}");
                    }
                }

                var package = new Package(inputs, encodeMode, listMode, outputTrim, AsmISA.Z80);
                if (package.Errors.Length == 0)
                {
                    package.Assemble();
                    if (package.Errors.Length == 0)
                    {
                        package.SaveOutput(outputFiles);
                    }
                }

                package.OutputError();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error:{ex.Message}");
                return false;
            }
            return true;
        }

        private static void OutputStart()
        {
            Console.WriteLine(ProductInfo.ProductLongName);
            Console.WriteLine(ProductInfo.Copyright);
            Console.WriteLine("");
        }

        private static void OutputStartForDebug(FileInfo fileInfo)
        {
            Console.WriteLine(ProductInfo.ProductLongName);
            Console.WriteLine(ProductInfo.Copyright);
            Console.WriteLine("");
            //OutputDebug(new[] { ProductInfo.ProductLongName, ProductInfo.Copyright, $"Assemble start:{DateTime.Now}", "" }, fileInfo, false);
        }

        private static void OutputDebug(string[] targets, FileInfo fileInfo)
        {
            //OutputDebug(targets, fileInfo, true);
        }

        private static void DebugWriteLine(string[] targets, FileInfo fileInfo, bool display)
        {
            if (fileInfo == default)
            {
                return;
            }

            using (var streamWriter = fileInfo.AppendText())
            {
                foreach (var item in targets)
                {
                    if (display)
                    {
                        Console.WriteLine(item);
                    }
                    streamWriter.WriteLine(item);
                }
            }
        }
    }
}
