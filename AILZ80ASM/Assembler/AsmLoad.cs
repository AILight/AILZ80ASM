using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AILZ80ASM.InstructionSet;
using AILZ80ASM.Exceptions;
using AILZ80ASM.AILight;
using AILZ80ASM.LineDetailItems;
using AILZ80ASM.LineDetailItems.ScopeItem;
using System.Linq.Expressions;
using System.Reflection.Emit;
using System.Reflection;

namespace AILZ80ASM.Assembler
{
    public partial class AsmLoad
    {
        // アセンブルオプション
        public AsmOption AssembleOption { get; private set; }

        // 命令セット
        public ISA ISA { get; private set; }

        // 共有データ
        public AsmLoadShare Share { get; private set; }

        // スコープデータ
        public AsmLoadScope Scope { get; private set; }

        // スコープの親
        private AsmLoad ParentAsmLoad { get; set; } = default;


        public bool IsNamespaceUsed {
            get
            {
                var globalLabelnames = new List<string>();
                var asmLoad = this;

                while (asmLoad != default)
                {
                    globalLabelnames.AddRange(Scope.Labels.GroupBy(m => m.GlobalLabelName).Select(m => m.Key));
                    asmLoad = ParentAsmLoad;
                }

                return globalLabelnames.Count > 1;
            }
        }


        /// <summary>
        /// アセンブルエラー
        /// </summary>
        public ErrorLineItem[] AssembleErrors
        {
            get
            {
                return Share.Errors.Where(m => m.ErrorType == Error.ErrorTypeEnum.Error && (AssembleOption.DisableWarningCodes == default || !AssembleOption.DisableWarningCodes.Any(n => m.ErrorCode == n))).ToArray();
            }
        }

        /// <summary>
        /// アセンブルワーニング
        /// </summary>
        public ErrorLineItem[] AssembleWarnings
        {
            get
            {
                return Share.Errors.Where(m => m.ErrorType == Error.ErrorTypeEnum.Warning && (AssembleOption.DisableWarningCodes == default || !AssembleOption.DisableWarningCodes.Any(n => m.ErrorCode == n))).ToArray();
            }
        }

