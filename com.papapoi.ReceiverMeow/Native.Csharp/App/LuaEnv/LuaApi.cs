using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Runtime.InteropServices;
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
            bmp.Save(AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "data/image/" + result + ".luatemp", ImageFormat.Jpeg);
            return result + ".luatemp";
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
        /// <param name="fileName">路径</param>
        /// <param name="timeout">超时时间</param>
        /// <returns>下载结果</returns>
        public static bool HttpDownload(string Url, string fileName, int timeout = 5000)
        {
            //fileName = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "data/" + fileName;
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
            catch (Exception e)
            {
                Common.CqApi.AddLoger(Sdk.Cqp.Enum.LogerLevel.Error, "下载文件错误", e.ToString());
            }
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
            catch (Exception e)
            {
                Common.CqApi.AddLoger(Sdk.Cqp.Enum.LogerLevel.Error, "get错误", e.ToString());
            }
            return "";
        }

        /// <summary>
        /// POST请求与获取结果
        /// </summary>
        public static string HttpPost(string Url, string postDataStr, int timeout = 5000,
            string cookie = "",string contentType = "application/x-www-form-urlencoded")
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
                request.ContentType = contentType + "; charset=UTF-8";
                byte[] byteResquest = Encoding.UTF8.GetBytes(postDataStr);
                request.ContentLength = byteResquest.Length;
                request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/71.0.3578.98 Safari/537.36 Vivaldi/2.2.1388.37";
                if (cookie != "")
                    request.Headers.Add("cookie", cookie);

                Stream stream = request.GetRequestStream();
                stream.Write(byteResquest, 0, byteResquest.Length);
                stream.Close();
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
            {
                Common.CqApi.AddLoger(Sdk.Cqp.Enum.LogerLevel.Error, "post错误", e.ToString());
            }
            return "";
        }

        /// <summary>
        /// 获取在线文件的base64结果
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string Base64File(string url)
        {
            try
            {
                //请求前设置一下使用的安全协议类型 System.Net
                if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
                {
                    ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback((object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors) =>
                    {
                        return true; //总是接受
                    });
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                }
                //获取网址里的图片
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                WebResponse response = request.GetResponse();
                request.Timeout = 15000;
                Stream stream = response.GetResponseStream();

                Bitmap bmp = new Bitmap(stream);
                MemoryStream ms = new MemoryStream();
                bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                byte[] arr = new byte[ms.Length];
                ms.Position = 0;
                ms.Read(arr, 0, (int)ms.Length);
                ms.Close();
                return Convert.ToBase64String(arr);
            }
            catch (Exception e)
            {
                Common.CqApi.AddLoger(Sdk.Cqp.Enum.LogerLevel.Error, "base64错误", e.ToString());
            }
            return "";
        }

        private static Dictionary<string, string> luaTemp = new Dictionary<string, string>();
        /// <summary>
        /// 把值存入ram
        /// </summary>
        /// <param name="n"></param>
        /// <param name="d"></param>
        public static void SetVar(string n,string d)
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

        public static string CqCode_At(long qq) => Common.CqApi.CqCode_At(qq);
        //获取酷Q "At某人" 代码
        public static string CqCode_Emoji(int id) => Common.CqApi.CqCode_Emoji(id);
        //获取酷Q "emoji表情" 代码
        public static string CqCode_Face(int id) => Common.CqApi.CqCode_Face((Sdk.Cqp.Enum.Face)id);
        //获取酷Q "表情" 代码
        public static string CqCode_Shake() => Common.CqApi.CqCode_Shake();
        //获取酷Q "窗口抖动" 代码
        public static string CqCode_Trope(string str) => Common.CqApi.CqCode_Trope(str);
        //获取字符串的转义形式
        public static string CqCode_UnTrope(string str) => Common.CqApi.CqCode_UnTrope(str);
        //获取字符串的非转义形式
        public static string CqCode_ShareLink(string url, string title, string content, string imgUrl) => Common.CqApi.CqCode_ShareLink(url, title, content, imgUrl);
        //获取酷Q "链接分享" 代码
        public static string CqCode_ShareCard(string cardType, long id) => Common.CqApi.CqCode_ShareCard(cardType, id);
        //获取酷Q "名片分享" 代码
        public static string CqCode_ShareGPS(string site, string detail, double lat, double lon, int zoom) => Common.CqApi.CqCode_ShareGPS(site, detail, lat, lon, zoom);
        //获取酷Q "位置分享" 代码
        public static string CqCode_Anonymous(bool forced) => Common.CqApi.CqCode_Anonymous(forced);
        //获取酷Q "匿名" 代码
        public static string CqCode_Image(string path) => Common.CqApi.CqCode_Image(path);
        //获取酷Q "图片" 代码
        public static string CqCode_Music(long id, string type, bool newStyle) => Common.CqApi.CqCode_Music(id, type, newStyle);
        //获取酷Q "音乐" 代码
        public static string CqCode_MusciDIY(string url, string musicUrl, string title, string content, string imgUrl) => Common.CqApi.CqCode_MusciDIY(url, musicUrl, title, content, imgUrl);
        //获取酷Q "音乐自定义" 代码
        public static string CqCode_Record(string path) => Common.CqApi.CqCode_Record(path);
        //获取酷Q "语音" 代码
        public static int SendGroupMessage(long groupId, string message) => Common.CqApi.SendGroupMessage(groupId, message);
        //发送群消息
        public static int SendPrivateMessage(long qqId, string message) => Common.CqApi.SendPrivateMessage(qqId, message);
        //发送私聊消息
        public static int SendDiscussMessage(long discussId, string message) => Common.CqApi.SendDiscussMessage(discussId, message);
        //发送讨论组消息
        public static int SendPraise(long qqId, int count) => Common.CqApi.SendPraise(qqId, count);
        //发送赞
        public static int RepealMessage(int id) => Common.CqApi.RepealMessage(id);
        //撤回消息
        public static long GetLoginQQ() => Common.CqApi.GetLoginQQ();
        //取登录QQ
        public static string GetLoginNick() => Common.CqApi.GetLoginNick();
        //获取当前登录QQ的昵称
        public static string GetAppDirectory() => Common.CqApi.GetAppDirectory();
        //取应用目录
        public static int AddLoger(int level, string type, string content) => Common.CqApi.AddLoger((Sdk.Cqp.Enum.LogerLevel)level, type, content);
        //添加日志
        public static int AddFatalError(string msg) => Common.CqApi.AddFatalError(msg);
        //添加致命错误提示
        public static int SetGroupWholeBanSpeak(long groupId, bool isOpen) => Common.CqApi.SetGroupWholeBanSpeak(groupId, isOpen);
        //置全群禁言
        public static int SetFriendAddRequest(string tag,int respond,string msg) => Common.CqApi.SetFriendAddRequest(tag, (Sdk.Cqp.Enum.ResponseType)respond, msg);
        //置好友添加请求
        public static int SetGroupAddRequest(string tag, int request, int respond, string msg) => Common.CqApi.SetGroupAddRequest(tag, (Sdk.Cqp.Enum.RequestType)request, (Sdk.Cqp.Enum.ResponseType)respond, msg);
        //置群添加请求
        public static int SetGroupMemberNewCard(long groupId, long qqId, string newNick) => Common.CqApi.SetGroupMemberNewCard(groupId, qqId, newNick);
        //置群成员名片
        public static int SetGroupManager(long groupId, long qqId, bool isCalcel) => Common.CqApi.SetGroupManager(groupId, qqId, isCalcel);
        //置群管理员
        public static int SetAnonymousStatus(long groupId, bool isOpen) => Common.CqApi.SetAnonymousStatus(groupId, isOpen);
        //置群匿名设置
        public static int SetGroupMemberRemove(long groupId, long qqId, bool notAccept) => Common.CqApi.SetGroupMemberRemove(groupId, qqId, notAccept);
        //置群员移除
        public static int SetDiscussExit(long discussId) => Common.CqApi.SetDiscussExit(discussId);
        //置讨论组退出
    }
}
