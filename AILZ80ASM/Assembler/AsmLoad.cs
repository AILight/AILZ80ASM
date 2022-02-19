using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AILZ80ASM.InstructionSet;
using AILZ80ASM.Exceptions;
using AILZ80ASM.AILight;

namespace AILZ80ASM.Assembler
{
    public class AsmLoad
    {
        public enum ScopeModeEnum
        {
            Global,
            Local
        }

        public enum EncodeModeEnum
        {
            AUTO,
            UTF_8,
            SHIFT_JIS,
        }

        public enum OutputModeEnum
        {
            BIN,
            HEX,
            T88,
            CMT,
            LST,
            SYM,
            DBG,
        }

        public enum ListModeEnum
        {
            Simple,
            Middle,
            Full,
        }

        private ScopeModeEnum ScopeMode { get; set; } = ScopeModeEnum.Global;

        public string GlobalLabelName { get; private set; }
        public string LabelName { get; private set; }

        public Stack<FileInfo> LoadFiles { get; private set; } = new Stack<FileInfo>(); //Include循環展開チェック用
        public Stack<Macro> LoadMacros { get; private set; } = new Stack<Macro>(); //マクロ循環展開チェック用

        public Label[] AllLabels => Labels.Union(LocalLabels).ToArray();
        public Function[] AllFunctions => Functions.Union(LocalFunctions).ToArray();
        public List<AsmAddress> AsmAddresses { get; private set; } = new List<AsmAddress>();
        public List<Label> Labels { get; private set; } = new List<Label>();
        public List<Label> LocalLabels { get; private set; } = new List<Label>();
        public List<Macro> Macros { get; private set; } = new List<Macro>();
        public List<Function> Functions { get; private set; } = new List<Function>();
        public List<Function> LocalFunctions { get; private set; } = new List<Function>();
        public List<ErrorLineItem> Errors { get; private set; } = new List<ErrorLineItem>();
        public List<AsmLoad> AsmLoads { get; private set; } = new List<AsmLoad>();
        public List<OperationItem> TirmOperationITems { get; private set; } = new List<OperationItem>();

        public LineDetailItem LineDetailItemForExpandItem { get; set; } = null;
        public ISA ISA { get; private set; }
        public EncodeModeEnum InputEncodeMode { get; set; }
        public EncodeModeEnum OutputEncodeMode { get; set; } = EncodeModeEnum.UTF_8;
        public ListModeEnum ListMode { get; set; } = ListModeEnum.Full;
        public bool OutputTrim { get; internal set; }
        public Error.ErrorCodeEnum[] DisableWarningCodes { get; internal set; }
        public string CharMap { get; set; } = "SJIS";

        public AsmLoad(ISA isa)
        {
            ISA = isa;
        }

        /// <summary>
        /// クローン
        /// </summary>
        /// <returns></returns>
        public AsmLoad Clone()
        {
            return Clone(this.ScopeMode);
        }

