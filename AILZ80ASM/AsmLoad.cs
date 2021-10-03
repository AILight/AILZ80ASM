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
            };
        }

        public void LoadCloseValidate(IList<ErrorFileInfoMessage> errorMessages)
        {
            if (this.LineDetailItemForExpandItem is LineDetailItemMacroDefine)
            {
                var errorMessageException = new ErrorMessageException(Error.ErrorCodeEnum.E1001);
                errorMessages.Add(new ErrorFileInfoMessage(new[] { new ErrorLineItemMessage(errorMessageException, LineDetailItemForExpandItem.LineItem) }, LineDetailItemForExpandItem.LineItem.FileInfo));
            }

            if (this.LineDetailItemForExpandItem is LineDetailItemRepeat)
            {
                var errorMessageException = new ErrorMessageException(Error.ErrorCodeEnum.E1011);
                errorMessages.Add(new ErrorFileInfoMessage(new[] { new ErrorLineItemMessage(errorMessageException, LineDetailItemForExpandItem.LineItem) }, LineDetailItemForExpandItem.LineItem.FileInfo));
            }

            if (this.LineDetailItemForExpandItem is LineDetailItemConditional)
            {
                var errorMessageException = new ErrorMessageException(Error.ErrorCodeEnum.E1021);
                errorMessages.Add(new ErrorFileInfoMessage(new[] { new ErrorLineItemMessage(errorMessageException, LineDetailItemForExpandItem.LineItem) }, LineDetailItemForExpandItem.LineItem.FileInfo));
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
