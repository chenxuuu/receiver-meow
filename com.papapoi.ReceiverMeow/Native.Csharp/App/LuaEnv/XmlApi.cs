using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;

namespace Native.Csharp.App.LuaEnv
{
    class XmlApi
    {
        public static string path = Common.AppDirectory + "xml/";

        public static void set(string group, string msg,string str)
        {
            del(group, msg);
            insert(group, msg, str);
        }

        /// <summary>
        /// 随机获取一条结果
        /// </summary>
        /// <param name="group"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static string replay_get(string group, string msg)
        {
            dircheck(group);
            XElement root = XElement.Load(path + group + ".xml");
            Random ran = new Random(System.DateTime.Now.Millisecond);
            int RandKey;
            string ansall = "";
            var element = from ee in root.Elements()
                          where msg.IndexOf(ee.Element("msg").Value) != -1
                          select ee;
            XElement[] result = element.ToArray();
            if (result.Count() > 0)
            {
                RandKey = ran.Next(0, result.Count());
                ansall = result[RandKey].Element("ans").Value;
            }
            return ansall;
        }

        public static string xml_get(string group, string msg)
        {
            dircheck(group);
            XElement root = XElement.Load(path + group + ".xml");
            string ansall = "";
            var element = from ee in root.Elements()
                          where ee.Element("msg").Value == msg
                          select ee;
            if (element.Count() > 0)
                ansall = element.First().Element("ans").Value;
            return ansall;
        }

        public static string xml_row(string group, string msg)
        {
            dircheck(group);
            XElement root = XElement.Load(path + group + ".xml");
            string ansall = "";
            var element = from ee in root.Elements()
                          where ee.Element("ans").Value == msg
                          select ee;
            if (element.Count() > 0)
                ansall = element.First().Element("msg").Value;
            return ansall;
        }

        public static string list_get(string group, string msg)
        {
            dircheck(group);
            XElement root = XElement.Load(path + group + ".xml");
            string ansall = "";
            var element = from ee in root.Elements()
                          where ee.Element("msg").Value == msg
                          select ee;
            XElement[] result = element.ToArray();
            foreach (XElement mm in result)
                ansall = ansall + mm.Element("ans").Value + "\r\n";
            ansall = ansall + "一共有" + element.Count() + "条回复";
            return ansall;
        }

        public static void del(string group, string msg)
        {
            dircheck(group);
            string gg = group.ToString();
            XElement root = XElement.Load(path + group + ".xml");

            var element = from ee in root.Elements()
                          where (string)ee.Element("msg") == msg
                          select ee;
            if (element.Count() > 0)
                element.Remove();
            root.Save(path + group + ".xml");
        }

        public static void remove(string group, string msg, string ans)
        {
            dircheck(group);
            string gg = group.ToString();
            XElement root = XElement.Load(path + group + ".xml");

            var element = from ee in root.Elements()
                          where (string)ee.Element("msg") == msg && (string)ee.Element("ans") == ans
                          select ee;
            if (element.Count() > 0)
                element.First().Remove();
            root.Save(path + group + ".xml");
        }


        public static void insert(string group, string msg, string ans)
        {
            if (msg.IndexOf("\r\n") < 0 & msg != "")
            {
                dircheck(group);
                XElement root = XElement.Load(path + group + ".xml");

                XElement read = root.Element("msginfo");

                read.AddBeforeSelf(new XElement("msginfo",
                       new XElement("msg", msg),
                       new XElement("ans", ans)
                       ));

                root.Save(path + group + ".xml");
            }
        }

        public static void dircheck(string group)
        {
            if (!File.Exists(path + group + ".xml"))
            {
                XElement root = new XElement("Categories",
                    new XElement("msginfo",
                        new XElement("msg", "初始问题"),
                        new XElement("ans", "初始回答")
                        )
                   );
                root.Save(path + group + ".xml");
            }
        }
    }
}
