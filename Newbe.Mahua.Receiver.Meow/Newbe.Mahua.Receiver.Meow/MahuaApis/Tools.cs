using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Newbe.Mahua.Receiver.Meow.MahuaApis
{
    class Tools
    {
        //机器人qq号
        public static string qqNumber = XmlSolve.xml_get("settings","robot");
        //主人qq号
        public static string adminNumber = XmlSolve.xml_get("settings", "admin");
        //管理群的号码
        public static string mainGroupNumber = XmlSolve.xml_get("settings", "mainGroup");
        //开启专属功能？（一般不开）
        public static string special = XmlSolve.xml_get("settings", "special");
        //whatanime的api
        public static string whatanimeApi = XmlSolve.xml_get("settings", "whatanimeApi");
        //http代理的网址
        public static string proxyUrl = XmlSolve.xml_get("settings", "proxy");
        public static int messageCount = 0;
        public static string now = DateTime.Now.ToString();
        /// <summary>
        /// 判断是否发消息，消息速率限制函数
        /// </summary>
        /// <returns></returns>
        public static bool MessageControl(int count)
        {
            if (now == DateTime.Now.ToString())
            {
                if (messageCount < count)
                {
                    messageCount++;
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                now = DateTime.Now.ToString();
                messageCount = 0;
                return false;
            }
        }




        /// <summary>
        /// 获取at某人的整合格式
        /// </summary>
        /// <param name="qq"></param>
        /// <returns></returns>
        public static string At(string qq)
        {
            return "[CQ:at,qq=" + qq + "]";
        }

        /// <summary>
        /// 获取复读语句
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="group"></param>
        /// <returns></returns>
        public static string GetRepeatString(string msg, string group)
        {
            int sum = 0;
            try
            {
                sum = int.Parse(XmlSolve.xml_get("repeat_settings", group));
                if (GetRandomNumber(0, 100) < sum)
                {
                    if (sum <= 100)
                        return msg;
                    else
                    {
                        string result = "";
                        for (int i = 0; i < sum / 100; i++)
                            result += msg;
                        if (GetRandomNumber(0, 100) < sum % 100)
                            return result + msg;
                        else
                            return result;
                    }
                }
                else
                    return "";
            }
            catch
            {
                return "";
            }
        }



        /// <summary>
        /// 设置复读概率
        /// </summary>
        /// <param name="input"></param>
        /// <param name="group"></param>
        /// <returns></returns>
        public static string SetRepeat(string input, string group)
        {
            int sum = 0;
            try
            {
                sum = int.Parse(input);
                if (sum > 1000)
                    sum = 1000;
                XmlSolve.del("repeat_settings", group);
                XmlSolve.insert("repeat_settings", group, sum.ToString());
                return "设置完成，复读概率更改为" + sum + "%";
            }
            catch
            {
                return "设置失败，请检查参数";
            }
        }


        /// <summary>
        /// 获取随机数
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static int GetRandomNumber(int min, int max)
        {
            Random ran = new Random(System.DateTime.Now.Millisecond);
            return ran.Next(min, max);
        }

        /// <summary>
        /// 简化正则的操作
        /// </summary>
        /// <param name="str"></param>
        /// <param name="regstr"></param>
        /// <returns></returns>
        public static MatchCollection Reg_solve(string str, string regstr)
        {
            Regex reg = new Regex(regstr, RegexOptions.IgnoreCase);
            return reg.Matches(str);
        }

        /// <summary>
        /// 直接获取正则表达式的最后一组匹配值
        /// </summary>
        /// <param name="str"></param>
        /// <param name="regstr"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string Reg_get(string str, string regstr, string name)
        {
            string result = "";
            MatchCollection matchs = Reg_solve(str, regstr);
            foreach (Match item in matchs)
            {
                if (item.Success)
                {
                    result = item.Groups[name].Value;
                }
            }
            return result;
        }

        /// <summary>
        /// 直接获取正则表达式的所有匹配值
        /// </summary>
        /// <param name="str"></param>
        /// <param name="regstr"></param>
        /// <param name="name"></param>
        /// <param name="name">连接符</param>
        /// <returns></returns>
        public static string Reg_get_all(string str, string regstr, string name, string connect)
        {
            string result = "";
            MatchCollection matchs = Reg_solve(str, regstr);
            foreach (Match item in matchs)
            {
                if (item.Success)
                {
                    result += connect + item.Groups[name].Value;
                }
            }
            if (result != "")
                return result.Substring(connect.Length);
            else
                return result;
        }

        /// <summary>
        /// 获取字符串中的数字
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns>数字</returns>
        public static string GetNumber(string str)
        {
            string result = "";
            if (str != null && str != string.Empty)
            {
                // 正则表达式剔除非数字字符（不包含小数点.）
                str = Regex.Replace(str, @"[^\d.\d]", "");
                // 如果是数字，则转换为decimal类型
                if (Regex.IsMatch(str, @"^[+-]?\d*[.]?\d*$"))
                {
                    result = str;
                }
            }
            return result;
        }



        /// <summary>
        /// 获取CMD执行结果
        /// </summary>
        /// <param name="command">命令</param>
        /// <returns></returns>
        public static string execCMD(string command)
        {
            System.Diagnostics.Process pro = new System.Diagnostics.Process();
            pro.StartInfo.FileName = "cmd.exe";
            pro.StartInfo.UseShellExecute = false;
            pro.StartInfo.RedirectStandardError = true;
            pro.StartInfo.RedirectStandardInput = true;
            pro.StartInfo.RedirectStandardOutput = true;
            pro.StartInfo.CreateNoWindow = true;
            //pro.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            pro.Start();
            pro.StandardInput.WriteLine(command);
            pro.StandardInput.WriteLine("exit");
            pro.StandardInput.AutoFlush = true;
            //获取cmd窗口的输出信息
            string output = pro.StandardOutput.ReadToEnd();
            pro.WaitForExit();//等待程序执行完退出进程
            pro.Close();
            output = output.Substring(output.IndexOf(command));
            return output.Substring(0, output.Length - 1);
        }


        /// <summary>
        /// GET 请求与获取结果
        /// </summary>
        public static string HttpGet(string Url, string postDataStr = "", int timeout = 5000,
            string cookie = "")
        {
            try
            {
                //请求前设置一下使用的安全协议类型 System.Net
                if (Url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
                {
                    ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback((object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors) =>
                    {
                        return true; //总是接受
                    });
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                }
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url + (postDataStr == "" ? "" : "?") + postDataStr);
                if(proxyUrl!="")
                    request.Proxy = new WebProxy(proxyUrl);
                request.Method = "GET";
                request.ContentType = "text/html;charset=UTF-8";
                request.Timeout = timeout;
                request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/71.0.3578.98 Safari/537.36 Vivaldi/2.2.1388.37";
                if(cookie!="")
                    request.Headers.Add("cookie", cookie);

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                string encoding = response.ContentEncoding;
                if (encoding == null || encoding.Length < 1)
                {
                    encoding = "UTF-8"; //默认编码
                }
                Stream myResponseStream = response.GetResponseStream();
                StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding(encoding));

                string retString = myStreamReader.ReadToEnd();
                myStreamReader.Close();
                myResponseStream.Close();

                return retString;
            }
            catch { }
            return "";
        }

        /// <summary>
        /// POST请求与获取结果
        /// </summary>
        public static string HttpPost(string Url, string postDataStr, int timeout = 5000,
            string cookie = "")
        {
            try
            {
                //请求前设置一下使用的安全协议类型 System.Net
                if (Url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
                {
                    ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback((object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors) =>
                    {
                        return true; //总是接受
                    });
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                }
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
                if (proxyUrl != "")
                    request.Proxy = new WebProxy(proxyUrl);
                request.Method = "POST";
                request.Timeout = timeout;
                request.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
                request.ContentLength = postDataStr.Length;
                request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/71.0.3578.98 Safari/537.36 Vivaldi/2.2.1388.37";
                if (cookie != "")
                    request.Headers.Add("cookie", cookie);

                StreamWriter writer = new StreamWriter(request.GetRequestStream(), Encoding.ASCII);
                writer.Write(postDataStr);
                writer.Flush();
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                string encoding = response.ContentEncoding;
                if (encoding == null || encoding.Length < 1)
                {
                    encoding = "UTF-8"; //默认编码
                }
                StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding(encoding));
                string retString = reader.ReadToEnd();
                return retString;
            }
            catch (Exception e)
            { }
            return "";
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="Url">网址</param>
        /// <param name="fileName">文件名</param>
        /// <param name="timeout">超时时间</param>
        /// <returns></returns>
        public static bool FileDownload(string Url, string fileName, int timeout = 5000, bool change = true)
        {
            try
            {
                //请求前设置一下使用的安全协议类型 System.Net
                if (Url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
                {
                    ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback((object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors) =>
                    {
                        return true; //总是接受
                    });
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                }
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
                if (proxyUrl != "")
                    request.Proxy = new WebProxy(proxyUrl);
                request.ContentType = "text/html;charset=UTF-8";
                request.Timeout = timeout;
                request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/71.0.3578.98 Safari/537.36 Vivaldi/2.2.1388.37";

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                bool result = false;
                if (!response.ContentType.ToLower().StartsWith("text/") &&
                    response.ContentLength < 1024*1024*20)
                {
                    if (change)
                        result = SaveBinaryFile(response, AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "data/image/download/" + fileName + "old");
                    else
                        result = SaveBinaryFile(response, AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "data/image/download/" + fileName);
                }

                //防和谐处理
                if(result && change)
                {
                    FileStream fs = File.OpenRead(AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "data/image/download/" + fileName + "old"); //OpenRead
                    int filelength = 0;
                    filelength = (int)fs.Length; //获得文件长度
                    Byte[] image = new Byte[filelength]; //建立一个字节数组
                    fs.Read(image, 0, filelength); //按字节流读取
                    System.Drawing.Image r = System.Drawing.Image.FromStream(fs);
                    fs.Close();
                    Bitmap bit = new Bitmap(r);//图片对象
                    bit.SetPixel(1, 1, bit.GetPixel(1, 1));
                    bit.Save(AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "data/image/download/" + fileName,ImageFormat.Jpeg);
                }

                return result;
            }
            catch { }
            return false;
        }


        /// <summary>
        /// Save a binary file to disk.
        /// </summary>
        /// <param name="response">The response used to save the file</param>
        public static bool SaveBinaryFile(WebResponse response, string FileName)
        {
            bool Value = true;
            byte[] buffer = new byte[1024];

            try
            {
                if (File.Exists(FileName))
                    File.Delete(FileName);
                Stream outStream = System.IO.File.Create(FileName);
                Stream inStream = response.GetResponseStream();
                //inStream.ContentLength
                int l;
                do
                {
                    l = inStream.Read(buffer, 0, buffer.Length);
                    if (l > 0)
                        outStream.Write(buffer, 0, l);
                }
                while (l > 0);

                outStream.Close();
                inStream.Close();
            }
            catch
            {
                Value = false;
            }

            return Value;
        }

        ///<summary>
        ///生成随机字符串
        ///</summary>
        ///<param name="length">目标字符串的长度</param>
        ///<param name="useNum">是否包含数字，1=包含，默认为包含</param>
        ///<param name="useLow">是否包含小写字母，1=包含，默认为包含</param>
        ///<param name="useUpp">是否包含大写字母，1=包含，默认为包含</param>
        ///<param name="useSpe">是否包含特殊字符，1=包含，默认为不包含</param>
        ///<param name="custom">要包含的自定义字符，直接输入要包含的字符列表</param>
        ///<returns>指定长度的随机字符串</returns>
        public static string GetRandomString(int length, bool useNum, bool useLow, bool useUpp, bool useSpe, string custom)
        {
            byte[] b = new byte[4];
            new System.Security.Cryptography.RNGCryptoServiceProvider().GetBytes(b);
            Random r = new Random(BitConverter.ToInt32(b, 0));
            string s = null, str = custom;
            if (useNum == true) { str += "0123456789"; }
            if (useLow == true) { str += "abcdefghijklmnopqrstuvwxyz"; }
            if (useUpp == true) { str += "ABCDEFGHIJKLMNOPQRSTUVWXYZ"; }
            if (useSpe == true) { str += "!\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~"; }
            for (int i = 0; i < length; i++)
            {
                s += str.Substring(r.Next(0, str.Length - 1), 1);
            }
            return s;
        }


        ///  <summary>
        /// 获取指定驱动器的剩余空间总大小(单位为MB)
        ///  </summary>
        ///  <param name="str_HardDiskName">只需输入代表驱动器的字母即可 </param>
        ///  <returns> </returns>
        public static long GetHardDiskFreeSpace(string str_HardDiskName)
        {
            long freeSpace = new long();
            str_HardDiskName = str_HardDiskName + ":\\";
            System.IO.DriveInfo[] drives = System.IO.DriveInfo.GetDrives();
            foreach (System.IO.DriveInfo drive in drives)
            {
                if (drive.Name == str_HardDiskName)
                {
                    freeSpace = drive.TotalFreeSpace / (1024 * 1024);
                }
            }
            return freeSpace;
        }

        /// <summary>
        /// 获取xml存储的数值
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        public static int GetXmlNumber(string xml,string info)
        {
            string resultStr = XmlSolve.xml_get(xml, info);
            int result = 0;
            try
            {
                result = int.Parse(resultStr);
            }
            catch { }
            return result;
        }

        /// <summary>
        /// 获取xml存储的字符串
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        public static string GetXmlString(string xml, string info)
        {
            return XmlSolve.xml_get(xml, info);
        }

        /// <summary>
        /// 将xml值增加指定数值
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="info"></param>
        /// <param name="add"></param>
        public static int AddXmlNumber(string xml,string info,int add)
        {
            string resultStr = XmlSolve.xml_get(xml, info);
            int result = 0;
            try
            {
                result = int.Parse(resultStr);
            }
            catch { }
            result += add;
            XmlSolve.del(xml, info);
            XmlSolve.insert(xml, info, result.ToString());
            return result;
        }


        /// <summary>
        /// 将xml值更改为指定值
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="info"></param>
        /// <param name="add"></param>
        public static void SetXmlString(string xml, string info, string str)
        {
            XmlSolve.del(xml, info);
            XmlSolve.insert(xml, info, str);
        }


        /// <summary>
        /// 获取所有qq群的列表
        /// </summary>
        /// <returns>ArrayList列表</returns>
        public static ArrayList GetGroupList()
        {
            ArrayList list = new ArrayList();
            DirectoryInfo dir = new DirectoryInfo(AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "data/");
            FileSystemInfo[] files = dir.GetFileSystemInfos();
            for (int i = 0; i < files.Length; i++)
            {
                FileInfo file = files[i] as FileInfo;
                //是文件
                if (file != null)
                {
                    string group = file.Name.ToUpper().Replace(".XML", "");
                    string pattern = @"^[0-9]+$";
                    Match match = Regex.Match(group, pattern);
                    if (match.Success && group.Length > 6)
                    {
                        list.Add(group);
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// 统计字符串中某字符出现的次数
        /// </summary>
        /// <param name="str"></param>
        /// <param name="search"></param>
        /// <returns></returns>
        public static int CharNum(string str, string search)
        {
            string[] resultString = Regex.Split(str, search, RegexOptions.IgnoreCase);
            return resultString.Length;
        }

        /// <summary>
        /// 运行lua脚本
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string RunLua(string text,string headRun = "")
        {
            LuaTimeout lua = new LuaTimeout();
            lua.code = text;
            lua.headRun = headRun;
            lua.CallWithTimeout(15000);
            return lua.result;
        }

        /// <summary>
        /// lua读取数据
        /// </summary>
        /// <param name="name"></param>
        /// <param name="qq"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string LuaGetXml(string qq, string name)
        {
            return GetXmlString("luaData", qq + "." + name);
        }

        /// <summary>
        /// lua存储数据
        /// </summary>
        /// <param name="qq"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static void LuaSetXml(string qq, string name, string str)
        {
            SetXmlString("luaData", qq + "." + name, str);
        }
    }


    /// <summary>
    /// 带超时处理的lua运行类
    /// </summary>
    class LuaTimeout
    {
        public string result = "lua脚本运行超时，请检查代码";
        public string code;
        public string headRun = "";

        public void Run(string code)
        {
            using (NLua.Lua lua = new NLua.Lua())
            {
                lua["lua_run_result_var"] = "";
                try
                {
                    lua.RegisterFunction("httpGet_row", null, typeof(Tools).GetMethod("HttpGet"));
                    lua.RegisterFunction("httpPost_row", null, typeof(Tools).GetMethod("HttpPost"));
                    lua.RegisterFunction("setData_row", null, typeof(Tools).GetMethod("LuaSetXml"));
                    lua.RegisterFunction("getData_row", null, typeof(Tools).GetMethod("LuaGetXml"));
                    lua.RegisterFunction("fileDownload", null, typeof(Tools).GetMethod("FileDownload"));
                    lua.RegisterFunction("getImg", null, typeof(LuaApi).GetMethod("GetBitmap"));
                    lua.RegisterFunction("setImgText", null, typeof(LuaApi).GetMethod("PutText"));
                    lua.RegisterFunction("putImgBlock", null, typeof(LuaApi).GetMethod("putBlock"));
                    lua.RegisterFunction("setImgImage", null, typeof(LuaApi).GetMethod("setImage"));
                    lua.RegisterFunction("getImgDir", null, typeof(LuaApi).GetMethod("GetDir"));
                    lua.RegisterFunction("getPath", null, typeof(LuaApi).GetMethod("GetPath"));
                    lua.RegisterFunction("sendGroupMessage", null, typeof(LuaApi).GetMethod("SendGroupMessage"));
                    lua.RegisterFunction("sendPrivateMessage", null, typeof(LuaApi).GetMethod("SendPrivateMessage"));
                    lua.RegisterFunction("getImageUrl", null, typeof(LuaApi).GetMethod("GetImageUrl"));
                    lua.RegisterFunction("httpDownload", null, typeof(LuaApi).GetMethod("HttpDownload"));
                    lua.DoFile(AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "lua/head.lua");
                    lua.DoString(Encoding.UTF8.GetBytes(headRun));
                    lua.DoString(Encoding.UTF8.GetBytes(code));
                    if (Tools.CharNum(lua["lua_run_result_var"].ToString(), "\n") > 40 ||
                        Tools.CharNum(lua["lua_run_result_var"].ToString(), "\r") > 40)
                        result = "行数超过了20行，限制一下吧";
                    else if (lua["lua_run_result_var"].ToString().Length > 2000)
                        result = "字数超过了2000，限制一下吧";
                    else
                        result = lua["lua_run_result_var"].ToString();
                }
                catch (Exception e)
                {
                    string err = e.Message;
                    int l = err.IndexOf("lua/");
                    if (l >= 0)
                        err = err.Substring(l);
                    result = "代码崩掉啦\r\n" + err;
                }
            }
        }

        public void CallWithTimeout(int timeoutMilliseconds)
        {
            Thread threadToKill = null;
            Action wrappedAction = () =>
            {
                threadToKill = Thread.CurrentThread;
                Run(code);
            };

            IAsyncResult result = wrappedAction.BeginInvoke(null, null);
            if (result.AsyncWaitHandle.WaitOne(timeoutMilliseconds))
            {
                wrappedAction.EndInvoke(result);
            }
            else
            {
                threadToKill.Abort();
            }
        }
    }
}
