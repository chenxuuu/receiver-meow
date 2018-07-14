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
        public static string GetReplay(string fromqq,string msg, IMahuaApi _mahuaApi, string fromgroup = "common")
        {
            string result = "";
            if (msg == "赞我" || msg == "点赞")
            {
                for (int i = 0; i < 10; i++)
                    _mahuaApi.SendLike(fromqq);
                result += Tools.At(fromqq) + "已为你点赞十次，一天只能点赞十次哦~";
            }
            else if (msg.ToUpper() == "HELP" || msg == "帮助")
            {
                result += "命令帮助：\r\n！add 词条：回答\r\n！del 词条：回答\r\n！list 词条\r\n" +
                    "所有符号均为全角符号\r\n词条中请勿包含冒号\r\n" +
                    "发送“坷垃金曲”+数字序号即可点金坷垃歌（如坷垃金曲21，最大71）\r\n" +
                    "私聊发送“赞我”可使接待给你点赞\r\n" +
                    "发送“今日运势”可以查看今日运势\r\n" +
                    "发送“淘宝”+关键词即可搜索淘宝优惠搜索结果\r\n" +
                    "发送“pixel”可以查看像素游戏图片\r\n" +
                    "发送“查快递”和单号即可搜索快递物流信息\r\n" +
                    "发送“网易云”和歌曲id号/歌曲名即可定向点歌\r\n" +
                    "发送“正则”+字符串+“换行”+正则表达式，可查询C#正则\r\n" +
                    "发送“空气质量”可查询当前时间的空气质量\r\n" +
                    "发送“宠物助手”可查询QQ宠物代挂的帮助信息\r\n" +
                    "发送“查磁链”+“关键词”可查询磁链\r\n" +
                    "如有建议请到https://github.com/chenxuuu/receiver-meow/issues反馈";
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
            if (msg.IndexOf("宠物") == 0)
            {
                //获取uin和skey
                string uin = XmlSolve.replay_get("qq_pet_uin", fromqq);
                string skey = XmlSolve.replay_get("qq_pet_skey", fromqq);
                if (msg == "宠物状态")
                {
                    result += Tools.At(fromqq) +  "\r\n" + QQPet.GetPetState(uin, skey);
                }
                else if (msg == "宠物资料")
                {
                    result += Tools.At(fromqq) +  "\r\n" + QQPet.GetPetMore(uin, skey);
                }
                else if (msg.IndexOf("宠物喂养") == 0)
                {
                    result += Tools.At(fromqq) +  "\r\n" + QQPet.FeedPetSelect(uin, skey, msg.Replace("宠物喂养", ""));
                }
                else if (msg.IndexOf("宠物清洁") == 0)
                {
                    result += Tools.At(fromqq) +  "\r\n" + QQPet.WashPetSelect(uin, skey, msg.Replace("宠物清洁", ""));
                }
                else if (msg.IndexOf("宠物治疗") == 0)
                {
                    result += Tools.At(fromqq) +  "\r\n" + QQPet.CurePetSelect(uin, skey, msg.Replace("宠物治疗", ""));
                }
                else if (msg.IndexOf("宠物使用") == 0)
                {
                    result += Tools.At(fromqq) +  "\r\n" + QQPet.UsePet(uin, skey, msg.Replace("宠物使用", ""));
                }
                else if (msg == "宠物学习开启")
                {
                    XmlSolve.del("qq_pet_study", fromqq);
                    XmlSolve.insert("qq_pet_study", fromqq, "0");
                    result += Tools.At(fromqq) +  "\r\n" + "已开启自动学习（自动上课与换课程）";
                }
                else if (msg == "宠物学习关闭")
                {
                    XmlSolve.del("qq_pet_study", fromqq);
                    XmlSolve.insert("qq_pet_study", fromqq, "28");
                    result += Tools.At(fromqq) +  "\r\n" + "已关闭自动学习（学习完后不会自动继续学）";
                }
                else if (msg == "宠物解绑")
                {
                    XmlSolve.del("qq_pet_study", fromqq);
                    XmlSolve.del("qq_pet_study", fromqq);
                    result += Tools.At(fromqq) +  "\r\n" + "\r\n解绑成功！";
                }
                else if (msg == "宠物绑定方法")
                {
                    result += Tools.At(fromqq) +  "\r\n" + "[CQ:image,file=7CE7991F3D714978606B41C816FBC549.jpg]";
                }
            }
            else
            {
                result += XmlSolve.ReplayGroupStatic(fromgroup, msg);
            }
            return result;
        }
    }
}
