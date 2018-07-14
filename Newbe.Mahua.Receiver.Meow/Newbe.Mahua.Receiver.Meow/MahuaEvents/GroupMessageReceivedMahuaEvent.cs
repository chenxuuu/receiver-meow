using Newbe.Mahua.MahuaEvents;
using System;
using Newbe.Mahua.Receiver.Meow.MahuaApis;

namespace Newbe.Mahua.Receiver.Meow.MahuaEvents
{
    /// <summary>
    /// 群消息接收事件
    /// </summary>
    public class GroupMessageReceivedMahuaEvent
        : IGroupMessageReceivedMahuaEvent
    {
        private readonly IMahuaApi _mahuaApi;

        public GroupMessageReceivedMahuaEvent(
            IMahuaApi mahuaApi)
        {
            _mahuaApi = mahuaApi;
        }

        public void ProcessGroupMessage(GroupMessageReceivedContext context)
        {
            if (context.FromQq == "80000000" || context.FromQq == "1811436283")
                return;
            string replay = MessageSolve.GetReplay(context.FromQq, context.Message, _mahuaApi, context.FromGroup);
            if (replay != "")
            {
                _mahuaApi.SendGroupMessage(context.FromGroup, replay);
            }
        }
    }
}
