--[[
处理收到的好友添加请求

提前收到的声明数据为：
fromqq  请求的qq号码 number类型
message 附加信息     string类型
tag     反馈标志     string类型

注意：拦截消息后请将变量handled置true，表示消息已被拦截，如：
handled = true

下面的代码为我当前接待喵逻辑使用的代码，可以重写也可以按自己需求进行更改
详细请参考readme
]]

--cqSetFriendAddRequest(tag,1,"") --同意请求
--cqSetFriendAddRequest(tag,2,"不加好友哦") --拒绝请求

