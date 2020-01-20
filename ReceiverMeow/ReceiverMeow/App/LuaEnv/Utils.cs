using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Native.Csharp.App.LuaEnv
{
    class Utils
    {
        /// <summary>
        /// 转时间戳
        /// https://blog.csdn.net/qq_24025219/article/details/100146913
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static int DateTimeToInt(TimeSpan ts2)
        {
            int date = 0;
            TimeSpan ts3 = new TimeSpan(DateTime.Parse("1970-01-01").Ticks);
            TimeSpan ts_1 = ts2.Subtract(ts3).Duration();
            int NowMinu = (int)ts_1.TotalSeconds;
            date = NowMinu;
            return date;
        }
    }
}
