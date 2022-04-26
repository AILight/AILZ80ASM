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
            BuildLabel,
            ValidateOutputAddress,
            Complete,
        }

        // エラー保存用
        public List<ErrorLineItem> Errors { get; set; } = default;

        // 展開判断用
        public LineDetailItem LineDetailItemForExpandItem { get; set; } = default;

        // ORG保存用
        public List<AsmORG> AsmORGs { get; set; } = default;

        // ORG命令の保持
        public List<LineDetailItemAddress> LineDetailItemAddreses { get; set; } = default;

        // 循環展開確認用
        public Stack<FileInfo> LoadFiles { get; set; } = default;
        public Stack<Macro> LoadMacros { get; set; } = default;

        // 出力されたファイルを管理
        public List<FileInfo> ListedFiles { get; set; } = default;

        // アセンブル状態
        public AsmStepEnum AsmStep { get; set; } = AsmStepEnum.None;

        // Pragma 一度だけファイルをロードする機能用
        public List<FileInfo> PragmaOnceFiles { get; set; } = default;
    }
}
