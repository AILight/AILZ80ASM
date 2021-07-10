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
            E0010,
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

        private static Dictionary<ErrorCodeEnum, string> ErrorMessages = new Dictionary<ErrorCodeEnum, string>()
        {
            [ErrorCodeEnum.E0001] = "無効な命令が指定されました。",
            [ErrorCodeEnum.E0002] = "バイト変換で有効桁数をオーバーしました。",
            [ErrorCodeEnum.E0003] = "相対ジャンプの範囲違反、有効範囲は{SByte.MinValue}～{SByte.MaxValue}までです。",
            [ErrorCodeEnum.E0004] = "演算に失敗しました。",
            [ErrorCodeEnum.E0005] = "16進数の変換に失敗しました。",
            [ErrorCodeEnum.E0006] = "10進数の変換に失敗しました。",
            [ErrorCodeEnum.E0007] = "8進数の変換に失敗しました。",
            [ErrorCodeEnum.E0008] = "2進数の変換に失敗しました。",
            [ErrorCodeEnum.E0009] = "ORGに指定した出力アドレス上に既にアセンブリ結果があります。",
            [ErrorCodeEnum.E0010] = "",

            [ErrorCodeEnum.W0001] = "未定義",
            [ErrorCodeEnum.I0001] = "未定義"
        };
    }
}
