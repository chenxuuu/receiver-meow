--统一的消息处理函数
local msg,qq,group,id
local handled = false

--发送消息
--自动判断群聊与私聊
local function sendMessage(s)
    if group then
        cqSendGroupMessage(group,s)
    else
        cqSendPrivateMessage(qq,s)
    end
end


--所有需要运行的app
local apps = {
    {--点赞
        check = function ()--检查函数，拦截则返回true
            return msg=="点赞" or msg=="赞我"
        end,
        run = function ()--匹配后进行运行的函数
            cqSendPraise(qq,1)
            sendMessage(cqCode_At(qq).."已为你点赞[CQ:emoji,id=128077]")
            return true
        end,
        explain = function ()--功能解释，返回为字符串，若无需显示解释，返回nil即可
            return "[CQ:emoji,id=128077]点赞"
        end
    }
}











return function (inmsg,inqq,ingroup,inid)
    msg,qq,group,id = inmsg,inqq,ingroup,inid
    if msg:lower()=="help" or msg=="帮助" then
        local allApp = {}
        for i=1,#apps do
            local appExplain = apps[i].explain()
            if appExplain then
                table.insert(allApp, appExplain)
            end
        end
        sendMessage("[CQ:emoji,id=128172]命令帮助\r\n"..table.concat(allApp, "\r\n"))
        return true
    end
    --遍历所有功能
    for i=1,#apps do
        if apps[i].check() then
            handled = apps[i].run()
            break
        end
    end
    return handled
end
