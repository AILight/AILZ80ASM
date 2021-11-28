using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;

namespace AILZ80ASM.CommandLine
{
    public class RootCommand
    {
        private string Description { get; set; }
        private List<IOption> Options { get; set; } = new();
        private string ApplicationName => Assembly.GetExecutingAssembly().GetName().Name;
        private string HelpTitleMessage
        {
            get
            {
                var result = default(string);
                result += $"{ApplicationName}:\n";
                result += $"  {Description}\n";

                return result;
            }
        }

        public string ParseMessage { get; internal set; }
        public string HelpMessage
        {
            get
            {
                var result = default(string);
                result += $"{HelpTitleMessage}\n";
                result += $"Usage:\n";
                result += $"  {ApplicationName} [options]\n\n";
                result += $"Options:\n";
                foreach (var item in Options)
                {
                    var commandList = new List<string>();
                    var commandWidth = 27;
                    var descriptionList = new List<string>();

                    // コマンド作成
                    var tmpList = new List<string>();
                    foreach (var index in Enumerable.Range(0, item.Aliases.Length))
                    {
                        tmpList.Add(item.Aliases[index] + (index == item.Aliases.Length - 1 ? item.GetType().GenericTypeArguments[0] != typeof(bool) ? $" <{item.OptionName}>" : "" : ", "));
                    }

                    // コマンド側のリストを作成
                    foreach (var command in tmpList)
                    {
                        if (commandList.Count == 0 || (commandList.Last() + command).Length > commandWidth)
                        {
                            commandList.Add("");
                        }
                        commandList[commandList.Count - 1] += command;
                    }

                    // 説明文の作成
                    descriptionList.Add(item.Description);

                    // 表示処理を作成
                    foreach (var index in Enumerable.Range(0, Math.Max(commandList.Count, descriptionList.Count)))
                    {
                        var tmpComand = (commandList.Count > index ? commandList[index] : "").PadRight(commandWidth);
                        var tmpDescription = descriptionList.Count > index ? descriptionList[index] : "";

                        result += $"  {tmpComand}{tmpDescription}\n";
                    }
                }

                return result;
            }
        }

        public string HelpCommand(string[] arguments)
        {
            if (arguments == default || string.IsNullOrEmpty(arguments[0]))
            {
                return HelpMessage;
            }

            var result = default(string);
            result += $"{HelpTitleMessage}\n";
            result += $"Usage:\n";
            foreach (var argument in arguments)
            {
                var option = this.Options.Where(m => m.Aliases.Contains(argument)).FirstOrDefault();
                if (option == default)
                {
                    continue;
                }
                result += $"  {ApplicationName} {argument} <parameter>\n\n";

                if (!string.IsNullOrEmpty(option.Description))
                {
                    result += $"Description:\n";
                    result += $"  {option.Description}\n\n";
                }

                if (!string.IsNullOrEmpty(option.DefaultValue))
                {
                    result += $"Default:\n";
                    result += $"  {option.DefaultValue}\n\n";
                }

                if (option.Parameters != default)
                {
                    result += $"Parameters:\n";
                    var maxLength = option.Parameters.Select(m => m.Name.Length).Max();
                    if (maxLength < 5)
                    {
                        maxLength = 5;
                    }
                    foreach (var parameter in option.Parameters)
                    {
                        result += $"  {parameter.Name}".PadRight(maxLength + 3) + $"{parameter.Description}\n";
                    }
                    result += $"\n";
                }

            }
            return result;
        }

        public RootCommand(string description)
        {
            this.Description = description;
        }

        public void AddOption<T>(Option<T> inputOption)
        {
            Options.Add(inputOption);
        }

        public bool Parse(string[] args)
        {
            try
            {
                // オプションに値を積む
                foreach (var item in InternalParse(args))
                {
                    var option = Options.FirstOrDefault(m => m.Aliases.Contains(item.Key));
                    if (option == default)
                    {
                        throw new Exception($"{item.Key}は、有効なコマンドではありません。");
                    }
                    option.SetValue(item.Value);
                }

                // 応答メッセージ有のオプションを処理
                foreach (var option in Options.Where(m => m.OptionFunc != default))
                {
                    if (args.Any(m => option.Aliases.Any(n => n == m)))
                    {
                        this.ParseMessage = option.OptionFunc.Invoke(new string[] { GetValue<string>(option.Name) });
                        return false;
                    }
                }

                // 必須チェック
                {
                    var option = Options.FirstOrDefault(m => m.Required && !m.HasValue);
                    if (option != default)
                    {
                        throw new Exception($"{option.Name}は必須項目です。値を指定する必要があります。");
                    }
                }
            }
            catch (Exception ex)
            {
                this.ParseMessage = $"{ex.Message}\n\n{this.HelpMessage}";
                return false;
            }

            return true;
            
        }

        public T GetValue<T>(string targetName)
        {
            var option = Options.FirstOrDefault(m => m.Name == targetName);
            if (option == default)
            {
                throw new Exception($"有効な名前ではありません。{targetName}");
            }

            return ((Option<T>)option).Value;
        }


        /// <summary>
        /// 引数をParseする
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private Dictionary<string, List<string>> InternalParse(string[] args)
        {
            var result = new Dictionary<string, List<string>>();
            var key = default(string);
            var helpMode = false;

            foreach (var value in args)
            {
                if (value.StartsWith("-") && !helpMode)
                {
                    key = value;
                    result.Add(key, new List<string>());
                    // HelpModeチェック
                    if (Options.Where(m => m.Name == "help").Any(m => m.Aliases.Contains(key)))
                    {
                        helpMode = true;
                    }
                }
                else
                {
                    helpMode = false;
                    if (!result.ContainsKey(key))
                    {
                        throw new Exception($"コマンドの指定が間違っています。{string.Join(" ", args)}");
                    }

                    result[key].Add(value);
                }
            }

            return result;
        }

    }
}