--统一的消息处理函数
local msg,qq,group,id







function run()
    if not group then
        cqSendPrivateMessage(qq,"你发送了"..msg)
    end
    if qq == 961726194 and group then
        cqSendGroupMessage(group,"lua测试消息："..msg)
    end
end






return function (inmsg,inqq,ingroup,inid)
    msg,qq,group,id = inmsg,inqq,ingroup,inid
    run()
end
