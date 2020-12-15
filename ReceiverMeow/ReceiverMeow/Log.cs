using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReceiverMeow
{
    class Log
    {
        private static string GetTime() => DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss:ffff]");
        /// <summary>
        /// 显示VT100格式控制编码
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        private static string v(int n) => Utils.Setting.Colorful ? $"\x1B[{n}m" : $"";

        /// <summary>
        /// debug日志，release模式下不显示
        /// </summary>
        /// <param name="m">模块名</param>
        /// <param name="s">日志信息</param>
        public static void Debug(string m, string s)
        {
#if DEBUG
            Console.WriteLine($"{v(0)}{GetTime()}{v(30)}{v(47)}[调试]{v(0)}[{m}]{s}{v(0)}");
#endif
        }

        /// <summary>
        /// 显示信息
        /// </summary>
        /// <param name="m">模块名</param>
        /// <param name="s">日志信息</param>
        public static void Info(string m, string s) =>
            Console.WriteLine($"{v(0)}{GetTime()}{v(42)}{v(37)}[信息]{v(0)}{v(32)}{v(1)}[{m}]{s}{v(0)}");

        /// <summary>
        /// 显示警告
        /// </summary>
        /// <param name="m">模块名</param>
        /// <param name="s">日志信息</param>
        public static void Warn(string m, string s) =>
            Console.WriteLine($"{v(0)}{GetTime()}{v(43)}{v(37)}[警告]{v(0)}{v(33)}{v(1)}[{m}]{s}{v(0)}");

        /// <summary>
        /// 显示错误，并退出软件
        /// 一般只用在开头启动时，软件运行中别用
        /// </summary>
        /// <param name="m">模块名</param>
        /// <param name="s">日志信息</param>
        public static void Error(string m, string s)
        {
            Console.WriteLine($"{v(0)}{GetTime()}{v(41)}{v(37)}[错误]{v(0)}{v(31)}{v(1)}[{m}]{s}{v(0)}");
            Environment.Exit(-1);
        }
    }
}
