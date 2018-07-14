using Newbe.Mahua.MahuaEvents;
using System;
using Newbe.Mahua.Receiver.Meow.MahuaApis;

namespace Newbe.Mahua.Receiver.Meow.MahuaEvents
{
    /// <summary>
    /// 群文件上传事件
    /// </summary>
    public class GroupUploadedMahuaEvent1
        : IGroupUploadedMahuaEvent
    {
        private readonly IMahuaApi _mahuaApi;

        public GroupUploadedMahuaEvent1(
            IMahuaApi mahuaApi)
        {
            _mahuaApi = mahuaApi;
        }

        public void ProcessGroupUploaded(GroupUploadedContext context)
        {
            _mahuaApi.SendGroupMessage(context.FromGroup,
                "群员" + Tools.At(context.FromQq) + "上传了一个文件，大家快来看看这是什么吧！");
        }
    }
}
