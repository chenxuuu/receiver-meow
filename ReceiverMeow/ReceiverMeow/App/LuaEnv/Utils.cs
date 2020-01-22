using Native.Csharp.Tool.IniConfig.Linq;
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


    }
}
