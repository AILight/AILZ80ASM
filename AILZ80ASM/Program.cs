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

            try
            {
                OutputStart();

                rootCommand.AddOption(outputOption);
                rootCommand.Handler =
                  CommandHandler.Create<FileInfo[], FileInfo>(Assember);

                return await rootCommand.InvokeAsync(args);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return await Task.FromResult(1);
        }

        static public void Assember(
          FileInfo[] input, FileInfo output)
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
                package.Assemble();
                if (package.Errors.Length == 0)
                {
                    package.Save(output);
                }
                package.OutputError();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error:{ex.Message}");
            }
        }

        private static void OutputStart()
        {
            Console.WriteLine($"*** AILZ80ASM *** Z-80 Assembler, .NET Core version {System.Reflection.Assembly.GetExecutingAssembly().GetName().Version}");
            Console.WriteLine($"Copyright (C) {DateTime.Today.Year:0} by M.Ishino (AILight)");
        }
    }
}
