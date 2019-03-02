--[[
处理收到的群请求 (申请入群) 事件

提前收到的声明数据为：
fromqq      消息的qq号码     number类型
fromgroup   消息的群号码     number类型
message     请求加群附加消息 string类型

注意：拦截消息后请将变量handled置true，表示消息已被拦截，如：
handled = true

下面的代码为我当前接待喵逻辑使用的代码，可以重写也可以按自己需求进行更改
详细请参考readme
]]

--cqSetGroupAddRequest(fromgroup,1,1,"")        --同意加群
--cqSetGroupAddRequest(fromgroup,1,2,"不加新人") --拒绝加群

