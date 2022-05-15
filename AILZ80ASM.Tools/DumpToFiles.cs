using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AILZ80ASM.Tools
{
    public class DumpToFiles
    {
        public static void SaveFiles(FileInfo fileInfo)
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            var encoding = System.Text.Encoding.GetEncoding("SHIFT_JIS");
            var text = File.ReadAllText(fileInfo.FullName, encoding).Replace("\r\n", "\n");
            var lines = text.Split(new char[] { '\n' }).Skip(7);
            var mainLines = new List<string>();
            var outputLines = new List<string>();
            var includeFlg = false;

            foreach (var item in lines)
            {
                if (item.StartsWith("[EOF:"))
                {
                    var fileName = item.Substring(5, item.Length - 6);
                    if (includeFlg)
                    {
                        File.WriteAllText(Path.Combine(fileInfo.DirectoryName ?? "", fileName), String.Join(System.Environment.NewLine, outputLines), encoding);
                        outputLines.Clear();
                        includeFlg = false;
                    }
                    else
                    {
                        File.WriteAllText(Path.Combine(fileInfo.DirectoryName ?? "", fileName), String.Join(System.Environment.NewLine, mainLines), encoding);
                        mainLines.Clear();
                    }
                }
                else if (item.Length > 23)
                {
                    var source = item.Substring(23);
                    Console.WriteLine(source);
                    if (source.StartsWith("include"))
                    {
                        includeFlg = true;
                        mainLines.Add(source);
                    }
                    else
                    {
                        if (item[22] != '+')
                        {
                            if (includeFlg)
                            {
                                outputLines.Add(source);
                            }
                            else
                            {
                                mainLines.Add(source);
                            }
                        }
                    }
                }
            }
        }
    }
}
