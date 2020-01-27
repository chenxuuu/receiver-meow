using LibGit2Sharp;
using Native.Csharp.Sdk.Cqp.Enum;
using Native.Csharp.Tool.IniConfig.Linq;
using Newtonsoft.Json;
using ReceiverMeow.UI;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Native.Csharp.App.LuaEnv
{
    class Utils
    {
        public static Settings setting;

        private static bool initialFlag = false;
        /// <summary>
        /// 插件启动后的所有东西初始化
        /// </summary>
        public static void Initial()
        {
            //加载配置
            if (File.Exists(Common.AppData.CQApi.AppDirectory + "settings.json"))
            {
                setting = JsonConvert.DeserializeObject<Settings>(
                    File.ReadAllText(Common.AppData.CQApi.AppDirectory + "settings.json"));
            }
            else
            {
                setting = new Settings();
            }
            setting.TcpServerEnable = setting.TcpServerEnable;
            //TimerRun.Start();//清理文件定时器任务，可能存在内存泄漏问题，暂时不加这个功能
            TcpServer.SendList();//tcp定时器任务

            if (initialFlag)
                return;
            initialFlag = true;

            //ui界面数据绑定
            Global.Settings = setting;
            //git按键回调函数
            ReceiverMeow.UI.Global.GitInitial += (s, e) =>
            {
                string gitPath = Common.AppData.CQApi.AppDirectory + "lua/";
                if (Directory.Exists(gitPath))
                {
                    if (!Repository.IsValid(gitPath))
                    {
                        Common.AppData.CQLog.Warning("Lua插件初始化脚本", $"lua目录已存在，且不存在Git结构，为了防止误删文件，请自行处理删除目录后再试（{gitPath}）");
                        return;
                    }
                    else
                    {
                        try
                        {
                            Common.AppData.CQLog.Info("Lua插件更新脚本", "正在更新脚本，请稍后");
                            var options = new LibGit2Sharp.PullOptions();
                            options.FetchOptions = new FetchOptions();
                            var signature = new LibGit2Sharp.Signature(
                                new Identity("MERGE_USER_NAME", "MERGE_USER_EMAIL"), DateTimeOffset.Now);
                            using var repo = new Repository(gitPath);
                            Commands.Pull(repo, signature, options);
                            LuaStates.Clear();
                            LuaEnv.LuaStates.Run("main", "AppEnable", new { });
                            Common.AppData.CQLog.Info("Lua插件初始化脚本", "更新完成！您可以开始用了");
                            return;
                        }
                        catch (Exception ee)
                        {
                            Common.AppData.CQLog.Warning("Lua插件更新脚本", $"更新脚本文件失败，错误信息：{ee.Message}");
                            return;//pull失败
                        }
                    }
                }
                Common.AppData.CQLog.Info("Lua插件初始化脚本", "正在下载初始脚本，请稍后");
                try
                {
                    Repository.Clone("https://gitee.com/chenxuuu/receiver-meow-lua.git", gitPath);
                    LuaStates.Clear();
                    LuaEnv.LuaStates.Run("main", "AppEnable", new { });
                    Common.AppData.CQLog.Info("Lua插件初始化脚本", "初始化完成！您可以开始用了");
                }
                catch(Exception ee)
                {
                    Common.AppData.CQLog.Warning("Lua插件初始化脚本", $"初始化脚本文件失败，错误信息：{ee.Message}");
                    return;//clone失败，还原
                }
            };
            //重载虚拟机按键回调函数
            ReceiverMeow.UI.Global.LuaInitial += (s, e) =>
            {
                LuaStates.Clear();
                LuaEnv.LuaStates.Run("main", "AppEnable", new { });
            };
            //重载Xml按键回调函数
            ReceiverMeow.UI.Global.XmlInitial += (s, e) =>
            {
                XmlApi.Clear();
            };
        }

        /// <summary>
        /// 获取版本号
        /// </summary>
        /// <returns></returns>
        public static string GetVersion() => Common.AppInfo.Version.ToString();

        /// <summary>
        /// 转时间戳
        /// https://blog.csdn.net/qq_24025219/article/details/100146913
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static int DateTimeToInt(TimeSpan ts2)
        {
            TimeSpan ts3 = new TimeSpan(DateTime.Parse("1970-01-01").Ticks);
            TimeSpan ts_1 = ts2.Subtract(ts3).Duration();
            int NowMinu = (int)ts_1.TotalSeconds;
            int date = NowMinu;
            return date;
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
        /// 获取图片对象
        /// </summary>
        /// <param name="width">宽度</param>
        /// <param name="length">高度</param>
        /// <returns>图片对象</returns>
        public static Bitmap GetBitmap(int width, int length)
        {
            Bitmap bmp = new Bitmap(width, length);
            return bmp;
        }

        /// <summary>
        /// 摆放文字
        /// </summary>
        /// <param name="bmp">图片对象</param>
        /// <param name="x">x坐标</param>
        /// <param name="y">y坐标</param>
        /// <param name="text">文字内容</param>
        /// <param name="type">字体名称</param>
        /// <param name="size">字体大小</param>
        /// <param name="r">r</param>
        /// <param name="g">g</param>
        /// <param name="b">b</param>
        /// <returns>图片对象</returns>
        public static Bitmap PutText(Bitmap bmp, int x, int y, string text, string type = "宋体", int size = 9,
            int r = 0, int g = 0, int b = 0)
        {
            using Graphics pic = Graphics.FromImage(bmp);
            using Font font = new Font(type, size);
            Color myColor = Color.FromArgb(r, g, b);
            using SolidBrush myBrush = new SolidBrush(myColor);
            pic.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            pic.DrawString(text, font, myBrush, new PointF() { X = x, Y = y });
            return bmp;
        }

        /// <summary>
        /// 填充矩形
        /// </summary>
        /// <param name="bmp">图片对象</param>
        /// <param name="x">起始x坐标</param>
        /// <param name="y">起始y坐标</param>
        /// <param name="xx">结束x坐标</param>
        /// <param name="yy">结束y坐标</param>
        /// <param name="r">r</param>
        /// <param name="g">g</param>
        /// <param name="b">b</param>
        /// <returns>图片对象</returns>
        public static Bitmap PutBlock(Bitmap bmp, int x, int y, int xx, int yy,
            int r = 0, int g = 0, int b = 0)
        {
            Color myColor = Color.FromArgb(r, g, b);
            //遍历矩形框内的各象素点
            for (int i = x; i <= xx; i++)
            {
                for (int j = y; j <= yy; j++)
                {
                    bmp.SetPixel(i, j, myColor);//设置当前象素点的颜色
                }
            }
            return bmp;
        }

        /// <summary>
        /// 摆放图片
        /// </summary>
        /// <param name="bmp">图片对象</param>
        /// <param name="x">起始x坐标</param>
        /// <param name="y">起始y坐标</param>
        /// <param name="path">图片路径</param>
        /// <param name="xx">摆放图片宽度</param>
        /// <param name="yy">摆放图片高度</param>
        /// <returns>图片对象</returns>
        public static Bitmap SetImage(Bitmap bmp, int x, int y, string path, int xx = 0, int yy = 0)
        {
            if (!File.Exists(path))
                return bmp;
            using Bitmap b = new Bitmap(path);
            using Graphics pic = Graphics.FromImage(bmp);
            if (xx != 0 && yy != 0)
                pic.DrawImage(b, x, y, xx, yy);
            else
                pic.DrawImage(b, x, y);
            return bmp;
        }

        /// <summary>
        /// 保存并获取图片路径
        /// </summary>
        /// <param name="bmp">图片对象</param>
        /// <returns>图片路径</returns>
        public static string SaveImage(Bitmap bmp,string name)
        {
            bmp.Save(AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "data/image/" + name + ".luatemp", System.Drawing.Imaging.ImageFormat.Jpeg);
            bmp.Dispose();
            return name + ".luatemp";
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
        /// 用MD5加密字符串
        /// </summary>
        /// <param name="password">待加密的字符串</param>
        /// <returns></returns>
        public static string MD5Encrypt(string password)
        {
            MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider();
            byte[] hashedDataBytes;
            hashedDataBytes = md5Hasher.ComputeHash(Encoding.Default.GetBytes(password));
            StringBuilder tmp = new StringBuilder();
            foreach (byte i in hashedDataBytes)
            {
                tmp.Append(i.ToString("x2"));
            }
            return tmp.ToString();
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="Url">文件网址</param>
        /// <param name="fileName">路径</param>
        /// <param name="timeout">超时时间</param>
        /// <returns>下载结果</returns>
        public static bool HttpDownload(string Url, string fileName, long maxLength, long timeout = 5000)
        {
            HttpWebRequest request = null;
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
                request = (HttpWebRequest)WebRequest.Create(Url);
                request.ContentType = "text/html;charset=UTF-8";
                request.Timeout = (int)timeout;
                request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/71.0.3578.98 Safari/537.36 Vivaldi/2.2.1388.37";

                using HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (response.ContentLength < maxLength)//1024 * 1024 * 20)//超过20M的文件不下载
                {
                    try
                    {
                        if (File.Exists(fileName))
                            File.Delete(fileName);
                        using Stream outStream = System.IO.File.Create(fileName);
                        using Stream inStream = response.GetResponseStream();
                        int l;
                        byte[] buffer = new byte[1024];
                        do
                        {
                            l = inStream.Read(buffer, 0, buffer.Length);
                            if (l > 0)
                                outStream.Write(buffer, 0, l);
                        }
                        while (l > 0);
                        outStream.Close();
                        inStream.Close();
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                Common.AppData.CQLog.Error("lua插件错误", $"下载文件错误：{e.Message}");
            }
            finally
            {
                request?.Abort();
            }
            return false;
        }


        /// <summary>
        /// GET 请求与获取结果
        /// </summary>
        public static string HttpGet(string Url, string postDataStr = "", long timeout = 5000,
            string cookie = "")
        {
            HttpWebRequest request = null;
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
                request = (HttpWebRequest)WebRequest.Create(Url + (postDataStr == "" ? "" : "?") + postDataStr);
                request.Method = "GET";
                request.ContentType = "text/html;charset=UTF-8";
                request.Timeout = (int)timeout;
                request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/71.0.3578.98 Safari/537.36 Vivaldi/2.2.1388.37";
                if (cookie != "")
                    request.Headers.Add("cookie", cookie);

                using HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                string encoding = response.ContentEncoding;
                if (encoding == null || encoding.Length < 1)
                {
                    encoding = "UTF-8"; //默认编码
                }
                using Stream myResponseStream = response.GetResponseStream();
                using StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding(encoding));

                string retString = myStreamReader.ReadToEnd();
                myStreamReader.Close();
                myResponseStream.Close();

                return retString;
            }
            catch (Exception e)
            {
                Common.AppData.CQLog.Error("lua插件错误", $"get错误：{e.Message}");
            }
            finally
            {
                request?.Abort();
            }
            return "";
        }


        /// <summary>
        /// POST请求与获取结果
        /// </summary>
        public static string HttpPost(string Url, string postDataStr, long timeout = 5000,
            string cookie = "", string contentType = "application/x-www-form-urlencoded")
        {
            HttpWebRequest request = null;
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
                request = (HttpWebRequest)WebRequest.Create(Url);
                request.Method = "POST";
                request.Timeout = (int)timeout;
                request.ContentType = contentType + "; charset=UTF-8";
                byte[] byteResquest = Encoding.UTF8.GetBytes(postDataStr);
                request.ContentLength = byteResquest.Length;
                request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/71.0.3578.98 Safari/537.36 Vivaldi/2.2.1388.37";
                if (cookie != "")
                    request.Headers.Add("cookie", cookie);

                using Stream stream = request.GetRequestStream();
                stream.Write(byteResquest, 0, byteResquest.Length);
                stream.Close();
                using HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                string encoding = response.ContentEncoding;
                if (encoding == null || encoding.Length < 1)
                {
                    encoding = "UTF-8"; //默认编码
                }
                using StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding(encoding));
                string retString = reader.ReadToEnd();
                return retString;
            }
            catch (Exception e)
            {
                Common.AppData.CQLog.Error("lua插件错误", $"post错误：{e.Message}");
            }
            finally
            {
                request?.Abort();
            }
            return "";
        }

        /// <summary>
        /// 模拟multipart/form-data文件上传
        /// </summary>
        public static string HttpUploadFile(string Url, string paramName, string path, long timeout = 5000, string cookie = "")
        {
            HttpWebRequest request = null;
            if (path == null || path.Length < 1)
            {
                return null;
            }
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

                string boundary = "---------------------------" + GetRandomString(16, true, true, true, false);
                string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\n" +
                    "Content-Type: application/octet-stream\r\n\r\n";
                string header = string.Format(headerTemplate, paramName, Path.GetFileName(path));
                byte[] headerbytes = Encoding.UTF8.GetBytes(header);
                byte[] boundarybytes = Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");
                byte[] endbytes = Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
                request = (HttpWebRequest)WebRequest.Create(Url);
                request.Method = "POST";
                request.Timeout = (int)timeout;
                request.ContentType = "multipart/form-data; boundary=" + boundary;
                request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/71.0.3578.98 Safari/537.36 Vivaldi/2.2.1388.37";
                if (cookie != "")
                    request.Headers.Add("cookie", cookie);

                using FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
                request.ContentLength = boundarybytes.Length + headerbytes.Length + fileStream.Length + endbytes.Length;
                using Stream stream = request.GetRequestStream();
                stream.Write(boundarybytes, 0, boundarybytes.Length);
                stream.Write(headerbytes, 0, headerbytes.Length);
                byte[] buffer = new byte[4096];
                int bytesRead = 0;
                while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    stream.Write(buffer, 0, bytesRead);//将字节写入
                }
                stream.Write(endbytes, 0, endbytes.Length);
                using HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                string encoding = response.ContentEncoding;
                if (encoding == null || encoding.Length < 1)
                {
                    encoding = "UTF-8"; //默认编码
                }
                using StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding(encoding));
                string retString = reader.ReadToEnd();
                return retString;
            }
            catch (Exception e)
            {
                Common.AppData.CQLog.Error("lua插件错误", $"上传错误：{e.Message}");
            }
            finally
            {
                request?.Abort();
            }
            return null;
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
        public static string GetRandomString(int length, bool useNum = true, bool useLow = true, bool useUpp = true, bool useSpe = false, string custom = "")
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

        /// <summary>
        /// 获取本地图片的base64结果，会转成jpeg
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string Base64File(string path)
        {
            try
            {
                using Bitmap bmp = new Bitmap(path);
                using MemoryStream ms = new MemoryStream();
                bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                byte[] arr = new byte[ms.Length];
                ms.Position = 0;
                ms.Read(arr, 0, (int)ms.Length);
                ms.Close();
                return Convert.ToBase64String(arr);
            }
            catch (Exception e)
            {
                Common.AppData.CQLog.Error("lua插件错误", $"base64错误：{e.Message}");
            }
            return "";
        }

        /// <summary>
        /// 获取图片源文件高度
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static int GetRawPictureHeight(string path)
        {
            try
            {
                using Bitmap bmp = new Bitmap(path);
                return bmp.Height;
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// 获取图片源文件宽度
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static int GetRawPictureWidth(string path)
        {
            try
            {
                using Bitmap bmp = new Bitmap(path);
                return bmp.Width;
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// 获取图片宽度
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static int GetPictureWidth(string image)
        {
            string fileName = Reg_get(image, "\\[CQ:image,file=(?<name>.*?)\\]", "name") + ".cqimg";//获取文件名
            string filePath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"data\image\" + fileName;
            if (File.Exists(filePath))
            {
                IniObject iObject = IniObject.Load(filePath, Encoding.Default);
                return iObject["image"]["width"].ToInt32();
            }
            return 0;//没这个文件
        }

        /// <summary>
        /// 获取图片高度
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static int GetPictureHeight(string image)
        {
            string fileName = Reg_get(image, "\\[CQ:image,file=(?<name>.*?)\\]", "name") + ".cqimg";//获取文件名
            string filePath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"data\image\" + fileName;
            if (File.Exists(filePath))
            {
                IniObject iObject = IniObject.Load(filePath, Encoding.Default);
                return iObject["image"]["height"].ToInt32();

            }
            return 0;//没这个文件
        }

        /// <summary>
        /// 接收并获取qq消息中图片的路径
        /// </summary>
        /// <param name="image">图片字符串，如“[CQ:image,file=123123]”</param>
        /// <returns>网址</returns>
        public static string GetImagePath(string image)
        {
            string fileName = Reg_get(image, "\\[CQ:image,file=(?<name>.*?)\\]", "name");//获取文件
            if (fileName == "")
                return "";
            return Common.AppData.CQApi.ReceiveImage(fileName);
        }


        private static ConcurrentDictionary<string, string> luaTemp = new ConcurrentDictionary<string, string>();
        /// <summary>
        /// 把值存入ram
        /// </summary>
        /// <param name="n"></param>
        /// <param name="d"></param>
        public static void SetVar(string n, string d)
        {
            luaTemp[n] = d;
        }

        /// <summary>
        /// 取出某值
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public static string GetVar(string n)
        {
            if (luaTemp.ContainsKey(n))
                return luaTemp[n];
            else
                return "";
        }

        /// <summary>
        /// 获取字符串ascii编码的hex串
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string GetAsciiHex(string str)
        {
            return BitConverter.ToString(Encoding.Default.GetBytes(str)).Replace("-", "");
        }

        /// <summary>
        /// 获取qq消息中图片的网址
        /// </summary>
        /// <param name="image">图片字符串，如“[CQ:image,file=123123]”</param>
        /// <returns>网址</returns>
        public static string GetImageUrl(string image)
        {
            string fileName = Reg_get(image, "\\[CQ:image,file=(?<name>.*?)\\]", "name") + ".cqimg";//获取文件名
            if (File.Exists(AppDomain.CurrentDomain.SetupInformation.ApplicationBase +
                        @"data\image\" + fileName))
                return Reg_get(File.ReadAllText(AppDomain.CurrentDomain.SetupInformation.ApplicationBase +
                        @"data\image\" + fileName).Replace("\r", "").Replace("\n", ""),
                        "url=(?<name>.*?)addtime=", "name");//过滤出图片网址
            return "";//没这个文件
        }

        /// <summary>
        /// 在沙盒中运行代码，仅允许安全地运行
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static string RunSandBox(string code)
        {
            using var lua = new NLua.Lua();
            try
            {
                lua.State.Encoding = Encoding.UTF8;
                lua.LoadCLRPackage();
                lua["lua_run_result_var"] = "";//返回值所在的变量
                lua.DoFile(Common.AppData.CQApi.AppDirectory + "lua/sandbox/head.lua");
                lua.DoString(code);
                return lua["lua_run_result_var"].ToString();
            }
            catch (Exception e)
            {
                Common.AppData.CQLog.Warning("lua沙盒错误", e.Message);
                return "运行错误：" + e.ToString();
            }
        }

        /// <summary>
        /// 将字符串转为base64
        /// </summary>
        /// <param name="s">输入</param>
        /// <returns>base64结果</returns>
        public static string ConvertBase64(string s)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(s));
        }

        public static string CQCode_At(long qq) => Sdk.Cqp.CQApi.CQCode_At(qq).ToString();
        public static string CQCode_AtAll() => Sdk.Cqp.CQApi.CQCode_AtAll().ToString(); 
        public static string CQCode_Emoji(int id) => Sdk.Cqp.CQApi.CQCode_Emoji(id).ToString();
        public static string CQCode_Face(int id) => Sdk.Cqp.CQApi.CQCode_Face((CQFace)id).ToString();
        public static string CQCode_Shake() => Sdk.Cqp.CQApi.CQCode_Shake().ToString(); 
        public static string CQEnCode(string s) => Sdk.Cqp.CQApi.CQEnCode(s,false).ToString(); 
        public static string CQDeCode(string s) => Sdk.Cqp.CQApi.CQDeCode(s).ToString(); 
        public static string CQCode_ShareLink(string url, string title, string content, string imageUrl = null) => 
            Sdk.Cqp.CQApi.CQCode_ShareLink(url,title,content,imageUrl).ToString(); 
        public static string CQCode_ShareFriendCard(long id) => Sdk.Cqp.CQApi.CQCode_ShareFriendCard(id).ToString(); 
        public static string CQCode_ShareGroupCard(long id) => Sdk.Cqp.CQApi.CQCode_ShareGroupCard(id).ToString();
        public static string CQCode_ShareGPS(string site, string detail, double lat, double lon, int zoom = 15) =>
            Sdk.Cqp.CQApi.CQCode_ShareGPS(site, detail, lat, lon, zoom).ToString();
        public static string CQCode_Anonymous(bool forced) => Sdk.Cqp.CQApi.CQCode_Anonymous(forced).ToString();
        public static string CQCode_Music(long id, int type, int style) => 
            Sdk.Cqp.CQApi.CQCode_Music(id,(CQMusicType)type,(CQMusicStyle)style).ToString();
        public static string CQCode_DIYMusic(string url, string musicUrl, string title, string content, string imageUrl) => 
            Sdk.Cqp.CQApi.CQCode_DIYMusic(url,musicUrl,title,content,imageUrl).ToString();
        public static string CQCode_Image(string path) => Sdk.Cqp.CQApi.CQCode_Image(path).ToString();
        public static string CQCode_Record(string path) => Sdk.Cqp.CQApi.CQCode_Image(path).ToString();
    }
}
