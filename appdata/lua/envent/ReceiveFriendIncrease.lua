--[[
处理好友已经添加事件

提前收到的声明数据为：
fromqq  请求的qq号码 number类型

注意：拦截消息后请将变量handled置true，表示消息已被拦截，如：
handled = true

下面的代码为我当前接待喵逻辑使用的代码，可以重写也可以按自己需求进行更改
详细请参考readme
]]

--cqSendPrivateMessage(fromqq,"我们已经是好友啦，快给我打钱")
