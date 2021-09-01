using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace AILZ80ASM
{
    public class Macro
    {
        public string GlobalLabelName { get; private set; }
        public string Name { get; private set; }
        public string FullName => $"{GlobalLabelName}.{Name}";

        public List<string> Args { get; private set; } = new List<string>();
        public List<LineItem> LineItems { get; private set; } = new List<LineItem>();

        public Macro(string macroName, string args, LineItem[] lineItems, AsmLoad asmLoad)
        {
            this.GlobalLabelName = asmLoad.GlobalLableName;
            this.Name = macroName;
            Args.AddRange(args.Split(',').Select(m => m.Trim()));

            LineItems.AddRange(lineItems);
        }

        /*
        public Macro(List<LineItem> lineItems, FileItem fileItem)
        {
            FileItem = fileItem;
            GlobalMacroName = fileItem.WorkGlobalLabelName;
            foreach (var item in lineItems)
            {
                //item.IsAssembled = true;
            }

            var firstLineItem = lineItems.First();
            var macroIndex = firstLineItem.OperationString.IndexOf("macro", StringComparison.OrdinalIgnoreCase);
            var operationString = firstLineItem.OperationString.Substring(macroIndex + 5).TrimStart();
            var argsIndex = operationString.IndexOf(" ");
            if (argsIndex == -1)
            {
                Name = operationString;
            }
            else
            {
                Name = operationString.Substring(0, argsIndex);
                Args.AddRange(operationString.Substring(argsIndex).Split(",").Select(m => m.Trim()));
            }

            LineItems.AddRange(lineItems.Skip(1).SkipLast(1));
        }
        */

    }
}
