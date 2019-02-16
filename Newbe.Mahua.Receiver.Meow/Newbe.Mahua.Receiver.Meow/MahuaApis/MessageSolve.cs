using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newbe.Mahua.MahuaEvents;
using Newbe.Mahua.Receiver.Meow.MahuaApis;
using Newbe.Mahua.Receiver.Meow;
using System.Web;
using System.IO;

namespace Newbe.Mahua.Receiver.Meow.MahuaApis
{
    class MessageSolve
    {
        private static string prem = "[CQ:emoji,id=128683]你没有权限调教接待喵，权限获取方法请去问开发者";
        public static string GetReplay(string fromqq,string msg, IMahuaApi _mahuaApi, string fromgroup = "common")
        {
            if (Tools.MessageControl(5))
                return "";
            if (msg.IndexOf("！ban ") == 0 && msg.Length > 6)
            {
                if ((XmlSolve.AdminCheck(fromqq) >= 1 && fromgroup != "common") || (fromgroup == "common" && fromqq == Tools.adminNumber))
                {
                    string get_msg = msg.Replace("！ban ", "");
                    XmlSolve.insert(fromgroup, get_msg, "[ban]");
                    return "[CQ:emoji,id=128683]" + get_msg;
                }
                else
                {
                    return prem;
                }
            }
            else if (msg.IndexOf("！unban ") == 0 && msg.Length > 7)
            {
                if ((XmlSolve.AdminCheck(fromqq) >= 1 && fromgroup != "common") || (fromgroup == "common" && fromqq == Tools.adminNumber))
                {
                    string get_msg = msg.Replace("！unban ", "");
                    XmlSolve.remove(fromgroup, get_msg, "[ban]");
                    return "[CQ:emoji,id=9989]" + get_msg;
                }
                else
                {
                    return prem;
                }
            }
            if (XmlSolve.IsBaned(fromgroup, msg))
                return "";

            string result = "";
            if (msg == "赞我" || msg == "点赞")
            {
                _mahuaApi.SendLike(fromqq);
                result += Tools.At(fromqq) + "已为你点赞[CQ:emoji,id=128077]";
            }
            else if (msg.ToUpper() == "HELP" || msg == "帮助" || msg == "菜单")
            {
                result += Tools.RunLua(File.ReadAllText(AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "全局词条的lua脚本/help.lua"),
                            "fromqq = \"" + fromqq + "\"\r\n" +
                            "fromgroup = \"" + fromgroup + "\"");
            }
            else if(msg.ToUpper()=="！LUA")
            {
                result += @"[CQ:emoji,id=128295]lua功能使用帮助：
#lua 加 代码 直接运行
！luaadd 关键词：代码
！luadel 关键词
！lualist
！luasee 关键词
查看可供使用的api说明：
https://github.com/chenxuuu/receiver-meow/blob/master/lua.md";
            }
            else if (msg.IndexOf("！list ") == 0)
            {
                result += string.Format("[CQ:emoji,id=128221]当前词条回复如下：\r\n{0}\r\n全局词库内容：\r\n{1}",
                                        XmlSolve.list_get(fromgroup, msg.Replace("！list ", "")),
                                        XmlSolve.list_get("common", msg.Replace("！list ", "")));
            }
            else if (msg.IndexOf("！lualist") == 0)
            {
                result += string.Format("[CQ:emoji,id=128295]本群脚本：\r\n{0}\r\n全局脚本：\r\n{1}",
                                        XmlSolve.lua_list_get(fromgroup),
                                        XmlSolve.lua_list_get("common"));
            }
            else if (msg.IndexOf("！luaadd ") == 0 && msg.Length > 10)
            {
                if ((XmlSolve.AdminCheck(fromqq) >= 1 && fromgroup != "common") || (fromgroup == "common" && fromqq == Tools.adminNumber))
                {
                    string luaMsg = msg.Replace("！luaadd ", "");
                    int len = luaMsg.IndexOf("：");
                    if(len > 0 && luaMsg.Length > len)
                    {
                        string tmsg = luaMsg.Substring(0, len), tans = luaMsg.Substring(len + 1);
                        XmlSolve.insert(fromgroup, tmsg, "[lua]" + fromqq + "." + tans);
                        result += "[CQ:emoji,id=9989]添加完成！\r\n词条：" + tmsg + "\r\n回答为一个脚本";
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
            else if(msg.IndexOf("！luadel ") == 0 && msg.Length > 8)
            {
                if ((XmlSolve.AdminCheck(fromqq) >= 1 && fromgroup != "common") || (fromgroup == "common" && fromqq == Tools.adminNumber))
                {
                    string get_msg = msg.Replace("！luadel ", "");
                    XmlSolve.luadel(fromgroup, get_msg);
                    result += "[CQ:emoji,id=128465]删除完成！\r\n脚本触发词为：" + get_msg;
                }
                else
                {
                    result += prem;
                }
            }
            else if (msg.IndexOf("！luasee ") == 0 && msg.Length > 8)
            {
                if ((XmlSolve.AdminCheck(fromqq) >= 1 && fromgroup != "common") || (fromgroup == "common" && fromqq == Tools.adminNumber))
                {
                    string get_msg = msg.Replace("！luasee ", "");
                    result += XmlSolve.xml_get(fromgroup, get_msg);
                }
                else
                {
                    result += prem;
                }
            }
            else if (msg.IndexOf("！add ") == 0 && msg.IndexOf("：") >= 0 && msg.IndexOf("：") < msg.Length)
            {
                if ((XmlSolve.AdminCheck(fromqq) >= 1 && fromgroup != "common") || (fromgroup == "common" && fromqq == Tools.adminNumber))
                {
                    string get_msg = msg.Replace("！add ", "");

                    int len = get_msg.IndexOf("：");
                    string tmsg = get_msg.Substring(0, len), tans = get_msg.Substring(len + 1);
                    if (len >= 0 && tans.Length > 0)
                    {
                        XmlSolve.insert(fromgroup, tmsg, tans);
                        result += "[CQ:emoji,id=9989]添加完成！\r\n词条：" + tmsg + "\r\n回答为：" + tans;
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
            else if (msg.IndexOf("！del ") == 0 && msg.IndexOf("：") >= 0 && msg.IndexOf("：") < msg.Length)
            {
                if ((XmlSolve.AdminCheck(fromqq) >= 1 && fromgroup != "common") || (fromgroup == "common" && fromqq == Tools.adminNumber))
                {
                    string get_msg = msg.Replace("！del ", "");

                    int len = get_msg.IndexOf("：");
                    string tmsg = get_msg.Substring(0, len), tans = get_msg.Substring(len + 1);
                    if (len >= 0 && tans.Length > 0)
                    {
                        XmlSolve.remove(fromgroup, tmsg, tans);
                        result += "[CQ:emoji,id=128465]删除完成！\r\n词条：" + tmsg + "\r\n回答为：" + tans;
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
            else if (msg.IndexOf("！delall ") == 0 && msg.Length > 8)
            {
                if ((XmlSolve.AdminCheck(fromqq) >= 1 && fromgroup != "common") || (fromgroup == "common" && fromqq == Tools.adminNumber))
                {
                    string get_msg = msg.Replace("！delall ", "");
                    if (get_msg.Length > 0)
                    {
                        XmlSolve.del(fromgroup, get_msg);
                        result += "[CQ:emoji,id=128465]删除完成！\r\n触发词：" + get_msg;
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
            else if (msg.IndexOf("！addadmin ") == 0 && fromqq == Tools.adminNumber)
            {
                XmlSolve.insert("admin_list", "给我列一下狗管理", msg.Replace("！addadmin ", ""));
                result += "已添加一位狗管理";
            }
            else if (msg.IndexOf("！deladmin ") == 0 && fromqq == Tools.adminNumber)
            {
                XmlSolve.remove("admin_list", "给我列一下狗管理", msg.Replace("！deladmin ", ""));
                result += "已删除一位狗管理";
            }
            else if (msg == "给我列一下狗管理")
            {
                result += "当前狗管理如下：\r\n" + XmlSolve.list_get("admin_list", "给我列一下狗管理");
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
            else if (msg.IndexOf("&#91;送礼物&#93; 为[CQ:at,qq="+Tools.qqNumber+"] 送出 ") == 0 && fromgroup != "common")
            {
                result += LotteryEvent.ReceiveGift(fromqq);
            }
            else if (msg.IndexOf("空气质量") == 0)
            {
                result += Tools.GetAir(msg, fromqq);
            }
            else if (msg.IndexOf("cmd ") == 0 && fromqq == Tools.adminNumber)
            {
                result += Tools.execCMD(HttpUtility.HtmlDecode(msg.Replace("cmd ", "")));
            }
            else if (msg.IndexOf("bedrock ") == 0 && fromqq == Tools.adminNumber && Tools.special.Length > 0)
            {
                Tools.HttpGet("http://localhost:2333/list", ""); 
                 result += Tools.HttpGet("http://localhost:2333/" + HttpUtility.HtmlDecode(msg.Replace("bedrock ", "")), "") + "--返回";
            }
            else if (msg.IndexOf("复读") == 0 && fromgroup != "common")
            {
                if (XmlSolve.AdminCheck(fromqq) >= 1)
                    result += Tools.At(fromqq) + Tools.SetRepeat(Tools.GetNumber(msg), fromgroup);
                else
                    result += prem;
            }
            else if (msg.IndexOf("搜动画") != -1 || msg.IndexOf("搜番") != -1 || 
                msg.IndexOf("查动画") != -1 || msg.IndexOf("查番") != -1)
            {
                result += Tools.At(fromqq) + "\r\n" + WhatAnime.GetAnime(msg.Replace("\r", "").Replace("\n", ""));
            }
            else if (msg.IndexOf("#lua") == 0 && msg.Length > 4)
            {
                if (XmlSolve.AdminCheck(fromqq) >= 1)//管理员才能用
                {
                    result += Tools.At(fromqq) + "\r\n" + Tools.RunLua(HttpUtility.HtmlDecode(msg.Substring(4)),
                        string.Format("fromqq=\"{0}\"\r\nfromgroup=\"{1}\"\r\n", fromqq, fromgroup));
                }
                else
                {
                    result += prem;
                }
            }
            else if (fromgroup == "241464054") //糖拌群
                result += MinecraftSolve.SolvePlayer(fromqq, msg, _mahuaApi);
            else if (fromgroup == "567145439") //分赃群
                result += MinecraftSolve.SolveAdmin(fromqq, msg, _mahuaApi);
            if(result == "")
                result += XmlSolve.ReplayGroupStatic(fromgroup, msg, fromqq);

            if (result == "")
                result += Tools.GetRepeatString(msg, fromgroup);

            return result;
        }
    }
}