        public AsmLoad Clone(ScopeModeEnum scopeMode)
        {
            var asmLoad = new AsmLoad(this.ISA)
            {
                GlobalLabelName = this.GlobalLabelName,
                LabelName = this.LabelName,
                AsmAddresses = this.AsmAddresses,
                LoadFiles = this.LoadFiles,
                LoadMacros = this.LoadMacros,
                Macros = this.Macros,
                LineDetailItemForExpandItem = this.LineDetailItemForExpandItem,

                Labels = this.Labels,
                LocalLabels = scopeMode == ScopeModeEnum.Global ? this.LocalLabels : new List<Label>(),

                Functions = this.Functions,
                LocalFunctions = scopeMode == ScopeModeEnum.Global ? this.LocalFunctions : new List<Function>(),

                Errors = this.Errors,
                OutputTrim = this.OutputTrim,
                TirmOperationITems = this.TirmOperationITems,
                CharMap = this.CharMap,
            };
            switch (scopeMode)
            {
                case ScopeModeEnum.Global:
                    asmLoad.ScopeMode = ScopeModeEnum.Global;
                    asmLoad.LocalLabels = this.LocalLabels;
                    asmLoad.LocalFunctions = this.LocalFunctions;
                    break;
                case ScopeModeEnum.Local:
                    switch (this.ScopeMode)
                    {
                        case ScopeModeEnum.Global:
                            asmLoad.ScopeMode = ScopeModeEnum.Local;
                            asmLoad.LocalLabels = new List<Label>();
                            asmLoad.LocalFunctions = new List<Function>();
                            break;
                        case ScopeModeEnum.Local:
                            asmLoad.ScopeMode = ScopeModeEnum.Local;
                            asmLoad.LocalLabels = this.LocalLabels;
                            asmLoad.LocalFunctions = this.LocalFunctions;
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }


            asmLoad.AsmLoads.AddRange(this.AsmLoads);
            asmLoad.AsmLoads.Add(this);

            return asmLoad;
        }

        public void LoadCloseValidate()
        {
            if (this.LineDetailItemForExpandItem is LineDetailItemMacroDefine)
            {
                this.Errors.Add(new ErrorLineItem(LineDetailItemForExpandItem.LineItem, Error.ErrorCodeEnum.E3001));
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

        public void SetScope(AsmLoad asmLoad)
        {
            this.GlobalLabelName = asmLoad.GlobalLabelName;
            this.LabelName = asmLoad.LabelName;
        }

        public void AddLabel(Label label)
        {
            // 同一名のラベル
            if (label.LabelLevel != Label.LabelLevelEnum.GlobalLabel &&
                this.AllLabels.Any(m => string.Compare(m.LongLabelName, label.LongLabelName, true) == 0))
            {
                throw new ErrorAssembleException(Error.ErrorCodeEnum.E0014);
            }
            // グローバルラベルとの比較
            switch (label.LabelLevel)
            {
                case Label.LabelLevelEnum.GlobalLabel:
                    if (this.AllLabels.Any(m => m.LabelLevel == Label.LabelLevelEnum.Label &&
                        string.Compare(m.LabelName, label.GlobalLabelName, true) == 0))
                    {
                        throw new ErrorAssembleException(Error.ErrorCodeEnum.E0017);
                    }
                    this.GlobalLabelName = label.GlobalLabelName;
                    this.LabelName = "";
                    break;
                case Label.LabelLevelEnum.Label:
                    if (this.AllLabels.Any(m => m.LabelLevel == Label.LabelLevelEnum.GlobalLabel &&
                        string.Compare(m.GlobalLabelName, label.LabelName, true) == 0))
                    {
                        throw new ErrorAssembleException(Error.ErrorCodeEnum.E0018);
                    }
                    this.LabelName = label.LabelName;
                    break;
                case Label.LabelLevelEnum.SubLabel:
                    // 何もしない
                    break;
                default:
                    throw new InvalidOperationException();
            }


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

        public void AddFunction(Function function)
        {
            if (this.AllFunctions.Any(m => string.Compare(m.FullName, function.FullName, true) == 0))
            {
                throw new ErrorAssembleException(Error.ErrorCodeEnum.E4001);
            }

            switch (ScopeMode)
            {
                case ScopeModeEnum.Global:
                    Functions.Add(function);
                    break;
                case ScopeModeEnum.Local:
                    LocalFunctions.Add(function);
                    break;
                default:
                    throw new InvalidOperationException();
            }
        }

        public Label FindLabel(string target)
        {
            return FindLabel(target, true);
        }

        public Label FindLabel(string target, bool hasValue)
        {
            var longLabelName = Label.GetLongLabelName(target, this);
            var label = AllLabels.Where(m => (m.HasValue || !hasValue) && string.Compare(m.LongLabelName, longLabelName, true) == 0).FirstOrDefault();
            if (label == default && this.ScopeMode == ScopeModeEnum.Local &&
                !AllLabels.Any(m => string.Compare(m.LongLabelName, longLabelName, true) == 0))
            {
                var asmLoad = this.AsmLoads.LastOrDefault(m => m.ScopeMode == ScopeModeEnum.Global);
                if (asmLoad != default)
                {
                    longLabelName = Label.GetLongLabelName(target, asmLoad);
                    label = AllLabels.Where(m => (m.HasValue || !hasValue) && string.Compare(m.LongLabelName, longLabelName, true) == 0).FirstOrDefault();
                }
            }

            return label;
        }

        public Function FindFunction(string target)
        {
            var longFunctionName = Function.GetLongFunctionName(target, this);
            var function = Functions.Where(m => string.Compare(m.FullName, longFunctionName, true) == 0).FirstOrDefault();
            if (function == default && this.ScopeMode == ScopeModeEnum.Local &&
                !Functions.Any(m => string.Compare(m.FullName, longFunctionName, true) == 0))
            {
                var asmLoad = this.AsmLoads.LastOrDefault(m => m.ScopeMode == ScopeModeEnum.Global);
                if (asmLoad != default)
                {
                    longFunctionName = Function.GetLongFunctionName(target, asmLoad);
                    function = Functions.Where(m => string.Compare(m.FullName, longFunctionName, true) == 0).FirstOrDefault();
                }
            }

            return function;
        }

        public void AddAsmAddress(AsmAddress asmAddress)
        {
            AsmAddresses.Add(asmAddress);
        }

        public void AddTrimOperationItem(OperationItem operationItem)
        {
            TirmOperationITems.Add(operationItem);
        }

        public void ClearTrimOperationItem()
        {
            TirmOperationITems.Clear();
        }


        public EncodeModeEnum GetEncodMode(FileInfo fileInfo)
        {
            var encodeMode = InputEncodeMode;
            if (encodeMode == EncodeModeEnum.AUTO)
            {
                using var readStream = fileInfo.OpenRead();
                using var memoryStream = new MemoryStream();
                readStream.CopyTo(memoryStream);
                var bytes = memoryStream.ToArray();

                var isUTF8 = AIEncode.IsUTF8(bytes);
                var isSHIFT_JIS = AIEncode.IsSHIFT_JIS(bytes);
                encodeMode = EncodeModeEnum.UTF_8;
                if (!isUTF8 && isSHIFT_JIS)
                {
                    encodeMode = EncodeModeEnum.SHIFT_JIS;
                }
            }
            return encodeMode;
        }

        public System.Text.Encoding GetInputEncoding(FileInfo fileInfo)
        {
            var encodeMode = GetEncodMode(fileInfo);
            var encoding = GetInputEncoding(encodeMode);

            return encoding;
        }

        public System.Text.Encoding GetInputEncoding(EncodeModeEnum encodeMode)
        {
            if (encodeMode == EncodeModeEnum.AUTO)
            {
                throw new ArgumentException("encodeMode:AUTOは指定できません");
            }

            OutputEncodeMode = encodeMode;
            var encoding = GetEncoding(encodeMode);

            return encoding;
        }


        public System.Text.Encoding GetOutputEncoding()
        {
            var encoding = GetEncoding(OutputEncodeMode);
            return encoding;
        }

        public static System.Text.Encoding GetEncoding(EncodeModeEnum encodeMode)
        {
            var encoding = default(System.Text.Encoding);

            switch (encodeMode)
            {
                case EncodeModeEnum.UTF_8:
                    encoding = System.Text.Encoding.UTF8;
                    break;
                case EncodeModeEnum.SHIFT_JIS:
                    try
                    {
                        System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                        encoding = System.Text.Encoding.GetEncoding("SHIFT_JIS");
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("お使いの環境では、SHIFT_JISをご利用いただくことは出来ません。", ex);
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }

            return encoding;
        }

    }
}
