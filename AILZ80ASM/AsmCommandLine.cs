using AILZ80ASM.CommandLine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AILZ80ASM
{
    public static class AsmCommandLine
    {
        public static RootCommand SettingRootCommand()
        {
            var rootCommand = new RootCommand(
             description: "AILight Z80 Assember.");

            rootCommand.AddOption(new Option<FileInfo[]>()
            {
                Name = "input",
                ArgumentName = "files",
                Aliases = new[] { "-i", "--input" },
                Description = "アセンブリ対象のファイルをスペース区切りで指定します。",
                Required = true,
                IsDefineOptional = true,
            });

            // 隠しコマンド（将来の拡張用）
            rootCommand.AddOption(new Option<string>()
            {
                Name = "inputMode",
                ArgumentName = "mode",
                Aliases = new[] { "-im", "--input-mode" },
                Description = "入力ファイルのモードを選択します。",
                DefaultValue = "txt",
                Parameters = new[] { new Parameter { Name = "txt", Description = "テキストファイルを入力します。" } },
                Required = false,
                IsHide = true,
            });

            rootCommand.AddOption(new Option<string>()
            {
                Name = "encodeMode",
                ArgumentName = "mode",
                Aliases = new[] { "-e", "--encode" },
                Description = "ファイルのエンコードを選択します。",
                DefaultValue = "auto",
                Parameters = new[] { new Parameter { Name = "auto", Description = "自動判断します。不明な場合はUTF-8で処理します。" },
                                     new Parameter { Name = "utf-8", Description = "入力ファイルをUTF-8で開きます。" },
                                     new Parameter { Name = "shift_jis", Description = "入力ファイルをSHIFT_JISで開きます" },},
                Required = false
            });

            rootCommand.AddOption(new Option<FileInfo>()
            {
                Name = "output",
                ArgumentName = "file",
                Aliases = new string[] { "-o", "--output" },
                Description = "出力ファイルを指定します。",
                Required = true,
                DefaultFunc = (options) => { return GetDefaulFilenameForOutput(options); }
            });

            rootCommand.AddOption(new Option<string>()
            {
                Name = "outputMode",
                ArgumentName = "mode",
                Aliases = new[] { "-om", "--output-mode" },
                Description = "出力ファイルのモードを選択します。",
                DefaultValue = "bin",
                Parameters = new[] { new Parameter { Name = "bin", ShortCut = "-bin", Description = "出力ファイルをBIN形式で出力します。" },
                                     new Parameter { Name = "hex", ShortCut = "-hex", Description = "出力ファイルをHEX形式で出力します。（未対応）" },
                                     new Parameter { Name = "t88", ShortCut = "-t88", Description = "出力ファイルをT88形式で出力します。（ベータ）" },
                                     new Parameter { Name = "cmt", ShortCut = "-cmt", Description = "出力ファイルをCMT形式で出力します。（ベータ）" },},
                Required = false
            });

            // 隠しコマンド
            rootCommand.AddOption(new Option<FileInfo>()
            {
                Name = "outputBin",
                ArgumentName = "file",
                Aliases = new[] { "-bin" },
                Description = "出力ファイルをBIN形式で出力します。",
                Required = false,
                IsHide = true,
                DefaultFunc = (options) => { return GetDefaulFilename(options, ".bin"); }
            });

            // 隠しコマンド
            rootCommand.AddOption(new Option<FileInfo>()
            {
                Name = "outputHex",
                ArgumentName = "file",
                Aliases = new[] { "-hex" },
                Description = "出力ファイルをHEX形式で出力します。",
                Required = false,
                IsHide = true,
                DefaultFunc = (options) => { return GetDefaulFilename(options, ".hex"); }
            });

            // 隠しコマンド
            rootCommand.AddOption(new Option<FileInfo>()
            {
                Name = "outputT88",
                ArgumentName = "file",
                Aliases = new[] { "-t88" },
                Description = "出力ファイルをT88形式で出力します。",
                Required = false,
                IsHide = true,
                DefaultFunc = (options) => { return GetDefaulFilename(options, ".t88"); }
            });

            // 隠しコマンド
            rootCommand.AddOption(new Option<FileInfo>()
            {
                Name = "outputCMT",
                ArgumentName = "file",
                Aliases = new[] { "-cmt" },
                Description = "出力ファイルをCMT形式で出力します。",
                Required = false,
                IsHide = true,
                DefaultFunc = (options) => { return GetDefaulFilename(options, ".cmt"); }
            });

            rootCommand.AddOption(new Option<FileInfo>()
            {
                Name = "symbol",
                ArgumentName = "file",
                Aliases = new[] { "-s", "--symbol" },
                Description = "シンボルファイルを出力します。",
                Required = false,
                DefaultFunc = (options) => { return GetDefaulFilename(options, ".sym"); }
            });

            rootCommand.AddOption(new Option<FileInfo>()
            {
                Name = "list",
                ArgumentName = "file",
                Aliases = new[] { "-l", "--list" },
                Description = "リストファイルを出力します。",
                Required = false,
                DefaultFunc = (options) => { return GetDefaulFilename(options, ".lst"); }
            });

            rootCommand.AddOption(new Option<bool>()
            {
                Name = "outputTrim",
                Aliases = new[] { "-t", "--trim" },
                Description = "DSで確保したメモリが、出力データの最後にある場合にトリムされます。",
                Required = false,
            });

            rootCommand.AddOption(new Option<bool>()
            {
                Name = "version",
                Aliases = new[] { "-v", "--version" },
                Description = "バージョンを表示します。",
                Required = false,
                OptionFunc = (argument) => { return System.Environment.Version.ToString(); }
            });

            rootCommand.AddOption(new Option<FileInfo>()
            {
                Name = "debug",
                Aliases = new[] { "-d", "--debug" },
                Description = "デバッグ情報を記録します",
                Required = false,
                IsHide = true,
                DefaultFunc = (options) => { return GetDefaulFilename(options, ".dbg"); }
            });

            rootCommand.AddOption(new Option<string>()
            {
                Name = "help",
                Aliases = new[] { "-?", "-h", "--help" },
                Description = "ヘルプを表示します。各オプションの詳細ヘルプを表示します。例： -help --input-mode",
                Required = false,
                IsHelp = true,
                OptionFunc = (argument) => { return rootCommand.HelpCommand(argument); }
            });

            return rootCommand;
        }

        public static Dictionary<AsmLoad.OutputModeEnum, FileInfo> GetOutputFiles(this RootCommand rootCommand)
        {
            var result = new Dictionary<AsmLoad.OutputModeEnum, FileInfo>();

            var output = rootCommand.GetValue<FileInfo>("output");
            var outputSelected = rootCommand.GetSelected("output");
            var outputMode = rootCommand.GetValue<string>("outputMode");
            var outputModeSelected = rootCommand.GetSelected("outputMode");
            var outputBin = rootCommand.GetValue<FileInfo>("outputBin");
            var outputHex = rootCommand.GetValue<FileInfo>("outputHex");
            var outputT88 = rootCommand.GetValue<FileInfo>("outputT88");
            var outputCMT = rootCommand.GetValue<FileInfo>("outputCMT");

            if (outputBin != default)
            {
                result.Add(AsmLoad.OutputModeEnum.BIN, outputBin);
            }

            if (outputHex != default)
            {
                result.Add(AsmLoad.OutputModeEnum.HEX, outputHex);
            }

            if (outputT88 != default)
            {
                result.Add(AsmLoad.OutputModeEnum.T88, outputT88);
            }

            if (outputCMT != default)
            {
                result.Add(AsmLoad.OutputModeEnum.CMT, outputCMT);
            }

            if (result.Count == 0 || outputSelected || outputModeSelected)
            {
                var outputModeEnum = outputMode switch
                {
                    "bin" => AsmLoad.OutputModeEnum.BIN,
                    "hex" => AsmLoad.OutputModeEnum.HEX,
                    "t88" => AsmLoad.OutputModeEnum.T88,
                    "cmt" => AsmLoad.OutputModeEnum.CMT,
                    _ => throw new InvalidOperationException()
                };
                result.Add(outputModeEnum, output);
            }

            return result;
        }

        public static AsmLoad.EncodeModeEnum GetEncodeMode(this RootCommand rootCommand)
        {
            var outputMode = rootCommand.GetValue<string>("encodeMode");

            var encodeMode = outputMode switch
            {
                "auto" => AsmLoad.EncodeModeEnum.AUTO,
                "utf-8" => AsmLoad.EncodeModeEnum.UTF_8,
                "shift_jis" => AsmLoad.EncodeModeEnum.SHIFT_JIS,
                _ => throw new InvalidOperationException()
            };

            return encodeMode;
        }

        private static string[] GetDefaulFilenameForOutput(IOption[] options)
        {
            var inputOption = (Option<FileInfo[]>)options.Where(m => m.Name == "input").FirstOrDefault();
            var outputOption = (Option<FileInfo>)options.Where(m => m.Name == "output").FirstOrDefault();
            var outputModeOption = (Option<string>)options.Where(m => m.Name == "outputMode").FirstOrDefault();

            if (inputOption == default || outputOption == default || outputModeOption == default)
            {
                return Array.Empty<string>();
            }

            if (inputOption.Value == default || inputOption.Value.Length == 0)
            {
                return Array.Empty<string>();
            }

            if (string.IsNullOrEmpty(outputModeOption.Value))
            {
                return Array.Empty<string>();
            }

            var fileName = Path.ChangeExtension(inputOption.Value.First().FullName, $".{outputModeOption.Value}");

            return new[] { fileName };
        }

        /// <summary>
        /// デフォルトファイル名の取得
        /// </summary>
        /// <param name="options"></param>
        /// <param name="extension"></param>
        /// <returns></returns>
        private static string[] GetDefaulFilename(IOption[] options, string extension)
        {
            var inputOption = (Option<FileInfo[]>)options.Where(m => m.Name == "input").FirstOrDefault();
            if (inputOption == default)
            {
                return Array.Empty<string>();
            }

            if (inputOption.Value == default || inputOption.Value.Length == 0)
            {
                return Array.Empty<string>();
            }

            var fileName = Path.ChangeExtension(inputOption.Value.First().FullName, extension);

            return new[] { fileName };
        }


    }
}
