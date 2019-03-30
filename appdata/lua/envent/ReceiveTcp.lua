--[[
处理收到的tcp消息

提前收到的声明数据为：
message 消息内容     string类型

下面的代码为我当前接待喵逻辑使用的代码，可以重写也可以按自己需求进行更改
详细请参考readme
]]

local messageType = message:sub(1,1)

local solve = {
    l = function (msg)
        cqSendGroupMessage(241464054,msg.."上线了")
    end,
    d = function (msg)
        cqSendGroupMessage(241464054,msg.."掉线了")
    end,
    m = function (msg)
        cqSendGroupMessage(241464054,cqCqCode_Trope(msg))
    end,
}

if solve[messageType] then
    solve[messageType](message:sub(2))
end
