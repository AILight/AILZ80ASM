using AILZ80ASM.Assembler;

namespace AILZ80ASM.Assembler
{
    public class ErrorAsmList
    {
        public int LineIndex { get; private set; }
        public AsmList List { get; private set; }

        public ErrorAsmList(AsmList list, int lineIndex)
        {
            List = list;
            LineIndex = lineIndex;
        }
    }
}
