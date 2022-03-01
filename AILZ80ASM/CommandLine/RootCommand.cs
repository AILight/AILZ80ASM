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
                foreach (var item in Options.Where(m => !m.IsHide))
                {
                    var commandList = new List<string>();
                    var commandWidth = 27;
                    var descriptionList = new List<string>();

                    // コマンド作成
                    var tmpList = new List<string>();
                    for (var index = 0; index < item.Aliases.Length; index++)
                    {
                        var outputAliase = item.Aliases[index];
                        if (index < item.Aliases.Length - 1)
                        {
                            outputAliase += ", ";
                        }
                        else if (index == item.Aliases.Length - 1 && item.GetType().GenericTypeArguments[0] != typeof(bool))
                        {
                            outputAliase += $" <{item.ArgumentName}>";
                        }

                        tmpList.Add(outputAliase);
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
                    var description = item.Description;
                    if (item.IsDefineOptional)
                    {
                        description += "(オプション名の省略が可能）";
                    }

                    descriptionList.Add(description);

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

                // ショートカット
                var shortcuts = option.Parameters.Where(m => !string.IsNullOrEmpty(m.ShortCut));
                if (shortcuts.Count() > 0)
                {
                    result += $"Shortcut Usage:\n";
                    result += $"  {ApplicationName} ";
                    result += string.Join(" ", shortcuts.Select(m => m.ShortCut));
                    result += $"\n\n";
                }

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
            if (string.IsNullOrEmpty(inputOption.ArgumentName))
            {
                inputOption.ArgumentName =inputOption.Name;
            }

            if (inputOption.Required && !string.IsNullOrEmpty(inputOption.DefaultValue))
            {
                throw new Exception("RequiredがTrueですが、DefaultValueが設定されています。");
            }

            Options.Add(inputOption);
        }

        public bool Parse(string[] args)
        {
            try
            {
                // 設定をクリアする
                foreach (var item in Options)
                {
                    item.Clear();
                }

                // デフォルト値の設定
                foreach (var item in Options.Where(m => !string.IsNullOrEmpty(m.DefaultValue)))
                {
                    item.SetValue(new[] { item.DefaultValue });
                    item.Selected = false;                      // デフォルト設定は選択をクリア
                }

                // オプションに値を積む
                foreach (var item in InternalParse(args))
                {
                    item.Key.SetValue(item.Value.ToArray());
                }

                // 応答メッセージ有のオプションを処理
                foreach (var option in Options.Where(m => m.OptionFunc != default))
                {
                    if (args.Any(m => option.Aliases.Any(n => string.Compare(n, m, true) == 0)))
                    {
                        if (option.GetType()?.GenericTypeArguments?.FirstOrDefault() == typeof(bool))
                        {
                            this.ParseMessage = option.OptionFunc.Invoke(new string[] { "" });
                        }
                        else
                        {
                            this.ParseMessage = option.OptionFunc.Invoke(new string[] { GetValue<string>(option.Name) });
                        }

                        return false;
                    }
                }

                // デフォルトオプションの実行（必須または、宣言のみ）
                foreach (var option in Options.Where(m => !m.HasValue && (m.Required || m.Selected) && m.DefaultFunc != default))
                {
                    var values = option.DefaultFunc.Invoke(Options.ToArray());
                    if (values.Length > 0)
                    {
                        option.SetValue(values);
                        option.Selected = false;
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

        public bool GetSelected(string targetName)
        {
            var option = Options.FirstOrDefault(m => m.Name == targetName);
            if (option == default)
            {
                throw new Exception($"有効な名前ではありません。{targetName}");
            }

            return option.Selected;
        }

        private Dictionary<IOption, List<string>> InternalParse(string[] args)
        {
            var result = new Dictionary<IOption, List<string>>();
            var existOptionForParameter = new List<IOption>();

            var key = default(IOption);
            var helpMode = false;
            var saveOption = default(IOption);
            var saveParameter = default(Parameter);
            // デフォルトパラメーターを取得
            var defineOptionalOption = Options.FirstOrDefault(m => m.IsDefineOptional);


            foreach (var value in args)
            {
                if (value.StartsWith("-") && !helpMode)
                {
                    // saveParameterに値があったら、オプションとして積む
                    ParameterEntry(saveOption, saveParameter, existOptionForParameter, result);

                    // Shortcutチェック
                    var matched = false;
                    foreach (var option in Options)
                    {
                        saveOption = option;
                        saveParameter = option.Parameters?.FirstOrDefault(m => !string.IsNullOrEmpty(m.ShortCut) && m.ShortCut == value);
                        if (saveParameter != default && !existOptionForParameter.Contains(option))
                        {
                            matched = true;
                            break;
                        }
                        else if (option.Aliases.Any(m => m == value))
                        {
                            // オプションとマッチ
                            key = option;
                            if (!result.ContainsKey(key))
                            {
                                result.Add(key, new List<string>());
                                matched = true;
                                // ヘルプチェック
                                helpMode = option.IsHelp;

                                break;
                            }
                        }
                    }
                    if (!matched)
                    {
                        throw new Exception($"{value}は、有効なコマンドではありません。");
                    }
                }
                else
                {
                    helpMode = false;
                    // デフォルトの宣言対応
                    if (key == default && defineOptionalOption != default)
                    {
                        key = defineOptionalOption;
                        result.Add(key, new List<string>());
                    }

                    // パラメータ直後の値の場合は、オプションとのマッチを行う
                    if (saveParameter != default)
                    {
                        var option = Options.FirstOrDefault(m => m.Aliases.Contains(saveParameter.ShortCut));
                        if (option != default)
                        {
                            key = option;
                            if (!result.ContainsKey(key))
                            {
                                result.Add(key, new List<string>());
                            }
                        }
                        else
                        {
                            throw new Exception($"{value}は、有効な値ではありません。");
                        }
                        saveParameter = default;
                    }

                    // 値を積む
                    result[key].Add(value);
                }
            }

            // saveParameterに値があったら、オプションとして積む
            ParameterEntry(saveOption, saveParameter, existOptionForParameter, result);

            return result;

            static void ParameterEntry(IOption option, Parameter parameter, List<IOption> existOptionForParameter, Dictionary<IOption, List<string>> result)
            {
                if (parameter != default)
                {
                    if (!result.ContainsKey(option))
                    {
                        result.Add(option, new List<string>());
                    }
                    result[option].Add(parameter.Name);
                    existOptionForParameter.Add(option);
                }
            }
        }
    }
}