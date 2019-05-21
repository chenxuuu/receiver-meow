--[[
处理收到的群成员减少事件

提前收到的声明数据为：
fromqq      被操作qq号码    number类型
fromgroup   消息的群号码    number类型
doqq        操作者qq号码    number类型  如果为自己退群，此值为nil

注意：拦截消息后请将变量handled置true，表示消息已被拦截，如：
handled = true

下面的代码为我当前接待喵逻辑使用的代码，可以重写也可以按自己需求进行更改
详细请参考readme
]]

cqSendGroupMessage(fromgroup,tostring(fromqq).."永远地离开了这个世界。。")
if fromgroup == 241464054 then
    local player = apiXmlGet("bindQq",tostring(fromqq))
    if player ~= "" then
        apiTcpSend("lp user "..player.." permission set group.default",true)
        apiTcpSend("lp user "..player.." permission unset group.whitelist",true)
    end
    apiXmlDelete("bindStep",tostring(fromqq))
    apiXmlDelete("bindQq",tostring(fromqq))
end
