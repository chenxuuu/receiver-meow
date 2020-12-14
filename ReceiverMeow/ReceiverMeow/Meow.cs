using System;
using System.Collections.Generic;
using System.Text;
using WebSocketSharp;

namespace ReceiverMeow
{
    class Meow
    {
        static void Main(string[] args)
        {
            Tools.Initial();

            //处理命令行
            string wsUrl = "127.0.0.1";
            int wsPort = 6700;
            string httpUrl = "127.0.0.1";
            int httpPort = 5700;
            if (args.Length >= 4)
            {
                try
                {
                    wsPort = int.Parse(args[1]);
                    httpPort = int.Parse(args[3]);
                    wsUrl = args[0];
                    httpUrl = args[2];
                }
                catch
                {
                    Log.Error("终端", $"参数错误，请使用 xxx.exe [wsUrl] [wsPort] [httpUrl] [httpPort]");
                }
            }
            GoHttp.Http.Set(httpUrl, httpPort);
            GoHttp.Ws.Connect(wsUrl, wsPort);

            //命令行命令处理
            Dictionary<string, CommandContain> commandList = new Dictionary<string, CommandContain>
            {
                ["rua".ToUpper()] = new CommandContain
                {
                    explain = "重载所有lua虚拟机",
                    run = (s) => { }
                },
                ["color".ToUpper()] = new CommandContain
                {
                    explain = "切换终端颜色显示。如果显示不正常，可以更改此选项关闭颜色",
                    run = (s) => 
                    {
                        Tools.Setting.Colorful = !Tools.Setting.Colorful;
                        Log.Info($"命令", $"彩色终端状态：{Tools.Setting.Colorful}");
                    }
                },
#if DEBUG
                ["ws".ToUpper()] = new CommandContain
                {
                    explain = "发送ws数据",
                    run = (s) => GoHttp.Ws.Send(Encoding.Default.GetBytes(s.Substring(3)))
                },
                ["http".ToUpper()] = new CommandContain
                {
                    explain = "发送http数据",
                    run = (s) => {
                        s = s.Substring(5);
                        var index = s.IndexOf(" ");
                        var r = GoHttp.Http.Send(s.Substring(0, index), s.Substring(index + 1));
                        Log.Debug("http返回", r);
                    }
                },
#endif
            };
            while(true)
            {
                var command = Console.ReadLine();
                var cp = command.IndexOf(" ") >= 0 ? command.Substring(0, command.IndexOf(" ")) : command;
                if(commandList.ContainsKey(cp.ToUpper()))
                {
                    try
                    {
                        commandList[cp.ToUpper()].run(command);
                        Log.Info($"命令", $"执行完毕");
                    }
                    catch (Exception e)
                    {
                        Log.Warn("命令", $"{command}执行报错，错误信息：\n{e.Message}");
                    }
                }
                else if(command.ToUpper() == "HELP" || command == "?" || command == "？")
                {
                    Log.Info("命令", "可用命令列表：");
                    foreach (var item in commandList)
                    {
                        Log.Info($"{item.Key.ToLower()}", $"\t{item.Value.explain}");
                    }
                }
                else
                {
                    Log.Info("命令", $"未知命令，使用help获取命令帮助");
                }
            }
        }
    }

    class CommandContain
    {
        public string explain = "";
        public Action<string> run;
    }
}
