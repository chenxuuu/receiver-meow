/*
 * 此文件由T4引擎自动生成, 请勿修改此文件中的代码!
 */
using System;
using System.Reflection;
using System.Runtime.InteropServices;
using Native.Csharp.Sdk.Cqp;
using Native.Csharp.App.Common;
using Native.Csharp.Sdk.Cqp.EventArgs;
using Native.Csharp.Sdk.Cqp.Interface;
using Native.Csharp.Sdk.Cqp.Model;
using Unity;

namespace Native.Csharp.App.Export
{
	/// <summary>	
	/// 表示酷Q状态导出的类	
	/// </summary>	
	public class CQStatusExport	
	{	
		#region --字段--	
		private static CQApi api = null;	
		private static CQLog log = null;	
		#endregion	
		
		#region --构造函数--	
		/// <summary>	
		/// 由托管环境初始化的 <see cref="CQStatusExport"/> 的新实例	
		/// </summary>	
		static CQStatusExport ()	
		{	
			api = AppInfo.UnityContainer.Resolve<CQApi> (AppInfo.Id);	
			log = AppInfo.UnityContainer.Resolve<CQLog> (AppInfo.Id);	
			
			// 调用方法进行实例化	
			ResolveBackcall ();	
		}	
		#endregion	
		
		#region --私有方法--	
		/// <summary>	
		/// 读取容器中的注册项, 进行事件分发	
		/// </summary>	
		private static void ResolveBackcall ()	
		{	
		}	
		#endregion	
		
		#region --导出方法--	
		#endregion	
	}	
}
