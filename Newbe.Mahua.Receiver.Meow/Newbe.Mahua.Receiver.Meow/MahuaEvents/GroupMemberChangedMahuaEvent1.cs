using Newbe.Mahua.MahuaEvents;
using System;

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
            
        }
    }
}
