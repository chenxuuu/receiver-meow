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
    class Tools
    {
        /// <summary>
        /// 全局配置
        /// </summary>
        public static Settings Setting;

        /// <summary>
        /// 软件根目录完整路径
        /// </summary>
        public static string Path;

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

            
        }
    }
}
