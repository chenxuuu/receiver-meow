using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Native.Csharp.App.LuaEnv
{
    /// <summary>
    /// 定时任务类
    /// </summary>
    class TimerRun
    {
        private static bool start = false;
        public static void Start()
        {
            if (start)
                return;
            start = true;
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Enabled = true;
            timer.Interval = 60000;//60s
            timer.Elapsed += new System.Timers.ElapsedEventHandler((s,e) => 
            {
                try
                {
                    task();
                }
                catch(Exception ee)
                {
                    Common.AppData.CQLog.Error("lua插件", $"timer任务报错：{ee.Message}");
                }
            });
            timer.Start();
        }

        private static void task()  //1m定时程序
        {
            //删除过期图片文件
            DirectoryInfo downloadDir = new DirectoryInfo(AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "data/image/");
            FileSystemInfo[] downloadFiles = downloadDir.GetFileSystemInfos();
            for (int i = 0; i < downloadFiles.Length; i++)
            {
                FileInfo file = downloadFiles[i] as FileInfo;
                //是文件
                if (file != null && file.Name.IndexOf(".luatemp") == file.Name.Length - (".luatemp").Length ||
                    file != null && file.Name.IndexOf(".jpg") == file.Name.Length - (".jpg").Length ||
                    file != null && file.Name.IndexOf(".png") == file.Name.Length - (".png").Length ||
                    file != null && file.Name.IndexOf(".gif") == file.Name.Length - (".gif").Length)
                {
                    TimeSpan time = DateTime.Now - file.CreationTime;
                    if (time.TotalSeconds > 120)//大于两分钟前的文件
                        file.Delete();
                }
            }

            //删除过期语音文件
            DirectoryInfo recordDir = new DirectoryInfo(AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "data/record/");
            FileSystemInfo[] recordFiles = recordDir.GetFileSystemInfos();
            for (int i = 0; i < recordFiles.Length; i++)
            {
                FileInfo file = recordFiles[i] as FileInfo;
                //是文件
                if (file != null)
                {
                    TimeSpan time = DateTime.Now - file.CreationTime;
                    if (time.TotalSeconds > 60)
                        file.Delete();
                }
            }
        }
    }
}
