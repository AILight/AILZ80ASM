using System;
using System.Collections.Generic;

namespace AILZ80ASM.Assembler
{
    public static class Error
    {
        public enum ErrorTypeEnum
        {
            Error,
            Warning,
            Information,
        }

        public enum ErrorCodeEnum
        {
            E0000,

            E0001,
            E0002,
            E0003,
            E0004,
            E0005,
            E0006,
            E0007,
            E0008,
            E0009,
            E0010,
            E0011,
            //E0012,
            E0013,
            E0014,
            E0015,
            E0016,
            E0017,
            E0018,
            E0019,
            E0020,
            E0021,
            E0022,
            E0023,
            E0024,
            E0025,

            E1011,
            E1012,
            E1013,
            E1014,
            E1015,
            E1016,

            E1021,
            E1022,
            E1023,
            E1024,

            E1031,
            E1032,

            E1041,
            E1042,

            E1051,
            E1052,

            // Include
            //E2001,
            E2002,
            E2003,
            E2004,
            E2005,
            E2006,
            //E2007,
            E2008,
            E2009,

            // CharMap
            E2101,
            //E2102,
            E2103,
            E2104,
            E2105,
            E2106,
            E2107,
            E2108,

            // マクロ
            E3001,
            E3002,
            //E3003,
            E3004,
            E3005,
            E3006,
            E3007,
            E3008,
            //E3009,
            E3010,

            // ファンクション
            E4001,
            E4002,
            E4003,
            E4004,
            E4005,

            // ALIGN ブロック
            E5001,
            E5002,

            W0001,
            W0002,
            W0003,

            W9001,
            W9002,
            W9003,
            W9004,
            W9005,

            I0001,
            I0002,
        }

        public static ErrorTypeEnum GetErrorType(ErrorCodeEnum errorCode)
        {
            if (errorCode < ErrorCodeEnum.W0001)
            {
                return ErrorTypeEnum.Error;
            }
            else if (errorCode < ErrorCodeEnum.I0001)
            {
                return ErrorTypeEnum.Warning;
            }
            else
            {
                return ErrorTypeEnum.Information;
            }
        }

        public static string GetMessage(ErrorCodeEnum errorCode)
        {
            return GetMessage(errorCode, default);
        }

        public static string GetMessage(ErrorCodeEnum errorCode, params object[] parameters)
        {
            var message = ErrorMessages[errorCode];
            return parameters == default ? message : string.Format(message, parameters);
        }

