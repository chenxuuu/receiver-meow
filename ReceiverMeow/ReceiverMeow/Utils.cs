using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
        }

    }
}
