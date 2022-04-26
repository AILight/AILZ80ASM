using AILZ80ASM.CommandLine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AILZ80ASM.Assembler
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
                IsSimple = true,
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
                Name = "inputEncode",
                ArgumentName = "mode",
                Aliases = new[] { "-ie", "--input-encode" },
                Description = "入力ファイルのエンコードを選択します。",
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
                Parameters = new[] { 
                                        new Parameter { Name = "bin", ShortCut = "-bin", Description = "出力ファイルをBIN形式で出力します。" },
                                        //new Parameter { Name = "hex", ShortCut = "-hex", Description = "出力ファイルをHEX形式で出力します。（未対応）" },
                                        new Parameter { Name = "t88", ShortCut = "-t88", Description = "出力ファイルをT88形式で出力します。" },
                                        new Parameter { Name = "cmt", ShortCut = "-cmt", Description = "出力ファイルをCMT形式で出力します。" },
                                        new Parameter { Name = "sym", ShortCut = "-sym", Description = "シンボルファイルを出力します。" },
                                        new Parameter { Name = "equ", ShortCut = "-equ", Description = "イコールラベルファイルを出力します。" },
                                        new Parameter { Name = "lst", ShortCut = "-lst", Description = "リストファイルを出力します。" },
                                        new Parameter { Name = "err", ShortCut = "-err", Description = "エラーファイルを出力します。" },
                                        //new Parameter { Name = "dbg", ShortCut = "-dbg", Description = "デバッグファイルを出力します。" },
                                    },
                Required = false
            });

            rootCommand.AddOption(new Option<string>()
            {
                Name = "outputEncode",
                ArgumentName = "mode",
                Aliases = new[] { "-oe", "--output-encode" },
                Description = "出力ファイルのエンコードを選択します。",
                DefaultValue = "auto",
                Parameters = new[] { new Parameter { Name = "auto", Description = "自動判断します。不明な場合はUTF-8で処理します。" },
                                     new Parameter { Name = "utf-8", Description = "入力ファイルをUTF-8で開きます。" },
                                     new Parameter { Name = "shift_jis", Description = "入力ファイルをSHIFT_JISで開きます" },},
                Required = false
            });

            // 隠しコマンド
            rootCommand.AddOption(new Option<FileInfo>()
            {
                Name = "outputBin",
                ArgumentName = "file",
                Aliases = new[] { "-bin" },
                Description = "BIN形式で出力します。（file名を省略可能）",
                Required = false,
                IsShortCut = true,
                IsSimple = true,
                DefaultFunc = (options) => { return GetDefaulFilename(options, ".bin"); }
            });

            /*
            rootCommand.AddOption(new Option<FileInfo>()
            {
                Name = "outputHex",
                ArgumentName = "file",
                Aliases = new[] { "-hex" },
                Description = "HEX形式で出力します。（file名を省略可能）",
                Required = false,
                IsShortCut = true,
                DefaultFunc = (options) => { return GetDefaulFilename(options, ".hex"); }
            });
            */

            rootCommand.AddOption(new Option<FileInfo>()
            {
                Name = "outputT88",
                ArgumentName = "file",
                Aliases = new[] { "-t88" },
                Description = "T88形式で出力します。（file名を省略可能）",
                Required = false,
                IsShortCut = true,
                DefaultFunc = (options) => { return GetDefaulFilename(options, ".t88"); }
            });

            rootCommand.AddOption(new Option<FileInfo>()
            {
                Name = "outputCMT",
                ArgumentName = "file",
                Aliases = new[] { "-cmt" },
                Description = "CMT形式で出力します。（file名を省略可能）",
                Required = false,
                IsShortCut = true,
                DefaultFunc = (options) => { return GetDefaulFilename(options, ".cmt"); }
            });

            rootCommand.AddOption(new Option<FileInfo>()
            {
                Name = "outputSYM",
                ArgumentName = "file",
                Aliases = new[] { "-sym" },
                Description = "シンボルファイルを出力します。（file名を省略可能）",
                Required = false,
                IsShortCut = true,
                IsSimple = true,
                DefaultFunc = (options) => { return GetDefaulFilename(options, ".sym"); }
            });

            rootCommand.AddOption(new Option<FileInfo>()
            {
                Name = "outputEQU",
                ArgumentName = "file",
                Aliases = new[] { "-equ" },
                Description = "イコールラベルファイルを出力します。（file名を省略可能）",
                Required = false,
                IsShortCut = true,
                IsSimple = true,
                DefaultFunc = (options) => { return GetDefaulFilename(options, ".equ"); }
            });

            rootCommand.AddOption(new Option<FileInfo>()
            {
                Name = "outputADR",
                ArgumentName = "file",
                Aliases = new[] { "-adr" },
                Description = "アドレスラベルファイルを出力します。（file名を省略可能）",
                Required = false,
                IsShortCut = true,
                IsSimple = true,
                DefaultFunc = (options) => { return GetDefaulFilename(options, ".adr"); }
            });

            rootCommand.AddOption(new Option<FileInfo>()
            {
                Name = "outputLST",
                ArgumentName = "file",
                Aliases = new[] { "-lst" },
                Description = "リストファイルを出力します。（file名を省略可能）",
                Required = false,
                IsShortCut = true,
                IsSimple = true,
                DefaultFunc = (options) => { return GetDefaulFilename(options, ".lst"); }
            });

            rootCommand.AddOption(new Option<FileInfo>()
            {
                Name = "outputERR",
                ArgumentName = "file",
                Aliases = new[] { "-err" },
                Description = "アセンブル結果を出力します。（file名を省略可能）",
                Required = false,
                IsShortCut = true,
                IsSimple = true,
                DefaultFunc = (options) => { return GetDefaulFilename(options, ".err"); }
            });

            /*
            rootCommand.AddOption(new Option<FileInfo>()
            {
                Name = "debug",
                Aliases = new[] { "-d", "--debug" },
                Description = "デバッグ情報を記録します",
                Required = false,
                IsHide = true,
                DefaultFunc = (options) => { return GetDefaulFilename(options, ".dbg"); }
            });
            */

            rootCommand.AddOption(new Option<string>()
            {
                Name = "listMode",
                ArgumentName = "mode",
                Aliases = new[] { "-lm", "--list-mode" },
                Description = "リストの出力形式を選択します。",
                DefaultValue = "full",
                Parameters = new[] { new Parameter { Name = "simple", Description = "最小の項目で出力します。" },
                                     new Parameter { Name = "middle", Description = "出力アドレス無しで出力します。" },
                                     new Parameter { Name = "full", Description = "出力アドレスを含めて出力します。" },},
                Required = false
            });

            rootCommand.AddOption(new Option<int>()
            {
                Name = "tabSize",
                ArgumentName = "size",
                Aliases = new[] { "-ts", "--tab-size" },
                Description = "TABのサイズを指定します。",
                DefaultValue = "4",
                Required = false
            });

            rootCommand.AddOption(new Option<Error.ErrorCodeEnum[]>()
            {
                Name = "disableWarningCode",
                ArgumentName = "codes",
                Aliases = new[] { "-dw", "--disable-warning" },
                Description = "Warning、Informationをオフにするコードをスペース区切りで指定します。",
                Required = false,
            });

            rootCommand.AddOption(new Option<bool>()
            {
                Name = "unUsedLabel",
                Aliases = new[] { "-ul", "--unused-label" },
                Description = "未使用ラベルを確認します。",
                Required = false,
            });

            rootCommand.AddOption(new Option<DirectoryInfo>()
            {
                Name = "changeDirectory",
                ArgumentName = "directory",
                Aliases = new[] { "-cd", "--change-dir" },
                Description = "アセンブル実行時のカレントディレクトリを変更します。終了時に元に戻ります。",
                Required = false,
            });

            rootCommand.AddOption(new Option<bool>()
            {
                Name = "diffFile",
                Aliases = new[] { "-df", "--diff-file" },
                Description = "アセンブル出力結果のDIFFを取ります。アセンブル結果は出力されません。",
                Required = false,
            });

            rootCommand.AddOption(new Option<bool>()
            {
                Name = "version",
                Aliases = new[] { "-v", "--version" },
                Description = "バージョンを表示します。",
                Required = false,
                IsSimple = true,
                OptionFunc = (argument) => { return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString(); }
            });

            rootCommand.AddOption(new Option<string>()
            {
                Name = "help",
                Aliases = new[] { "-?", "-h", "--help" },
                Description = "ヘルプを表示します。各オプションの詳細ヘルプを表示します。例： -h --input-mode",
                Required = false,
                IsHelp = true,
                IsSimple = true,
                OptionFunc = (argument) => { return rootCommand.HelpCommand(argument); }
            });

            rootCommand.AddOption(new Option<bool>()
            {
                Name = "readme",
                Aliases = new[] { "-??", "--readme" },
                Description = "Readme.mdを表示します。",
                Required = false,
                IsHelp = true,
                IsSimple = true,
                OptionFunc = (argument) => ReadMe(),
            });

            return rootCommand;
        }

        public static Dictionary<AsmEnum.FileTypeEnum, FileInfo[]> GetInputFiles(this RootCommand rootCommand)
        {
            var result = new Dictionary<AsmEnum.FileTypeEnum, FileInfo[]>();

            result.Add(AsmEnum.FileTypeEnum.Z80, rootCommand.GetValue<FileInfo[]>("input"));

            return result;
        }

        public static Dictionary<AsmEnum.FileTypeEnum, FileInfo> GetOutputFiles(this RootCommand rootCommand)
        {
            var result = new Dictionary<AsmEnum.FileTypeEnum, FileInfo>();

            var output = rootCommand.GetValue<FileInfo>("output");
            var outputSelected = rootCommand.GetSelected("output");
            var outputMode = rootCommand.GetValue<string>("outputMode");
            var outputModeSelected = rootCommand.GetSelected("outputMode");
            
            var outputDic = new Dictionary<AsmEnum.FileTypeEnum, string>
            {
                [AsmEnum.FileTypeEnum.BIN] = "outputBin",
                //[AsmEnum.FileTypeEnum.HEX] = "outputHex",
                [AsmEnum.FileTypeEnum.T88] = "outputT88",
                [AsmEnum.FileTypeEnum.CMT] = "outputCMT",
                [AsmEnum.FileTypeEnum.SYM] = "outputSYM",
                [AsmEnum.FileTypeEnum.EQU] = "outputEQU",
                [AsmEnum.FileTypeEnum.ADR] = "outputADR",
                [AsmEnum.FileTypeEnum.LST] = "outputLST",
                [AsmEnum.FileTypeEnum.ERR] = "outputERR",
                //[AsmEnum.FileTypeEnum.DBG] = "outputDBG",
            };

            foreach (var item in outputDic)
            {
                var outputFileInfo = rootCommand.GetValue<FileInfo>(item.Value);
                if (outputFileInfo != default)
                {
                    result.Add(item.Key, outputFileInfo);
                }

            }

            if (result.Count == 0 || outputSelected || outputModeSelected)
            {
                var outputModeEnum = outputMode switch
                {
                    "bin" => AsmEnum.FileTypeEnum.BIN,
                    "hex" => AsmEnum.FileTypeEnum.HEX,
                    "t88" => AsmEnum.FileTypeEnum.T88,
                    "cmt" => AsmEnum.FileTypeEnum.CMT,
                    "lst" => AsmEnum.FileTypeEnum.LST,
                    "sym" => AsmEnum.FileTypeEnum.SYM,
                    "equ" => AsmEnum.FileTypeEnum.EQU,
                    "adr" => AsmEnum.FileTypeEnum.ADR,
                    "dbg" => AsmEnum.FileTypeEnum.DBG,
                    "err" => AsmEnum.FileTypeEnum.ERR,
                    _ => throw new InvalidOperationException()
                };
                result.Add(outputModeEnum, output);
            }

            return result;
        }

        public static AsmEnum.EncodeModeEnum GetInputEncodeMode(this RootCommand rootCommand)
        {
            var outputMode = rootCommand.GetValue<string>("inputEncode");

            return GetEncodeMode(outputMode);
        }

        public static AsmEnum.EncodeModeEnum GetOutputEncodeMode(this RootCommand rootCommand)
        {
            var outputMode = rootCommand.GetValue<string>("outputEncode");

            return GetEncodeMode(outputMode);
        }

        private static AsmEnum.EncodeModeEnum GetEncodeMode(string target)
        {
            var encodeMode = target switch
            {
                "auto" => AsmEnum.EncodeModeEnum.AUTO,
                "utf-8" => AsmEnum.EncodeModeEnum.UTF_8,
                "shift_jis" => AsmEnum.EncodeModeEnum.SHIFT_JIS,
                _ => throw new InvalidOperationException()
            };

            return encodeMode;
        }

        public static AsmEnum.ListFormatEnum GetListMode(this RootCommand rootCommand)
        {
            var listMode = rootCommand.GetValue<string>("listMode");

            var encodeMode = listMode switch
            {
                "simple" => AsmEnum.ListFormatEnum.Simple,
                "middle" => AsmEnum.ListFormatEnum.Middle,
                "full" => AsmEnum.ListFormatEnum.Full,
                _ => throw new InvalidOperationException()
            };

            return encodeMode;
        }

        public static int GetTabSize(this RootCommand rootCommand)
        {
            var tabSize = rootCommand.GetValue<int>("tabSize");

            return tabSize;
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

            var inputFile = inputOption.Value.First();
            var extension = $".{outputModeOption.Value}";
            var fileName = GetChangeExtension(inputFile, extension);

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

            var inputFile = inputOption.Value.First();
            var fileName = GetChangeExtension(inputFile, extension);

            return new[] { fileName };
        }

        private static string GetChangeExtension(FileInfo fileInfo, string extension)
        {
            if (fileInfo.Extension.ToUpper() == fileInfo.Extension)
            {
                extension = extension.ToUpper();
            }
            else
            {
                extension = extension.ToLower();
            }

            var fileName = Path.ChangeExtension(fileInfo.FullName, extension);

            return fileName;
        }

        /// <summary>
        /// ReadMe.MD
        /// </summary>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="FileLoadException"></exception>
        private static string ReadMe()
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var resourceName = "AILZ80ASM.Documents.README.md";

            if (!assembly.GetManifestResourceNames().Any(m => m == resourceName))
            {
                throw new FileNotFoundException("リソースが見つかりませんでした。", resourceName);
            }

            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == default)
                {
                    throw new FileLoadException("リソースが読み込みできませんでした。", resourceName);
                }

                using (var reader = new StreamReader(stream))
                {
                    var readme = reader.ReadToEnd();

                    readme = Regex.Replace(readme, "^######", "□□", RegexOptions.Multiline);
                    readme = Regex.Replace(readme, "^#####", "□□", RegexOptions.Multiline);
                    readme = Regex.Replace(readme, "^####", "■■", RegexOptions.Multiline);
                    readme = Regex.Replace(readme, "^###", "■■", RegexOptions.Multiline);
                    readme = Regex.Replace(readme, "^##", "■", RegexOptions.Multiline);
                    readme = Regex.Replace(readme, "^#", "■", RegexOptions.Multiline);
                    readme = Regex.Replace(readme, "^- ", " ・ ", RegexOptions.Multiline);
                    readme = Regex.Replace(readme, "^ - ", " ・ ", RegexOptions.Multiline);
                    readme = Regex.Replace(readme, "^\t- ", " 　 → ", RegexOptions.Multiline);
                    readme = Regex.Replace(readme, "^```", $"{Environment.NewLine}{new String('-', 80)}{Environment.NewLine}", RegexOptions.Multiline);

                    return readme;
                }
            }
        }

    }
}
