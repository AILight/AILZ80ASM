using AILZ80ASM.AILight;
using AILZ80ASM.Assembler;
using AILZ80ASM.Exceptions;
using AILZ80ASM.LineDetailItems.ScopeItem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AILZ80ASM.LineDetailItems
{
    public class LineDetailItemEnum : LineDetailItem
    {
        private static readonly string RegexPatternEnumStart = @"^(?<enum_name>[a-zA-Z0-9_]+)\s+Enum\s*$";
        private static readonly string RegexPatternEnumItem = @"^\s*(?<item>[a-zA-Z0-9_]+)";
        private static readonly string RegexPatternEnumEnd = @"^\s*Endm\s*$";

        public class EnumItem
        {
            public LineItem LineItem { get; set; }
            public LineDetailItemEnum LineDetailEnumItem { get; set; }
            public string Name { get; set; }
            public string Value { get; set; }
            public string Length { get; set; } = "1";
        }

        public enum ItemModeEnum
        {
            None,
            Item
        }

        public ItemModeEnum ItemMode { get; set; } = ItemModeEnum.None;
        public string EnumName { get; set; }
        public string EnumValue { get; set; } = "0";

        public LineDetailItem LineDetailItemEqualItem { get; set; }
        public List<EnumItem> EnumItems { get; private set; } = new List<EnumItem>();

        public override AsmList[] Lists
        {
            get
            {
                if (!AsmLoad.Share.IsOutputList)
                {
                    return new AsmList[] { };
                }

                if (ItemMode == ItemModeEnum.Item)
                {
                    if (LineDetailItemEqualItem is LineDetailItemEqual lineDetailItemEqual &&
                        lineDetailItemEqual.EquLabel.Value != default &&
                        lineDetailItemEqual.EquLabel.Value.TryParse<UInt16>(out var resultValue))
                    {
                        return new[]
                        {
                            AsmList.CreateLineItemEnum(resultValue, this.LineItem)
                        };
                    }
                    else
                    {
                        return base.Lists;
                    }
                }
                else
                {
                    return base.Lists;
                }
            }
        }

        private LineDetailItemEnum(LineItem lineItem, AsmLoad asmLoad)
            : base(lineItem, asmLoad)
        {

        }

        private LineDetailItemEnum(LineItem lineItem, string enumName, AsmLoad asmLoad)
            : base(lineItem, asmLoad)
        {
            EnumName = enumName;
        }

        private LineDetailItemEnum(LineItem lineItem, string enumName, string enumValue, AsmLoad asmLoad)
            : base(lineItem, asmLoad)
        {
            EnumName = enumName;
            EnumValue = enumValue;
            ItemMode = ItemModeEnum.Item;
        }

        public static LineDetailItemEnum Create(LineItem lineItem, AsmLoad asmLoad)
        {
            if (!lineItem.IsCollectOperationString)
            {
                return default(LineDetailItemEnum);
            }

            var ifMatched = Regex.Match(lineItem.OperationString, RegexPatternEnumStart, RegexOptions.Singleline | RegexOptions.IgnoreCase);
            var endMatched = Regex.Match(lineItem.OperationString, RegexPatternEnumEnd, RegexOptions.Singleline | RegexOptions.IgnoreCase);

            // 条件処理処理中
            if (asmLoad.Share.LineDetailItemForExpandItem is LineDetailItemEnum asmLoad_LineDetailItemEnum)
            {
                // 終了条件チェック
                if (endMatched.Success)
                {
                    asmLoad.Share.LineDetailItemForExpandItem = default;
                    return new LineDetailItemEnum(lineItem, asmLoad);
                }

                // アイテムを
                var enumItemMatched = Regex.Match(lineItem.OperationString, RegexPatternEnumItem, RegexOptions.Singleline | RegexOptions.IgnoreCase);

                if (enumItemMatched.Success)
                {
                    try
                    {
                        var itemName = enumItemMatched.Groups["item"].Value;
                        var option = lineItem.OperationString.Substring(itemName.Length).Trim();
                        var length = "1";
                        var value = asmLoad_LineDetailItemEnum.EnumValue;

                        if (option.Length > 0)
                        {
                            var indexOfEqual = IndexOfEqual(option);
                            if (option[0] == ':')
                            {
                                // レングスを利用している
                                if (indexOfEqual == -1)
                                {
                                    length = option.Substring(1).Trim();
                                }
                                else
                                {
                                    length = option.Substring(1, indexOfEqual - 1).Trim();
                                }
                            }
                            if (indexOfEqual != -1)
                            {
                                value = option.Substring(indexOfEqual + 1).Trim();
                            }
                        }
                        // valueの中に、Itemの要素があった場合に、.を付けてローカルラベルにする。
                        foreach (var item in asmLoad_LineDetailItemEnum.EnumItems)
                        {
                            value = Regex.Replace(value, @$"(?<![a-zA-Z0-9_.])({item.Name})(?![a-zA-Z0-9_])", ".$1", RegexOptions.Singleline | RegexOptions.IgnoreCase);
                        }

                        var lineDetailItemEnum = new LineDetailItemEnum(lineItem, itemName, value, asmLoad);
                        var enumItem = new EnumItem
                        {
                            LineItem = lineItem,
                            Name = itemName,
                            Value = value,
                            Length = length,
                            LineDetailEnumItem = lineDetailItemEnum
                        };
                        asmLoad_LineDetailItemEnum.EnumItems.Add(enumItem);
                        asmLoad_LineDetailItemEnum.EnumValue = $"({enumItem.Value}) + ({enumItem.Length})";

                        return lineDetailItemEnum;
                    }
                    catch
                    {
                        throw new ErrorAssembleException(Error.ErrorCodeEnum.E6102);
                    }
                }
                else if (string.IsNullOrEmpty(lineItem.OperationString))
                {
                    var lineDetailItemEnum = new LineDetailItemEnum(lineItem, asmLoad);
                    return lineDetailItemEnum;
                }
                else
                {
                    throw new ErrorAssembleException(Error.ErrorCodeEnum.E6102);
                }
            }
            else
            {
                // 終了条件チェック
                if (endMatched.Success)
                {
                    throw new ErrorAssembleException(Error.ErrorCodeEnum.E6101);
                }

                // 開始条件チェック
                if (ifMatched.Success)
                {
                    var enumName = ifMatched.Groups["enum_name"].Value;
                    if (!AIName.DeclareLabelValidate($"{enumName}:", asmLoad))
                    {
                        throw new ErrorAssembleException(Error.ErrorCodeEnum.E6103);
                    }

                    var lineDetailItemEnum = new LineDetailItemEnum(lineItem, enumName, asmLoad);
                    asmLoad.Share.LineDetailItemForExpandItem = lineDetailItemEnum;

                    return lineDetailItemEnum;
                }
            }

            return default;
        }
        
        /// <summary>
        /// イコールの場所を探し出す処理
        /// </summary>
        /// <param name="tareget"></param>
        /// <returns></returns>
        private static int IndexOfEqual(string tareget)
        {
            var index = tareget.IndexOf("=");
            if (index == -1)
            {
                return -1;
            }
            while (index != -1)
            {
                // 以下の物以外は、=の場所と判断する
                // <=, >=, ==, !=
                if ((index > 0 && (new []{'<', '>', '!', '='}).Any(m => m == tareget[index - 1])) ||
                    (index < tareget.Length - 1 && tareget[index + 1] == '='))
                {
                    // 演算子の場合は、こちらに来る
                }
                else
                {
                    return index;
                }

                index = tareget.IndexOf("=", index + 1);
            }
            return -1;
        }

        public override void ExpansionItem()
        {
            //base.ExpansionItem();
            foreach (var item in EnumItems)
            {
                var lineItem = new LineItem($"{EnumName}.{item.Name} EQU {item.Value}", item.LineItem.LineIndex, item.LineItem.FileInfo);
                item.LineDetailEnumItem.LineDetailItemEqualItem = LineDetailItem.CreateLineDetailItem(lineItem, AsmLoad);
            }
        }

        public override void PreAssemble(ref AsmAddress asmAddress)
        {
            LineDetailItemEqualItem?.PreAssemble(ref asmAddress);
            base.PreAssemble(ref asmAddress);
        }
    }
}
