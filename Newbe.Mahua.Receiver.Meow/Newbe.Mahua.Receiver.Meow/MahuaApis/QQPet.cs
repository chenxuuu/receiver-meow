using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Newbe.Mahua.Receiver.Meow.MahuaApis
{
    class QQPet
    {
        public static string pleaseLogin = "信息获取失败，请重新发送再试\r\n如果没有绑定宠物信息，请绑定！\r\n绑定方法请回复“宠物绑定方法”查看";
        public static string petMore = "更多宠物命令请回复“宠物助手”";

        /// <summary>
        /// 获取宠物基本信息
        /// </summary>
        /// <param name="uin"></param>
        /// <param name="skey"></param>
        /// <returns></returns>
        public static string GetPetState(string uin, string skey)
        {
            string result = "";
            string html = Tools.HttpGetPet("http://qqpet.wapsns.3g.qq.com/qqpet/fcgi-bin/phone_pet", "", uin, skey);
            html = html.Replace("\r", "");
            html = html.Replace("\n", "");
            if (html.IndexOf("手机统一登录") != -1 || html == "")
                return pleaseLogin;
            result += Tools.Reg_get(html, "alt=\"QQ宠物\"/></p><p>(?<say>.*?)<", "say").Replace(" ", "") + "\r\n"
                    + "宠物名：" + Tools.Reg_get(html, "昵称：(?<level>.*?)<", "level").Replace(" ", "") + "\r\n"
                    + "等级：" + Tools.Reg_get(html, "等级：(?<level>.*?)<", "level").Replace(" ", "") + "\r\n"
                    + "状态：" + Tools.Reg_get(html, "状态：(?<now>.*?)<", "now").Replace(" ", "") + "\r\n"
                    + "成长值：" + Tools.Reg_get(html, "成长值：(?<now>.*?)<", "now").Replace(" ", "") + "\r\n"
                    + "饥饿值：" + Tools.Reg_get(html, "饥饿值：(?<now>.*?)<", "now").Replace(" ", "") + "\r\n"
                    + "清洁值：" + Tools.Reg_get(html, "清洁值：(?<now>.*?)<", "now").Replace(" ", "") + "\r\n"
                    + "心情：" + Tools.Reg_get(html, "心情：(?<now>.*?)<", "now").Replace(" ", "") + "\r\n" + petMore;
            return result;
        }

        /// <summary>
        /// 获取宠物详细信息
        /// </summary>
        /// <param name="uin"></param>
        /// <param name="skey"></param>
        /// <returns></returns>
        public static string GetPetMore(string uin, string skey)
        {
            string result = "";
            string html = Tools.HttpGetPet("http://qqpet.wapsns.3g.qq.com/qqpet/fcgi-bin/phone_pet", "cmd=2", uin, skey);
            html = html.Replace("\r", "");
            html = html.Replace("\n", "");
            if (html.IndexOf("手机统一登录") != -1 || html == "")
                return pleaseLogin;
            result += "主人昵称：" + Tools.Reg_get(html, "主人昵称：(?<name>.*?)&nbsp;", "name").Replace(" ", "") + "\r\n"
                    + "宠物性别：" + Tools.Reg_get(html, "宠物性别：(?<level>.*?)<", "level").Replace(" ", "") + "\r\n"
                    + "生日：" + Tools.Reg_get(html, "生日：(?<now>.*?)<", "now").Replace(" ", "") + "\r\n"
                    + "等级：" + Tools.Reg_get(html, "等级：(?<now>.*?)<", "now").Replace(" ", "") + "\r\n"
                    + "年龄：" + Tools.Reg_get(html, "年龄：(?<now>.*?)<", "now").Replace(" ", "") + "\r\n"
                    + "代数：" + Tools.Reg_get(html, "代数：(?<now>.*?)<", "now").Replace(" ", "") + "\r\n"
                    + "成长值：" + Tools.Reg_get(html, "成长值：(?<now>.*?)<", "now").Replace(" ", "") + "\r\n"
                    + "武力值：" + Tools.Reg_get(html, "武力值：(?<now>.*?)<", "now").Replace(" ", "") + "\r\n"
                    + "智力值：" + Tools.Reg_get(html, "智力值：(?<now>.*?)<", "now").Replace(" ", "") + "\r\n"
                    + "魅力值：" + Tools.Reg_get(html, "魅力值：(?<now>.*?)<", "now").Replace(" ", "") + "\r\n"
                    + "活跃度：" + Tools.Reg_get(html, "活跃度：(?<now>.*?)<", "now").Replace(" ", "") + "\r\n"
                    + "配偶：" + Tools.Reg_get(html, "配偶：(?<now>.*?)<", "now").Replace(" ", "") + "\r\n"
                    + petMore;
            return result;
        }

        /// <summary>
        /// 喂养宠物选择页面
        /// </summary>
        /// <param name="uin"></param>
        /// <param name="skey"></param>
        /// <returns></returns>
        public static string FeedPetSelect(string uin, string skey, string page)
        {
            string result = "";
            string html = Tools.HttpGetPet("http://qqpet.wapsns.3g.qq.com/qqpet/fcgi-bin/phone_pet", "cmd=3&page=" + page, uin, skey);
            html = html.Replace("\r", "");
            html = html.Replace("\n", "");
            if (html.IndexOf("手机统一登录") != -1 || html == "")
                return pleaseLogin;
            result += "我的账户：" + Tools.Reg_get(html, "我的账户：(?<name>.*?)<", "name").Replace(" ", "") + "\r\n";
            int i = 0;
            MatchCollection matchs = Tools.Reg_solve(Tools.Reg_get(html, "元宝(?<name>.*?)上页", "name").Replace(" ", ""), "<p>(?<name>.*?)<br");
            string[] name = new string[matchs.Count];
            string[] id = new string[matchs.Count];
            foreach (Match item in matchs)
            {
                if (item.Success)
                {
                    name[i++] += item.Groups["name"].Value;
                }
            }
            i = 0;
            matchs = Tools.Reg_solve(Tools.Reg_get(html, "元宝(?<name>.*?)上页", "name").Replace(" ", ""), "goodid=(?<id>.*?)\"");
            foreach (Match item in matchs)
            {
                if (item.Success)
                {
                    id[i++] += "物品id：" + item.Groups["id"].Value;
                }
            }

            for (int j = 0; j < i; j++)
            {
                result += name[j] + "\r\n" + id[j] + "\r\n";
            }
            if (Tools.Reg_get(html, "上页</a>(?<name>.*?)/", "name").Replace(" ", "") != "")
            {
                result += "第" + Tools.Reg_get(html, "上页</a>(?<name>.*?)/", "name").Replace(" ", "") + "页，共"
                       + Tools.Reg_get(html, "上页</a>(.*?)/(?<name>\\d+)", "name").Replace(" ", "") + "页" + "\r\n";
            }
            else
            {
                result += "第" + Tools.Reg_get(html, "上页(?<name>.*?)/", "name").Replace(" ", "") + "页，共"
                        + Tools.Reg_get(html, "上页(.*?)/(?<name>\\d+)", "name").Replace(" ", "") + "页" + "\r\n";
            }


            return result + petMore;
        }

        /// <summary>
        /// 清洁宠物选择页面
        /// </summary>
        /// <param name="uin"></param>
        /// <param name="skey"></param>
        /// <returns></returns>
        public static string WashPetSelect(string uin, string skey, string page)
        {
            string result = "";
            string html = Tools.HttpGetPet("http://qqpet.wapsns.3g.qq.com/qqpet/fcgi-bin/phone_pet", "cmd=3&page=" + page + "&eatwash=1", uin, skey);
            html = html.Replace("\r", "");
            html = html.Replace("\n", "");
            if (html.IndexOf("手机统一登录") != -1 || html == "")
                return pleaseLogin;
            result += "我的账户：" + Tools.Reg_get(html, "我的账户：(?<name>.*?)<", "name").Replace(" ", "") + "\r\n";
            int i = 0;
            MatchCollection matchs = Tools.Reg_solve(Tools.Reg_get(html, "元宝(?<name>.*?)上页", "name").Replace(" ", ""), "<p>(?<name>.*?)<br");
            string[] name = new string[matchs.Count];
            string[] id = new string[matchs.Count];
            foreach (Match item in matchs)
            {
                if (item.Success)
                {
                    name[i++] += item.Groups["name"].Value;
                }
            }
            i = 0;
            matchs = Tools.Reg_solve(Tools.Reg_get(html, "元宝(?<name>.*?)上页", "name").Replace(" ", ""), "goodid=(?<id>.*?)\"");
            foreach (Match item in matchs)
            {
                if (item.Success)
                {
                    id[i++] += "物品id：" + item.Groups["id"].Value;
                }
            }

            for (int j = 0; j < i; j++)
            {
                result += name[j] + "\r\n" + id[j] + "\r\n";
            }
            if (Tools.Reg_get(html, "上页</a>(?<name>.*?)/", "name").Replace(" ", "") != "")
            {
                result += "第" + Tools.Reg_get(html, "上页</a>(?<name>.*?)/", "name").Replace(" ", "") + "页，共"
                       + Tools.Reg_get(html, "上页</a>(.*?)/(?<name>\\d+)", "name").Replace(" ", "") + "页" + "\r\n";
            }
            else
            {
                result += "第" + Tools.Reg_get(html, "上页(?<name>.*?)/", "name").Replace(" ", "") + "页，共"
                        + Tools.Reg_get(html, "上页(.*?)/(?<name>\\d+)", "name").Replace(" ", "") + "页" + "\r\n";
            }

            return result + petMore;
        }

        /// <summary>
        /// 治疗宠物选择页面
        /// </summary>
        /// <param name="uin"></param>
        /// <param name="skey"></param>
        /// <returns></returns>
        public static string CurePetSelect(string uin, string skey, string page)
        {
            string result = "";
            string html = Tools.HttpGetPet("http://qqpet.wapsns.3g.qq.com/qqpet/fcgi-bin/phone_pet", "cmd=3&page=" + page + "&eatwash=2", uin, skey);
            html = html.Replace("\r", "");
            html = html.Replace("\n", "");
            if (html.IndexOf("手机统一登录") != -1 || html == "")
                return pleaseLogin;
            result += "我的账户：" + Tools.Reg_get(html, "我的账户：(?<name>.*?)<", "name").Replace(" ", "") + "\r\n";
            int i = 0;
            MatchCollection matchs = Tools.Reg_solve(Tools.Reg_get(html, "元宝(?<name>.*?)上页", "name").Replace(" ", ""), "<p>(?<name>.*?)<br");
            string[] name = new string[matchs.Count];
            string[] id = new string[matchs.Count];
            foreach (Match item in matchs)
            {
                if (item.Success)
                {
                    name[i++] += item.Groups["name"].Value;
                }
            }
            i = 0;
            matchs = Tools.Reg_solve(Tools.Reg_get(html, "元宝(?<name>.*?)上页", "name").Replace(" ", ""), "goodid=(?<id>.*?)\"");
            foreach (Match item in matchs)
            {
                if (item.Success)
                {
                    id[i++] += "物品id：" + item.Groups["id"].Value;
                }
            }

            for (int j = 0; j < i; j++)
            {
                result += name[j] + "\r\n" + id[j] + "\r\n";
            }
            if (Tools.Reg_get(html, "上页</a>(?<name>.*?)/", "name").Replace(" ", "") != "")
            {
                result += "第" + Tools.Reg_get(html, "上页</a>(?<name>.*?)/", "name").Replace(" ", "") + "页，共"
                       + Tools.Reg_get(html, "上页</a>(.*?)/(?<name>\\d+)", "name").Replace(" ", "") + "页" + "\r\n";
            }
            else
            {
                result += "第" + Tools.Reg_get(html, "上页(?<name>.*?)/", "name").Replace(" ", "") + "页，共"
                        + Tools.Reg_get(html, "上页(.*?)/(?<name>\\d+)", "name").Replace(" ", "") + "页" + "\r\n";
            }

            return result + petMore;
        }

        /// <summary>
        /// 使用宠物物品
        /// </summary>
        /// <param name="uin"></param>
        /// <param name="skey"></param>
        /// <returns></returns>
        public static string UsePet(string uin, string skey, string goodid)
        {
            string result = "";
            string html = Tools.HttpGetPet("http://qqpet.wapsns.3g.qq.com/qqpet/fcgi-bin/phone_pet", "cmd=3&feed=3&goodid=" + goodid, uin, skey);
            html = html.Replace("\r", "");
            html = html.Replace("\n", "");
            if (html.IndexOf("手机统一登录") != -1 || html == "")
                return pleaseLogin;
            if (html.IndexOf("不合法") != -1)
                return "你没有这个东西，使用失败\r\n" + petMore;
            result += "物品使用结果：\r\n" + Tools.Reg_get(html, "值：(?<name>.*?)<br", "name").Replace(" ", "").Replace("</b>", "") + "\r\n"
                    + petMore;
            return result;
        }

        /// <summary>
        /// 宠物学习选择页面
        /// </summary>
        /// <param name="uin"></param>
        /// <param name="skey"></param>
        /// <returns></returns>
        public static string StudyPetSelect()
        {
            return "课程id：\r\n小学体育：1\r\n小学劳技：2\r\n小学武术：5\r\n小学语文：4\r\n小学数学：5\r\n小学政治：6\r\n小学美术：7\r\n小学音乐：8\r\n小学礼仪：9\r\n\r\n中学体育：10\r\n中学劳技：11\r\n中学武术：12\r\n中学语文：13\r\n中学数学：14\r\n中学政治：15\r\n中学美术：16\r\n中学音乐：17\r\n中学礼仪：18\r\n\r\n大学体育：19\r\n大学劳技：20\r\n大学武术：21\r\n大学语文：22\r\n大学数学：23\r\n大学政治：24\r\n大学美术：25\r\n大学音乐：26\r\n大学礼仪：27\r\n" + petMore;
        }

        /// <summary>
        /// 学习宠物课程
        /// </summary>
        /// <param name="uin"></param>
        /// <param name="skey"></param>
        /// <returns></returns>
        public static string StudyPet(string uin, string skey, string classid)
        {
            string result = "";
            string html = Tools.HttpGetPet("http://qqpet.wapsns.3g.qq.com/qqpet/fcgi-bin/phone_pet", "cmd=5&courseid=" + classid + "&study=2", uin, skey);
            html = html.Replace("\r", "");
            html = html.Replace("\n", "");
            if (html.IndexOf("手机统一登录") != -1 || html == "")
                return pleaseLogin;
            if (html.IndexOf("亲爱的") != -1)
            {
                result += Tools.Reg_get(html, "同学：(?<name>.*?)<", "name").Replace(" ", "") + "\r\n";
            }
            if (html.IndexOf("您已经完成了") != -1)
            {
                result += "您已经完成了" + Tools.Reg_get(html, "您已经完成了(?<name>.*?)<", "name").Replace(" ", "") + "\r\n";
            }
            if (html.IndexOf("你的宠物") != -1)
            {
                result += "你的宠物" + Tools.Reg_get(html, "你的宠物(?<name>.*?)<", "name").Replace(" ", "") + "\r\n";
            }
            return "课程学习结果：\r\n" + result + petMore;
        }
    }
}
