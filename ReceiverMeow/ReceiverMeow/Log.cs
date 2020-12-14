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
        public static void Debug(string m, string s)
        {
#if DEBUG
            Console.WriteLine($"\x1B[0m{GetTime()}\x1B[30m\x1B[47m[调试]\x1B[0m[{m}]{s}\x1B[0m");
#endif
        }
        public static void Info(string m, string s) =>
            Console.WriteLine($"\x1B[0m{GetTime()}\x1B[42m\x1B[37m[信息]\x1B[0m\x1B[32m\x1B[1m[{m}]{s}\x1B[0m");
        public static void Warn(string m, string s) =>
            Console.WriteLine($"\x1B[0m{GetTime()}\x1B[43m\x1B[37m[警告]\x1B[0m\x1B[33m\x1B[1m[{m}]{s}\x1B[0m");
        public static void Error(string m, string s)
        {
            Console.WriteLine($"\x1B[0m{GetTime()}\x1B[41m\x1B[37m[错误]\x1B[0m\x1B[31m\x1B[1m[{m}]{s}\x1B[0m");
            Environment.Exit(-1);
        }
    }
}
