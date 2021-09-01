using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AILZ80ASM
{
    public class AsmLoad
    {
        private string _GlobalLableName;

        public string GlobalLableName 
        {
            get { return _GlobalLableName; }
            set
            {
                _GlobalLableName = value;
                LabelName = "";
            }
        }
        public string LabelName { get; set; }
        public Stack<FileInfo> LoadFiles { get; private set; } = new Stack<FileInfo>();
        public List<Label> Labels { get; private set; } = new List<Label>();
        public List<Macro> Macros { get; private set; } = new List<Macro>();

        public LineDetailItemMacro LineDetailItemMacro { get; set; } = null;

        public AsmLoad()
        {
        }
    }
}
