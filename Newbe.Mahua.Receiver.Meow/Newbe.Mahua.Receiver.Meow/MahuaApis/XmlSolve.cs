using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Newbe.Mahua.Receiver.Meow.MahuaApis
{
    class XmlSolve
    {
        public static string path = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "data/";//AppDomain.CurrentDomain.SetupInformation.ApplicationBase

        public static string ReplayGroupStatic(string fromGroup, string msg)
        {
            string replay_ok = replay_get(fromGroup, msg);
            string replay_common = replay_get("common", msg);

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

        public static string replay_get(string group, string msg)
        {
            dircheck(group);
            XElement root = XElement.Load(path + group + ".xml");
            string[] replay_str = new string[50];
            int count = 0;
            Random ran = new Random(System.DateTime.Now.Millisecond);
            int RandKey;
            string ansall = "";
            foreach (XElement mm in root.Elements("msginfo"))
            {
                if (msg.IndexOf(mm.Element("msg").Value) != -1 && count < 50)
                {
                    replay_str[count] = mm.Element("ans").Value;
                    count++;
                }
            }
            if (count != 0)
            {
                RandKey = ran.Next(0, count);
                ansall = replay_str[RandKey];
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
            foreach (XElement mm in root.Elements("msginfo"))
            {
                if (mm.Element("ans").Value.IndexOf(msg) != -1 )
                {
                    return true;
                }
            }
            return false;
        }

        public static string qq_get(string msg)
        {
            string group = "bind_qq";
            dircheck(group);
            XElement root = XElement.Load(path + group + ".xml");
            string ansall = "0";
            foreach (XElement mm in root.Elements("msginfo"))
            {
                if (msg == mm.Element("ans").Value)
                {
                    ansall = mm.Element("msg").Value;
                    break;
                }
            }

            return ansall;
        }

        public static string qq_get_unregister(string msg)
        {
            string group = "bind_qq_wait";
            dircheck(group);
            XElement root = XElement.Load(path + group + ".xml");
            string ansall = "0";
            foreach (XElement mm in root.Elements("msginfo"))
            {
                if (msg == mm.Element("ans").Value)
                {
                    ansall = mm.Element("msg").Value;
                    break;
                }
            }

            return ansall;
        }

        public static string xml_get(string group, string msg)
        {
            dircheck(group);
            XElement root = XElement.Load(path + group + ".xml");
            string ansall = "";
            foreach (XElement mm in root.Elements("msginfo"))
            {
                if (msg == mm.Element("msg").Value)
                {
                    ansall = mm.Element("ans").Value;
                    break;
                }
            }

            return ansall;
        }

        public static string xml_dic_get(string num)
        {
            XElement root = XElement.Load(path + "dic.xml");
            string ans = "";
            foreach (XElement mm in root.Elements("msginfo"))
            {
                if (num == mm.Element("sum").Value)
                {
                    ans = mm.Element("word").Value + "\r\n" + mm.Element("translate").Value;
                    break;
                }
            }
            return ans;
        }

        public static string list_get(string group, string msg)
        {
            dircheck(group);
            XElement root = XElement.Load(path + group + ".xml");
            int count = 0;
            string ansall = "";
            foreach (XElement mm in root.Elements("msginfo"))
            {
                if (msg == mm.Element("msg").Value)
                {
                    ansall = ansall + mm.Element("ans").Value + "\r\n";
                    count++;
                }
            }
            ansall = ansall + "一共有" + count.ToString() + "条回复";
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
