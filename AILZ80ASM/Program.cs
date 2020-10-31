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
            rootCommand.Handler =
              CommandHandler.Create<FileInfo[], FileInfo>(Assember);

            return await rootCommand.InvokeAsync(args);
        }

        static public void Assember(
          FileInfo[] input, FileInfo output)
        {

            var package = new Package(input);
            package.Assemble();
            package.Save(output);

            package.OutputError();

        }
    }
}
