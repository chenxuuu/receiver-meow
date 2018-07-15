using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newbe.Mahua.MahuaEvents;
using Newbe.Mahua.Receiver.Meow.MahuaApis;
using Newbe.Mahua.Receiver.Meow;

namespace Newbe.Mahua.Receiver.Meow.MahuaApis
{
    class MessageSolve
    {
        private static string prem = "你没有权限调教接待喵，权限获取方法请去问开发者";
        public static string GetReplay(string fromqq,string msg, IMahuaApi _mahuaApi, string fromgroup = "common")
        {
            if (Tools.MessageControl(5))  //限制消息速率
                return "";
            string result = "";
            if (msg == "赞我" || msg == "点赞")
            {
                for (int i = 0; i < 50; i++)
                    _mahuaApi.SendLike(fromqq);
                result += Tools.At(fromqq) + "已为你点赞50次，一天只能点赞50次哦~";
            }
            else if (msg.ToUpper() == "HELP" || msg == "帮助" || msg == "菜单")
            {
                result += "命令帮助：\r\n！add 词条：回答\r\n！del 词条：回答\r\n！list 词条\r\n！delall 词条\r\n" +
                    "所有符号均为中文全角符号\r\n" +
                    "发送“坷垃金曲”+数字序号即可点金坷垃歌（如坷垃金曲21，最大71）\r\n" +
                    "发送“点赞”可使接待给你点赞\r\n" +
                    "发送“今日运势”可以查看今日运势\r\n" +
                    "发送“查快递”和单号即可搜索快递物流信息\r\n" +
                    "发送“空气质量”可查询当前时间的空气质量\r\n" +
                    "发送“宠物助手”可查询QQ宠物代挂的帮助信息\r\n" +
                    "发送“复读”+百分比可更改复读概率\r\n" +
                    "每秒最多响应5条消息\r\n" +
                    "如有建议请到https://git.io/fNmBc反馈，欢迎star";
            }
            else if (msg == "宠物助手")
            {
                result += "宠物助手：\r\n" +
                    "发送“宠物状态”可查看宠物状态\r\n" +
                    "发送“宠物资料”可查看宠物详细资料\r\n" +
                    "发送“宠物喂养”加页码数可查看宠物喂养物品列表\r\n" +
                    "发送“宠物清洁”加页码数可查看宠物清洁物品列表\r\n" +
                    "发送“宠物治疗”加页码数可查看宠物药物物品列表\r\n" +
                    "发送“宠物使用”加物品代码可使用宠物物品\r\n" +
                    "发送“宠物学习开启”可开启自动学习\r\n" +
                    "发送“宠物学习关闭”可关闭自动学习\r\n" +
                    "发送“宠物解绑”可解除绑定，停止代挂\r\n" +
                    "挂机时会自动喂养与清洗，并且自动种菜收菜\r\n" +
                    "测试功能，如有bug请反馈";
            }
            else if (msg.IndexOf("宠物") == 0)
            {
                //获取uin和skey
                string uin = XmlSolve.replay_get("qq_pet_uin", fromqq);
                string skey = XmlSolve.replay_get("qq_pet_skey", fromqq);
                if (msg == "宠物状态")
                {
                    result += Tools.At(fromqq) + "\r\n" + QQPet.GetPetState(uin, skey);
                }
                else if (msg == "宠物资料")
                {
                    result += Tools.At(fromqq) + "\r\n" + QQPet.GetPetMore(uin, skey);
                }
                else if (msg.IndexOf("宠物喂养") == 0)
                {
                    result += Tools.At(fromqq) + "\r\n" + QQPet.FeedPetSelect(uin, skey, msg.Replace("宠物喂养", ""));
                }
                else if (msg.IndexOf("宠物清洁") == 0)
                {
                    result += Tools.At(fromqq) + "\r\n" + QQPet.WashPetSelect(uin, skey, msg.Replace("宠物清洁", ""));
                }
                else if (msg.IndexOf("宠物治疗") == 0)
                {
                    result += Tools.At(fromqq) + "\r\n" + QQPet.CurePetSelect(uin, skey, msg.Replace("宠物治疗", ""));
                }
                else if (msg.IndexOf("宠物使用") == 0)
                {
                    result += Tools.At(fromqq) + "\r\n" + QQPet.UsePet(uin, skey, msg.Replace("宠物使用", ""));
                }
                else if (msg == "宠物学习开启")
                {
                    XmlSolve.del("qq_pet_study", fromqq);
                    XmlSolve.insert("qq_pet_study", fromqq, "0");
                    result += Tools.At(fromqq) + "\r\n" + "已开启自动学习（自动上课与换课程）";
                }
                else if (msg == "宠物学习关闭")
                {
                    XmlSolve.del("qq_pet_study", fromqq);
                    XmlSolve.insert("qq_pet_study", fromqq, "28");
                    result += Tools.At(fromqq) + "\r\n" + "已关闭自动学习（学习完后不会自动继续学）";
                }
                else if (msg == "宠物解绑")
                {
                    XmlSolve.del("qq_pet_study", fromqq);
                    XmlSolve.del("qq_pet_study", fromqq);
                    result += Tools.At(fromqq) + "\r\n" + "\r\n解绑成功！";
                }
                else if (msg == "宠物绑定方法")
                {
                    result += Tools.At(fromqq) + "\r\n" + "[CQ:image,file=7CE7991F3D714978606B41C816FBC549.jpg]";
                }
            }
            else if (msg.IndexOf("坷垃金曲") == 0)
            {
                int song = 0;
                try
                {
                    song = int.Parse(msg.Replace("坷垃金曲", ""));
                }
                catch { }
                if (song >= 1 && song <= 71)
                {
                    result += "[CQ:record,file=CoolQ 语音时代！\\坷垃金曲\\" + song.ToString().PadLeft(3, '0') + ".mp3]";
                }
                else
                {
                    result += Tools.At(fromqq) + "编号不对哦，编号只能是1-71";
                }
            }
            else if (msg.IndexOf("！list ") == 0)
            {
                result += string.Format("当前词条回复如下：\r\n{0}\r\n全局词库内容：\r\n{1}",
                                        XmlSolve.list_get(fromgroup, msg.Replace("！list ", "")),
                                        XmlSolve.list_get("common", msg.Replace("！list ", "")));
            }
            else if (msg.IndexOf("！add ") == 0)
            {
                if ((XmlSolve.AdminCheck(fromqq) >= 1 && fromgroup != "common") || (fromgroup == "common" && fromqq == "961726194"))
                {
                    string get_msg = msg.Replace("！add ", ""), tmsg = "", tans = "";

                    if (get_msg.IndexOf("：") >= 1 && get_msg.IndexOf("：") != get_msg.Length - 1)
                    {
                        string[] str2;
                        int count_temp = 0;
                        str2 = get_msg.Split('：');
                        foreach (string i in str2)
                        {
                            if (count_temp == 0)
                            {
                                tmsg = i.ToString();
                                count_temp++;
                            }
                            else
                            {
                                tans += i.ToString();
                            }
                        }
                        XmlSolve.insert(fromgroup, tmsg, tans);
                        result += "添加完成！\r\n词条：" + tmsg + "\r\n回答为：" + tans;
                    }
                    else
                    {
                        result += "格式错误！";
                    }
                }
                else
                {
                    result += prem;
                }
            }
            else if (msg.IndexOf("！del ") == 0)
            {
                if ((XmlSolve.AdminCheck(fromqq) >= 1 && fromgroup != "common") || (fromgroup == "common" && fromqq == "961726194"))
                {
                    string get_msg = msg.Replace("！del ", ""), tmsg = "", tans = "";
                    if (get_msg.IndexOf("：") >= 1 && get_msg.IndexOf("：") != get_msg.Length - 1)
                    {
                        string[] str2;
                        int count_temp = 0;
                        str2 = get_msg.Split('：');
                        foreach (string i in str2)
                        {
                            if (count_temp == 0)
                            {
                                tmsg = i.ToString();
                                count_temp++;
                            }
                            else
                            {
                                tans += i.ToString();
                            }
                        }
                        XmlSolve.remove(fromgroup, tmsg, tans);
                        result += "删除完成！\r\n词条：" + tmsg + "\r\n回答为：" + tans;
                    }
                    else
                    {
                        result += "格式错误！";
                    }
                }
                else
                {
                    result += prem;
                }
            }
            else if (msg.IndexOf("！delall ") == 0)
            {
                if ((XmlSolve.AdminCheck(fromqq) >= 1 && fromgroup != "common") || (fromgroup == "common" && fromqq == "961726194"))
                {
                    string get_msg = msg.Replace("！delall ", "");
                    if (get_msg.Length > 0)
                    {
                        XmlSolve.del(fromgroup, get_msg);
                        result += "删除完成！\r\n触发词：" + get_msg;
                    }
                    else
                    {
                        result += "格式错误！";
                    }
                }
                else
                {
                    result += prem;
                }
            }
            else if (msg.IndexOf("[CQ:hb,title=") != -1 && msg.IndexOf("]") != -1 && fromgroup == "common")
            {
                XmlSolve.insert("admin_list", "给我列一下狗管理", fromqq);
                result += "已给予" + fromqq + "词条编辑权限。";
            }
            else if (msg.IndexOf("！addadmin ") == 0 && fromqq == "961726194")
            {
                XmlSolve.insert("admin_list", "给我列一下狗管理", msg.Replace("！addadmin ", ""));
                result += "已添加一位狗管理";
            }
            else if (msg.IndexOf("！deladmin ") == 0 && fromqq == "961726194")
            {
                XmlSolve.remove("admin_list", "给我列一下狗管理", msg.Replace("！deladmin ", ""));
                result += "已删除一位狗管理";
            }
            else if (msg == "给我列一下狗管理")
            {
                result += "当前狗管理如下：\r\n" + XmlSolve.list_get("admin_list", "给我列一下狗管理");
            }
            else if (msg == "今日黄历" || msg == "今日运势" || msg == "今天运势" || msg == "今天黄历")
            {
                result += TodaysAlmanac.GetAlmanac(fromqq, DateTime.Now.DayOfYear);
            }
            else if (msg == "昨日黄历" || msg == "昨日运势" || msg == "昨天运势" || msg == "昨天黄历")
            {
                result += TodaysAlmanac.GetAlmanac(fromqq, DateTime.Now.DayOfYear - 1);
            }
            else if (msg == "明日黄历" || msg == "明日运势" || msg == "明天运势" || msg == "明天黄历")
            {
                result += TodaysAlmanac.GetAlmanac(fromqq, DateTime.Now.DayOfYear + 1);
            }
            else if (msg == "抽奖" && fromgroup != "common")
            {
                result += LotteryEvent.Lottery(fromqq, _mahuaApi, fromgroup);
            }
            else if (msg == "禁言卡")
            {
                result += LotteryEvent.GetBanCard(fromqq);
            }
            else if (msg.IndexOf("禁言") == 0 && msg.Length > 2 && fromgroup != "common")
            {
                result += LotteryEvent.BanSomebody(fromqq, Tools.GetNumber(msg), fromgroup, _mahuaApi);
            }
            else if (msg.IndexOf("解禁") == 0 && msg.Length > 2 && fromgroup != "common")
            {
                result += LotteryEvent.UnbanSomebody(fromqq, Tools.GetNumber(msg), fromgroup, _mahuaApi);
            }
            else if (msg.IndexOf("查快递") == 0)
            {
                result += Tools.GetExpress(Tools.GetNumber(msg), fromqq);
            }
            else if (msg == "开车")
            {
                result += "magnet:?xt=urn:btih:" + Tools.GetRandomString(40, true, false, false, false, "ABCDEF");
            }
            else if (msg.IndexOf("空气质量") == 0)
            {
                result += Tools.GetAir(msg, fromqq);
            }
            else if (msg.IndexOf("cmd ") == 0 && fromqq == "961726194")
            {
                result += Tools.execCMD(msg.Replace("cmd ", ""));
            }
            else if (msg.IndexOf("复读") == 0 && fromgroup != "common")
            {
                if (XmlSolve.AdminCheck(fromqq) >= 1)
                    result += Tools.At(fromqq) + Tools.SetRepeat(Tools.GetNumber(msg), fromgroup);
                else
                    result += prem;

            }
            else if (Tools.GetRepeat(fromgroup))
            {
                result += msg;
            }
            else
            {
                result += XmlSolve.ReplayGroupStatic(fromgroup, msg);
            }
            return result;
        }
    }
}
