using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AILZ80ASM.Assembler
{
    public class AsmOption
    {
        // 出力結果にトリムをするか
        public bool OutputTrim { get; set; }
        // ワーニングのオフになる対象一覧
        public Error.ErrorCodeEnum[] DisableWarningCodes { get; set; }
        // 入力Encode
        public AsmEnum.EncodeModeEnum InputEncodeMode { get; set; }
        // 出力Encode
        public AsmEnum.EncodeModeEnum OutputEncodeMode { get; set; } = AsmEnum.EncodeModeEnum.UTF_8;
        // リストのモード
        public AsmEnum.ListFormatEnum ListMode { get; set; } = AsmEnum.ListFormatEnum.Full;

        /*
        public 

        public void Validate()
        {
            // 入力内容の確認
            if (inputs == default || inputs.Length == 0)
            {
                throw new ArgumentException($"入力ファイルが指定されていません。");
            }

            if (outputFiles == default)
            {
                throw new ArgumentException($"出力ファイルが指定されていません。");
            }

            foreach (var item in outputFiles)
            {
                if (inputs.Any(m => m.FullName == item.Value.FullName))
                {
                    throw new ArgumentException($"出力ファイルに入力ファイルは指定できません。ファイル: {item.Value.Name}");
                }
            }

            if (disableWarningCodes != default)
            {
                foreach (var item in disableWarningCodes)
                {
                    if (Error.GetErrorType(item) != Error.ErrorTypeEnum.Warning &&
                        Error.GetErrorType(item) != Error.ErrorTypeEnum.Information)
                    {
                        throw new ArgumentException($"ワーニング出力のキャンセルに以下のコードは指定できません。コード: {item}");
                    }
                }
            }
        }
        */
    }
}
