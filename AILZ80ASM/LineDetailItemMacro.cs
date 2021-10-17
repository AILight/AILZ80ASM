using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace AILZ80ASM
{
    public class LineDetailItemMacro : LineDetailItem
    {
        private Macro Macro { get; set; }
        private string[] Argmuments { get; set; }

        private LineDetailItemMacro(LineItem lineItem, AsmLoad asmLoad)
            : base(lineItem, asmLoad)
        {
        }

        public static LineDetailItem Create(LineItem lineItem, AsmLoad asmLoad)
        {
            if (string.IsNullOrEmpty( lineItem.OperationString))
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
                    throw new ErrorAssembleException(Error.ErrorCodeEnum.E1009, foundItem.Macro.FullName);
                }
            }

            this.LineDetailScopeItems = foundItem.Macro.Expansion(LineItem, foundItem.Arguments, AsmLoad);

            base.ExpansionItem();
        }

    }
}
