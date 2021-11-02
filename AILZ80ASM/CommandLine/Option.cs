using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;

namespace AILZ80ASM.CommandLine
{
    public class Option<T> : IOption
    {
        public string Name { get; set; }
        public string[] Aliases { get; set; }
        public string Description { get; set; }
        public bool Required { get; set; }
        public Func<string> OptionFunc { get; set; }
        public T Value { get; set; } = default(T);
        public bool HasValue { get; set; }

        public Option(string name, string[] aliases, string description, bool required)
        {
            this.Name = name;
            this.Aliases = aliases;
            this.Description = description;
            this.Required = required;
        }

        public Option(string name, string[] aliases, string description, bool required, Func<string> optionFunc)
        {
            this.Name = name;
            this.Aliases = aliases;
            this.Description = description;
            this.Required = required;
            this.OptionFunc = optionFunc;
        }

        public void SetValue(List<string> value)
        {
            if (typeof(T) == typeof(FileInfo))
            {
                if (value.Count != 1)
                {
                    throw new Exception($"{Name}に、ファイルを指定する必要があります。（1ファイルのみ指定可能）");
                }

                Value = (T)(object)new FileInfo(value.First());
                HasValue = true;
            }
            else if (typeof(T) == typeof(FileInfo[]))
            {
                if (value.Count == 0)
                {
                    throw new Exception($"{Name}に、ファイルを指定する必要があります。（複数ファイル指定可能）");
                }

                Value = (T)(object)value.Select(m => new FileInfo(m)).ToArray();
                HasValue = true;
            }
            else if (typeof(T) == typeof(bool))
            {
                Value = (T)(object)true;
                HasValue = true;
            }
            else
            {
                throw new NotImplementedException();
            }    
        }
    }
}