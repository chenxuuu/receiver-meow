using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Newbe.Mahua.Receiver.Meow.MahuaApis
{
    class WhatAnime
    {
        public static string GetAnime(string picName)
        {
            string result = "";
            string fileName = Tools.Reg_get(picName, "\\[CQ:image,file=(?<name>.*?)\\]", "name") + ".cqimg";//获取文件名
            if (fileName == ".cqimg")
                return "没有在消息中过滤出图片";
            try
            {
                string imgInfo = "";
                using (System.IO.StreamReader file = 
                    new System.IO.StreamReader(AppDomain.CurrentDomain.SetupInformation.ApplicationBase +
                        @"data\image\" + fileName))//读取文件
                {
                    imgInfo = file.ReadToEnd();
                    imgInfo = imgInfo.Replace("\r", "").Replace("\n", "");
                    imgInfo = Tools.Reg_get(imgInfo, "url=(?<name>.*?)addtime=", "name");//过滤出图片网址
                }
                //获取网址里的图片
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(imgInfo);
                WebResponse response = request.GetResponse();
                request.Timeout = 15000;
                Stream stream = response.GetResponseStream();
                //把图片转成base64
                string imgBase64 = ImgToBase64Stream(stream);
                //请求api，搜索该图片
                string token = XmlSolve.ReplayGroupStatic("common", "whatanime.ga[api]");//获取保存的token
                string html = HttpPost("https://whatanime.ga/api/search?token=" + token,
                        "image=\"data:image/jpeg;base64," + imgBase64 + "\"");
                if (html == "")
                    return "查找失败，网站炸了";
                JObject jo = (JObject)JsonConvert.DeserializeObject(html);
                try
                {
                    result = "搜索结果：";
                    result += "\r\n动画名：" + jo["docs"][0]["title_native"] + "(" + jo["docs"][0]["title_romaji"] + ")";
                    try { result += "\r\n中文名：" + jo["docs"][0]["title_chinese"]; } catch { }
                    try { result += "\r\n准确度：" + ((int)jo["docs"][0]["similarity"]).ToString("p"); } catch { }
                    try { result += "\r\n话数：" + jo["docs"][0]["episode"]; } catch { }
                    try { result += "\r\n截图出现在：" + jo["docs"][0]["from"] + "-" + jo["docs"][0]["to"] + "秒"; } catch { }
                }
                catch (Exception e)
                {
                    result = "没搜到结果，请选一张清晰的图片，或者小一点的图片(图片大小不可超过1MB)";
                }
            }
            catch (Exception e)
            {
                result = "机器人爆炸了，错误信息：\r\n" + e.ToString();
            }
            
            return result;
        }

        /// <summary>
        /// POST请求与获取结果，whatanime.ga专用，tls1.1连接
        /// </summary>
        public static string HttpPost(string Url, string postDataStr)
        {
            try
            {
                //必须设定https协议类型，不然连不上服务器
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11;
                ServicePointManager.ServerCertificateValidationCallback =
                    new System.Net.Security.RemoteCertificateValidationCallback(CheckValidationResult);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
                request.Method = "POST";
                request.Timeout = 15000;
                request.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
                request.ContentLength = postDataStr.Length;

                request.Host = "whatanime.ga";
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
            {
                return "";
            }
            return "";
        }

        public static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true;
        }

        //图片转为base64编码的字符串
        public static string ImgToBase64String(string Imagefilename)
        {
            try
            {
                Bitmap bmp = new Bitmap(Imagefilename);

                MemoryStream ms = new MemoryStream();
                bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                byte[] arr = new byte[ms.Length];
                ms.Position = 0;
                ms.Read(arr, 0, (int)ms.Length);
                ms.Close();
                return Convert.ToBase64String(arr);
            }
            catch
            {
                return "";
            }
        }

        //图片转为base64编码的字符串
        public static string ImgToBase64Stream(Stream Imagefile)
        {
            try
            {
                Bitmap bmp = new Bitmap(Imagefile);

                MemoryStream ms = new MemoryStream();
                bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                byte[] arr = new byte[ms.Length];
                ms.Position = 0;
                ms.Read(arr, 0, (int)ms.Length);
                ms.Close();
                return Convert.ToBase64String(arr);
            }
            catch
            {
                return "";
            }
        }
    }
}
