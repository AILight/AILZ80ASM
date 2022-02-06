using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using AILZ80ASM.CommandLine;

namespace AILZ80ASM
{
    public class Program
    {
        public const string PROFILE_FILENAME = "AILZ80ASM.json";

        public static int Main(params string[] args)
        {
            try
            {
                // Tranceの書き出し先を削除
                TraceListenerRemoveAll();
                Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));

                // デフォルト設定のロード
                try
                {
                    var processDirectory = Path.GetDirectoryName(Environment.ProcessPath);
                    var profilePath = Path.Combine(processDirectory, PROFILE_FILENAME);
                    if (File.Exists(profilePath))
                    {
                        var defaultProfile = JsonSerializer.Deserialize<AILZ80ASM.Models.Profile>(File.ReadAllText(profilePath));
                        var profileArguments = new List<string>();
                        profileArguments.AddRange(defaultProfile.DefaultOptions);
                        if (defaultProfile.DisableWarnings != default && defaultProfile.DisableWarnings.Count() > 0)
                        {
                            profileArguments.Add("-dw");
                            profileArguments.AddRange(defaultProfile.DisableWarnings);
                        }
                        args = args.Concat(profileArguments).ToArray();
                    }
                }
                catch { }

                // コマンドラインの設定をする
                var rootCommand = AsmCommandLine.SettingRootCommand();

                // 引数の名前とRootCommand.Optionの名前が一致していないと変数展開されない
                if (rootCommand.Parse(args))
                {
                    var result = Assember(rootCommand);
                    return result ? 0 : 1;
                }
                else
                {
                    OutputStart();
                    Trace.WriteLine(rootCommand.ParseMessage);

                    return 2;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"Error:{ex.Message}");
                return 3;
            }
        }

        public static bool Assember(RootCommand rootCommand)
        {
            return Assember(rootCommand.GetValue<FileInfo[]>("input"),
                            rootCommand.GetEncodeMode(),
                            rootCommand.GetOutputFiles(),
                            rootCommand.GetListMode(),
                            rootCommand.GetValue<bool>("outputTrim"),
                            rootCommand.GetValue<FileInfo>("error"),
                            rootCommand.GetValue<Error.ErrorCodeEnum[]>("disableWarningCode"));
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
                FileInfo[] inputs, AsmLoad.EncodeModeEnum encodeMode, Dictionary<AsmLoad.OutputModeEnum, FileInfo> outputFiles, AsmLoad.ListModeEnum listMode, bool outputTrim, FileInfo traceFile, Error.ErrorCodeEnum[] disableWarningCodes)
        {
            var assembleResult = false;
            try
            {
                // Traceの書き出し先を設定
                if (traceFile != default)
                {
                    traceFile.Delete();
                    Trace.Listeners.Add(new TextWriterTraceListener(traceFile.FullName, "error"));
                    Trace.AutoFlush = true;
                }

                OutputStart();


                // デバッグ情報
                /*
                if (debugFileInfo != default)
                {
                    if (debugFileInfo.Exists)
                    {
                        Trace.Write("デバッグファイルが既に存在しています。削除してから書き込みますか？ (y/n)");
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
                
                // 入力内容の確認
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

                if (disableWarningCodes != default)
                {
                    foreach (var item in disableWarningCodes)
                    {
                        if (Error.GetErrorType(item) != Error.ErrorTypeEnum.Warning &&
                            Error.GetErrorType(item) != Error.ErrorTypeEnum.Information)
                        {
                            throw new ArgumentException($"ワーニング出力のキャンセルに以下のコードは指定できません。コード: {item}");
                        }
                    }
                }

                // アセンブル実行

                var package = new Package(inputs, encodeMode, listMode, outputTrim, disableWarningCodes, AsmISA.Z80);
                if (package.Errors.Length == 0)
                {
                    package.Assemble();
                    if (package.Errors.Length == 0)
                    {
                        package.SaveOutput(outputFiles);
                        assembleResult = true;
                    }
                }

                package.OutputError();
            }
            catch (Exception)
            {
                throw;
            }

            return assembleResult;
        }

        private static void TraceListenerRemoveAll()
        {
            var listenerNames = new List<string>();
            foreach (TraceListener item in Trace.Listeners)
            {
                listenerNames.Add(item.Name);
            }
            foreach (var item in listenerNames)
            {
                Trace.Listeners.Remove(item);
            }
        }

        private static void OutputStart()
        {
            Trace.WriteLine(ProductInfo.ProductLongName);
            Trace.WriteLine(ProductInfo.Copyright);
            Trace.WriteLine("");
        }

        private static void OutputStartForDebug(FileInfo fileInfo)
        {
            Trace.WriteLine(ProductInfo.ProductLongName);
            Trace.WriteLine(ProductInfo.Copyright);
            Trace.WriteLine("");
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
                        Trace.WriteLine(item);
                    }
                    streamWriter.WriteLine(item);
                }
            }
        }
    }
}
