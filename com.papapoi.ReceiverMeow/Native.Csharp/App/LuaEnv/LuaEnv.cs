using MoonSharp.Interpreter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Native.Csharp.App;

namespace Native.Csharp.App.LuaEnv
{
    class LuaEnv
    {
        public static int SetGroupSpecialTitle(long groupId, long qqId, string specialTitle, int time)
        {
            TimeSpan span = new TimeSpan(time/60/60/24, time/60/60%60, time/60%60, time%60);
            return Common.CqApi.SetGroupSpecialTitle(groupId, qqId, specialTitle, span);
        }


        public static int SetGroupAnonymousBanSpeak(long groupId, string anonymous, int time)
        {
            TimeSpan span = new TimeSpan(time / 60 / 60 / 24, time / 60 / 60 % 60, time / 60 % 60, time % 60);
            return Common.CqApi.SetGroupAnonymousBanSpeak(groupId, anonymous, span);
        }

        public static int SetGroupBanSpeak(long groupId, long qqId, int time)
        {
            TimeSpan span = new TimeSpan(time / 60 / 60 / 24, time / 60 / 60 % 60, time / 60 % 60, time % 60);
            return Common.CqApi.SetGroupBanSpeak(groupId, qqId, span);
        }

        /// <summary>
        /// 初始化lua对象
        /// </summary>
        /// <param name="lua"></param>
        /// <returns></returns>
        public static Script Initial(string code)
        {
            var lua = new Script();
            lua.Globals["cqCqCode_At"] = (Func<long, bool, string>)Common.CqApi.CqCode_At;
            //获取酷Q "At某人" 代码
            lua.Globals["cqCqCode_Emoji"] = (Func<int, string>)Common.CqApi.CqCode_Emoji;
            //获取酷Q "emoji表情" 代码
            lua.Globals["cqCqCode_Face"] = (Func<Sdk.Cqp.Enum.Face, string>)Common.CqApi.CqCode_Face;
            //获取酷Q "表情" 代码
            lua.Globals["cqCqCode_Shake"] = (Func<string>)Common.CqApi.CqCode_Shake;
            //获取酷Q "窗口抖动" 代码
            lua.Globals["cqCqCode_Trope"] = (Func<string, bool, string>)Common.CqApi.CqCode_Trope;
            //获取字符串的转义形式
            lua.Globals["cqCqCode_UnTrope"] = (Func<string, string>)Common.CqApi.CqCode_UnTrope;
            //获取字符串的非转义形式
            lua.Globals["cqCqCode_ShareLink"] = (Func<string, string, string, string, string>)Common.CqApi.CqCode_ShareLink;
            //获取酷Q "链接分享" 代码
            lua.Globals["cqCqCode_ShareCard"] = (Func<string, long, string>)Common.CqApi.CqCode_ShareCard;
            //获取酷Q "名片分享" 代码
            lua.Globals["cqCqCode_ShareGPS"] = (Func<string, string, double, double, int, string>)Common.CqApi.CqCode_ShareGPS;
            //获取酷Q "位置分享" 代码
            lua.Globals["cqCqCode_Anonymous"] = (Func<bool, string>)Common.CqApi.CqCode_Anonymous;
            //获取酷Q "匿名" 代码
            lua.Globals["cqCqCode_Image"] = (Func<string, string>)Common.CqApi.CqCode_Image;
            //获取酷Q "图片" 代码
            lua.Globals["cqCqCode_Music"] = (Func<long, string, bool, string>)Common.CqApi.CqCode_Music;
            //获取酷Q "音乐" 代码
            lua.Globals["cqCqCode_MusciDIY"] = (Func<string, string, string, string, string, string>)Common.CqApi.CqCode_MusciDIY;
            //获取酷Q "音乐自定义" 代码
            lua.Globals["cqCqCode_Record"] = (Func<string, string>)Common.CqApi.CqCode_Record;
            //获取酷Q "语音" 代码
            lua.Globals["cqSendGroupMessage"] = (Func<long, string, int>)Common.CqApi.SendGroupMessage;
            //发送群消息
            lua.Globals["cqSendPrivateMessage"] = (Func<long, string, int>)Common.CqApi.SendPrivateMessage;
            //发送私聊消息
            lua.Globals["cqSendDiscussMessage"] = (Func<long, string, int>)Common.CqApi.SendDiscussMessage;
            //发送讨论组消息
            lua.Globals["cqSendPraise"] = (Func<long, int, int>)Common.CqApi.SendPraise;
            //发送赞
            lua.Globals["cqRepealMessage"] = (Func<long, int>)Common.CqApi.RepealMessage;
            //撤回消息
            lua.Globals["cqGetLoginQQ"] = (Func<long>)Common.CqApi.GetLoginQQ;
            //取登录QQ
            lua.Globals["cqGetLoginNick"] = (Func<string>)Common.CqApi.GetLoginNick;
            //获取当前登录QQ的昵称
            lua.Globals["cqAppDirectory"] = (Func<string>)Common.CqApi.GetAppDirectory;
            //取应用目录
            lua.Globals["cqAddLoger"] = (Func<Sdk.Cqp.Enum.LogerLevel, string, string, int>)Common.CqApi.AddLoger;
            //添加日志
            lua.Globals["cqAddFatalError"] = (Func<string, int>)Common.CqApi.AddFatalError;
            //添加致命错误提示
            lua.Globals["cqSetGroupWholeBanSpeak"] = (Func<long, bool, int>)Common.CqApi.SetGroupWholeBanSpeak;
            //置全群禁言
            lua.Globals["cqSetGroupMemberNewCard"] = (Func<long, long, string, int>)Common.CqApi.SetGroupMemberNewCard;
            //置群成员名片
            lua.Globals["cqSetGroupManager"] = (Func<long, long, bool, int>)Common.CqApi.SetGroupManager;
            //置群管理员
            lua.Globals["cqSetAnonymousStatus"] = (Func<long, bool, int>)Common.CqApi.SetAnonymousStatus;
            //置群匿名设置
            lua.Globals["cqSetGroupMemberRemove"] = (Func<long, long, bool, int>)Common.CqApi.SetGroupMemberRemove;
            //置群员移除
            lua.Globals["cqSetDiscussExit"] = (Func<long, int>)Common.CqApi.SetDiscussExit;
            //置讨论组退出
            lua.Globals["cqSetGroupSpecialTitle"] = (Func<long, long, string, int, int>)SetGroupSpecialTitle;
            //置群成员专属头衔
            lua.Globals["cqSetGroupAnonymousBanSpeak"] = (Func<long, string, int, int>)SetGroupAnonymousBanSpeak;
            //置匿名群员禁言
            lua.Globals["cqSetGroupBanSpeak"] = (Func<long, long, int, int>)SetGroupBanSpeak;
            //置群员禁言

            lua.DoFile(Common.AppDirectory + "lua/require/head.lua");
            lua.DoString(code);
            return lua;
        }


        /// <summary>
        /// 运行lua文件
        /// </summary>
        /// <param name="code">提前运行的代码</param>
        /// <param name="file">文件路径（app/xxx.xxx.xx/lua/开头）</param>
        public static void RunLua(string code,string file = "")
        {
            try
            {
                var lua = Initial(code);
                if (file != "")
                    lua.DoFile(Common.AppDirectory + "lua/" + file);
            }
            catch(Exception e)
            {
                Common.CqApi.AddLoger(Sdk.Cqp.Enum.LogerLevel.Error, "lua脚本错误", e.ToString());
            }
        }
    }
}
