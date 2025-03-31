using AILZ80ASM.CharMaps;
using AILZ80ASM.LineDetailItems;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AILZ80ASM.Assembler
{
    public class AsmLoadShare
    {
        public enum AsmStepEnum
        {
            None,
            ExpansionItem,
            PreAssemble,
            AdjustAssemble,
            InternalAssemble,
            BuildLabels,
            ValidateLabels,
            ValidateOutputAddress,
            Complete,
        }

        // エラー保存用
        public List<ErrorLineItem> Errors { get; set; } = default;

        // 展開判断用
        public LineDetailItem LineDetailItemForExpandItem { get; set; } = default;

        // ORG保存用
        public List<AsmORG> AsmORGs { get; set; } = default;

        // 循環展開確認用
        public Stack<FileInfo> LoadFiles { get; set; } = default;
        public Stack<Macro> LoadMacros { get; set; } = default;

        // 出力されたファイルを管理
        public List<FileInfo> ListedFiles { get; set; } = default;

        // アセンブル状態
        public AsmStepEnum AsmStep { get; set; } = AsmStepEnum.None;

        // Pragma 一度だけファイルをロードする機能用
        public List<FileInfo> PragmaOnceFiles { get; set; } = default;

        // CharMapConvert
        public CharMapConverter CharMapConverter { get; set; } = default;

        // Listの内容は揮発するのでこちらで保存
        public List<AsmList> AsmLists { get; set; } = default;
        
        // ギャップバイト
        public byte GapByte { get; set; } = byte.MaxValue;

        // エントリーポイント
        public AsmDefinedAddress EntryPoint { get; set; } = default;
        
        // ロードアドレス
        public AsmDefinedAddress LoadAddress { get; set; } = default;

        // スーパーアセンブル用
        public AsmSuperAssemble AsmSuperAssembleMode { get; set; } = default;

        // List出力フラグ
        public bool IsOutputList { get; set; } = true;

        // 出力サイズを計測する
        public List<LineDetailItem> ValidateAssembles { get; set; } = default;

        // チェックブロック用
        public Stack<LineDetailItemCheck> CheckLineDetailItemStack { get; set; } = default;
    }
}
