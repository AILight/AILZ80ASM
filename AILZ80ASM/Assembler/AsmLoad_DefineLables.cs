using AILZ80ASM.AILight;
using AILZ80ASM.Exceptions;
using AILZ80ASM.LineDetailItems;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AILZ80ASM.Assembler
{
    public partial class AsmLoad
    {
        private static readonly string RegexPatternCommandlineDefineLabel = @"(?<Namespace>\w+)?(?:\.(?<Label>\w+))|(?<Label>\w+)";
        private static readonly Regex CompiledRegexPatternCommandlineDefineLabel = new Regex(
            RegexPatternCommandlineDefineLabel, RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase
        );

        public void SettingComandlineOptionDefineLabels()
        {
            if (this.AssembleOption.DefineLabels == default)
            {
                return;
            }

            var defaultNamespace = this.Scope.GlobalLabelName;

            foreach (var defineLabel in this.AssembleOption.DefineLabels)
            {
                var value = defineLabel.Split('=').Select(m => m.Trim()).ToArray();
                var label = default(Label);
                var labelName = value[0];
                var namespaceIndexOf = labelName.IndexOf('.');
                var defineNamespace = namespaceIndexOf == -1 ? "" : labelName.Substring(0, namespaceIndexOf);
                var defineLabelName = namespaceIndexOf == -1 ? labelName : labelName.Substring(namespaceIndexOf + 1);

                // ネームスペース
                if (string.IsNullOrEmpty(defineNamespace))
                {
                    InternalResetDefaultNamespace(defaultNamespace);
                }
                else
                {
                    var namespaceLabel = new LabelAdr($"[{defineNamespace}]", this);
                    if (namespaceLabel.Invalidate)
                    {
                        InternalDefineLabelException(labelName);
                    }
                    this.AddLabel(namespaceLabel);
                }

                // ラベル
                switch (value.Length)
                {
                    case 1:
                        label = new LabelEqu(defineLabelName, "#TRUE", this);
                        break;
                    case 2:
                        label = new LabelEqu(defineLabelName, value[1], this);
                        break;
                    default:
                        InternalDefineLabelException(labelName);
                        break;

                }
                if (label.Invalidate)
                {
                    InternalDefineLabelException(labelName);
                }
                this.AddLabel(label);
            }

            // ネームスペースを元に戻す
            InternalResetDefaultNamespace(defaultNamespace);
        }

        private void InternalDefineLabelException(string labelName)
        {
            throw new InvalidCommanLineOptionException($"--define-labelで指定した、ラベル名が正しくありません。lable:{labelName}");
        }

        private void InternalResetDefaultNamespace(string defaultNamespace)
        {
            if (defaultNamespace != this.Scope.GlobalLabelName)
            {
                var defaultNamespaceLabel = new LabelAdr($"[{defaultNamespace}]", this);
                this.AddLabel(defaultNamespaceLabel);
            }
        }
    }
}
