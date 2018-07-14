using Newbe.Mahua.MahuaEvents;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Newbe.Mahua.Receiver.Meow.MahuaApis;
using System.Xml.Linq;

namespace Newbe.Mahua.Receiver.Meow.MahuaEvents
{
    /// <summary>
    /// 插件初始化事件
    /// </summary>
    public class InitializationMahuaEvent
        : IInitializationMahuaEvent
    {
        private readonly IMahuaApi _mahuaApi;

        public InitializationMahuaEvent(
            IMahuaApi mahuaApi)
        {
            _mahuaApi = mahuaApi;
        }

        public void Initialized(InitializedContext context)
        {
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Enabled = true;
            timer.Interval = 1000;// 执行间隔时间, 单位为毫秒  
            timer.Start();
            timer.Elapsed += new System.Timers.ElapsedEventHandler(Timer1_Elapsed);
        }

        private void Timer1_Elapsed(object sender, System.Timers.ElapsedEventArgs e)  //定时程序
        {

            // 得到 hour minute second  如果等于某个值就开始执行某个程序。  
            int intHour = e.SignalTime.Hour;
            int intMinute = e.SignalTime.Minute;
            int intSecond = e.SignalTime.Second;

            // 十二点执行
            if (intMinute == 0 && intSecond == 10 && intHour == 0)
            {
                string qd_get = XmlSolve.xml_get("daily_sign_in_count_all", "qd");
                int qd = 0;
                try
                {
                    qd = int.Parse(qd_get);
                }
                catch { }
                XmlSolve.del("daily_sign_in_count_all", "qd");
                Random ran = new Random();
                _mahuaApi.SendGroupMessage("241464054", "新的一天已经到来了哦，现在时间是\r\n" + DateTime.Now.ToString() + "\r\n昨天一共有" + qd + "人签到哦");
                _mahuaApi.SendGroupMessage("567145439", DateTime.Now.ToString() + "\r\n昨天一共有" + qd + "人签到哦");
            }


            if (intMinute == 0 && intSecond == 10 && intHour == 3)
            {
                _mahuaApi.SendGroupMessage("567145439", "服务器备份已开始，硬盘可用空间：\r\n" +
                        "服务器盘剩余空间：" + ((float)Tools.GetHardDiskFreeSpace("D") / 1024).ToString(".00") + "GB\r\n" +
                        "备份盘剩余空间：" + ((float)Tools.GetHardDiskFreeSpace("E") / 1024).ToString(".00") + "GB");
                System.Diagnostics.Process.Start(@"D:\backup.bat");
            }
            if (intMinute == 0 && intSecond == 10 && intHour == 4)
            {
                _mahuaApi.SendGroupMessage("567145439", "服务器备份肯定已经结束了，硬盘可用空间：\r\n" +
                        "服务器盘剩余空间：" + ((float)Tools.GetHardDiskFreeSpace("D") / 1024).ToString(".00") + "GB\r\n" +
                        "备份盘剩余空间：" + ((float)Tools.GetHardDiskFreeSpace("E") / 1024).ToString(".00") + "GB");
                if (Tools.GetHardDiskFreeSpace("E") < 1024 * 10)
                {
                    _mahuaApi.SendGroupMessage("567145439", "警告：服务器备份盘可用空间仅剩余" +
                        ((float)Tools.GetHardDiskFreeSpace("E") / 1024).ToString(".00") + "G！请及时清理多余文件！");
                }
            }

            if (intMinute % 15 == 0)
            {
                XmlSolve.dircheck("qq_pet_fail_count");
                XElement uin = XElement.Load(XmlSolve.path + "qq_pet_fail_count.xml");
                foreach (XElement mm in uin.Elements("msginfo"))
                {
                    try
                    {
                        if (int.Parse(mm.Element("ans").Value) > 3)
                        {
                            _mahuaApi.SendPrivateMessage(mm.Element("msg").Value,
                                "你的宠物cookie绑定可能已过期，因为获取宠物信息失败次数已经超过3次，请检查后重新绑定");
                            XmlSolve.del("qq_pet_uin", mm.Element("msg").Value);
                            XmlSolve.del("qq_pet_skey", mm.Element("msg").Value);
                            XmlSolve.del("qq_pet_study", mm.Element("msg").Value);
                            XmlSolve.del("qq_pet_fail_count", mm.Element("msg").Value);
                        }

                    }
                    catch { }
                }
            }
        }
    }
}
