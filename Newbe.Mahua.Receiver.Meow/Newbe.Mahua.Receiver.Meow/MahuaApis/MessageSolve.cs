using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Newbe.Mahua.Receiver.Meow.MahuaApis
{
    class MessageSolve
    {
        public static string GetReplay(string fromqq,string msg,string fromgroup = "common")
        {
            string result = "";

            result = XmlSolve.ReplayGroupStatic(fromgroup, msg);
            return result;
        }
    }
}
