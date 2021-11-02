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


        public string ParseMessage { get; internal set; }
        public string HelpMessage
        {
            get
            {
                var result = default(string);
                var aplicationName = Assembly.GetExecutingAssembly().GetName().Name;
                result += $"{aplicationName}:\n";
                result += $"  {Description}\n\n";
                result += $"Usage:\n";
                result += $"  {aplicationName} [options]\n\n";
                result += $"Options:\n";
                foreach (var item in Options)
                {
                    var tmpValue = string.Join(", ", item.Aliases);
                    if (!(item is Option<bool>))
                    {
                        tmpValue += $" <{item.Name}>";
                    }
                    tmpValue = tmpValue.PadRight(25);
                    tmpValue += item.Description;

                    result += $"  {tmpValue}\n";
                }

                return result;
            }
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
                // 応答メッセージ有のオプションを処理
                foreach (var option in Options.Where(m => m.OptionFunc != default))
                {
                    if (args.Any(m => option.Aliases.Any(n => n == m)))
                    {
                        this.ParseMessage = option.OptionFunc.Invoke();
                        return false;
                    }
                }

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

            //ここでパースする
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

            foreach (var value in args)
            {
                if (value.StartsWith("-"))
                {
                    key = value;
                    result.Add(key, new List<string>());
                }
                else
                {
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