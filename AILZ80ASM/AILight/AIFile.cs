using System.IO;

namespace AILZ80ASM.AILight
{
    public static class AIFile
    {
        /// <summary>
        /// ファイルシステムが持つファイル名を取得
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <returns></returns>
        public static string GetFullNameCaseSensitivity(this FileInfo fileInfo)
        {
            var files = Directory.GetFiles(Path.GetDirectoryName(fileInfo.FullName), Path.GetFileName(fileInfo.FullName));
            if (files.Length == 0)
            {
                return fileInfo.FullName;
            }
            else
            {
                return files[0];
            }
        }
    }
}
