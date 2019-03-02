--[[
处理收到的群文件上传结果

提前收到的声明数据为：
fromqq      消息的qq号码    number类型
fromgroup   消息的群号码    number类型
fileName    文件名          string类型
id          文件id          number类型
size        文件大小        number类型

注意：拦截消息后请将变量handled置true，表示消息已被拦截，如：
handled = true

下面的代码为我当前接待喵逻辑使用的代码，可以重写也可以按自己需求进行更改
详细请参考readme
]]


