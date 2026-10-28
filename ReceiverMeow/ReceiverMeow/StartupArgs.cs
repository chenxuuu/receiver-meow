using System;
using System.Collections.Generic;

namespace ReceiverMeow
{
    /// <summary>
    /// 启动命令行参数
    /// 用法：receiver-meow.exe [nows] [wsUrl] [wsPort] [httpUrl] [httpPort]
    /// </summary>
    class StartupArgs
    {
        /// <summary>
        /// OneBot websocket地址
        /// </summary>
        public string WsUrl = "127.0.0.1";
        /// <summary>
        /// OneBot websocket端口
        /// </summary>
        public int WsPort = 6700;
        /// <summary>
        /// OneBot http地址
        /// </summary>
        public string HttpUrl = "127.0.0.1";
        /// <summary>
        /// OneBot http端口
        /// </summary>
        public int HttpPort = 5700;
        /// <summary>
        /// 是否不连接websocket（nows参数）
        /// </summary>
        public bool NoWs = false;

        private const string ExeName = "receiver-meow.exe";

        /// <summary>
        /// 查看帮助的参数
        /// </summary>
        private static readonly string[] HelpFlags = { "-H", "--HELP", "-?", "/?", "HELP" };

        /// <summary>
        /// 软件版本号，此处自行获取，避免提前触发Utils的静态初始化（会加载词典等资源）
        /// </summary>
        private static string Version =>
            System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();

        /// <summary>
        /// 解析命令行参数
        /// 参数有误或要求查看帮助时，打印提示后直接结束进程
        /// </summary>
        /// <param name="args">原始命令行参数</param>
        public static StartupArgs Parse(string[] args)
        {
            var result = new StartupArgs();
            var positional = new List<string>();

            foreach (var item in args)
            {
                if (string.IsNullOrWhiteSpace(item))
                    continue;
                var arg = item.Trim();
                if (Array.IndexOf(HelpFlags, arg.ToUpper()) >= 0)
                {
                    PrintUsage();
                    Environment.Exit(0);
                }
                if (arg.ToUpper() == "NOWS")
                    result.NoWs = true;
                else
                    positional.Add(arg);
            }

            //一个都不填，全部使用默认值
            if (positional.Count == 0)
                return result;

            if (positional.Count != 4)
            {
                Fail($"位置参数需要 0 个（全部用默认值）或 4 个（wsUrl wsPort httpUrl httpPort），" +
                     $"当前填了 {positional.Count} 个：{string.Join(" ", positional)}");
            }

            result.WsUrl = CheckUrl(positional[0], "wsUrl", "wsPort");
            result.WsPort = CheckPort(positional[1], "wsPort");
            result.HttpUrl = CheckUrl(positional[2], "httpUrl", "httpPort");
            result.HttpPort = CheckPort(positional[3], "httpPort");

            return result;
        }

        /// <summary>
        /// 校验地址参数
        /// </summary>
        /// <param name="value">填写的值</param>
        /// <param name="name">参数名</param>
        /// <param name="portName">对应的端口参数名</param>
        private static string CheckUrl(string value, string name, string portName)
        {
            if (value.Contains("://"))
            {
                Fail($"{name} 填的是「{value}」，这里只填 IP 或域名，不要带 http:// 、ws:// 这类前缀");
            }
            if (value.Contains(":"))
            {
                Fail($"{name} 填的是「{value}」，地址里不要写端口号，端口要单独写在 {portName} 参数里");
            }
            if (value.Contains("/") || value.Contains("?"))
            {
                Fail($"{name} 填的是「{value}」，这里只填 IP 或域名，不要带路径和参数");
            }
            return value;
        }

        /// <summary>
        /// 校验端口参数
        /// </summary>
        /// <param name="value">填写的值</param>
        /// <param name="name">参数名</param>
        private static int CheckPort(string value, string name)
        {
            if (!int.TryParse(value, out var port))
            {
                Fail($"{name} 必须是数字，当前填的是「{value}」");
            }
            if (port < 1 || port > 65535)
            {
                Fail($"{name} 必须在 1~65535 之间，当前填的是 {port}");
            }
            return port;
        }

        /// <summary>
        /// 打印错误信息与用法说明，然后退出
        /// </summary>
        private static void Fail(string message)
        {
            var old = Console.ForegroundColor;
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("参数错误：" + message);
            Console.ForegroundColor = old;
            Console.WriteLine();
            PrintUsage();
            Environment.Exit(1);
        }

        /// <summary>
        /// 打印用法说明
        /// </summary>
        public static void PrintUsage()
        {
            Console.WriteLine($@"接待喵 {Version} 启动参数说明

用法：
  {ExeName} [nows] [wsUrl] [wsPort] [httpUrl] [httpPort]

位置参数（要么 4 个都不填，要么 4 个一起填，按顺序对应）：
  wsUrl     OneBot 的 WebSocket 地址，只填 IP 或域名              默认 127.0.0.1
  wsPort    OneBot 的 WebSocket 端口，1~65535 的数字             默认 6700
  httpUrl   OneBot 的 HTTP 地址，只填 IP 或域名                   默认 127.0.0.1
  httpPort  OneBot 的 HTTP 端口，1~65535 的数字                  默认 5700

开关（可放任意位置）：
  nows        不连接 WebSocket，只走 HTTP 上报
  -h, --help  打印本说明并退出

示例：
  {ExeName}
      用默认的 127.0.0.1:6700(ws) / 127.0.0.1:5700(http) 启动

  {ExeName} 127.0.0.1 6700 127.0.0.1 5700
      显式指定 4 个参数，顺序为 wsUrl wsPort httpUrl httpPort

  {ExeName} 192.168.1.20 6700 192.168.1.20 5700
      框架（如 go-cqhttp）跑在另一台机器上时，填它的 IP

  {ExeName} nows 127.0.0.1 6700 127.0.0.1 5700
      不连 WebSocket，只用 HTTP

  {ExeName} -h
      查看本说明

注意：
  · 地址不要带 http:// 、ws:// 前缀，端口也不要写在地址里
  · 启动后会打印实际使用的连接配置，可对照检查是否填对
  · 启动完成后输入 ? 可查看软件运行中的命令");
        }
    }
}
