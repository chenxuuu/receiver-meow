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

--去除字符串开头的空格
local function kickSpace(s)
    if type(s) ~= "string" then return end
    while s:sub(1,1) == " " do
        s = s:sub(2)
    end
    return s
end

--所有需要运行的app
local apps = {
    {--!add
        check = function ()--检查函数，拦截则返回true
            return msg:find("！ *add *.+：.+") == 1 or msg:find("! *add *.+:.+") == 1
        end,
        run = function ()--匹配后进行运行的函数
            local keyWord,answer = msg:match("！ *add *(.+)：(.+)")
            if not keyWord then keyWord,answer = msg:match("! *add *(.+):(.+)") end
            keyWord = kickSpace(keyWord)
            answer = kickSpace(keyWord)
            if not keyWord or not answer or keyWord:len() == 0 or answer:len() == 0 then
                sendMessage(cqCode_At(qq).."格式错误，请检查") return true
            end
            apiXmlInsert(tostring(group or "common"),keyWord,answer)
            sendMessage(cqCode_At(qq).."\r\n[CQ:emoji,id=9989]添加完成！\r\n"..
            "词条："..keyWord.."\r\n"..
            "回答："..answer)
            return true
        end,
        explain = function ()--功能解释，返回为字符串，若无需显示解释，返回nil即可
            return "[CQ:emoji,id=128227] !add关键词:回答"
        end
    },
    {--!del
        check = function ()
            return msg:find("！ *del *.+：.+") == 1 or msg:find("! *del *.+:.+") == 1
        end,
        run = function ()
            local keyWord,answer = msg:match("！ *del *(.+)：(.+)")
            if not keyWord then keyWord,answer = msg:match("! *del *(.+):(.+)") end
            keyWord = kickSpace(keyWord)
            answer = kickSpace(keyWord)
            if not keyWord or not answer or keyWord:len() == 0 or answer:len() == 0 then
                sendMessage(cqCode_At(qq).."格式错误，请检查") return true
            end
            apiXmlRemove(tostring(group or "common"),keyWord,answer)
            sendMessage(cqCode_At(qq).."\r\n[CQ:emoji,id=128465]删除完成！\r\n"..
            "词条："..keyWord.."\r\n"..
            "回答："..answer)
            return true
        end,
        explain = function ()
            return "[CQ:emoji,id=128465] !del关键词:回答"
        end
    },
    {--!list
        check = function ()
            return msg:find("！ *list *.+") == 1 or msg:find("! *list *.+") == 1
        end,
        run = function ()
            local keyWord = msg:match("！ *list *(.+)")
            if not keyWord then keyWord = msg:match("! *list *(.+)") end
            keyWord = kickSpace(keyWord)
            sendMessage(cqCode_At(qq).."\r\n[CQ:emoji,id=128221]当前词条回复如下：\r\n"..
            apiXmlListGet(tostring(group),keyWord).."\r\n"..
            "全局词库内容：\r\n"..apiXmlListGet("common",keyWord))
            return true
        end,
        explain = function ()
            return "[CQ:emoji,id=128221] !list关键词"
        end
    },
    {--!delall
        check = function ()
            return msg:find("！ *delall *.+") == 1 or msg:find("! *delall *.+") == 1
        end,
        run = function ()
            local keyWord = msg:match("！ *delall *(.+)")
            if not keyWord then keyWord = msg:match("! *delall *(.+)") end
            keyWord = kickSpace(keyWord)
            apiXmlDelete(tostring(group or "common"),keyWord)
            sendMessage(cqCode_At(qq).."\r\n[CQ:emoji,id=128465]删除完成！\r\n"..
            "词条："..keyWord)
            return true
        end,
        explain = function ()
            return "[CQ:emoji,id=128465] !delall关键词"
        end
    },
    {--今日运势
        check = function ()
            return msg=="今日运势" or msg=="明日运势" or msg=="昨日运势"
        end,
        run = function ()
            local getAlmanac = require("app.almanac")
            sendMessage(cqCode_At(qq)..getAlmanac(qq))
            return true
        end,
        explain = function ()
            return "[CQ:emoji,id=127881]昨日/今日/明日运势"
        end
    },
    {--查快递
        check = function ()
            return msg:find("查快递") == 1
        end,
        run = function ()
            local express = require("app.express")
            sendMessage(express(qq,msg))
            return true
        end,
        explain = function ()
            return "[CQ:emoji,id=128667]查快递 加 单号"
        end
    },
    {--空气质量
        check = function ()
            return msg:find("空气质量") == 1
        end,
        run = function ()
            local air = require("app.air")
            sendMessage(cqCode_At(qq).."\r\n"..air(msg))
            return true
        end,
        explain = function ()
            return "[CQ:emoji,id=128168]空气质量"
        end
    },
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
    },
    {--点歌
        check = function ()
            return msg:find("点歌") == 1
        end,
        run = function ()
            local qqmusic = require("app.qqmusic")
            sendMessage(qqmusic(msg))
            return true
        end,
        explain = function ()
            return "[CQ:emoji,id=127932]点歌 加 qq音乐id或歌名"
        end
    },
    {--通用回复
    check = function ()
        return true
    end,
    run = function ()
        local replyGroup = apiXmlReplayGet(tostring(group),msg)
        local replyCommon = apiXmlReplayGet("common",msg)
        if replyGroup == "" and replyCommon ~= "" then
            sendMessage(replyCommon)
        elseif replyGroup ~= "" and replyCommon == "" then
            sendMessage(replyGroup)
        else
            sendMessage(math.random(1,10)>=5 and replyCommon or replyGroup)
        end
        return true
    end
},
}

--对外提供的函数接口
return function (inmsg,inqq,ingroup,inid)
    msg,qq,group,id = inmsg,inqq,ingroup,inid
    if msg:lower()=="help" or msg=="帮助" or msg=="菜单" then
        local allApp = {}
        for i=1,#apps do
            local appExplain = apps[i].explain and apps[i].explain()
            if appExplain then
                table.insert(allApp, appExplain)
            end
        end
        sendMessage("[CQ:emoji,id=128172]命令帮助\r\n"..table.concat(allApp, "\r\n"))
        return true
    end

    --遍历所有功能
    for i=1,#apps do
        if apps[i].check and apps[i].check() then
            handled = apps[i].run()
            break
        end
    end
    return handled
end
