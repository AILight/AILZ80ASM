using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AILZ80ASM.Assembler
{
    public class AsmDefinedAddress
    {
        public enum SetByEnum
        {
            None,        // 未設定
            Defined,     // 明示的に定義された（例：コマンドライン等）
            Calculated   // アセンブル処理で算出された
        }

        public ushort? Value { get; private set; }
        public SetByEnum SetBy { get; private set; } = SetByEnum.None;

        /// <summary>
        /// 値が設定されているかどうかを返します。
        /// </summary>
        public bool HasValue => Value.HasValue;

        /// <summary>
        /// 明示的な定義（例：コマンドライン）によって値を設定します。
        /// </summary>
        public void SetByDefined(ushort value)
        {
            Value = value;
            SetBy = SetByEnum.Defined;
        }

        /// <summary>
        /// アセンブル処理の結果として算出された値を設定します。
        /// </summary>
        public void SetByCalculated(ushort value)
        {
            Value = value;
            SetBy = SetByEnum.Calculated;
        }

        /// <summary>
        /// 値と設定元を初期状態にリセットします。
        /// </summary>
        public void Clear()
        {
            Value = null;
            SetBy = SetByEnum.None;
        }

        /// <summary>
        /// デバッグ用の出力（値＋由来）
        /// </summary>
        public override string ToString()
        {
            return Value.HasValue ? $"0x{Value:X4} ({SetBy})" : "(unset)";
        }
    }
}
