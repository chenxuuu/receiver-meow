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
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Utils.Initial();

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
            if (!(args.Length >= 1 && args[0] == "nows"))
            {
                GoHttp.Http.Set(httpUrl, httpPort);
                GoHttp.Ws.Connect(wsUrl, wsPort);
            }

            //lua启动事件
            Utils.ReloadLua();
            //命令行命令处理
            ReceiveCommands();
        }


        private static void ReceiveCommands()
        {
            //命令列表
            Dictionary<string, CommandContain> commandList = new Dictionary<string, CommandContain>
            {
                ["admin".ToUpper()] = new CommandContain
                {
                    explain = "设置/查看管理员qq号",
                    run = (s) =>
                    {
                        if(s.Contains(" "))
                        {
                            long qq;
                            if(long.TryParse(s.Substring(6),out qq))
                            {
                                Utils.Setting.AdminQQ = qq;
                                Log.Info("管理员QQ", $"管理员QQ更改为{Utils.Setting.AdminQQ}");
                            }
                        }
                        else
                        {
                            Log.Info("管理员QQ", $"当前管理员QQ为{Utils.Setting.AdminQQ}");
                        }
                    }
                },
                ["rua".ToUpper()] = new CommandContain
                {
                    explain = "重载所有lua虚拟机",
                    run = (s) => Utils.ReloadLua()
                },
                ["uua".ToUpper()] = new CommandContain
                {
                    explain = "尝试更新脚本并重载虚拟机",
                    run = (s) =>
                    {
                        Utils.CheckLuaUpdate();
                        Utils.ReloadLua();
                        Utils.CheckUpdate();
                    }
                },
                ["xml".ToUpper()] = new CommandContain
                {
                    explain = "清除XML数据缓存，重新读取XML文件",
                    run = (s) => LuaEnv.XmlApi.Clear()
                },
                ["color".ToUpper()] = new CommandContain
                {
                    explain = "切换终端颜色显示。如果显示不正常，可以更改此选项关闭颜色",
                    run = (s) =>
                    {
                        Utils.Setting.Colorful = !Utils.Setting.Colorful;
                        Log.Info($"命令", $"彩色终端状态：{Utils.Setting.Colorful}");
                    }
                },
                ["mqtt".ToUpper()] = new CommandContain
                {
                    explain = "查看/更改mqtt配置",
                    run = (s) =>
                    {
                        if(s.IndexOf(" ") > 0)
                        {
                            var t = s.Split(' ');
                            if(t.Length >= 3)
                            {
                                switch (t[1].ToUpper())
                                {
                                    case "ENABLE":
                                        Utils.Setting.MqttEnable = t[2].ToUpper() == "TRUE";
                                        break;
                                    case "HOST":
                                        Utils.Setting.MqttBroker = t[2];
                                        break;
                                    case "PORT":
                                        int p;
                                        var r = int.TryParse(t[2], out p);
                                        if(r)
                                            Utils.Setting.MqttPort = p;
                                        break;
                                    case "USER":
                                        Utils.Setting.MqttUser = t[2];
                                        break;
                                    case "PASSWORD":
                                        Utils.Setting.MqttPassword = t[2];
                                        break;
                                    case "TLS":
                                        Utils.Setting.MqttTLS = t[2].ToUpper() == "TRUE";
                                        break;
                                    case "KEEPALIVE":
                                        int pa;
                                        var ra = int.TryParse(t[2], out pa);
                                        if (ra)
                                            Utils.Setting.KeepAlive = pa;
                                        break;
                                    default:
                                        Log.Info($"MQTT", "命令格式不正确，输入mqtt命令查询命令用法");
                                        break;
                                }
                            }
                            else
                            {
                                Log.Info($"MQTT", "命令格式不正确，输入mqtt命令查询命令用法");
                            }
                        }
                        else
                        {
                            Log.Info($"MQTT", @$"↓
当前MQTT配置信息如下：
mqtt功能启用状态：{Utils.Setting.MqttEnable}
服务器地址：{Utils.Setting.MqttBroker}
服务器端口：{Utils.Setting.MqttPort}
用户名：{Utils.Setting.MqttUser}
密码：{Utils.Setting.MqttPassword}
启用tls：{Utils.Setting.MqttTLS}
client ID：{Utils.Setting.ClientID}
心跳时长：{Utils.Setting.KeepAlive}秒

更改配置信息：mqtt <enable,host,port,user,password,tls,clientid,keepalive> <value>
注意：更改完配置后，手动开关mqtt服务器才会生效
");
                        }
                    }
                },
                ["tcp".ToUpper()] = new CommandContain
                {
                    explain = "查看/更改tcp配置",
                    run = (s) =>
                    {
                        if (s.IndexOf(" ") > 0)
                        {
                            var t = s.Split(' ');
                            if (t.Length >= 3)
                            {
                                switch (t[1].ToUpper())
                                {
                                    case "ENABLE":
                                        Utils.Setting.TcpServerEnable = t[2].ToUpper() == "TRUE";
                                        break;
                                    case "PORT":
                                        int p;
                                        var r = int.TryParse(t[2], out p);
                                        if (r)
                                            Utils.Setting.TcpServerPort = p;
                                        break;
                                    default:
                                        Log.Info($"TCP", "命令格式不正确，输入tcp命令查询命令用法");
                                        break;
                                }
                            }
                            else
                            {
                                Log.Info($"TCP", "命令格式不正确，输入tcp命令查询命令用法");
                            }
                        }
                        else
                        {
                            Log.Info($"TCP", @$"↓
当前TCP服务端配置信息如下：
启用状态：{Utils.Setting.TcpServerEnable}
端口：{Utils.Setting.TcpServerPort}

更改配置信息：mqtt <enable,port> <value>
注意：更改完配置后，手动开关TCP服务器才会生效
");
                        }
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
                ["stop".ToUpper()] = new CommandContain
                {
                    explain = "退出软件",
                    run = (s) =>
                    {
                        Log.Warn("退出软件", "bye~");
                        Environment.Exit(0);
                    }
                },
            };
            while (true)
            {
                var command = Console.ReadLine();
                var cp = command.IndexOf(" ") >= 0 ? command.Substring(0, command.IndexOf(" ")) : command;
                if (commandList.ContainsKey(cp.ToUpper()))
                {
                    try
                    {
                        commandList[cp.ToUpper()].run(command);
                        Log.Info($"命令", $"{command}执行完毕");
                    }
                    catch (Exception e)
                    {
                        Log.Warn("命令", $"{command}执行报错，错误信息：\n{e.Message}");
                    }
                }
                else if (command.ToUpper() == "HELP" || command == "?" || command == "？")
                {
                    Log.Info("命令", "可用命令列表：");
                    foreach (var item in commandList)
                    {
                        Log.Info($"{item.Key.ToLower()}", $"\t{item.Value.explain}");
                    }
                }
                else
                {
                    Log.Info("命令", $"未知命令，使用?获取命令帮助");
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
