--[[
处理收到的tcp消息

提前收到的声明数据为：
message 消息内容     string类型

下面的代码为我当前接待喵逻辑使用的代码，可以重写也可以按自己需求进行更改
详细请参考readme
]]

--添加在线的人
local function onlineAdd(p)
    local onlineData = apiXmlGet("minecraftData","[online]")
    local online = {}--存储在线所有人id
    if onlineData ~= "" then
        online = onlineData:split(",")
    end
    table.insert(online,p)
    apiXmlSet("minecraftData","[online]",table.concat(online,","))
end

--删除在线的人
local function onlineDel(p)
    local onlineData = apiXmlGet("minecraftData","[online]")
    local online = {}--存储在线所有人id
    if onlineData ~= "" then
        online = onlineData:split(",")
    end
    local onlineResult = {}
    while #online > 0 do
        local player = table.remove(online,1)
        if player ~= p then
            table.insert(onlineResult,player)
        end
    end
    apiXmlSet("minecraftData","[online]",table.concat(onlineResult,","))
end

local messageType = message:sub(1,1)

local solve = {
    l = function (msg)
        onlineAdd(msg)
        cqSendGroupMessage(241464054,msg.."上线了")
    end,
    d = function (msg)
        onlineDel(msg)
        cqSendGroupMessage(241464054,msg.."掉线了")
    end,
    m = function (msg)
        if msg:find("%[主世界%]") or
           msg:find("%[旧世界%]") or
           msg:find("%[创造界%]") or
           msg:find("%[下界%]") or
           msg:find("%[末地%]") then
            cqSendGroupMessage(241464054,cqCqCode_Trope(msg))
        end
    end,
    c = function (msg)
        cqSendGroupMessage(241464054,"服务器已启动完成")
        apiXmlSet("minecraftData","[online]","")
    end,
}

if solve[messageType] then
    solve[messageType](message:sub(2))
end
