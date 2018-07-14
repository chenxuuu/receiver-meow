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
            if (context.Message.IndexOf("宠物绑定") == 0)
            {
                XmlSolve.del("qq_pet_uin", context.FromQq.ToString());
                XmlSolve.del("qq_pet_skey", context.FromQq.ToString());
                string[] str2;
                int count_temp = 0;
                str2 = context.Message.Replace("宠物绑定", "").Split('/');
                foreach (string i in str2)
                {
                    if (count_temp == 0)
                    {
                        XmlSolve.insert("qq_pet_uin", context.FromQq.ToString(), i);
                        count_temp++;
                    }
                    else if (count_temp == 1)
                    {
                        XmlSolve.insert("qq_pet_skey", context.FromQq.ToString(), i);
                        count_temp++;
                    }
                }
                _mahuaApi.SendPrivateMessage(context.FromQq).Text("宠物绑定成功！").Done();
            }
            else
            {
                string replay = MessageSolve.GetReplay(context.FromQq, context.Message, _mahuaApi);
                if (replay != "")
                {
                    _mahuaApi.SendPrivateMessage(context.FromQq, replay);
                }
            }
            

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
