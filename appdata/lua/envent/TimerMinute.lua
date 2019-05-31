--[[
每分钟会被触发一次的脚本


下面的代码为我当前接待喵逻辑使用的代码，可以重写也可以按自己需求进行更改
详细请参考readme
]]

if cqGetLoginQQ() ~= 751323264 then return end--仅限官方群里的机器人号用这个功能

local time = os.date("*t")

lowSpace = 10 --剩余空间多少G

function getSpaceOk()
    return apiGetHardDiskFreeSpace("D") > 1024 * lowSpace
end

if time.min == 0 and not getSpaceOk() then
    cqSendGroupMessage(567145439,
    cqCode_At(961726194).."你的小垃圾服务器空间只有"..tostring(apiGetHardDiskFreeSpace("D")).."M空间了知道吗？快去清理")
end

if time.min == 59 and time.hour == 2 and getSpaceOk() then
    cqSendGroupMessage(241464054,"一分钟后，将自动进行服务器例行重启与资源世界回档，请注意自己身上的物品")
    apiTcpSend("一分钟后，将自动进行服务器例行重启与资源世界回档，请注意自己身上的物品")
end

if time.min == 0 and time.hour == 3 and getSpaceOk() then
    apiTcpSend("cmdstop")
end

if time.min == 0 and time.hour == 5 then
    apiTcpSend("cmdworld create mine")
end


function checkGitHub(url,save)
    local githubRss = apiHttpGet(url)
    if githubRss or githubRss ~= "" then--获取成功的话
        local xml2lua = loadfile(apiGetPath().."/data/app/com.papapoi.ReceiverMeow/lua/require/xml2lua.lua")()
        --Uses a handler that converts the XML to a Lua table
        local handler = loadfile(apiGetPath().."/data/app/com.papapoi.ReceiverMeow/lua/require/xmlhandler/tree.lua")()
        local parser = xml2lua.parser(handler)
        parser:parse(githubRss)
        local lastUpdate = handler.root.feed.updated
        if lastUpdate and lastUpdate ~= apiXmlGet("settings",save) then
            apiXmlSet("settings",save,lastUpdate)
            for i,j in pairs(handler.root.feed.entry) do
                --缩短网址
                local shortUrl = apiHttpPost("https://git.io/create","url="..j.link._attr.href:urlEncode())
                shortUrl = (not shortUrl or shortUrl == "") and j.link._attr.href or "https://biu.papapoi.com/"..shortUrl

                --返回结果
                local toSend = "更新时间(UTC)："..(lastUpdate):gsub("T"," "):gsub("Z"," ").."\r\n"..
                "提交内容："..j.title.."\r\n"..
                "查看变动代码："..shortUrl
                return true,toSend
            end
        end
    end
end

function checkGitRelease(url,save)
    local release = apiHttpGet(url)
    local d,r,e = jsonDecode(release)
    if not r or not d then return end
    if d.id and tostring(d.id) ~= apiXmlGet("settings",save) then
        apiXmlSet("settings",save,tostring(d.id))
        --缩短网址
        local shortUrl = apiHttpPost("https://git.io/create","url="..d.html_url:urlEncode())
        shortUrl = (not shortUrl or shortUrl == "") and d.html_url or "https://biu.papapoi.com/"..shortUrl

        --返回结果
        local toSend = "更新时间(UTC)："..(d.created_at):gsub("T"," "):gsub("Z"," ").."\r\n"..
        "版本："..d.tag_name.."\r\n"..
        d.name.."\r\n"..
        d.body.."\r\n"..
        "查看更新："..shortUrl
        return true,toSend
    end
end

--检查GitHub项目是否有更新
if time.min % 10 == 0 then--十分钟检查一次
    local r,t = checkGitHub("https://github.com/chenxuuu/receiver-meow/commits/master.atom","githubLastUpdate")
    if r and t then
        local text = "接待喵lua插件在GitHub上有更新啦\r\n"..t
        cqSendGroupMessage(567145439, text)
    end

    r,t = checkGitRelease("https://api.github.com/repos/chenxuuu/receiver-meow/releases/latest","githubRelease")
    if r and t then
        local text = "接待喵lua插件发现插件版本更新\r\n"..t
        cqSendGroupMessage(931546484, text)
    end
