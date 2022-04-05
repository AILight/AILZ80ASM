using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AILZ80ASM.Assembler
{
    public class AsmLoadScope
    {
        // 実行グローバルラベル
        public string GlobalLabelName { get; set; }

        // 実行ラベル
        public string LabelName { get; set; }

        // デフォルトキャラクターマップ
        public string DefaultCharMap { get; set; }

        // アセンブル終了フラグ
        public bool AssembleEndFlg { get; set; } = false;

        // ラベル
        public List<Label> Labels { get; set; } = default;

        // マクロ
        public List<Macro> Macros { get; set; } = default;

        // ファンクション
        public List<Function> Functions { get; set; } = default;

        // ネームスペースの保存
        public List<string> GlobalLabelNames { get; set; } = default;

        public AsmLoadScope CreateScope()
        {
            var asmLoadScope = new AsmLoadScope();

            asmLoadScope.Restore(this);
            asmLoadScope.Labels = Labels;
            asmLoadScope.Macros = Macros;
            asmLoadScope.Functions = Functions;
            asmLoadScope.GlobalLabelNames = GlobalLabelNames;

            return asmLoadScope;
        }

        public AsmLoadScope CreateLocalScope()
        {
            var asmLoadScope = new AsmLoadScope();

            asmLoadScope.Restore(this);
            asmLoadScope.Labels = new List<Label>();
            asmLoadScope.Macros = new List<Macro>();
            asmLoadScope.Functions = new List<Function>();
            asmLoadScope.GlobalLabelNames = new List<string>();

            return asmLoadScope;
        }

        public void Restore(AsmLoadScope scope)
        {
            this.GlobalLabelName = scope.GlobalLabelName;
            this.LabelName = scope.LabelName;
            this.DefaultCharMap = scope.DefaultCharMap;
            this.AssembleEndFlg = scope.AssembleEndFlg;
        }

    }
}
