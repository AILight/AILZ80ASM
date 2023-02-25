using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AILZ80ASM.Assembler
{
    public class AsmSuperAssemble
    {
        private const int MAX_ASSEMBLY_COUNT = 64; // こんなに回ったら嫌だよね？（笑）

        public List<ErrorLineItem> Errors { get; set; } = new List<ErrorLineItem>();

        public List<AsmAddress> AsmORG_AddressList { get; set; } = new List<AsmAddress>();

        public AsmSuperAssemble() {  }

        public int LoopCounter { get; private set; }

        /// <summary>
        /// SuperAssemble
        /// </summary>
        public bool IsLoop => MAX_ASSEMBLY_COUNT > LoopCounter;

        public bool IsSuperAssemble => this.IsInitializeOutputAddress;

        public bool IsInitializeOutputAddress => AsmORG_AddressList.Any();

        /// <summary>
        /// アセンブルが完了したら呼び出す
        /// </summary>
        public void Assembled()
        {
            LoopCounter++;
        }

        /// <summary>
        /// 同じエラーが生成されていたら True
        /// </summary>
        /// <param name="assembleErrors"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public bool HasSameError(ErrorLineItem[] assembleErrors)
        {
            return assembleErrors.Any(m => Errors.Any(n => m.ErrorCode == n.ErrorCode &&
                                                           m.LineItem.LineString == n.LineItem.LineString &&
                                                           m.LineItem.LineIndex == n.LineItem.LineIndex &&
                                                           (m.LineItem?.FileInfo.FullName ?? "") == (n.LineItem?.FileInfo.FullName ?? "")));

        }
    }
}
