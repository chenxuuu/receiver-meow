using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using Native.Csharp.Sdk.Cqp.Interface;
using ReceiverMeow.UI;

namespace Native.Csharp.App
{
	/// <summary>
	/// 酷Q应用主入口类
	/// </summary>
	public static class CQMain
	{
		/// <summary>
		/// 在应用被加载时将调用此方法进行事件注册, 请在此方法里向 <see cref="IUnityContainer"/> 容器中注册需要使用的事件
		/// </summary>
		/// <param name="container">用于注册的 IOC 容器 </param>
		public static void Register (IUnityContainer container)
		{
			container.RegisterType<IGroupMessage, EventGroupMessage>("群消息处理");
			container.RegisterType<IAppDisable, EventAppDisable>("应用将被停用");
			container.RegisterType<IAppEnable, EventAppEnable>("应用已被启用");
			container.RegisterType<ICQExit, EventCQExit>("酷Q关闭事件");
			container.RegisterType<ICQStartup, EventCQStartup>("酷Q启动事件");
			container.RegisterType<IDiscussMessage, EventDiscussMessage>("讨论组消息处理");
			container.RegisterType<IFriendAdd, EventFriendAdd>("好友已添加事件处理");
			container.RegisterType<IFriendAddRequest, EventFriendAddRequest>("好友添加请求处理");
			container.RegisterType<IGroupAddRequest, EventGroupAddRequest>("群添加请求处理");
			container.RegisterType<IGroupBanSpeak, EventGroupBanSpeak>("群禁言事件处理");
			container.RegisterType<IGroupManageChange, EventGroupManageChange>("群管理变动事件处理");
			container.RegisterType<IGroupMemberDecrease, EventGroupMemberDecrease>("群成员减少事件处理");
			container.RegisterType<IGroupMemberIncrease, EventGroupMemberIncrease>("群成员增加事件处理");
			container.RegisterType<IGroupUpload, EventGroupUpload>("群文件上传事件处理");
			container.RegisterType<IPrivateMessage, EventPrivateMessage>("私聊消息处理");
			container.RegisterType<IMenuCall, MainWindow>("插件基本设置");
		}
	}
}
