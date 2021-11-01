using System.Collections.Generic;
using System.Linq;

namespace AILZ80ASM
{
    public class LineDetailItemMacro : LineDetailItem
    {
        private Macro Macro { get; set; }
        private string[] Argmuments { get; set; }
        public override AsmList[] Lists
        {
            get
            {
                var lists = new List<AsmList>();
                lists.Add(AsmList.CreateLineItem(LineItem));

                foreach (var item in this.LineDetailScopeItems)
                {
                    lists.AddRange(item.Lists);
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

        public static LineDetailItem Create(LineItem lineItem, AsmLoad asmLoad)
        {
            if (string.IsNullOrEmpty(lineItem.OperationString))
            {
                return default;
            }

            return new LineDetailItemMacro(lineItem, asmLoad);

        }

        public override void ExpansionItem()
        {
            var foundItem = Macro.Find(LineItem, AsmLoad);
            if (foundItem == default)
            {
                // グローバル名
                foundItem = Macro.FindWithoutLongName(LineItem, AsmLoad);
                if (foundItem == default)
                {
                    throw new ErrorAssembleException(Error.ErrorCodeEnum.E0001);
                }
                else
                {
                    throw new ErrorAssembleException(Error.ErrorCodeEnum.E3009, foundItem.Macro.FullName);
                }
            }

            this.LineDetailScopeItems = foundItem.Macro.Expansion(LineItem, foundItem.Arguments, AsmLoad);

            base.ExpansionItem();
        }
    }
}
