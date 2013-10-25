using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.Diagnostics;
namespace BrowserSelector
{
    class Program
    {
        [DllImport("kernel32")]
        static extern bool AllocConsole();

        static void Main(string[] args)
        {
            if ( args.Length > 0 )
            {
                if ( args[0].ToLower() == "--install" )
                {
                    InstallDefaultBrowser();
                }
                else if ( args[0].ToLower() == "--help" )
                {
                    Help();
                }
                else
                {
                    RunCheck(args);
                }
            }
            else
            {
                Help();
            }
        }

        private static void Error(string message)
        {
            AllocConsole();
            Console.WriteLine(message);
            Console.Read();
        }

        const string jsonFile = "BrowserSelector.json";
        private static void RunCheck(string[] args)
        {
            var jsonPath = 
                Path.Combine(Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]) ,jsonFile);

            if ( !File.Exists(jsonPath) )
            {
                Error(jsonPath + "が見つかりません");
                return;
            }
            try
            {
                using ( var stream = File.OpenRead(jsonPath) )
                {
                    var data = JsonData.Load(stream);
                    Run(args, data.Items);
                }
            }
            catch ( Exception e )
            {
                Error(e.Message);
                return;
            }

        }

        public static void Run(string[] urls, JsonDataItem[] items)
        {
            var runList = new Dictionary<JsonDataItem, List<string>>(items.Length+1);
            foreach ( var url in urls )
            {
                var key = items.FirstOrDefault(x => x.Regex.IsMatch(url));
                if ( !runList.ContainsKey(key) )
                {
                    runList[key] = new List<string>();
                }
                runList[key].Add(url);
            }

            foreach ( var kv in runList )
            {
                var item = kv.Key;
                var urlList = kv.Value;

                if ( item == null )
                    continue;

                if ( item.IsSupportMultiUrl )
                {
                    var urlListStr
                        = string.Join<string>(" ", urlList);
                    var @params = string.IsNullOrEmpty(item.Params)
                        ? urlListStr
                        : item.Params.Replace("%1", urlListStr);
                    
                    Process.Start(item.ExePath, @params);
                }
                else
                {
                    
                    foreach (var url in urlList)
                    {
                        var param = string.IsNullOrEmpty(item.Params)
                          ? url
                          : item.Params.Replace("%1", url);
                        Process.Start ( item.ExePath, param);
                    }
                    Error("");
                }

            }
        }

        #region Help
        const string helpString = @"BrowserSelector.exe [ --install | --help ] [urls...]

--install
  BrowserSelector.exeをデフォルトブラウザに設定します。

--help
  現在のヘルプを表示します。

urls
  ブラウザで開くUrlを設定します。
";
        public static void Help()
        {
            Console.WriteLine(helpString); 
        }
        #endregion Help
        #region Install

        public static void InstallDefaultBrowser()
        {
            AllocConsole();
            Console.Write("BrowserSelector.exeをデフォルトブラウザに設定してもよろしいでしょうか？(Y/N):");
            if (Console.ReadKey(false).Key == ConsoleKey.Y)
            {
              Console.WriteLine();
              try
              {
                  var value = "\"" + Environment.GetCommandLineArgs()[0] + "\" \"%1\"";
                  WriteRegKey( Registry.ClassesRoot ,@"http\shell\open\command", value);
                  WriteRegKey( Registry.ClassesRoot ,@"https\shell\open\command", value);
              }
              catch ( Exception e)
                  {
                      Console.WriteLine(e.Message);
                  }
              Console.Read();
            }
        }

        private static void WriteRegKey(RegistryKey root, string key , string value)
        {
            string nowValue;
            using ( var regkey = root.OpenSubKey(key, true) )
            {
                nowValue = (string)regkey.GetValue(string.Empty);
                if ( value != nowValue )
                    regkey.SetValue(string.Empty, value, RegistryValueKind.String);
            }
        }

        #endregion Install
    }

}
