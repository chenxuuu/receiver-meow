using Newbe.Mahua.MahuaEvents;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Newbe.Mahua.Receiver.Meow.MahuaApis;
using System.Xml.Linq;

namespace Newbe.Mahua.Receiver.Meow.MahuaEvents
{
    /// <summary>
    /// 插件初始化事件
    /// </summary>
    public class InitializationMahuaEvent
        : IInitializationMahuaEvent
    {
        private readonly IMahuaApi _mahuaApi;

        public InitializationMahuaEvent(
            IMahuaApi mahuaApi)
        {
            _mahuaApi = mahuaApi;
        }

        public void Initialized(InitializedContext context)
        {

        }
    }
}
