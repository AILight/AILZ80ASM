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

        // 出力結果の差分を取るか
        public bool DiffFile { get; set; } = false;

        // エントリーポイント
        public UInt16? EntryPoint { get; set; } = default;

        // ロードアドレス
        public UInt16? LoadAddress { get; set; } = default;

        // タブサイズ
        public int TabSize { get; set; } = 4;

        // ギャップバイト
        public byte GapByte { get; set; } = byte.MaxValue;

        // 未使用ラベルのチェック
        public bool CheckUnuseLabel { get; set; } = false;

        // フォースオプション
        public bool Force { get; set; } = false;

        // スーパーアセンブルモードの不使用
        public bool NoSuperAsmAssemble { get; set; } = false;

        // ワーニングのオフになる対象一覧
        public Error.ErrorCodeEnum[] DisableWarningCodes { get; set; }
        // 入力Encode
        public AsmEnum.EncodeModeEnum InputEncodeMode { get; set; }

        // 入力ファイルのエンコード状況
        public int InputEncodeCount_UTF8 { get; set; } = 0;
        public int InputEncodeCount_SJIS { get; set; } = 0;
        public int InputEncodeCount_ASCII { get; set; } = 0;

        // 出力Encode
        public AsmEnum.EncodeModeEnum OutputEncodeMode { get; set; } = AsmEnum.EncodeModeEnum.UTF_8;

        // バイナリーファイルの省略出力
        public bool ListOmitBinaryFile { get; set; }
        
        // 出力開始アドレスを指定
        public UInt16? StartAddress { get; set; } = default;

        // ラベル定義を設定
        public string[] DefineLabels { get; set; } = default;

        // インクルードのパス
        public DirectoryInfo[] IncludePaths { get; set; } = default;

        // 互換モード（文字列を@付きとして扱います。）
        public bool CompatRawString { get; set; } = false;

        /// <summary>
        /// 出力用の確定したエンコードを返す
        /// </summary>
        public AsmEnum.EncodeModeEnum DecidedOutputEncodeMode 
        {
            get 
            {
                if (OutputEncodeMode != AsmEnum.EncodeModeEnum.AUTO)
                {
                    return OutputEncodeMode;
                }
                if (InputEncodeMode != AsmEnum.EncodeModeEnum.AUTO)
                {
                    return InputEncodeMode;
                }

                var encodeMode = AsmEnum.EncodeModeEnum.UTF_8;
                if (InputEncodeCount_SJIS > InputEncodeCount_UTF8)
                {
                    encodeMode = AsmEnum.EncodeModeEnum.SHIFT_JIS;
                }

                return encodeMode;
            }
        }
        // リストのモード
        public AsmEnum.ListFormatEnum ListMode { get; set; } = AsmEnum.ListFormatEnum.Full;
        public AsmEnum.SymbolFormatEnum SymbolMode { get; set; } = AsmEnum.SymbolFormatEnum.Normal;

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
            InputEncodeMode = rootCommand.GetInputEncodeMode();

            OutputFiles = rootCommand.GetOutputFiles();
            OutputEncodeMode = rootCommand.GetOutputEncodeMode();

            ListMode = rootCommand.GetListMode();
            SymbolMode = rootCommand.GetSymbolMode();
            DiffFile = rootCommand.GetValue<bool>("diffFile");
            EntryPoint = rootCommand.GetValue<ushort?>("entryPoint");
            LoadAddress = rootCommand.GetValue<ushort?>("loadAddress");
            TabSize = rootCommand.GetValue<int>("tabSize");
            CheckUnuseLabel = rootCommand.GetValue<bool>("unUsedLabel");
            Force = rootCommand.GetValue<bool>("force");
            NoSuperAsmAssemble = rootCommand.GetValue<bool>("noSuperAssemble");
            StartAddress = rootCommand.GetValue<ushort?>("startAddress");

            IncludePaths = rootCommand.GetValue<DirectoryInfo[]>("includePaths");

            DisableWarningCodes = rootCommand.GetValue<Error.ErrorCodeEnum[]>("disableWarningCode") ?? Array.Empty<Error.ErrorCodeEnum>();
            // 未使用ラベルをチェックする場合にはDisableWaringCodeを積み込まない
            if (!CheckUnuseLabel)
            {
                DisableWarningCodes = DisableWarningCodes.Concat(new[] { Error.ErrorCodeEnum.I0002 }).Distinct().ToArray();
            }
            else
            {
                DisableWarningCodes = DisableWarningCodes.Where(m => m != Error.ErrorCodeEnum.I0002).ToArray();
            }
            GapByte = rootCommand.GetValue<byte>("gapByte");
            ListOmitBinaryFile = rootCommand.GetValue<bool>("listOmitBinaryFile");
            DefineLabels = rootCommand.GetValue<string[]>("defineLabel");

            CompatRawString = rootCommand.GetValue<bool>("compatRawString");
        }

        /// <summary>
        /// 入力内容のチェック
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public void Validate()
        {
            // 入力内容の確認
            if (InputFiles == default || InputFiles.Any(m => m.Value == default || m.Value.Length == 0))
            {
                throw new ArgumentException($"入力ファイルが指定されていません。");
            }

            // 入力ファイルの存在チェック
            var notFoundFiles = InputFiles.SelectMany(m => m.Value).Where(m => !m.Exists);
            if (notFoundFiles.Any())
            {
                throw new ArgumentException($"入力ファイルが見つかりません。ファイル: {notFoundFiles.First().Name}");
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

            // インクルードの存在チェック
            if (IncludePaths != default)
            {
                foreach (var item in IncludePaths)
                {
                    if (!item.Exists)
                    {
                        throw new ArgumentException($"インクルードのパスが見つかりません。ファイル: {item.ToString()}");
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

        /// <summary>
        /// 事前にエンコードモードをチェックする
        /// </summary>
        public void CheckEncodeMode()
        {
            foreach (var fileInfo in InputFiles.SelectMany(m => m.Value))
            {
                var internalEncodeMode = InternalCheckEncodeMode(fileInfo);
                switch (internalEncodeMode)
                {
                    case AsmEnum.InternalEncodeModeEnum.ASCII:
                        InputEncodeCount_ASCII++;
                        break;
                    case AsmEnum.InternalEncodeModeEnum.UTF_8:
                        InputEncodeCount_UTF8++;
                        break;
                    case AsmEnum.InternalEncodeModeEnum.SHIFT_JIS:
                        InputEncodeCount_SJIS++;
                        break;
                    default:
                        throw new InvalidOperationException();
                }
            }
        }

        /// <summary>
        /// エンコードモードの確認を行います
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <returns></returns>
        public AsmEnum.EncodeModeEnum CheckEncodeMode(FileInfo fileInfo)
        {
            var internalEncodeMode = InternalCheckEncodeMode(fileInfo);
            var encodeMode = internalEncodeMode switch
            {
                AsmEnum.InternalEncodeModeEnum.ASCII => AsmEnum.EncodeModeEnum.UTF_8,
                AsmEnum.InternalEncodeModeEnum.SHIFT_JIS => AsmEnum.EncodeModeEnum.SHIFT_JIS,
                AsmEnum.InternalEncodeModeEnum.UTF_8 => AsmEnum.EncodeModeEnum.UTF_8,
                _ => throw new InvalidOperationException()
            };
            return encodeMode;
        }

        private AsmEnum.InternalEncodeModeEnum InternalCheckEncodeMode(FileInfo fileInfo)
        {
            using var readStream = fileInfo.OpenRead();
            using var memoryStream = new MemoryStream();
            readStream.CopyTo(memoryStream);
            var bytes = memoryStream.ToArray();

            var isUTF8 = AILight.AIEncode.IsUTF8(bytes);
            var isSHIFT_JIS = AILight.AIEncode.IsSHIFT_JIS(bytes);
            var encodeMode = AsmEnum.InternalEncodeModeEnum.UTF_8;

            if (isUTF8 && isSHIFT_JIS)
            {
                return AsmEnum.InternalEncodeModeEnum.ASCII;
            }
            else if (!isUTF8 && isSHIFT_JIS)
            {
                return AsmEnum.InternalEncodeModeEnum.SHIFT_JIS;
            }

            return encodeMode;
        }
    }
}
