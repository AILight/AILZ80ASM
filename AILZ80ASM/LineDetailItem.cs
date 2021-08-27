using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace AILZ80ASM
{
    public abstract class LineDetailItem
    {

        public static LineDetailItem CreateLineDetailItem(string operationString)
        {
            // インクルードのチェック
            var lineDetailItem = default(LineDetailItem);

            lineDetailItem = lineDetailItem ?? LineDetailItemInclude.Create(operationString);
            lineDetailItem = lineDetailItem ?? new LineDetailItemOperation(operationString);

            return lineDetailItem;
        }

        public virtual LineAssemblyItem[] LineAssemblyItems { get; } = new LineAssemblyItem[] { };
    }
}
