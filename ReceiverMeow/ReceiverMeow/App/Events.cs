using Native.Csharp.App.LuaEnv;
using Native.Csharp.Sdk.Cqp.EventArgs;
using Native.Csharp.Sdk.Cqp.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Native.Csharp.App
{
    class EventAppDisable : IAppDisable
    {
        public void AppDisable(object sender, CQAppDisableEventArgs e)
        {
            LuaEnv.LuaStates.Run("main", "AppDisable", new {});
        }
    }

    class EventAppEnable : IAppEnable
    {
        public void AppEnable(object sender, CQAppEnableEventArgs e)
        {
            LuaEnv.LuaStates.Run("main", "AppEnable", new { });
        }
    }

    class EventCQExit : ICQExit
    {
        public void CQExit(object sender, CQExitEventArgs e)
        {
            LuaEnv.LuaStates.Run("main", "CQExit", new { });
        }
    }

    class EventCQStartup : ICQStartup
    {
        public void CQStartup(object sender, CQStartupEventArgs e)
        {
            LuaEnv.LuaStates.Run("main", "CQStartup", new { });
        }
    }

    class EventDiscussMessage : IDiscussMessage
    {
        public void DiscussMessage(object sender, CQDiscussMessageEventArgs e)
        {
            if (e.SubType == Sdk.Cqp.Enum.CQDiscussMessageType.Discuss)
            {
                LuaEnv.LuaStates.Run($"Discuss{e.FromDiscuss.Id}", "DiscussMessage", new
                {
                    fromDiscuss = e.FromDiscuss.Id,
                    qq = e.FromQQ.Id,
                    msg = e.Message.Text,
                    id = e.Message.Id
                });
            }
        }
    }

    class EventFriendAdd : IFriendAdd
    {
        public void FriendAdd(object sender, CQFriendAddEventArgs e)
        {
            if (e.SubType == Sdk.Cqp.Enum.CQFriendAddType.FriendAdd)
            {
                LuaEnv.LuaStates.Run("main", "FriendAdd", new
                {
                    qq = e.FromQQ.Id,
                });
            }
        }
    }

    class EventFriendAddRequest : IFriendAddRequest
    {
        public void FriendAddRequest(object sender, CQFriendAddRequestEventArgs e)
        {
            if (e.SubType == Sdk.Cqp.Enum.CQFriendAddRequestType.FriendAdd)
            {
                LuaEnv.LuaStates.Run("main", "FriendAddRequest", new
                {
                    qq = e.FromQQ.Id,
                    msg = e.AppendMessage,
                    tag = e.ResponseFlag
                });
            }
        }
    }

    class EventGroupAddRequest : IGroupAddRequest
    {
        public void GroupAddRequest(object sender, CQGroupAddRequestEventArgs e)
        {
            if (e.SubType == Sdk.Cqp.Enum.CQGroupAddRequestType.ApplyAddGroup)
            {
                LuaEnv.LuaStates.Run("main", "GroupAddRequest", new
                {
                    qq = e.FromQQ.Id,
                    msg = e.AppendMessage,
                    group = e.FromGroup.Id,
                    tag = e.ResponseFlag
                });
            }
            else if(e.SubType == Sdk.Cqp.Enum.CQGroupAddRequestType.RobotBeInviteAddGroup)
            {
                LuaEnv.LuaStates.Run("main", "GroupAddInvite", new
                {
                    qq = e.FromQQ.Id,
                    msg = e.AppendMessage,
                    group = e.FromGroup.Id,
                    tag = e.ResponseFlag
                });
            }
        }
    }

    class EventGroupBanSpeak : IGroupBanSpeak
    {
        public void GroupBanSpeak(object sender, CQGroupBanSpeakEventArgs e)
        {
            if (e.SubType == Sdk.Cqp.Enum.CQGroupBanSpeakType.SetBanSpeak)
            {
                long banqq = 0;
                if (e.BeingOperateQQ != null)
                    banqq = e.BeingOperateQQ.Id;
                LuaEnv.LuaStates.Run(e.FromGroup.Id, "GroupBanSpeak", new
                {
                    fromqq = e.FromQQ.Id,
                    group = e.FromGroup.Id,
                    banqq,
                    all = e.IsAllBanSpeak,
                    time = Utils.DateTimeToInt(e.BanSpeakTimeSpan ?? new TimeSpan(0)),
                });
            }
            else if(e.SubType == Sdk.Cqp.Enum.CQGroupBanSpeakType.RemoveBanSpeak)
            {
                long banqq = 0;
                if (e.BeingOperateQQ != null)
                    banqq = e.BeingOperateQQ.Id;
                LuaEnv.LuaStates.Run(e.FromGroup.Id, "GroupUnBanSpeak", new
                {
                    fromqq = e.FromQQ.Id,
                    group = e.FromGroup.Id,
                    banqq,
                    all = e.IsAllBanSpeak
                });
            }
        }
    }

    class EventGroupManageChange : IGroupManageChange
    {
        public void GroupManageChange(object sender, CQGroupManageChangeEventArgs e)
        {
            if (e.SubType == Sdk.Cqp.Enum.CQGroupManageChangeType.SetManage)
            {
                LuaEnv.LuaStates.Run(e.FromGroup.Id, "GroupManageSet", new
                {
                    group = e.FromGroup.Id,
                    qq = e.BeingOperateQQ.Id,
                });
            }
            else if(e.SubType == Sdk.Cqp.Enum.CQGroupManageChangeType.RemoveManage)
            {
                LuaEnv.LuaStates.Run(e.FromGroup.Id, "GroupManageRemove", new
                {
                    group = e.FromGroup.Id,
                    qq = e.BeingOperateQQ.Id,
                });
            }
        }
    }

    class EventGroupMemberDecrease : IGroupMemberDecrease
    {
        public void GroupMemberDecrease(object sender, CQGroupMemberDecreaseEventArgs e)
        {
            if (e.SubType == Sdk.Cqp.Enum.CQGroupMemberDecreaseType.ExitGroup)
            {
                LuaEnv.LuaStates.Run(e.FromGroup.Id, "GroupMemberExit", new
                {
                    group = e.FromGroup.Id,
                    qq = e.BeingOperateQQ,
                });
            }
            else if (e.SubType == Sdk.Cqp.Enum.CQGroupMemberDecreaseType.RemoveGroup)
            {
                LuaEnv.LuaStates.Run(e.FromGroup.Id, "GroupMemberRemove", new
                {
                    group = e.FromGroup.Id,
                    qq = e.BeingOperateQQ,
                    fromqq = e.FromGroup.Id
                });
            }
        }
    }

    class EventGroupMemberIncrease : IGroupMemberIncrease
    {
        public void GroupMemberIncrease(object sender, CQGroupMemberIncreaseEventArgs e)
        {
            if (e.SubType == Sdk.Cqp.Enum.CQGroupMemberIncreaseType.Invite)
            {
                LuaEnv.LuaStates.Run(e.FromGroup.Id, "GroupMemberInvite", new
                {
                    group = e.FromGroup.Id,
                    qq = e.BeingOperateQQ,
                    fromqq = e.FromGroup.Id
                });
            }
            else if (e.SubType == Sdk.Cqp.Enum.CQGroupMemberIncreaseType.Pass)
            {
                LuaEnv.LuaStates.Run(e.FromGroup.Id, "GroupMemberPass", new
                {
                    group = e.FromGroup.Id,
                    qq = e.BeingOperateQQ,
                    fromqq = e.FromGroup.Id
                });
            }
        }
    }

    class EventGroupMessage : IGroupMessage
    {
        public void GroupMessage(object sender, CQGroupMessageEventArgs e)
        {
            if (e.SubType == Sdk.Cqp.Enum.CQGroupMessageType.Group)
            {
                LuaEnv.LuaStates.Run(e.FromGroup.Id, "GroupMessage", new
                {
                    group = e.FromGroup.Id,
                    qq = e.FromQQ.Id,
                    msg = e.Message.Text,
                    id = e.Message.Id,
                    fromAnonymous = e.IsFromAnonymous
                });
            }
        }
    }

    class EventGroupUpload : IGroupUpload
    {
        public void GroupUpload(object sender, CQGroupUploadEventArgs e)
        {
            if (e.SubType == Sdk.Cqp.Enum.CQGroupFileUploadType.FileUpload)
            {
                LuaEnv.LuaStates.Run(e.FromGroup.Id, "GroupFileUpload", new
                {
                    group = e.FromGroup.Id,
                    qq = e.FromQQ.Id,
                    file = e.FileInfo
                });
            }
        }
    }

    class EventPrivateMessage : IPrivateMessage
    {
        public void PrivateMessage(object sender, CQPrivateMessageEventArgs e)
        {
            LuaEnv.LuaStates.Run("private", "GroupFileUpload", new
            {
                from = e.SubType,
                qq = e.FromQQ.Id,
                msg = e.Message.Text,
                id = e.Message.Id
            });
        }
    }
}
