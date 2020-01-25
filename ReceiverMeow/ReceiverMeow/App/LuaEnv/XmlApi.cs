using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Native.Csharp.App.LuaEnv
{
    class XmlApi
    {
        public const string rootNode = "Categories";
        public const string keyNode = "msginfo";
        public const string keyName = "msg";
        public const string valueName = "ans";

        public static string path = Common.AppData.CQApi.AppDirectory + "xml/";

        //存放xml列表数据在内存，加快读取速度
        private static ConcurrentDictionary<string, XElement> xmlList = 
            new ConcurrentDictionary<string, XElement>();

        private static readonly object objLock = new object();

        /// <summary>
        /// 随机获取一条结果
        /// </summary>
        /// <param name="group"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static string RandomGet(string group, string msg)
        {
            XElement root = GetXml(group);
            Random ran = new Random(System.DateTime.Now.Millisecond);
            int RandKey;
            string ansall = "";
            var element = from ee in root.Elements()
                          where msg.IndexOf(ee.Element(keyName).Value) != -1
                          select ee;
            XElement[] result = element.ToArray();
            if (result.Count() > 0)
            {
                RandKey = ran.Next(0, result.Count());
                ansall = result[RandKey].Element(valueName).Value;
            }
            return ansall;
        }

        /// <summary>
        /// 设置所有符合名字的值为指定结果
        /// </summary>
        /// <param name="group"></param>
        /// <param name="msg"></param>
        /// <param name="str"></param>
        public static void Set(string group, string msg, string str)
        {
            Delete(group, msg);
            Add(group, msg, str);
        }

        /// <summary>
        /// 获取某项目的第一个值
        /// </summary>
        /// <param name="group"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static string Get(string group, string msg)
        {
            XElement root = GetXml(group);
            string ansall = "";
            var element = from ee in root.Elements()
                          where ee.Element(keyName).Value == msg
                          select ee;
            if (element.Count() > 0)
                ansall = element.First().Element(valueName).Value;
            return ansall;
        }

        /// <summary>
        /// 反查某项目的第一个值
        /// </summary>
        /// <param name="group"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static string Row(string group, string msg)
        {
            XElement root = GetXml(group);
            string ansall = "";
            var element = from ee in root.Elements()
                          where ee.Element(valueName).Value == msg
                          select ee;
            if (element.Count() > 0)
                ansall = element.First().Element(keyName).Value;
            return ansall;
        }

        /// <summary>
        /// 获取条目列表
        /// </summary>
        /// <param name="group"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static List<string> GetList(string group, string msg)
        {
            XElement root = GetXml(group);
            List<string> ansall = new List<string>();
            var element = from ee in root.Elements()
                          where ee.Element(keyName).Value == msg
                          select ee;
            XElement[] result = element.ToArray();
            foreach (XElement mm in result)
                ansall.Add(mm.Element(valueName).Value);
            return ansall;
        }

        /// <summary>
        /// 删除所有匹配的值
        /// </summary>
        /// <param name="group"></param>
        /// <param name="msg"></param>
        public static void Delete(string group, string msg)
        {
            XElement root = GetXml(group);
            var element = from ee in root.Elements()
                          where (string)ee.Element(keyName) == msg
                          select ee;
            if (element.Count() > 0)
                element.Remove();
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            lock (objLock)
                root.Save(path + group + ".xml");
        }

        /// <summary>
        /// 删除键值完全匹配的第一条数据
        /// </summary>
        /// <param name="group"></param>
        /// <param name="msg"></param>
        /// <param name="ans"></param>
        public static void DeleteOne(string group, string msg, string ans)
        {
            XElement root = GetXml(group);
            var element = from ee in root.Elements()
                          where (string)ee.Element(keyName) == msg && (string)ee.Element(valueName) == ans
                          select ee;
            if (element.Count() > 0)
                element.First().Remove();
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            lock (objLock)
                root.Save(path + group + ".xml");
        }

        /// <summary>
        /// 添加一条记录
        /// </summary>
        /// <param name="group"></param>
        /// <param name="msg"></param>
        /// <param name="ans"></param>
        public static void Add(string group, string msg, string ans)
        {
            if (msg.IndexOf("\r\n") < 0 & msg != "")
            {
                XElement root = GetXml(group);
                XElement read = root.Element(keyNode);
                if (read == null)
                {
                    root.Add(new XElement(keyNode,
                      new XElement(keyName, msg),
                      new XElement(valueName, ans)
                      ));
                }
                else
                {
                    read.AddBeforeSelf(new XElement(keyNode,
                      new XElement(keyName, msg),
                      new XElement(valueName, ans)
                      ));
                }
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                lock (objLock)
                    root.Save(path + group + ".xml");
            }
        }


        /// <summary>
        /// 获取xml数据对象
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        private static XElement GetXml(string group)
        {
            if (!xmlList.ContainsKey(group))
            {
                if (!File.Exists(path + group + ".xml"))
                {
                    xmlList[group] = new XElement(rootNode,
                            new XElement(keyNode,
                                new XElement(keyName, "初始问题"),
                                new XElement(valueName, "初始回答")
                                )
                           );
                }
                else
                {
                    try
                    {
                        xmlList[group] = XElement.Load(path + group + ".xml");
                    }
                    catch(Exception e)
                    {
                        Common.AppData.CQLog.Fatal("Lua插件",$"xml文件加载错误({group})！{e.Message}");
                        return null;
                    }
                }
            }
            return xmlList[group];
        }
    }
}
