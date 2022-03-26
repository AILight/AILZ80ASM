using AILZ80ASM.AILight;
using AILZ80ASM.Assembler;
using AILZ80ASM.CommandLine;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;

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
                    var currentDirectory = "";
                    // 実行時のディレクトリを変更する
                    var directoryInfo = rootCommand.GetValue<DirectoryInfo>("changeDirectory");
                    if (directoryInfo != default)
                    {
                        currentDirectory = System.Environment.CurrentDirectory;
                        System.Environment.CurrentDirectory = directoryInfo.FullName;

                        // 再度パースを行う。ディレクトリを再設定するため。
                        rootCommand.Parse(args);
                    }

                    try
                    {
                        var result = Assember(rootCommand);
                        return result ? 0 : 1;
                    }
                    finally
                    {
                        // 保存したディレクトリに戻る
                        if (!string.IsNullOrEmpty(currentDirectory) &&
                            Directory.Exists(currentDirectory))
                        {
                            System.Environment.CurrentDirectory = currentDirectory;
                        }
                    }
                }
                else
                {
                    if (!rootCommand.GetSelected("version"))
                    {
                        OutputStart();
                    }
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

        /// <summary>
        /// アセンブル
        /// </summary>
        /// <param name="rootCommand"></param>
        /// <returns></returns>
        public static bool Assember(RootCommand rootCommand)
        {
            var asmOption = new AsmOption(rootCommand);
            return Assember(asmOption);
        }

        /// <summary>
        /// アセンブル
        /// </summary>
        /// <param name="asmOption"></param>
        /// <returns></returns>
        public static bool Assember(AsmOption asmOption)
        {
            var assembleResult = false;
            try
            {
                // 入力ファイルのエンコードだけ調査する
                if (asmOption.InputEncodeMode == AsmEnum.EncodeModeEnum.AUTO &&
                    asmOption.OutputEncodeMode == AsmEnum.EncodeModeEnum.AUTO)
                {
                    try
                    {
                        asmOption.CheckEncodeMode();
                    }
                    catch { }
                }

                // Traceの書き出し先を設定
                if (asmOption.OutputFiles.ContainsKey(AsmEnum.FileTypeEnum.ERR))
                {
                    var traceFile = asmOption.OutputFiles[AsmEnum.FileTypeEnum.ERR];

                    traceFile.Delete();
                    var streamWriter = new StreamWriter(traceFile.FullName, true, AsmLoad.GetEncoding(asmOption.DecidedOutputEncodeMode));
                    Trace.Listeners.Add(new TextWriterTraceListener(streamWriter, "error"));
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

                asmOption.Validate();

                // アセンブル実行
                var package = new Package(asmOption, AsmISA.Z80);
                //　パッケージステータス
                package.Trace_Information();

                // エラーが無ければアセンブル
                if (package.Errors.Length == 0)
                {
                    package.Assemble();
                }

                // 出力調整
                try
                {
                    assembleResult = package.Errors.Length == 0;
                    var outputFiles = asmOption.OutputFiles.Where(m => m.Key != AsmEnum.FileTypeEnum.ERR).ToDictionary(k => k.Key, v => v.Value);
                    // エラー発生時は、リスティングファイルだけでも出力する
                    if (package.Errors.Length != 0)
                    {
                        outputFiles = outputFiles.Where(m => m.Key == AsmEnum.FileTypeEnum.LST).ToDictionary(k => k.Key, v => v.Value);
                    }
                    
                    if (asmOption.FileDiff)
                    {
                        assembleResult = package.DiffOutput(outputFiles);
                    }
                    else
                    {
                        assembleResult = package.SaveOutput(outputFiles);
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine($"ファイルエラー:{ex.Message}");
                    assembleResult = false;
                }

                package.OutputError();

                if (asmOption.FileDiff)
                {
                    if (package.Errors.Length > 0)
                    {
                        Trace.WriteLine("アセンブル時にエラーが発生したため、ファイルの比較が中止されました。");
                    }
                }
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

        private static void OutputFileList(Dictionary<AsmEnum.FileTypeEnum, FileInfo> outputFiles)
        {
            foreach (var outputFile in outputFiles)
            {
                Trace.WriteLine($"{outputFile.Value.Name}");
            }
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