        private static readonly Dictionary<ErrorCodeEnum, string> ErrorMessages = new Dictionary<ErrorCodeEnum, string>()
        {
            // 処理できないエラー
            [ErrorCodeEnum.E0000] = "処理できないエラー。開発者にお伝えください。{0}",

            [ErrorCodeEnum.E0001] = "無効な命令が指定されました。{0}",
            [ErrorCodeEnum.E0002] = "ビット変換で有効桁数をオーバーしました。[指定ビット数:{0}, 指定値{1}]",
            [ErrorCodeEnum.E0003] = $"相対ジャンプの範囲違反、有効範囲は{SByte.MinValue}～{SByte.MaxValue}までです。[{{0}}]",
            [ErrorCodeEnum.E0004] = "演算、もしくはラベルの解決に失敗しました。定義を確認してください。[{0}]",
            [ErrorCodeEnum.E0005] = "エスケープシーケンスの表記が間違っています。[{0}]",
            [ErrorCodeEnum.E0006] = "0 で除算しようとしました。 [{0}]",
            [ErrorCodeEnum.E0007] = "ラベルが循環参照されました。自分自身が参照されていないか確認してください。[{0}]",
            [ErrorCodeEnum.E0008] = "ラベルの参照が曖昧です。次の指定方法を検討してください。[{0}]",
            [ErrorCodeEnum.E0009] = "ORGに指定した出力アドレス上に既にアセンブリ結果があります。",
            [ErrorCodeEnum.E0010] = "出力アドレスに影響する場所では$$は使えません。",
            [ErrorCodeEnum.E0011] = "参照したラベルのプログラムアドレスが確定できませんでした。{0}",
            [ErrorCodeEnum.E0013] = "ラベルの指定が間違っています。記号や予約語や数値に変換できる文字列は使えません。",
            [ErrorCodeEnum.E0014] = "同名のラベルが既に定義されています。{0}",
            [ErrorCodeEnum.E0015] = "ALIGNに指定したアドレスは、2のべき乗である必要があります。",
            [ErrorCodeEnum.E0016] = "指定できる値は、0～7です。[{0}]",
            [ErrorCodeEnum.E0017] = "ラベル名と同じネームスペース名は付ける事が出来ません。[{0}]",
            [ErrorCodeEnum.E0018] = "ネームスペース名と同じラベル名は付ける事が出来ません。[{0}]",
            [ErrorCodeEnum.E0019] = "指定できる値は、0です。[{0}]",
            [ErrorCodeEnum.E0020] = "指定できる値は、00H,08H,10H,18H,20H,28H,30H,38Hです。[{0}]",
            [ErrorCodeEnum.E0021] = "DBへの変換に失敗しました。[{0}]",
            [ErrorCodeEnum.E0022] = "DWへの変換に失敗しました。[{0}]",
            [ErrorCodeEnum.E0023] = "式の解析に失敗しました。式を確認してください。[{0}]",
            [ErrorCodeEnum.E0024] = "DBFILへの変換に失敗しました。[{0}]",
            [ErrorCodeEnum.E0025] = "DWFILへの変換に失敗しました。[{0}]",

            //[ErrorCodeEnum.E9999] = $"オフセット値の範囲違反、有効範囲は{SByte.MinValue}～{SByte.MaxValue}までです。[{{0}}]",

            // リピート
            [ErrorCodeEnum.E1011] = "REPEAT (REPT) に対応するEND REPEAT (ENDM) が見つかりませんでした。",
            [ErrorCodeEnum.E1012] = "END REPEAT (ENDM) が先に見つかりました。",
            [ErrorCodeEnum.E1013] = "REPEAT (REPT) LASTに指定した値が不正です。負の値を指定してください。[{0}]",
            [ErrorCodeEnum.E1014] = "REPEAT (REPT) では、ローカルラベルしか使えません。",
            [ErrorCodeEnum.E1015] = "REPEAT (REPT) に指定した値が不正です。[{0}]",
            [ErrorCodeEnum.E1016] = "REPEAT (REPT) LASTに指定した値が不正です。削除できる命令数を超えています。",

            // コンディショナル
            [ErrorCodeEnum.E1021] = "#IFに対応する#ENDIFが見つかりませんでした。",
            [ErrorCodeEnum.E1022] = "#ENDIFが先に見つかりました。",
            [ErrorCodeEnum.E1023] = "#ELSEの後に#ELIF、#ELSEは設定できません。",
            [ErrorCodeEnum.E1024] = "#ELIF、#ELSE、#ENDIFにラベルは設定できません。",

            // エラー
            [ErrorCodeEnum.E1031] = "#ERROR:{0}",
            [ErrorCodeEnum.E1032] = "#ERRORにラベルは設定できません。",

            // プリント
            [ErrorCodeEnum.E1041] = "引数の設定が間違っています。{0}",
            [ErrorCodeEnum.E1042] = "#PRINTにラベルは設定できません。",

            // リスト
            [ErrorCodeEnum.E1051] = "引数の設定が間違っています。{0}",
            [ErrorCodeEnum.E1052] = "#LISTにラベルは設定できません。",

            // Include
            //[ErrorCodeEnum.E2001] = "",
            [ErrorCodeEnum.E2002] = "Include ファイルが存在しませんでした。[{0}]",
            [ErrorCodeEnum.E2003] = "Include 既に読み込み済みのファイルです。[{0}]",
            [ErrorCodeEnum.E2004] = "Include 開始アドレスの指定が間違っています。",
            [ErrorCodeEnum.E2005] = "Include 長さの指定が間違っています。",
            [ErrorCodeEnum.E2006] = "Include 開始アドレスがファイルの長さを超えています。",
            //[ErrorCodeEnum.E2007] = "",
            [ErrorCodeEnum.E2008] = "Include ファイルタイプには、TEXTかBINARYを指定してください。",
            [ErrorCodeEnum.E2009] = "Include ファイルタイプ:TEXTでは、開始位置、長さの指定は出来ません。",

            // CharMap
            [ErrorCodeEnum.E2101] = "CharMap ファイルが見つかりませんでした。[{0}]",
            //[ErrorCodeEnum.E2102] = "",
            [ErrorCodeEnum.E2103] = "CharMap ファイルの指定が間違っています。\"\"で囲って下さい。[{0}]",
            [ErrorCodeEnum.E2104] = "CharMap ファイルの変換に失敗しました。[{0}]",
            [ErrorCodeEnum.E2105] = "CharMap 変換テーブルに値が見つかりませんでした。[{0}]",
            [ErrorCodeEnum.E2106] = "CharMap 変換テーブルが見つかりませんでした。[{0}]",
            [ErrorCodeEnum.E2107] = "CharMap ラベルは設定できません。",
            [ErrorCodeEnum.E2108] = "CharMap 既に設定済みです。[{0}]",

            // マクロ
            [ErrorCodeEnum.E3001] = "MACROに対応するEND MACRO (ENDM) が見つかりませんでした。",
            [ErrorCodeEnum.E3002] = "END MACRO (ENDM) が先に見つかりました。",
            //[ErrorCodeEnum.E3003] = ",
            [ErrorCodeEnum.E3004] = "MACROの引数の数が一致していません。",
            [ErrorCodeEnum.E3005] = "MACROの引数名が有効ではありません。記号や予約語は使えません。[{0}]",
            [ErrorCodeEnum.E3006] = "MACROでは、ローカルラベル以外は使えません。",
            [ErrorCodeEnum.E3007] = "MACROの名前が有効ではありません。記号や予約語は使えません。",
            [ErrorCodeEnum.E3008] = "MACROの中から自分自身のMACROを呼び出すことは出来ません。",
            //[ErrorCodeEnum.E3009] = "",
            [ErrorCodeEnum.E3010] = "同名のMACROが既に定義されています。",

            // ファンクション
            [ErrorCodeEnum.E4001] = "同名のFunctionが既に定義されています。",
            [ErrorCodeEnum.E4002] = "Functionの名前が有効ではありません。記号は使えません。",
            [ErrorCodeEnum.E4003] = "Functionの再起呼び出しの回数が閾値を超えました。",
            [ErrorCodeEnum.E4004] = "Functionの引数の数が一致していません。",
            [ErrorCodeEnum.E4005] = "Functionの引数名が有効ではありません。[{0}]",

            // ALIGN ブロック
            [ErrorCodeEnum.E5001] = "ALIGN BLOCK に対応するENDM が見つかりませんでした。",
            [ErrorCodeEnum.E5002] = "ALIGN BLOCK ブロック内のアセンブル結果がALIGN値を超えました。{0}",

            // ワーニング
            [ErrorCodeEnum.W0001] = "1バイトの指定場所に、[ 0x{0:X} : {0} ]が設定されています。1バイトに丸められます。",
            [ErrorCodeEnum.W0002] = "2バイトの指定場所に、[ 0x{0:X} : {0} ]が設定されています。2バイトに丸められます。",
            [ErrorCodeEnum.W0003] = "1バイト（符号付き）の指定場所に、[ 0x{0:X} : {0} ]が設定されています。1バイトに丸められます。",

            // あいまいさの許容
            [ErrorCodeEnum.W9001] = "(IX)は、(IX+0)として処理されました。",
            [ErrorCodeEnum.W9002] = "(IY)は、(IY+0)として処理されました。",
            [ErrorCodeEnum.W9003] = "SUB A,は、SUBとして処理されました。",
            [ErrorCodeEnum.W9004] = "EX HL,DE は、EX DE,HLとして処理されました。",
            [ErrorCodeEnum.W9005] = "ローカルラベルの「:」は、無いものとして処理されました。",

            [ErrorCodeEnum.I0001] = "#PRINT: {0}",
            [ErrorCodeEnum.I0002] = "未使用ラベル: {0}"
        };
    }
}
