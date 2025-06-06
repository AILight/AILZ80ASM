using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;

namespace AILZ80ASM.CommandLine
{
    public class RootCommand
    {
        private const int COMMAND_WIDTH = 27;

        private string Description { get; set; }
        private List<IOption> Options { get; set; } = new();
        private string ApplicationName => Assembly.GetExecutingAssembly().GetName().Name;
        private string HelpTitleMessage
        {
            get
            {
                var result = default(string);
                result += $"{ApplicationName}:{Environment.NewLine}";
                result += $"  {Description}{Environment.NewLine}";

                return result;
            }
        }

        public string ParseMessage { get; internal set; }
        public string HelpMessage
        {
            get
            {
                return CreateHelpMessage(false);
            }
        }

        public bool HasHelpArgument(string[] args)
        {
            return Options.Any(m => m.IsHelp && m.Aliases.Any(n => args.Any(i => n == i)));
        }

        public string CreateHelpMessage(bool simpleMessage)
        {
            var result = default(string);
            result += $"{HelpTitleMessage}{Environment.NewLine}";
            result += $"Usage:{Environment.NewLine}";
            result += $"  {ApplicationName} [options]{Environment.NewLine}{Environment.NewLine}";

            // オプション
            result += $"Options:{Environment.NewLine}";
            result += OptionsToString(Options.Where(m => !m.IsHide && !m.IsShortCut && (!simpleMessage || m.IsSimple)));

            // ショートカット
            var options = Options.Where(m => !m.IsHide && m.IsShortCut && (!simpleMessage || m.IsSimple));
            if (options.Any())
            {
                result += $"{Environment.NewLine}";
                result += $"ShortCut Options:{Environment.NewLine}";
                result += OptionsToString(options);
            }
            return result;
        }

        public string HelpCommand(string[] arguments)
        {
            if (arguments == default || string.IsNullOrEmpty(arguments[0]))
            {
                return HelpMessage;
            }

            var result = default(string);
            result += $"{HelpTitleMessage}{Environment.NewLine}";
            result += $"Usage:{Environment.NewLine}";
            foreach (var argument in arguments)
            {
                var option = this.Options.Where(m => m.Aliases.Contains(argument)).FirstOrDefault();
                if (option == default)
                {
                    continue;
                }
                result += $"  {ApplicationName} {argument} <{option.ArgumentName}>{Environment.NewLine}{Environment.NewLine}";

                // ショートカット
                var shortcuts = option.Parameters?.Where(m => !string.IsNullOrEmpty(m.ShortCut)) ?? Array.Empty<Parameter>();
                if (shortcuts.Count() > 0)
                {
                    result += $"Shortcut Usage:{Environment.NewLine}";
                    result += $"  {ApplicationName} ";
                    result += string.Join(" ", shortcuts.Select(m => m.ShortCut));
                    result += $"{Environment.NewLine}{Environment.NewLine}";
                }

                if (!string.IsNullOrEmpty(option.Description))
                {
                    result += $"Description:{Environment.NewLine}";
                    result += $"  {option.Description}{Environment.NewLine}{Environment.NewLine}";
                }

                if (!string.IsNullOrEmpty(option.DefaultValue))
                {
                    result += $"Default:{Environment.NewLine}";
                    result += $"  {option.DefaultValue}{Environment.NewLine}{Environment.NewLine}";
                }

                if (option.Parameters != default)
                {
                    result += $"Parameters:{Environment.NewLine}";
                    var maxLength = option.Parameters.Select(m => m.Name.Length).Max();
                    if (maxLength < 5)
                    {
                        maxLength = 5;
                    }
                    foreach (var parameter in option.Parameters)
                    {
                        result += $"  {parameter.Name}".PadRight(maxLength + 3) + $"{parameter.Description}{Environment.NewLine}";
                    }
                    result += $"{Environment.NewLine}";

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

            // 既に登録済みの名前だとエラー
            if (Options.Any(m => m.Name == inputOption.Name))
            {
                throw new Exception($"登録済みのオプションです。Name:{inputOption.Name}");
            }

            // 起動オプション名の重複チェック
            if (Options.Any(m => m.Aliases.Any(n => inputOption.Aliases.Contains(n))))
            {
                throw new Exception($"登録済みのエイリアスです。Alias:{string.Join(",", inputOption.Aliases)}");
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
                    item.SettingDefaultValue();
                }

                // オプションに値を積む
                foreach (var item in InternalParse(args))
                {
                    item.Key.SetValue(item.Value.ToArray());
                }

                // 応答メッセージ有のオプションを処理
                foreach (var option in Options.Where(m => m.OptionFunc != default))
                {
                    if (args.Any(m => option.Aliases.Any(n => string.Equals(n, m, StringComparison.OrdinalIgnoreCase))))
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
                this.ParseMessage = $"{ex.Message}{Environment.NewLine}{Environment.NewLine}{CreateHelpMessage(true)}";
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
                if (value.StartsWith('-') && !helpMode)
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

        public static string OptionsToString(IEnumerable<IOption> options)
        {
            var result = "";
            var commandWidth = options.Max(m => ($"{String.Join(", ", m.Aliases)} {options.Max(m => (m.GetType().GenericTypeArguments[0] != typeof(bool) ? $"<{m.ArgumentName}>" : ""))}").Length);

            foreach (var option in options)
            {
                result += OptionToString(option, commandWidth + 1);
            }

            return result;
        }


        public static string OptionToString(IOption option, int commandWidth)
        {
            var result = "";
            var command = String.Join(", ", option.Aliases);
            var argument = option.GetType().GenericTypeArguments[0] != typeof(bool) ? $"<{option.ArgumentName}>" : "";
            var description = option.Description + (option.IsDefineOptional ? " (オプション名の省略が可能)" : "");
            var optionDescription = option.Parameters != default && option.Parameters.Length > 0 ? $" [{string.Join(", ", option.Parameters.Select(m => m.Name))}]" : "";
            var optionDefault = !string.IsNullOrEmpty(option.DefaultValue) ? $" デフォルト値:{option.DefaultValue}" : "";

            var commandList = new List<string>();
            var descriptionList = new List<string>();

            // コマンド作成
            commandList.Add($"{command} {argument}".PadRight(commandWidth));

            // 説明文作成
            if (GetWidth(description + optionDescription + optionDefault) + commandWidth < 110 || string.IsNullOrEmpty(optionDescription))
            {
                descriptionList.Add(description + optionDescription + optionDefault);
            }
            else
            {
                descriptionList.Add(description);
                descriptionList.Add((optionDescription + optionDefault).TrimStart());
            }

            // 行数を合わせる
            if (commandList.Count > descriptionList.Count)
            {
                descriptionList.Add("");
            }
            else if (commandList.Count < descriptionList.Count)
            {
                commandList.Add("");
            }

            for (var index = 0; index < commandList.Count; index++)
            {
                result += $"  {commandList[index].PadRight(commandWidth)}";
                result += $" {descriptionList[index]}{Environment.NewLine}";

            }

            return result;
        }

        /// <summary>
        /// 文字の幅を求める
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        private static int GetWidth(string target)
        {
            return target.ToArray().Sum(m => char.IsAscii(m) ? 1 : 2);
        }
    }
}