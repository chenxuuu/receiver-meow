--[[
每分钟会被触发一次的脚本


下面的代码为我当前接待喵逻辑使用的代码，可以重写也可以按自己需求进行更改
详细请参考readme
]]

hour = math.floor(os.time()/3600) % 24
minute = math.floor(os.time()/60) % 60

lowSpace = 10 --剩余空间多少G

function getSpaceOk()
    return apiGetHardDiskFreeSpace("D") > 1024 * lowSpace
end

if minute == 0 and not getSpaceOk() then
    cqSendGroupMessage(567145439,
    cqCode_At(961726194).."你的小垃圾服务器空间只有"..tostring(apiGetHardDiskFreeSpace("D")).."M空间了知道吗？快去清理")
end

if minute == 59 and hour == 2 and getSpaceOk() then
    cqSendGroupMessage(241464054,"一分钟后，将自动进行服务器例行重启与资源世界回档，请注意自己身上的物品")
    apiTcpSend("一分钟后，将自动进行服务器例行重启与资源世界回档，请注意自己身上的物品")
end

if minute == 0 and hour == 3 and getSpaceOk() then
    apiTcpSend("cmdstop")
end
