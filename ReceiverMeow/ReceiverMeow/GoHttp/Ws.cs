using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebSocketSharp;

namespace ReceiverMeow.GoHttp
{
    class Ws
    {
        private static WebSocket ws = null;
        private static string module = "WS";
        public static void Connect(string url, int port)
        {
            if(ws!=null)
                Log.Error(module, $"禁止多次初始化 ws！");
            Log.Info(module, $"开始连接 ws://{url}:{port}");
            try
            {
                ws = new WebSocket($"ws://{url}:{port}/");
                ws.OnClose += Ws_OnClose;
                ws.OnOpen += Ws_OnOpen;
                ws.OnMessage += Ws_OnMessage;
                ws.Connect();
            }
            catch(Exception e)
            {
                Log.Error(module, $"连接错误：\n{e.Message}");
            }
        }

        private static void Ws_OnMessage(object sender, MessageEventArgs e)
        {
            var j = Encoding.Default.GetString(e.RawData);
            Log.Debug(module, $"收到消息：{j}");
            try
            {
                Events(j);
            }
            catch(Exception ee)
            {
                Log.Warn(module, $"处理事件报错：{ee.Message}");
            }
        }

        private static void Ws_OnOpen(object sender, EventArgs e)
        {
            Log.Info(module, $"已连接！");
        }

        private static void Ws_OnClose(object sender, CloseEventArgs e)
        {
            while(true)
            {
                try
                {
                    Log.Warn(module, $"连接断开！尝试两秒后重连...");
                    Thread.Sleep(2);
                    ws.Connect();
                    break;
                }
                catch{}
            }
        }


