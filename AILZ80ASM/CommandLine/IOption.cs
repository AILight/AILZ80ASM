using System;
using System.Collections.Generic;

namespace AILZ80ASM.CommandLine
{
    public interface IOption
    {
        string Name { get; set; }
        string[] Aliases { get; set; }
        string Description { get; set; }
        bool Required { get; set; }
        bool HasValue { get; set; }
        Func<string> OptionFunc { get; set; }

        void SetValue(List<string> value);
    }
}