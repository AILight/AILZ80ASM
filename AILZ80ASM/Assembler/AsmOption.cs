using AILZ80ASM.CommandLine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AILZ80ASM.Assembler
{
    public class AsmOption
    {
        // 入力ファイル
        public Dictionary<AsmEnum.FileTypeEnum, FileInfo[]> InputFiles { get; set; }
        // 出力ファイル
        public Dictionary<AsmEnum.FileTypeEnum, FileInfo> OutputFiles { get; set; }

        // 出力結果にトリムをするか
        public bool OutputTrim { get; set; }

        // 出力結果の差分を取るか
        public bool FileDiff { get; set; }

        // ワーニングのオフになる対象一覧
        public Error.ErrorCodeEnum[] DisableWarningCodes { get; set; }
        // 入力Encode
        public AsmEnum.EncodeModeEnum InputEncodeMode { get; set; }
        // 出力Encode
        public AsmEnum.EncodeModeEnum OutputEncodeMode { get; set; } = AsmEnum.EncodeModeEnum.UTF_8;
        // リストのモード
        public AsmEnum.ListFormatEnum ListMode { get; set; } = AsmEnum.ListFormatEnum.Full;

        public AsmOption()
        {

        }

        /// <summary>
        /// RootCommandからAsmOptionを作成する
        /// </summary>
        /// <param name="rootCommand"></param>
        public AsmOption(RootCommand rootCommand)
        {
            InputFiles = rootCommand.GetInputFiles();
            InputEncodeMode = rootCommand.GetEncodeMode();
            OutputFiles = rootCommand.GetOutputFiles();
            ListMode = rootCommand.GetListMode();
            OutputTrim = rootCommand.GetValue<bool>("outputTrim");
            FileDiff = rootCommand.GetValue<bool>("fileDiff");
            DisableWarningCodes = rootCommand.GetValue<Error.ErrorCodeEnum[]>("disableWarningCode");
        }

        public void Validate()
        {
                        // 入力内容の確認
            if (InputFiles == default || InputFiles.Any(m => m.Value == default || m.Value.Length == 0))
            {
                throw new ArgumentException($"入力ファイルが指定されていません。");
            }

            if (OutputFiles == default || OutputFiles.Count == 0)
            {
                throw new ArgumentException($"出力ファイルが指定されていません。");
            }

            foreach (var outputItem in OutputFiles)
            {
                foreach (var inputItem in InputFiles)
                {
                    if (inputItem.Value.Any(m => m.FullName == outputItem.Value.FullName))
                    {
                        throw new ArgumentException($"出力ファイルに入力ファイルは指定できません。ファイル: {outputItem.Value.Name}");
                    }
                }

            }

            if (DisableWarningCodes != default)
            {
                foreach (var item in DisableWarningCodes)
                {
                    if (Error.GetErrorType(item) != Error.ErrorTypeEnum.Warning &&
                        Error.GetErrorType(item) != Error.ErrorTypeEnum.Information)
                    {
                        throw new ArgumentException($"ワーニング出力のキャンセルに以下のコードは指定できません。コード: {item}");
                    }
                }
            }
        }
    }
}
