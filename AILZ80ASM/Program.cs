using AILZ80ASM.AILight;
using AILZ80ASM.Assembler;
using AILZ80ASM.CommandLine;
using AILZ80ASM.Exceptions;
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

                // コマンドラインの設定をする
                var rootCommand = AsmCommandLine.SettingRootCommand();

                // デフォルト設定のロード
                if (!rootCommand.HasHelpArgument(args))
                {
                    try
                    {
                        var processDirectory = Path.GetDirectoryName(Environment.ProcessPath);
                        var profilePath = Path.Combine(processDirectory, PROFILE_FILENAME);
                        if (File.Exists(profilePath))
                        {
                            var profileString = File.ReadAllText(profilePath);
                            var profileArguments = AsmCommandLine.ParseArgumentsFromJsonString(profileString);
                            args = args.Concat(profileArguments).ToArray();
                        }
                    }
                    catch (System.Text.Json.JsonException ex)
                    {
                        throw new Exception($"{PROFILE_FILENAME}:{ex.LineNumber}行目に問題があります。");
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"{PROFILE_FILENAME}:{ex.Message}", ex);
                    }
                }

                // 引数の名前とRootCommand.Optionの名前が一致していないと変数展開されない
                if (rootCommand.Parse(args))
                {
                    var currentDirectory = "";
                    // 実行時のディレクトリを変更する
                    var directoryInfo = rootCommand.GetValue<DirectoryInfo>("changeDirectory");
                    if (directoryInfo != default && !directoryInfo.Exists)
                    {
                        throw new Exception($"アセンブル先のディレクトリが見つかりません。[{directoryInfo.Name}]");
                    }
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
                OutputStart();
                Trace.WriteLine($"Error:{ex.Message}");
                return 3;
            }
            finally
            {
                Trace.Close();
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
                    try
                    {
                        package.Assemble();
                    }
                    catch (ErrorAssembleException ex)
                    {
                        Trace.WriteLine($"アセンブルエラー:{ex.Message}");
                        assembleResult = false;
                    }
                    catch (ErrorLineItemException ex)
                    {
                        Trace.WriteLine($"アセンブルエラー:{ex.Message} ファイル:{ex.ErrorLineItem.LineItem.FileInfo.Name}:{ex.ErrorLineItem.LineItem.LineIndex}");
                        assembleResult = false;
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine($"内部エラー:{ex.Message}");
                        assembleResult = false;
                    }
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
                    
                    if (asmOption.DiffFile)
                    {
                        assembleResult &= package.DiffOutput(outputFiles);
                    }
                    else
                    {
                        assembleResult &= package.SaveOutput(outputFiles);
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine($"ファイルエラー:{ex.Message}");
                    assembleResult = false;
                }

                // エラー結果の出力
                package.OutputError();
                if (asmOption.OutputFiles.Any(m => m.Key == AsmEnum.FileTypeEnum.LST))
                {
                    package.OutputErrorForList();
                }
                package.OutputErrorSummary();

                if (asmOption.DiffFile)
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

    }
}
