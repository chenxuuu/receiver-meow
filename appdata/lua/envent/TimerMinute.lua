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

--检查GitHub项目是否有更新
if time.min % 10 == 0 then--十分钟检查一次
    local githubRss = apiHttpGet("https://github.com/chenxuuu/receiver-meow/commits/master.atom")
    if githubRss or githubRss ~= "" then--获取成功的话
        local xml2lua = require("xml2lua")
        --Uses a handler that converts the XML to a Lua table
        local handler = require("xmlhandler.tree")
        local parser = xml2lua.parser(handler)
        parser:parse(githubRss)
        local lastUpdate = handler.root.feed.updated
        if lastUpdate and lastUpdate == apiXmlGet("settings","githubLastUpdate") then
            apiXmlSet("settings","githubLastUpdate",lastUpdate)
            for i,j in pairs(handler.root.feed.entry) do
                --缩短网址
                local shortUrl = apiHttpPost("https://git.io/create","url="..j.link._attr.href:urlEncode())
                shortUrl = (not shortUrl or shortUrl == "") and j.link._attr.href or "https://biu.papapoi.com/"..shortUrl

                --返回结果
                local toSend = "接待喵lua插件在GitHub上有更新啦\r\n"..
                "更新时间："..(lastUpdate):gsub("T"," "):gsub("Z"," ").."\r\n"..
                "提交内容："..j.title.."\r\n"..
                "查看变动代码："..shortUrl
                print(toSend)
                --cqSendGroupMessage(931546484, toSend)
                break
            end
        end
    end
end

