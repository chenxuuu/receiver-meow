using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Newbe.Mahua.Receiver.Meow.MahuaApis
{
    class LotteryEvent
    {
        /// <summary>
        /// 抽奖方法，获取抽奖结果并执行禁言
        /// </summary>
        /// <param name="qq"></param>
        /// <param name="_mahuaApi"></param>
        /// <param name="group"></param>
        /// <returns></returns>
        public static string Lottery(string qq, IMahuaApi _mahuaApi,string group)
        {
            string result = "";
            int need_add = 0;
            Random ran = new Random(System.DateTime.Now.Millisecond);
            int RandKey = ran.Next(1, 22);
            int RandKey2 = ran.Next(0, 10);
            if (RandKey > 12)
            {
                result += Tools.At(qq) + "\r\n恭喜你！什么也没有抽中！";
            }
            else if (RandKey == 1 && RandKey2 != 0)
            {
                need_add += 10;
                TimeSpan span = new TimeSpan(0, 10, 0, 0);
                _mahuaApi.BanGroupMember(group, qq, span);
                result += Tools.At(qq) + "\r\n恭喜你抽中了超豪华禁言套餐，并附赠10张禁言卡！奖励已发放！";
            }
            else if (RandKey == 1 && RandKey2 == 0)
            {
                need_add += 200;
                TimeSpan span = new TimeSpan(30, 0, 0, 0);
                _mahuaApi.BanGroupMember(group, qq, span);
                result += Tools.At(qq) + "\r\n恭喜你抽中了顶级豪华月卡禁言套餐，并附赠200张禁言卡！奖励已发放！";

            }
            else if (RandKey < 11)
            {
                TimeSpan span = new TimeSpan(0, RandKey, 0, 0);
                _mahuaApi.BanGroupMember(group, qq, span);
                result += Tools.At(qq) + "\r\n恭喜你抽中了禁言" + RandKey + "小时！奖励已发放到你的QQ~";
            }
            else
            {
                need_add += 1;
                result += Tools.At(qq) + "\r\n恭喜你抽中了一张禁言卡，回复“禁言卡”可以查看使用帮助。";
            }

            if(need_add != 0)
            {
                int fk = 0;
                string fks = XmlSolve.xml_get("ban_card", qq);
                if (fks != "")
                {
                    fk = int.Parse(fks);
                }
                fk += need_add;
                XmlSolve.del("ban_card", qq);
                XmlSolve.insert("ban_card", qq, fk.ToString());
            }
            return result;
        }

        /// <summary>
        /// 获取禁言卡数量方法
        /// </summary>
        /// <param name="qq"></param>
        /// <returns></returns>
        public static string GetBanCard(string qq)
        {
            int fk = 0;
            string fks = XmlSolve.xml_get("ban_card", qq);
            if (fks != "")
            {
                fk = int.Parse(fks);
            }
            return Tools.At(qq) +
            "\r\n禁言卡可用于禁言或解禁他人，如果接待权限足够。\r\n" +
            "使用方法：发送禁言或解禁加上@那个人\r\n" +
            "禁言时长将为1分钟-10分钟随机\r\n" +
            "获取方式：抽奖时有十分之一的概率获得\r\n" +
            "你当前剩余的禁言卡数量：" +
            fk.ToString();
        }

        /// <summary>
        /// 禁言某人
        /// </summary>
        /// <param name="fromqq"></param>
        /// <param name="banqq"></param>
        /// <param name="group"></param>
        /// <param name="_mahuaApi"></param>
        /// <returns></returns>
        public static string BanSomebody(string fromqq, string banqq, string group, IMahuaApi _mahuaApi)
        {
            int fk = 0;
            string fks = XmlSolve.xml_get("ban_card", fromqq);
            if (fks != "")
            {
                fk = int.Parse(fks);
            }
            if (fk > 0)
            {
                try
                {
                    Random ran = new Random(System.DateTime.Now.Millisecond);
                    int RandKey = ran.Next(0, 60);
                    TimeSpan span = new TimeSpan(0, 0, RandKey, 0);
                    _mahuaApi.BanGroupMember(group, banqq, span);
                    fk--;
                    XmlSolve.del("ban_card", fromqq);
                    XmlSolve.insert("ban_card", fromqq, fk.ToString());

                    return Tools.At(fromqq) + "\r\n已将" + banqq + "禁言" + RandKey + "分钟";
                }
                catch
                {
                    return Tools.At(fromqq) + "\r\n执行失败。";
                }
            }
            else
            {
                return Tools.At(fromqq) + "\r\n你哪儿有禁言卡？";
            }
        }

        /// <summary>
        /// 解禁某人
        /// </summary>
        /// <param name="fromqq"></param>
        /// <param name="banqq"></param>
        /// <param name="group"></param>
        /// <param name="_mahuaApi"></param>
        /// <returns></returns>
        public static string UnbanSomebody(string fromqq, string banqq, string group, IMahuaApi _mahuaApi)
        {
            int fk = 0;
            string fks = XmlSolve.xml_get("ban_card", fromqq);
            if (fks != "")
            {
                fk = int.Parse(fks);
            }
            if (fk > 0)
            {
                try
                {
                    TimeSpan span = new TimeSpan(0, 0, 0, 0);
                    _mahuaApi.BanGroupMember(group, banqq, span);
                    fk--;
                    XmlSolve.del("ban_card", fromqq);
                    XmlSolve.insert("ban_card", fromqq, fk.ToString());

                    return Tools.At(fromqq) + "\r\n已将" + banqq + "解除禁言";
                }
                catch
                {
                    return Tools.At(fromqq) + "\r\n执行失败。";
                }
            }
            else
            {
                return Tools.At(fromqq) + "\r\n你哪儿有禁言卡？";
            }
        }
    }
}
