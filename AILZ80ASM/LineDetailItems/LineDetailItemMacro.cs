using AILZ80ASM.Assembler;
using AILZ80ASM.Exceptions;
using System.Linq;
using System.Collections.Generic;

namespace AILZ80ASM.LineDetailItems
{
    public class LineDetailItemMacro : LineDetailItem
    {
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

        public override void PreAssemble(ref AsmAddress asmAddress)
        {
            base.PreAssemble(ref asmAddress);

            var foundItem = Macro.Find(LineItem, AsmLoad);
            if (foundItem == default)
            {
                // マクロが見つからないケースは、エラーとする
                throw new ErrorAssembleException(Error.ErrorCodeEnum.E0001);
            }

            this.LineDetailScopeItems = foundItem.Macro.Expansion(LineItem, foundItem.Arguments, AsmLoad, ref asmAddress);
        }
    }
}
