using System;
using System.Collections.Generic;
using System.Text;

namespace AILZ80ASM
{
    public static class Error
    {
        public enum ErrorTypeEnum
        {
            Error,
            Warning,
            Infomation,
        }

        public enum ErrorCodeEnum
        {
            E0001,
            E0002,
            E0003,
            E0004,
            E0005,
            E0006,
            E0007,
            E0008,
            E0009,
            E0012,
            E0013,

            E1001,
            E1002,
            E1003,
            E1004,
            E1005,
            E1006,

            E1011,
            E1012,
            E1013,
            E1014,

            E2001,
            E2002,
            E2003,

            W0001,
            I0001,
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
                return ErrorTypeEnum.Infomation;
            }
        }

        public static string GetMessage(ErrorCodeEnum errorCode)
        {
            return ErrorMessages[errorCode];
        }

        private static readonly Dictionary<ErrorCodeEnum, string> ErrorMessages = new Dictionary<ErrorCodeEnum, string>()
        {
            [ErrorCodeEnum.E0001] = "無効な命令が指定されました。",
            [ErrorCodeEnum.E0002] = "バイト変換で有効桁数をオーバーしました。",
            [ErrorCodeEnum.E0003] = "相対ジャンプの範囲違反、有効範囲は{SByte.MinValue}～{SByte.MaxValue}までです。",
            [ErrorCodeEnum.E0004] = "演算に失敗しました。{0}",
            [ErrorCodeEnum.E0005] = "16進数の変換に失敗しました。",
            [ErrorCodeEnum.E0006] = "10進数の変換に失敗しました。",
            [ErrorCodeEnum.E0007] = "8進数の変換に失敗しました。",
            [ErrorCodeEnum.E0008] = "2進数の変換に失敗しました。",
            [ErrorCodeEnum.E0009] = "ORGに指定した出力アドレス上に既にアセンブリ結果があります。",
            [ErrorCodeEnum.E0012] = "データの指定が間違っています。",
            [ErrorCodeEnum.E0013] = "ラベルの指定が間違っています。",
            // マクロ
            [ErrorCodeEnum.E1001] = "MACROに対応するEND MACROが見つかりませんでした。",
            [ErrorCodeEnum.E1002] = "END MACROが先に見つかりました。",
            [ErrorCodeEnum.E1003] = "MACROが重複登録されていますので、名前解決が出来ません。",
            [ErrorCodeEnum.E1004] = "MACROの引数の数が一致していません。",
            [ErrorCodeEnum.E1005] = "MACROの引数名が有効ではありません。",
            [ErrorCodeEnum.E1006] = "MACROでは、ローカルラベル以外は使えません。",

            // マクロリピート
            [ErrorCodeEnum.E1011] = "REPEATに対応するEND REPEATが見つかりませんでした。",
            [ErrorCodeEnum.E1012] = "END REPEATが先に見つかりました。",
            [ErrorCodeEnum.E1013] = "LASTに指定した値が不正です。",
            [ErrorCodeEnum.E1014] = "REPEATでは、ローカルラベルしか使えません。",

            // Include
            [ErrorCodeEnum.E2001] = "Include 展開先でエラーが発生しました。",
            [ErrorCodeEnum.E2002] = "Include ファイルが存在しませんでした。[{0}]",
            [ErrorCodeEnum.E2003] = "Include 既に読み込み済みのファイルです。[{0}]",

            [ErrorCodeEnum.W0001] = "未定義",
            [ErrorCodeEnum.I0001] = "未定義"
        };
    }
}
