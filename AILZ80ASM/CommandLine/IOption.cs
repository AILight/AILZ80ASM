using System;
using System.Collections.Generic;

namespace AILZ80ASM.CommandLine
{
    public interface IOption
    {
        string Name { get; set; }
        string OptionName { get; set; }
        string[] Aliases { get; set; }
        string Description { get; set; }
        bool Required { get; set; }
        string DefaultValue { get; set; }
        Parameter[] Parameters { get; set; }
        bool HasValue { get; set; }
        Func<string[], string> OptionFunc { get; set; }

        void SetValue(List<string> value);
    }
}