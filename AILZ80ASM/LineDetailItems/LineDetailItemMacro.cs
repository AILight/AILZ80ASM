using AILZ80ASM.Assembler;
using AILZ80ASM.Exceptions;
using System.Linq;
using System.Collections.Generic;
using System;
using AILZ80ASM.LineDetailItems.ScopeItem;

namespace AILZ80ASM.LineDetailItems
{
    public class LineDetailItemMacro : LineDetailItem
    {
        private MacroExpansionResult MacroResult { get; set; }

        public override AsmList[] Lists
        {
            get
            {
                if (!AsmLoad.Share.IsOutputList)
                {
                    return new AsmList[] { };
                }

                var lists = new List<AsmList>();
                // 宣言行
                lists.Add(AsmList.CreateLineItem(LineItem));
                // 引数
                foreach (var label in MacroResult?.ArgumetLabels ?? Array.Empty<Label>())
                {
                    var value = label.DataType switch
                    {
                        Label.DataTypeEnum.Invalid => "Invalid",
                        Label.DataTypeEnum.None => label.ValueString,
                        _ => label.Value.ValueType.HasFlag(AILight.AIValue.ValueTypeEnum.Int32) ?
                             $"0x{label.Value.ConvertTo<int>():x4}:{label.Value.ConvertTo<int>()}" :
                             label.Value.ConvertTo<object>().ToString(),
                    };

                    lists.Add(AsmList.CreateSource($"; {label.LabelShortName} = {value}"));
                } 

                if (MacroResult?.LineItems != default)
                {
                    foreach (var item in MacroResult.LineItems)
                    {
                        lists.AddRange(item.Lists);
                    }
                }

                // 先頭行は抜かす
                foreach (var item in lists.Skip(1))
                {
                    item.PushNestedCodeType(AsmList.NestedCodeTypeEnum.Macro);
                }

                return lists.ToArray();
            }
        }

        private LineDetailItemMacro(LineItem lineItem, AsmLoad asmLoad)
            : base(lineItem, asmLoad)
        {
        }

        public static LineDetailItemMacro Create(LineItem lineItem, AsmLoad asmLoad)
        {
            if (!lineItem.IsCollectOperationString)
            {
                return default(LineDetailItemMacro);
            }

            if (string.IsNullOrEmpty(lineItem.OperationString))
            {
                return default;
            }

            return new LineDetailItemMacro(lineItem, asmLoad);

        }

        public override void PreAssemble(ref AsmAddress asmAddress)
        {
            base.PreAssemble(ref asmAddress);

            var macro = Macro.Find(LineItem, AsmLoad);
            if (macro == default)
            {
                var errorMessage = "";
                if (LineItem.LineString.Length > 0 && string.IsNullOrEmpty(LineItem.LabelString) && !char.IsWhiteSpace(LineItem.LineString[0]) && char.IsAscii(LineItem.LineString[0]))
                {
                    errorMessage = "ラベルとして指定する場合には、末尾に:が必要です。";
                }
                else
                {
                    // 同名のラベルがあるか調査する
                    var macros = Macro.FindsWithoutNamespace(LineItem, AsmLoad);
                    if (macros != null && macros.Length > 0) 
                    {
                        errorMessage = $"マクロが見つからない場合には、ネームスペース付きを検討してください。[{string.Join(", ", macros.Select(m => m.FullName))}]";
                    }
                }

                // マクロが見つからないケースは、エラーとする
                throw new ErrorAssembleException(Error.ErrorCodeEnum.E0001, errorMessage);
            }

            MacroResult = macro.Macro.Expansion(LineItem, macro.Arguments, AsmLoad, ref asmAddress);
            this.LineDetailScopeItems = MacroResult.LineItems.SelectMany(m => m.LineDetailItem.LineDetailScopeItems ?? Array.Empty<LineDetailScopeItem>()).ToArray();
        }

        public override void AdjustAssemble(ref uint outputAddress)
        {
            Address = new AsmAddress(Address.Value.Program, outputAddress);
        }
    }
}
