using AILZ80ASM.Assembler;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AILZ80ASM
{
    public partial class Package
    {
        public void TraceTitle_Inputs()
        {
            Trace.WriteLine($"# Inputs");
            Trace.WriteLine("");

            foreach (var item in AssembleOption.InputFiles)
            {
                foreach (var fileInfo in item.Value)
                {
                    Trace.WriteLine($"- {item.Key.ToString()} filename [{fileInfo.Name}]");
                }
            }
            Trace.WriteLine("");
        }

        public void TraceTitle_AssembleStatus()
        {
            Trace.WriteLine($"# Assemble Status");
            Trace.WriteLine("");
        }


        internal void TraceTitle_AbortAssemble()
        {
            Trace.WriteLine($"# Assemble Information");
            Trace.WriteLine($"");
            Trace.WriteLine($"- Assemble was aborted.");
            Trace.WriteLine($"");
        }

        public void TraceTitle_OutputFilesConfirm(Dictionary<AsmEnum.FileTypeEnum, FileInfo> outputFiles)
        {
            Trace.WriteLine("# Confirm Outputfiles");
            Trace.WriteLine("");

            foreach (var item in outputFiles)
            {
                Trace.WriteLine($"- {item.Key.ToString()} filename [{item.Value.Name}]");
            }

            Trace.WriteLine("");
        }

        public void TraceTitle_DiffFileMode()
        {
            Trace.WriteLine("# 出力ファイル差分確認モード");
            Trace.WriteLine("");
        }

        public void TraceTitle_Outputs()
        {
            Trace.WriteLine($"# Outputs");
            Trace.WriteLine("");
        }

    }
}
