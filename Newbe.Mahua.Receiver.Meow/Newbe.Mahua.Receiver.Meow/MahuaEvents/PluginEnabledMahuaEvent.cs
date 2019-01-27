using Newbe.Mahua.MahuaEvents;
using Newbe.Mahua.Receiver.Meow.MahuaApis;
using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Newbe.Mahua.Receiver.Meow.MahuaEvents
{
    /// <summary>
    /// 插件被启用事件
    /// </summary>
    public class PluginEnabledMahuaEvent
        : IPluginEnabledMahuaEvent
    {
        private readonly IMahuaApi _mahuaApi;

        public PluginEnabledMahuaEvent(
            IMahuaApi mahuaApi)
        {
            _mahuaApi = mahuaApi;
        }

        public void Enabled(PluginEnabledContext context)
        {
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Enabled = true;
            timer.Interval = 1000;// 执行间隔时间, 单位为毫秒  
            timer.Elapsed += new System.Timers.ElapsedEventHandler(Timer1_Elapsed);
            timer.Start();
        }

        private void Timer1_Elapsed(object sender, System.Timers.ElapsedEventArgs e)  //定时程序
        {
            // 得到 hour minute second  如果等于某个值就开始执行某个程序。  
            int intHour = e.SignalTime.Hour;
            int intMinute = e.SignalTime.Minute;
            int intSecond = e.SignalTime.Second;

            //删除过期文件
            DirectoryInfo downloadDir = new DirectoryInfo(AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "data/image/download/");
            FileSystemInfo[] downloadFiles = downloadDir.GetFileSystemInfos();
            for (int i = 0; i < downloadFiles.Length; i++)
            {
                FileInfo file = downloadFiles[i] as FileInfo;
                //是文件
                if (file != null)
                {
                    TimeSpan time = DateTime.Now - file.CreationTime;
                    if(time.TotalSeconds > 30)
                        file.Delete();
                }
            }

            if (intMinute == 0 && intSecond == 10 && intHour == 0 && Tools.special.Length > 0)
            {
                var _m = MahuaRobotManager.Instance.CreateSession().MahuaApi;
                int count = Tools.GetXmlNumber("daily_sign_in_count_all", System.DateTime.Today.AddDays(-1).ToString());
                _m.SendGroupMessage("241464054", "新的一天已经到来了哦，现在时间是\r\n" + DateTime.Now.ToString() + "\r\n昨日一共有" + count + "人签到哦");
                _m.SendGroupMessage("567145439", DateTime.Now.ToString() + "\r\n今天一共有" + count + "人签到哦");
            }
            if (intMinute == 0 && intSecond == 10 && intHour == 4 && Tools.special.Length > 0)
            {
                var _m = MahuaRobotManager.Instance.CreateSession().MahuaApi;
                _m.SendGroupMessage("567145439", "服务器备份已开始，硬盘可用空间：\r\n" +
                        "服务器盘剩余空间：" + ((float)Tools.GetHardDiskFreeSpace("D") / 1024).ToString(".00") + "GB\r\n" +
                        "备份盘剩余空间：" + ((float)Tools.GetHardDiskFreeSpace("E") / 1024).ToString(".00") + "GB");
                System.Diagnostics.Process.Start(@"D:\backup.bat");
            }
            if (intMinute == 0 && intSecond == 10 && intHour == 5 && Tools.special.Length > 0)
            {
                var _m = MahuaRobotManager.Instance.CreateSession().MahuaApi;
                _m.SendGroupMessage("567145439", "服务器备份肯定已经结束了，硬盘可用空间：\r\n" +
                        "服务器盘剩余空间：" + ((float)Tools.GetHardDiskFreeSpace("D") / 1024).ToString(".00") + "GB\r\n" +
                        "备份盘剩余空间：" + ((float)Tools.GetHardDiskFreeSpace("E") / 1024).ToString(".00") + "GB");
                if (Tools.GetHardDiskFreeSpace("E") < 1024 * 10)
                {
                    _m.SendGroupMessage("567145439", Tools.At(Tools.adminNumber) + "警告：服务器备份盘可用空间仅剩余" +
                        ((float)Tools.GetHardDiskFreeSpace("E") / 1024).ToString(".00") + "G！请及时清理多于文件！");
                }
            }

            if(intMinute == 0 && intSecond == 0 && intHour == 1)
            {
                var _m = MahuaRobotManager.Instance.CreateSession().MahuaApi;
                _m.SendGroupMessage(Tools.mainGroupNumber, "开始文件自动清理任务");
                int records = 0,imageall = 0, imgdel = 0;

                DirectoryInfo dir = new DirectoryInfo(AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "data/record/");
                FileSystemInfo[] files = dir.GetFileSystemInfos();
                for (int i = 0; i < files.Length; i++)
                {
                    FileInfo file = files[i] as FileInfo;
                    //是文件
                    if (file != null)
                    {
                        file.Delete();
                        records++;
                    }
                }

                DirectoryInfo imgdir = new DirectoryInfo(AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "data/image/");
                FileSystemInfo[] imgfiles = imgdir.GetFileSystemInfos();
                for (int i = 0; i < imgfiles.Length; i++)
                {
                    FileInfo file = imgfiles[i] as FileInfo;
                    //是文件
                    if (file != null)
                    {
                        string img = file.Name.Substring(0,file.Name.IndexOf("."));
                        bool match = false;
                        imageall++;
                        foreach(string group in Tools.GetGroupList())
                        {
                            if (XmlSolve.IsAnswer(group, img))
                            {
                                match = true;
                                break;
                            }
                        }
                        if (XmlSolve.IsAnswer("common", img))
                            match = true;
                        if (!match)
                        {
                            imgdel++;
                            file.Delete();
                        }
                    }
                }

                _m.SendGroupMessage(Tools.mainGroupNumber, "任务执行完毕\r\n共删除" + records + "个语音文件\r\n"
                    + "删除" + imageall + "张图片中的" + imgdel + "张");
            }
        }
    }
}
