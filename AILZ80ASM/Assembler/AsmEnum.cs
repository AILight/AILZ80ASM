using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AILZ80ASM.Assembler
{
    public class AsmEnum
    {
        /// <summary>
        /// ファイルタイプ
        /// </summary>
        public enum FileTypeEnum
        {
            Z80,
            BIN,
            HEX,
            T88,
            CMT,
            LST,
            SYM,
            ERR,
            DBG,
        }

        /// <summary>
        /// ファイルのデータタイプ
        /// </summary>
        public enum FileDataTypeEnum
        {
            Binary,
            Text,
        }

        /// <summary>
        /// リスティングファイルの形式
        /// </summary>
        public enum ListFormatEnum
        {
            Simple,
            Middle,
            Full,
        }

        /// <summary>
        /// エンコードのモード
        /// </summary>
        public enum EncodeModeEnum
        {
            AUTO,
            UTF_8,
            SHIFT_JIS,
        }

    }
}
