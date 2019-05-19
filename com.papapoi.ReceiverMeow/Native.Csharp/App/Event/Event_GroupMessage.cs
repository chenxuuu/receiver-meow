using Native.Csharp.Sdk.Cqp;
using Native.Csharp.Sdk.Cqp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Native.Csharp.App.Model;
using Native.Csharp.App.Interface;
using System.Collections;

namespace Native.Csharp.App.Event
{
	public class Event_GroupMessage : IEvent_GroupMessage
	{
		#region --公开方法--
		/// <summary>
		/// Type=2 群消息<para/>
		/// 处理收到的群消息
		/// </summary>
		/// <param name="sender">事件的触发对象</param>
		/// <param name="e">事件的附加参数</param>
		public void ReceiveGroupMessage (object sender, GroupMessageEventArgs e)
		{
            // 本子程序会在酷Q【线程】中被调用，请注意使用对象等需要初始化(CoInitialize,CoUninitialize)。
            // 这里处理消息
            e.Handled = LuaEnv.LuaEnv.RunLua(
                $"fromqq={e.FromQQ} " +
                $"fromgroup={e.FromGroup} " +
                $"message=[[{e.Msg.Replace("]", "] ")}]] " +
                $"id={e.MsgId} " +
                $"fromAnonymous={e.IsAnonymousMsg.ToString().ToLower()}",
                "envent/ReceiveGroupMessage.lua");

		}

		/// <summary>
		/// Type=21 群私聊<para/>
		/// 处理收到的群私聊消息
		/// </summary>
		/// <param name="sender">事件的触发对象</param>
		/// <param name="e">事件的附加参数</param>
		public void ReceiveGroupPrivateMessage (object sender, PrivateMessageEventArgs e)
		{
            // 本子程序会在酷Q【线程】中被调用，请注意使用对象等需要初始化(CoInitialize,CoUninitialize)。
            // 这里处理消息
            e.Handled = LuaEnv.LuaEnv.RunLua(
                $"fromqq={e.FromQQ} " +
                $"message=[[{e.Msg.Replace("]", "] ")}]] " +
                $"id={e.MsgId}",
                "envent/ReceivePrivateMessage.lua");

            //e.Handled = false;  // 关于返回说明, 请参见 "Event_FriendMessage.ReceiveFriendMessage" 方法
		}

		/// <summary>
		/// Type=11 群文件上传事件<para/>
		/// 处理收到的群文件上传结果
		/// </summary>
		/// <param name="sender">事件的触发对象</param>
		/// <param name="e">事件的附加参数</param>
		public void ReceiveGroupFileUpload (object sender, FileUploadMessageEventArgs e)
		{
            // 本子程序会在酷Q【线程】中被调用，请注意使用对象等需要初始化(CoInitialize,CoUninitialize)。
            // 这里处理消息
            // 关于文件信息, 触发事件时已经转换完毕, 请直接使用

            e.Handled = LuaEnv.LuaEnv.RunLua(
                $"fromqq={e.FromQQ} " +
                $"fromgroup={e.FromGroup} " +
                $"fileName=[[{e.File.Name.Replace("]", "] ")}]] " +
                $"id=[[{e.File.Id.Replace("]", "] ")}]] " +
                $"size={e.File.Size}",
                "envent/ReceiveGroupFileUpload.lua");

            //e.Handled = false;   // 关于返回说明, 请参见 "Event_FriendMessage.ReceiveFriendMessage" 方法
        }

		/// <summary>
		/// Type=101 群事件 - 管理员增加<para/>
		/// 处理收到的群管理员增加事件
		/// </summary>
		/// <param name="sender">事件的触发对象</param>
		/// <param name="e">事件的附加参数</param>
		public void ReceiveGroupManageIncrease (object sender, GroupManageAlterEventArgs e)
		{
            // 本子程序会在酷Q【线程】中被调用，请注意使用对象等需要初始化(CoInitialize,CoUninitialize)。
            // 这里处理消息
            e.Handled = LuaEnv.LuaEnv.RunLua(
                $"fromqq={e.BeingOperateQQ} " +
                $"fromgroup={e.FromGroup} " +
                $"manager=true",
                "envent/ReceiveGroupManage.lua");

            //e.Handled = false;  // 关于返回说明, 请参见 "Event_FriendMessage.ReceiveFriendMessage" 方法
		}

		/// <summary>
		/// Type=101 群事件 - 管理员减少<para/>
		/// 处理收到的群管理员减少事件
		/// </summary>
		/// <param name="sender">事件的触发对象</param>
		/// <param name="e">事件的附加参数</param>
		public void ReceiveGroupManageDecrease (object sender, GroupManageAlterEventArgs e)
		{
            // 本子程序会在酷Q【线程】中被调用，请注意使用对象等需要初始化(CoInitialize,CoUninitialize)。
            // 这里处理消息
            e.Handled = LuaEnv.LuaEnv.RunLua(
                $"fromqq={e.BeingOperateQQ} " +
                $"fromgroup={e.FromGroup} " +
                $"manager=false",
                "envent/ReceiveGroupManage.lua");

            //e.Handled = false;  // 关于返回说明, 请参见 "Event_FriendMessage.ReceiveFriendMessage" 方法
		}

		/// <summary>
		/// Type=103 群事件 - 群成员增加 - 主动入群<para/>
		/// 处理收到的群成员增加 (主动入群) 事件
		/// </summary>
		/// <param name="sender">事件的触发对象</param>
		/// <param name="e">事件的附加参数</param>
		public void ReceiveGroupMemberJoin (object sender, GroupMemberAlterEventArgs e)
		{
            // 本子程序会在酷Q【线程】中被调用，请注意使用对象等需要初始化(CoInitialize,CoUninitialize)。
            // 这里处理消息
            e.Handled = LuaEnv.LuaEnv.RunLua(
                $"fromqq={e.BeingOperateQQ} " +
                $"fromgroup={e.FromGroup}",
                "envent/ReceiveGroupMemberJoin.lua");
            
            //e.Handled = false;  // 关于返回说明, 请参见 "Event_FriendMessage.ReceiveFriendMessage" 方法
		}

		/// <summary>
		/// Type=103 群事件 - 群成员增加 - 被邀入群<para/>
		/// 处理收到的群成员增加 (被邀入群) 事件
		/// </summary>
		/// <param name="sender">事件的触发对象</param>
		/// <param name="e">事件的附加参数</param>
		public void ReceiveGroupMemberInvitee (object sender, GroupMemberAlterEventArgs e)
		{
            // 本子程序会在酷Q【线程】中被调用, 请注意使用对象等需要初始化(ConIntialize, CoUninitialize).
            // 这里处理消息
            e.Handled = LuaEnv.LuaEnv.RunLua(
                $"fromqq={e.BeingOperateQQ} " +
                $"fromgroup={e.FromGroup}",
                "envent/ReceiveGroupMemberJoin.lua");
            
            //e.Handled = false;  // 关于返回说明, 请参见 "Event_FriendMessage.ReceiveFriendMessage" 方法
		}

		/// <summary>
		/// Type=102 群事件 - 群成员减少 - 成员离开<para/>
		/// 处理收到的群成员减少 (成员离开) 事件
		/// </summary>
		/// <param name="sender">事件的触发对象</param>
		/// <param name="e">事件的附加参数</param>
		public void ReceiveGroupMemberLeave (object sender, GroupMemberAlterEventArgs e)
		{
            // 本子程序会在酷Q【线程】中被调用, 请注意使用对象等需要初始化(ConIntialize, CoUninitialize).
            // 这里处理消息
            e.Handled = LuaEnv.LuaEnv.RunLua(
                $"fromqq={e.BeingOperateQQ} " +
                $"fromgroup={e.FromGroup}",
                "envent/ReceiveGroupMemberLeave.lua");
            
            //e.Handled = false;  // 关于返回说明, 请参见 "Event_FriendMessage.ReceiveFriendMessage" 方法
		}

		/// <summary>
		/// Type=102 群事件 - 群成员减少 - 成员移除<para/>
		/// 处理收到的群成员减少 (成员移除) 事件
		/// </summary>
		/// <param name="sender">事件的触发对象</param>
		/// <param name="e">事件的附加参数</param>
		public void ReceiveGroupMemberRemove (object sender, GroupMemberAlterEventArgs e)
		{
            // 本子程序会在酷Q【线程】中被调用, 请注意使用对象等需要初始化(ConIntialize, CoUninitialize).
            // 这里处理消息
            e.Handled = LuaEnv.LuaEnv.RunLua(
                $"fromqq={e.BeingOperateQQ} " +
                $"fromgroup={e.FromGroup} " +
                $"doqq={e.FromQQ}",
                "envent/ReceiveGroupMemberLeave.lua");

            //e.Handled = false;  // 关于返回说明, 请参见 "Event_FriendMessage.ReceiveFriendMessage" 方法
		}

		/// <summary>
		/// Type=302 群事件 - 群请求 - 申请入群<para/>
		/// 处理收到的群请求 (申请入群) 事件
		/// </summary>
		/// <param name="sender">事件的触发对象</param>
		/// <param name="e">事件的附加参数</param>
		public void ReceiveGroupAddApply (object sender, GroupAddRequestEventArgs e)
		{
            // 本子程序会在酷Q【线程】中被调用, 请注意使用对象等需要初始化(ConIntialize, CoUninitialize).
            // 这里处理消息
            e.Handled = LuaEnv.LuaEnv.RunLua(
                $"fromqq={e.FromQQ} " +
                $"tag=[[{e.Tag.Replace("]", "] ")}]] " +
                $"fromgroup={e.FromGroup} " +
                $"message=[[{e.AppendMsg.Replace("]", "] ")}]]",
                "envent/ReceiveGroupAddApply.lua");

            //e.Handled = false;  // 关于返回说明, 请参见 "Event_FriendMessage.ReceiveFriendMessage" 方法
        }

		/// <summary>
		/// Type=302 群事件 - 群请求 - 被邀入群 (机器人被邀)<para/>
		/// 处理收到的群请求 (被邀入群) 事件
		/// </summary>
		/// <param name="sender">事件的触发对象</param>
		/// <param name="e">事件的附加参数</param>
		public void ReceiveGroupAddInvitee (object sender, GroupAddRequestEventArgs e)
		{
            // 本子程序会在酷Q【线程】中被调用, 请注意使用对象等需要初始化(ConIntialize, CoUninitialize).
            // 这里处理消息
            e.Handled = LuaEnv.LuaEnv.RunLua(
                $"fromqq={e.FromQQ} " +
                $"tag=[[{e.Tag.Replace("]", "] ")}]] " +
                $"fromgroup={e.FromGroup} ",
                "envent/ReceiveGroupAddInvitee.lua");

            //e.Handled = false;  // 关于返回说明, 请参见 "Event_FriendMessage.ReceiveFriendMessage" 方法
        }
		#endregion
	}
}
