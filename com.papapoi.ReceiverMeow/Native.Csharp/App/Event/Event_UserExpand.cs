using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Native.Csharp.App.Interface;
using Native.Csharp.Sdk.Cqp;

namespace Native.Csharp.App.Event
{
	public class Event_UserExpand : IEvent_UserExpand
	{
		#region --公开方法--
		/// <summary>
		/// 打开控制台窗口
		/// </summary>
		/// <param name="sender">触发此事件的对象</param>
		/// <param name="e">附加的参数</param>
		public void OpenConsoleWindow (object sender, EventArgs e)
		{
            // 本事件将会在酷Q【主线程】中被触发, 请注意线程以免卡住酷Q
            Common.CqApi.AddLoger(Sdk.Cqp.Enum.LogerLevel.Error, "提示", "请打开你的日志哦~");
            Common.CqApi.AddLoger(Sdk.Cqp.Enum.LogerLevel.Info, "lua插件提示",
                "这不是错误提示哦~\r\n" +
                "懒得给插件写窗口了，就在这里显示详细信息把\r\n" +
                "软件使用方法/反馈/建议可以到GitHub提交哦\r\n" +
                "https://github.com/chenxuuu/receiver-meow\r\n" +
                "右击即可复制到剪贴板");
		}
		#endregion

		/*
		 *	关于导出方法:
		 *		
		 *		1. 请在此类中完成 IEvent_UserExpand 接口的实现, 以便 SDK 能够更灵活的将此实例注入到导出方法中
		 *		2. 请按照规范在此实现接口以免以后升级时带来不便
		 *	
		 *	至此!
		 */
	}
}
