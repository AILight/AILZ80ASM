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
        public enum ScopeModeEnum
        {
            Global,
            Local
        }

        private ScopeModeEnum ScopeMode { get; set; } = ScopeModeEnum.Global;
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
                ScopeMode = this.ScopeMode,
                GlobalLableName = this.GlobalLableName,
                LabelName = this.LabelName,
                LoadFiles = this.LoadFiles,
                Macros = this.Macros,
                LineDetailItemMacro = this.LineDetailItemMacro,
                LineDetailItemRepeat = LineDetailItemRepeat,

                Labels = this.Labels,
                LocalLabels = this.LocalLabels,
            };
        }

        public AsmLoad Clone(ScopeModeEnum scopeMode)
        {
            return new AsmLoad
            {
                ScopeMode = scopeMode,
                GlobalLableName = this.GlobalLableName,
                LabelName = this.LabelName,
                LoadFiles = this.LoadFiles,
                Macros = this.Macros,
                LineDetailItemMacro = this.LineDetailItemMacro,
                LineDetailItemRepeat = LineDetailItemRepeat,

                Labels = this.Labels,
                LocalLabels = scopeMode == ScopeModeEnum.Global ? this.LocalLabels : new List<Label>(),
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

        public void AddLabel(Label label)
        {
            switch (ScopeMode)
            {
                case ScopeModeEnum.Global:
                    Labels.Add(label);
                    break;
                case ScopeModeEnum.Local:
                    LocalLabels.Add(label);
                    break;
                default:
                    throw new InvalidOperationException();
            }
        }
    }
}
