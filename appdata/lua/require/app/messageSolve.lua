--统一的消息处理函数
msg,qq,group,id = nil,nil,nil,nil
local handled = false

--发送消息
--自动判断群聊与私聊
function sendMessage(s)
    if group then
        if cqSendGroupMessage(group,s) == -34 then
            --在群内被禁言了，打上标记
            apiXmlSet("ban",tostring(group),tostring(os.time()))
        end
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
            return (msg:find("！ *add *.+：.+") == 1 or msg:find("! *add *.+:.+") == 1)
			and not (msg:find("！ *addadmin *.+") == 1 or msg:find("! *addadmin *.+") == 1)
        end,
        run = function ()--匹配后进行运行的函数
            if (apiXmlGet("adminList",tostring(qq)) ~= "admin" or not group) and qq ~= admin then
                sendMessage(cqCode_At(qq).."你不是狗管理，想成为狗管理请找我的主人呢")
                return true
            end
            local keyWord,answer = msg:match("！ *add *(.+)：(.+)")
            if not keyWord then keyWord,answer = msg:match("! *add *(.+):(.+)") end
            keyWord = kickSpace(keyWord)
            answer = kickSpace(answer)
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
            return (msg:find("！ *del *.+：.+") == 1 or msg:find("! *del *.+:.+") == 1)
			and not (msg:find("！ *deladmin *.+") == 1 or msg:find("! *deladmin *.+") == 1)
        end,
        run = function ()
            if (apiXmlGet("adminList",tostring(qq)) ~= "admin" or not group) and qq ~= admin then
                sendMessage(cqCode_At(qq).."你不是狗管理，想成为狗管理请找我的主人呢")
                return true
            end
            local keyWord,answer = msg:match("！ *del *(.+)：(.+)")
            if not keyWord then keyWord,answer = msg:match("! *del *(.+):(.+)") end
            keyWord = kickSpace(keyWord)
            answer = kickSpace(answer)
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
            if (apiXmlGet("adminList",tostring(qq)) ~= "admin" or not group) and qq ~= admin then
                sendMessage(cqCode_At(qq).."你不是狗管理，想成为狗管理请找我的主人呢")
                return true
            end
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
    {--查动画
        check = function ()
            return msg:find("查动画") or msg:find("搜动画") or msg:find("查番") or msg:find("搜番")
        end,
        run = function ()
            local animeSearch = require("app.animeSearch")
            sendMessage(cqCode_At(qq).."\r\n"..animeSearch(msg))
            return true
        end,
        explain = function ()
            return "[CQ:emoji,id=128444]查动画 加 没裁剪过的视频截图"
        end
    },
    {--象棋
        check = function ()
            return msg:find("象棋") == 1
        end,
        run = function ()
            local chess = require("app.chess")
            sendMessage(chess(qq,msg))
            return true
        end,
        explain = function ()
            return "[CQ:emoji,id=128195]象棋 查看象棋功能帮助"
        end
    },
    {--抽奖
        check = function ()
            return msg == "抽奖" or msg:find("禁言") == 1
        end,
        run = function ()
            local banPlay = require("app.banPlay")
            sendMessage(banPlay(msg,qq,group))
            return true
        end,
        explain = function ()
            return "[CQ:emoji,id=128142]抽奖/禁言卡"
        end
    },
    {--签到
        check = function ()
            return msg == "签到" or msg:find("%[CQ:sign,") == 1
        end,
        run = function ()
            local sign = require("app.sign")
            sendMessage(cqCode_At(qq)..sign(qq))
            return true
        end,
        explain = function ()
            return "[CQ:emoji,id=9728]签到"
        end
    },
    {--b站av号解析
        check = function ()
            return msg:find("av%d+")
        end,
        run = function ()
            local av = require("app.av")
            sendMessage(av(msg))
            return true
        end,
        explain = function ()
            return "[CQ:emoji,id=127902]b站av号解析"
        end
    },
    {--一言
        check = function ()
            return msg == "一言"
        end,
        run = function ()
            local hitokoto = require("app.hitokoto")
            sendMessage(hitokoto())
            return true
        end,
        explain = function ()
            return "[CQ:emoji,id=128226]一言"
        end
    },
    {--必应美图
        check = function ()
            return msg:find("必应") == 1 and (message:find("美图") or message:find("壁纸"))
        end,
        run = function ()
            local bing = require("app.bing")
            sendMessage(bing())
            return true
        end,
        explain = function ()
            return "[CQ:emoji,id=127964]必应壁纸"
        end
    },
    {--测试代码
        check = function ()
            return msg:find("#lua") == 1
        end,
        run = function ()
            if qq == admin then
                local result, info = pcall(function ()
                    print = function (s)
                        sendMessage(tostring(s))
                    end
                    load(cqCqCode_UnTrope(msg:sub(5)))()
                end)
                if result then
                    sendMessage(cqCode_At(qq).."成功运行")
                else
                    sendMessage(cqCode_At(qq).."运行失败\r\n"..info)
                end
            else
                sendMessage(cqCode_At(qq).."\r\n"..apiSandBox(cqCqCode_UnTrope(msg:sub(5))))
            end
            return true
        end,
        explain = function ()
            return "[CQ:emoji,id=9000]#lua运行lua代码"
        end
    },
    {--!addadmin
        check = function ()
            return (msg:find("！ *addadmin *.+") == 1 or msg:find("! *addadmin *.+") == 1) and
             qq == admin
        end,
        run = function ()
            local keyWord = msg:match("(%d+)")
            if keyWord and apiXmlGet("adminList",keyWord) == "admin" then
                sendMessage(cqCode_At(qq).."\r\n"..keyWord.."已经是狗管理了")
            else
                apiXmlSet("adminList",keyWord,"admin")
                sendMessage(cqCode_At(qq).."\r\n已添加狗管理"..keyWord)
            end
            return true
        end,
    },
    {--!deladmin
        check = function ()
            return (msg:find("！ *deladmin *.+") == 1 or msg:find("! *deladmin *.+") == 1) and
             qq == admin
        end,
        run = function ()
            local keyWord = msg:match("(%d+)")
            if keyWord and apiXmlGet("adminList",keyWord) == "" then
                sendMessage(cqCode_At(qq).."\r\n狗管理"..keyWord.."已经挂了")
            else
                apiXmlDelete("adminList",keyWord)
                sendMessage(cqCode_At(qq).."\r\n已宰掉狗管理"..keyWord)
            end
            return true
        end,
    },
    {--@触发图灵机器人
        check = function ()
            return msg:find("%[CQ:at,qq="..cqGetLoginQQ().."%]") and msg:gsub("%[CQ:.-%]",""):len() > 2
        end,
        run = function ()
            local requestData =
            {
                reqType = 0,
                perception = {
                    inputText = {
                        text = msg:gsub("%[CQ:.-%]",""):gsub(" ","")
                    }
                },
                userInfo = {
                    apiKey = apiXmlGet("settings","tuling"),--请自己申请接口，在setting.xml里设置
                    userId = tostring(qq),
                    groupId = tostring(group),
                    userIdName = tostring(qq),
                },
            }
            requestData = jsonEncode(requestData)
            cqAddLoger(0, "lua图灵接口", requestData)
            local rr = apiHttpPost("http://openapi.tuling123.com/openapi/api/v2",
                        requestData,nil,nil,"application/json")
            cqAddLoger(0, "lua图灵接口", rr)
            if not rr or rr == "" then return end--没获取到数据
            local d,r,e = jsonDecode(rr)
            if not r then cqAddLoger(30, "lua图灵接口", e) return end
            if d and d.intent and d.intent.code and d.intent.code > 9000 and d.results then
                for i=1,#d.results do
                    cqAddLoger(0, "lua图灵接口", d.results[i].resultType)
                    if d.results[i].resultType == "text" then
                        cqAddLoger(0, "lua图灵接口", d.results[i].values.text)
                        sendMessage(cqCode_At(qq)..d.results[i].values.text)
                        return true
                    end
                end
            end
            cqAddLoger(30, "lua图灵接口", rr)
        end,
    },
    {--通用回复
        check = function ()
            return true
        end,
        run = function ()
            if qq ~= 1000000 and (group == 241464054 or group == 567145439) and--mc群消息处理
                require("app.minecraftGroup")(msg,qq,group) then
                return true
            end
            local replyGroup = group and apiXmlReplayGet(tostring(group),msg) or ""
            local replyCommon = apiXmlReplayGet("common",msg)
            if replyGroup == "" and replyCommon ~= "" then
                sendMessage(replyCommon)
            elseif replyGroup ~= "" and replyCommon == "" then
                sendMessage(replyGroup)
            elseif replyGroup ~= "" and replyCommon ~= "" then
                sendMessage(math.random(1,10)>=5 and replyCommon or replyGroup)
            else
                return false
            end
            return true
        end
    },
}

