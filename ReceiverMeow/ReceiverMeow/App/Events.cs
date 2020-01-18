using Native.Csharp.Sdk.Cqp.EventArgs;
using Native.Csharp.Sdk.Cqp.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Native.Csharp.App
{
    class EventGroupMessage : IGroupMessage
    {
        public void GroupMessage(object sender, CQGroupMessageEventArgs e)
        {
            if(e.SubType == Sdk.Cqp.Enum.CQGroupMessageType.Group)
            {
                LuaEnv.LuaStates.Run(e.FromGroup.Id.ToString(), "groupMessage", new
                {
                    group = e.FromGroup.Id,
                    qq = e.FromQQ.Id,
                    msg = e.Message.Text,
                });
            }
        }
    }
}
