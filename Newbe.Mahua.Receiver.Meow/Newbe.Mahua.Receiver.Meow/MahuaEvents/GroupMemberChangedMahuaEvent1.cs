using Newbe.Mahua.MahuaEvents;
using System;
using Newbe.Mahua.Receiver.Meow.MahuaApis;

namespace Newbe.Mahua.Receiver.Meow.MahuaEvents
{
    /// <summary>
    /// 群成员变更事件
    /// </summary>
    public class GroupMemberChangedMahuaEvent1
        : IGroupMemberChangedMahuaEvent
    {
        private readonly IMahuaApi _mahuaApi;

        public GroupMemberChangedMahuaEvent1(
            IMahuaApi mahuaApi)
        {
            _mahuaApi = mahuaApi;
        }

        public void ProcessGroupMemberChanged(GroupMemberChangedContext context)
        {
            if(context.GroupMemberChangedType.ToString() == "Increased") //进群
            {
                _mahuaApi.SendGroupMessage(context.FromGroup, "欢迎" + Tools.At(context.JoinedOrLeftQq) + "进群！请仔细阅读群公告哦~");
            }
            else if (context.GroupMemberChangedType.ToString() == "Decreased")//退群
            {
                if(context.FromGroup== "241464054")
                {
                    string player = XmlSolve.xml_get("bind_qq", context.JoinedOrLeftQq);
                    if(player!="")
                    {
                        _mahuaApi.SendGroupMessage("567145439", "检测到玩家" + player + "已退群，请管理进入游戏，执行\r\n/code "+
                            MinecraftSolve.DelNewCode(player) +"\r\n命令来删除该玩家的白名单");
                        XmlSolve.del("bind_qq_wait", context.JoinedOrLeftQq);
                        XmlSolve.del("bind_qq", context.JoinedOrLeftQq);
                    }
                }
            }
        }
    }
}
