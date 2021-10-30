using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Threading.Tasks;

namespace AILZ80ASM
{
    class Program
    {
        public static async Task<int> Main(params string[] args)
        {
            var rootCommand = new RootCommand(
              description: "AILight Z80 Assember.");

            var inputOption = new Option(
              aliases: new string[] { "--input", "-i" }
              , description: "アセンブリ対象のファイルをカンマ区切りで指定します。")
            { Argument = new Argument<FileInfo[]>() };
            rootCommand.AddOption(inputOption);

            var outputOption = new Option(
              aliases: new string[] { "--output", "-o" }
              , description: "出力ファイルを指定します。")
            { Argument = new Argument<FileInfo>() };
            rootCommand.AddOption(outputOption);

            var symbolOption = new Option(
              aliases: new string[] { "--symbol", "-s" }
              , description: "シンボルファイルを指定します。")
            { Argument = new Argument<FileInfo>(), IsRequired = false };
            rootCommand.AddOption(symbolOption);

            var listOption = new Option(
              aliases: new string[] { "--list", "-l" }
              , description: "リストファイルを指定します。")
            { Argument = new Argument<FileInfo>(), IsRequired = false };
            rootCommand.AddOption(listOption);

            try
            {
                OutputStart();

                // 引数の名前とRootCommand.Optionの名前が一致していないと変数展開されない
                rootCommand.Handler =
                  CommandHandler.Create<FileInfo[], FileInfo, FileInfo, FileInfo>(Assember);

                return await rootCommand.InvokeAsync(args);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return await Task.FromResult(1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        static public void Assember(
          FileInfo[] input, FileInfo output, FileInfo symbol, FileInfo list)
        {
            try
            {
                if (input == default)
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
            }
        }

        private static void OutputStart()
        {
            Console.WriteLine(ProductInfo.ProductLongName);
            Console.WriteLine(ProductInfo.Copyright);
        }
    }
}
