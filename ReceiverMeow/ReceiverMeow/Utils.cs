using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
该版本为测试版本，可能会有无法预料的行为
如有需稳定运行，请使用正式版
**********************************");

            Setting.MqttEnable = Setting.MqttEnable;
            Setting.TcpServerEnable = Setting.TcpServerEnable;
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
    }
}
