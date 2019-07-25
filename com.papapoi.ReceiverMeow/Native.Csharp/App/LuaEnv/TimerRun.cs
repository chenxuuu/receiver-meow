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
        public static int luaWait = 60;//间隔多少秒执行一次
        private static uint count = 60;
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

            count++;
            if (count >= luaWait)//每分钟执行脚本
            {
                LuaEnv.RunLua("", "envent/TimerMinute.lua");
                count = 0;
            }

            //检查升级
            if (intSecond == 0 && intMinute==0 && intHour == 3)
            {
                //检查是否开启了检查更新
                if (XmlApi.xml_get("settings", "autoUpdate").ToUpper() != "TRUE")
                    return;

                Common.CqApi.AddLoger(Sdk.Cqp.Enum.LogerLevel.Info,"lua脚本更新检查", "正在检查脚本更新");
                string gitPath = Common.AppDirectory + "git/";
                if (!Repository.IsValid(gitPath))
                {
                    Common.CqApi.AddLoger(Sdk.Cqp.Enum.LogerLevel.Error, "lua脚本更新检查", "未检测到git仓库！");
                    return;//工程不存在
                }

                using (var repo = new Repository(gitPath))
                {
                    string lastCommit = repo.Commits.First().Sha;//当前提交的特征值

                    // Credential information to fetch
                    LibGit2Sharp.PullOptions options = new LibGit2Sharp.PullOptions();
                    options.FetchOptions = new FetchOptions();

                    // User information to create a merge commit
                    var signature = new LibGit2Sharp.Signature(
                        new Identity("MERGE_USER_NAME", "MERGE_USER_EMAIL"), DateTimeOffset.Now);

                    // Pull
                    try
                    {
                        Commands.Pull(repo, signature, options);
                    }
                    catch
                    {
                        Common.CqApi.AddLoger(Sdk.Cqp.Enum.LogerLevel.Warning, "lua脚本更新检查", "代码拉取失败，请检查网络！");
                        return;
                    }

                    string newCommit = repo.Commits.First().Sha;//pull后的特征值
                    if(lastCommit != newCommit)
                    {
                        Common.CqApi.AddLoger(Sdk.Cqp.Enum.LogerLevel.Info, "lua脚本更新检查", "检测到更新内容，正在替换脚本\r\n" +
                            "注意可能会出现消息报错，无视就好");
                        Directory.Delete(Common.AppDirectory + "lua", true);
                        Tools.CopyDirectory(gitPath + "appdata/lua", Common.AppDirectory + "lua");
                        Common.CqApi.AddLoger(Sdk.Cqp.Enum.LogerLevel.Info, "lua脚本更新检查", "脚本更新完成！");
                    }
                    else
                    {
                        Common.CqApi.AddLoger(Sdk.Cqp.Enum.LogerLevel.Info, "lua脚本更新检查", "没有检测到脚本更新");
                    }
                }

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
