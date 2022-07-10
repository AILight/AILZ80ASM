using System;
using System.Collections.Generic;

namespace AILZ80ASM.CommandLine
{
    public interface IOption
    {
        string Name { get; set; }
        string ArgumentName { get; set; }
        string[] Aliases { get; set; }
        string Description { get; set; }
        bool Required { get; set; }
        bool IsDefineOptional { get; set; }
        bool IsHide { get; set; }
        bool IsSimple { get; set; }
        bool IsShortCut { get; set; }
        bool IsHelp { get; set; }
        string DefaultValue { get; set; }
        Parameter[] Parameters { get; set; }
        bool HasValue { get; set; }
        bool Selected { get; set; }
        Func<string[], string> OptionFunc { get; set; }
        Func<IOption[], string[]> DefaultFunc { get; set; }

        void Clear();
        void SetValue(string[] values);
        void SettingDefaultValue();
    }

}