end


if time.min % 5 == 0 then--5分钟检查一次
    --臭dd检查youtube是否开播
    function v2b(channel)
        local html = apiHttpGet("https://y2b.wvvwvw.com/channel/"..channel.."/featured")
        if not html or html == "" then return end--获取失败了
        --local isclose = html:find("Upcoming live streams")
        local isopen = html:find("LIVE NOW\"")
        --if not isopen and not isclose then return end --啥都没匹配到
        local lastStatus = apiXmlGet("settings","youtuber_"..channel)--获取上次状态
        if isopen then
            if lastStatus == "live" then return end--上次提醒过了
            local title,description,name,url = html:match([[,"simpleText":"(.-)"},"descriptionSnippet":{"simpleText":"(.-)"},"longBylineText":{"runs":%[{"text":"(.-)","navigationEndpoint":{"click.-"watchEndpoint":{"videoId":"(.-)"}}.-LIVE NOW"]])
            apiXmlSet("settings","youtuber_"..channel,"live")
            return {
                name = name or "获取失败",
                title = title or "获取失败",
                description = description and description:gsub("\\n","\n") or "获取失败",
                url = url and "https://www.youtube.com/watch?v="..url or "获取失败",
            }
        elseif lastStatus == "live" then--没开播
            apiXmlSet("settings","youtuber_"..channel,"close live")
        end
    end

    function checkdd(channel)
        local v = v2b(channel)
        if v then
            cqSendGroupMessage(261037783,
            "频道："..v.name.."\r\n"..
            "标题："..v.title.."\r\n"..
            "简介："..v.description.."\r\n"..
            "前往查看："..v.url)
        end
        cqAddLoger(0, "直播检查", channel .. (v and "状态更新" or "状态不变"))
    end
    local ddList = {
    --要监控的y2b频道
    "UCWCc8tO-uUl_7SJXIKJACMw",
    "UCQ0UDLQCjY0rmuxCDE38FGg",
    "UC1opHUrw8rvnsadT-iGp7Cg",
    "UCrhx4PaF3uIo9mDcTxHnmIg",
    "UChN7P9OhRltW3w9IesC92PA",
    "UC8NZiqKx6fsDT3AVcMiVFyA",
    "UCH0ObmokE-zUOeihkKwWySA",
    "UCIaC5td9nGG6JeKllWLwFLA",
    "UCvEX2UICvFAa_T6pqizC20g",
    }

    for i=1,#ddList do
        checkdd(ddList[i])
    end
end

if time.min % 5 == 0 then--5分钟检查一次
    function blive(id)
        id = tostring(id)
        html = apiHttpGet("https://api.live.bilibili.com/room/v1/Room/get_info?room_id="..id)
        if not html or html == "" then return end--获取失败了
        local d,r,e = jsonDecode(html)
        if not r or not d then return end --获取失败了
        local lastStatus = apiXmlGet("settings","bilibili_live_"..id)--获取上次状态
        if d.data.live_status == 1 then
            if lastStatus == "live" then return end--上次提醒过了
            apiXmlSet("settings","bilibili_live_"..id,"live")
            return {
                title = d.data.title,
                image = d.data.user_cover,
                tag = d.data.tags,
                url = "https://live.bilibili.com/"..id,
            }
        elseif lastStatus == "live" then--没开播
            apiXmlSet("settings","bilibili_live_"..id,"close live")
        end
    end

    function checkb(id)
        local v = blive(id)
        if v then
            cqSendGroupMessage(261037783,
            image(v.image).."\r\n"..
            "标题："..v.title.."\r\n"..
            "tag："..v.tag.."\r\n"..
            "前往查看："..v.url)
        end
        cqAddLoger(0, "直播检查", tostring(id) .. (v and "状态更新" or "状态不变"))
    end

    local bList = {
        --要监控的bilibili频道
        14917277,
        4895312,
        7962050,
        13946381,
        10545,
        92613,
        291,
        12235923,
    }

    for i=1,#bList do
        checkb(bList[i])
    end
end
