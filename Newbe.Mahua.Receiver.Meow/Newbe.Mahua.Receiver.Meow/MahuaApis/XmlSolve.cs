using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;

namespace Newbe.Mahua.Receiver.Meow.MahuaApis
{
    class XmlSolve
    {
        public static string path = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "data/";//AppDomain.CurrentDomain.SetupInformation.ApplicationBase

        public static string ReplayGroupStatic(string fromGroup, string msg, string fromqq = "")
        {
            string replay_ok = replay_get(fromGroup, msg, fromqq);
            string replay_common = replay_get("common", msg, fromqq);

            if (replay_ok != "")
            {
                if (replay_common != "")
                {
                    Random ran = new Random(System.DateTime.Now.Millisecond);
                    int RandKey = ran.Next(0, 2);
                    if (RandKey == 0) { return replay_ok; } else { return replay_common; }
                }
                else
                {
                    return replay_ok;
                }
            }
            else if (replay_common != "")
            {
                return replay_common;
            }
            return "";
        }

        public static string replay_get(string group, string msg, string fromqq = "")
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
            if(ansall.IndexOf("[lua]") == 0)
            {
                int len = ansall.IndexOf(".");
                msg = HttpUtility.HtmlDecode(msg.Replace("]]", ""));
                if(msg.Substring(msg.Length-1) == "]")
                    msg += " ";
                ansall = Tools.RunLua(HttpUtility.HtmlDecode(ansall.Substring(len + 1)),
                        "message=[[" + msg + "]]\r\n" +
                        "fromqq = \"" + fromqq + "\"\r\n" +
                        "fromgroup = \"" + group + "\"");
            }
            return ansall;
        }

        /// <summary>
        /// 检查是否有该词条的回复
        /// </summary>
        /// <param name="group"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static bool IsAnswer(string group, string msg)
        {
            dircheck(group);
            XElement root = XElement.Load(path + group + ".xml");
            var element = from ee in root.Elements()
                          where ee.Element("ans").Value.IndexOf(msg) != -1
                          select ee;
            return element.Count() > 0;
        }

        /// <summary>
        /// 是否已禁止某词条的回复
        /// </summary>
        /// <param name="group"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static bool IsBaned(string group, string msg)
        {
            dircheck(group);
            XElement root = XElement.Load(path + group + ".xml");
            var element = from ee in root.Elements()
                          where ee.Element("ans").Value == "[ban]" 
                            &&  msg.IndexOf(ee.Element("msg").Value) != -1
                          select ee;
            return element.Count() > 0;
        }

        public static string xml_get(string group, string msg)
        {
            dircheck(group);
            XElement root = XElement.Load(path + group + ".xml");
            string ansall = "";
            var element = from ee in root.Elements()
                          where ee.Element("msg").Value == msg
                          select ee;
            if(element.Count() > 0)
            {
                ansall = element.First().Element("ans").Value;
            }
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
            {
                if (mm.Element("ans").Value.IndexOf("[lua]") == 0)
                {
                    int len = mm.Element("ans").Value.IndexOf(".");
                    ansall += Tools.At(mm.Element("ans").Value.Substring(5, len - 5)) + "的lua脚本\r\n";
                }
                else
                {
                    ansall = ansall + mm.Element("ans").Value + "\r\n";
                }
            }
            ansall = ansall + "一共有" + element.Count() + "条回复";
            return ansall;
        }

        public static string lua_list_get(string group)
        {
            dircheck(group);
            XElement root = XElement.Load(path + group + ".xml");
            string ansall = "";
            var element = from ee in root.Elements()
                          where ee.Element("ans").Value.IndexOf("[lua]") == 0
                          select ee;
            XElement[] result = element.ToArray();
            foreach (XElement mm in result)
            {
                int len = mm.Element("ans").Value.IndexOf(".");
                ansall += mm.Element("msg").Value + "：" +
                    Tools.At(mm.Element("ans").Value.Substring(5, len - 5)) + "的lua脚本\r\n";
            }
            ansall = ansall + "一共有" + element.Count() + "个脚本";
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
            {
                //element.First().Remove();
                element.Remove();
            }
            root.Save(path + group + ".xml");
        }

        public static void luadel(string group, string msg)
        {
            dircheck(group);
            string gg = group.ToString();
            XElement root = XElement.Load(path + group + ".xml");

            var element = from ee in root.Elements()
                          where (string)ee.Element("msg") == msg && ((string)ee.Element("ans")).IndexOf("[lua]") == 0
                          select ee;
            if (element.Count() > 0)
            {
                //element.First().Remove();
                element.Remove();
            }
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
            {
                element.First().Remove();
            }
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
                       //new XElement("group", group),
                       new XElement("msg", msg),
                       new XElement("ans", ans)
                       ));

                root.Save(path + group + ".xml");
            }
        }

        public static void createxml(string group)
        {
            XElement root = new XElement("Categories",
                new XElement("msginfo",
                    //new XElement("group", 123),
                    new XElement("msg", "初始问题"),
                    new XElement("ans", "初始回答")
                    )
               );
            root.Save(path + group + ".xml");
        }

        public static void dircheck(string group)
        {
            if (File.Exists(path + group + ".xml"))
            {
                //MessageBox.Show("存在文件");
                //File.Delete(dddd);//删除该文件
            }
            else
            {
                //MessageBox.Show("不存在文件");
                createxml(group);//创建该文件，如果路径文件夹不存在，则报错。
            }
        }

        public static int AdminCheck(string fromQQ)
        {
            dircheck("admin_list");

            XElement root = XElement.Load(path + "admin_list.xml");
            int count = 0;
            foreach (XElement mm in root.Elements("msginfo"))
            {
                if (mm.Element("ans").Value == fromQQ.ToString())
                {
                    count = 1;
                }
            }
            return count;
        }
    }
}