        /// <summary>
        /// アセンブルインフォメーション
        /// </summary>
        public ErrorLineItem[] AssembleInformation
        {
            get
            {
                return Share.Errors.Where(m => m.ErrorType == Error.ErrorTypeEnum.Information && (AssembleOption.DisableWarningCodes == default || !AssembleOption.DisableWarningCodes.Any(n => m.ErrorCode == n))).ToArray();
            }
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="assembleOption"></param>
        /// <param name="isa"></param>
        public AsmLoad(AsmOption assembleOption, ISA isa)
        {
            AssembleOption = assembleOption;
            ISA = isa;

            Share = new AsmLoadShare();
            Share.Errors = new List<ErrorLineItem>();
            Share.LineDetailItemForExpandItem = default;
            Share.AsmORGs = new List<AsmORG>() { new AsmORG() };
            Share.LoadFiles = new Stack<FileInfo>();
            Share.LoadMacros = new Stack<Macro>();
            Share.ListedFiles = new List<FileInfo>();
            Share.AsmStep = AsmLoadShare.AsmStepEnum.None;
            Share.PragmaOnceFiles = new List<FileInfo>();
            Share.CharMapConverter = new CharMaps.CharMapConverter();
            Share.AsmLists = new List<AsmList>();
            Share.GapByte = assembleOption.GapByte;
            Share.EntryPoint = null;
            Share.AsmSuperAssembleMode = new AsmSuperAssemble();
            Share.ValidateAssembles = new List<LineDetailItem>();
            Share.CheckLineDetailItemStack = new Stack<LineDetailItemCheck>();

            Scope = new AsmLoadScope();
            Scope.Labels = new List<Label>();
            Scope.Macros = new List<Macro>();
            Scope.Functions = new List<Function>();
            Scope.GlobalLabelNames = new List<string>();
            Scope.IsRegisterLabel = false;
        }

        /// <summary>
        /// 再アセンブル用に内容を初期化します
        /// </summary>
        public void ReAssembleInitialize()
        {
            var asmSuperAssembleMode = this.Share.AsmSuperAssembleMode;
            Share = new AsmLoadShare();
            Share.Errors = new List<ErrorLineItem>();
            Share.LineDetailItemForExpandItem = default;
            Share.AsmORGs = new List<AsmORG>() { new AsmORG() };
            Share.LoadFiles = new Stack<FileInfo>();
            Share.LoadMacros = new Stack<Macro>();
            Share.ListedFiles = new List<FileInfo>();
            Share.AsmStep = AsmLoadShare.AsmStepEnum.None;
            Share.PragmaOnceFiles = new List<FileInfo>();
            Share.CharMapConverter = new CharMaps.CharMapConverter();
            Share.AsmLists = new List<AsmList>();
            Share.GapByte = AssembleOption.GapByte;
            Share.EntryPoint = null;
            Share.AsmSuperAssembleMode = asmSuperAssembleMode;
            Share.ValidateAssembles = new List<LineDetailItem>();
            Share.CheckLineDetailItemStack = new Stack<LineDetailItemCheck>();

            Scope = new AsmLoadScope();
            Scope.Labels = new List<Label>();
            Scope.Macros = new List<Macro>();
            Scope.Functions = new List<Function>();
            Scope.GlobalLabelNames = new List<string>();
            Scope.IsRegisterLabel = false;
        }

        /// <summary>
        /// 新しいスコープで処理をする
        /// </summary>
        /// <param name="func"></param>
        public void CreateScope(Action<AsmLoad> func)
        {
            var asmLoad = new AsmLoad(this.AssembleOption, this.ISA)
            {
                Share = this.Share,
            };
            asmLoad.Scope = this.Scope.CreateScope();
            asmLoad.ParentAsmLoad = this.ParentAsmLoad;

            func.Invoke(asmLoad);

            this.Scope.Restore(asmLoad.Scope);
        }
        
        /// <summary>
        /// 新しいローカルスコープを作成
        /// </summary>
        /// <param name="globalLabelName"></param>
        /// <param name="labelName"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public AIValue CreateLocalScope(string globalLabelName, string labelName, Func<AsmLoad, AIValue> func)
        {
            var asmLoad = new AsmLoad(this.AssembleOption, this.ISA)
            {
                Share = this.Share,
            };
            asmLoad.Scope = this.Scope.CreateLocalScope();
            asmLoad.Scope.GlobalLabelName = globalLabelName;
            asmLoad.Scope.LabelName = labelName;
            asmLoad.Scope.GlobalLabelNames.Add(globalLabelName);
            asmLoad.ParentAsmLoad = this;

            return func.Invoke(asmLoad);
        }

        /// <summary>
        /// 新しいスコープを作成する
        /// </summary>
        /// <param name="globalLabelName"></param>
        /// <param name="labelName"></param>
        /// <param name="action"></param>
        public void CreateNewScope(string globalLabelName, string labelName, Action<AsmLoad> action)
        {
            CreateLocalScope(globalLabelName, labelName, localAsmLoad => 
            {
                action.Invoke(localAsmLoad);

                return new AIValue(0, AIValue.ValueInt32TypeEnum.Dec);
            });
        }

        /// <summary>
        /// ラベルの処理を行います
        /// </summary>
        /// <param name="func"></param>
        /// <param name="lineItem"></param>
        /// <returns></returns>
        public LineDetailItem ProcessLabel(Func<LineDetailItem> func, LineItem lineItem)
        {
            // ラベル前処理
            ProcessLabel_Pre(lineItem);

            var lineDetailItem = func.Invoke();

            // ラベル後処理
            ProcessLabel_Post(lineDetailItem);

            return lineDetailItem;
        }

        /// <summary>
        /// ラベルの前処理
        /// </summary>
        /// <param name="lineItem"></param>
        /// <exception cref="ErrorAssembleException"></exception>
        private void ProcessLabel_Pre(LineItem lineItem)
        {
            if (string.IsNullOrEmpty(lineItem.LabelString))
            {
                return;
            }

            var label = new LabelAdr(lineItem.LabelString, this);
            if (label.LabelLevel == Label.LabelLevelEnum.GlobalLabel)
            {
                // ラベルと同じ名前は付けられない
                if (this.Scope.Labels.Any(m => string.Compare(m.LabelName, label.LabelFullName, true) == 0))
                {
                    throw new ErrorAssembleException(Error.ErrorCodeEnum.E0017, label.LabelFullName);
                }

                if (!this.Scope.GlobalLabelNames.Any(m => string.Compare(m, label.LabelFullName, true) == 0))
                {
                    // ネームスペースが変わるときには保存する
                    this.Scope.GlobalLabelNames.Add(label.LabelFullName);
                }
                this.Scope.GlobalLabelName = label.GlobalLabelName;
                this.Scope.LabelName = label.LabelName;
                this.Scope.SubLabelName = label.SubLabelName;
            }
            else
            {
                // ネームスペースとと同じ名前は付けられない
                if (this.Scope.GlobalLabelNames.Any(m => string.Compare(m, label.LabelName, true) == 0))
                {
                    throw new ErrorAssembleException(Error.ErrorCodeEnum.E0018, label.LabelName);
                }
                this.Scope.LabelName = label.LabelName;
                this.Scope.SubLabelName= label.SubLabelName;
            }
        }

        /// <summary>
        /// ラベルの後処理
        /// </summary>
        /// <param name="lineDetailItem"></param>
        /// <exception cref="ErrorAssembleException"></exception>
        private void ProcessLabel_Post(LineDetailItem lineDetailItem)
        {
            // ラベルを処理する
            if (!string.IsNullOrEmpty(lineDetailItem.LineItem.LabelString))
            {
                var label = default(Label);
                if (lineDetailItem is LineDetailItemEqual lineDetailItemEqual)
                {
                    label = new LabelEqu(lineDetailItemEqual, this);
                    lineDetailItemEqual.EquLabel = label;
                }
                else
                {
                    label = new LabelAdr(lineDetailItem, this);
                }

                if (label.Invalidate)
                {
                    throw new ErrorAssembleException(Error.ErrorCodeEnum.E0013);
                }

                this.AddLabel(label);
                // ローカルラベルの末尾に「:」がついている場合にはワーニング
                if (lineDetailItem.LineItem.LabelString.StartsWith('.') && 
                    lineDetailItem.LineItem.LabelString.EndsWith(':'))
                {
                    this.AddError(new ErrorLineItem(lineDetailItem.LineItem, Error.ErrorCodeEnum.W9005));
                }
            }
        }

        /// <summary>
        /// ラベルをビルドする（値を確定させる）
        /// </summary>
        public void BuildLabel()
        {
            foreach (var item in this.Scope.Labels.Where(m => m.LabelLevel != Label.LabelLevelEnum.GlobalLabel && m.DataType == Label.DataTypeEnum.None))
            {
                item.BuildLabel();
            }
        }

        /// <summary>
        /// マクロ関連が閉じられているかを確認する
        /// </summary>
        public void LoadCloseValidate()
        {
            if (this.Share.LineDetailItemForExpandItem is LineDetailItemMacroDefine)
            {
                this.AddError(new ErrorLineItem(this.Share.LineDetailItemForExpandItem.LineItem, Error.ErrorCodeEnum.E3001));
            }

            if (this.Share.LineDetailItemForExpandItem is LineDetailItemRepeat)
            {
                this.AddError(new ErrorLineItem(this.Share.LineDetailItemForExpandItem.LineItem, Error.ErrorCodeEnum.E1011));
            }

            if (this.Share.LineDetailItemForExpandItem is LineDetailItemPreProcConditional)
            {
                this.AddError(new ErrorLineItem(this.Share.LineDetailItemForExpandItem.LineItem, Error.ErrorCodeEnum.E1021));
            }
        }

        /// <summary>
        /// チェック関連が閉じられているかを確認する
        /// </summary>
        public void CheckCloseValidate()
        {
            foreach (var checkLineDetailItem in this.Share.CheckLineDetailItemStack)
            {
                if (checkLineDetailItem is LineDetailItemCheckAlign)
                {
                    this.AddError(new ErrorLineItem(checkLineDetailItem.LineItem, Error.ErrorCodeEnum.E6011));
                }
            }
        }

        /// <summary>
        /// エラーの追加
        /// </summary>
        /// <param name="errorLineItem"></param>
        public void AddError(ErrorLineItem errorLineItem)
        {
            Share.Errors.Add(errorLineItem);
        }

        /// <summary>
        /// 複数エラーの追加
        /// </summary>
        /// <param name="errorLineItems"></param>
        public void AddErrors(IEnumerable<ErrorLineItem> errorLineItems)
        {
            Share.Errors.AddRange(errorLineItems);
        }

        /// <summary>
        /// ラベルの追加
        /// </summary>
        /// <param name="label"></param>
        /// <exception cref="ErrorAssembleException"></exception>
        public void AddLabel(Label label)
        {
            // 同一名のラベル
            if (this.Scope.Labels.Any(m => string.Compare(m.LabelFullName, label.LabelFullName, true) == 0))
            {
                var isLocalLabel = label.LabelLevel == Label.LabelLevelEnum.SubLabel;

                var message = "";
                if (isLocalLabel)
                {
                    message += "上位ラベルの宣言を確認してください。" ;
                }

                // equの場合は値が一致しているかを確認する
                if (label.LabelType == Label.LabelTypeEnum.Equ)
                {
                    if (AIMath.TryParse(label.ValueString, out var labelValue))
                    {
                        var sameLables = this.Scope.Labels.Where(m => string.Compare(m.LabelFullName, label.LabelFullName, true) == 0);
                        foreach (var sameLabel in sameLables)
                        {
                            if (AIMath.TryParse(sameLabel.ValueString, out var sameLabelValue))
                            {
                                if (labelValue.Equals(sameLabelValue)) 
                                {
                                    this.AddError(new ErrorLineItem(label.LineItem, Error.ErrorCodeEnum.I0003, label.LabelFullName));
                                    return;
                                }
                            }
                        }
                    }

                }
                throw new ErrorAssembleException(Error.ErrorCodeEnum.E0014, new[] { message });
            }
            if (label.LabelLevel == Label.LabelLevelEnum.GlobalLabel)
            {
                // ラベルと同じ名前は付けられない
                if (this.Scope.Labels.Any(m => string.Compare(m.LabelName, label.LabelFullName, true) == 0))
                {
                    throw new ErrorAssembleException(Error.ErrorCodeEnum.E0017, label.LabelFullName);
                }

                if (!this.Scope.GlobalLabelNames.Any(m => string.Compare(m, label.LabelFullName, true) == 0))
                {
                    // ネームスペースが変わるときには保存する
                    this.Scope.GlobalLabelNames.Add(label.LabelFullName);
                }
                this.Scope.GlobalLabelName = label.GlobalLabelName;
                this.Scope.LabelName = label.LabelName;
            }
            else
            {
                // ネームスペースとと同じ名前は付けられない
                if (this.Scope.GlobalLabelNames.Any(m => string.Compare(m, label.LabelName, true) == 0))
                {
                    throw new ErrorAssembleException(Error.ErrorCodeEnum.E0018, label.LabelName);
                }
                this.Scope.Labels.Add(label);
                if (label.LabelType == Label.LabelTypeEnum.Equ || label.LabelType == Label.LabelTypeEnum.Adr)
                {
                    this.Scope.LabelName = label.LabelName;
                    this.Scope.SubLabelName = label.SubLabelName;
                }
            }

            this.Scope.IsRegisterLabel |= (label.LabelValueType == Label.LabelValueTypeEnum.Register);
        }

        /// <summary>
        /// ファンクションの追加
        /// </summary>
        /// <param name="function"></param>
        /// <exception cref="ErrorAssembleException"></exception>
        public void AddFunction(Function function)
        {
            if (this.Scope.Functions.Any(m => string.Compare(m.FullName, function.FullName, true) == 0))
            {
                throw new ErrorAssembleException(Error.ErrorCodeEnum.E4001);
            }
            this.Scope.Functions.Add(function);
        }

        /// <summary>
        /// マクロの追加
        /// </summary>
        /// <param name="macro"></param>
        /// <exception cref="ErrorAssembleException"></exception>
        public void AddMacro(Macro macro)
        {
            if (this.Scope.Macros.Any(m => string.Compare(m.FullName, macro.FullName, true) == 0))
            {
                throw new ErrorAssembleException(Error.ErrorCodeEnum.E3010);
            }
            this.Scope.Macros.Add(macro);
        }

        /// <summary>
        /// ORGにぶら下がる明細を追加
        /// </summary>
        /// <param name="lineDetailItem"></param>
        public void AddLineDetailItem(LineDetailItem lineDetailItem)
        {
            this.Share.AsmORGs.Last().AddLineDetailItem(lineDetailItem);
        }

        /// <summary>
        /// ORGにぶら下がるエラー明細を追加
        /// </summary>
        /// <param name="lineDetailItem"></param>
        public void AddErrorLineDetailItem(LineDetailItem lineDetailItem)
        {
            this.Share.AsmORGs.Last().AddErrorLineDetailItem(lineDetailItem);
        }

        /// <summary>
        /// 読み込みファイルを追加
        /// </summary>
        /// <param name="fileInfo"></param>
        public void AddListedFile(FileInfo fileInfo)
        {
            this.Share.ListedFiles.Add(fileInfo);
        }

        /// <summary>
        /// AsmORGを追加
        /// </summary>
        /// <param name="asmORG"></param>
        public void AddORG(AsmORG asmORG)
        {
            this.Share.AsmORGs.Add(asmORG);
        }

        /// <summary>
        /// Pragma
        /// </summary>
        /// <param name="fileInfo"></param>
        public void AddPragmaOnceFileInfo(FileInfo fileInfo)
        {
            this.Share.PragmaOnceFiles.Add(fileInfo);
        }

        public bool ListedFileExists(FileInfo fileInfo)
        {
            return this.Share.ListedFiles.Any(m => m.FullName == fileInfo.FullName);
        }

        public void ListedFileClear()
        {
            this.Share.ListedFiles.Clear();
        }

        public string FindGlobalLabelName(string target)
        {
            var targetAsmLoad = this;

            while (targetAsmLoad != default)
            {
                var name = targetAsmLoad.Scope.GlobalLabelNames.Where(m => string.Compare(m, target, true) == 0).FirstOrDefault();
                if (name != default)
                {
                    return name;
                }
                targetAsmLoad = targetAsmLoad.ParentAsmLoad;
            }
            return default;
        }

        public Label[] FindLabels(string target)
        {
            try
            {
                if (target.StartsWith(".@@"))
                {
                    var targetAsmLoad = this;
                    while (targetAsmLoad != default)
                    {
                        var labels = targetAsmLoad.Scope.Labels.Where(m => m.GlobalLabelName == targetAsmLoad.Scope.GlobalLabelName && m.LabelLevel == Label.LabelLevelEnum.AnonLabel);
                        if (labels.Count() > 0)
                        {
                            return labels.ToArray();
                        }
                        targetAsmLoad = targetAsmLoad.ParentAsmLoad;
                    }
                }
                else
                {
                    // 完全一致を取得
                    var targetAsmLoad = this;
                    while (targetAsmLoad != default)
                    {
                        var labelFullName = Label.GetLabelFullName(target, targetAsmLoad);
                        var scopelabels = targetAsmLoad.Scope.Labels.Where(m => string.Compare(m.LabelFullName, labelFullName, true) == 0);
                        var label = scopelabels.FirstOrDefault();
                        if (label != default)
                        {
                            return new[] { label };
                        }
                        targetAsmLoad = targetAsmLoad.ParentAsmLoad;
                    }
                }
                // テンポラリーラベルを処理
                if (target.StartsWith(".@"))
                {
                    // .@のみの指定の場合
                    {
                        var targetAsmLoad = this;
                        target = $"{targetAsmLoad.Scope.LabelName}.{target}";
                        while (targetAsmLoad != default)
                        {
                            var labelFullName = Label.GetLabelFullName(target, targetAsmLoad);
                            var scopelabels = targetAsmLoad.Scope.Labels.Where(m => string.Compare(m.LabelFullName, labelFullName, true) == 0);
                            var label = scopelabels.FirstOrDefault();
                            if (label != default)
                            {
                                return new[] { label };
                            }
                            targetAsmLoad = targetAsmLoad.ParentAsmLoad;
                        }
                    }

                    // 曖昧ラベルの場合
                    {
                        var targetAsmLoad = this;
                        while (targetAsmLoad != default)
                        {
                            var labelFullName = Label.GetLabelFullName(target, targetAsmLoad);
                            var labelNames = labelFullName.Split(".");
                            var scopelabels = targetAsmLoad.Scope.Labels.Where(m => string.Compare(m.GlobalLabelName, labelNames[0], true) == 0 &&
                                                                               string.Compare(m.LabelName, labelNames[1], true) == 0 &&
                                                                               string.Compare(m.TmpLabelName, labelNames[3], true) == 0);

                            if (scopelabels.Count() > 0)
                            {
                                return scopelabels.ToArray();
                            }
                            targetAsmLoad = targetAsmLoad.ParentAsmLoad;
                        }
                    }
                }

                return new Label[] { };
            }
            catch (ErrorAssembleException)
            {
                throw;
            }
            catch
            {
                return new Label[] { };
            }
        }

        public Label FindLabel(string target)
        {
            var labels = FindLabels(target);
            if (labels.Count() > 1)
            {
                if (labels.Any(m => m.LabelLevel == Label.LabelLevelEnum.AnonLabel))
                {
                    throw new ErrorAssembleException(Error.ErrorCodeEnum.E0012);
                }
                else
                {
                    throw new ErrorAssembleException(Error.ErrorCodeEnum.E0008, string.Join(", ", labels.Select(m => m.LabelShortName)));
                }
            }
            return labels.FirstOrDefault();

        }

        public Label FindLabelForRegister(string target)
        {
            try
            {
                while (true)
                {
                    var lable = FindLabel(target);
                    if (lable == default)
                    {
                        return default;
                    }
                    if (lable.LabelValueType == Label.LabelValueTypeEnum.Register)
                    {
                        return lable;
                    }
                    if (target == lable.ValueString)
                    {
                        return default;
                    }

                    target = lable.ValueString;
                }
            }
            catch
            {
                return default;
            }
        }

        public Function FindFunction(string target)
        {
            var targetAsmLoad = this;

            while (targetAsmLoad != default)
            {
                var longFunctionName = Function.GetFunctionFullName(target, targetAsmLoad);
                var function = targetAsmLoad.Scope.Functions.Where(m => string.Compare(m.FullName, longFunctionName, true) == 0).FirstOrDefault();
                if (function != default)
                {
                    return function;
                }
                targetAsmLoad = targetAsmLoad.ParentAsmLoad;
            }
            return default;
        }

        public Macro[] FindMacros(string target)
        {
            var targetAsmLoad = this;
            var macroIndex = target.IndexOf('.');
            var shortMacroName = macroIndex == -1 ? target : target.Substring(0, macroIndex);

            while (targetAsmLoad != default)
            {
                var macros = targetAsmLoad.Scope.Macros.Where(m => string.Compare(m.Name, shortMacroName, true) == 0).ToArray();
                if (macros != default)
                {
                    return macros;
                }
                targetAsmLoad = targetAsmLoad.ParentAsmLoad;
            }
            return default;
        }

        public Macro FindMacro(string target)
        {
            var targetAsmLoad = this;

            while (targetAsmLoad != default)
            {
                var longMacroName = Macro.GetMacroFullName(target, targetAsmLoad);
                var function = targetAsmLoad.Scope.Macros.Where(m => string.Compare(m.FullName, longMacroName, true) == 0).FirstOrDefault();
                if (function != default)
                {
                    return function;
                }
                targetAsmLoad = targetAsmLoad.ParentAsmLoad;
            }
            return default;
        }

        /// <summary>
        /// 出力のスペース領域を埋める
        /// </summary>
        /// <param name="fromOutputAddress"></param>
        /// <param name="toOutputAddress"></param>
        /// <param name="stream"></param>
        /// <exception cref="NotSupportedException"></exception>
        public void OutputORGSpace(ref UInt32 fromOutputAddress, UInt32 toOutputAddress, Stream stream)
        {
            var outputAddress = fromOutputAddress;
            var asmORGs = this.Share.AsmORGs.OrderBy(m => m.OutputAddress).ToList();
            var startORG = asmORGs.Where(m => m.OutputAddress <= outputAddress).OrderBy(m => m.OutputAddress).LastOrDefault();
            var endORG = asmORGs.Where(m => m.OutputAddress <= toOutputAddress).OrderBy(m => m.OutputAddress).LastOrDefault();
            var startIndex = asmORGs.IndexOf(startORG);
            var endIndex = asmORGs.IndexOf(endORG);

            if (startIndex == -1 || endIndex == -1)
            {
                throw new NotSupportedException();
            }

            for (var index = startIndex; index <= endIndex; index++)
            {
                var length = default(UInt32);
                if (index < endIndex)
                {
                    length = asmORGs[index + 1].OutputAddress.Value - outputAddress;
                }
                else
                {
                    length = toOutputAddress - outputAddress;

                }
                var bytes = Enumerable.Repeat<byte>(asmORGs[index].FillByte, (int)length).ToArray();
                stream.Write(bytes, 0, bytes.Length);
                outputAddress += length;
            }

            fromOutputAddress = outputAddress;
        }

        public FileInfo FindPramgaOnceFile(FileInfo fileInfo)
        {
            return this.Share.PragmaOnceFiles.FirstOrDefault(m => m.GetFullNameCaseSensitivity() == fileInfo.GetFullNameCaseSensitivity());
        }


        public AsmEnum.EncodeModeEnum GetEncodMode(FileInfo fileInfo)
        {
            var encodeMode =　AssembleOption.InputEncodeMode;
            if (encodeMode == AsmEnum.EncodeModeEnum.AUTO)
            {
                encodeMode = AssembleOption.CheckEncodeMode(fileInfo);
            }
            return encodeMode;
        }

        public System.Text.Encoding GetInputEncoding(AsmEnum.EncodeModeEnum encodeMode)
        {
            if (encodeMode == AsmEnum.EncodeModeEnum.AUTO)
            {
                throw new ArgumentException("encodeMode:AUTOは指定できません");
            }

            var encoding = GetEncoding(encodeMode);

            return encoding;
        }

        /// <summary>
        /// エンコードモードにしたがって、.NETのEncodingを返す
        /// </summary>
        /// <param name="encodeMode"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public static System.Text.Encoding GetEncoding(AsmEnum.EncodeModeEnum encodeMode)
        {
            var encoding = encodeMode switch
            {
                AsmEnum.EncodeModeEnum.UTF_8 => AIEncode.GetEncodingUTF8(),
                AsmEnum.EncodeModeEnum.SHIFT_JIS => AIEncode.GetEncodingSJIS(),
                _ => throw new NotImplementedException()
            };

            return encoding;
        }

        /// <summary>
        /// アウトプットモードのファイルタイプを返す
        /// </summary>
        /// <param name="outputMode"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public static AsmEnum.FileDataTypeEnum GetFileType(AsmEnum.FileTypeEnum fileType)
        {
            var dataType = fileType switch
            {
                AsmEnum.FileTypeEnum.BIN => AsmEnum.FileDataTypeEnum.Binary,
                AsmEnum.FileTypeEnum.HEX => AsmEnum.FileDataTypeEnum.Text,
                AsmEnum.FileTypeEnum.T88 => AsmEnum.FileDataTypeEnum.Binary,
                AsmEnum.FileTypeEnum.CMT => AsmEnum.FileDataTypeEnum.Binary,
                AsmEnum.FileTypeEnum.LST => AsmEnum.FileDataTypeEnum.Text,
                AsmEnum.FileTypeEnum.SYM => AsmEnum.FileDataTypeEnum.Text,
                AsmEnum.FileTypeEnum.EQU => AsmEnum.FileDataTypeEnum.Text,
                AsmEnum.FileTypeEnum.DBG => AsmEnum.FileDataTypeEnum.Text,
                AsmEnum.FileTypeEnum.TAG => AsmEnum.FileDataTypeEnum.Text,
                _ => throw new NotImplementedException()
            };

            return dataType;
        }

        /// <summary>
        /// CharMapConverter.IsContains
        /// </summary>
        /// <param name="map"></param>
        /// <returns></returns>
        public bool CharMapConverter_IsContains(string map)
        {
            return this.Share.CharMapConverter.IsContains(map);
        }

        /// <summary>
        /// CharMapConverter.ReadCharMapFromResource
        /// </summary>
        /// <param name="map"></param>
        public void CharMapConverter_ReadCharMapFromResource(string map)
        {
            this.Share.CharMapConverter.ReadCharMapFromResource(map, this);
        }

        /// <summary>
        /// CharMapConverter.ConvertToBytes
        /// </summary>
        /// <param name="map"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public byte[] CharMapConverter_ConvertToBytes(string map, string target)
        {
            return this.Share.CharMapConverter.ConvertToBytes(map, target);
        }

        /// <summary>
        /// CharMapConverter.ReadCharMapFromFile
        /// </summary>
        /// <param name="map"></param>
        /// <param name="filePath"></param>
        public void CharMapConverter_ReadCharMapFromFile(string map, FileInfo fileInfo)
        {
            this.Share.CharMapConverter.ReadCharMapFromFile(map, fileInfo, this);
        }

        /// <summary>
        /// エラーをリストに関連付ける
        /// </summary>
        public void AssociateError()
        {
            //Share.Errors
            foreach (var error in Share.Errors)
            {
                error.AssociateErrorLineItem();
            }
        }

        /// <summary>
        /// 同じエラーを持っているか確認する
        /// </summary>
        public bool HasSameError(ErrorLineItem errorListItem)
        {
            return this.Share.Errors.Any(m => m.ErrorCode == errorListItem.ErrorCode &&
                                              m.LineItem.LineString == errorListItem.LineItem.LineString &&
                                              m.LineItem.LineIndex == errorListItem.LineItem.LineIndex &&
                                             (m.LineItem?.FileInfo.FullName ?? "") == (errorListItem.LineItem?.FileInfo.FullName ?? ""));
        }

        /// <summary>
        /// ValidateAssembleの追加
        /// </summary>
        /// <param name="alignBlock"></param>
        public void AddValidateAssembles(LineDetailItem lineDetailItem)
        {
            Share.ValidateAssembles.Add(lineDetailItem);
        }

    }
}
