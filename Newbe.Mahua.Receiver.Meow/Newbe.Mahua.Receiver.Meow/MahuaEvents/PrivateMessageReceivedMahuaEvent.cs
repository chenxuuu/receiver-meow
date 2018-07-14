using Newbe.Mahua.MahuaEvents;
using System;
using System.Threading.Tasks;
using Newbe.Mahua.Receiver.Meow.MahuaApis;

namespace Newbe.Mahua.Receiver.Meow.MahuaEvents
{
    /// <summary>
    /// 私聊消息接收事件
    /// </summary>
    public class PrivateMessageReceivedMahuaEvent
        : IPrivateMessageReceivedMahuaEvent
    {
        private readonly IMahuaApi _mahuaApi;

        public PrivateMessageReceivedMahuaEvent(
            IMahuaApi mahuaApi)
        {
            _mahuaApi = mahuaApi;
        }

        public void ProcessPrivateMessage(PrivateMessageReceivedContext context)
        {
            // 戳一戳
            _mahuaApi.SendPrivateMessage(context.FromQq)
                .Shake()
                .Done();

            // 换行，重复消息
            _mahuaApi.SendPrivateMessage(context.FromQq)
                .Text(XmlSolve.ReplayGroupStatic(2333, context.Message))
                .Done();

            

            // 异步发送消息，不能使用 _mahuaApi 实例，需要另外开启Session
            //Task.Factory.StartNew(() =>
            //{
            //    using (var robotSession = MahuaRobotManager.Instance.CreateSession())
            //    {
            //        var api = robotSession.MahuaApi;
            //        api.SendPrivateMessage(context.FromQq, "异步的嘤嘤嘤");
            //    }
            //});
        }
    }
}
