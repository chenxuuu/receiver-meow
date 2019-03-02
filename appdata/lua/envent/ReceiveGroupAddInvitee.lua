--[[
处理收到的群请求 (机器人被邀入群) 事件

提前收到的声明数据为：
fromqq      消息的qq号码     number类型
fromgroup   消息的群号码     number类型

注意：拦截消息后请将变量handled置true，表示消息已被拦截，如：
handled = true

下面的代码为我当前接待喵逻辑使用的代码，可以重写也可以按自己需求进行更改
详细请参考readme
]]

--cqSetGroupAddRequest(fromgroup,2,1,"")        --同意邀请
--cqSetGroupAddRequest(fromgroup,2,2,"不加新群") --拒绝邀请
