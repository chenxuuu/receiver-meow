import("com.papapoi.ReceiverMeow","Native.Csharp.App.Common")

sys.tiggerRegister("groupMessage",function (data)
    AppData.CQLog:Debug("Lua收到消息",data.msg)
    AppData.CQApi:SendGroupMessage(data.group,"收到消息："..data.msg)
end)

