using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Native.Csharp.App.LuaEnv
{
    class LuaApi
    {
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
            Graphics pic = Graphics.FromImage(bmp);
            Font font = new Font(type, size);
            Color myColor = Color.FromArgb(r, g, b);
            SolidBrush myBrush = new SolidBrush(myColor);
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
        public static Bitmap putBlock(Bitmap bmp, int x, int y, int xx, int yy,
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
        public static Bitmap setImage(Bitmap bmp, int x, int y, string path, int xx = 0, int yy = 0)
        {
            if (!File.Exists(path))
                return bmp;
            Bitmap b = new Bitmap(path);
            Graphics pic = Graphics.FromImage(bmp);
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
        public static string GetDir(Bitmap bmp)
        {
            string result = Tools.GetRandomString(32, true, false, false, false, "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz");
            bmp.Save(AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "data/image/download/lua" + result + ".jpg", ImageFormat.Jpeg);
            return "download/lua" + result + ".jpg";
        }


        /// <summary>
        /// 获取程序运行目录
        /// </summary>
        /// <returns>主程序运行目录</returns>
        public static string GetPath()
        {
            return AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
        }

        /// <summary>
        /// 获取qq消息中图片的网址
        /// </summary>
        /// <param name="image">图片字符串，如“[CQ:image,file=123123]”</param>
        /// <returns>网址</returns>
        public static string GetImageUrl(string image)
        {
            string fileName = Tools.Reg_get(image, "\\[CQ:image,file=(?<name>.*?)\\]", "name") + ".cqimg";//获取文件名
            if (File.Exists(AppDomain.CurrentDomain.SetupInformation.ApplicationBase +
                        @"data\image\" + fileName))
                return Tools.Reg_get(File.ReadAllText(AppDomain.CurrentDomain.SetupInformation.ApplicationBase +
                        @"data\image\" + fileName).Replace("\r", "").Replace("\n", ""),
                        "url=(?<name>.*?)addtime=", "name");//过滤出图片网址
            return "";//没这个文件
        }


        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="Url">文件网址</param>
        /// <param name="fileName">文件名+路径</param>
        /// <param name="timeout">超时时间</param>
        /// <returns>下载结果</returns>
        public static bool HttpDownload(string Url, string fileName, int timeout = 5000)
        {
            fileName = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "data/" + fileName;
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
                request.ContentType = "text/html;charset=UTF-8";
                request.Timeout = timeout;
                request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/71.0.3578.98 Safari/537.36 Vivaldi/2.2.1388.37";

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (response.ContentLength < 1024 * 1024 * 20)//超过20M的文件不下载
                {
                    return Tools.SaveBinaryFile(response, fileName);
                }
                else
                {
                    return false;
                }
            }
            catch { }
            return false;
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
                request.Method = "GET";
                request.ContentType = "text/html;charset=UTF-8";
                request.Timeout = timeout;
                request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/71.0.3578.98 Safari/537.36 Vivaldi/2.2.1388.37";
                if (cookie != "")
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
    }
}
