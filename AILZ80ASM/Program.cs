using System;
using System.IO;
using System.Threading.Tasks;
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
                aliases: new string[] { "-i", "--input" },
                description: "アセンブリ対象のファイルをスペース区切りで指定します。",
                required: true));

            rootCommand.AddOption(new Option<FileInfo>(
                name: "output",
                aliases: new string[] { "-o", "--output" },
                description: "出力ファイルを指定します。",
                required: true));

            rootCommand.AddOption(new Option<FileInfo>(
                name: "symbol",
                aliases: new string[] { "-s", "--symbol"  },
                description: "シンボルファイルを指定します。",
                required: false));

            rootCommand.AddOption(new Option<FileInfo>(
                name: "list",
                aliases: new string[] { "-l", "--list" },
                description: "リストファイルを指定します。",
                required: false));

            rootCommand.AddOption(new Option<bool>(
                name: "version",
                aliases: new string[] { "-v", "--version" },
                description: "バージョンを表示します。",
                required: false,
                optionFunc: () => { return System.Environment.Version.ToString(); }));

            rootCommand.AddOption(new Option<bool>(
                name: "version",
                aliases: new string[] { "-?", "-h", "--help" },
                description: "バージョンを表示します。",
                required: false,
                optionFunc: () => { return rootCommand.HelpMessage; }));

            try
            {
                OutputStart();

                // 引数の名前とRootCommand.Optionの名前が一致していないと変数展開されない
                if (rootCommand.Parse(args))
                {
                    return Assember(rootCommand.GetValue<FileInfo[]>("input"),
                                    rootCommand.GetValue<FileInfo>("output"),
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
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        static public bool Assember(
                FileInfo[] input, FileInfo output, FileInfo symbol, FileInfo list)
        {
            try
            {
                if (input == default || input.Length == 0)
                {
                    throw new ArgumentException($"入力ファイルが指定されていません");
                }

                if (output == default)
                {
                    throw new ArgumentException($"出力ファイルが指定されていません");
                }

                var package = new Package(input);
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
