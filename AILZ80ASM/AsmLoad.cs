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
        public Label[] AllLables => Labels.Union(LocalLabels).ToArray();
        public List<Label> Labels { get; private set; } = new List<Label>();
        public List<Label> LocalLabels { get; private set; } = new List<Label>();
        public List<Macro> Macros { get; private set; } = new List<Macro>();

        public LineDetailItemMacro LineDetailItemMacro { get; set; } = null;
        public LineDetailItemRepeat LineDetailItemRepeat { get; set; } = null; 

        public AsmLoad()
        {
        }

        /// <summary>
        /// クローン
        /// </summary>
        /// <returns></returns>
        public AsmLoad Clone()
        {
            return new AsmLoad
            {
                GlobalLableName = this.GlobalLableName,
                LabelName = this.LabelName,
                LoadFiles = this.LoadFiles,
                Macros = this.Macros,
                LineDetailItemMacro = this.LineDetailItemMacro,
                Labels = this.Labels,
                LocalLabels = this.LocalLabels,
                LineDetailItemRepeat = LineDetailItemRepeat
            };
        }

        /// <summary>
        /// クローン（ローカルラベルはコピーしない）
        /// </summary>
        /// <returns></returns>
        public AsmLoad CloneForLocal()
        {
            return new AsmLoad
            {
                GlobalLableName = this.GlobalLableName,
                LabelName = this.LabelName,
                LoadFiles = this.LoadFiles,
                Macros = this.Macros,
                LineDetailItemMacro = this.LineDetailItemMacro,
                Labels = this.Labels,
                LineDetailItemRepeat = LineDetailItemRepeat
            };
        }

        public void LoadCloseValidate()
        {
            if (LineDetailItemMacro != default)
            {
                throw new ErrorMessageException(Error.ErrorCodeEnum.E1001);
            }

            if (LineDetailItemRepeat != default)
            {
                throw new ErrorMessageException(Error.ErrorCodeEnum.E1011);
            }
        }
    }
}