        /// <summary>
        /// 发送ws数据
        /// </summary>
        /// <param name="d">数据</param>
        /// <returns>是否成功</returns>
        public static bool Send(byte[] d)
        {
            try
            {
                if (ws != null)
                {
                    ws.Send(d);
                    Log.Debug(module, $"发送数据：{Encoding.Default.GetString(d)}");
                }
            }
            catch 
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 分发消息到lua虚拟机
        /// </summary>
        /// <param name="j"></param>
        public static void Events(string j)
        {
            JObject o = JsonConvert.DeserializeObject<JObject>(j);
            switch((string)o["post_type"])
            {
                case "message":
                    if ((string)o["message_type"] == "group")
                    {
                        //群消息
                        LuaEnv.LuaStates.Run(o["group_id"].ToString(), "GroupMessage", new
                        {
                            group = (long)o["group_id"],
                            qq = (long)o["user_id"],
                            msg = (string)o["message"],
                            id = (long)o["message_id"],
                            raw = o
                        });
                    }
                    else if ((string)o["message_type"] == "private")
                    {
                        //私聊
                        LuaEnv.LuaStates.Run("private", "PrivateMessage", new
                        {
                            from = (string)o["sub_type"],
                            qq = (long)o["user_id"],
                            msg = (string)o["message"],
                            id = (long)o["message_id"],
                            raw = o
                        });
                    }
                    else if ((string)o["message_type"] == "guild")
                    {
                        //频道
                        LuaEnv.LuaStates.Run($"c{o["guild_id"]},{o["channel_id"]}", "GroupMessage", new
                        {
                            group = $"c{o["guild_id"]},{o["channel_id"]}",
                            qq = (string)o["user_id"],
                            msg = (string)o["message"],
                            id = (string)o["message_id"],
                            raw = o
                        });
                    }
                    break;
                case "notice":
                    switch ((string)o["notice_type"])
                    {
                        case "group_upload"://群文件上传
                            LuaEnv.LuaStates.Run(o["group_id"].ToString(), "GroupFileUpload", new
                            {
                                group = (long)o["group_id"],
                                qq = (long)o["user_id"],
                                file = (string)o["file"],
                                raw = o
                            });
                            break;
                        case "group_admin"://管理员变动
                            LuaEnv.LuaStates.Run(
                                o["group_id"].ToString(),
                                (string)o["sub_type"] == "set" ? "GroupManageSet" : "GroupManageRemove", 
                                new
                                {
                                    group = (long)o["group_id"],
                                    qq = (long)o["user_id"],
                                    raw = o
                                });
                            break;
                        case "group_decrease"://群成员增加
                            LuaEnv.LuaStates.Run(
                                o["group_id"].ToString(),
                                (string)o["sub_type"] == "leave" ? "GroupMemberExit" : "GroupMemberRemove",
                                new
                                {
                                    group = (long)o["group_id"],
                                    qq = (long)o["user_id"],
                                    fromqq = (long)o["operator_id"],
                                    raw = o
                                });
                            break;
                        case "group_increase"://群成员减少
                            LuaEnv.LuaStates.Run(
                                o["group_id"].ToString(),
                                (string)o["sub_type"] == "approve" ? "GroupMemberPass" : "GroupMemberInvite",
                                new
                                {
                                    group = (long)o["group_id"],
                                    qq = (long)o["user_id"],
                                    fromqq = (long)o["operator_id"],
                                    raw = o
                                });
                            break;
                        case "group_ban"://禁言
                            LuaEnv.LuaStates.Run(
                                o["group_id"].ToString(),
                                (string)o["sub_type"] == "ban" ? "GroupBanSpeak" : "GroupUnBanSpeak",
                                new
                                {
                                    fromqq = (long)o["operator_id"],
                                    group = (long)o["group_id"],
                                    banqq = (long)o["user_id"],
                                    time = (long)o["duration"],
                                    raw = o
                                });
                            break;
                        case "friend_add"://好友增加
                            LuaEnv.LuaStates.Run("main", "FriendAdd", new
                            {
                                qq = (long)o["user_id"],
                                raw = o
                            });
                            break;
                        case "group_recall"://群消息撤回
                            LuaEnv.LuaStates.Run(o["group_id"].ToString(), "GroupRecall", new
                            {
                                group = (long)o["group_id"],
                                qq = (long)o["user_id"],
                                fromqq = (long)o["operator_id"],
                                id = (long)o["message_id"],
                                raw = o
                            });
                            break;
                        case "friend_recall"://好友消息撤回
                            LuaEnv.LuaStates.Run("private", "FriendRecall", new
                            {
                                qq = (long)o["user_id"],
                                id = (long)o["message_id"],
                                raw = o
                            });
                            break;
                        case "poke"://群内戳一戳
                            LuaEnv.LuaStates.Run(o["group_id"].ToString(), "Poke", new
                            {
                                group = (long)o["group_id"],
                                qq = (long)o["target_id"],
                                fromqq = (long)o["user_id"],
                                raw = o
                            });
                            break;
                        case "lucky_king"://群红包运气王
                            LuaEnv.LuaStates.Run(o["group_id"].ToString(), "LuckyKing", new
                            {
                                group = (long)o["group_id"],
                                qq = (long)o["target_id"],
                                fromqq = (long)o["user_id"],
                                raw = o
                            });
                            break;
                        case "honor"://群成员荣誉变更
                            LuaEnv.LuaStates.Run(o["group_id"].ToString(), "Honor", new
                            {
                                group = (long)o["group_id"],
                                honor = (string)o["honor_type"],
                                qq = (long)o["user_id"],
                                raw = o
                            });
                            break;
                        case "notify"://适配下onebot11的分类
                            switch ((string)o["sub_type"])
                            {
                                case "poke"://群内戳一戳
                                    LuaEnv.LuaStates.Run(o["group_id"].ToString(), "Poke", new
                                    {
                                        group = (long)o["group_id"],
                                        qq = (long)o["target_id"],
                                        fromqq = (long)o["user_id"],
                                        raw = o
                                    });
                                    break;
                                case "lucky_king"://群红包运气王
                                    LuaEnv.LuaStates.Run(o["group_id"].ToString(), "LuckyKing", new
                                    {
                                        group = (long)o["group_id"],
                                        qq = (long)o["target_id"],
                                        fromqq = (long)o["user_id"],
                                        raw = o
                                    });
                                    break;
                                case "honor"://群成员荣誉变更
                                    LuaEnv.LuaStates.Run(o["group_id"].ToString(), "Honor", new
                                    {
                                        group = (long)o["group_id"],
                                        honor = (string)o["honor_type"],
                                        qq = (long)o["user_id"],
                                        raw = o
                                    });
                                    break;
                            }
                            break;
                    }
                    break;
                case "request":
                    if ((string)o["request_type"] == "group")
                    {
                        //加群请求/邀请
                        LuaEnv.LuaStates.Run(
                            "main",
                            (string)o["sub_type"] == "add" ? "GroupAddRequest" : "GroupAddInvite",
                            new
                            {
                                group = (long)o["group_id"],
                                qq = (long)o["user_id"],
                                msg = (string)o["comment"],
                                tag = (string)o["flag"],
                                raw = o
                            });
                    }
                    else if ((string)o["request_type"] == "friend")
                    {
                        //加好友请求
                        LuaEnv.LuaStates.Run("main", "FriendAddRequest", new
                        {
                            qq = (long)o["user_id"],
                            msg = (string)o["comment"],
                            tag = (string)o["flag"],
                            raw = o
                        });
                    }
                    break;
            }
        }
    }
}
