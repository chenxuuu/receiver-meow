using LibGit2Sharp;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ReceiverMeow
{
    class Utils
    {
        /// <summary>
        /// 全局配置
        /// </summary>
        public static Settings Setting = new Settings();
        /// <summary>
        /// 软件根目录完整路径
        /// </summary>
        public static string Path;
        /// <summary>
        /// 软件版本号
        /// </summary>
        public static string Version =
            System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();

        /// <summary>
        /// 初始化函数
        /// </summary>
        public static void Initial()
        {
            using (var processModule = Process.GetCurrentProcess().MainModule)
                Path = System.IO.Path.GetDirectoryName(processModule?.FileName)+"/";
            if (File.Exists(Path + "settings.json"))
            {
                Setting = JsonConvert.DeserializeObject<Settings>(
                    File.ReadAllText(Path + "settings.json"));
            }
            else
            {
                Setting = new Settings();
            }

            Log.Info("启动", $"接待喵 {Version} 版本正在启动中");
            Log.Debug("", @"
**************提示*****************
该版本为测试版本，会持续输出详细日志
如果觉得输出太多太烦，请使用正式版
**********************************");


            Setting.MqttEnable = Setting.MqttEnable;
            Setting.TcpServerEnable = Setting.TcpServerEnable;

            //更新脚本
            CheckLuaUpdate();

            CheckUpdate();

            //tcp队列定时发送检测
            LuaEnv.TcpServer.SendList();
        }

        /// <summary>
        /// 重载所有lua虚拟机
        /// </summary>
        public static void ReloadLua()
        {
            LuaEnv.LuaStates.Clear();
            LuaEnv.LuaStates.Run("main", "AppEnable", new { });
        }

        /// <summary>
        /// 检查主程序更新
        /// </summary>
        public static void CheckUpdate()
        {
            Log.Info("检查主程序更新", $"正在检查新版本。。。");
            try
            {
                var s = HttpGet("https://api.github.com/repos/chenxuuu/receiver-meow/releases/latest");
                JObject o = JsonConvert.DeserializeObject<JObject>(s);
                if((string)o["tag_name"] != Version)
                {
                    Log.Warn("检查主程序更新", $"发现新版本：{o["tag_name"]}");
                    Log.Warn("检查主程序更新", $"新版本下载页面：{o["html_url"]}");
                }
                else
                {
                    Log.Info("检查主程序更新", $"已是最新版！");
                }
            }
            catch (Exception ee)
            {
                Log.Warn("检查主程序更新", $"检查更新失败，错误信息：{ee.Message}");
            }
        }

        /// <summary>
        /// 更新脚本
        /// </summary>
        public static void CheckLuaUpdate()
        {
            var gitPath = $"{Path}lua/";
            var git = "https://gitee.com/chenxuuu/receiver-meow-lua.git";

            if (!Directory.Exists(gitPath))
            {
                Log.Warn("初始化Lua脚本", "没有检测到Lua脚本文件夹，即将下载最新脚本");
                try
                {
                    Log.Info("初始化Lua脚本", "正在获取脚本，请稍后");
                    Repository.Clone(git, gitPath);
                    Log.Info("初始化Lua脚本", "更新完成！可以开始用了");
                }
                catch (Exception ee)
                {
                    if (!Directory.Exists(gitPath))
                        Log.Error("初始化Lua脚本", $"更新脚本文件失败，错误信息：{ee.Message}\r\n如果始终失败，请手动执行下面的命令，初始化脚本后，再尝试运行：\r\n" +
                                 $"git clone {git} lua");
                    Log.Warn("更新Lua脚本", $"更新脚本文件失败，错误原因：\n{e.Message}");
                }
            }
            else
            {
                Log.Info("更新Lua脚本", "正在检查Lua脚本是否有更新。。。");
                if (Repository.IsValid($"{Path}lua/"))
                {
                    Log.Info("更新Lua脚本", "正在尝试更新脚本，请稍后");
                    try
                    {
                        var options = new LibGit2Sharp.PullOptions();
                        options.FetchOptions = new FetchOptions();
                        var signature = new LibGit2Sharp.Signature(
                            new Identity("MERGE_USER_NAME", "MERGE_USER_EMAIL"), DateTimeOffset.Now);
                        using var repo = new Repository(gitPath);
                        Commands.Pull(repo, signature, options);
                        Log.Info("更新Lua脚本", "更新操作执行完毕");
                    }
                    catch(Exception e)
                    {
                        Log.Warn("更新Lua脚本", $"拉取最新脚本代码失败，错误原因：\n{e.Message}");
                    }
                }
                else
                {
                    Log.Warn("更新Lua脚本",
                        "检测不到Git目录结构，如果你是自己写的脚本，请忽略该信息。如果你想恢复到默认脚本，请删除lua文件夹后重启软件");
                }
            }
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
        public static string MD5Encrypt(string s)
        {
            MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider();
            var hashedDataBytes = md5Hasher.ComputeHash(Encoding.Default.GetBytes(s));
            return BitConverter.ToString(hashedDataBytes).Replace("-", "");
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
                lua.DoFile(Path + "lua/sandbox/head.lua");
                lua.DoString(code);
                return lua["lua_run_result_var"].ToString();
            }
            catch (Exception e)
            {
                Log.Warn("lua沙盒错误", e.Message);
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

        /// <summary>
        /// http get接口
        /// </summary>
        /// <param name="url"></param>
        /// <param name="para"></param>
        /// <param name="timeout"></param>
        /// <param name="cookie"></param>
        /// <returns></returns>
        public static string HttpGet(string url,string para = "",long timeout = 5000,string cookie = "")
        {
            var client = new RestClient();
            if (!string.IsNullOrEmpty(para))
                url = $"{url}?{para}";//直接把参数拼后面
            client.BaseUrl = new Uri(url);
            var request = new RestRequest(Method.GET);
            request.Timeout = (int)timeout;
            if (!string.IsNullOrEmpty(cookie))
                request.AddHeader("cookie", cookie);
            var response = client.Execute(request);
            var encode = "UTF-8";//编码
            foreach (var p in response.Headers)
            {
                if(p.Name.ToLower() == "content-type" && p.Value.ToString().ToLower().IndexOf("charset=") >= 0)
                {
                    var tmp = response.ContentType.Split(';').Select(s => s.Split('='));
                    var arr = tmp.LastOrDefault(t => t.Length == 2 && t[0].Trim().ToLower() == "charset");
                    if (arr != null)
                        encode = arr[1].Trim();//如果网站有写编码
                }
            }
            return Encoding.GetEncoding(encode).GetString(response.RawBytes);
        }

        /// <summary>
        /// http post
        /// </summary>
        /// <param name="url"></param>
        /// <param name="para"></param>
        /// <param name="timeout"></param>
        /// <param name="cookie"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        public static string HttpPost(string url,string para = "",long timeout = 5000,
            string cookie = "", string contentType = "application/x-www-form-urlencoded")
        {
            var client = new RestClient();
            client.BaseUrl = new Uri(url);
            var request = new RestRequest(Method.POST);
            request.Timeout = (int)timeout;
            if (!string.IsNullOrEmpty(cookie))
                request.AddHeader("cookie", cookie);
            request.AddParameter(contentType, para, ParameterType.RequestBody);
            var response = client.Execute(request);
            var encode = "UTF-8";//编码
            foreach (var p in response.Headers)
            {
                if(p.Name.ToLower() == "content-type" && p.Value.ToString().ToLower().IndexOf("charset=") >= 0)
                {
                    var tmp = response.ContentType.Split(';').Select(s => s.Split('='));
                    var arr = tmp.LastOrDefault(t => t.Length == 2 && t[0].Trim().ToLower() == "charset");
                    if (arr != null)
                        encode = arr[1].Trim();//如果网站有写编码
                }
            }
            return Encoding.GetEncoding(encode).GetString(response.RawBytes);
        }

        /// <summary>
        /// http 下载文件接口
        /// </summary>
        /// <param name="url"></param>
        /// <param name="para"></param>
        /// <param name="timeout"></param>
        /// <param name="cookie"></param>
        /// <returns>文件长度</returns>
        public static int HttpDownload(string url, string para = "", long timeout = 15000, string cookie = "",
            string path = ".")
        {
            var client = new RestClient();
            if (!string.IsNullOrEmpty(para))
                url = $"{url}?{para}";//直接把参数拼后面
            client.BaseUrl = new Uri(url);
            var request = new RestRequest(Method.GET);
            request.Timeout = (int)timeout;
            if (!string.IsNullOrEmpty(cookie))
                request.AddHeader("cookie", cookie);
            var response = client.DownloadData(request);
            File.WriteAllBytes(path, response);
            return response.Length;
        }


        /// <summary>
        /// 获取图片对象
        /// </summary>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        /// <returns>图片对象</returns>
        public static Image GetBitmap(int width, int height)
        {
            Image bmp = new Image<Rgba32>(width, height);
            return bmp;
        }

        /// <summary>
        /// 摆放文字
        /// </summary>
        /// <param name="image">图片对象</param>
        /// <param name="x">x坐标</param>
        /// <param name="y">y坐标</param>
        /// <param name="text">文字内容</param>
        /// <param name="font">字体文件路径</param>
        /// <param name="size">字体大小</param>
        /// <param name="r">r</param>
        /// <param name="g">g</param>
        /// <param name="b">b</param>
        /// <returns>图片对象</returns>
        public static Image PutText(Image image, float xx, float yy, string text, string font = "", int size = 9,
            int r = 0, int g = 0, int b = 0)
        {
            FontCollection collection = new FontCollection();
            FontFamily family = collection.Install(font);
            Font f = family.CreateFont(size, FontStyle.Regular);
            image.Mutate(x => x.DrawText(text, f,
                new Color(new Rgba32(r / 255f, g / 255f, b / 255f)), new PointF(xx, yy)));
            return image;
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
        public static Image PutBlock(Image bmp, int x, int y, int xx, int yy,
            int r = 0, int g = 0, int b = 0)
        {
            Color myColor = new Color(new Rgba32(r / 255f, g / 255f, b / 255f));
            var rectangle = new RectangleF(x, y, xx - x + 1, yy - y + 1);

            bmp.Mutate(i => i.Fill(myColor, rectangle));
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
        public static Image SetImage(Image bmp, int x, int y, string path, int xx = 0, int yy = 0)
        {
            if (!File.Exists(path))
                return bmp;
            using Image b = Image.Load(path);

            if (xx != 0 && yy != 0)
            {
                b.Mutate(i => i.Resize(xx,yy));
                bmp.Mutate(i => i.DrawImage(b, new Point(x, y), 1));
            }
            else
            {
                bmp.Mutate(i => i.DrawImage(b, new Point(x, y), 1));
            }
            return bmp;
        }


        /// <summary>
        /// 保存并获取图片路径
        /// </summary>
        /// <param name="bmp">图片对象</param>
        /// <returns>图片路径</returns>
        public static bool SaveImage(Image bmp, string path)
        {
            bmp.SaveAsJpeg(path);
            bmp.Dispose();
            return true;
        }

        /// <summary>
        /// 保存并获取图片路径
        /// </summary>
        /// <param name="bmp">图片对象</param>
        /// <returns>图片路径</returns>
        public static string ImageBase64(Image bmp)
        {
            var s = bmp.ToBase64String(JpegFormat.Instance);
            bmp.Dispose();
            return s;
        }

    }
}
