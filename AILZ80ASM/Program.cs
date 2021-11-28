using System;
using System.IO;
using System.Linq;
using AILZ80ASM.CommandLine;

namespace AILZ80ASM
{
    class Program
    {
        public static int Main(params string[] args)
        {
            var rootCommand = new RootCommand(
              description: "AILight Z80 Assember.");

            rootCommand.AddOption(new Option<FileInfo[]>(
                name: "input",
                aliases: new [] { "-i", "--input" },
                description: "アセンブリ対象のファイルをスペース区切りで指定します。",
                required: true));

            rootCommand.AddOption(new Option<string>(
                name: "inputMode",
                optionName: "inMode",
                aliases: new [] { "-im", "--input-mode" },
                description: "入力ファイルのモードを選択します。",
                defaultValue: "UTF-8",
                parameters: new[] { new Parameter { Name = "UTF-8", Description = "入力ファイルをUTF-8で開きます。" },
                                    new Parameter { Name = "SHIFT_JIS", Description = "入力ファイルをSHIFT_JISで開きます" },},
                required: false));

            rootCommand.AddOption(new Option<FileInfo>(
                name: "output",
                aliases: new string[] { "-o", "--output" },
                description: "出力ファイルを指定します。",
                required: true));

            rootCommand.AddOption(new Option<string>(
                name: "outputMode",
                optionName: "outMode",
                aliases: new [] { "-om", "--output-mode" },
                description: "出力ファイルのモードを選択します。",
                defaultValue: "BIN",
                parameters: new[] { new Parameter { Name = "BIN", Description = "出力ファイルをBIN形式で出力します。" },
                                    new Parameter { Name = "HEX", Description = "（未対応）出力ファイルをHEX形式で出力します。" },
                                    new Parameter { Name = "T88", Description = "（未対応）出力ファイルをT88形式で出力します。" },
                                    new Parameter { Name = "CMT", Description = "（未対応）出力ファイルをCMT形式で出力します。" },},
                required: false));

            rootCommand.AddOption(new Option<FileInfo>(
                name: "symbol",
                aliases: new [] { "-s", "--symbol"  },
                description: "シンボルファイルを指定します。",
                required: false));

            rootCommand.AddOption(new Option<FileInfo>(
                name: "list",
                aliases: new [] { "-l", "--list" },
                description: "リストファイルを指定します。",
                required: false));

            rootCommand.AddOption(new Option<bool>(
                name: "version",
                aliases: new [] { "-v", "--version" },
                description: "バージョンを表示します。",
                required: false,
                optionFunc: (argument) => { return System.Environment.Version.ToString(); }));

            rootCommand.AddOption(new Option<string>(
                name: "help",
                aliases: new [] { "-?", "-h", "--help" },
                description: "ヘルプを表示します。各オプションの詳細は以下の通りに指定してください。例： --help --input-mode",
                required: false,
                optionFunc: (argument) => { return rootCommand.HelpCommand(argument); }));

            try
            {
                OutputStart();

                // 引数の名前とRootCommand.Optionの名前が一致していないと変数展開されない
                if (rootCommand.Parse(args))
                {
                    return Assember(rootCommand.GetValue<FileInfo[]>("input"),
                                    rootCommand.GetValue<string>("inputMode"),
                                    rootCommand.GetValue<FileInfo>("output"),
                                    rootCommand.GetValue<string>("outputMode"),
                                    rootCommand.GetValue<FileInfo>("symbol"),
                                    rootCommand.GetValue<FileInfo>("list")) ? 0 : 1;
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
        static public bool Assember(
                FileInfo[] inputs, string inputMode, FileInfo output, string outputMode, FileInfo symbol, FileInfo list)
        {
            try
            {
                if (inputs == default || inputs.Length == 0)
                {
                    throw new ArgumentException($"入力ファイルが指定されていません。");
                }

                if (output == default)
                {
                    throw new ArgumentException($"出力ファイルが指定されていません。");
                }

                if (output != default && inputs.Any(m => m.FullName == output.FullName))
                {
                    throw new ArgumentException($"出力ファイルに入力ファイルは指定できません。ファイル: {output.Name}");
                }

                if (list != default && inputs.Any(m => m.FullName == list.FullName))
                {
                    throw new ArgumentException($"出力ファイルに入力ファイルは指定できません。ファイル: {list.Name}");
                }

                if (symbol != default && inputs.Any(m => m.FullName == symbol.FullName))
                {
                    throw new ArgumentException($"出力ファイルに入力ファイルは指定できません。ファイル: {symbol.Name}");
                }

                var package = new Package(inputs, inputMode, AsmISA.Z80);
                if (package.Errors.Length == 0)
                {
                    package.Assemble();
                    if (package.Errors.Length == 0)
                    {
                        package.SaveBin(output);
                    }
                }
                package.OutputError();
                if (symbol != default)
                {
                    package.SaveSymbol(symbol);
                }
                if (list != default)
                {
                    package.SaveList(list);
                }
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
    }
}
