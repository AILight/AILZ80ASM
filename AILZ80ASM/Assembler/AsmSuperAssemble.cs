using AILZ80ASM.SuperAssembles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AILZ80ASM.Assembler
{
    public class AsmSuperAssemble
    {
        private const int MAX_ASSEMBLY_COUNT = 256; // スーパーアセンブルモード単位の最大試行回数

        private List<ErrorLineItem> Errors { get; set; } = new List<ErrorLineItem>();
        private int MaxLoopCount { get; set; } = MAX_ASSEMBLY_COUNT;

        public bool IsLoop => MaxLoopCount > LoopCounter;
        public int LoopCounter { get; private set; } = 1;

        // E0010 対応
        public bool IsE0010 => E0010_AddressList.Any();
        public List<AsmAddress> E0010_AddressList { get; set; } = new List<AsmAddress>();

        // E0011 対応
        public bool IsE0011 => E0011_AddressDictionary.Any();
        public Dictionary<string, UInt16> E0011_AddressDictionary { get; set; } = new Dictionary<string, ushort>();

        public AsmSuperAssemble() { }


        /// <summary>
        /// スーパーアセンブルを実行する時に呼び出す（終了条件の設定）
        /// </summary>
        public void StartSuperAssemble()
        {
            MaxLoopCount = LoopCounter + MAX_ASSEMBLY_COUNT;
        }

        /// <summary>
        /// スーパーアセンブルが終わった時に呼び出す（終了カウンターを進める）
        /// </summary>
        public void EndSuperAssemble()
        {
            LoopCounter++;
        }

        /// <summary>
        /// 同じエラーエラーが含まれているか？
        /// </summary>
        /// <returns>同じエラーが含まれていたら True</returns>
        public bool HasSameError(AsmLoad asmLoad)
        {
            return Errors.Any(m => asmLoad.HasSameError(m));
        }

        /// <summary>
        /// スーパーアセンブルを行うための対象処理を取り出す
        /// </summary>
        /// <param name="asmLoad"></param>
        /// <returns></returns>
        public ISuperAssemble GetSuperAssembler(AsmLoad asmLoad)
        {
            // 過去と同じエラーを検出、ループの上限に達した場合
            if (HasSameError(asmLoad) || !this.IsLoop)
            {
                // リセットビルドを行って終了する
                return new SP_Reset();
            }


            // 対象のSPアセンブルを処理する
            foreach (var suerAssemble in SuperAssembles())
            {
                if (suerAssemble.CheckSuperAssemble(asmLoad))
                {
                    return suerAssemble;
                }
            }

            if (LoopCounter == 1)
            {
                // スーパーアセンブル対象のエラーが見つからなかった場合には、何も市内
                return default;
            }
            else
            {
                // SPアセンブルを実行した事があれば、リセットビルドを行って終了する（エラーメッセージ再作成のため）
                return new SP_Reset();
            }
        }

        /// <summary>
        /// スーパーアセンブルモード対応の処理を積む
        /// </summary>
        /// <returns></returns>
        private IEnumerable<ISuperAssemble> SuperAssembles()
        {
            // 処理順に並べる
            yield return new SP_E0010();
            yield return new SP_E0011();
        }

        /// <summary>
        /// 同一エラーを確認するためのエラーを追加する
        /// </summary>
        /// <param name="errorLineItem"></param>
        public void AddError(ErrorLineItem errorLineItem)
        {
            this.Errors.Add(errorLineItem);
        }
    }
}