--对外提供的函数接口
return function (inmsg,inqq,ingroup,inid)
    --禁言锁，最长持续一个月
    if (tonumber(apiXmlGet("ban",tostring(ingroup))) or 0) > os.time() - 3600 * 24 * 30 then
        if inmsg:find("%("..tostring(cqGetLoginQQ()).."%) 被管理员解除禁言") then
            apiXmlDelete("ban",tostring(ingroup))
        elseif ingroup then
            return false
        end
    elseif inmsg:find("%("..tostring(cqGetLoginQQ()).."%) 被管理员禁言") then
        apiXmlSet("ban",tostring(group),tostring(os.time()))
    end

    msg,qq,group,id = inmsg,inqq,ingroup,inid
    --帮助列表每页最多显示数量
    local maxEachPage = 8
    --匹配是否需要获取帮助
    if msg:lower():find("help *%d*") == 1 or msg:find("帮助 *%d*") == 1 or msg:find("菜单 *%d*") == 1 then
        local page = msg:lower():match("help *(%d+)") or msg:match("帮助 *(%d+)") or
                     msg:find("菜单 *(%d+)") or 1
        page = tonumber(page)--获取页码
        local maxPage = math.ceil(#apps/maxEachPage)
        page = page > maxPage and maxPage or page

        --开始与结束序号
        local fromApp = (page - 1) * maxEachPage + 1
        local endApp = fromApp + maxEachPage - 1
        endApp = endApp > #apps and #apps or endApp

        local allApp = {}
        for i=fromApp,endApp do
            local appExplain = apps[i].explain and apps[i].explain()
            if appExplain then
                table.insert(allApp, appExplain)
            end
        end
        sendMessage("[CQ:emoji,id=128172]命令帮助("..tostring(page).."/"..tostring(maxPage).."页)\r\n"..
        table.concat(allApp, "\r\n").."\r\n"..
        "[CQ:emoji,id=128483]开源代码：https://github.com/chenxuuu/receiver-meow")
        return true
    end

    --遍历所有功能
    for i=1,#apps do
        if apps[i].check and apps[i].check() then
            if apps[i].run() then
                handled = true
                break
            end
        end
    end
    return handled
end
