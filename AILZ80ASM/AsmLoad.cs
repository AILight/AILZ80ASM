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
        public Stack<FileInfo> LoadFiles { get; private set; } = new Stack<FileInfo>(); //Include用

        public Label[] AllLables => Labels.Union(LocalLabels).ToArray();
        public List<Label> Labels { get; private set; } = new List<Label>();
        public List<Label> LocalLabels { get; private set; } = new List<Label>();
        public List<Macro> Macros { get; private set; } = new List<Macro>();
        public List<ErrorLineItem> Errors { get; private set; } = new List<ErrorLineItem>();

        public LineDetailItem LineDetailItemForExpandItem { get; set; } = null;

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
                LineDetailItemForExpandItem = this.LineDetailItemForExpandItem,

                Labels = this.Labels,
                LocalLabels = this.LocalLabels,
                Errors = this.Errors
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
                LineDetailItemForExpandItem = this.LineDetailItemForExpandItem,

                Labels = this.Labels,
                LocalLabels = scopeMode == ScopeModeEnum.Global ? this.LocalLabels : new List<Label>(),
                Errors = this.Errors
            };
        }

        public void LoadCloseValidate()
        {
            if (this.LineDetailItemForExpandItem is LineDetailItemMacroDefine)
            {
                this.Errors.Add(new ErrorLineItem(LineDetailItemForExpandItem.LineItem, Error.ErrorCodeEnum.E1001));
            }

            if (this.LineDetailItemForExpandItem is LineDetailItemRepeat)
            {
                this.Errors.Add(new ErrorLineItem(LineDetailItemForExpandItem.LineItem, Error.ErrorCodeEnum.E1011));
            }

            if (this.LineDetailItemForExpandItem is LineDetailItemConditional)
            {
                this.Errors.Add(new ErrorLineItem(LineDetailItemForExpandItem.LineItem, Error.ErrorCodeEnum.E1021));
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
