using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Native.Csharp.App;

namespace Native.Csharp.App.LuaEnv
{
    class LuaEnv
    {
        private static readonly object objLock = new object();
        
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
        public static void Initial(XLua.LuaEnv lua)
        {
            ///////////////
            //酷q类的接口//
            //////////////
            lua.DoString("cqCode_At = CS.Native.Csharp.App.LuaEnv.LuaApi.CqCode_At");
            //获取酷Q "At某人" 代码
            lua.DoString("cqCqCode_Emoji = CS.Native.Csharp.App.LuaEnv.LuaApi.CqCode_Emoji");
            //获取酷Q "emoji表情" 代码
            lua.DoString("cqCqCode_Face = CS.Native.Csharp.App.LuaEnv.LuaApi.CqCode_Face");
            //获取酷Q "表情" 代码
            lua.DoString("cqCqCode_Shake = CS.Native.Csharp.App.LuaEnv.LuaApi.CqCode_Shake");
            //获取酷Q "窗口抖动" 代码
            lua.DoString("cqCqCode_Trope = CS.Native.Csharp.App.LuaEnv.LuaApi.CqCode_Trope");
            //获取字符串的转义形式
            lua.DoString("cqCqCode_UnTrope = CS.Native.Csharp.App.LuaEnv.LuaApi.CqCode_UnTrope");
            //获取字符串的非转义形式
            lua.DoString("cqCqCode_ShareLink = CS.Native.Csharp.App.LuaEnv.LuaApi.CqCode_ShareLink");
            //获取酷Q "链接分享" 代码
            lua.DoString("cqCqCode_ShareCard = CS.Native.Csharp.App.LuaEnv.LuaApi.CqCode_ShareCard");
            //获取酷Q "名片分享" 代码
            lua.DoString("cqCqCode_ShareGPS = CS.Native.Csharp.App.LuaEnv.LuaApi.CqCode_ShareGPS");
            //获取酷Q "位置分享" 代码
            lua.DoString("cqCqCode_Anonymous = CS.Native.Csharp.App.LuaEnv.LuaApi.CqCode_Anonymous");
            //获取酷Q "匿名" 代码
            lua.DoString("cqCqCode_Image = CS.Native.Csharp.App.LuaEnv.LuaApi.CqCode_Image");
            //获取酷Q "图片" 代码
            lua.DoString("cqCqCode_Music = CS.Native.Csharp.App.LuaEnv.LuaApi.CqCode_Music");
            //获取酷Q "音乐" 代码
            lua.DoString("cqCqCode_MusciDIY = CS.Native.Csharp.App.LuaEnv.LuaApi.CqCode_MusciDIY");
            //获取酷Q "音乐自定义" 代码
            lua.DoString("cqCqCode_Record = CS.Native.Csharp.App.LuaEnv.LuaApi.CqCode_Record");
            //获取酷Q "语音" 代码
            lua.DoString("cqSendGroupMessage = CS.Native.Csharp.App.LuaEnv.LuaApi.SendGroupMessage");
            //发送群消息
            lua.DoString("cqSendPrivateMessage = CS.Native.Csharp.App.LuaEnv.LuaApi.SendPrivateMessage");
            //发送私聊消息
            lua.DoString("cqSendDiscussMessage = CS.Native.Csharp.App.LuaEnv.LuaApi.SendDiscussMessage");
            //发送讨论组消息
            lua.DoString("cqSendPraise = CS.Native.Csharp.App.LuaEnv.LuaApi.SendPraise");
            //发送赞
            lua.DoString("cqRepealMessage = CS.Native.Csharp.App.LuaEnv.LuaApi.RepealMessage");
            //撤回消息
            lua.DoString("cqGetLoginQQ = CS.Native.Csharp.App.LuaEnv.LuaApi.GetLoginQQ");
            //取登录QQ
            lua.DoString("cqGetLoginNick = CS.Native.Csharp.App.LuaEnv.LuaApi.GetLoginNick");
            //获取当前登录QQ的昵称
            lua.DoString("cqAppDirectory = CS.Native.Csharp.App.LuaEnv.LuaApi.GetAppDirectory");
            //取应用目录
            lua.DoString("cqAddLoger = CS.Native.Csharp.App.LuaEnv.LuaApi.AddLoger");
            //添加日志
            lua.DoString("cqAddFatalError = CS.Native.Csharp.App.LuaEnv.LuaApi.AddFatalError");
            //添加致命错误提示
            lua.DoString("cqSetGroupWholeBanSpeak = CS.Native.Csharp.App.LuaEnv.LuaApi.SetGroupWholeBanSpeak");
            //置全群禁言
            lua.DoString("cqSetGroupMemberNewCard = CS.Native.Csharp.App.LuaEnv.LuaApi.SetGroupMemberNewCard");
            //置群成员名片
            lua.DoString("cqSetGroupManager = CS.Native.Csharp.App.LuaEnv.LuaApi.SetGroupManager");
            //置群管理员
            lua.DoString("cqSetAnonymousStatus = CS.Native.Csharp.App.LuaEnv.LuaApi.SetAnonymousStatus");
            //置群匿名设置
            lua.DoString("cqSetGroupMemberRemove = CS.Native.Csharp.App.LuaEnv.LuaApi.SetGroupMemberRemove");
            //置群员移除
            lua.DoString("cqSetDiscussExit = CS.Native.Csharp.App.LuaEnv.LuaApi.SetDiscussExit");
            //置讨论组退出
            lua.DoString("cqSetGroupSpecialTitle = CS.Native.Csharp.App.LuaEnv.LuaEnv.SetGroupSpecialTitle");
            //置群成员专属头衔
            lua.DoString("cqSetGroupAnonymousBanSpeak = CS.Native.Csharp.App.LuaEnv.LuaEnv.SetGroupAnonymousBanSpeak");
            //置匿名群员禁言
            lua.DoString("cqSetGroupBanSpeak = CS.Native.Csharp.App.LuaEnv.LuaEnv.SetGroupBanSpeak");
            //置群员禁言
            lua.DoString("cqSetFriendAddRequest = CS.Native.Csharp.App.LuaEnv.LuaApi.SetFriendAddRequest");
            //置好友添加请求
            lua.DoString("cqSetGroupAddRequest = CS.Native.Csharp.App.LuaEnv.LuaApi.SetGroupAddRequest");
            //置群添加请求

            /////////////
            //工具类接口//
            /////////////
            lua.DoString("apiGetPath = CS.Native.Csharp.App.LuaEnv.LuaApi.GetPath");
            //获取程序运行目录
            lua.DoString("apiGetBitmap = CS.Native.Csharp.App.LuaEnv.LuaApi.GetBitmap");
            //获取图片对象
            lua.DoString("apiPutText = CS.Native.Csharp.App.LuaEnv.LuaApi.PutText");
            //摆放文字
            lua.DoString("apiPutBlock = CS.Native.Csharp.App.LuaEnv.LuaApi.PutBlock");
            //填充矩形
            lua.DoString("apiSetImage = CS.Native.Csharp.App.LuaEnv.LuaApi.SetImage");
            //摆放图片
            lua.DoString("apiGetDir = CS.Native.Csharp.App.LuaEnv.LuaApi.GetDir");
            //保存并获取图片路径

            lua.DoString("apiGetImagePath = CS.Native.Csharp.App.LuaEnv.LuaApi.GetImagePath");
            //获取qq消息中图片的网址

            lua.DoString("apiHttpDownload = CS.Native.Csharp.App.LuaEnv.LuaApi.HttpDownload");
            //下载文件
            lua.DoString("apiHttpGet = CS.Native.Csharp.App.LuaEnv.LuaApi.HttpGet");
            //GET 请求与获取结果
            lua.DoString("apiHttpPost = CS.Native.Csharp.App.LuaEnv.LuaApi.HttpPost");
            //POST 请求与获取结果
            lua.DoString("apiBase64File = CS.Native.Csharp.App.LuaEnv.LuaApi.Base64File");
            //获取在线文件的base64结果
            lua.DoString("apiGetPictureWidth = CS.Native.Csharp.App.LuaEnv.LuaApi.GetPictureWidth");
            //获取在线文件的base64结果
            lua.DoString("apiGetPictureHeight = CS.Native.Csharp.App.LuaEnv.LuaApi.GetPictureHeight");
            //获取在线文件的base64结果
            lua.DoString("apiSetVar = CS.Native.Csharp.App.LuaEnv.LuaApi.SetVar");
            //设置某值存入ram
            lua.DoString("apiGetVar = CS.Native.Csharp.App.LuaEnv.LuaApi.GetVar");
            //取出某缓存的值
            lua.DoString("apiGetAsciiHex = CS.Native.Csharp.App.LuaEnv.LuaApi.GetAsciiHex");
            //获取字符串ascii编码的hex串

            lua.DoString("apiGetHardDiskFreeSpace = CS.Native.Csharp.App.LuaEnv.Tools.GetHardDiskFreeSpace");
            //获取指定驱动器的剩余空间总大小(单位为MB)
            lua.DoString("apiMD5Encrypt = CS.Native.Csharp.App.LuaEnv.Tools.MD5Encrypt");
            //计算MD5

            lua.DoString("apiTcpSend = CS.Native.Csharp.App.LuaEnv.TcpServer.Send");
            //发送tcp广播数据

            lua.DoString("apiSandBox = CS.Native.Csharp.App.LuaEnv.LuaEnv.RunSandBox");
            //沙盒环境

            ///////////////
            //XML操作接口//
            //////////////
            lua.DoString("apiXmlReplayGet = CS.Native.Csharp.App.LuaEnv.XmlApi.replay_get");
            //随机获取一条结果
            lua.DoString("apiXmlListGet = CS.Native.Csharp.App.LuaEnv.XmlApi.list_get");
            //获取所有回复的列表
            lua.DoString("apiXmlDelete = CS.Native.Csharp.App.LuaEnv.XmlApi.del");
            //删除所有匹配的条目
            lua.DoString("apiXmlRemove = CS.Native.Csharp.App.LuaEnv.XmlApi.remove");
            //删除完全匹配的第一个条目
            lua.DoString("apiXmlInsert = CS.Native.Csharp.App.LuaEnv.XmlApi.insert");
            //插入一个词条
            lua.DoString("apiXmlSet = CS.Native.Csharp.App.LuaEnv.XmlApi.set");
            //更改某条的值
            lua.DoString("apiXmlGet = CS.Native.Csharp.App.LuaEnv.XmlApi.xml_get");
            //获取某条的结果
            lua.DoString("apiXmlRow = CS.Native.Csharp.App.LuaEnv.XmlApi.xml_row");
            //按结果查源头（反查）

            lua.DoString(@"--加上需要require的路径
local rootPath = apiGetAsciiHex(apiGetPath())
rootPath = rootPath:gsub('[%s%p]', ''):upper()
rootPath = rootPath:gsub('%x%x', function(c)
                                    return string.char(tonumber(c, 16))
                                end)
package.path = package.path..
';'..rootPath..'data/app/com.papapoi.ReceiverMeow/lua/require/?.lua'..
';'..rootPath..'data/app/com.papapoi.ReceiverMeow/lua/?.lua'
");

            lua.DoString("require('head')");
        }


        /// <summary>
        /// 运行lua文件
        /// </summary>
        /// <param name="code">提前运行的代码</param>
        /// <param name="file">文件路径（app/xxx.xxx.xx/lua/开头）</param>
        public static bool RunLua(string code,string file,ArrayList args = null)
        {
            //还没下载lua脚本，先不响应消息
            if (!File.Exists(Common.AppDirectory + "lua/require/head.lua"))
                return false;

            //只许实例化一个
            lock (objLock)
            {
                using (var lua = new XLua.LuaEnv())
                {
                    bool handled = false;
                    try
                    {
                        lua.Global.SetInPath("handled", false);//处理标志
                        Initial(lua);
                        if (args != null)
                            for (int i = 0; i < args.Count; i += 2)
                            {
                                lua.Global.SetInPath((string)args[i], args[i + 1]);
                            }
                        lua.DoString(code);
                        if (file != "")
                            lua.DoString($"require('{file.Replace("/", ".").Replace("\\", ".").Substring(0, file.Length - 4)}')");
                        handled = lua.Global.GetInPath<bool>("handled");
                    }
                    catch (Exception e)
                    {
                        Common.CqApi.AddLoger(Sdk.Cqp.Enum.LogerLevel.Error, "lua脚本错误", e.ToString());

                    }
                    return handled;
                }
                
            }
        }

        /// <summary>
        /// 在沙盒中运行代码，仅允许安全地运行
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static string RunSandBox(string code)
        {
            lock (objLock)
            {
                using (var lua = new XLua.LuaEnv())
                {
                    string r = "";
                    try
                    {
                        lua.DoString("apiGetPath = CS.Native.Csharp.App.LuaEnv.LuaApi.GetPath");
                        //获取程序运行目录
                        lua.DoString("apiGetAsciiHex = CS.Native.Csharp.App.LuaEnv.LuaApi.GetAsciiHex");
                        //获取字符串ascii编码的hex串
                        lua.Global.SetInPath("lua_run_result_var", "");//返回值所在的变量
                        lua.DoString(@"--加上需要require的路径
local rootPath = apiGetAsciiHex(apiGetPath())
rootPath = rootPath:gsub('[%s%p]', ''):upper()
rootPath = rootPath:gsub('%x%x', function(c)
                                    return string.char(tonumber(c, 16))
                                end)
package.path = package.path..
';'..rootPath..'data/app/com.papapoi.ReceiverMeow/lua/require/sandbox/?.lua'
");
                        lua.DoString("require('sandbox.head')");
                        lua.DoString(code);
                        r = lua.Global.GetInPath<string>("lua_run_result_var");

                    }
                    catch (Exception e)
                    {
                        Common.CqApi.AddLoger(Sdk.Cqp.Enum.LogerLevel.Error, "沙盒lua脚本错误", e.ToString());
                        r = "运行错误：" + e.ToString();

                    }
                    return r;
                }
            }
        }
    }
}
