--基岩服务器专用，其他群不用管这个
print((function ()
    if fromgroup~= "543632048" or message:find("命令") ~= 1 then return "" end
    if message:find("命令绑定") == 1 then
        local id = message:gsub("命令绑定 *","")
        setData(fromqq,"minecraft bedrock",id)
        print(at(fromqq))
        return "绑定"..id.."完成"
    elseif getData(fromqq,"minecraft bedrock") == "" then
        print(at(fromqq))
        return "你还没有绑定id，发送“命令绑定id”来绑定你的id"
    elseif message == "命令" then
        print(at(fromqq))
        return "当前支持的命令：\r\n"..
        "命令回家 回到上次睡觉的地点\r\n"..
        "命令传送 加 玩家id 传送到目标玩家地点"
    elseif message == "命令回家" then
        httpGet("http://127.0.0.1:666/"..("kill "..getData(fromqq,"minecraft bedrock")):urlEncode())
        print(at(fromqq))
        return "已执行回到上次睡觉地点操作"
    elseif message:find("命令传送") == 1 then
        local id = message:gsub("命令传送 *","")
        httpGet("http://127.0.0.1:666/"..("tp "..getData(fromqq,"minecraft bedrock").." "..id):urlEncode())
        print(at(fromqq))
        return "已执行传送到玩家"..id.."处操作"
    end
end)())
