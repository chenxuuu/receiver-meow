using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Native.Csharp.App.LuaEnv
{
    class TimerRun
    {
        private static bool start = false;
        public static void TimerStart()
        {
            if (start)
                return;
            start = true;
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Enabled = true;
            timer.Interval = 1000;//1s
            timer.Elapsed += new System.Timers.ElapsedEventHandler(Timer1_Elapsed);
            timer.Start();

            System.Timers.Timer timer2 = new System.Timers.Timer();
            timer2.Enabled = true;
            timer2.Interval = 60000;//1m
            timer2.Elapsed += new System.Timers.ElapsedEventHandler(Timer2_Elapsed);
            timer2.Start();
        }

        public static void Timer1_Elapsed(object sender, System.Timers.ElapsedEventArgs e)  //1s定时程序
        {
            // 得到 hour minute second  如果等于某个值就开始执行某个程序。  
            int intHour = e.SignalTime.Hour;
            int intMinute = e.SignalTime.Minute;
            int intSecond = e.SignalTime.Second;

            if (intSecond == 0)//每分钟执行脚本
                LuaEnv.RunLua("", "envent/TimerMinute.lua");

            //检查升级
            if(intSecond == 0 && intMinute==0 && intHour == 3)
            {

            }
        }

        public static void Timer2_Elapsed(object sender, System.Timers.ElapsedEventArgs e)  //1m定时程序
        {
            // 得到 hour minute second  如果等于某个值就开始执行某个程序。  
            int intHour = e.SignalTime.Hour;
            int intMinute = e.SignalTime.Minute;

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
                    if (time.TotalSeconds > 120)
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
