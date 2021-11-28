using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;

namespace AILZ80ASM.CommandLine
{
    public class Option<T> : IOption
    {
        public string Name { get; set; }
        public string OptionName { get; set; }
        public string[] Aliases { get; set; }
        public string Description { get; set; }
        public bool Required { get; set; }
        public Func<string[], string> OptionFunc { get; set; }
        public T Value { get; set; } = default(T);
        public bool HasValue { get; set; }
        public string DefaultValue { get; set; }
        public Parameter[] Parameters { get; set; }

        public Option(string name, string[] aliases, string description, bool required)
            : this(name, default, aliases, description, required, default)
        {
        }

        public Option(string name, string optionName, string[] aliases, string description, bool required)
            : this(name, optionName, aliases, description, required, default)
        {
        }

        public Option(string name, string[] aliases, string description, bool required, Func<string[], string> optionFunc)
            : this(name, default, aliases, description, required, default, default, optionFunc)
        {
        }

        public Option(string name, string optionName, string[] aliases, string description, bool required, Func<string[], string> optionFunc)
            : this(name, optionName, aliases, description, required, default, default, optionFunc)
        {
        }

        public Option(string name, string[] aliases, string description, bool required, string defaultValue, Parameter[] parameters)
            : this(name, default, aliases, description, required, defaultValue, parameters, default)
        {

        }

        public Option(string name, string optionName, string[] aliases, string description, bool required, string defaultValue, Parameter[] parameters)
            : this(name, optionName, aliases, description, required, defaultValue, parameters, default)
        {

        }

        public Option(string name, string optionName, string[] aliases, string description, bool required, string defaultValue, Parameter[] parameters, Func<string[], string> optionFunc)
        {
            this.Name = name;
            this.OptionName = string.IsNullOrEmpty(optionName) ? name : optionName;
            this.Aliases = aliases;
            this.Description = description;
            this.Required = required;
            this.DefaultValue = defaultValue;
            this.Parameters = parameters;
            this.OptionFunc = optionFunc;

            if (this.Required && !string.IsNullOrEmpty(this.DefaultValue))
            {
                throw new Exception("RequiredがTrueですが、DefaultValueが設定されています。");
            }
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
            else if (typeof(T) == typeof(string))
            {
                if (value.Count > 1 && string.IsNullOrEmpty(DefaultValue))
                {
                    var optionName = "値";
                    if (Parameters != default)
                    {
                        optionName = string.Join(", ", Parameters.Select(m => m.Name));
                    }
                    throw new Exception($"{Name}に、{optionName}を指定する必要があります。");
                }

                if (value.Count == 0)
                {
                    Value = (T)(object)DefaultValue;
                }
                else
                {
                    var parameterName = value[0];
                    if (Parameters != default)
                    {
                        var parameter = Parameters.FirstOrDefault(m => string.Compare(m.Name, parameterName, true) == 0);
                        if (parameter != default)
                        {
                            Value = (T)(object)parameter.Name;
                        }
                        else
                        {
                            throw new Exception($"{Name}に、{string.Join(", ", Parameters.Select(m => m.Name))}を指定する必要があります。");
                        }
                    }
                    else
                    {
                        Value = (T)(object)parameterName;
                    }
                }
                HasValue = true;
            }
            else
            {
                throw new NotImplementedException();
            }    
        }
    }
}