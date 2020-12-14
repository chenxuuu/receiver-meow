using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReceiverMeow.GoHttp
{
    class CQ
    {
        /// <summary>
        /// 转义cq码
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string Encode(string s) => 
            s.Replace("&", "&amp;").Replace("[", "&#91;").Replace("]", "&#93;");

        /// <summary>
        /// 去转义cq码
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string Decode(string s) =>
            s.Replace("&#91;", "[").Replace("&#93;", "]").Replace("&amp;", "&");
    }
}
